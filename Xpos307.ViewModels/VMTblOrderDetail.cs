using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xpos307.ViewModels
{
    public class VMTblOrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        [StringLength(100)]
        public string ProductName { get; set; }

        public int Qty { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SumPrice { get; set; }
        public bool IsDelete { get; set; }
        public int CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public string TrxCode { get; set; }
        public int CustomerId { get; set; }
        [StringLength(50)]
        public string CustomerName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public int totalQty { get; set; }
        public bool IsCheckout { get; set; }
    }
}
