using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xpos307.ViewModels
{
    public class VMTblOrderHeader
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string TrxCode { get; set; }
        public int CustomerId { get; set; }
        [StringLength(50)]
        public string CustomerName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public int TotalQty { get; set; }
        public bool IsCheckout { get; set; }
        public bool IsDelete { get; set; }
        public int CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
    }
}
