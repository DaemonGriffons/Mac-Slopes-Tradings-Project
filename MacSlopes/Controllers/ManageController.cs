using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ClientNotifications;
using ClientNotifications.Helpers;
using MacSlopes.Entities;
using MacSlopes.Extensions;
using MacSlopes.Models.ManageViewModel;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MacSlopes.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IClientNotification _clientNotification;
        private readonly UrlEncoder _urlEncoder;
        private readonly IFileManager _fileManager;
        private PhotoSettings _options;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public ManageController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender, 
            IClientNotification clientNotification,
            UrlEncoder urlEncoder,
            IFileManager fileManager,
            IOptionsSnapshot<PhotoSettings> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _clientNotification = clientNotification;
            _urlEncoder = urlEncoder;
            _fileManager = fileManager;
            _options = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists",NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _clientNotification.AddToastNotification($"Unable to load user with ID '{_userManager.GetUserId(User)}'.", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                    return View(model);
                }

                var email = user.Email;
                if (model.Email != email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        _clientNotification.AddToastNotification($"Unexpected error occurred setting email for you", NotificationHelper.NotificationType.error, new ToastNotificationOption
                        {
                            NewestOnTop = true,
                            CloseButton = true,
                            PositionClass = "toast-top-full-width",
                            PreventDuplicates = true
                        });
                        return View(model);
                    }
                }

                var phoneNumber = user.PhoneNumber;
                if (model.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        _clientNotification.AddToastNotification($"Unexpected error occurred setting phone number for you", NotificationHelper.NotificationType.error, new ToastNotificationOption
                        {
                            NewestOnTop = true,
                            CloseButton = true,
                            PositionClass = "toast-top-full-width",
                            PreventDuplicates = true
                        });
                        return View(model);
                    }
                }
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user==null)
                {
                    _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                var email = user.Email;
                await _emailSender.SendMail(email, "Confirm Your Account",
                    $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _clientNotification.AddToastNotification("You Have Made Errors", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            var haspassword = await _userManager.HasPasswordAsync(user);
            if (!haspassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                }

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    Errors(result);
                    return View(model);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(ChangePassword));
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                }

                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!result.Succeeded)
                {
                    Errors(result);
                    return View(model);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(SetPassword));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2FAWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            if (!user.TwoFactorEnabled)
            {
                _clientNotification.AddToastNotification("Unexpected Error occured disabling 2-factor Auth for You", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            return View(nameof(Disable2FA));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2FA()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                _clientNotification.AddToastNotification("Unexpected Error occured disabling 2-factor Auth for You", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            var model=new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                _clientNotification.AddToastNotification("Verification code is invalid.", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new ShowRecoveryCodesViewModel
            {
                RecoveryCodes = recoveryCodes
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            if (!user.TwoFactorEnabled)
            {
                _clientNotification.AddToastNotification("Cannot generate recovery codes for you because you have disabled Two-factor Authentication", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            return View(nameof(GenerateRecoveryCodes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-right",
                    PreventDuplicates = true
                });
            }
            if (!user.TwoFactorEnabled)
            {
                _clientNotification.AddToastNotification("Cannot generate recovery codes for you because you have disabled Two-factor Authentication", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };
            return View(nameof(ShowRecoveryCodes), model);
        }


        [HttpGet]
        public async Task<IActionResult> Photos()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user==null)
            {
                _clientNotification.AddToastNotification("No Such User Exists", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
                return View();
            }

            var model = new PhotoUploadViewModel
            {
                Thumbnail = user.ImageUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Photos(PhotoUploadViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _clientNotification.AddToastNotification("No Such User Exists",
                        NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                    return View(model);
                }
                if (model.Image==null)
                {
                    _clientNotification.AddToastNotification("You Have to Upload An Image!",
                        NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                    return View(model);
                }
                else
                {
                    if (model.Image.Length> _options.MaxBytes)
                    {
                        _clientNotification.AddToastNotification("Image File Size Exceeded",
                            NotificationHelper.NotificationType.error, new ToastNotificationOption
                        {
                            NewestOnTop = true,
                            CloseButton = true,
                            PositionClass = "toast-top-full-width",
                            PreventDuplicates = true
                        });
                        return View(model);
                    }

                    if (!_options.IsSupported(model.Image.FileName))
                    {
                        _clientNotification.AddToastNotification("Invalid File Type",
                            NotificationHelper.NotificationType.error, new ToastNotificationOption
                        {
                            NewestOnTop = true,
                            CloseButton = true,
                            PositionClass = "toast-top-full-width",
                            PreventDuplicates = true
                        });
                        return View(model);
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(user.ImageUrl))
                        {
                            _fileManager.RemoveImage(user.ImageUrl);
                            user.ImageUrl = _fileManager.SaveImage(model.Image);
                            var res = await _userManager.UpdateAsync(user);
                            if (res.Succeeded)
                            {
                                _clientNotification.AddToastNotification("Image Successfully Uploaded", NotificationHelper.NotificationType.success,
                                    new ToastNotificationOption
                                    {
                                        NewestOnTop = true,
                                        CloseButton = true,
                                        PositionClass = "toast-top-full-width",
                                        PreventDuplicates = true
                                    });
                                return RedirectToAction(nameof(Photos));
                            }
                        }
                        user.ImageUrl = _fileManager.SaveImage(model.Image);
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            _clientNotification.AddToastNotification("Image Successfully Uploaded", NotificationHelper.NotificationType.success,
                                new ToastNotificationOption
                                {
                                    NewestOnTop = true,
                                    CloseButton = true,
                                    PositionClass = "toast-top-full-width",
                                    PreventDuplicates = true
                                });
                            return RedirectToAction(nameof(Photos));
                        }
                        Errors(result);
                        return View(model);
                    }
                }
            }

            _clientNotification.AddToastNotification("You have made Errors", NotificationHelper.NotificationType.error,
                new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            return View(model);
        }

        #region Helpers
        private void Errors(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                _clientNotification.AddToastNotification($"{item.Description}",
                    NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
            }
        }


        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Mac Slopes Trading"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(User user, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        #endregion
    }
}