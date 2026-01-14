using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Inventory
{
    public class CreatedSalesOrderSuccessDto
    {
        public string DocumentNo { get; }

        public CreatedSalesOrderSuccessDto(string documentNo)
        {
            DocumentNo = documentNo;
        }
    }
}
