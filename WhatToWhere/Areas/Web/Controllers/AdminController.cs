using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WhatToWhere.Areas.Web.Controllers
{
    public class AdminController : Controller
    {
        // GET: Web/Admin
        public ActionResult Dashboard()
        {
            ViewBag.title = "Dashboard";
            return View();
        }
    }
}