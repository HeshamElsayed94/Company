using Entities.Responses;

namespace CompanyEmployees.Presentation.ApiBaseResponseExtensions;

public static class ApiBaseResponseExtensions
{
    public static T GetResult<T>(this ApiBaseResponse apiBaseResponse)
        => ((ApiOkResponse<T>)apiBaseResponse).Result;
}