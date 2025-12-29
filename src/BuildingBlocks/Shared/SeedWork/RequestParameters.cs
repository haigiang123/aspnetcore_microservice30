using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SeedWork
{
    public class RequestParameters : PagingRequestParameters
    {
        public string? OrderBy { get; set; }
    }
}
