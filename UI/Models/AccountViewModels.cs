using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using Microsoft.VisualBasic;
using WcResources;
using WebGrease;
using WebGrease.Css.Visitor;

namespace UI.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

	public class SendCodeViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
		public string ReturnUrl { get; set; }
	}

	public class VerifyCodeViewModel
	{
		[Required]
		public string Provider { get; set; }

		[Required]
		[Display(Name = "Code")]
		public string Code { get; set; }
		public string ReturnUrl { get; set; }

		[Display(Name = "Remember this browser?")]
		public bool RememberBrowser { get; set; }
	}

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class LoginViewModel
	{
		[Required(ErrorMessageResourceName = "UserNameEmailRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
		[Display(Name = "UserNameEmailforLogin", ResourceType = typeof(Resources))]
		//[EmailAddress(ErrorMessageResourceName = "EmailFormatError", ErrorMessageResourceType = typeof(Resources))]
		public string UserNameEmail { get; set; }

		[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Resources))]
		public string Password { get; set; }

		[Display(Name = "RememberMe", ResourceType = typeof(Resources))]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		[Required(ErrorMessageResourceName = "UserNameRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
		[Display(Name = "UserName", ResourceType = typeof(Resources))]
		public string UserName { get; set; }

		[Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(Resources),ErrorMessage=null)]
		[EmailAddress(ErrorMessageResourceName = "EmailFormatError",ErrorMessageResourceType = typeof(Resources), ErrorMessage=null)]
		[Display(Name = "Email", ResourceType = typeof(Resources))]
		public string Email { get; set; }

		[Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
		[StringLength(20, ErrorMessageResourceName = "PasswordLengthError", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null, MinimumLength = 6)]
		//[MaxLength(ErrorMessageResourceName = "MaxPasswordLengthError", ErrorMessageResourceType = typeof(Resources))]
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(Resources))]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "ConfirmPassword", ResourceType = typeof(Resources))]
		[Compare("Password", ErrorMessageResourceName = "ConfirmPasswordError", ErrorMessageResourceType = typeof(Resources), ErrorMessage = null)]
		public string ConfirmPassword { get; set; }
	}

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
