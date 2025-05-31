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

    public (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection
        (IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
            throw new CompanyCollectionBadRequest();

        var companyEntities = _mapper.ToCompanyEntity(companyCollection).ToList();

        repository.Companies.CreateCompanyCollection(companyEntities);

        repository.Save();

        var companiesDto = _mapper.ToCompanyDto(companyEntities).ToList();

        var ids = string.Join(',', companiesDto.Select(c => c.Id));

        return (companiesDto, ids);
    }

    public void DeleteCompany(Guid companyId, bool trackChanges)
    {
        var company = repository.Companies.GetCompany(companyId, false)
            ?? throw new CompanyNotFoundException(companyId);

        repository.Companies.DeleteCompany(company);
        repository.Save();

    }

    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
    {

        var companies = repository.Companies.GetAllCompanies(trackChanges);

        return [.. _mapper.ToCompanyDto(companies)];

    }

    public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new CollectionByIdsBadRequestException();

        var companiesEntities = repository.Companies.GetByIds(ids, trackChanges);

        if (ids.Count() != companiesEntities.Count())
            throw new CollectionByIdsBadRequestException();

        return _mapper.ToCompanyDto(companiesEntities);
    }

    public CompanyDto GetCompany(Guid companyId, bool trackChanges)
    {
        var company = repository.Companies.GetCompany(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        return _mapper.ToCompanyDto(company);

    }

    public void UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
    {
        var companyEntity = repository.Companies.GetCompany(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        _mapper.UpdateCompany(companyForUpdate, companyEntity);
        repository.Save();
    }
}