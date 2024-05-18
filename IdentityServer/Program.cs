using IdentityServer;
using IdentityServer.Data;
using IdentityServer.IdentityConfiguration;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbConstring = Environment.GetEnvironmentVariable(builder.Configuration.GetConnectionString("Default") ?? string.Empty,
                              EnvironmentVariableTarget.Process) ?? builder.Configuration.GetValue<string>("ConnectionStrings:Default");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(dbConstring));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
        .AddInMemoryClients(Clients.Get())
        .AddInMemoryIdentityResources(Resources.GetIdentityResources())
        .AddInMemoryApiResources(Resources.GetApiResources())
        .AddInMemoryApiScopes(Scopes.GetApiScopes())
        .AddAspNetIdentity<ApplicationUser>()
        .AddInMemoryPersistedGrants()
        .AddDeveloperSigningCredential();


builder.Services.AddControllersWithViews();
var app = builder.Build();

try { app.SeedRoleData().Wait(); } catch (Exception ex) { Console.WriteLine(ex.Message, ex); }

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

//app.MapGet("/", () => "Hello World!");

app.Run();