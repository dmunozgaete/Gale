using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Queryable.OData.Builders
{
    public abstract class HttpQueryBuilder<TModel> : Gale.REST.Queryable.Primitive.Generic.AbstractQueryBuilder<TModel> where TModel : class
    {
        public HttpRequestMessage Request { get; private set; }
        public GQLConfiguration Kql { get; private set; }

        public HttpQueryBuilder(Gale.Db.IDataActions databaseFactory, HttpRequestMessage request, GQLConfiguration configuration)
            : base(databaseFactory)
        {
            Request = request;
            System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(Request.RequestUri.Query);

            Kql = new GQLConfiguration();

            #region OFFSET
            if (query.AllKeys.Contains("$offset"))
            {
                int _offset = 0;
                int.TryParse(query["$offset"], out _offset);
                Kql.offset = _offset;
            }
            #endregion

            #region LIMIT
            if (query.AllKeys.Contains("$limit"))
            {
                int _limit = 0;
                int.TryParse(query["$limit"], out  _limit);
                if (_limit > 0)
                {
                    Kql.limit = _limit;
                }
            }
            #endregion

            #region ORDER BY
            if (query.AllKeys.Contains("$orderBy"))
            {
                String[] values = query["$orderBy"].Split(' ');

                Exception.GaleException.Guard(() => { return values.Length != 2; }, "API017");

                String fieldName = values[0].ToLower();
                String order = values[1].Trim().ToLower();

                Exception.GaleException.Guard(() => { return order != "asc" && order != "desc"; }, "API016");

                this.Kql.orderBy = new GQLConfiguration.OrderBy()
                {
                    name = fieldName,
                    order = (order == "asc") ? GQLConfiguration.OrderBy.orderEnum.asc : GQLConfiguration.OrderBy.orderEnum.desc
                };
            }
            #endregion

            #region SELECT
            if (query.AllKeys.Contains("$select"))
            {
                string fields = query["$select"].Trim();
                if (fields.Length > 0)
                {
                    List<string> selectedFields = fields.Split(',').ToList();

                    Exception.GaleException.Guard(() => { return selectedFields.Count == 0; }, "API011");
                    foreach (var fieldName in selectedFields)
                    {
                        Exception.GaleException.Guard(() => { return fieldName.Trim().Length == 0; }, "API011");
                    }

                    this.Kql.fields = selectedFields;
                }
            }
            #endregion

            #region FILTERS
            if (query.AllKeys.Contains("$filter"))
            {
                List<string> filters = query["$filter"].Trim().Split(',').ToList();
                foreach (string filterString in filters)
                {
                    string text = filterString;
                    Func<string> reduceFragment = new Func<string>(() =>
                    {
                        string _fragment;
                        int space_ordinal = text.IndexOf(" ");

                        //Gale Exception
                        Exception.GaleException.Guard(() => { return space_ordinal == 0; }, "API011");

                        _fragment = text.Substring(0, space_ordinal);
                        text = text.Substring(space_ordinal + 1);   //Reduce Filter

                        return _fragment;
                    });

                    //FILTER FORMAT => {field} {operatorAlias} {value}
                    string field = reduceFragment();
                    string operatorAlias = reduceFragment();
                    string value = text;

                    this.Kql.filters.Add(new GQLConfiguration.Filter()
                    {
                        field = field,
                        operatorAlias = operatorAlias,
                        value = text
                    });
                }
            }
            #endregion

            #region SETUP Manual GQLConfiguration
            if (configuration != null)
            {
                configuration.filters.ForEach((filter) =>
                {

                    this.Kql.filters.Add(filter);

                });

                configuration.fields.ForEach((fields) =>
                {

                    this.Kql.fields.Add(fields.ToLower());

                });

                if (configuration.limit > 0)
                {
                    this.Kql.limit = configuration.limit;
                }

                if (configuration.offset > 0)
                {
                    this.Kql.offset = configuration.offset;
                }

                if (configuration.orderBy != null && !String.IsNullOrEmpty(configuration.orderBy.name))
                {
                    this.Kql.orderBy = configuration.orderBy;
                }

            }
            #endregion
        }

        /// <summary>
        /// Retrieves the primitive result
        /// </summary>
        /// <returns></returns>
        public Gale.REST.Queryable.Primitive.Result GetResult()
        {
            return base.Execute();
        }

        /// <summary>
        /// Retrieves the response Http Message
        /// </summary>
        /// <returns></returns>
        public virtual HttpResponseMessage GetResponse()
        {
            return Request.CreateResponse<Gale.REST.Queryable.Primitive.Result>(GetResult());
        }
    }
}
