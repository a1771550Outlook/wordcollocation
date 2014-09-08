using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using UI.Controllers.Abstract;
using UI.Models.Abstract;

namespace UI.Controllers
{
	[Authorize(Roles = "Admin")]
    public class ColPosController : ControllerBase<ColPos>
    {
		private readonly ColPosRepository repo = new ColPosRepository();
		// GET: Pos
		public ActionResult Index()
		{
			List<ColPos> colPoss = repo.GetList();
			ViewBag.Entity = WcEntity.ColPos;
			return View("CommonList", colPoss);
		}

		public ViewResult Edit(string id)
		{
			var colPos = repo.GetObjectById(id);
			ViewBag.Title = (id == "0" || id == null) ? "Create" : "Edit";
			ViewBag.Id = id;
			ViewBag.Entity = WcEntity.ColPos.ToString();
			return View("CommonEdit", colPos);
		}

		// POST: Pos/Edit/5
		[HttpPost]
		public ActionResult Edit(ColPos colPos, string returnUrl = null)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (colPos.Id == "0") //newly created colPos
					{
						try
						{
							var p = new ColPos
							{
								Entry = colPos.Entry,
								EntryZht = colPos.EntryZht,
								EntryZhs = colPos.EntryZhs,
								EntryJap = colPos.EntryJap
							};

							bool[] isOk = repo.Add(p);

							// duplicated entry
							if (isOk[0])
							{
								ViewBag.IsDuplicatedEntry = true;
								return View("CommonEdit", colPos);
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
							throw new Exception("Create Failed", exception.InnerException);
						}

					}
					else //edit colPos
					{
						var Id = Convert.ToInt16(colPos.Id);
						ColPos p = repo.GetObjectById(Id.ToString());
						if (p != null)
						{
							p.Id = Id.ToString();
							p.Entry = colPos.Entry;
							p.EntryZht = colPos.EntryZht;
							p.EntryZhs = colPos.EntryZhs;
							p.EntryJap = colPos.EntryJap;
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
