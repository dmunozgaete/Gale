using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gale.REST.Queryable.OData.Builders;
using Gale.REST.Queryable.Primitive.Reflected;

namespace Gale.REST.Queryable.Primitive
{

    public abstract class Parser
    {
        private Gale.REST.Queryable.Primitive.AbstractQueryBuilder _builder = null;

        private Gale.REST.Queryable.Primitive.AbstractQueryBuilder QueryBuilder
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
        public void _SetBuilder(Gale.REST.Queryable.Primitive.AbstractQueryBuilder builder)
        {
            this._builder = builder;
        }

        public abstract String Parse(GQLConfiguration configuration, Model model);

        private String _callOperator(string operatorAlias, Field field, string value)
        {
            Type op = (from t in QueryBuilder.RegisteredOperators() where t.Key.ToLower() == operatorAlias.ToLower() select t.Value).FirstOrDefault();
            if (op == null)
            {
                throw new Gale.Exception.GaleException("API004", operatorAlias);
            }

            Gale.REST.Queryable.Primitive.Operator _op = (Gale.REST.Queryable.Primitive.Operator)Activator.CreateInstance(op);
            return _op.Parse(field, value);
        }

        internal String CallOperator(string filter, Model model)
        {
            string _property = null;
            string _operator = null;
            string _value = null;

            string trimmed = filter.Trim();
            char charKey = ' ';

            //Property
            int charKeyPosition = trimmed.IndexOf(charKey);
            if (charKeyPosition >= 0)
            {
                _property = trimmed.Substring(0, charKeyPosition).ToLower();
                trimmed = trimmed.Substring(charKeyPosition).Trim();
            }

            //Operator
            charKeyPosition = trimmed.IndexOf(charKey);
            if (charKeyPosition >= 0)
            {
                _operator = trimmed.Substring(0, charKeyPosition).ToLower();
                trimmed = trimmed.Substring(charKeyPosition).Trim();
            }

            //Value Sanitization
            _value = trimmed.Replace("%", "").Replace("'", "");

         
            if (!String.IsNullOrEmpty(_property) && !String.IsNullOrEmpty(_operator) && !String.IsNullOrEmpty(_value) )
            {
               
                Field field = (from t in model.Fields where t.Name.ToLower() == _property select t).FirstOrDefault();

                if (field == null)
                {
                    throw new Gale.Exception.GaleException("API005", _property, filter);
                }

                return _callOperator(_operator, field, _value);
            }
            else
            {
                throw new Gale.Exception.GaleException("API006", filter);
            }
        }
        internal String CallOperator(string filter)
        {
            return this.CallOperator(filter, this._builder.ReflectedModel());
        }
    }
}
