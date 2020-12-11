using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DealTask
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int DealSer { get; set; }
        public int ProductId { get; set; }
        public string Cid { get; set; }
        public DateTime? TimeSent { get; set; }
        public DateTime? TimeRecv { get; set; }
        public byte TaskStatus { get; set; }
        public byte[] Context { get; set; }
    }
}
