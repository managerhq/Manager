using System;
using System.Linq;

namespace ManagerServer.Model.Attributes
{
    public abstract class IfContainsAttribute : Attribute
    {
        public abstract bool Contains(Database database);
    }

    public sealed class IfContainsAttribute<T> : IfContainsAttribute where T : ManagerServer.Model.Object, new()
    {
        public override bool Contains(Database database)
        {
            return database.OfType<T>().Any();
        }
    }

    public sealed class IfContainsAttribute<T1, T2> : IfContainsAttribute where T1 : ManagerServer.Model.Object, new() where T2 : ManagerServer.Model.Object, new()
    {
        public override bool Contains(Database database)
        {
            return database.OfType<T1>().Any() || database.OfType<T2>().Any();
        }
    }

    public sealed class IfContainsAttribute<T1, T2, T3> : IfContainsAttribute where T1 : ManagerServer.Model.Object, new() where T2 : ManagerServer.Model.Object, new() where T3 : ManagerServer.Model.Object, new()
    {
        public override bool Contains(Database database)
        {
            return database.OfType<T1>().Any() || database.OfType<T2>().Any() || database.OfType<T3>().Any();
        }
    }
}
