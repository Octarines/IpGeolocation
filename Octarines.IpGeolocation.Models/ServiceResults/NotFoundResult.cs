using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octarines.IpGeolocation.Models.ServiceResults
{
    public class NotFoundResult<T> : Result<T>
    {
        private readonly IEnumerable<string> _errors;

        public override ResultType ResultType => ResultType.NotFound;

        public override IEnumerable<string> Errors => _errors ?? new List<string> { "The entity you're looking for cannot be found" };


        public NotFoundResult() { }

        public NotFoundResult(string error)
        {
            _errors = new List<string>() { error };
        }

        public NotFoundResult(IEnumerable<string> errors)
        {
            _errors = errors;
        }
    }

    public class NotFoundResult : NotFoundResult<object>
    {
        public NotFoundResult() : base() { }

        public NotFoundResult(string error) : base(error)
        {

        }

        public NotFoundResult(IEnumerable<string> errors) : base(errors)
        {

        }
    }
}
