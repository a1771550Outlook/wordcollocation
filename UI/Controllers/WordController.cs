using System;
using System.Web.Mvc;
using BLL;
using UI.Controllers.Abstract;
using UI.Models;
using UI.Models.Abstract;

namespace UI.Controllers
{
	public class WordController : ControllerBase<Word>
	{
		private readonly WordRepository repo = new WordRepository();

		// for demo purpose...
		private ActionResult Index(int page = 1)
		{
			var model = new CommonWordViewModel(WcEntity.Word, page);
			return View("CommonWordList",model);
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
			var word = repo.GetObjectById(id);
			ViewBag.Title = (id == "0" || id == null) ? "Create" : "Edit";
			ViewBag.Id = id;
			ViewBag.Entity = WcEntity.Word.ToString();
			return View("CommonEdit", word);
		}

		// POST: Word/Edit/5
		[HttpPost]
		public ActionResult Edit(Word word, string returnUrl = null)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (word.Id == "0") //newly created word
					{
						try
						{
							var p = new Word
							{
								Entry = word.Entry,
								EntryZht = word.EntryZht,
								EntryZhs = word.EntryZhs,
								EntryJap = word.EntryJap
							};

							bool[] isOk = repo.Add(p);

							// duplicated entry
							if (isOk[0])
							{
								ViewBag.IsDuplicatedEntry = true;
								return View("CommonEdit", word);
							}

							if (!isOk[1])  // add failed!
								return View("_DbActionErrorPartial");

							// add ok!
							if (!isOk[0] && isOk[1])
							{
								if (User.IsInRole("Admin"))
									return RedirectToRoute(new {action = "IndexForAdmin"});
								if (User.IsInRole("Editor"))
									return RedirectToRoute(new {action = "IndexForEditor"});
							}
						}
						catch (Exception exception)
						{
							throw new Exception("Create Failed", exception.InnerException);
						}

					}
					else //edit word
					{
						var Id = Convert.ToInt64(word.Id);
						Word p = repo.GetObjectById(Id.ToString());
						if (p != null)
						{
							p.Id = Id.ToString();
							p.Entry = word.Entry;
							p.EntryZht = word.EntryZht;
							p.EntryZhs = word.EntryZhs;
							p.EntryJap = word.EntryJap;
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
