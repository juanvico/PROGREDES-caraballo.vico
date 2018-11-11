using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace PlayerCRUDServiceInterfaces
{
    [ServiceContract]
    public interface IPlayerCRUDService
    {
        [OperationContract]
        Player Add(Player player);

        [OperationContract]
        Player Get(Guid id);

        [OperationContract]
        Player Update(Guid id, Player updatedPlayer);

        [OperationContract]
        List<Player> GetPlayers();

        [OperationContract]
        void Delete(Guid id);

        [OperationContract]
        bool Exists(Guid id);

        [OperationContract]
        bool ExistsByNickname(string nickname);
    }
}
