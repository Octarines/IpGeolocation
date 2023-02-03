using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octarines.IpGeolocation.Models.ServiceResults
{
    public class UnauthorizedResult<T> : Result<T>
    {
        public override ResultType ResultType => ResultType.Unauthorized;

        public override IEnumerable<string> Errors => new List<string> { "The request was unauthorized." };

        public override T Value => default(T);
    }

    public class UnauthorizedResult : UnauthorizedResult<object>
    {

    }
}
