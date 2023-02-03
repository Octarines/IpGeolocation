using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octarines.IpGeolocation.Models.ServiceResults
{
    public abstract class Result<T>: IResult
    {
        public abstract ResultType ResultType { get; }
        public virtual IEnumerable<string> Errors { get; }
        public virtual T Value => default(T);

        public bool HasErrors => Errors != null && Errors.Any();

        public Result<U> AsErrorResult<U>()
        {
            switch (ResultType)
            {
                case ResultType.NotFound:
                    return new NotFoundResult<U>(Errors);
                case ResultType.Invalid:
                    return new InvalidResult<U>(Errors);
                case ResultType.Unexpected:
                    return new UnexpectedResult<U>(Errors);
                case ResultType.Unauthorized:
                    return new UnauthorizedResult<U>();
                default:
                    throw new Exception("An unhandled result has occurred as a result of a service call.");
            }
        }
    }
}
