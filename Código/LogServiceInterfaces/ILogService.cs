using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LogServiceInterfaces
{
    [ServiceContract]
    public interface ILogService
    {
        [OperationContract]
        void StartNewGameLog();

        [OperationContract]
        void AddNewLogEntry(string entry);

        [OperationContract]
        List<string> GetGameLog();

        [OperationContract]
        void AddNewGameResult(List<PlayerStats> players);

        [OperationContract]
        List<PlayerStats> TopTenScores();

        [OperationContract]
        List<Match> GetMatchesStats();
    }
}
