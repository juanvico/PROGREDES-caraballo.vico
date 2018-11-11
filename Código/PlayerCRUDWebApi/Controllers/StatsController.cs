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
    public class StatsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            List<MatchModel> ret = new List<MatchModel>();

            using (var logger = new Logger.LogServiceClient())
            {
                foreach (Match m in logger.GetMatchesStats())
                {
                    ret.Add(MatchModel.ToModel(m));
                }
            }
            
            return Json(ret);
        }
    }
}
