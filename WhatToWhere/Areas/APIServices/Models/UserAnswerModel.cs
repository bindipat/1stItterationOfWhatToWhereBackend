using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToWhere.Areas.APIServices.Models
{
    public class UserAnswerModel
    {
        public UserAnswerModel()
        {
            Answer = new List<AnsweModel>();
        }
        public int QueId { get; set; }
        public string Question { get; set; }
        public string ChoiceType { get; set; }
        public bool IsImageQuestion { get; set; }
        public List<AnsweModel> Answer { get; set; }
    }

    public class AnsweModel
    {
        public int AnsId { get; set; }
        public string DisplayText { get; set; }
        public string ImagePath { get; set; }
        public bool IsSelected { get; set; }
        public string Comment { get; set; }
    }

    public class UserAnswerRequestModel
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public List<SelectedAnswers> SelectedAnswers { get; set; }
    }

    public class SelectedAnswers
    {
        [Required]
        public int QueId { get; set; }
        [Required]
        public int AnsId { get; set; }
        public string Comment { get; set; }
   }
}