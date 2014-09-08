using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using UI.Models.Abstract;
using WcResources;
using CreateMode = UI.Controllers.Abstract.CreateMode;

namespace UI.Models
{
	public class WcExampleViewModel : WcManageViewModelBase
	{
		private readonly long collocationId;
		public long CollocationId { get { return collocationId; } }
		public bool NeedRemark { get; set; }
		//public override bool NeedRemark { get; set; }
		private readonly WcExampleRepository repo = new WcExampleRepository();
		private readonly WcExample _wcExample;
		private readonly List<WcExample> _WcExampleList;
		private List<IGrouping<long, WcExample>> _WcExampleListInGroup;
		private readonly int page;
		public WcExamplePagingInfo WcExamplePagingInfo { get; set; }
		public List<WcExample> WcExampleList { get { return _WcExampleList; } }
		public List<IGrouping<long, WcExample>> WcExampleListInGroup { get { return _WcExampleListInGroup; } }

		public int PageSize = SiteConfiguration.WcViewPageSize;
		public WcExample WcExample { get { return _wcExample; } }

		//private readonly CreateMode createMode;
		//public CreateMode CreateMode { get { return createMode; } }

		//public WcExampleViewModel()
		//{
		//	NeedRemark = false;
		//}

		public WcExampleViewModel(long id, CreateMode mode)
		{
			switch (mode)
			{
				case CreateMode.Collocation:
					collocationId = id;
					break;
				case CreateMode.WcExample:
					_wcExample = repo.GetObjectById(id.ToString());
					break;
			}
			_WcExampleList = new List<WcExample> { _wcExample };
		}

		public WcExampleViewModel(int page = 1)
		{
			this.page = page;
			GetWcExampleListInGroup();
		}

		public WcExampleViewModel(WcExample example)
		{
			_WcExampleList = new List<WcExample> { example };
		}
		private void GetWcExampleListInGroup()
		{
			_WcExampleListInGroup = repo.GetListInGroup();
			SetPageInfo();
		}
		private void SetPageInfo()
		{
			WcExamplePagingInfo pagingInfo = new WcExamplePagingInfo();
			pagingInfo.CurrentPage = page;
			pagingInfo.WcExamplesPerPage = PageSize;

			if (_WcExampleListInGroup != null && _WcExampleListInGroup.Count > 0)
			{
				if (page >= 1)
				{
					pagingInfo.TotalWcExamples = _WcExampleListInGroup.Count();
					WcExamplePagingInfo = pagingInfo;
					_WcExampleListInGroup = _WcExampleListInGroup.Skip((page - 1) * PageSize).Take(PageSize).ToList();
				}
			}
		}

		public List<SelectListItem> CollocationIdDropDownList
		{
			get
			{
				var ddlCollocation = new List<SelectListItem>();
				ddlCollocation.Add(new SelectListItem { Selected = collocationId == 0, Text = string.Format("- {0} -", Resources.WordCollocation), Value = "0" });
				var crepo = new CollocationRepository();
				List<Collocation> cList = crepo.GetList();

				ddlCollocation.AddRange(from entity in cList let isSelected = entity.Id.ToString().Equals(collocationId.ToString(), StringComparison.OrdinalIgnoreCase) select new SelectListItem { Text = entity.Id.ToString(), Value = entity.Id.ToString(), Selected = isSelected });

				return ddlCollocation;
			}
		}
	}
}