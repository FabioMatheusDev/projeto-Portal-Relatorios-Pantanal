using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PortalRelatorios.Application.Abstractions;
using PortalRelatorios.Application.Configuration;
using PortalRelatorios.Application.Services;
using PortalRelatorios.Domain.Repositories;
using PortalRelatorios.Infrastructure.Identity;
using PortalRelatorios.Infrastructure.Persistence;
using PortalRelatorios.Infrastructure.Persistence.Repositories;
using PortalRelatorios.Infrastructure.Sap;
using Sap.EntityFrameworkCore.Hana;

namespace PortalRelatorios.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<LdapOptions>(configuration.GetSection(LdapOptions.SectionName));
        services.Configure<SapServiceLayerOptions>(configuration.GetSection(SapServiceLayerOptions.SectionName));
        services.Configure<ReportEndpointOptions>(configuration.GetSection(ReportEndpointOptions.SectionName));

        var db = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.AddDbContext<AppDbContext>(options =>
        {
            if (db.UseInMemoryDatabase)
            {
                options.UseInMemoryDatabase("PortalRelatorios");
            }
            else
            {
                var cs = db.HanaConnectionString
                           ?? throw new InvalidOperationException(
                               "Configure Database:HanaConnectionString ou Database:UseInMemoryDatabase=true.");
                options.UseHana(cs);
            }
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISectorRepository, SectorRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IActiveDirectoryAuthService, ActiveDirectoryAuthService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services.AddSingleton<SapCookieContainer>();
        services.AddSingleton<SapHttpHandler>();
        services.AddHttpClient<ISapServiceLayerService, SapServiceLayerService>()
            .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<SapHttpHandler>())
            .ConfigureHttpClient((sp, client) =>
            {
                var opt = sp.GetRequiredService<IOptions<SapServiceLayerOptions>>().Value;
                var baseUrl = opt.BaseUrl.TrimEnd('/') + "/";
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromMinutes(5);
            });

        services.AddScoped<IAuthAppService, AuthAppService>();
        services.AddScoped<IPermissionAppService, PermissionAppService>();

        return services;
    }
}
