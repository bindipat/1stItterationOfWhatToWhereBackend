using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatToWhere.Areas.APIServices.Models;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.Web.Controllers
{
    public class QuizController : Controller
    {
        // GET: Web/Quiz
        public ActionResult Index()
        {
            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                var data = _dbcontext.tbl_question.Include("tbl_answer").OrderByDescending(x => x.QueId).ToList();
                return View(data);
            }
        }

        public ActionResult NewQuestion(int? QueId)
        {
            tbl_question model = new tbl_question();
            if (QueId.HasValue && QueId.Value > 0)
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    model = _dbcontext.tbl_question.Include("tbl_answer").Where(x => x.QueId == QueId).FirstOrDefault();
                    if (model != null)
                    {
                        return View(model);
                    }
                    else
                    {
                        //data not found
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult SaveQuestion(QuestionModel model)
        {
            if (ModelState.IsValid)
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    if (model.QueId != 0)
                    {
                        var existsData = _dbcontext.tbl_question.Where(x => x.QueId == model.QueId).FirstOrDefault();
                        if (existsData == null)
                        {
                            return Json(new { issuccess = false, message = "Something weent wrong. Try again." }, JsonRequestBehavior.AllowGet);
                        }
                        existsData.Question = model.Question;
                        existsData.ChoiceType = model.ChoiceType;
                        existsData.IsImageQuestion = model.IsImageQuestion;
                        _dbcontext.SaveChanges();
                    }
                    else
                    {
                        tbl_question data = new tbl_question();
                        data.Question = model.Question;
                        data.ChoiceType = model.ChoiceType;
                        data.IsImageQuestion = model.IsImageQuestion;
                        _dbcontext.tbl_question.Add(data);
                        _dbcontext.SaveChanges();
                        model.QueId = data.QueId;
                    }
                }
                return Json(new { issuccess = true, QueId = model.QueId }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { issuccess = false, message = "Enter values for all fields" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveAnswer(AnswerModel model)
        {
            if (ModelState.IsValid)
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    if (model.QueId != 0 && model.AnsId != 0)
                    {
                        var existsData = _dbcontext.tbl_answer.Where(x => x.QueId == model.QueId && x.AnsId == model.AnsId).FirstOrDefault();
                        if (existsData == null)
                        {
                            return Json(new { issuccess = false, message = "Something weent wrong. Try again." }, JsonRequestBehavior.AllowGet);
                        }
                        existsData.DisplayText = model.DisplayText;
                        existsData.Value = model.Value;

                        if (!string.IsNullOrEmpty(model.ImageString))
                        {
                            //remove existing image
                            string existingImage = existsData.ImagePath;
                            if (!string.IsNullOrEmpty(existingImage))
                            {
                                string fileName = Path.GetFileName(existingImage);
                                String path = System.Web.HttpContext.Current.Server.MapPath("~/Images/Quiz");
                                System.IO.File.Delete(path + "/" + fileName);
                            }
                            existsData.ImagePath = SaveImage(model.ImageString);
                        }

                        _dbcontext.SaveChanges();
                    }
                    else
                    {
                        tbl_answer data = new tbl_answer();
                        data.QueId = model.QueId;
                        data.DisplayText = model.DisplayText;
                        data.Value = model.Value;

                        if (!string.IsNullOrEmpty(model.ImageString))
                        {
                            data.ImagePath = SaveImage(model.ImageString);
                        }

                        _dbcontext.tbl_answer.Add(data);
                        _dbcontext.SaveChanges();
                        model.AnsId = data.AnsId;
                    }
                }
                return Json(new { issuccess = true, AnsId = model.AnsId }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { issuccess = false, message = "Enter values for all fields" }, JsonRequestBehavior.AllowGet);
            }
        }

        private string SaveImage(string ImageString)
        {
            string ImgName = Guid.NewGuid().ToString();

            //Image image = Base64ToImage(Image.base64Image);
            String path = System.Web.HttpContext.Current.Server.MapPath("~/Images/Quiz"); //Path                                                                                                                     
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string imageName = ImgName + ".jpg";
            //set the image path
            string imgPath = Path.Combine(path, imageName);
            if (ImageString.Contains("data:image"))
            {
                //Need To remove some header information at the beginning if image data contains
                //ImageDataUrl = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD....";
                //Otherwise, this will give an error.
                //Remove everything in front of the DataUrl and including the first comma.
                //ImageDataUrl = "9j/4AAQSkZJRgABAQAAAQABAAD...
                ImageString = ImageString.Substring(ImageString.LastIndexOf(',') + 1);
                // removing extra header information 
            }
            byte[] imageBytes = Convert.FromBase64String(ImageString);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            image.Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            return "http://www.whattowhere.net/Images/Quiz/" + imageName;
        }

        [HttpDelete]
        public JsonResult DeleteAnswer(int AnsId)
        {
            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                if (AnsId > 0)
                {
                    fn_DeleteAnswer(AnsId);
                    return Json(new { issuccess = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { issuccess = false, message = "Something weent wrong. Try again." }, JsonRequestBehavior.AllowGet);
                }
            }

        }

        [HttpDelete]
        public JsonResult DeleteQuestion(int QueId)
        {
            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                if (QueId > 0)
                {
                    var existsData = _dbcontext.tbl_question.Where(x => x.QueId == QueId).FirstOrDefault();
                    if (existsData == null)
                    {
                        return Json(new { issuccess = false, message = "Data not found. Try again." }, JsonRequestBehavior.AllowGet);
                    }

                    fn_DeleteAnswerByQueId(existsData.QueId);

                    _dbcontext.tbl_question.Remove(existsData);
                    _dbcontext.SaveChanges();
                    return Json(new { issuccess = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { issuccess = false, message = "Something weent wrong. Try again." }, JsonRequestBehavior.AllowGet);
                }
            }

        }

        private void fn_DeleteAnswerByQueId(int queId)
        {
            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                var model = _dbcontext.tbl_question.Include("tbl_answer").Where(x => x.QueId == queId).FirstOrDefault();
               
                if(model.tbl_answer!=null || model.tbl_answer.Count > 0)
                {
                    foreach(var item in model.tbl_answer)
                    {
                        fn_DeleteAnswer(item.AnsId);
                    }
                }                 
            }
        }
        private void fn_DeleteAnswer(int ansId)
        {
            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                var model = _dbcontext.tbl_answer.Where(x => x.AnsId == ansId).FirstOrDefault();
                if (model != null)
                {

                    if (!string.IsNullOrEmpty(model.ImagePath))
                    {
                        string existingImage = model.ImagePath;
                        if (!string.IsNullOrEmpty(existingImage))
                        {
                            string fileName = Path.GetFileName(existingImage);
                            String path = System.Web.HttpContext.Current.Server.MapPath("~/Images/Quiz");
                            System.IO.File.Delete(path + "/" + fileName);
                        }
                    }
                    DeleteUserAnswer(model.AnsId);
                    _dbcontext.tbl_answer.Remove(model);
                    _dbcontext.SaveChanges();
                }
            }
        }

        private void DeleteUserAnswer(int ansId)
        {
            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                var data = _dbcontext.tbl_useranswer.Where(x => x.AnsId == ansId).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        _dbcontext.tbl_useranswer.Remove(item);
                    }
                    _dbcontext.SaveChanges();
                }
            }
        }
    }
}