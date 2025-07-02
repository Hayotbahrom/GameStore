// <copyright file="HomeController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.MVC.Controllers
{
    using System.Diagnostics;
    using GameStore.MVC.Models;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller for handling home page and privacy page requests.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Handles requests to the home page of the application.
        /// </summary>
        /// <returns>Index view. </returns>
        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        ///  Renders the privacy page of the application.
        /// </summary>
        /// <returns>Privacy view. </returns>
        public IActionResult Privacy()
        {
            return this.View();
        }

        /// <summary>
        /// Renders the error page with request id.
        /// </summary>
        /// <returns>Error view with <see cref="ErrorViewModel"/>.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
