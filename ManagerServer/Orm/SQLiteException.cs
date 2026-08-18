using System;

namespace ManagerServer.Orm
{
    public class SQLiteException : Exception
    {
        public SQLite3.Result Result { get; }

        public SQLiteException(SQLite3.Result result, string message)
            : base(message)
        {
            Result = result;
        }
    }
}
