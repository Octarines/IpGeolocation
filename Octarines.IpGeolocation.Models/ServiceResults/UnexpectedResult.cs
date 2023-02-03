using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octarines.IpGeolocation.Models.ServiceResults
{
    public class UnexpectedResult<T>: Result<T>
    {
        private readonly IEnumerable<string> _errors;

        public override ResultType ResultType => ResultType.Unexpected;

        public override IEnumerable<string> Errors => _errors ?? new List<string> { "There was an unexpected problem" };


        public UnexpectedResult() { }

        public UnexpectedResult(string error)
        {
            _errors = new List<string>() { error };
        }

        public UnexpectedResult(IEnumerable<string> errors)
        {
            _errors = errors;
        }
    }

    public class UnexpectedResult : UnexpectedResult<object>
    {
        public UnexpectedResult() : base() { }

        public UnexpectedResult(string error) : base(error)
        {

        }

        public UnexpectedResult(IEnumerable<string> errors) : base(errors)
        {

        }
    }
}
