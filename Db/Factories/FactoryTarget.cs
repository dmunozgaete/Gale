using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.Db.Factories
{

    /// <summary>
    /// Specific Class , to associate a Class of interface dependant of the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FactoryTarget : System.Attribute
    {
        private Type _factory;

        /// <summary>
        /// Associate a Class of Interface dependant of the Database
        /// </summary>
        /// <param name="factory">Database Factory Associated with the Class</param>
        public FactoryTarget(Type factory)
        {
            this._factory = factory;
        }

        /// <summary>
        /// Factory Target for the defined Class
        /// </summary>
        public Type Factory
        {
            get
            {
                return _factory;
            }
        }

        /// <summary>
        /// Find all Class which inherits from the TFactory Type, and are associated with Specific Database Factory
        /// </summary>
        /// <typeparam name="TFactory">Abstract Class To match in the Searched Classes</typeparam>
        /// <returns></returns>
        public static List<Type> GetTypesByDatabaseTarget<TFactory>() where TFactory : class
        {
            return GetTypesByDatabaseTarget<TFactory>(null);
        }

        /// <summary>
        /// Find all Class which inherits from the TFactory Type, and are associated with Specific Database Factory
        /// </summary>
        /// <typeparam name="TFactory">Abstract Class To match in the Searched Classes</typeparam>
        /// <param name="TypeResolver">Callback to execute if has a more than Types matched in the search</param>
        /// <returns></returns>
        public static List<Type> GetTypesByDatabaseTarget<TFactory>(Func<List<Type>, List<Type>> TypeResolver) where TFactory : class
        {
            //Extract the Database Factory based in the "Selected Factory"
            //Get factory type from the web.config
            var cnx = System.Configuration.ConfigurationManager.ConnectionStrings[Gale.REST.Resources.GALE_CONNECTION_DEFAULT_KEY];
            Type factory_type = Type.GetType(cnx.ProviderName);

            List<Type> matchedTypes = new List<Type>();

            //Get all builder's from the assembly which factory resides
            var builders = (from q_type in
                                factory_type.Assembly.GetTypes()
                            where
                              q_type.IsClass &&
                              q_type.IsAbstract == false &&
                              typeof(TFactory).IsAssignableFrom(q_type)
                            select new
                            {
                                type = q_type,
                                attrSelector = q_type.GetCustomAttributes(typeof(Gale.Db.Factories.FactoryTarget), true).FirstOrDefault()
                            });


            //Find the correct QueryBuilder<> associated with the DB Factory
            foreach (var builder in builders)
            {

                //Check if the attribute has the factory selector attribute
                Gale.Exception.RestException.Guard(() => { return builder.attrSelector == null; }, "ALL_FACTORIES_SEARCHED_MUSTHAVE_FACTORYTARGET_ATTRIBUTE", Gale.Exception.Errors.ResourceManager);

                //If the correct Builder??
                var attr = (Gale.Db.Factories.FactoryTarget)builder.attrSelector;
                if (attr.Factory == factory_type)
                {
                    //Add to the list
                    matchedTypes.Add(builder.type);
                }
            }

            //More than One Result??
            if (TypeResolver != null && matchedTypes.Count >= 1)
            {
                matchedTypes = TypeResolver(matchedTypes);
            }

            return matchedTypes;
        }
    }
}
