using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WhatToWhere.Areas.APIServices.Models;
using WhatToWhere.Filter;
using WhatToWhere.Models;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.APIServices.Controllers
{
    
    [Route("api/post/{action}")]
    [APILogFilter]

    public class PostController : ApiController
    {
        [HttpPost]
       
        public ResponseModel CreatePost()
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                string filePath = "";
                tbl_post model = new tbl_post();
                model.CreatedDate = DateTime.Now;
                model.LikeCount = 0;
                model.DislikeCount = 0;

                if (HttpContext.Current.Request.Form.AllKeys.Contains("UserId"))
                {
                    model.CreatedBy = Convert.ToInt64(HttpContext.Current.Request.Form["UserId"]);
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "UserId parameter is missing.";
                    return _respModel;
                }
                if (HttpContext.Current.Request.Form.AllKeys.Contains("Description"))
                {
                    model.Description = Convert.ToString(HttpContext.Current.Request.Form["Description"]);
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Description parameter is missing.";
                    return _respModel;
                }
                if (HttpContext.Current.Request.Form.AllKeys.Contains("ShareType"))
                {
                    model.ShareType = Convert.ToString(HttpContext.Current.Request.Form["ShareType"]);
                    if (model.ShareType.ToLower() != "public" && model.ShareType.ToLower() != "private")
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "ShareType should be 'private' or 'public'.";
                        return _respModel;
                    }
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "ShareType parameter is missing.";
                    return _respModel;
                }

                var file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = model.CreatedBy + "_postpic_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var path = HttpContext.Current.Server.MapPath("~/Images/" + model.CreatedBy + "/post/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    file.SaveAs(Path.Combine(path, fileName));
                    filePath = "www.whattowhere.net/Images/" + model.CreatedBy + "/post/" + fileName;
                    model.ImagePath = filePath;
                }

                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    _dbcontext.tbl_post.Add(model);
                    _dbcontext.SaveChanges();
                    _respModel.issuccess = true;
                    _respModel.message = "Post created successfully.";
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

        [HttpGet]
        public ResponseModel GetMyPost(long UserId)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                if (UserId >= 0)
                {
                    using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                    {
                        var data = _dbcontext.tbl_post.Where(x => x.CreatedBy == UserId).OrderByDescending(x => x.CreatedDate).ToList();
                        _respModel.issuccess = true;
                        _respModel.data = data;
                    }
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Invalid input details";
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

        [HttpGet]
        public ResponseModel GetPostForDashboard(long UserId)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                if (UserId >= 0)
                {
                    using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                    {
                        var data = _dbcontext.vw_dashboard_post.Where(x => x.UserId == 0 || x.UserId == UserId).OrderByDescending(x => x.CreatedDate).ToList();
                        _respModel.issuccess = true;
                        _respModel.data = data;
                    }
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Invalid input details";
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