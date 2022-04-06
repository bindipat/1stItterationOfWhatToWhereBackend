using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatToWhere.Models;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.Web.Controllers
{
    public class RequestController : Controller
    {
        // GET: Web/Request
        public ActionResult Artist()
        {
            return View();
        }

        public ActionResult LoadData(DataTablesParam param)//(InwardRegisterModel model)
        {
            try
            {
                FilterOptions op = new FilterOptions();

                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var record = _dbcontext.tbl_user.OrderByDescending(x => x.UserId);
                    int TotalRecords = record.Count();

                    int pageNo = 1;
                    if (param.iDisplayStart >= param.iDisplayLength)
                    {
                        pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;
                    }

                    var Data = record.Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();

                    return Json(new
                    {
                        aaData = Data,
                        sEcho = param.sEcho,
                        iTotalDisplayRecords = (Data.Count > 0) ? TotalRecords : 0,
                        iTotalRecords = (Data.Count > 0) ? TotalRecords : 0

                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // log4net.Error(ex);
                throw;
            }

        }
    }
}