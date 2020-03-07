using ClientNotifications;
using ClientNotifications.Helpers;
using MacSlopes.Entities;
using MacSlopes.Extensions;
using MacSlopes.Models.AccountViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MacSlopes.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IClientNotification _notification;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, 
                                SignInManager<User> signInManager,
                                RoleManager<IdentityRole> roleManager,
                                IClientNotification notification,
                                IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _notification = notification;
            _notification = notification;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl=null)
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model,string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                _notification.AddToastNotification("Something Went Wrong",
                    NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-right",
                        PreventDuplicates = true
                    });
                return View(model);
            }

            //TODO: Check if the user has Confirmed their Email 
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                _notification.AddToastNotification("Something Went Wrong",
                   NotificationHelper.NotificationType.error, new ToastNotificationOption
                   {
                       NewestOnTop = true,
                       CloseButton = true,
                       PositionClass = "toast-bottom-right",
                       PreventDuplicates = true
                   });
                return View(model);
            }
            //      Before Signing them In

            if (user.EmailConfirmed==false)
            {
                return RedirectToAction(nameof(UnConfirmed));
            }


            var result = await _signInManager
                .PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            if (result.IsNotAllowed)
            {
                return RedirectToAction(nameof(AccessDenied));
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(Login2Factor));
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(LockedOut));
            }
            else
            {
                _notification.AddToastNotification("Invalid Login Attempt",
                    NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-bottom-right",
                        PreventDuplicates = true
                    });
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult UnConfirmed() => View();


        [HttpGet]
        public async Task<IActionResult> Login2Factor(bool RememberMe, string returnUrl)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user==null)
            {
                _notification.AddToastNotification("Could not load 2-Factor Authentication for you",
                    NotificationHelper.NotificationType.error,new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-right",
                    PreventDuplicates = true
                });
                return View();
            }

            var model = new Login2FAViewModel
            {
                RememberMe = RememberMe
            };
            ViewData["returnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login2Factor(Login2FAViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    _notification.AddToastNotification("No Such User Exists!", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-right",
                        PreventDuplicates = true
                    });
                    return View(model);
                }

                var authcode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authcode, model.RememberMe, model.RememberMachine);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction(nameof(LockedOut));
                }
                if (result.IsNotAllowed)
                {
                    return RedirectToAction(nameof(AccessDenied));
                }
            }
            _notification.AddToastNotification("You have Made Errors", NotificationHelper.NotificationType.error, new ToastNotificationOption
            {
                NewestOnTop = true,
                CloseButton = true,
                PositionClass = "toast-top-right",
                PreventDuplicates = true
            });
            return View(model);
            
        }

        [HttpGet]
        public async Task<IActionResult> LoginWithRecoverCode(string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user==null)
            {
                _notification.AddToastNotification($"Unable To Load 2-Factor Authentication user",
                    NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-right",
                        PreventDuplicates = true
                    });
            }

            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoverCodeViewModel model,
            string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    _notification.AddToastNotification($"Unable To Load 2-Factor Authentication user",
                        NotificationHelper.NotificationType.error, new ToastNotificationOption
                        {
                            NewestOnTop = true,
                            CloseButton = true,
                            PositionClass = "toast-top-right",
                            PreventDuplicates = true
                        });
                }

                var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);
                var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction(nameof(LockedOut));
                }
                else
                {
                    _notification.AddToastNotification("Invalid recovery code entered.", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-right",
                        PreventDuplicates = true
                    });
                    return View();
                }

            }
            _notification.AddToastNotification("You have Made Errors", NotificationHelper.NotificationType.error, new ToastNotificationOption
            {
                NewestOnTop = true,
                CloseButton = true,
                PositionClass = "toast-top-right",
                PreventDuplicates = true
            });
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl=null)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.Username,
                    PhoneNumber = model.Phone,
                    Email = model.Email,
                    DateRegistered = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var roleExists = await _roleManager.RoleExistsAsync("Member");
                    if (!roleExists)
                    {
                        var role = new IdentityRole("Member");
                        var roleresult = await _roleManager.CreateAsync(role);
                        if (roleresult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, role.Name);
                        }

                        Errors(roleresult);
                    }
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendMail(model.Email, "User account confirmation",
                        $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>Confirm Your Account</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
                Errors(result);
                return View(model);
            }
            _notification.AddToastNotification("Something Went Wrong",
                NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-right",
                    PreventDuplicates = true
                });
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _notification.AddToastNotification($"No Such User Exists",
                   NotificationHelper.NotificationType.error, new ToastNotificationOption
                   {
                       NewestOnTop = true,
                       CloseButton = true,
                       PositionClass = "toast-top-right",
                       PreventDuplicates = true
                   });
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View(nameof(ConfirmEmail));
            }
            return View("Error");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendMail(model.Email, "Reset Your Password", $" Please Reset Your Password By Clicking Here <a href='{UrlEncoder.Default.Encode(callbackUrl)}'>Reset Password</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            _notification.AddToastNotification("Something Went Wrong",
                NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-right",
                    PreventDuplicates = true
                });
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code=null)
        {
            if (code==null)
            {
                _notification.AddToastNotification("A code Must Be supplied for Password Reset",
                    NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-right",
                        PreventDuplicates = true
                    });
            }

            var model = new ResetPasswordViewModel
            {
                Code = code
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                Errors(result);
            }
            _notification.AddToastNotification($"Something Went Wrong",
                NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-right",
                    PreventDuplicates = true
                });
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation() => View();

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LockedOut()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [AcceptVerbs("POST", "GET")]
        public IActionResult ValidateUsername(string username)
        {
            if (_userManager.Users.Any(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                return Json($"The Username named {username} already exists.try another one");
            }
            return Json(true);
        }

        #region Helper Methods

        public IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return RedirectToLocal(returnUrl);
            else return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void Errors(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                _notification.AddToastNotification($"{item.Description}",
                    NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-right",
                        PreventDuplicates = true
                    });
            }
        }
        #endregion
    }
}