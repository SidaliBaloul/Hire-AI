using System.Diagnostics.CodeAnalysis;

namespace HireAI.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public Result(bool issuccess, Error error)
    {
        if(issuccess && error != Error.None || !issuccess && error == Error.None)
        {
            throw new ArgumentException($"Invalid Error {nameof(error)}");
        }


        IsSuccess = issuccess;
        Error = error;
    }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return new Result<TValue>(value, true, Error.None);
    }

    public static Result<TValue> Failure<TValue>(Error error)
    {
        return new Result<TValue>(default, false, error);
    }

}

public class Result<TValue> : Result
{
    public TValue? _Value { get;  set; }

    public Result(TValue? value, bool issuccess, Error error) : base(issuccess, error)
    {
        _Value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _Value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");


    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}
