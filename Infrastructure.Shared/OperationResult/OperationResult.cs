namespace Infrastructure.Shared.OperationResult
{
    public class OperationResult : OperationResultBase<OperationResult, OperationError>
    {
    }
    
    public class OperationResult<TResultValue> : OperationResultBase<OperationResult<TResultValue>, OperationError, TResultValue>
    {
    }
    
    public class OperationResult<TResultValue, TErrorValue>
        : OperationResultBase<
            OperationResult<TResultValue, TErrorValue>,
            OperationError<TErrorValue>,
            TResultValue>
    {
    }
}
