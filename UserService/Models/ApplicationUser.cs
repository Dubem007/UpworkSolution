﻿using Common.Extensions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserServices.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string RoleId { get; set; }
        public string? SecretKey { get; set; }
        public string? SubjectId { get; set; }
        public string CountryCode { get; set; }
        public bool SendHelpfulEmails { get; set; }
        public bool HasReadPolicy { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? IsDeleted { get; set; } = false;
        public UserProfile MyProperty { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string OperationBy { get; set; } = AppConstants.AppSystem;
    }
}
