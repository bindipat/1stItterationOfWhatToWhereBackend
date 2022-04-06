using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.APIServices.Models
{
    public class PostModel:tbl_post
    {
        //public long UserId { get; set; }
        //public string ImagePath { get; set; }
        //public string Description { get; set; }
        //public string ShareType { get; set; }
    }

    public class DashboardPostModel: vw_dashboard_post
    { }
}