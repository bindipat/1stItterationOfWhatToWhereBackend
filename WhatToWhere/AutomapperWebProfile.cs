using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WhatToWhere.Areas.APIServices.Models.DataModel;
using WhereToWhere.Data;

namespace WhatToWhere
{
    public class AutomapperWebProfile : AutoMapper.Profile
    {
        public AutomapperWebProfile()
        {
            CreateMap<tbl_user, UserModel>();
        }

        public static void Run()
        {
            AutoMapper.Mapper.Initialize(a =>
            {
                a.AddProfile<AutomapperWebProfile>();
            });
        }
    }
}