using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octarines.IpGeolocation.Models.ServiceResults
{
    public class InvalidResult<T> : Result<T>
    {
        private IEnumerable<string> _errors;
        
        public override ResultType ResultType => ResultType.Invalid;

        public override IEnumerable<string> Errors => _errors ?? new List<string> { "The input was invalid." };


        public InvalidResult() { }

        public InvalidResult(string error)
        {
            _errors = new List<string>() { error };
        }

        public InvalidResult(IEnumerable<string> errors)
        {
            _errors = errors;
        }
    }

    public class InvalidResult : InvalidResult<object>
    {
        public InvalidResult() : base() { }

        public InvalidResult(string error) : base(error)
        {

        }

        public InvalidResult(IEnumerable<string> errors) : base(errors)
        {

        }
    }
}
