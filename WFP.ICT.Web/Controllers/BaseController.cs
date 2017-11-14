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
        int pageSize = 15;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public void SetupLoggedInUser(string UserName)
        {
            var user = Db.Users.Include(u => u.Roles).FirstOrDefault(x => x.UserName == UserName);
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

        readonly string _imagesPath = $"~/Images/";
        protected string ImagesPath
        {
            get
            {
                return Server.MapPath(_imagesPath);
            }
        }

        readonly string _signPath = $"~/Images/Sign/";
        protected string SignPath
        {
            get
            {
                return Server.MapPath(_signPath);
            }
        }

        readonly string _pianoImagesPath = $"~/Images/Piano/";
        protected string PianoImagesPath
        {
            get
            {
                return Server.MapPath(_pianoImagesPath);
            }
        }

        string _uploadsPath = "~/Uploads";
        public string UploadsPath
        {
            get
            {
                string uploadsPath = Server.MapPath(_uploadsPath);
                if (!System.IO.Directory.Exists(uploadsPath)) System.IO.Directory.CreateDirectory(uploadsPath);
                return uploadsPath;
            }
        }

        string _downloadPath = "~/Downloads";
        public string DownloadsPath
        {
            get
            {
                string downloadPath = Server.MapPath(_downloadPath);
                if (!System.IO.Directory.Exists(downloadPath)) System.IO.Directory.CreateDirectory(downloadPath);
                return downloadPath;
            }
        }

        public string UploadsFormsPath
        {
            get
            {
                return Server.MapPath("~/Uploads/Forms/");
            }
        }

        public string DownloadsFormsPath
        {
            get
            {
                return Server.MapPath("~/Downloads/Forms/");
            }
        }


        public string GlobalImagesPath
        {
            get
            {
                string uploadPath = Server.MapPath("~/assets/global/img");
                return uploadPath;
            }
        }


    }
}