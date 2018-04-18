﻿using CRM.Models;
using CRM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRM.Attributes
{
    public class LogHistoryAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);

            var url = context.HttpContext.Request.Url?.AbsolutePath;
            var urlReffer = context.HttpContext.Request.UrlReferrer?.AbsolutePath;
            var user = context.HttpContext.User.Identity.Name.Split('|')[1];
            var model = new LogHistoryModel(url, urlReffer, user);
            new Task(() => { LogHistoryService.Log(model); }).Start();
        }
    }
}