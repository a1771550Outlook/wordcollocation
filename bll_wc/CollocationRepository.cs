using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Caching;
using BLL.Abstract;
using BLL.Helpers;
using DAL;
using DAL.WordCollocationDSTableAdapters;

namespace BLL
{
	public class CollocationRepository : RepositoryBase<Collocation>, IRepository<Collocation>
	{

		private CollocationsTableAdapter CollocationsTableAdapter;

		private List<Collocation> ConvertCollocationDataTableToList(WordCollocationDS.CollocationsDataTable collocations)
		{
			return (from WordCollocationDS.CollocationsRow row in collocations
					select new Collocation
					{
						Id = row.Id,
						posId = row.posId,
						colPosId = row.colPosId,
						wordId = row.wordId,
						colWordId = row.colWordId,
						PatternTextArray = BLLHelper.GetPatternArray((CollocationPattern)row.CollocationPattern),
						PatternTextString = BLLHelper.GetPatternString((CollocationPattern)row.CollocationPattern),
						RowVersion = row.RowVersion,
						Pos = row.Pos,
						PosZht = row.PosZht,
						PosZhs = row.PosZhs,
						PosJap = row.PosJap,
						ColPos = row.ColPos,
						ColPosZht = row.ColPosZht,
						ColPosZhs = row.ColPosZhs,
						ColPosJap = row.ColPosJap,
						Word = row.Word,
						WordZht = row.WordZht,
						WordZhs = row.WordZhs,
						WordJap = row.WordJap,
						ColWord = row.ColWord,
						ColWordZht = row.ColWordZht,
						ColWordZhs = row.ColWordZhs,
						ColWordJap = row.ColWordJap,
						CollocationPattern = (CollocationPattern)row.CollocationPattern,
						WcExampleList = ConvertWcExampleTableToList(row.GetWcExamples())
					}
				).OrderByDescending(x => x.Id).ToList();
		}
		protected CollocationsTableAdapter Adapter
		{
			get { return CollocationsTableAdapter ?? (CollocationsTableAdapter = new CollocationsTableAdapter()); }
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		private WordCollocationDS.CollocationsDataTable GetCollocations()
		{
			WordCollocationDS.CollocationsDataTable collocations = HttpContext.Current.Cache[GetCacheName] as WordCollocationDS.CollocationsDataTable;
			if (collocations == null)
			{
				collocations = Adapter.GetList();
				HttpContext.Current.Cache.Insert(GetCacheName, collocations, null,
												 DateTime.Now.AddMinutes(ModelAppSettings.CacheExpiration_Minutes),
												 TimeSpan.Zero, CacheItemPriority.High, null);

			}
			else
				collocations = (WordCollocationDS.CollocationsDataTable)HttpContext.Current.Cache[GetCacheName];
			return collocations;

		}

		public Dictionary<string, int> GetColPatternDictionary()
		{
			Dictionary<string, int> ColPatternDictionary = new Dictionary<string, int>
				{
					{CollocationPattern.noun_verb.ToString(), ((int) CollocationPattern.noun_verb)},
					{CollocationPattern.verb_noun.ToString(), ((int) CollocationPattern.verb_noun)},
					{CollocationPattern.adjective_noun.ToString(), ((int) CollocationPattern.adjective_noun)},
					{CollocationPattern.adverb_verb.ToString(), ((int) CollocationPattern.adverb_verb)},
					{CollocationPattern.verb_preposition.ToString(), ((int) CollocationPattern.verb_preposition)},
					//{CollocationPattern.preposition_verb.ToString(), ((int) CollocationPattern.preposition_verb)},
					{CollocationPattern.phrase_noun.ToString(), ((int) CollocationPattern.phrase_noun)}
				};
			return ColPatternDictionary;
		}

		private static IEnumerable<CollocationPattern> GetColPatternList()
		{
			List<CollocationPattern> ColPatternList = new List<CollocationPattern>
				{
					CollocationPattern.adjective_noun,
					CollocationPattern.adverb_verb,
					CollocationPattern.noun_verb,
					CollocationPattern.phrase_noun,
					//CollocationPattern.preposition_verb,
					CollocationPattern.verb_noun,
					CollocationPattern.verb_preposition
				};

			return ColPatternList;
		}

		public CollocationPattern GetColPatternKeyByValue(int index)
		{
			return GetColPatternList().Single(p => (int)p == index);
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public List<Collocation> GetCollocationListInGroup(string letter = null)
		{
			var collocations = GetCollocations();
			var collocationList = ConvertCollocationDataTableToList(collocations);
			if (!string.IsNullOrEmpty(letter))
				collocationList =
					collocationList.Where(c => String.Equals(c.Word, letter, StringComparison.CurrentCultureIgnoreCase)).ToList();

			var collist = collocationList.GroupBy(c => c.Word + "|" + c.CollocationPattern).Select(g => new { g.Key, Collocation = g });
			var cvList = new List<Collocation>();

			foreach (var l in collist)
			{
				Collocation cv = new Collocation
				{
					Word = l.Key.Split('|')[0]
					//CollocationPattern = (CollocationPattern)(Convert.ToInt32(l..Split('|')[1]))
				};

				//just loop once, in fact just a transition from the grouped values to the properties of the new Collocation class, not really a loop at all
				foreach (var c in l.Collocation)
				{
					cv.CollocationPattern = c.CollocationPattern;
					cv.Pos = c.Pos;
					cv.PosZht = c.PosZht;
					cv.PosZhs = c.PosZhs;
					cv.PosJap = c.PosJap;
					cv.ColPos = c.ColPos;
					cv.ColPosZht = c.ColPosZht;
					cv.ColPosZhs = c.ColPosZhs;
					cv.ColPosJap = c.ColPosJap;
					cv.Word = c.Word;
					cv.WordZht = c.WordZht;
					cv.WordZhs = c.WordZhs;
					cv.WordJap = c.WordJap;
					cv.ColWord = c.ColWord;
					cv.ColWordZht = c.ColWordZht;
					cv.ColWordZhs = c.ColWordZhs;
					cv.ColWordJap = c.ColWordJap;
					cv.Id = c.Id;
					cv.WcExampleList = c.WcExampleList;
					//cvList.Add(cv);
				}
				cvList.Add(cv);
			}

			return cvList;
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public List<Collocation> GetCollocationListByWordColPosId(string word, short colPosId)
		{
			var collocations = GetCollocations();
			var collocationList = ConvertCollocationDataTableToList(collocations);
			var collist = collocationList.Where(c => c.Word.ToLower() == word.ToLower() && c.colPosId == colPosId).ToList();
			return collist;
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public List<Collocation> GetCollocationListByWordPattern(string word, CollocationPattern pattern)
		{
			var collocations = GetCollocations();
			var colList = ConvertCollocationDataTableToList(collocations);
			return colList.Where(c => string.Equals(c.Word, word, StringComparison.CurrentCultureIgnoreCase) && c.CollocationPattern == pattern).ToList();
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public List<Collocation> GetCollocationListByWord(string word)
		{
			var collocations = GetCollocations();
			var collocationList = ConvertCollocationDataTableToList(collocations);
			return collocationList.Where(c => String.Equals(c.Word, word, StringComparison.CurrentCultureIgnoreCase)).ToList();
		}

		public List<WcExample> ConvertWcExampleTableToList(WordCollocationDS.WcExamplesDataTable exTable)
		{
			return (from WordCollocationDS.WcExamplesRow row in exTable
					select new WcExample
					{
						Id = row.Id.ToString(),
						Entry = row.Entry,
						EntryZht = row.EntryZht,
						EntryZhs = row.EntryZhs,
						EntryJap = row.EntryJap,
						RowVersion = row.RowVersion,
						CollocationId = row.CollocationId,
						RemarkZht = row.RemarkZht,
						RemarkZhs = row.RemarkZhs,
						RemarkJap = row.RemarkJap,
						Source = row.Source,
						//Must NOT run the function below, as it cause infinitive loop!!
						//Collocation = new WcExampleRepository().GetCollocationObjectByWcExampleId(row.Id)
					}).
			ToList();
		}

		internal override string GetCacheName { get { return "GetCollocationsCacheName"; } }

		public override List<Collocation> GetList()
		{
			var collocations = GetCollocations();
			return ConvertCollocationDataTableToList(collocations);
		}

		public override Collocation GetObjectById(string id)
		{
			//var collocation = GetCollocations().SingleOrDefault(x => x.Id == id);
			return GetList().SingleOrDefault(x => x.Id == long.Parse(id));
		}

		public bool Update(Collocation collocation)
		{
			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var rv = collocation.RowVersion;
					var affectedRow = Convert.ToInt32(Adapter.Update(collocation.posId, collocation.colPosId, collocation.wordId, collocation.colWordId, (int)collocation.CollocationPattern, collocation.Id, rv));
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
			var Id = long.Parse(id);

			// Delete related Examples first, if any...
			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var repo = new WcExampleRepository();
					repo.DeleteWcExamplesByCollocationId(Id);
					scope.Complete();
					CacheHelper.Clear(GetCacheName);
				}
			}
			catch (TransactionException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}

			// Then delete the collocation...
			try
			{
				WordCollocationDS.CollocationsDataTable currentCollocations = Adapter.GetObjectById(Id);
				if (currentCollocations.Count == 0) return false;
				WordCollocationDS.CollocationsRow row = currentCollocations[0];
				byte[] rv = row.RowVersion;

				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var affectedRow = Convert.ToInt32(Adapter.Delete(Id, rv));
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

		public bool[] Add(Collocation collocation)
		{
			bool[] bRet = new bool[3];
			bRet[0] = CheckIfDuplicatedEntry(collocation.posId.ToString(), collocation.colPosId.ToString(), collocation.wordId.ToString(), collocation.colWordId.ToString(), collocation.CollocationPattern.ToString());

			if (bRet[0])
			{
				bRet[1] = false;
				return bRet;
			}

			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var affectedRow = Convert.ToInt64(Adapter.Insert(collocation.posId, collocation.colPosId, collocation.wordId, collocation.colWordId,
					(int)collocation.CollocationPattern));
					scope.Complete();
					bRet[1] = affectedRow == 1;
				}
			}
			catch (TransactionException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
			CacheHelper.Clear(GetCacheName);

			bRet[2] = UpdateOtherIDTables(collocation);

			return bRet;
		}

		public bool[] Add(Collocation collocation, out long collocationId)
		{
			bool[] bRet = new bool[3];
			collocationId = 0;

			bRet[0] = CheckIfDuplicatedEntry(collocation.posId.ToString(), collocation.colPosId.ToString(), collocation.wordId.ToString(), collocation.colWordId.ToString(), collocation.CollocationPattern.ToString());

			if (bRet[0])
			{
				bRet[1] = false;
				return bRet;
			}

			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					collocationId = Convert.ToInt64(Adapter.InsertQuery(collocation.posId, collocation.colPosId, collocation.wordId, collocation.colWordId,
					(int)collocation.CollocationPattern));
					scope.Complete();
					bRet[1] = collocationId > 0;
				}
			}
			catch (TransactionException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
			CacheHelper.Clear(GetCacheName);

			bRet[2]=UpdateOtherIDTables(collocation);

			return bRet;
		}

		static bool UpdateOtherIDTables(Collocation collocation)
		{
			bool bRet;
			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					new PosRepository().UpdateCanDel(collocation.posId.ToString());
					new ColPosRepository().UpdateCanDel(collocation.colPosId.ToString());
					new WordRepository().UpdateCanDel(collocation.wordId.ToString());
					bRet = new ColWordRepository().UpdateCanDel(collocation.colWordId.ToString());
					scope.Complete();
				}
			}
			catch (TransactionException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
			return bRet;
		}

		public override bool CheckIfDuplicatedEntry(params string[] ids)
		{
			bool bRet =
				GetList()
					.Any(
						x =>
							x.posId == short.Parse(ids[0]) && x.colPosId == short.Parse(ids[1]) &&
							x.wordId == long.Parse(ids[2]) && x.colWordId == long.Parse(ids[3]) &&
							x.CollocationPattern == (CollocationPattern)Enum.Parse(typeof(CollocationPattern), ids[4], true));

			return bRet;
		}

		public Collocation GetObjectByIds(params string[] ids)
		{
			return GetList().SingleOrDefault(x =>
				x.posId == short.Parse(ids[0]) && x.colPosId == short.Parse(ids[1]) &&
				x.wordId == long.Parse(ids[2]) && x.colWordId == long.Parse(ids[3]) &&
				x.CollocationPattern == (CollocationPattern)Enum.Parse(typeof(CollocationPattern), ids[4], true));
		}
	}
}
