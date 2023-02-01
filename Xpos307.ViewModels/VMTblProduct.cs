using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Xpos307.ViewModels
{
    public class VMTblProduct
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
       
        public int VariantId { get; set; }
        [StringLength(50)]
        public string VariantName { get; set; }
        
        public int CategoryId { get; set; }
        [StringLength(50)]
        public string CategoryName { get; set; }
        
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public bool IsDelete { get; set; }
        public int CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
    }
}
