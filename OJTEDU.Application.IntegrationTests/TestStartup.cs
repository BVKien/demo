using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Infrastructure.Data;
using System;

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register your DbContext with an in-memory database for testing
        services.AddDbContext<OJTEDU_DB_V1Context>(options =>
            options.UseInMemoryDatabase("TestDatabase"));

        // Register the services you want to test (UserService, RoleService)
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        // Register any other dependencies if needed
        services.AddLogging();
    }
}
