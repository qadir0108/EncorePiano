﻿using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using System;
using System.Globalization;
using System.Linq;
using Hangfire;
using Microsoft.AspNet.Identity.EntityFramework;
using WFP.ICT.Common;
using WFP.ICT.Data.Entities;
using WFP.ICT.Web.Async;
using WFP.ICT.Web.Helpers;

[assembly: OwinStartup(typeof(WFP.ICT.Web.Startup))]
namespace WFP.ICT.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            ConfigureAuth(app);

            using (var db = new WFPICTContext())
            {
                var ad = db.Addresses.FirstOrDefault();
            }
            
            GlobalConfiguration.Configuration.UseSqlServerStorage("WFPICTContext");
            //app.UseHangfireDashboard();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseHangfireServer();
        }
    }
}
