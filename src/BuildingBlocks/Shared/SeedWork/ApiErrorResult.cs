using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SeedWork
{
    public class ApiErrorResult<T> : ApiResult<T>
    {
        public string[] Errors { get; set; }

        public ApiErrorResult() : this("Something was wrong. Please try again later")
        { }

        public ApiErrorResult(string message) : base(false, message)
        { }

        public ApiErrorResult(string[] errors) : base(false)
        {
            Errors = errors;
        }
    }
}
