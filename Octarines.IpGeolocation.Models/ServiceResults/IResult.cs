using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octarines.IpGeolocation.Models.ServiceResults
{
    public interface IResult
    {
        public ResultType ResultType { get; }

        public IEnumerable<string> Errors { get; }

        public bool HasErrors => Errors != null && Errors.Any();

        public Result<U> AsErrorResult<U>();
    }
}
