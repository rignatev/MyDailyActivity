using System;

namespace Shared.Infrastructure.OperationResult
{
    public abstract class OperationResultBase<TResult, TError> : IOperationResult<TResult, TError>
        where TResult : OperationResultBase<TResult, TError>, new()
        where TError : IOperationError
    {
        /// <inheritdoc />
        public bool Success => !this.Failure;

        /// <inheritdoc />
        public bool Failure => this.Error != null;

        /// <inheritdoc />
        public TError Error { get; init; }

        static public TResult Ok() =>
            new();

        static public TResult Fail(TError error) =>
            new() { Error = error };
    }

    public abstract class OperationResultBase<TResult, TError, TResultValue> : IOperationResult<TResult, TError>
        where TResult : OperationResultBase<TResult, TError, TResultValue>, new()
        where TError : IOperationError
    {
        public TResultValue Value { get; init; }

        /// <inheritdoc />
        public bool Success => !this.Failure;

        /// <inheritdoc />
        public bool Failure => this.Error != null;

        /// <inheritdoc />
        public TError Error { get; private init; }

        static public TResult Ok(TResultValue value) =>
            new() { Value = value };

        static public TResult Fail(TError error) =>
            new() { Error = error };

        public virtual TResultValue GetValueOrThrow()
        {
            if (this.Success && this.Value != null)
            {
                return this.Value;
            }

            throw new Exception($"Failed to get operation result value {typeof(TResultValue)}");
        }
    }
}
