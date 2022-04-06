using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhatToWhere.Areas.APIServices.Models
{
    public class AnswerModel
    {
        [Required]
        public int QueId { get; set; }
        [Required]
        public int AnsId { get; set; }
        public string DisplayText { get; set; }
        [Required]
        public string Value { get; set; }
        public string ImageString { get; set; }
    }
}