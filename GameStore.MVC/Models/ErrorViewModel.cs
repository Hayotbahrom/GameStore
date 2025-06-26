// <copyright file="ErrorViewModel.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.MVC.Models
{
    /// <summary>
    /// ViewModel for displaying error information in the application.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the request that caused the error.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the request ID should be shown in the error view.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
    }
}
