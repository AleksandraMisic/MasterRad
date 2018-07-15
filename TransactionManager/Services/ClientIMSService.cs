using IMSContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionManager.Services
{
    public class ClientIMSService : IIMSContract
    {
        public void AddElementStateReport(SwitchStateReport report)
        {
            throw new NotImplementedException();
        }

        public void AddReport(IncidentReport report)
        {
            throw new NotImplementedException();
        }

        public List<SwitchStateReport> GetAllElementStateReports()
        {
            throw new NotImplementedException();
        }

        public List<IncidentReport> GetAllReports()
        {
            throw new NotImplementedException();
        }

        public List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids)
        {
            throw new NotImplementedException();
        }

        public List<Crew> GetCrews()
        {
            throw new NotImplementedException();
        }

        public List<List<SwitchStateReport>> GetElementStateReportsForMrID(string mrID)
        {
            throw new NotImplementedException();
        }

        public IncidentReport GetReport(DateTime id)
        {
            throw new NotImplementedException();
        }

        public List<List<IncidentReport>> GetReportsForMrID(string mrID)
        {
            throw new NotImplementedException();
        }

        public List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date)
        {
            throw new NotImplementedException();
        }

        public bool Ping()
        {
            throw new NotImplementedException();
        }

        public void UpdateReport(IncidentReport report)
        {
            throw new NotImplementedException();
        }
    }
}
