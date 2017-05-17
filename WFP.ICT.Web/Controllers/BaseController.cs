using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WFP.ICT.Data.Entities;
using WFP.ICT.Data.EntityManager;
using WFP.ICT.Enum;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.Controllers
{
    public class BaseController : BaseControllerLists 
    {
        public void SetupLoggedInUser(string UserName)
        {
            var user = db.Users.Include(u => u.Roles).FirstOrDefault(x => x.UserName == UserName);
            Session["user"] = user;
        }

        public WFPUser LoggedInUser
        {
            get
            {
                return Session["user"] as WFPUser;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return LoggedInUser != null && (LoggedInUser.UserType == (int)UserTypeEnum.Admin);
            }
        }

        string _uploadPath = "~/Uploads";
        public string UploadPath
        {
            get
            {
                string uploadPath = Server.MapPath(_uploadPath);
                if (!System.IO.Directory.Exists(uploadPath)) System.IO.Directory.CreateDirectory(uploadPath);
                return uploadPath;
            }
        }

    }
}