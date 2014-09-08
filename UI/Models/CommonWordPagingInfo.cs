using System;
using UI.Models.Abstract;

namespace UI.Models
{
	public class CommonWordPagingInfo:PagingInfoBase
	{
		public int TotalWords { get; set; }
		public int WordsPerPage { get; set; }
		public override int TotalPages { get { return (int)Math.Ceiling((decimal)TotalWords / WordsPerPage); } }
	}
}