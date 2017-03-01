using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimplestMVCApplication.Models;

namespace SimplestMVCApplication.Controllers
{
    public class HelloController : Controller
    {
        //
        // GET: /Hello/
        public ActionResult Index()
        {
	        var model = FIO.GetCurrentFIO();
			return View(model);
        }

		[HttpGet]
		public ActionResult Edit(long id)
		{
			var model = FIO.GetFIOById(id);
			return View(model);
		}

	    [HttpPost]
	    public ActionResult Save(FIO model)
	    {
			FIO.SetFIO(model);
		    model = FIO.GetFIOById(model.Id);
			return View("Index", model);
	    }

		[HttpPost]
		public ActionResult AddDetail(long id, string detail)
		{
			var model = FIO.AddDetail(id, detail);
			return View("Edit", model);
		}
	}
}