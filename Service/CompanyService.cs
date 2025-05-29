using Contracts;
using Entities.Exceptions;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager loggerManager) : ICompanyService
{
    private readonly MappingProfile _mapper = new();

    public bool CompanyExists(Guid companyId) => repository.Companies.CompanyExists(companyId);

    public CompanyDto CreateCompany(CompanyForCreationDto company)
    {
        var CompanyEntity = _mapper.ToCompanyEntity(company);

        repository.Companies.CreateCompany(CompanyEntity);
        repository.Save();

        return _mapper.ToCompanyDto(CompanyEntity);
    }

    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
    {

        var companies = repository.Companies.GetAllCompanies(trackChanges);

        return [.. _mapper.ToCompanyDto(companies)];

    }

    public CompanyDto GetCompany(Guid companyId, bool trackChanges)
    {
        var company = repository.Companies.GetCompany(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        return _mapper.ToCompanyDto(company);

    }
}