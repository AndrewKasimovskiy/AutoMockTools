using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public interface IService
    {
        bool AddTask(DealTask item);
        DealTask GetTask(string cid);
        bool GetNextTaskId(string cid, out string nextCid);
        bool GetNextTaskId(int dealSer, int productId, out string nextCid);
        bool SetTaskStatus(string cid, TaskStatus status);
        bool ArchiveTask(string cid);
        bool ArchiveDeal(int dealSer, int productId);
        bool CanceledTask(string cid);
        bool CanceledDeal(int dealSer, int productId);
        List<string> GetAllTaskCids();
        List<string> GetAllTaskCids(int taskType);
        List<string> GetAllTaskCids(int taskType, int dealSer, int productId);
    }
}
