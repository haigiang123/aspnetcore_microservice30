using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; set; }

        public ValidationException() : base("One or more failure validation have occured")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
        {
            Errors = failures.GroupBy(x => x.ErrorCode, x => x.ErrorMessage).ToDictionary(y => y.Key, y => y.ToArray());
        }
    }
}
