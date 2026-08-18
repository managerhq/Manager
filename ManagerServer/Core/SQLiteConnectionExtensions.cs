using ManagerServer.Orm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ManagerServer
{
    public static class SQLiteConnectionExtensions
    {
        public static T[] OfType<T>(this Orm.SQLiteConnection db) where T : ManagerServer.Model.Object, new()
        {
            var contentType = ManagerServer.Model.Object.GetGuidByType(typeof(T));

            return db.Table<ManagerServer.ApplicationData.Object>()
                .Where(x => x.ContentType == contentType && x.Key != contentType)
                .ToArray()
                .Select(x => Deserialize(contentType, x.Content, x.Key, x.Timestamp))
                .Cast<T>()
                .ToArray();
        }

        public static T Single<T>(this Orm.SQLiteConnection db) where T : ManagerServer.Model.Object, new()
        {
            var contentType = ManagerServer.Model.Object.GetGuidByType(typeof(T));

            return db.Table<ManagerServer.ApplicationData.Object>()
                .Where(x => x.ContentType == contentType && x.Key == contentType)
                .ToArray()
                .Select(x => Deserialize(contentType, x.Content, x.Key, x.Timestamp))
                .Cast<T>()
                .SingleOrDefault() ?? new T() { Key = contentType };
        }

        public static ManagerServer.Model.Object SingleOrDefault(this Orm.SQLiteConnection db, Guid? key)
        {
            if (!key.HasValue) return null;

            return db.Table<ManagerServer.ApplicationData.Object>()
                .Where(x => x.Key == key)
                .ToArray()
                .Select(x => Deserialize(x.ContentType, x.Content, x.Key, x.Timestamp))
                .SingleOrDefault();
        }

        public static T SingleOrDefault<T>(this Orm.SQLiteConnection db, Guid? key) where T : ManagerServer.Model.Object, new()
        {
            if (!key.HasValue) return null;

            var contentType = ManagerServer.Model.Object.GetGuidByType(typeof(T));

            return db.Table<ManagerServer.ApplicationData.Object>()
                .Where(x => x.ContentType == contentType && x.Key == key)
                .ToArray()
                .Select(x => Deserialize(contentType, x.Content, x.Key, x.Timestamp))                
                .Cast<T>()
                .SingleOrDefault();
        }

        public static void InsertOrReplace2(this Orm.SQLiteTransaction tx, ManagerServer.Model.Object o)
        {
            var o2 = ManagerServer.ApplicationData.Object.From(o);
            tx.InsertOrReplace(o2);
        }

        public static void Delete2(this Orm.SQLiteTransaction tx, Guid key)
        {
            tx.Delete<ManagerServer.ApplicationData.Object>(key);
        }

        private static ManagerServer.Model.Object Deserialize(Guid contentType, ReadOnlySpan<byte> content, Guid key, long timestamp)
        {
            var o = Deserialize(contentType, content);
            if (o == null) return null;
            o.Key = key;
            o.Timestamp = timestamp;
            return o;
        }

        private static ManagerServer.Model.Object Deserialize(Guid contentType, ReadOnlySpan<byte> content)
        {
            var type = ManagerServer.Model.Object.GetTypeByGuid(contentType);
            if (type == null) return null;
            if (content.Length == 0) return (ManagerServer.Model.Object)Activator.CreateInstance(type);

            var o = (ManagerServer.Model.Object)ProtoBuf.Serializer.NonGeneric.Deserialize(type, content);
            return o;
        }
    }
}
