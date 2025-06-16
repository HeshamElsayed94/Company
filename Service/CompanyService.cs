using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Entities.Responses;
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
        var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

        repository.Companies.DeleteCompany(company);
        await repository.SaveAsync();

    }

    public async Task<ApiBaseResponse> GetAllCompaniesAsync(bool trackChanges)
    {
        var companies = await repository.Companies.GetAllCompaniesAsync(trackChanges);

        var companiesDto = _mapper.ToCompanyDto(companies);

        return new ApiOkResponse<IEnumerable<CompanyDto>>(companiesDto);
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

    public async Task<ApiBaseResponse> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await GetCompany(companyId, trackChanges);

        if (company is null)
            return new CompanyNotFoundResponse(companyId);

        var companyDto = _mapper.ToCompanyDto(company);

        return new ApiOkResponse<CompanyDto>(companyDto);
    }

    public async Task<(CompanyForUpdateDto CompanyToPatch, Company companyEntity)> GetCompanyForPatchAsync(Guid companyId, bool trackChanges)
    {
        var companyEntity = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

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
        var companyEntity = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

        _mapper.UpdateCompany(companyForUpdate, companyEntity);
        await repository.SaveAsync();
    }

    private async Task<Company> GetCompanyAndCheckIfItExists(Guid companyId, bool trackChanges)
        => await GetCompany(companyId, trackChanges)
        ?? throw new CompanyNotFoundException(companyId);

    private async Task<Company?> GetCompany(Guid companyId, bool trackChanges)
        => await repository.Companies.GetCompanyAsync(companyId, trackChanges);
}