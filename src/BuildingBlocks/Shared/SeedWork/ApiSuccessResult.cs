using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SeedWork
{
    public class ApiSuccessResult<T> : ApiResult<T>
    {
        public ApiSuccessResult(T data, string message) : base(true, message, data)
        { }

        public ApiSuccessResult(T data) : base(true, "Success", data)
        {

        }
    }
}
