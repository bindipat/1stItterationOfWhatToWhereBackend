using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatToWhere.Filter;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.Web.Controllers
{
    
    public class UserController : Controller
    {
        // GET: Web/User
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tbl_admin model)
        {
            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                if(_dbcontext.tbl_admin.Where(x=>x.Email==model.Email && x.Password == model.Password).Any())
                {
                    return RedirectToAction("dashboard", "admin");
                }
                else
                {
                    //error
                    ViewBag.Error = "Invalid username and password";
                    return View(model);
                }
            }
        }
    }
}