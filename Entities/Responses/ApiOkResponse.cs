namespace Entities.Responses;

public sealed class ApiOkResponse<T>(T result) : ApiBaseResponse(true)
{
    public T Result { get; } = result;
}