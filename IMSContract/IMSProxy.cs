using OMSCommon;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace IMSContract
{
    public class IMSProxy : ClientBase<IIMSContract>, IIMSContract
    {
        public IMSProxy() : base(NetTcpBindingCreator.Create(), new EndpointAddress("net.tcp://localhost:6000/IMSService"))
        {

        }

        public void AddElementStateReport(SwitchStateReport report)
        {
            Channel.AddElementStateReport(report);
        }

        public void AddReport(IncidentReport report)
        {
            Channel.AddReport(report);
        }

        public List<SwitchStateReport> GetAllElementStateReports()
        {
            return Channel.GetAllElementStateReports();
        }

        public List<IncidentReport> GetAllReports()
        {
            return Channel.GetAllReports();
        }

        public List<Crew> GetCrews()
        {
            return Channel.GetCrews();
        }

        public List<List<SwitchStateReport>> GetElementStateReportsForMrID(string mrID)
        {
            return Channel.GetElementStateReportsForMrID(mrID);
        }
      
        public IncidentReport GetReport(DateTime id)
        {
            return Channel.GetReport(id);
        }

        public List<List<IncidentReport>> GetReportsForMrID(string mrID)
        {
            return Channel.GetReportsForMrID(mrID);
        }
      
        public List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date)
        {
            return Channel.GetReportsForSpecificDateSortByBreaker(mrids, date);
        }

        public List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids)
        {
            return Channel.GetAllReportsSortByBreaker(mrids);
        }

        public bool Ping()
        {
            return Channel.Ping();
        }

        public void UpdateReport(IncidentReport report)
        {
            Channel.UpdateReport(report);
        }
    }
}
