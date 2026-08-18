using SQLitePCL;

namespace ManagerServer.Orm
{
    public static class SQLite3
    {
        public enum Result
        {
            OK = 0,
            Error = 1,
            Internal = 2,
            Perm = 3,
            Abort = 4,
            Busy = 5,
            Locked = 6,
            NoMem = 7,
            ReadOnly = 8,
            Interrupt = 9,
            IOError = 10,
            Corrupt = 11,
            NotFound = 12,
            Full = 13,
            CannotOpen = 14,
            LockErr = 15,
            Empty = 16,
            SchemaChngd = 17,
            TooBig = 18,
            Constraint = 19,
            Mismatch = 20,
            Misuse = 21,
            NotImplementedLFS = 22,
            AccessDenied = 23,
            Format = 24,
            Range = 25,
            NonDBFile = 26,
            Row = 100,
            Done = 101
        }

        public static sqlite3_stmt Prepare2(sqlite3 db, string sql)
        {
            var rc = raw.sqlite3_prepare_v2(db, sql, out var stmt);
            if (rc != raw.SQLITE_OK)
                throw new SQLiteException((Result)rc, raw.sqlite3_errmsg(db).utf8_to_string());
            return stmt;
        }

        public static Result Step(sqlite3_stmt stmt)
        {
            return (Result)raw.sqlite3_step(stmt);
        }

        public static string ColumnString(sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_text(stmt, index).utf8_to_string();
        }

        public static long ColumnInt64(sqlite3_stmt stmt, int index)
        {
            return raw.sqlite3_column_int64(stmt, index);
        }
    }
}
