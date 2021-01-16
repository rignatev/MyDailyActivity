using System;

namespace Infrastructure.Shared.OperationResult
{
    public interface IOperationError
    {
        string Message { get; }

        Exception Exception { get; }

        bool IsExceptionalError { get; }
    }
}
