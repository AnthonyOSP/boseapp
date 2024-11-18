// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using boseapp.Models;

namespace boseapp.Areas.Identity.Pages.Account
{
    public class LoginRegisterModel : PageModel
    {
        private readonly SignInManager<UserCliente> _signInManager;
        private readonly UserManager<UserCliente> _userManager;
        private readonly IUserStore<UserCliente> _userStore;
        private readonly IUserEmailStore<UserCliente> _emailStore;
        private readonly ILogger<LoginRegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public LoginRegisterModel(UserManager<UserCliente> userManager,
            IUserStore<UserCliente> userStore, SignInManager<UserCliente> signInManager, ILogger<LoginRegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        [BindProperty]
        public InputLoginModel InputLogin { get; set; }
        [BindProperty]
        public InputRegisterModel InputRegister { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public class InputLoginModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public class InputRegisterModel
        {
            [Required(ErrorMessage = "El campo Email es obligatorio")]
            [EmailAddress(ErrorMessage = "El formato del email no es válido")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "El campo Password es obligatorio")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "El campo Nombre es obligatorio")]
            [StringLength(50, ErrorMessage = "El nombre no puede tener más de {1} caracteres.")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "El campo Apellido es obligatorio")]
            [StringLength(50, ErrorMessage = "El apellido no puede tener más de {1} caracteres.")]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string action, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (action == "login")
            {
                ModelState.Clear();
                if (ModelState.IsValid && TryValidateModel(InputLogin, nameof(InputLogin)))
                {
                    var result = await _signInManager.PasswordSignInAsync(InputLogin.Email, InputLogin.Password, InputLogin.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = InputLogin.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
            }
            else if (action == "register")
            {
                _logger.LogInformation("Starting registration process");
                ModelState.Clear();
                if (ModelState.IsValid && TryValidateModel(InputRegister, nameof(InputRegister)))
                {
                    _logger.LogInformation("Model state is valid");

                    var user = CreateUser();
                    try
                    {
                        await _userStore.SetUserNameAsync(user, InputRegister.Email, CancellationToken.None);
                        await _emailStore.SetEmailAsync(user, InputRegister.Email, CancellationToken.None);
                        user.Nombre = InputRegister.Nombre;
                        user.Apellido = InputRegister.Apellido;

                        _logger.LogInformation("Creating user with email: {Email}, Nombre: {Nombre}, Apellido: {Apellido}",
                InputRegister.Email,
                InputRegister.Nombre,
                InputRegister.Apellido);

                        var result = await _userManager.CreateAsync(user, InputRegister.Password);

                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User created successfully");
                            _logger.LogInformation("User created a new account with password.");

                            var userId = await _userManager.GetUserIdAsync(user);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                                protocol: Request.Scheme);

                            await _emailSender.SendEmailAsync(InputRegister.Email, "Confirm your email",
                                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {
                                return RedirectToPage("RegisterConfirmation", new { email = InputRegister.Email, returnUrl = returnUrl });
                            }
                            else
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception occurred during user creation");
                        ModelState.AddModelError(string.Empty, "Ha ocurrido un error durante el registro.");
                    }
                }
                else
                {
                    _logger.LogWarning("Model state is invalid. Errors: {Errors}",
            string.Join(", ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)));
                }
            }

            return Page();
        }

        private UserCliente CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UserCliente>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserCliente)}'. " +
                    $"Ensure that '{nameof(UserCliente)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<UserCliente> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<UserCliente>)_userStore;
        }
    }
}
