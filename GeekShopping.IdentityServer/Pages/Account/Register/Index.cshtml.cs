using Duende.IdentityServer.Events;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer;
using foo.Pages;
using GeekShopping.IdentityServer.Model;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static foo.Pages.Login.ViewModel;
using System.Security.Claims;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeekShopping.IdentityServer.Pages.Account.Register
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        public IndexModel(UserManager<ApplicationUser> userManager
            ,SignInManager<ApplicationUser> signInManager
            ,RoleManager<IdentityRole> roleManager
            ,IIdentityServerInteractionService interaction
            ,IClientStore clientStore
            ,IAuthenticationSchemeProvider schemeProvider
            ,IIdentityProviderStore identityProviderStore 
            ,IEventService events)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        public async Task<IActionResult> OnGet(string returnUrl)
        {
            var vm = await BuildRegisterViewModelAsync(returnUrl);

            return Page();
        }
        [BindProperty]
        public InputRegisterViewModel Input { get; set; }

        public RegisterViewModel ViewModel { get; set; }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
       
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    EmailConfirmed = true,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(Input.RoleName).GetAwaiter().GetResult())
                    {
                        var userRole = new IdentityRole
                        {
                            Name = Input.RoleName,
                            NormalizedName = Input.RoleName,

                        };
                        await _roleManager.CreateAsync(userRole);
                    }

                    await _userManager.AddToRoleAsync(user, Input.RoleName);

                    await _userManager.AddClaimsAsync(user, new Claim[]{
                    new Claim(JwtClaimTypes.Name, Input.Username),
                    new Claim(JwtClaimTypes.Email, Input.Email),
                    new Claim(JwtClaimTypes.FamilyName, Input.FirstName),
                        new Claim(JwtClaimTypes.GivenName, Input.LastName),
                    new Claim(JwtClaimTypes.WebSite, $"http://{Input.Username}.com"),
                    new Claim(JwtClaimTypes.Role,"User") });

                    var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

                    var loginresult = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, lockoutOnFailure: true);

                    if (loginresult.Succeeded)
                    {
                        var checkuser = await _userManager.FindByNameAsync(Input.Username);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(checkuser.UserName, checkuser.Id, checkuser.UserName, clientId: context?.Client.ClientId));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                // The client is native, so this change in how to
                                // return the response is for better UX for the end user.
                                return this.LoadingPage(returnUrl);
                            }

                            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                            return Redirect(returnUrl);
                        }

                        // request for a local page
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else if (string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect("~/");
                        }
                        else
                        {
                            // user might have clicked on a malicious link - should be logged
                            throw new Exception("invalid return URL");
                        }
                    }

                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<RegisterViewModel> BuildRegisterViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            List<SelectListItem> roles = new()
            {
                 new SelectListItem {Value = "Admin", Text = "Admin" },
                 new SelectListItem {Value = "Client", Text = "Client" },
            };

            ViewData["message"] = roles;

            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new RegisterViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider
            {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new RegisterViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

    }
}
