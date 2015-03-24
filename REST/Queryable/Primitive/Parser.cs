using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Karma.REST.Queryable.Primitive.Reflected;

namespace Karma.REST.Queryable.Primitive
{

    public abstract class Parser
    {
        private Karma.REST.Queryable.Primitive.IQueryBuilder _builder = null;

        private Karma.REST.Queryable.Primitive.IQueryBuilder QueryBuilder
        {
            get
            {
                return _builder;
            }
        }

        public Parser()
        {

        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void _SetBuilder(Karma.REST.Queryable.Primitive.IQueryBuilder builder)
        {
            this._builder = builder;
        }

        public abstract String Parse(String query, Model model);

        private String _callOperator(string operatorAlias, Field field, string value)
        {
            Type op = (from t in QueryBuilder.RegisteredOperators() where t.Key.ToLower() == operatorAlias.ToLower() select t.Value).FirstOrDefault();
            if (op == null)
            {
                throw new Karma.Exception.KarmaException("API004", operatorAlias);
            }

            Karma.REST.Queryable.Primitive.Operator _op = (Karma.REST.Queryable.Primitive.Operator)Activator.CreateInstance(op);
            return _op.Parse(field, value);
        }

        internal String CallOperator(string filter, Model model)
        {
            String[] values = filter.Trim().Split(' ');

            if (values.Length == 3)
            {
                string column_name = values[0].Trim().ToLower();
                string operatorAlias = values[1].Trim().ToLower();
                string value = values[2].Trim();

                Field field = (from t in model.Fields where t.Name == column_name select t).FirstOrDefault();

                if (field == null)
                {
                    throw new Karma.Exception.KarmaException("API005", column_name, filter);
                }

                return _callOperator(operatorAlias, field, value);
            }
            else
            {
                throw new Karma.Exception.KarmaException("API006", filter);
            }
        }
        internal String CallOperator(string filter)
        {
            return this.CallOperator(filter, this._builder.ReflectedModel());
        }
    }
}
