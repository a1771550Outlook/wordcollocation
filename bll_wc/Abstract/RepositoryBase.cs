using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Abstract
{
	public abstract class RepositoryBase<T> where T:WcBase
	{
		internal abstract string GetCacheName { get; }

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, true)]
		public virtual List<T> GetList()
		{
			return null;
		}

		public virtual List<IGrouping<long, T>> GetListInGroup()
		{
			return null;
		}
		
		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public abstract T GetObjectById(string id);

		public virtual bool CheckIfDuplicatedEntry(params string[] entities)
		{
			bool bRet = false;

			if (entities.Length == 1) // for pos,colpos,word,colword
			{
				bRet = GetList().Any(x => x.Entry.Equals(entities[0], StringComparison.OrdinalIgnoreCase));
			}
			
			return bRet;
		}

		public virtual WcBase GetObjectByEntries(params string[] entries)
		{
			return
				GetList()
					.SingleOrDefault(
						x => x.Entry == entries[0]);
		}

		public virtual bool UpdateCanDel(string id) { return false;}
	}
}
