using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using BLL;
using BLL.Helpers;
using UI.Controllers;

namespace UI.Models
{
	public enum CultureName
	{
		
	}
	public enum ViewMode
	{
		Admin,
		Home,
		SearchResult
	}
	public class WcSearchViewModel
	{
		public string ConnectionString { get; set; }

		private const string letters = "A;B;C;D;E;F;G;H;I;J;K;L;M;N;O;P;Q;R;S;T;U;V;W;X;Y;Z";
		public string[] Letters { get; set; }
		public CollocationPagingInfo CollocationPagingInfo { get; set; }

		public int PageSize = SiteConfiguration.WcColListPageSize;
		private readonly int page;
		private readonly string firstLetter;

		private readonly ViewMode mode;

		private string posJap, posZht, wordZht, wordJap, posZhs, wordZhs;
		private List<Collocation> _collocationList;

		public List<Collocation> CollocationList { get { return _collocationList; } }
		public string PosTrans { get; set; }
		public string WordTrans { get; set; }
		public string Word { get; set; }
		public string Pos { get; set; }
		public string ColPos { get; set; }
		public string ColPosId { get; set; }
		public string[] Pattern { get; set; }
		public string Action { get; set; }
		public string Controller { get; set; }
		public ViewMode ViewMode { get { return mode; } }

		public WcSearchViewModel()
		{ }
		public WcSearchViewModel(ViewMode mode)
			: this()
		{
			this.mode = mode;
		}

		public WcSearchViewModel(ViewMode mode, int page = 1)
			: this(mode)
		{
			this.page = page;
			GetCollocationList(true);
		}
		public WcSearchViewModel(ViewMode mode, string firstLetter = null, int page = 1)
			: this(mode, page)
		{
			this.firstLetter = !String.IsNullOrEmpty(firstLetter) ? firstLetter : "a";
			string[] lettterStrings = letters.Split(';');
			this.Letters = lettterStrings;
			//SetCollocationList(true);
		}

		private void SetPageInfo()
		{
			CollocationPagingInfo pagingInfo = new CollocationPagingInfo();
			pagingInfo.CurrentPage = page;
			pagingInfo.CollocationsPerPage = PageSize;

			if (_collocationList != null && _collocationList.Count > 0)
			{
				if (page >= 1)
				{
					pagingInfo.TotalCollocations = _collocationList.Count();
					this.CollocationPagingInfo = pagingInfo;
					_collocationList = _collocationList.Skip((page - 1) * PageSize).Take(PageSize).ToList();
				}
			}
		}

		private void GetConnectionStringForDebug()
		{
			var repo = new CollocationRepository();
			
		}

		private void GetCollocationList(bool setPageInfo = true)
		{
			var repo = new CollocationRepository();

			switch (mode)
			{
				case ViewMode.Admin:
					_collocationList = repo.GetCollocationListInGroup(firstLetter);
					Action = "Index";
					Controller = "Admin";
					break;
				case ViewMode.Home:
					//colList = repo.GetCollocationListByWordColPosId(word, Id);
					break;
				case ViewMode.SearchResult:
					_collocationList = (List<Collocation>)HttpContext.Current.Session[HomeController.CollocationListSessionName];
					var collocation = CollocationList[0];
					Word = collocation.Word;
					Pos = collocation.Pos;
					ColPos = collocation.ColPos;
					posJap = collocation.PosJap;
					posZht = collocation.PosZht;
					posZhs = collocation.PosZhs;
					wordZht = collocation.WordZht;
					wordZhs = collocation.WordZhs;
					wordJap = collocation.WordJap;
					Pattern = BLLHelper.GetPatternArray(collocation.CollocationPattern);
					var culturename = CultureHelper.GetCurrentCulture();
					if (culturename.Contains("hans"))
					{
						PosTrans = posZhs;
						WordTrans = wordZhs;
					}
					else if (culturename.Contains("ja"))
					{
						PosTrans = posJap;
						WordTrans = wordJap;
					}
					else
					{
						PosTrans = posZht;
						WordTrans = wordZht;
					}
					Action = "SearchResult";
					Controller = "Home";
					break;
			}

			if (setPageInfo) SetPageInfo();
		}

		public List<SelectListItem> ColPosDropDownList
		{
			get { return new ColPosViewModel().ColPosDropDownList; }
		}
	}
}