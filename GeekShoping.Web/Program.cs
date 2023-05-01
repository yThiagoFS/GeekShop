using GeekShoping.Web.Services;
using GeekShoping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultScheme = "Cookies";
        opts.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies", c => c.ExpireTimeSpan = TimeSpan.FromMinutes(10))
    .AddOpenIdConnect("oidc", opts => {
        opts.Authority = builder.Configuration.GetSection("ServicesUrls")["IdentityServer"];
        opts.GetClaimsFromUserInfoEndpoint = true;
        opts.ClientId = "geek_shopping";
        opts.ClientSecret = builder.Configuration["GeekShoppingSecret"];
        opts.ResponseType = "code";
        opts.ClaimActions.MapJsonKey("role", "role", "role");
        opts.ClaimActions.MapJsonKey("sub", "sub", "sub");
        opts.TokenValidationParameters.NameClaimType = "name";
        opts.TokenValidationParameters.RoleClaimType = "role";
        opts.Scope.Add("geek_shopping");
        opts.SaveTokens = true;
    });

var teste = builder.Configuration["GeekShoppingSecret"];

builder.Services.AddHttpClient<IProductService, ProductService>(c => c.BaseAddress 
    = new Uri(builder.Configuration.GetSection("ServicesUrls")["ProductAPI"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
