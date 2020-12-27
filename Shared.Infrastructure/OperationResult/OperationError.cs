using System;

namespace Shared.Infrastructure.OperationResult
{
    public class OperationError : OperationErrorBase
    {
        /// <inheritdoc />
        public OperationError(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public OperationError(Exception exception) : base(exception)
        {
        }
    }

    public class OperationError<TErrorValue> : OperationErrorBase<TErrorValue>
    {
        /// <inheritdoc />
        public OperationError(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public OperationError(Exception exception) : base(exception)
        {
        }

        /// <inheritdoc />
        public OperationError(string message, TErrorValue value) : base(message, value)
        {
        }

        /// <inheritdoc />
        public OperationError(Exception exception, TErrorValue value) : base(exception, value)
        {
        }
    }
}
