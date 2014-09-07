using System.ComponentModel.DataAnnotations;
using WcResources;

namespace BLL.Abstract
{
	public abstract class WcBase
	{
		[Key]
		public virtual string Id { get; set; }
		[Required(ErrorMessage = "Please enter an entry")]
		public virtual string Entry { get; set; }

		[Required(ErrorMessageResourceType = typeof (Resources), ErrorMessageResourceName = "EntryZhtRequired")]
		[Display(Name = "條目")]
		public virtual string EntryZht { get; set; }

		[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EntryZhsRequired")]
		[Display(Name = "条目")]
		public virtual string EntryZhs { get; set; }

		[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EntryJapRequired")]
		[Display(Name = "エントリー")]
		public virtual string EntryJap { get; set; }

		public virtual byte[] RowVersion { get; set; }

		public virtual bool? CanDel { get; set; }
	}
}
