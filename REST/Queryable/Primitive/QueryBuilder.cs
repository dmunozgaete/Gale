using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Karma.REST.Queryable.Primitive.Reflected;

namespace Karma.REST.Queryable.Primitive
{
    public abstract class QueryBuilder<TModel> : Karma.REST.Queryable.Primitive.IQueryBuilder
        where TModel : class
    {
        //------[ VARIABLES
        private Model _reflectedModel = null;
        private List<TypedItem> _parsers = new List<TypedItem>();
        private SortedList<String, Type> _operators = new SortedList<String, Type>();
        private Karma.Db.IDataActions _databaseFactory;
        private Format format = Format.Primitive;

        public enum Format
        {
            Primitive = 1,
            Table = 2
        }

        //------[ EVENT
        public delegate void ExecutedParserEventHandler(Karma.REST.Queryable.Primitive.ExecutedParserEventArgs e);
        public event ExecutedParserEventHandler OnExecutedParser;
        //---------------------------------------------------------

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
                    throw new Karma.Exception.KarmaException("API019", table.Name);
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

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="databaseFactory"></param>
        public QueryBuilder(Karma.Db.IDataActions databaseFactory)
        {
            //Setting the Database Factory Type
            this._databaseFactory = databaseFactory;

            //Entity Type for the CRUD
            Type entityType = typeof(TModel);

            //Field List
            this._reflectedModel = ReflectModel(entityType);
        }


        #region PROPERTIES

        public Model ReflectedModel()
        {
            return _reflectedModel;
        }

        private Karma.Db.IDataActions DatabaseFactory
        {
            get
            {
                return _databaseFactory;
            }
        }

        [System.ComponentModel.ReadOnly(true)]
        public List<TypedItem> RegisteredParsers()
        {
            return _parsers;
        }

        [System.ComponentModel.ReadOnly(true)]
        public SortedList<String, Type> RegisteredOperators()
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
                throw new Karma.Exception.KarmaException("API001", typeof(T).Name);
            }

            constraint.Table.SetDescriptor(expresion);
        }

        internal void RegisterParser<T>(string parsingQuery) where T : Karma.REST.Queryable.Primitive.Parser
        {
            this._parsers.Add(new TypedItem(typeof(T), parsingQuery));
        }

        public void RegisterOperator<T>() where T : Karma.REST.Queryable.Primitive.Operator
        {
            Type _operator = typeof(T);
            var attr = (Karma.REST.Queryable.Primitive.OperatorAttribute)(_operator.GetCustomAttributes(typeof(Karma.REST.Queryable.Primitive.OperatorAttribute), true).FirstOrDefault());

            if (attr == null || attr.Alias.Trim().Length == 0)
            {
                throw new Karma.Exception.KarmaException("API002", typeof(T).Name);
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
                Karma.REST.Queryable.Primitive.Parser _parser = (Karma.REST.Queryable.Primitive.Parser)(Activator.CreateInstance(parser.Type));
                _parser._SetBuilder(this);

                //Executing Parsing
                string parserFragment = _parser.Parse(parser.QueryFragment, _reflectedModel);

                //------------------------------------------
                ExecutedParserEventHandler handler = OnExecutedParser;
                if (handler != null)
                {
                    var e = new Karma.REST.Queryable.Primitive.ExecutedParserEventArgs(_parser, parserFragment);
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

        internal virtual String PrepareCall(List<String> statements, int offset, int limit)
        {
            return String.Join("", statements);
        }

        private Karma.REST.Queryable.Primitive.IResponse _Execute(String query, int offset, int limit, Format format, Karma.Db.IDataActions databaseFactory)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            //-------------------------------------------------------------------------------------
            //---[ DATABASE CALL
            using (Karma.Db.DataService svc = new Karma.Db.DataService(query))
            {
                System.Data.DataTable db_data = null;
                System.Data.DataTable db_pagination = null;
                Karma.Db.EntityRepository rep = null;

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
                    throw new Karma.Exception.KarmaException("API_DB_ERROR", message);
                }

                //--------[ GET ALL DATA IN AN ARRAY
                var data = new List<List<String>>();
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
                    throw new Karma.Exception.KarmaException("API003");
                }
                int total = Convert.ToInt32(db_pagination.Rows[0]["total"]);

                #region TABLE FORMAT

                //Save all data from the foreign table for the descriptor's
                SortedList<Type, Karma.Db.IEntityTable> _foreignTableDatas = new SortedList<Type, Karma.Db.IEntityTable>();

                //Starting Fetching Data
                for (var row_index = 0; row_index < db_data.Rows.Count; row_index++)
                {
                    List<String> item = new List<String>();

                    foreach (int ordinal in ordinalsFields.Keys)
                    {
                        Field field = ordinalsFields[ordinal];

                        Object db_value = db_data.Rows[row_index][ordinal];
                        String value = db_value.ToString();

                        if (db_value is DateTime)
                        {
                            value = Convert.ToDateTime(db_value).ToString("s"); //ISO 8601
                        }

                        /*
                        //If is PK, Encrypt Value
                        if (field.Specification == Field.SpecificationEnum.Pk)
                        {
                            //Encrypt Value
                            value = EncryptValue(value);
                        }
                        */

                        //If is FK , try to get the Descriptor, if not have descriptor, send Encripted Value :S
                        if (field.Specification == Field.SpecificationEnum.Fk)
                        {
                            Table table = _reflectedModel.Constraints.First(constraint => constraint.ThisField == field).Table;

                            if (table.Descriptor != null)
                            {
                                Karma.Db.IEntityTable tableData;
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

                                    tableData = (Karma.Db.IEntityTable)GetModelMethod.Invoke(rep, null);
                                    _foreignTableDatas.Add(table.Type, tableData);
                                }
                                #endregion


                                //GET Constraint Function Expression to Get the FK Descriptor
                                Object _item = tableData.GetType().GetMethod("get_Item").Invoke(tableData, new object[] { row_index });
                                value = table.Descriptor.DynamicInvoke(_item).ToString();
                            }/*
                            else
                            {
                                value = EncryptValue(value);
                            }
                              * */
                        }

                        item.Add(value);    //Column Value
                    }
                    data.Add(item);
                }
                #endregion

                var table_response  = new Karma.REST.Queryable.Primitive.Response(offset, limit, total, timer.Elapsed, _reflectedModel.SelectedFields, data);
                timer.Stop();

                if (format == Format.Primitive)
                {
                    return new PrimitiveResponse(table_response);
                }

                return table_response;

            }
            //-------------------------------------------------------------------------------------


        }

        /*
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual String EncryptValue(String value)
        {
            return Karma.Security.Cryptography.Rijndael.Encrypt(value, true);
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual String DecryptValue(String value)
        {
            return Karma.Security.Cryptography.Rijndael.Decrypt(value, true);
        }
        */
        public Karma.REST.Queryable.Primitive.IResponse Execute(int offset, int limit)
        {
            return Execute(offset, limit, format);
        }

        public Karma.REST.Queryable.Primitive.IResponse Execute(int offset, int limit, Format format)
        {
            List<String> Statements = this.BuildQuery();
            String query = PrepareCall(Statements, offset, limit);

            return this._Execute(query, offset, limit, format, this._databaseFactory);
        }
                #endregion


        private class DummyConstraint
        {
            private String _thisName;
            private String _otherName;
            private Table _table;

            public DummyConstraint(string thisName, string otherName, Table table)
            {
                this._thisName = thisName;
                this._otherName = otherName;
                this._table = table;
            }

            public String thisName
            {
                get
                {
                    return _thisName;
                }
            }

            public String otherName
            {
                get
                {
                    return _otherName;
                }
            }

            public Table table
            {
                get
                {
                    return _table;
                }
            }
        }
    }
}
