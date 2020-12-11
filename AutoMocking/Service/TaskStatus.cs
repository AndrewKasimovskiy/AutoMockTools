using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public enum TaskStatus
    {
        New = 0,
        Sent = 1,
        Finished = 2,
        Canceled = 3,
        Timeout = 4
    }
}
