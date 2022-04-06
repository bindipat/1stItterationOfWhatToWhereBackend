using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatToWhere.Models
{
    public class FilterOptions
    {
        public string draw { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public string searchValue { get; set; }
    }
}