using System.Collections.Generic;
using System.Web.Mvc;
using WcResources;

namespace UI.Models.Abstract
{
	public abstract class WcManageViewModelBase
	{
		public virtual List<SelectListItem> SourceList
		{
			get
			{
				var sourceWeb = new SelectListItem
				{
					Text = Resources.Web,
					Value = "Web",
					Selected = true
				};
				var sourceCH = new SelectListItem
				{
					Text = Resources.CH,
					Value = "CH"
				};
				var sourceOxford = new SelectListItem
				{
					Text = Resources.Oxford,
					Value = "Oxford"
				};

				List<SelectListItem> sourceList = new List<SelectListItem>
				{
					sourceWeb,
					sourceCH,
					sourceOxford
				};
				return sourceList;
			}
		}
	}
}