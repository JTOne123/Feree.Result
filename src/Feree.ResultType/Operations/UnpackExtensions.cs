using System;
using System.Threading.Tasks;
using Feree.ResultType.Results;

namespace Feree.ResultType.Operations
{
    public static class UnpackExtensions
    {
        public static bool IsSuccess<T>(this IResult<T> result, out T payload, out IError error)
        {
            switch (result)
            {
                case Success<T> success:
                    payload = success.Payload;
                    error = default;
                    return true;
                case Failure<T> failure:
                    payload = default;
                    error = failure.Error;
                    return false;
                default:
                    throw new InvalidOperationException("unknown result type");
            }
        }

        public static bool IsFailure<T>(this IResult<T> result, out T payload, out IError error) => !result.IsSuccess(out payload, out error);

        public static (T payload, IError error) Unpack<T>(this IResult<T> result)
        {
            result.IsSuccess(out var payload, out var error);
            return (payload, error);
        }
    }
}