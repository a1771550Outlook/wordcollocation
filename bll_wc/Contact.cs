using System.ComponentModel.DataAnnotations;
using WcResources;

namespace BLL
{
	public class Contact
	{
		[Display(Name = "Username", ResourceType = typeof(Resources))]
		[Required(ErrorMessageResourceType = typeof(Resources),
				  ErrorMessageResourceName = "UsernameRequired")]
		public string Username { get; set; }

		[Display(Name = "Email", ResourceType = typeof(Resources))]
		[Required(ErrorMessageResourceType = typeof(Resources),
				  ErrorMessageResourceName = "EmailRequired")]
		public string Email { get; set; }

		[Display(Name = "Message", ResourceType = typeof(Resources))]
		[Required(ErrorMessageResourceType = typeof(Resources),
				  ErrorMessageResourceName = "MessageRequired")]
		public string Message { get; set; }
	}
}