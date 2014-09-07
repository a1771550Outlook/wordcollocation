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
	public class PosRepository : RepositoryBase<Pos>, IRepository<Pos>
	{
		private PossTableAdapter possTableAdapter;

		protected PossTableAdapter Adapter
		{
			get { return possTableAdapter ?? (possTableAdapter = new PossTableAdapter()); }
		}

		[System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
		public WordCollocationDS.PossDataTable GetPoss()
		{
			WordCollocationDS.PossDataTable poss;
			if (CacheHelper.Exists(GetCacheName))
			{
				CacheHelper.Get(GetCacheName, out poss);
			}
			else
			{
				poss = Adapter.GetList();
				CacheHelper.Add(poss, GetCacheName, ModelAppSettings.CacheExpiration_Minutes);
			}
			return poss;
		}

		public string[] GetPos_TranByEntry(string entry)
		{
			var poss = GetPoss();
			var pos = poss.FirstOrDefault(p => String.Equals(p.Entry, entry, StringComparison.OrdinalIgnoreCase));

			return pos != null ? new[] { pos.EntryZht, pos.EntryZhs, pos.EntryJap } : null;
		}

		internal override string GetCacheName { get { return "GetPossCacheName"; } }
		public override List<Pos> GetList()
		{
			var pos = GetPoss();
			return (from WordCollocationDS.PossRow row in pos
					select new Pos
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

		public override Pos GetObjectById(string id)
		{
			return GetList().SingleOrDefault(x => x.Id == id);
		}

		public bool[] Add(Pos pos)
		{
			bool[] bRet = new bool[2];
			bRet[0] = CheckIfDuplicatedEntry(pos.Entry);

			if (bRet[0])
			{
				bRet[1] = false;
				return bRet;
			}

			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var affectedRow = Convert.ToInt32(Adapter.InsertDefault(pos.Entry, pos.EntryZht, pos.EntryZhs, pos.EntryJap));
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

		public bool Update(Pos pos)
		{
			try
			{
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var rv = pos.RowVersion;
					var affectedRow = Convert.ToInt32(Adapter.UpdateDefault(pos.Entry, pos.EntryZht, pos.EntryZhs, pos.EntryJap, pos.CanDel != null && pos.CanDel.Value, short.Parse(pos.Id), rv));
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
			var affectedRow = Adapter.UpdateCanDel(short.Parse(id));
			CacheHelper.Clear(GetCacheName);
			return affectedRow == 1;
		}

		public bool Delete(string id)
		{
			try
			{
				var Id = short.Parse(id);
				WordCollocationDS.PossDataTable currentPoss = Adapter.GetObjectById(Id);
				if (currentPoss.Count == 0) return false;
				WordCollocationDS.PossRow row = currentPoss[0];
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
