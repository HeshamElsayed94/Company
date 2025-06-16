namespace Entities.Responses;

public sealed class CompanyNotFoundResponse(Guid id)
    : ApiNotFoundResponse($"Company with id: {id} is not found in db.")
{
}