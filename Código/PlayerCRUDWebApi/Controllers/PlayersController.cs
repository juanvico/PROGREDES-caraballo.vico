using PlayerCRUDServiceInterfaces;
using PlayerCRUDWebApi.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PlayerCRUDWebApi.Controllers
{
    public class PlayersController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Get()
        {
            List<PlayerModel> ret = new List<PlayerModel>();
            using (var players = new CRUDService.PlayerCRUDServiceClient())
            {
                foreach (Player p in players.GetPlayers())
                {
                    ret.Add(PlayerModel.ToModel(p));
                }
            }
            return Json(ret);
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            using (var players = new CRUDService.PlayerCRUDServiceClient())
            {
                if (!players.Exists(id))
                {
                    return NotFound();
                }
                return Json(players.Get(id));
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]PlayerModel model)
        {
            using (var players = new CRUDService.PlayerCRUDServiceClient())
            {
                Player player = players.Add(PlayerModel.ToEntity(model));
                return Json(new { success = true });
            }
        }

        [HttpPut]
        public IHttpActionResult Put(Guid id, [FromBody]PlayerModel model)
        {
            using (var players = new CRUDService.PlayerCRUDServiceClient())
            {
                if (!players.Exists(id))
                {
                    return NotFound();
                }
                players.Update(id, PlayerModel.ToEntity(model));
            }
            return Json(new { success = true });
        }

        [HttpDelete]
        public IHttpActionResult Delete(Guid id)
        {
            using (var players = new CRUDService.PlayerCRUDServiceClient())
            {
                players.Delete(id);
            }
            return Json(new { success = true });
        }
    }
}
