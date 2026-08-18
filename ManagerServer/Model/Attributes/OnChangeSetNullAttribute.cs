using System;

namespace ManagerServer.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class OnChangeSetNullAttribute : Attribute
    {
        public string TargetField;

        public OnChangeSetNullAttribute(string targetField)
        {
            TargetField = targetField;
        }

        public string GetExpression()
        {
            return $"(typeof lineItem !== 'undefined' && lineItem != null ? (lineItem.{TargetField} = null) : ({TargetField} = null))";
        }
    }
}