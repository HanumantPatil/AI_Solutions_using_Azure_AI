using Agents.Application;
using Agents.Application.Interfaces;
using Agents.Infrastructure.Identity;
using Agents.Infrastructure.Identity.Contexts;
using Agents.Infrastructure.Identity.Models;
using Agents.Infrastructure.Identity.Seeds;
using Agents.Infrastructure.Persistence;
using Agents.Infrastructure.Persistence.Contexts;
using Agents.Infrastructure.Persistence.Seeds;
using Agents.Infrastructure.Resources;
using Agents.WebApi.Infrastructure.Extensions;
using Agents.WebApi.Infrastructure.Middlewares;
using Agents.WebApi.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

bool useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfrastructure(builder.Configuration, useInMemoryDatabase);
builder.Services.AddIdentityInfrastructure(builder.Configuration, useInMemoryDatabase);
builder.Services.AddResourcesInfrastructure();
builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
builder.Services.AddMediator();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCustomSwagger();
builder.Services.AddAnyCors();
builder.Services.AddAuthorization();
builder.Services.AddCustomLocalization(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    if (!useInMemoryDatabase)
    {
        await services.GetRequiredService<IdentityContext>().Database.MigrateAsync();
        await services.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
    }

    //Seed Data
    await DefaultRoles.SeedAsync(services.GetRequiredService<RoleManager<ApplicationRole>>());
    await DefaultBasicUser.SeedAsync(services.GetRequiredService<UserManager<ApplicationUser>>());
    await DefaultData.SeedAsync(services.GetRequiredService<ApplicationDbContext>());
}

app.UseCustomLocalization();
app.UseAnyCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCustomSwagger();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHealthChecks("/health");
app.MapEndpoints();
app.UseSerilogRequestLogging();

app.Run();

public partial class Program
{
}
