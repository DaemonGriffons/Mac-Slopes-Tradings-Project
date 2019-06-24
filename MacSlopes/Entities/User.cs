using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Entities
{
    public class User : IdentityUser
    {
        [Required,MaxLength(256)]
        [ProtectedPersonalData]
        public string Name { get; set; }

        [Required, MaxLength(256)]
        [ProtectedPersonalData]
        public string Surname { get; set; }

        [MaxLength(1024)]
        [ProtectedPersonalData]
        public string ImageUrl { get; set; }

        [ProtectedPersonalData]
        public DateTime DateRegistered { get; set; }

    }
}
