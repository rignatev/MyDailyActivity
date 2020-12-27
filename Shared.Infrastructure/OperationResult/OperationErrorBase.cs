using System;

namespace Shared.Infrastructure.OperationResult
{
    public abstract class OperationErrorBase : IOperationError
    {
        /// <inheritdoc />
        public string Message { get; }

        /// <inheritdoc />
        public Exception Exception { get; }

        /// <inheritdoc />
        public bool IsExceptionalError => this.Exception != null;

        protected OperationErrorBase(string message) =>
            this.Message = message;

        protected OperationErrorBase(Exception exception)
        {
            this.Exception = exception;
            this.Message = exception.Message;
        }
    }

    public abstract class OperationErrorBase<TErrorValue> : OperationErrorBase
    {
        public TErrorValue Value { get; }

        /// <inheritdoc />
        protected OperationErrorBase(string message) : base(message)
        {
        }

        /// <inheritdoc />
        protected OperationErrorBase(Exception exception) : base(exception)
        {
        }

        /// <inheritdoc />
        protected OperationErrorBase(string message, TErrorValue value) : base(message) =>
            this.Value = value;

        /// <inheritdoc />
        protected OperationErrorBase(Exception exception, TErrorValue value) : base(exception) =>
            this.Value = value;
    }
}
