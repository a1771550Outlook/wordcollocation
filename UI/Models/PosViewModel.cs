using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using UI.Models.Abstract;

namespace UI.Models
{
	public class PosViewModel:ViewModelBase
	{
		private readonly string id;
		public PosViewModel(string id = null)
		{
			this.id = id;
		}
		public IEnumerable<Pos> PosList
		{
			get { return new PosRepository().GetList(); }
		}
		public List<SelectListItem> PosDropDownList
		{
			get
			{
				return CreateDropDownList(WcEntity.Pos,id);
			}
		}

	}
}