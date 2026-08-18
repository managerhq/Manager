using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ManagerServer.Orm
{
    internal sealed class TableMapping
    {
        public string TableName { get; }
        public bool WithoutRowId { get; }
        public Column PrimaryKey { get; }
        public Column[] Columns { get; }

        private TableMapping(Type type)
        {
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            TableName = tableAttr?.Name ?? type.Name;
            WithoutRowId = tableAttr?.WithoutRowId ?? false;

            var columns = new List<Column>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var col = new Column(prop);
                    columns.Add(col);
                    if (col.IsPrimaryKey) PrimaryKey = col;
                }
            }
            Columns = columns.ToArray();
        }

        private static readonly ConcurrentDictionary<Type, TableMapping> Cache = new();

        public static TableMapping Get(Type type) => Cache.GetOrAdd(type, t => new TableMapping(t));
        public static TableMapping Get<T>() => Get(typeof(T));

        internal sealed class Column
        {
            public string Name { get; }
            public PropertyInfo Property { get; }
            public Type ClrType { get; }
            public bool IsPrimaryKey { get; }
            public bool IsIndexed { get; }

            public Column(PropertyInfo prop)
            {
                Name = prop.Name;
                Property = prop;
                ClrType = prop.PropertyType;
                IsPrimaryKey = prop.GetCustomAttribute<PrimaryKeyAttribute>() != null;
                IsIndexed = prop.GetCustomAttribute<IndexedAttribute>() != null;
            }

            public object GetValue(object obj) => Property.GetValue(obj);
            public void SetValue(object obj, object value) => Property.SetValue(obj, value);
        }
    }
}
