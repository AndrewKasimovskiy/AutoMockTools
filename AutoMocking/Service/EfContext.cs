using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EfContext : DbContext, IEfContext
    {
        public ObjectContext Context { get; set; }
        public EfContext(string connectionString) : base()
        {
            Context = new ObjectContext(connectionString);
        }

        public DbSet<Item> DealTasks { get; set; }
        public DbSet<ItemArch> DealTasksArch { get; set; }
        public DbSet<ItemCanceled> DealTasksCanceled { get; set; }

        public DbContextTransaction BeginTransaction() => Database.BeginTransaction();
        public DbContextTransaction BeginTransaction(System.Data.IsolationLevel isolation) => Database.BeginTransaction(isolation);

		public override int SaveChanges()
		{
			return base.SaveChanges();
		}

		public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(sql, parameters);
        }
    }
}
