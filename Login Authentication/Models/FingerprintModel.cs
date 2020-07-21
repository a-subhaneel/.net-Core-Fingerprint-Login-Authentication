using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace net_core_fingerprint_authorization.Models
{
    public class FingerprintModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ScannedID { get; set; }

        [Required(ErrorMessage = "Please provide a password.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please provide a password.")]
        public string EmailID { get; set; }

        [Required(ErrorMessage = "Please provide a password.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password and Confirmation Password must match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [NotMapped]
        public string ComparePassword { get; set; }

        [MaxLength(800)]
        [Required(ErrorMessage = "Please scan your finger!")]
        public string TemplateFormatDB { get; set; }

        //[Required(ErrorMessage = "Must provide fingerprint!")]
        //public string FingerprintImage { get; set; }
    }

}
