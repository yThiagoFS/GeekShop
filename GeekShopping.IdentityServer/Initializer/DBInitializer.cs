using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.IdentityServer.Initializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly SQLContext _sqlContext;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(SQLContext context
            ,UserManager<ApplicationUser> userManager
            ,RoleManager<IdentityRole> roleManager)
        {
            _sqlContext = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            if (_roleManager.FindByNameAsync(IdentityConfiguration.Admin).Result != null) return;

            await _roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Admin));

            await _roleManager.CreateAsync(new IdentityRole(IdentityConfiguration.Client));

            ApplicationUser admin = new ApplicationUser() 
            { 
                UserName = "thiago-admin",
                Email =  "thiagoadmin@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (11) 12345-6789",
                FirstName = "Thiago",
                LastName = "Admin"
                
            };

            await _userManager.CreateAsync(admin, "Thiago123$");
            await _userManager.AddToRoleAsync(admin, IdentityConfiguration.Admin);

            var adminClaims = await _userManager.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
                new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName, admin.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)
            }) ; 
            
            ApplicationUser client = new ApplicationUser() 
            { 
                UserName = "thiago-client",
                Email =  "thiagoclient@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (11) 12345-6789",
                FirstName = "Thiago",
                LastName = "Client"
                
            };

            await _userManager.CreateAsync(client, "Thiago123$");
            await _userManager.AddToRoleAsync(client, IdentityConfiguration.Client);

            var clientClaims = await _userManager.AddClaimsAsync(client, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
                new Claim(JwtClaimTypes.GivenName, client.FirstName),
                new Claim(JwtClaimTypes.FamilyName, client.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)
            }) ;

        }
    }
}
