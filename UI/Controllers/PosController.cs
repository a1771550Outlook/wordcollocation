using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using UI.Controllers.Abstract;
using UI.Models.Abstract;

namespace UI.Controllers
{
	[Authorize(Roles = "Admin")]
	public class PosController : ControllerBase<Pos>
	{
		private readonly PosRepository repo = new PosRepository();

		public ActionResult Index()
		{
			List<Pos> Poss = repo.GetList();
			ViewBag.Entity = WcEntity.Pos;
			return View("CommonList", Poss);
		}

		//public ActionResult IndexForAdmin()
		//{
		//	return Index();
		//}

		//[Authorize(Roles = "Editor")]
		//public ActionResult IndexForEditor()
		//{
		//	return Index();
		//}

		//public ActionResult Create()
		//{
		//	return View("CommonEdit", null);
		//}
		public ViewResult Edit(string id)
		{
			var pos = repo.GetObjectById(id);
			ViewBag.Title = (id == "0" || id == null) ? "Create" : "Edit";
			ViewBag.Id = id;
			ViewBag.Entity = WcEntity.Pos.ToString();
			return View("CommonEdit", pos);
		}

		// POST: Pos/Edit/5
		[HttpPost]
		public ActionResult Edit(Pos pos, string returnUrl = null)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (pos.Id == "0") //newly created pos
					{
						try
						{
							var p = new Pos
							{
								Entry = pos.Entry,
								EntryZht = pos.EntryZht,
								EntryZhs = pos.EntryZhs,
								EntryJap = pos.EntryJap
							};

							bool[] isOk = repo.Add(p);

							// duplicated entry
							if (isOk[0])
							{
								ViewBag.IsDuplicatedEntry = true;
								return View("CommonEdit", pos);
							}

							if (!isOk[1])  // add failed!
								return View("_DbActionErrorPartial");

							// add ok!
							if (!isOk[0] && isOk[1])
							{
								return RedirectToRoute(new { action = "Index" });
							}
						}
						catch (Exception exception)
						{
							throw new Exception(exception.Message, exception.InnerException);
						}

					}
					else //edit pos
					{
						var Id = Convert.ToInt16(pos.Id);
						Pos p = repo.GetObjectById(Id.ToString());
						if (p != null)
						{
							p.Id = Id.ToString();
							p.Entry = pos.Entry;
							p.EntryZht = pos.EntryZht;
							p.EntryZhs = pos.EntryZhs;
							p.EntryJap = pos.EntryJap;
							repo.Update(p);
							//CacheHelper.Clear(repo.GetCacheName);
							return RedirectToAction("Index");
						}
					}

				}
				else return View("CommonEdit");

			}
			catch (Exception exception)
			{
				ViewBag.ErrorMessage = exception.Message;
				ViewBag.InnerMessage = exception.InnerException;
				return View("CommonEdit");
			}
			return null;
		}

		// GET: Pos/Delete/5
		public ActionResult Delete(short id, string returnUrl = null)
		{
			try
			{
				if (repo.Delete(id.ToString()))
				{
					//CacheHelper.Clear(repo.GetCacheName);
					return RedirectToRoute(new { action = "Index" });
				}

				return View("_DbActionErrorPartial");
			}
			catch (Exception exception)
			{
				throw new Exception("Delete Failed", exception.InnerException);
			}
		}
	}
}
