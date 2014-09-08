using System;
using System.Text;
using System.Web.Mvc;
using UI.Models;
using UI.Models.Abstract;

namespace UI.Helpers
{
	public static class PagingHelper
	{
		public static MvcHtmlString PageLinks(this HtmlHelper helper, PagingInfoBase pagingInfo, Func<int, string> pageUrl)
		{
			StringBuilder lists = new StringBuilder();
			TagBuilder ulTag = new TagBuilder("ul");
			TagBuilder listTag;

			ulTag.InnerHtml += Environment.NewLine;

			TagBuilder PrevLinkTag = new TagBuilder("a");
			if (pagingInfo.CurrentPage > 1)
			{
				PrevLinkTag.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage - 1));
				PrevLinkTag.InnerHtml = "«";
				listTag = new TagBuilder("li");
				listTag.InnerHtml = PrevLinkTag.ToString();
				lists.Append(listTag);
			}
			

			for (int i = 1; i <= pagingInfo.TotalPages; i++)
			{
				listTag = new TagBuilder("li");
				
				TagBuilder linkTag = new TagBuilder("a");

				linkTag.MergeAttribute("href", pageUrl(i));
				linkTag.InnerHtml = i.ToString();

				if (i == pagingInfo.CurrentPage)
				{
					listTag.AddCssClass("active");
					//tag.AddCssClass("btn-primary");
				}
				//tag.AddCssClass("btn btn-default");
				listTag.InnerHtml = linkTag.ToString();
				lists.Append(listTag);
			}

			TagBuilder NextLinkTag = new TagBuilder("a");
			if (pagingInfo.CurrentPage < pagingInfo.TotalPages)
			{
				NextLinkTag.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage + 1));
				NextLinkTag.InnerHtml = "»";
				listTag = new TagBuilder("li");
				listTag.InnerHtml = NextLinkTag.ToString();
				lists.Append(listTag);
			}

			ulTag.InnerHtml = lists.ToString();
			return MvcHtmlString.Create(ulTag.ToString());
		}
	}
}