using System.ComponentModel.DataAnnotations;
using BLL.Abstract;

namespace BLL
{
	public class WcExample : WcBase
	{
		public string Source { get; set; }

		[Display(Name = "備註")]
		public string RemarkZhs { get; set; }

		[Display(Name = "备注")]
		public string RemarkZht { get; set; }

		[Display(Name = "備考")]
		public string RemarkJap { get; set; }
		[Required]
		public long CollocationId { get; set; }

		// disable this navigational property so as to avoid infinite loop
		//public Collocation Collocation { get; set; }

		public bool NeedRemark { get; set; }
	}
}
