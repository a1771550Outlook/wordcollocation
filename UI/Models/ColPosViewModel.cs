using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using UI.Models.Abstract;

namespace UI.Models
{
	public class ColPosViewModel:ViewModelBase
	{
		private readonly string id;
		public ColPosViewModel(string id = null)
		{
			this.id = id;
		}
		public IEnumerable<ColPos> ColPosList
		{
			get { return new ColPosRepository().GetList(); }
		}
		public List<SelectListItem> ColPosDropDownList
		{
			get
			{
				return CreateDropDownList(WcEntity.ColPos,id);
			}
		}
	}
}