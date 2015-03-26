using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace StageBeheersTool.ViewModels
{
    public class AdminVm
    {
        [HiddenInput]
        public string Id { get; set; }
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }
        public bool HasChanged { get; set; }
    }
}