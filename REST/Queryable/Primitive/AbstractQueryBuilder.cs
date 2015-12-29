using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gale.REST.Queryable.OData.Builders;
using Gale.REST.Queryable.Primitive.Reflected;

namespace Gale.REST.Queryable.Primitive
{
    public abstract class AbstractQueryBuilder
    {
        #region Variables

        //------[ VARIABLES
        private Model _reflectedModel = null;
        private List<TypedItem> _parsers = new List<TypedItem>();
        private SortedList<String, Type> _operators = new SortedList<String, Type>();
        private Gale.Db.IDataActions _databaseFactory;

        //------[ EVENT
        public delegate void ExecutedParserEventHandler(Gale.REST.Queryable.Primitive.ExecutedParserEventArgs e);
        public event ExecutedParserEventHandler OnExecutedParser;
        //---------------------------------------------------------

        #endregion

        /// <summary>
        /// Reflect Types And Properties for the T model
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Model ReflectModel(Type Model)
        {
            //-- FIRST BUILD THE CONSTRAINT'S
            var propertyConstraints = Model.GetProperties().Where((prop) =>
            {
                return Attribute.IsDefined(prop, typeof(System.Data.Linq.Mapping.AssociationAttribute)) &&
                       (prop.PropertyType.IsGenericType == false ||
                       (prop.PropertyType.IsGenericType == true &&
                        prop.PropertyType.GetGenericTypeDefinition() != typeof(System.Data.Linq.EntitySet<>)));
            }).ToList();

            //-- REFLECT TYPES AND COMPONENTS
            List<Field> fields = new List<Field>();
            List<Table> tables = new List<Table>() { new Table(Model) };
            List<DummyConstraint> dummyConstraints = new List<DummyConstraint>();

            //Start for the constraint's (dummy Creating)
            propertyConstraints.ForEach((property) =>
            {
                var attr = property.TryGetAttribute<System.Data.Linq.Mapping.AssociationAttribute>();
                if (attr.IsForeignKey == true)
                {
                    String thisName = attr.ThisKey.ToLower();
                    String otherName = attr.OtherKey.ToLower();

                    Table table = new Table(property.PropertyType, thisName);
                    tables.Add(table);
                    dummyConstraints.Add(new DummyConstraint(thisName, otherName, table));
                }
            });

            //Extract Fields From Each Table
            tables.ForEach((table) =>
            {
                var fieldProperties = table.Type.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.Data.Linq.Mapping.ColumnAttribute))).ToList();
                foreach (System.Reflection.PropertyInfo property in fieldProperties)
                {
                    var attr = property.TryGetAttribute<System.Data.Linq.Mapping.ColumnAttribute>();

                    bool hasForeign = dummyConstraints.Exists((constraint) =>
                    {
                        return constraint.thisName == property.Name.ToLower();
                    });

                    fields.Add(new Field(
                        property,
                        attr,
                        table,
                        (hasForeign ? Field.SpecificationEnum.Fk : Field.SpecificationEnum.None)
                    ));
                }

                if (table.PrimaryKey == null)
                {
                    throw new Gale.Exception.GaleException("API019", table.Name);
                }
            });

            //Create Real Constraint's
            List<Constraint> constraints = new List<Constraint>();
            dummyConstraints.ForEach((dummyConstraint) =>
            {
                Table table = dummyConstraint.table;
                Field thisField = fields.FirstOrDefault(field => field.Name == dummyConstraint.thisName);

                String otherFieldName = String.Format("{0}:({1})", dummyConstraint.table.Prefix, dummyConstraint.otherName);
                Field otherField = fields.FirstOrDefault(field => field.Name == otherFieldName);

                constraints.Add(new Constraint(thisField, otherField, table));
            });

            return new Model(fields, constraints, tables);
        }


        #region PROPERTIES AND CONSTRUCTOR'S
        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="databaseFactory"></param>
        public AbstractQueryBuilder(Gale.Db.IDataActions databaseFactory, Type TModel)
        {
            //Setting the Database Factory Type
            this._databaseFactory = databaseFactory;

            //Entity Type for the CRUD
            Type entityType = TModel;

            //Field List
            this._reflectedModel = ReflectModel(entityType);
        }

        internal Model ReflectedModel()
        {
            return _reflectedModel;
        }

        private Gale.Db.IDataActions DatabaseFactory
        {
            get
            {
                return _databaseFactory;
            }
        }

        [System.ComponentModel.ReadOnly(true)]
        internal List<TypedItem> RegisteredParsers()
        {
            return _parsers;
        }

        [System.ComponentModel.ReadOnly(true)]
        internal SortedList<String, Type> RegisteredOperators()
        {
            return _operators;

        }

        #endregion

        #region REGISTERED METHOD'S

        /// <summary>
        /// Register a delegate for a Foreign Columns
        /// </summary>
        /// <typeparam name="T">Foreign Table Type</typeparam>
        /// <param name="expresion"></param>
        public void RegisterForeignField<T>(Func<T, String> expresion)
        {
            Constraint constraint = _reflectedModel.Constraints.FirstOrDefault((cons) =>
            {
                return cons.Table.Type == typeof(T);
            });
            if (constraint == null)
            {
                throw new Gale.Exception.GaleException("API001", typeof(T).Name);
            }

            constraint.Table.SetDescriptor(expresion);
        }

        internal void RegisterParser<T>(GQLConfiguration configuration) where T : Gale.REST.Queryable.Primitive.Parser
        {
            this._parsers.Add(new TypedItem(typeof(T), configuration));
        }

        public void RegisterOperator<T>() where T : Gale.REST.Queryable.Primitive.Operator
        {
            Type _operator = typeof(T);
            var attr = (Gale.REST.Queryable.Primitive.OperatorAttribute)(_operator.GetCustomAttributes(typeof(Gale.REST.Queryable.Primitive.OperatorAttribute), true).FirstOrDefault());

            if (attr == null || attr.Alias.Trim().Length == 0)
            {
                throw new Gale.Exception.GaleException("API002", typeof(T).Name);
            }

            this._operators.Add(attr.Alias, _operator);
        }

        #endregion

        #region BUILDING AND EXECUTING METHODS
        private List<String> BuildQuery()
        {
            List<String> builder = new List<String>();

            _parsers.ForEach((parser) =>
            {
                //Bring up the Parser
                Gale.REST.Queryable.Primitive.Parser _parser = (Gale.REST.Queryable.Primitive.Parser)(Activator.CreateInstance(parser.Type));
                _parser._SetBuilder(this);

                //Executing Parsing
                string parserFragment = _parser.Parse(parser.Configuration, _reflectedModel);

                //------------------------------------------
                ExecutedParserEventHandler handler = OnExecutedParser;
                if (handler != null)
                {
                    var e = new Gale.REST.Queryable.Primitive.ExecutedParserEventArgs(_parser, parserFragment);
                    handler(e);
                    if (e.Changed)
                    {
                        parserFragment = e.ResultQueryFragment;
                    }
                }
                //------------------------------------------

                builder.Add(parserFragment);
            });

            return builder;
        }

        internal virtual String PrepareCall(List<String> statements)
        {
            return String.Join("", statements);
        }

        private Result _Execute(String query, Gale.Db.IDataActions databaseFactory)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            //-------------------------------------------------------------------------------------
            //---[ DATABASE CALL
            using (Gale.Db.DataService svc = new Gale.Db.DataService(query))
            {
                System.Data.DataTable db_data = null;
                System.Data.DataTable db_pagination = null;
                Gale.Db.EntityRepository rep = null;

                try
                {
                    //Create the repository
                    rep = this.DatabaseFactory.ExecuteSql(svc);

                    db_data = rep.GetRawTable(0);
                    db_pagination = rep.GetRawTable(1);

                }
                catch (System.Exception ex)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    throw new Gale.Exception.GaleException("API_DB_ERROR", message);
                }

                //--------[ GET ALL DATA IN AN ARRAY
                var data = new List<List<Object>>();
                SortedList<int, Field> ordinalsFields = new SortedList<int, Field>();

                var columns_count = db_data.Columns.Count;

                #region ORDINAL COLUMN SEARCH
                //SEARCH FOR ORDINAL'S
                foreach (Field field in _reflectedModel.SelectedFields)
                {
                    for (var column_index = 0; column_index < columns_count; column_index++)
                    {
                        if (field.Key.ToLower() == db_data.Columns[column_index].ColumnName.ToLower())
                        {
                            ordinalsFields.Add(db_data.Columns[column_index].Ordinal, field);
                            break;
                        }
                    }
                }
                #endregion


                if (db_pagination.Rows.Count != 1)
                {
                    throw new Gale.Exception.GaleException("API003");
                }
                int total = Convert.ToInt32(db_pagination.Rows[0]["total"]);

                #region TABLE FORMAT

                //Save all data from the foreign table for the descriptor's
                SortedList<Type, Gale.Db.IEntityTable> _foreignTableDatas = new SortedList<Type, Gale.Db.IEntityTable>();

                //Starting Fetching Data
                for (var row_index = 0; row_index < db_data.Rows.Count; row_index++)
                {
                    List<Object> item = new List<Object>();

                    foreach (int ordinal in ordinalsFields.Keys)
                    {
                        Field field = ordinalsFields[ordinal];

                        Object db_value = db_data.Rows[row_index][ordinal];

                        if (db_value is DateTime)
                        {
                            db_value = DateTime.SpecifyKind((DateTime)db_value, DateTimeKind.Local);
                        }

                        //If is FK , try to get the Descriptor, if not have descriptor, send Encripted Value :S
                        if (field.Specification == Field.SpecificationEnum.Fk)
                        {
                            Table table = _reflectedModel.Constraints.First(constraint => constraint.ThisField == field).Table;

                            if (table.Descriptor != null)
                            {
                                Gale.Db.IEntityTable tableData;
                                _foreignTableDatas.TryGetValue(table.Type, out tableData);

                                #region CREATE DATA TABLE FROM THE CURRENT SOURCE IF NOT EXIST YET
                                if (tableData == null)
                                {
                                    //Create Constraint Table Data
                                    System.Reflection.MethodInfo baseMethod = (from t in rep.GetType().GetMethods()
                                                                               where
                                                                                    t.GetGenericArguments().Count() > 0 &&
                                                                                    t.Name == "GetModel" && t.GetParameters().Count() == 0
                                                                               select t).FirstOrDefault();

                                    System.Reflection.MethodInfo GetModelMethod = baseMethod.MakeGenericMethod(table.Type);

                                    tableData = (Gale.Db.IEntityTable)GetModelMethod.Invoke(rep, null);
                                    _foreignTableDatas.Add(table.Type, tableData);
                                }
                                #endregion


                                //GET Constraint Function Expression to Get the FK Descriptor
                                Object _item = tableData.GetType().GetMethod("get_Item").Invoke(tableData, new object[] { row_index });
                                db_value = table.Descriptor.DynamicInvoke(_item).ToString();
                            }
                        }

                        item.Add(db_value);    //Column Value
                    }
                    data.Add(item);
                }
                #endregion

                timer.Stop();
                var response = new Gale.REST.Queryable.Primitive.Result(total, timer.Elapsed, _reflectedModel.SelectedFields, data);


                return response;

            }
            //-------------------------------------------------------------------------------------


        }

        internal Result Execute()
        {
            List<String> Statements = this.BuildQuery();
            String query = PrepareCall(Statements);

            return this._Execute(query, this._databaseFactory);
        }
        #endregion
    }
}
