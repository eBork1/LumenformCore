using Lumenform.Application.Interfaces;
using Lumenform.Application.Repositories;
using Lumenform.Application.Services;
using Lumenform.Infrastructure.Persistence;
using Lumenform.Infrastructure.Persistence.Repositories;
using Lumenform.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lumenform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Add repositories here
        services.AddScoped<ICohortRepository, CohortRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        
        // Add services here
        services.AddScoped<CohortService>();
        services.AddScoped<CohortMemberService>();
        services.AddScoped<CohortEventService>();
        services.AddScoped<AssignmentService>();
        services.AddScoped<ICohortAuthorizationService, CohortAuthorizationService>();
        services.AddHttpClient<ISupabaseUserService, SupabaseUserService>();
        
        return services;
    }
}