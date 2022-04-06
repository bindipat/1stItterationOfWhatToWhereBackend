using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;
using WhatToWhere.Models;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.APIServices.Controllers
{
    public class WardrobeController : ApiController
    {
        [Authorize]
        // GET: APIServices/Wardrobe
        public ResponseModel Create([Required] long ParentDirId, [Required][MaxLength(50)] string Name)
        {
            long userId = Convert.ToInt64(RequestContext.Principal.Identity.Name);
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    if (!_dbcontext.tbl_directory.Any(x => x.DirType == "w" && x.Name == Name && x.ParentDirId == ParentDirId && x.UserId == userId))
                    {
                        tbl_directory model = new tbl_directory();

                        model.Name = Name;
                        model.ParentDirId = ParentDirId;
                        model.DirType = "w";
                        model.UserId = userId;
                        _dbcontext.tbl_directory.Add(model);

                        _dbcontext.SaveChanges();

                        _respModel.data = model;
                        _respModel.issuccess = true;
                        _respModel.message = "Wardrobe created successfully.";
                        return _respModel;
                    }
                    _respModel.issuccess = false;
                    _respModel.message = Name + " wardrobe is existing.";
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }
    }
}