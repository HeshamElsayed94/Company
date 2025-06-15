using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace CompanyEmployees.API;

public class ConfigureJsonPatchInputFormatter(
    ILoggerFactory loggerFactory,
    ObjectPoolProvider objectPoolProvider,
    ArrayPool<char> charPool,
    IOptions<MvcNewtonsoftJsonOptions> jsonOptions) : IConfigureOptions<MvcOptions>
{
    public void Configure(MvcOptions options)
    {
        var patchFormatter = new NewtonsoftJsonPatchInputFormatter(
            loggerFactory.CreateLogger<NewtonsoftJsonPatchInputFormatter>(),
            jsonOptions.Value.SerializerSettings,
            charPool,
            objectPoolProvider,
            new MvcOptions(), // empty because we only need it for base setup
            jsonOptions.Value
        );

        options.InputFormatters.Insert(0, patchFormatter);
    }
}