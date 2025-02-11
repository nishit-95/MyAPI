using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class t_Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int c_ContactId { get; set; }

        public int c_UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string c_ContactName { get; set; }

        [Required]
        [StringLength(100)]
        public string c_Email { get; set; }


        [StringLength(50)]
        public string? c_Group { get; set; }

        [StringLength(500)]
        public string? c_Address { get; set; }

        [StringLength(50)]
        public string? c_Mobile { get; set; }

        [StringLength(4000)]
        public string? c_Image { get; set; }

        [StringLength(20)]
        public string? c_Status { get; set; }

        public IFormFile? ContactPicture { get; set; }
    }
}

