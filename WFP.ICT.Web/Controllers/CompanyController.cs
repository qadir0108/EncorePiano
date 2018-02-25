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
using DataTables.Mvc;
using System.Text;

namespace WFP.ICT.Web.Controllers
{
    [Authorize]
    [AjaxAuthorize]
    //[AuthorizeRole(Roles = SecurityConstants.RoleAdmin)]
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
                var company = mgr.GetAll().FirstOrDefault(x => x.Id == model.Id);
                company.Name = model.Name;
                company.Details = model.Details;
                company.WebSite = model.WebSite;
                company.Logo = model.Logo;
                mgr.Update(company);
                MvcApplication.LoadCompany();

                TempData["Success"] = $"Settings has been saved sucessfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "This is error while saving settings." + ex.Message;
            }
            return RedirectToAction("Settings");
        }

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

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult InitializeUsers([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            List<CompanyUsersVm> users = new List<CompanyUsersVm>();

            List<WFPUser> usersList = UserManager.Users.ToList();
            List<SelectItemPair> roles = RoleManager.Roles.Select(x => new SelectItemPair() { Text = x.Name, Value = x.Id }).ToList();

            foreach (var user in usersList)
            {
                var u = new CompanyUsersVm()
                {
                    Id = Guid.Parse(user.Id),
                    Name = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
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
                users.Add(u);
            }

            var totalCount = users.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                users = users.Where(p => p.Name.Contains(value)).ToList();
            }

            var filteredCount = users.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            if (sortedColumns.Count() > 0)
            {
                foreach (var column in sortedColumns)
                {
                    if (column.Data == "Name")
                    {
                        users = column.SortDirection.ToString() == "Ascendant" ?
                                    users.OrderBy(x => x.Name).ToList() :
                                    users.OrderByDescending(x => x.Name).ToList();
                    }

                    if (column.Data == "FirstName")
                    {
                        users = column.SortDirection.ToString() == "Ascendant" ?
                                    users.OrderBy(x => x.FirstName ).ToList() :
                                    users.OrderByDescending(x => x.FirstName).ToList();
                    }

                    if (column.Data == "LastName")
                    {
                        users = column.SortDirection.ToString() == "Ascendant" ?
                                    users.OrderBy(x => x.LastName).ToList() :
                                    users.OrderByDescending(x => x.LastName).ToList();
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                users = users.OrderBy(x => x.Name).ToList();
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                users = users.Skip(requestModel.Start).Take(requestModel.Length).ToList();
            }

            var result = users.
                         ToList()
                        .Select(x => new
                        {
                            Id = x.Id.ToString(),
                            Name = x.Name,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Status = x.Status,
                            Roles = GetRoles(x),
                            Actions = GetActionsUsers(x),
                        });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetRoles(CompanyUsersVm user)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<select class='roles' multiple='multiple' style='width:100%' id='{0}'>", user.Id);
            foreach (var item in user.Roles)
            {
                var selected = item.Selected ? "selected" : "";
                sb.AppendFormat(@"<option value = '{0}' {1} > {2} </option>", item.Value, selected, item.Text);
            }
            sb.AppendFormat(@"</select>");
            return sb.ToString();
        }

        public string GetActionsUsers(CompanyUsersVm user)
        {
            StringBuilder sb = new StringBuilder();
            if (user.Status == "Active")
            {
                sb.AppendFormat(@"<a href='#' id='btnLock' class='btn btn-default btnLock' data-tooltip='tooltip' data-id='{0}' title='Lock'>
                             <i class='fa fa-lock' aria-hidden='true'></i>
                             </span>
                          </a>", user.Id.ToString());
            } else
            {
                sb.AppendFormat(@"<a href='#' id='btnUnLock' class='btn btn-default btnLock' data-tooltip='tooltip' data-id='{0}' title='Un-Lock'>
                             <i class='fa fa-unlock' aria-hidden='true'></i>
                             </span>
                          </a>", user.Id.ToString());
            }
            sb.AppendFormat(@"<a href='#' id='btnOpenChangePassword' name='btnOpenChangePassword' class='btn btn-default btnOpenChangePassword' title='Change Password'
                                       data-toggle = 'modal' data-target = '#myModal' data-id = '{0}' data-user = '{1}' ><i class='fa fa-key' aria-hidden='true'></i></a>
                        <a class='btnDelete' data-id={0} href='#' data-tooltip='tooltip' title='Delete'>
                        <span class='glyphicon glyphicon-trash'></span>
                        </a>", user.Id.ToString(), user.Name);
            return sb.ToString();
        }

        public ActionResult SaveUser(CompanyUsersVm model)
        {
            try
            {
                var user = UserManager.FindById(model.Id.ToString());
                switch (model.ActionToTake)
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
                Db.SaveChanges();
                return Json(new JsonResponse() {IsSucess = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() {IsSucess = false, ErrorMessage = ex.Message},
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeUserRole(CompanyUsersVm model)
        {
            try
            {
                if(model.ActionToTake == "add")
                    UserManager.AddToRole(model.Id.ToString(), model.Role.Trim());
                else if(model.ActionToTake == "remove")
                    UserManager.RemoveFromRole(model.Id.ToString(), model.Role.Trim());
                Db.SaveChanges();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteUser(string Id)
        {
            try
            {
                var user = UserManager.FindById(Id);
                UserManager.Delete(user);
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
            return View();
        }

        public ActionResult InitializeRoles([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            var mgr = new AspNetClaimsManager();

            List<CompanyRoleVm> roles = new List<CompanyRoleVm>();
            foreach (var role in RoleManager.Roles)
            {
                var roleVm = new CompanyRoleVm()
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
                roles.Add(roleVm);
            }

            var totalCount = roles.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                roles = roles.Where(p => p.Name.Contains(value)).ToList();
            }

            var filteredCount = roles.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            if (sortedColumns.Count() > 0)
            {
                foreach (var column in sortedColumns)
                {
                    if (column.Data == "Name")
                    {
                        roles = column.SortDirection.ToString() == "Ascendant" ?
                                    roles.OrderBy(x => x.Name).ToList() :
                                    roles.OrderByDescending(x => x.Name).ToList();
                    }

                    if (column.Data == "Description")
                    {
                        roles = column.SortDirection.ToString() == "Ascendant" ?
                                    roles.OrderBy(x => x.Description).ToList() :
                                    roles.OrderByDescending(x => x.Description).ToList();
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                roles = roles.OrderBy(x => x.Name).ToList();
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                roles = roles.Skip(requestModel.Start).Take(requestModel.Length).ToList();
            }

            var result = roles.
                         ToList()
                        .Select(x => new
                        {
                            Id = x.Id.ToString(),
                            Name = x.Name,
                            Description = x.Description,
                            Permissions = GetPermissions(x),
                            Actions = GetActionsRoles(x.Id),
                        });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetPermissions(CompanyRoleVm role)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<select class='permissions' multiple='multiple' style='width:100%' id='{0}'>", role.Id);
            foreach(var item in role.Permissions)
            {
                var selected = item.Selected ? "selected" : "";
                sb.AppendFormat(@"<option value = '{0}' {1} > {2} </option>", item.Value, selected, item.Text);
            }
            sb.AppendFormat(@"</select>");
            return sb.ToString();
        }

        public string GetActionsRoles(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<a href='#' class='btnEdit' data-tooltip='tooltip' data-id='{0}' title='Edit'>
                             <span class='glyphicon glyphicon-pencil'>
                             </span>
                          </a>
                           <a class='btnDelete' data-id={0} href='#' data-tooltip='tooltip' title='Delete'>
                            <span class='glyphicon glyphicon-trash'></span>
                            </a>", id);
            return sb.ToString();
        }

        [HttpPost]
        public ActionResult EditRole(Guid? id)
        {
            try
            {
                var role = RoleManager.Roles.ToList().FirstOrDefault(x => x.Id == id.ToSafeString());
                var model = new CompanyRoleVm()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsDeletable = role.IsDeletable,
                    IsEditable = role.IsEditable
                };
                return PartialView("~/Views/Company/RoleAdd.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult SaveRole(CompanyRoleVm role)
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
                Db.SaveChanges();
                return Json(new JsonResponse() { IsSucess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeRolePermission(CompanyRoleVm model)
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
            TempData["ClaimTypes"] = new SelectList(ClaimTypes, "Value", "Text");
            return View();
        }

        public ActionResult InitializePermissions([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            var mgr = new AspNetClaimsManager();
            IEnumerable<CompanyPermissionVm> permissions = mgr.GetAll("RoleClaims").Select(x => new CompanyPermissionVm()
            {
                Id = x.Id.ToString(),
                ClaimType = x.ClaimType,
                ClaimValue = x.ClaimValue,
                RolesCount = x.RoleClaims.Count.ToString(),
                IsEditable = !SecurityConstants.ClaimsAll.Contains(x.ClaimValue),
                IsDeletable = !SecurityConstants.ClaimsAll.Contains(x.ClaimValue)
            }).ToList();

            var totalCount = permissions.Count();

            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                permissions = permissions.AsEnumerable().
                                          Where(p => p.ClaimValue.Contains(value)
                                         );
            }

            var filteredCount = permissions.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            if (sortedColumns.Count() > 0)
            {
                foreach (var column in sortedColumns)
                {
                    if (column.Data == "ClaimValue")
                    {
                        permissions = column.SortDirection.ToString() == "Ascendant" ?
                                    permissions.OrderBy(x => x.ClaimValue) :
                                    permissions.OrderByDescending(x => x.ClaimValue);
                    }

                    if (column.Data == "ClaimType")
                    {
                        permissions = column.SortDirection.ToString() == "Ascendant" ?
                                    permissions.OrderBy(x => x.ClaimType) :
                                    permissions.OrderByDescending(x => x.ClaimType);
                    }

                    if (column.Data == "RolesCount")
                    {
                        permissions = column.SortDirection.ToString() == "Ascendant" ?
                                    permissions.OrderBy(x => x.RolesCount) :
                                     permissions.OrderByDescending(x => x.RolesCount);
                    }
                }
                orderByString = "Ordered";
            }

            if (orderByString == string.Empty)
            {
                permissions = permissions.OrderBy(x => x.ClaimType);
            }
            #endregion Sorting

            // Paging
            if (requestModel.Length != -1)
            {
                permissions = permissions.Skip(requestModel.Start).Take(requestModel.Length);
            }

            var result = permissions.
                         ToList()
                        .Select(x => new
                        {
                            Id = x.Id.ToString(),
                            ClaimType = x.ClaimType,
                            ClaimValue = x.ClaimValue,
                            RolesCount = x.RolesCount.ToString(),
                            Actions = GetActionsPermissions(x.Id),
                        });

            return Json(new DataTablesResponse
            (requestModel.Draw, result, filteredCount, totalCount),
                        JsonRequestBehavior.AllowGet);
        }

        public string GetActionsPermissions(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<a href='#' class='btnEdit' data-tooltip='tooltip' data-id='{0}' title='Edit'>
                             <span class='glyphicon glyphicon-pencil'>
                             </span>
                          </a>
                           <a class='btnDelete' data-id={0} href='#' data-tooltip='tooltip' title='Delete'>
                            <span class='glyphicon glyphicon-trash'></span>
                            </a>", id);
            return sb.ToString();
        }

        [HttpPost]
        public ActionResult EditPermission(Guid? id)
        {
            try
            {
                var mgr = new AspNetClaimsManager();
                var claim = mgr.GetById(id.Value, "RoleClaims");
                var model = new CompanyPermissionVm()
                {
                    Id = claim.Id.ToString(),
                    ClaimType = claim.ClaimType,
                    ClaimValue = claim.ClaimValue,
                    RolesCount = claim.RoleClaims.Count.ToString(),
                    IsEditable = !SecurityConstants.ClaimsAll.Contains(claim.ClaimValue),
                    IsDeletable = !SecurityConstants.ClaimsAll.Contains(claim.ClaimValue)
                };
                TempData["ClaimTypes"] = new SelectList(ClaimTypes, "Value", "Text");
                return PartialView("~/Views/Company/PermissionAdd.cshtml", model);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult SavePermission(CompanyPermissionVm permission)
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