using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Networking;


/// <summary>
/// Extension for mimicking the more recent implementation (2020.x) of
/// <see cref="UnityWebRequest"/> and <see cref="UnityWebRequestAsyncOperation"/>
/// </summary>
public static class UnityWebRequestExtension
{
    public enum Result
    {
        InProgress,
        Success,
        ConnectionError,
        ProtocolError,
        DataProcessingError
    }


    public static TaskAwaiter<Result> GetAwaiter(this UnityWebRequestAsyncOperation operation)
    {
        TaskCompletionSource<Result> source = new TaskCompletionSource<Result>();
        operation.completed += op => source.TrySetResult(operation.webRequest.GetResult());

        if (operation.isDone)
            source.TrySetResult(operation.webRequest.GetResult());

        return source.Task.GetAwaiter();
    }


    public static Result GetResult(this UnityWebRequest request)
    {
        if (!request.isDone)
            return Result.InProgress;

        if (request.isNetworkError)
            return Result.ConnectionError;

        if (request.isHttpError)
            return Result.ProtocolError;

        //TODO(cb): Check, if we can get a reliable indicator for this case
        string error = request.error;
        if (!string.IsNullOrEmpty(error))
            return Result.DataProcessingError;

        return Result.Success;
    }
}