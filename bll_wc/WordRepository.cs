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
	public class WordRepository : RepositoryBase<Word>, IRepository<Word>
	{
		private WordsTableAdapter WordsTableAdapter;

		protected WordsTableAdapter Adapter
		{
			get { return WordsTableAdapter ?? (WordsTableAdapter = new WordsTableAdapter()); }
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public WordCollocationDS.WordsDataTable GetWords()
		{
			WordCollocationDS.WordsDataTable words;
			if (CacheHelper.Exists(GetCacheName))
			{
				CacheHelper.Get(GetCacheName, out words);
			}
			else
			{
				words = Adapter.GetList();
				CacheHelper.Add(words, GetCacheName, ModelAppSettings.CacheExpiration_Minutes);
			}
			return words;
		}

		internal override string GetCacheName { get { return "GetWordsCacheName"; } }

		public override List<Word> GetList()
		{
			var words = GetWords();
			return (from WordCollocationDS.WordsRow row in words
					select new Word
					{
						Id = row.Id.ToString(),
						RowVersion = row.RowVersion,
						Entry = row.Entry,
						EntryZht = row.EntryZht,
						EntryZhs = row.EntryZhs,
						EntryJap = row.EntryJap,
						CanDel = row.CanDel
					}
				).OrderBy(x => x.Entry).ToList();
		}

		public override Word GetObjectById(string id)
		{
			return GetList().SingleOrDefault(x => x.Id == id);
		}

		public bool[] Add(Word word)
		{
			bool[] bRet = new bool[2];
			bRet[0] = CheckIfDuplicatedEntry(word.Entry);

			if (bRet[0])
			{
				bRet[1] = false;
				return bRet;
			}

			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var affectedRow = Convert.ToInt32(Adapter.InsertDefault(word.Entry, word.EntryZht, word.EntryZhs, word.EntryJap));
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

		public bool Update(Word word)
		{
			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var rv = word.RowVersion;
					var affectedRow = Convert.ToInt32(Adapter.UpdateDefault(word.Entry, word.EntryZht, word.EntryZhs, word.EntryJap, word.CanDel!=null && word.CanDel.Value, long.Parse(word.Id), rv));
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
				WordCollocationDS.WordsDataTable currentWords = Adapter.GetObjectById(Id);
				if (currentWords.Count == 0) return false;
				WordCollocationDS.WordsRow row = currentWords[0];
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
