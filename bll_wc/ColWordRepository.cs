using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BLL.Abstract;
using BLL.Helpers;
using DAL;
using DAL.WordCollocationDSTableAdapters;

namespace BLL
{
	public class ColWordRepository : RepositoryBase<ColWord>, IRepository<ColWord>
	{
		private ColWordsTableAdapter ColWordsTableAdapter;

		protected ColWordsTableAdapter Adapter
		{
			get { return ColWordsTableAdapter ?? (ColWordsTableAdapter = new ColWordsTableAdapter()); }
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public WordCollocationDS.ColWordsDataTable GetColWords()
		{
			WordCollocationDS.ColWordsDataTable colWords;
			if (CacheHelper.Exists(GetCacheName))
			{
				CacheHelper.Get(GetCacheName, out colWords);
			}
			else
			{
				colWords = Adapter.GetList();
				CacheHelper.Add(colWords, GetCacheName, ModelAppSettings.CacheExpiration_Minutes);
			}
			return colWords;
		}


		internal override string GetCacheName { get { return "GetColWordsCacheName"; } }
		public override List<ColWord> GetList()
		{
			var colwords = GetColWords();
			return (from WordCollocationDS.ColWordsRow row in colwords
					select new ColWord
					{
						Id = row.Id.ToString(),
						RowVersion = row.RowVersion,
						Entry = row.Entry,
						EntryZht = row.EntryZht,
						EntryZhs = row.EntryZhs,
						EntryJap = row.EntryJap,
						CanDel = row.CanDel
					}).OrderBy(x => x.Entry).ToList();
		}

		public override ColWord GetObjectById(string id)
		{
			return GetList().SingleOrDefault(x => x.Id == id);
		}

		public bool[] Add(ColWord colWord)
		{
			bool[] bRet = new bool[2];
			bRet[0] = CheckIfDuplicatedEntry(colWord.Entry);

			if (bRet[0])
			{
				bRet[1] = false;
				return bRet;
			}

			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var affectedRow = Convert.ToInt32(Adapter.InsertDefault(colWord.Entry, colWord.EntryZht, colWord.EntryZhs, colWord.EntryJap));
					scope.Complete();
					CacheHelper.Clear(GetCacheName);
					bRet[1] = affectedRow == 1;
				}
			}
			catch (TransactionException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
			return bRet;
		}

		public bool Update(ColWord colWord)
		{
			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var rv = colWord.RowVersion;
					var affectedRow = Convert.ToInt32(Adapter.UpdateDefault(colWord.Entry, colWord.EntryZht, colWord.EntryZhs, colWord.EntryJap, colWord.CanDel!=null && colWord.CanDel.Value, short.Parse(colWord.Id), rv));
					scope.Complete();
					CacheHelper.Clear(GetCacheName);
					return affectedRow == 1;
				}
			}
			catch (TransactionException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
		}

		public bool Delete(string id)
		{
			try
			{
				var Id = long.Parse(id);
				WordCollocationDS.ColWordsDataTable currentColWords = Adapter.GetObjectById(Id);
				if (currentColWords.Count == 0) return false;
				WordCollocationDS.ColWordsRow row = currentColWords[0];
				byte[] rv = row.RowVersion;
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var affectedRow = Convert.ToInt32(Adapter.DeleteDefault(Id, rv));
					scope.Complete();
					CacheHelper.Clear(GetCacheName);
					return affectedRow == 1;
				}
			}
			catch (TransactionException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
		}

		public override bool UpdateCanDel(string id)
		{
			var affectedRow = Adapter.UpdateCanDel(long.Parse(id));
			CacheHelper.Clear(GetCacheName);
			return affectedRow == 1;
		}
	}
}
