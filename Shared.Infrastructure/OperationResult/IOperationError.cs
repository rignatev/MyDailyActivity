using System;

namespace Shared.Infrastructure.OperationResult
{
    public interface IOperationError
    {
        string Message { get; }

        Exception Exception { get; }

        bool IsExceptionalError { get; }
    }
}
