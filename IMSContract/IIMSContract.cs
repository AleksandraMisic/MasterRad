using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace IMSContract
{

	[ServiceContract]
	public interface IIMSContract
	{
        [OperationContract]
        bool Ping(); 

        [OperationContract]
        void AddReport(IncidentReport report);

        [OperationContract]
		List<IncidentReport> GetAllReports();

        [OperationContract]
        IncidentReport GetReport(DateTime id);

        [OperationContract]
        void UpdateReport(IncidentReport report);

        [OperationContract]
		List<List<IncidentReport>> GetReportsForMrID(string mrID);

        [OperationContract]
        List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids);

        [OperationContract]
        List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date);
      
        [OperationContract]
        void AddElementStateReport(SwitchStateReport report);

        [OperationContract]
        List<SwitchStateReport> GetAllElementStateReports();

        [OperationContract]
        List<List<SwitchStateReport>> GetElementStateReportsForMrID(string mrID);

        [OperationContract]
        List<Crew> GetCrews();
	}
}
