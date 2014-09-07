using System.Collections.Generic;

namespace BLL.Abstract
{
	public interface IRepository<in T> where T:WcBase
	{
		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Insert, true)]
		bool[] Add(T t);

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Update, true)]
		bool Update(T t);

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Delete, true)]
		bool Delete(string id);
	}
}
