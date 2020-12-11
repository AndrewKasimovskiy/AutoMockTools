using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service
{
    [Table("Storage")]
    public class Item
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
