namespace EICInventorySystem.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public string? ErrorCode { get; }

    protected Result(bool isSuccess, string error, string? errorCode = null)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new ArgumentException("Success result cannot have error message", nameof(error));

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new ArgumentException("Failure result must have error message", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new Result(true, string.Empty);
    public static Result Failure(string error, string? errorCode = null) => new Result(false, error, errorCode);
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException("Cannot access value of failed result");

            return _value!;
        }
    }

    protected Result(bool isSuccess, T? value, string error, string? errorCode = null)
        : base(isSuccess, error, errorCode)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, string.Empty);
    public static new Result<T> Failure(string error, string? errorCode = null) => new Result<T>(false, default, error, errorCode);
}

public static class ResultExtensions
{
    public static async Task<Result> Then(this Result result, Func<Task<Result>> next)
    {
        if (result.IsFailure)
            return result;

        return await next();
    }

    public static async Task<Result<T>> Then<T>(this Result result, Func<Task<Result<T>>> next)
    {
        if (result.IsFailure)
            return Result<T>.Failure(result.Error, result.ErrorCode);

        return await next();
    }

    public static async Task<Result> Then<T>(this Result<T> result, Func<T, Task<Result>> next)
    {
        if (result.IsFailure)
            return result;

        return await next(result.Value);
    }

    public static async Task<Result<TResult>> Then<T, TResult>(this Result<T> result, Func<T, Task<Result<TResult>>> next)
    {
        if (result.IsFailure)
            return Result<TResult>.Failure(result.Error, result.ErrorCode);

        return await next(result.Value);
    }
}
