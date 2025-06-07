using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager loggerManager) : ICompanyService
{
    private readonly MappingProfile _mapper = new();

    public async Task<bool> CompanyExistsAsync(Guid companyId)
        => await repository.Companies.CompanyExistsAsync(companyId);

    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
    {
        var CompanyEntity = _mapper.ToCompanyEntity(company);

        repository.Companies.CreateCompany(CompanyEntity);
        await repository.SaveAsync();

        return _mapper.ToCompanyDto(CompanyEntity);
    }

    public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync
        (IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
            throw new CompanyCollectionBadRequest();

        var companyEntities = _mapper.ToCompanyEntity(companyCollection).ToList();

        repository.Companies.CreateCompanyCollection(companyEntities);

        await repository.SaveAsync();

        var companiesDto = _mapper.ToCompanyDto(companyEntities).ToList();

        var ids = string.Join(',', companiesDto.Select(c => c.Id));

        return (companiesDto, ids);
    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await repository.Companies.GetCompanyAsync(companyId, false)
            ?? throw new CompanyNotFoundException(companyId);

        repository.Companies.DeleteCompany(company);
        await repository.SaveAsync();

    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
    {
        var companies = await repository.Companies.GetAllCompaniesAsync(trackChanges);

        return [.. _mapper.ToCompanyDto(companies)];

    }

    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new CollectionByIdsBadRequestException();

        var companiesEntities = await repository.Companies.GetByIdsAsync(ids, trackChanges);

        if (ids.Count() != companiesEntities.Count())
            throw new CollectionByIdsBadRequestException();

        return _mapper.ToCompanyDto(companiesEntities);
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await repository.Companies.GetCompanyAsync(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        return _mapper.ToCompanyDto(company);

    }

    public async Task<(CompanyForUpdateDto CompanyToPatch, Company companyEntity)> GetCompanyForPatchAsync(Guid companyId, bool trackChanges)
    {
        var companyEntity = await repository.Companies.GetCompanyAsync(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        var companyToPatch = _mapper.ToCompanyForUpdate(companyEntity);

        return (companyToPatch, companyEntity);
    }

    public async Task SaveChangesForPatchAsync(CompanyForUpdateDto companyToPatch, Company companyEntity)
    {
        _mapper.UpdateCompany(companyToPatch, companyEntity);
        await repository.SaveAsync();
    }

    public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
    {
        var companyEntity = await repository.Companies.GetCompanyAsync(companyId, trackChanges)
            ?? throw new CompanyNotFoundException(companyId);

        _mapper.UpdateCompany(companyForUpdate, companyEntity);
        await repository.SaveAsync();
    }
}