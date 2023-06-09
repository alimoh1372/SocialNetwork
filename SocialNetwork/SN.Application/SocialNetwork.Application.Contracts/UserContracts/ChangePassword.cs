﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Application.Contracts.UserContracts
{
    /// <summary>
    /// A Data transfer object to map client inputs to change user password
    /// </summary>
    public class ChangePassword
    {
        public long Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(30)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [StringLength(30)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}