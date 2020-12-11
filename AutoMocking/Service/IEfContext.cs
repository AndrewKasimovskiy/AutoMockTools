using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IEfContext
    {
        ObjectContext Context { get; set; }
        DbSet<Item> DealTasks { get; set; }
        DbSet<ItemArch> DealTasksArch { get; set; }
        DbSet<ItemCanceled> DealTasksCanceled { get; set; }
        int SaveChanges();
    }
}
