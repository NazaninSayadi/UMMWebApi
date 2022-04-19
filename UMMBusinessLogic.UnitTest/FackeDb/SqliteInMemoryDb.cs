using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;
using UMMBusinessLogic.UnitTest.FackeDb;
using UMMDomain;

namespace UMMBusinessLogic.UnitTest.FakeDb
{
    public class SqliteInMemoryDb : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<Context> _contextOptions;
        public SqliteInMemoryDb()
        {
            _connection = new SqliteConnection("datasource=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<Context>()
                .UseSqlite(_connection)
                .Options;

            using var context = new Context(_contextOptions);
            context.Database.EnsureCreated();
            context.AddRange(
           new BaseUnit { UnitName = "Meter", MetricName = "Length", Symbol = "m" });
            context.SaveChanges();
        }
        public void Dispose() => _connection.Dispose();
        public Context CreateContext() => new Context(_contextOptions);
    }
}