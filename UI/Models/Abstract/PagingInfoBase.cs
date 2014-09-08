namespace UI.Models.Abstract
{
	public abstract class PagingInfoBase
	{
		public int CurrentPage { get; set; }
		public virtual int TotalPages { get; set; }
	}
}