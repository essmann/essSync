using System;
using System.Collections.Generic;
using System.Text;

namespace essSync.src.Utils
{
    public static class DbPaths
    {
        public static string Sqlite =>
            Path.Combine(AppContext.BaseDirectory, "data", "database.sqlite");
    }
}
