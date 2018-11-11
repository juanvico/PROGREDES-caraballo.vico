using LogServiceInterfaces;
using PlayerCRUDWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlayerCRUDWebApi.Controllers
{
    public class LogsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult LastMatch()
        {
            List<string> logs = new List<string>();
            using (var logger = new Logger.LogServiceClient())
            {
                logs = logger.GetGameLog().ToList();
            }
            
            return Json(logs);
        } 
    }
}
