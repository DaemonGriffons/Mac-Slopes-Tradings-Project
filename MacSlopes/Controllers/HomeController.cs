using ClientNotifications;
using ClientNotifications.Helpers;
using MacSlopes.Models;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MacSlopes.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClientNotification _notification;
        private readonly IEmailSender _emailSender;

        public HomeController(IClientNotification notification, IEmailSender emailSender)
        {
            _notification = notification;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                _notification.AddToastNotification($"Welcome back {User.Identity.Name}!",
                    NotificationHelper.NotificationType.success, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-bottom-right",
                        PreventDuplicates = true
                    });
            }
            return View();
        }

        [ResponseCache(Location = ResponseCacheLocation.Client, NoStore =false)]
        public IActionResult About()
        {

            return View();
        }

        public IActionResult Contact()
        {

            return View();
        }


        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _emailSender.ContactEmail(model.Email, model.Name, model.Subject, model.Message);
                _notification.AddToastNotification("Message Sent Successfully",
                  NotificationHelper.NotificationType.success, new ToastNotificationOption
                  {
                      NewestOnTop = true,
                      CloseButton = true,
                      PositionClass = "toast-bottom-right",
                      PreventDuplicates = true
                  });
                return View();

            }
            _notification.AddToastNotification("You Have Made Errors",
                   NotificationHelper.NotificationType.error, new ToastNotificationOption
                   {
                       NewestOnTop = true,
                       CloseButton = true,
                       PositionClass = "toast-bottom-right",
                       PreventDuplicates = true
                   });
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
