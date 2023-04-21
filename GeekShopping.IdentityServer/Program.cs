using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Initializer;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SQLContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("GeekShopIdentityServer")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<SQLContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

var securityConfig = builder.Services.AddIdentityServer(opts => 
        {
            opts.Events.RaiseErrorEvents = true;
            opts.Events.RaiseInformationEvents = true;
            opts.Events.RaiseFailureEvents = true;
            opts.Events.RaiseSuccessEvents= true;
            opts.EmitStaticAudienceClaim = true;
        })
        .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
        .AddInMemoryClients(IdentityConfiguration.Clients(builder.Configuration))
        .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
        .AddAspNetIdentity<ApplicationUser>();

builder.Services.AddScoped<IDBInitializer, DBInitializer>();

securityConfig.AddDeveloperSigningCredential();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.Services
    .CreateScope()
    .ServiceProvider.GetRequiredService<IDBInitializer>()
    .Initialize();

app.MapRazorPages();

app.Run();
