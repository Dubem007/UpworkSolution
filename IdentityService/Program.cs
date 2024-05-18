using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using IdentityService.Extensions;
using IdentityService.IDConfigurations;
using IdentityService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly.GetName().Name;

builder.Services.AddScoped<IProfileService, CustomProfileService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.ConfigureSQLserver(builder.Configuration, assembly);

builder.Services.ConfigureIdentity();
builder.Services.ConfigureIdentityServer(builder.Configuration, assembly);

var app = builder.Build();
#region Initialized Database
//using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
//{
//    serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

//    var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
//    context.Database.Migrate();
//    if (!context.Clients.Any())
//    {
//        foreach (var client in Config.GetClients())
//        {
//            context.Clients.Add(client.ToEntity());
//        }
//        context.SaveChanges();
//    }

//    if (!context.IdentityResources.Any())
//    {
//        foreach (var resource in Config.GetIdentityResources())
//        {
//            context.IdentityResources.Add(resource.ToEntity());
//        }
//        context.SaveChanges();
//    }

//    if (!context.ApiScopes.Any())
//    {
//        foreach (var resource in Config.ApiScopes.ToList())
//        {
//            context.ApiScopes.Add(resource.ToEntity());
//        }

//        context.SaveChanges();
//    }

//    if (!context.ApiResources.Any())
//    {
//        foreach (var resource in Config.GetApis())
//        {
//            context.ApiResources.Add(resource.ToEntity());
//        }
//        context.SaveChanges();
//    }

//};
#endregion
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseIdentityServer();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
