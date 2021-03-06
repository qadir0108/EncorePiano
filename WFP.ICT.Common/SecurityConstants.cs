﻿using System.Collections.Generic;
using System.Linq;

namespace WFP.ICT.Common
{
    public class SecurityConstants
    {
        public const string UserAdmin = "encore.admin";
        public const string TestEmployeeUser = "test.user";
        public const string UserAdminPassword = "P@kistan1";

        public const string RoleAdmin = "Encore Admin Role";
        public const string RoleEmployee = "Encore Employee Role";
        public const string RolePrivate = "Encore Private Role";
        public const string RoleDealer = "Encore Dealer Role";
        public const string RoleManufacturer = "Encore Manufacturer Role";

        public static string[] ClaimsDefault = new string[] { "order.create", "order.read", "order.update", "order.delete" };
        public static string[] ClaimsPermissions = new string[] { "permissions.create", "permissions.read", "permissions.update", "permissions.delete" };
        public static string[] ClaimsRoles = new string[] { "roles.create", "roles.read", "roles.update", "roles.delete" };
        public static string[] ClaimsUsers = new string[] { "users.create", "users.read", "users.update", "users.delete" };
        public static string[] ClaimsAdmin = ClaimsPermissions.Concat(ClaimsRoles).Concat(ClaimsUsers).ToArray();

        public static List<string> ClaimsAll {
            get
            {
                return ClaimsDefault
                    .Concat(ClaimsAdmin)
                    .Distinct().OrderBy(x => x).ToList();
            }
        }
    }

    public class PermissionType
    {
        public static readonly string Order = "Order"; 
        public static readonly string Permission = "Permission";
    }
}
