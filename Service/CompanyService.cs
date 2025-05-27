using Contracts;
using Entities.Exceptions;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager loggerManager) : ICompanyService
{
    public bool CompanyExists(Guid companyId) => repository.Companies.CompanyExists(companyId);

    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
    {

        var companies = repository.Companies.GetAllCompanies(trackChanges);

        var companiesDto = new MappingProfile().ToCompanyDto(companies).ToList();

        return companiesDto;
    }

    public CompanyDto GetCompany(Guid companyId, bool trackChanges)
    {
        var company = repository.Companies.GetCompany(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        var companyDto = new MappingProfile().ToCompanyDto(company);

        return companyDto;
    }
}