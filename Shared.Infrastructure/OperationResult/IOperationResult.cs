namespace Shared.Infrastructure.OperationResult
{
    public interface IOperationResult<TResult, TError>
        where TResult : IOperationResult<TResult, TError>, new()
        where TError : IOperationError
    {
        bool Success { get; }

        bool Failure { get; }

        TError Error { get; init; }
    }
}
