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
    public class RankingsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            List<PlayerStatsModel> ret = new List<PlayerStatsModel>();
            using (var logger = new Logger.LogServiceClient())
            {
                foreach (PlayerStats p in logger.TopTenScores())
                {
                    ret.Add(PlayerStatsModel.ToModel(p));
                }
            }

            return Json(ret);
        }
    }
}
