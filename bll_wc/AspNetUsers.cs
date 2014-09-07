using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
	public class AspNetUsers
	{
		[Key]
		public string Id { get; set; }

		[Required]
		public string Email { get; set; }
		public bool EmailConfirmed { get; set; }
		public string Password { get; set; }
		public string PhoneNumber { get; set; }
		public bool TwoFactorEnabled { get; set; }
		public bool LockoutEnabled { get; set; }
		public DateTime LockoutEndDateUtc { get; set; }
		public int AccessFailedCount { get; set; }
		public string UserName { get; set; }
	}
}
