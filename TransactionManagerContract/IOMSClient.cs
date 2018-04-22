using DMSCommon.Model;
using FTN.Common;
using IMSContract;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace TransactionManagerContract
{
    [ServiceContract]
   public interface IOMSClient
    {
        /*CIMAdapter methods*/

        /// <summary>
        /// Update system Static Data. Called by ModelLabs (CIMAdapter) when Static data changes
        /// </summary>
        /// <param name="d">Delta</param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateSystem(Delta d);

        /*DispatcherApp methods*/
        
        [OperationContract]
        TMSAnswerToClient GetNetwork();

        [OperationContract]
        List<Source> GetSources();

        [OperationContract]
        void SendCommandToSCADA(TypeOfSCADACommand command, string mrid, OMSSCADACommon.CommandTypes commandtype, float value);

        [OperationContract]
        void SendCrew(IncidentReport report, Crew crew);

        [OperationContract]
        List<IncidentReport> GetAllIncidentReports();

        [OperationContract]
        List<List<IncidentReport>> GetReportsForMrID(string mrID);

        [OperationContract]
        List<List<SwitchStateReport>> GetElementStateReportsForMrID(string mrID);

        [OperationContract]
        List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date);

        [OperationContract]
        List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids);

        [OperationContract]
        void ClearNMSDB();      
    }
}
