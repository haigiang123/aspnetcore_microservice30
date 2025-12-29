using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SeedWork
{
    public class ApiResult<T>
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResult() { }

        public ApiResult(bool isSucceeded, string message, T data)
        {
            IsSucceeded = isSucceeded;
            Message = message;
            Data = data;
        }  
        
        public ApiResult(bool isSucceded, string message = "")
        {
            Message = message;
            IsSucceeded = isSucceded;
        }
    }
}
