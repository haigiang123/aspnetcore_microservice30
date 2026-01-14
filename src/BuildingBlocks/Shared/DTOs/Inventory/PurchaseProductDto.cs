using Shared.Enums.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Inventory
{
    public class PurchaseProductDto
    {
        public EDocumentType DocumentType => EDocumentType.Purchase;

        private string _itemNo { get; set; }

        public int Quantity { get; set; }

        public string GetItemNo() => _itemNo;

        public void SetItemNo(string itemNo)
        {
            _itemNo = itemNo;
        }
    }
}
