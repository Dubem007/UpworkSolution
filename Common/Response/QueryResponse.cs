
namespace Common.Response;

public class QueryResponse<T>
{
    public T Data { get;  set; }

    public int Page { get;  set; }

    public int Size { get;  set; }

    public int Count { get;  set; }

    public int Total { get; set; }

    public bool Success { get; set; }

    public string Message { get; set; }

    public bool IsPaginated => Total - ((Page > 0 ? Page : 1) * Size) > 0;

    public QueryResponse()
    {
    }

    private QueryResponse(T data,
        int page,
        int size,
        int count,
        int total,
        bool success = true,
        string message = null)
    {
        Data = data;
        Page = page;
        Size = size;
        Count = count;
        Total = total;
        Success = success;
        Message = message;
    }

    public static QueryResponse<T> Generate(T data,
        int page,
        int size,
        int count,
        int total,
        bool success = true,
        string message = null)
    {
        return new QueryResponse<T>(data,
            page,
            size,
            count,
            total,
            success,
            message);
    }
}
