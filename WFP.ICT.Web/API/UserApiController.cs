using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;
using WFP.ICT.Web.Helpers;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.API
{
    public class UserApiController : BaseApiController
    {
        // POST: api/user/login
        [Route("api/user/login")]
        public JsonResult<JsonResponse> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                var driver = db.Drivers.FirstOrDefault(x => x.Code == loginModel.Username && x.Password == loginModel.Password);
                var isValidDriver = driver != null;
                if (!isValidDriver)
                {
                    throw new Exception("User authentication failed.");
                }

                var token = KeyGenerator.GetUniqueKey(32);
                db.DriverLogins.Add(new DriverLogin()
                {
                    Id = Guid.NewGuid(),
                    DriverId = driver.Id,
                    CreatedAt = DateTime.Now,
                    Token = token
                });
                db.APIRequestLogs.Add(new APIRequestLog()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    RequestType = (int)RequestTypeEnum.Login
                });
                db.SaveChanges();

                driver.FCMToken = loginModel.FCMToken;
                db.SaveChanges();

                return Json(new JsonResponse() { IsSucess = true, IsTokenValid = true, AuthToken = token });
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse() { IsSucess = false, ErrorMessage = ex.Message });
            }
        }

    }
}
