using System;
using BLL;

namespace UI
{
	public partial class CollocationWithExamples : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				var repo = new CollocationRepository();
				CollocationList.DataSource = repo.GetList();
				CollocationList.DataBind();
			}
		}
	}
}