using System;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Security;
using System.Data.SqlClient;
using System.Data.Entity.Core;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace WFP.ICT.Web.Helpers
{
    public static class ExceptionHelper
    {
        public static string GetExceptionMessage(this Exception ex, string customMessage = "")
        {
            var message = (customMessage != "")
                ? customMessage + " " + ex.GetBaseException().Message
                : ex.GetBaseException().Message;

            while (ex.InnerException != null)
            {
                message += "</br>" + ex.InnerException.Message;
                ex = ex.InnerException;
            }

            if (ex is SqlException)
            {
                return "Could not connect to database" + message;
            }
            else if (ex is IndexOutOfRangeException)
            {
                return "IndexOutOfRangeException" + message;
            }
            else if (ex is DbEntityValidationException)
            {
                var exception = (DbEntityValidationException)ex;
                var errorMessages = exception.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var messages = errorMessages.Aggregate("",
                    (current, errorMessage) => current + ("<li>" + errorMessage + "</li>"));
                return message + "<br/>" + messages;
            }
            else if (ex is EntityCommandExecutionException)
            {
                return "Failed to query the DB. Trying again." + message;
            }
            else if (ex is SecurityException)
            {
                return "SecurityException" + message;
            }
            else if (ex is SystemException)
            {
                return message;
            }
            else if (ex is ApplicationException)
            {
                return message;
            }
            else
            {
                return message;
            }
        }
        public static void HandleException(this Exception ex, string customMessage = "")
        {
            var message = (customMessage != "")
                ? customMessage + " " + ex.GetBaseException().Message
                : ex.GetBaseException().Message;
            if (ex is SqlException)
            {
                //return "Could not connect to database" + message;
            }
            else if (ex is IndexOutOfRangeException)
            {
                //return "IndexOutOfRangeException" + message;
            }
            else if (ex is DbEntityValidationException)
            {
                var exception = (DbEntityValidationException)ex;
                var errorMessages = exception.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var messages = errorMessages.Aggregate("",
                    (current, errorMessage) => current + ("<li>" + errorMessage + "</li>"));
                //return message + "<br/>" + messages;
            }
            else if (ex is EntityCommandExecutionException)
            {
                //return "Failed to query the DB. Trying again." + message;
            }
            else if (ex is SecurityException)
            {
                //return "SecurityException" + message;
            }
            else if (ex is SystemException)
            {
                //return message;
            }
            else if (ex is ApplicationException)
            {
                //return message;
            }
            else
            {
                //return message;
            }
        }
    }
    /// <summary>
    /// GetModelStateErrors loops all model errors and return HTML list
    /// </summary>
    public static class ControllerExceptionHelper
    {
        public static string GetModelStateErrors(this ICollection<ModelState> values)
        {
            var sb = new StringBuilder();
            foreach (var modelState in values)
            {
                foreach (var error in modelState.Errors)
                {
                    var li = new TagBuilder("li");
                    li.SetInnerText(error.ErrorMessage);
                    sb.Append(li);
                }
            }
            return sb.ToString();
        }
    }
}