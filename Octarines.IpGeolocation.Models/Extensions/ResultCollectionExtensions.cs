using Octarines.IpGeolocation.Models.ServiceResults;

namespace Octarines.IpGeolocation.Models.Extensions;

public static class ResultCollectionExtensions
{
    public static Result<T> AsErrorResult<T>(this IEnumerable<IResult> results)
    {
        IEnumerable<IResult> errorResults = results.Where(x => x.HasErrors);

        if (errorResults.Any())
        {
            IEnumerable<string> errors = errorResults.SelectMany(x => x.Errors);

            switch (errorResults.First().ResultType)
            {
                case ResultType.NotFound:
                    return new NotFoundResult<T>(errors);
                case ResultType.Invalid:
                    return new InvalidResult<T>(errors);
                case ResultType.Unauthorized:
                    return new UnauthorizedResult<T>();
                default:
                case ResultType.Unexpected:
                    return new UnexpectedResult<T>(errors);
            }
        }

        throw new Exception("Selected collection does not contain any error results");
    }
}