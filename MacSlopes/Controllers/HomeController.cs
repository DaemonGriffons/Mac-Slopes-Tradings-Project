using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MacSlopes.Models;
using ClientNotifications;
using ClientNotifications.Helpers;
using static ClientNotifications.Helpers.NotificationHelper;

namespace MacSlopes.Controllers
{
    public class HomeController : Controller
    {
        private IClientNotification _notification;

        public HomeController(IClientNotification notification)
        {
            _notification = notification;
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
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
