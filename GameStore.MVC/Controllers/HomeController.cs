// <copyright file="HomeController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Diagnostics;
using GameStore.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.MVC.Controllers;

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
        return View();
    }

    /// <summary>
    ///  Renders the privacy page of the application.
    /// </summary>
    /// <returns>Privacy view. </returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Renders the error page with request id.
    /// </summary>
    /// <returns>Error view with <see cref="ErrorViewModel"/>.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
