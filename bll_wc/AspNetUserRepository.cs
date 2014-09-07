using DAL.IdentityDSTableAdapters;

namespace BLL
{
	public class AspNetUserRepository
	{
		private AspNetUsersTableAdapter adapter;

		protected AspNetUsersTableAdapter Adapter { get { return adapter ?? (adapter = new AspNetUsersTableAdapter()); } }

		public bool CheckIfDuplicatedEmail(string email)
		{
			var emailQuery = Adapter.EmailQuery(email);
			return emailQuery != null && (emailQuery.Value == 1);
		}
	}
}
