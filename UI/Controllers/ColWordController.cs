using System;
using System.Web.Mvc;
using BLL;
using UI.Controllers.Abstract;
using UI.Models;
using UI.Models.Abstract;

namespace UI.Controllers
{
    public class ColWordController : ControllerBase<ColWord>
    {
		private readonly ColWordRepository repo = new ColWordRepository();
		// for demo purpose...
		private ActionResult Index(int page = 1)
		{
			var model = new CommonWordViewModel(WcEntity.ColWord, page);
			return View("CommonWordList", model);
		}

		[Authorize(Roles = "Editor")]
		// GET: Word
		public ActionResult IndexForEditor(int page = 1)
		{
			return Index(page);
		}

		[Authorize(Roles = "Admin")]
		public ActionResult IndexForAdmin(int page = 1)
		{
			return Index(page);
		}

		public ViewResult Edit(string id)
		{
			var colword = repo.GetObjectById(id);
			ViewBag.Title = (id == "0" || id == null) ? "Create" : "Edit";
			ViewBag.Id = id;
			ViewBag.Entity = WcEntity.ColWord.ToString();
			return View("CommonEdit", colword);
		}

		// POST: Word/Edit/5
		[HttpPost]
		public ActionResult Edit(ColWord colWord, string returnUrl = null)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (colWord.Id == "0") //newly created colWord
					{
						try
						{
							var p = new ColWord
							{
								Entry = colWord.Entry,
								EntryZht = colWord.EntryZht,
								EntryZhs = colWord.EntryZhs,
								EntryJap = colWord.EntryJap
							};

							bool[] isOk = repo.Add(p);

							// duplicated entry
							if (isOk[0])
							{
								ViewBag.IsDuplicatedEntry = true;
								return View("CommonEdit", colWord);
							} 

							if (!isOk[1])  // add failed!
								return View("_DbActionErrorPartial");

							// add ok!
							if (!isOk[0] && isOk[1])
							{
								if (User.IsInRole("Admin"))
									return RedirectToRoute(new { action = "IndexForAdmin" });
								if (User.IsInRole("Editor"))
									return RedirectToRoute(new { action = "IndexForEditor" });
							}
						}
						catch (Exception exception)
						{
							throw new Exception("Create Failed", exception.InnerException);
						}

					}
					else //edit colWord
					{
						var Id = Convert.ToInt64(colWord.Id);
						ColWord p = repo.GetObjectById(Id.ToString());
						if (p != null)
						{
							p.Id = Id.ToString();
							p.Entry = colWord.Entry;
							p.EntryZht = colWord.EntryZht;
							p.EntryZhs = colWord.EntryZhs;
							p.EntryJap = colWord.EntryJap;
							repo.Update(p);

							if (User.IsInRole("Admin"))
								return RedirectToRoute(new { action = "IndexForAdmin" });
							if (User.IsInRole("Editor"))
								return RedirectToRoute(new { action = "IndexForEditor" });
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

		// GET: Word/Delete/5
		public ActionResult Delete(long id, string returnUrl = null)
		{
			try
			{
				if (repo.Delete(id.ToString()))
				{
					if (User.IsInRole("Admin"))
						return RedirectToRoute(new { action = "IndexForAdmin" });
					if (User.IsInRole("Editor"))
						return RedirectToRoute(new { action = "IndexForEditor" });
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
