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
	public class WcExampleRepository :RepositoryBase<WcExample>, IRepository<WcExample>
	{
		private WcExamplesTableAdapter WcExamplesTableAdapter;

		protected WcExamplesTableAdapter Adapter
		{
			get { return WcExamplesTableAdapter ?? (WcExamplesTableAdapter = new WcExamplesTableAdapter()); }
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public WordCollocationDS.WcExamplesDataTable GetWcExamples()
		{
			WordCollocationDS.WcExamplesDataTable wcExamples;
			if (CacheHelper.Exists(GetCacheName))
			{
				CacheHelper.Get(GetCacheName, out wcExamples);
			}
			else
			{
				wcExamples = Adapter.GetList();
				CacheHelper.Add(wcExamples, GetCacheName, ModelAppSettings.CacheExpiration_Minutes);
			}
			return wcExamples;
		}


		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public Collocation GetCollocationObjectByWcExampleId(long exampleId)
		{
			var example = GetObjectById(exampleId.ToString());
			var collocationId = example.CollocationId;
			return new CollocationRepository().GetObjectById(collocationId.ToString());
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public List<WcExample> GetObjectListByCollocationId(long collocationId)
		{
			return (from examples in GetListInGroup() from example in examples where example.CollocationId == collocationId select example).ToList();
		}


		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Delete, false)]
		public void DeleteWcExamplesByCollocationId(long collocationId)
		{
			Adapter.DeleteByCollocationId(collocationId);
			// return affectedRow >= 1; // no need to return affected rowcount, as some collocations maynot have any example yet => none of example rows contain that collocationId...
		}

		internal override string GetCacheName { get { return "GetWcExamplesCacheName"; } }
		public override List<IGrouping<long, WcExample>> GetListInGroup()
		{
			List<WcExample> exList = new List<WcExample>();
			var examples = GetWcExamples();
			foreach (var example in examples)
			{
				WcExample ex = new WcExample();
				ex.Id = example.Id.ToString();
				ex.Entry = example.Entry;
				ex.EntryZht = example.EntryZht;
				ex.EntryJap = example.EntryJap;
				ex.EntryZhs = example.EntryZhs;
				ex.RemarkZhs = example.RemarkZhs;
				ex.RemarkJap = example.RemarkJap;
				ex.RemarkZht = example.RemarkZht;
				ex.Source = example.Source;
				ex.CollocationId = example.CollocationId;
				ex.RowVersion = example.RowVersion;
				// ex.Collocation = GetCollocationObjectByWcExampleId(example.Id); => infinite loop

				// new property to fix 'display:none' checkbox for unknown reasons... 
				// no need for database persistence...
				ex.NeedRemark = false;

				exList.Add(ex);
			}
			return exList.GroupBy(x=>x.CollocationId).OrderByDescending(x=>x.Key).ToList();
		}


		public override WcExample GetObjectById(string id)
		{
			WcExample wcExample = null;
			var list = GetListInGroup();
			foreach (IGrouping<long, WcExample> examples in list)
			{
				foreach (WcExample example in examples)
				{
					if (example.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
					{
						wcExample = example;
						break;
					}
				}
			}
			return wcExample;
		}

		public bool[] Add(WcExample example)
		{
			bool[] bRet = new bool[2];
			bRet[0] = CheckIfDuplicatedEntry(example.Entry, example.CollocationId.ToString());

			if (bRet[0])
			{
				bRet[1] = false;
				return bRet;
			}

			var examples = new WordCollocationDS.WcExamplesDataTable();
			var row = examples.NewWcExamplesRow();
			row.Entry = example.Entry;
			if (example.EntryZht == null) row.SetEntryZhtNull();
			else row.EntryZht = example.EntryZht;
			if (example.EntryZhs == null) row.SetEntryZhsNull();
			else row.EntryZhs = example.EntryZhs;
			if (example.EntryJap == null) row.SetEntryJapNull();
			else row.EntryJap = example.EntryJap;
			if (example.Source == null) row.SetSourceNull();
			else row.Source = example.Source;
			if (example.RemarkZht == null) row.SetRemarkZhtNull();
			else row.RemarkZht = example.RemarkZht;
			if (example.RemarkZhs == null) row.SetRemarkZhsNull();
			else row.RemarkZhs = example.RemarkZhs;
			if (example.RemarkJap == null) row.SetRemarkJapNull();
			else row.RemarkJap = example.RemarkJap;
			row.CollocationId = example.CollocationId;
			examples.AddWcExamplesRow(row);

			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					int affectedRow = Adapter.Update(examples);
					scope.Complete();
					bRet[1] = affectedRow == 1;
					CacheHelper.Clear(GetCacheName);
					return bRet;
				}
			}
			catch (TransactionException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
		}

		public bool Update(WcExample example)
		{
			WordCollocationDS.WcExamplesDataTable examples = Adapter.GetObjectById(long.Parse(example.Id));
			if (examples.Count == 0) return false;
			var row = examples[0];

			row.Entry = example.Entry;
			if (example.EntryZht == null) row.SetEntryZhtNull();
			else row.EntryZht = example.EntryZht;
			if (example.EntryZhs == null) row.SetEntryZhsNull();
			else row.EntryZhs = example.EntryZhs;
			if (example.EntryJap == null) row.SetEntryJapNull();
			else row.EntryJap = example.EntryJap;
			if (example.Source == null) row.SetSourceNull();
			else row.Source = example.Source;
			if (example.RemarkZht == null) row.SetRemarkZhtNull();
			else row.RemarkZht = example.RemarkZht;
			if (example.RemarkZhs == null) row.SetRemarkZhsNull();
			else row.RemarkZhs = example.RemarkZhs;
			if (example.RemarkJap == null) row.SetRemarkJapNull();
			else row.RemarkJap = example.RemarkJap;
			row.CollocationId = example.CollocationId;

			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					int affectedRow = Adapter.Update(row);
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
			WordCollocationDS.WcExamplesDataTable currentWcExamples = Adapter.GetObjectById(Id);
			if (currentWcExamples.Count == 0) return false;
			WordCollocationDS.WcExamplesRow row = currentWcExamples[0];
			byte[] rv = row.RowVersion;

			try
			{
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

		public override bool CheckIfDuplicatedEntry(params string[] entities)
		{
			bool bRet = false;
			var list = GetListInGroup();
			foreach (var examples in list)
			{
				bRet = examples.Any(
						x => x.Entry.Equals(entities[0], StringComparison.OrdinalIgnoreCase) && x.CollocationId == long.Parse(entities[1]));
				if (bRet) break;
			}
			return bRet;
		}
	}
}
