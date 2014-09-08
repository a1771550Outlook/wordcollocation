using System.Text.RegularExpressions;
using System.Web.Services;
using BLL;

namespace UI.WebServices
{
	/// <summary>
	/// Summary description for WcServices
	/// </summary>
	[WebService(Namespace = "http://www.translationhall.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class WcServices : WebService
	{
		[WebMethod]
		public bool[] CheckEmail(string email)
		{
			bool[] bRet = new bool[2];
			var repo = new AspNetUserRepository();
			if (repo.CheckIfDuplicatedEmail(email)) bRet[0] = false;
			else bRet[0] = true;

			const string pattern = @"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})";
			Regex regex=new Regex(pattern, RegexOptions.IgnoreCase);
			if (!regex.IsMatch(email))
				bRet[1] = false;
			else bRet[1] = true;
			return bRet;
		}
	}
}
