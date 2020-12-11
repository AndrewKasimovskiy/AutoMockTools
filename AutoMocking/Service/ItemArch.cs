using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [Table("StorageCanceled")]
    public class ItemArch
    {
        [Key]
        [Column("Id")]
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
