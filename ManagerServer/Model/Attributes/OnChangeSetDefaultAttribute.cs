using System;

namespace ManagerServer.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class OnChangeSetDefaultAttribute : Attribute
    {
        public string Field;

        public OnChangeSetDefaultAttribute(string field)
        {
            Field = field;
        }

        public string GetExpression()
        {
            var expression = $"{Field} = item.Default{Field}";
            return $"if (item != null && (item.HasDefault{Field} == true || typeof item.HasDefault{Field} === 'undefined') && typeof item.Default{Field} !== 'undefined') (typeof lineItem !== typeof undefined ? (lineItem.{expression}) : ({expression}))";
        }
    }
}