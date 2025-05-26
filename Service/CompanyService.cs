using Contracts;
using Service.Contracts;
using Shared.DTOs;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager loggerManager) : ICompanyService
{
    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
    {
        try
        {
            var companies = repository.Companies.GetAllCompanies(trackChanges);

            var companiesDto = companies.Select(c => new CompanyDto(c.Id, c.Name ?? "",
                string.Join(' ', c.Address, c.Country)));

            return companiesDto;
        }
        catch (Exception ex)
        {

            loggerManager.LogError($"Something went wrong in the {nameof(GetAllCompanies)} service method {ex}");
            throw;
        }
    }
}