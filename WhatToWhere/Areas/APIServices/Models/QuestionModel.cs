using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToWhere.Areas.APIServices.Models
{
    public class QuestionModel
    {
        [Required]
        public int QueId { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public string ChoiceType { get; set; }
        [Required]
        public bool IsImageQuestion { get; set; }
    }
}