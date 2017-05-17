using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;
using WFP.ICT.Enum;
using WFP.ICT.Data.Entities;
using WFP.ICT.Common;
using WFP.ICT.Data.EntityManager;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    [AuthorizeRole(Roles = SecurityConstants.RoleAdmin)]
    public class CompanyController : BaseController
    {

        #region Settings

        public ActionResult Settings()
        {
            CompanySettingsVM model = new CompanySettingsVM();
            var mgr = new CompanyManager();
            var company = mgr.GetAll().FirstOrDefault();
            model = TinyMapper.Map<CompanySettingsVM>(company);
            return View(model);
        }

        [HttpPost]
        public ActionResult CheckMailSettings(CompanySettingsVM model)
        {
            try
            {
                bool IsAuthenticated = true;

                if (!IsAuthenticated)
                    throw new Exception("Email credentials are not valid.");

                return Json(new JsonResponse() { IsSucess = IsAuthenticated }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Settings(CompanySettingsVM model)
        {
            //var newCompany = TinyMapper.Map<WFP.ICT.Data.Entities.Company>(model);
            try
            {
                var mgr = new CompanyManager();
                var company = mgr.GetAll().FirstOrDefault(x => x.Id == model.ID);
                company.Name = model.Name;
                company.Details = model.Details;
                company.WebSite = model.WebSite;
                company.Logo = model.Logo;
                company.ActiveDiretoryDomain = model.ActiveDiretoryDomain;
                company.ActiveDiretoryUserName = model.ActiveDiretoryUserName;
                company.ActiveDiretoryPassword = model.ActiveDiretoryPassword;
                company.EmailServer = model.EmailServer;
                company.EmailUserName = model.EmailUserName;
                company.EmailPassword = model.EmailPassword;
                mgr.Update(company);
                MvcApplication.LoadCompany();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Offices

        //public ActionResult Offices()
        //{
        //    var companyOfficeVms = new List<CompanyOfficeVM>();
        //    var mgr = new OfficeManager();
        //    companyOfficeVms = mgr.GetAll().OrderBy(x => x.Code).Select(x => new CompanyOfficeVM()
        //    {
        //        Id = x.ID.ToString(),
        //        Code = x.Code,
        //        OfficeName = x.OfficeName,
        //        IsEditable = true,
        //        IsDeletable = true,
        //    }).ToList();
        //    return View(companyOfficeVms);
        //}

        //[HttpPost]
        //public ActionResult DeleteOffice(string Id)
        //{
        //    try
        //    {
        //        var mgr = new OfficeManager();
        //        mgr.DeleteById(Guid.Parse(Id));
        //        return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult SaveOffice(CompanyOfficeVM officeVm)
        //{
        //    try
        //    {
        //        var mgr = new OfficeManager();
        //        var code = (mgr.GetAll().Max(p => (int?)p.Code) ?? 0) + 1;

        //        if (officeVm.Id == null)
        //        {
        //            mgr.Insert(new Office()
        //            {
        //                ID = Guid.NewGuid(),
        //                Code = code,
        //                OfficeName = officeVm.OfficeName,
        //                OfficeCity = officeVm.OfficeName,
        //                OfficeCountry = "Pakistan",
        //                CreatedAt = DateTime.Now
        //            });
        //        }
        //        else
        //        {
        //            var office = mgr.GetById(Guid.Parse(officeVm.Id));
        //            office.Code = officeVm.Code;
        //            office.OfficeName = officeVm.OfficeName;
        //            office.OfficeCity = officeVm.OfficeName;
        //            mgr.Update(office);
        //        }
        //        return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion

        #region Units

        //public ActionResult Units()
        //{
        //    List<CompanyUnitVM> units = new List<CompanyUnitVM>();
        //    var mgr = new UnitManager();
        //    foreach (var unit in mgr.GetAll().OrderBy(x => x.Code))
        //    {
        //        var unitVM = new CompanyUnitVM()
        //        {
        //            Id = unit.ID.ToString(),
        //            Name = unit.UnitName,
        //            HeadID = unit.HeadID,
        //            FocalPersonID = unit.FocalPersonID,
        //            IsDeletable = false,
        //            IsEditable = false,
        //            Users = UsersList
        //        };
        //        units.Add(unitVM);
        //    }
        //    return View(units);
        //}

        //public ActionResult ChangeUnit(CompanyUnitVM model)
        //{
        //    try
        //    {
        //        var mgr = new UnitManager();
        //        var unit = mgr.GetById(Guid.Parse(model.Id));
        //        if (model.Action == "changeHead")
        //        {
        //            unit.HeadID = model.HeadID;
        //        }
        //        else if (model.Action == "changeFocalPerson")
        //        {
        //            unit.FocalPersonID = model.FocalPersonID;
        //        }
        //        mgr.Update(unit);
        //        return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion

        #region Users
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Users(Guid? officeId)
        {
            List<CompanyUsersVM> model = new List<CompanyUsersVM>();

            List<WFPUser> users = UserManager.Users.ToList();

            List<SelectItemPair> roles = RoleManager.Roles.Select(x => new SelectItemPair() { Text = x.Name, Value = x.Id }).ToList();

            //foreach (var user in users)
            foreach (var user in users.Where(x=>x.UserName != "ictpak"))
            {
                var u = new CompanyUsersVM()
                {
                    ID = Guid.Parse(user.Id),
                    Name = user.UserName,
                    Status = ((UserStatusEnum)user.Status).ToString(),
                };

                UserManager.GetRoles(user.Id);

                List<SelectItemPair> rolesThisUser = new List<SelectItemPair>();
                foreach (var role in roles)
                {
                    SelectItemPair roleThisUser = new SelectItemPair() { Text = role.Text, Value = role.Value };
                    if (user.Roles.Select(x => x.RoleId).Contains(role.Value))
                    {
                        roleThisUser.Selected = true;
                    }
                    rolesThisUser.Add(roleThisUser);
                }
                u.Roles = rolesThisUser;
                model.Add(u);
            }

            List<CompanyUsersGroupedVM> userGroup = 
                model.GroupBy(x => new { x.OfficeName, x.UnitName })
                     .Select(x => new CompanyUsersGroupedVM() {
                         OfficeName = x.Key.OfficeName,
                         UnitName = x.Key.UnitName,
                         Users = x.OrderBy(y => y.Name).ToList()
                      })
                     .OrderBy(x => x.OfficeName).ThenBy(x => x.UnitName)
                     .ToList();

            return View(userGroup);
        }
        
        public ActionResult SaveUser(CompanyUsersVM model)
        {
            try
            {
                var user = UserManager.FindById(model.ID.ToString());
                switch (model.Action)
                {
                    case "lock":
                        if (user.Status == (int)UserStatusEnum.Active)
                        {
                            user.Status = (int)UserStatusEnum.Locked;
                            user.LockoutEndDateUtc = new DateTime(9999, 12, 30);
                            bool a = UserManager.IsLockedOut(user.Id);
                        }
                        else if (user.Status == (int)UserStatusEnum.Locked)
                        {
                            user.Status = (int)UserStatusEnum.Active;
                            user.LockoutEndDateUtc = null;
                            bool a = UserManager.IsLockedOut(user.Id);
                        }
                        break;
                    case "password":
                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            string resetToken = UserManager.GeneratePasswordResetToken(user.Id);
                            IdentityResult passwordChangeResult = UserManager.ResetPassword(user.Id, resetToken, model.Password);
                            if (passwordChangeResult != IdentityResult.Success)
                            {
                                throw new Exception(passwordChangeResult.Errors.FirstOrDefault());
                            }
                        }
                        break;
                    case "create":
                        var newUser = new WFPUser()
                        {
                            Id = Guid.NewGuid().ToString(),
                            CreatedAt = DateTime.Now,
                            UserName = model.Name.Trim(),
                            Email = model.Name.Trim() + "@wfp.org",
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                        };
                        var result = UserManager.CreateAsync(newUser, model.Password);
                        if (result.Result != IdentityResult.Success)
                        {
                            throw new Exception("There is an error while creating user." + result.Result.Errors.FirstOrDefault());
                        }
                        break;
                    case "delete":
                        UserManager.Delete(user);
                        break;
                }
                db.SaveChanges();
                return Json(new JsonResponse() {IsSucess = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() {IsSucess = false, ErrorMessage = ex.Message},
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeUserRole(CompanyUsersVM model)
        {
            try
            {
                if(model.Action == "add")
                    UserManager.AddToRole(model.ID.ToString(), model.Role);
                else if(model.Action == "remove")
                    UserManager.RemoveFromRole(model.ID.ToString(), model.Role);
                db.SaveChanges();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Roles
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ActionResult Roles()
        {
            List<CompanyRoleVM> companyRoles = new List<CompanyRoleVM>();
            var mgr = new AspNetClaimsManager();
            foreach (var role in RoleManager.Roles)
            {
                var roleVm = new CompanyRoleVM()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsDeletable = role.IsDeletable,
                    IsEditable = role.IsEditable
                };
                
                List<SelectItemPair> permissions = new List<SelectItemPair>();
                foreach (var claim in mgr.GetAll("RoleClaims").ToList())
                {
                    var permission = new SelectItemPair() { Text = claim.ClaimValue, Value = claim.Id.ToString() };
                    var roleClaimRoleIds = claim.RoleClaims.Select(x => x.RoleID); // ctx.RoleClaims.Where(x => x.RoleID == roleVm.Id).Select(x => x.ClaimID);
                    if (roleClaimRoleIds.Contains(roleVm.Id))
                        permission.Selected = true;
                    permissions.Add(permission);
                }
                roleVm.Permissions = permissions;
                companyRoles.Add(roleVm);
            }
            return View(companyRoles);
        }

        [HttpPost]
        public ActionResult DeleteRole(string Id)
        {
            try
            {
                var role = RoleManager.FindById(Id);
                RoleManager.Delete(role);
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveRole(CompanyRoleVM role)
        {
            try
            {
                //var mgr = new RoleManager<WFPRole>(new RoleStore<WFPRole>(CurrentContextFromOwin));
                var roleToUpdate = RoleManager.FindById(role.Id);
                if (roleToUpdate == null)
                {
                    roleToUpdate = new WFPRole();
                    roleToUpdate.Id = Guid.NewGuid().ToString();
                    roleToUpdate.Name = role.Name;
                    roleToUpdate.Description = role.Description;
                    roleToUpdate.IsDeletable = true;
                    roleToUpdate.IsEditable = true;
                    RoleManager.Create(roleToUpdate);
                }
                else
                {
                    roleToUpdate.Name = role.Name;
                    roleToUpdate.Description = role.Description;
                    RoleManager.Update(roleToUpdate);
                }
                db.SaveChanges();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeRolePermission(CompanyRoleVM model)
        {
            try
            {
                var mgr = new AspNetRoleClaimsManager();
                if (model.Action == "add")
                {
                    mgr.Insert(new AspNetRoleClaims()
                    {
                        Id = Guid.NewGuid(),
                        RoleID = model.Id,
                        ClaimID = Guid.Parse(model.PermissionId),
                        CreatedAt = DateTime.Now
                    });
                }
                else if (model.Action == "remove")
                {
                    var roleClaim = mgr.GetAll().FirstOrDefault(x => x.RoleID == model.Id && x.ClaimID.ToString() == model.PermissionId);
                    if (roleClaim != null)
                        mgr.Delete(roleClaim);
                }
                mgr.SaveChanges();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Permissions

        public ActionResult Permissions()
        {
            var permissions = new List<CompanyPermissionVM>();
            var mgr = new AspNetClaimsManager();
            permissions = mgr.GetAll("RoleClaims").ToList().Select(x => new CompanyPermissionVM()
            {
                Id = x.Id.ToString(),
                ClaimType = x.ClaimType,
                ClaimValue = x.ClaimValue,
                RolesCount = x.RoleClaims.Count.ToString(),
                IsEditable = !SecurityConstants.ClaimsAll.Contains(x.ClaimValue),
                IsDeletable = !SecurityConstants.ClaimsAll.Contains(x.ClaimValue),
                ClaimTypes = ClaimTypes
            }).ToList();
            return View(permissions);
        }

        [HttpPost]
        public ActionResult DeletePermission(string Id)
        {
            try
            {
                var mgr = new AspNetClaimsManager();
                mgr.DeleteById(Guid.Parse(Id));
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SavePermission(CompanyPermissionVM permission)
        {
            try
            {
                var mgr = new AspNetClaimsManager();
                if (permission.Id == null)
                {
                    mgr.Insert(new AspNetClaims()
                    {
                        Id = Guid.NewGuid(),
                        ClaimType = permission.ClaimType,
                        ClaimValue = permission.ClaimValue,
                        CreatedAt = DateTime.Now
                    });
                }
                else
                {
                    var claim = mgr.GetById(Guid.Parse(permission.Id));
                    claim.ClaimValue = permission.ClaimValue;
                    claim.ClaimType = permission.ClaimType;
                    mgr.Update(claim);
                }
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }
}