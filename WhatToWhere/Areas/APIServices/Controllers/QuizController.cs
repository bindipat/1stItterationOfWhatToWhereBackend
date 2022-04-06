using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhatToWhere.Areas.APIServices.Models;
using WhatToWhere.Models;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.APIServices.Controllers
{
    public class QuizController : ApiController
    {
        [HttpGet]
        public ResponseModel GetData(long UserId)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                List<UserAnswerModel> userAnswers = new List<UserAnswerModel>();
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var questionList = _dbcontext.tbl_question.Include("tbl_answer").ToList();
                    var useranswer = _dbcontext.tbl_useranswer.Where(x => x.UserId == UserId).ToList();
                    bool isUserDataAvailable = useranswer.Any();
                    if (questionList != null)
                    {
                        foreach (var que in questionList)
                        {
                            var questionModel = new UserAnswerModel();
                            questionModel.QueId = que.QueId;
                            questionModel.Question = que.Question;
                            questionModel.ChoiceType = que.ChoiceType;
                            questionModel.IsImageQuestion = que.IsImageQuestion;

                            if (que.tbl_answer != null && que.tbl_answer.Count > 0)
                            {
                                foreach (var ans in que.tbl_answer)
                                {
                                    var answerModel = new AnsweModel();
                                    answerModel.AnsId = ans.AnsId;
                                    answerModel.DisplayText = ans.DisplayText;
                                    answerModel.ImagePath = ans.ImagePath;
                                   
                                    if (isUserDataAvailable)
                                    { 
                                        answerModel.IsSelected = useranswer.Any(x => x.QueId == que.QueId && x.AnsId == ans.AnsId);
                                        answerModel.Comment = useranswer.Where(x => x.QueId == que.QueId && x.AnsId == ans.AnsId).Select(x => x.Comment).FirstOrDefault();
                                    }
                                    questionModel.Answer.Add(answerModel);
                                }
                            }
                            userAnswers.Add(questionModel);
                        }
                        _respModel.issuccess = true;
                        _respModel.data = userAnswers;
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Quiz details not found.";
                    }
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

        [HttpPost]
        [ValidateModel]
        public ResponseModel SetData(UserAnswerRequestModel model)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    if (model.SelectedAnswers.Any())
                    {
                        var existingUserAnswers = _dbcontext.tbl_useranswer.Where(x => x.UserId == model.UserId);
                        if (existingUserAnswers.Any())
                        {
                            foreach (var item in existingUserAnswers)
                            {
                                _dbcontext.tbl_useranswer.Remove(item);
                            }
                            _dbcontext.SaveChanges();
                        }
                        foreach (var item in model.SelectedAnswers)
                        {
                            _dbcontext.tbl_useranswer.Add(new tbl_useranswer() { AnsId = item.AnsId, QueId = item.QueId, UserId = model.UserId, Comment=item.Comment});
                        }
                        _dbcontext.SaveChanges();

                        //var answerIds = model.SelectedAnswers.Select(x => x.AnsId).ToList();
                        //var category = (from a in _dbcontext.tbl_answer
                        //                where (a.tbl_question.IsImageQuestion && answerIds.Contains(a.AnsId) && !string.IsNullOrEmpty(a.ImagePath))
                        //                group a.Value by a.Value into gr
                        //                orderby gr.Count() descending
                        //                select gr.Key).FirstOrDefault();
                        _respModel.issuccess = true;
                        _respModel.message = "Data inserted successfully.";
                        //_respModel.data = new { Category = category };
                        return _respModel;
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Answer not selected.";
                        return _respModel;
                    }
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
