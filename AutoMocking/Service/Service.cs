using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
	public class Service : IService
    {
        private readonly IEfContext efContext;
        private readonly ILogger FLogger;

        public Service(IEfContext efContext, ILogger logger)
        {
            this.efContext = efContext;
            FLogger = logger;
        }

        public bool AddTask(DealTask task)
        {
            try
            {
                Item item  = MyMapper.Map<Item>(task);
                efContext.DealTasks.Add(item);
                int affected = efContext.SaveChanges();

                return affected == 1;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool ArchiveDeal(int dealSer, int productId)
        {
            IList<Item> items = efContext.DealTasks.Where(x => x.DealSer == dealSer && x.ProductId == productId).ToList();

            if (items.Count == 0) return false;

            efContext.DealTasksArch.AddRange(items.Select(x => MyMapper.Map<ItemArch>(x)).ToList());
            efContext.DealTasks.RemoveRange(items);
            efContext.SaveChanges();

            return true;
        }

        public bool ArchiveTask(string cid)
        {
            Item item = efContext.DealTasks.Where(x => x.Cid == cid).FirstOrDefault();
            
            if (item == null) return false;

            efContext.DealTasksArch.Add(MyMapper.Map<ItemArch>(item));
            efContext.DealTasks.Remove(item);
            efContext.SaveChanges();

            return true;
        }

        public bool CanceledDeal(int dealSer, int productId)
        {
            IList<Item> items = efContext.DealTasks.Where(x => x.DealSer == dealSer && x.ProductId == productId).ToList();

            if (items.Count == 0) return false;

            efContext.DealTasksCanceled.AddRange(items.Select(x => MyMapper.Map<ItemCanceled>(x)).ToList());
            efContext.DealTasks.RemoveRange(items);
            efContext.SaveChanges();

            return true;
        }

        public bool CanceledTask(string cid)
        {
            Item item = efContext.DealTasks.Where(x => x.Cid == cid).FirstOrDefault();

            if (item == null) return false;

            item.TaskStatus = (int)TaskStatus.Canceled;

            efContext.DealTasksCanceled.Add(MyMapper.Map<ItemCanceled>(item));
            efContext.DealTasks.Remove(item);
            efContext.SaveChanges();

            return true;
        }

        public List<string> GetAllTaskCids()
        => efContext.DealTasks.Select(x => x.Cid).ToList();

        public List<string> GetAllTaskCids(int taskType)
        => efContext.DealTasks.Where(x => x.TypeId == taskType).Select(x => x.Cid).ToList();

        public List<string> GetAllTaskCids(int taskType, int dealSer, int productId)
        => efContext.DealTasks.Where(x => x.TypeId == taskType && x.DealSer == dealSer && x.ProductId == productId).Select(x => x.Cid).ToList();

        public bool GetNextTaskId(string cid, out string nextCid)
        {
            nextCid = null;

            Item item = efContext.DealTasks.Where(x => x.Cid == cid).FirstOrDefault();

            if (item == null) return false;

            Item nextItem = efContext.DealTasks
                .Where(x => x.DealSer == item.DealSer && x.TaskStatus == (int)TaskStatus.New)
                .OrderBy(x => x.Id).FirstOrDefault();

            if (nextItem != null) nextCid = nextItem.Cid;

            return true;
        }

        public bool GetNextTaskId(int dealSer, int productId, out string nextCid)
        {
            nextCid = null;

            List<Item> tasks = efContext.DealTasks.Where(x => x.DealSer == dealSer && x.ProductId == productId).OrderBy(x => x.Id).ToList();

            if (tasks.Count == 0) return false;

            Item nextItem = tasks.Find(x => x.TaskStatus == (int)TaskStatus.New);
            if (nextItem != null) nextCid = nextItem.Cid;
            return true;
        }

        public DealTask GetTask(string cid)
        {
            Item task = efContext.DealTasks.Where(x => x.Cid == cid).FirstOrDefault();
            return task == null ? null : MyMapper.Map<DealTask>(task);
        }

        public bool SetTaskStatus(string cid, TaskStatus status)
        {
            Item item = efContext.DealTasks.Where(x => x.Cid == cid).FirstOrDefault();

            if (item == null) return false;

            if(item.TaskStatus != (byte)status)
            {
                item.TaskStatus = (byte)status;

                switch(status)
                {
                    case TaskStatus.Sent:
                        item.TimeSent = DateTime.Now;
                        break;

                    case TaskStatus.Finished:
                        item.TimeRecv = DateTime.Now;
                        break;
                }
                efContext.SaveChanges();
            }

            return true;
        }
    }
}
