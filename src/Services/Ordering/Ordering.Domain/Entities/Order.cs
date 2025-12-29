using Contracts.Domains;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Entities
{
    public class Order : EntityAuditBase<long>
    {
        [Required]
        public string UserName { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string FirstName { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string LastName { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string EmailAddress { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string ShippingAddress { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string InvoiceAddress { get; set; }

        public EOrderStatus Status { get; set; }
        public Guid DocumentNo { get; set; } = Guid.NewGuid();
        [NotMapped] public string FullName => FirstName + " " + LastName;
    }
}
