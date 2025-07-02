using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Application.DTOs.Platforms
{
    public class PlatformForResultDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the platform.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the type of platform.
        /// </summary>
        [Required]
        public string? Type { get; set; }
    }
}
