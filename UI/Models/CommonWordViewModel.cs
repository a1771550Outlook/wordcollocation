using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using UI.Models.Abstract;

namespace UI.Models
{
	public class CommonWordViewModel : ViewModelBase
	{
		private readonly WcEntity entity;
		public WcEntity Entity { get { return entity; } }
		private readonly string id;
		private WordRepository wrepo;
		private ColWordRepository cwrepo;
		private List<Word> _WordList;
		private List<ColWord> _ColWordList;
		private readonly int page;
		public CommonWordPagingInfo PagingInfo { get; set; }
		//public ColWordPagingInfo ColWordPagingInfo { get; set; }
		public List<ColWord> ColWordList { get { return _ColWordList; } }
		public List<Word> WordList { get { return _WordList; } }

		public int PageSize = SiteConfiguration.WcViewPageSize;

		public CommonWordViewModel(int page=1)
		{
			this.page = page;
		}

		public CommonWordViewModel(string id = null)
		{
			this.id = id;
		}

		public CommonWordViewModel(WcEntity entity,string id = null):this(id)
		{
			this.entity = entity;
		}

		public CommonWordViewModel(WcEntity entity, int page=1):this(page)
		{
			this.entity = entity;
			SetCommonWordList();
			SetPageInfo();
		}


		private void SetCommonWordList()
		{
			switch (entity)
			{
				case WcEntity.Word:
					wrepo = new WordRepository();
					_WordList = wrepo.GetList();
					break;
				case WcEntity.ColWord:
					cwrepo = new ColWordRepository();
					_ColWordList = cwrepo.GetList();
					break;

			}
		}
		private void SetPageInfo()
		{
			CommonWordPagingInfo pagingInfo = new CommonWordPagingInfo();
			pagingInfo.CurrentPage = page;
			pagingInfo.WordsPerPage = PageSize;

			switch (entity)
			{
				case WcEntity.Word:
					_ColWordList = null;
					setPagingDetails(ref _WordList, ref _ColWordList, pagingInfo);
					break;
				case WcEntity.ColWord:
					_WordList = null;
					setPagingDetails(ref _WordList, ref _ColWordList, pagingInfo);
					break;
			}

		}

		private void setPagingDetails(ref List<Word> wlist, ref List<ColWord> cwlist, CommonWordPagingInfo pagingInfo)
		{
			if (wlist != null && wlist.Count > 0)
			{
				if (page < 1) return;
				pagingInfo.TotalWords = wlist.Count();
				PagingInfo = pagingInfo;
				wlist = wlist.Skip((page - 1) * PageSize).Take(PageSize).ToList();
			}
			else if (cwlist != null && cwlist.Count > 0)
			{
				if (page < 1) return;
				pagingInfo.TotalWords = cwlist.Count();
				PagingInfo = pagingInfo;
				cwlist = cwlist.Skip((page - 1) * PageSize).Take(PageSize).ToList();
			}
		}

		public List<SelectListItem> CommonWordDropDownList
		{
			get
			{
				return CreateDropDownList(entity, id);
			}
		}
	}
}