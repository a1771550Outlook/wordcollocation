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
	public class ColPosRepository : RepositoryBase<ColPos>, IRepository<ColPos>
	{
		private ColPossTableAdapter ColPossTableAdapter;

		protected ColPossTableAdapter Adapter
		{
			get { return ColPossTableAdapter ?? (ColPossTableAdapter = new ColPossTableAdapter()); }
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public WordCollocationDS.ColPossDataTable GetColPoss()
		{
			WordCollocationDS.ColPossDataTable colPoss;
			if (CacheHelper.Exists(GetCacheName))
			{
				CacheHelper.Get(GetCacheName, out colPoss);
			}
			else
			{
				colPoss = Adapter.GetList();
				CacheHelper.Add(colPoss, GetCacheName, ModelAppSettings.CacheExpiration_Minutes);
			}
			return colPoss;
		}

		internal override string GetCacheName { get { return "GetColPossCacheName"; } }

		public override List<ColPos> GetList()
		{
			var pos = GetColPoss();
			return (from WordCollocationDS.ColPossRow row in pos
					select new ColPos
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

		public override ColPos GetObjectById(string id)
		{
			return GetList().SingleOrDefault(x => x.Id == id);
		}

		public override bool UpdateCanDel(string id)
		{
			var affectedRow = Adapter.UpdateCanDel(short.Parse(id));
			CacheHelper.Clear(GetCacheName);
			return affectedRow == 1;
		}

		public bool[] Add(ColPos colPos)
		{
			bool[] bRet = new bool[2];
			bRet[0] = CheckIfDuplicatedEntry(colPos.Entry);

			if (bRet[0])
			{
				bRet[1] = false;
				return bRet;
			}

			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var affectedRow = Convert.ToInt32(Adapter.InsertDefault(colPos.Entry, colPos.EntryZht, colPos.EntryZhs, colPos.EntryJap));
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

		public bool Update(ColPos colPos)
		{
			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var rv = colPos.RowVersion;
					var affectedRow = Convert.ToInt32(Adapter.UpdateDefault(colPos.Entry, colPos.EntryZht, colPos.EntryZhs, colPos.EntryJap, colPos.CanDel!=null && colPos.CanDel.Value, short.Parse(colPos.Id), rv));
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
				var Id = short.Parse(id);
				WordCollocationDS.ColPossDataTable currentColPoss = Adapter.GetObjectById(Id);
				if (currentColPoss.Count == 0) return false;
				WordCollocationDS.ColPossRow row = currentColPoss[0];
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
	}
}
