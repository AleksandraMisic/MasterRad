using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DMSCommon.Model;
using DMSContract;
using FTN.Common;
using IMSContract;
using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using SCADAContracts;
using TransactionManagerContract;

namespace TransactionManager
{
    public class OMSClient : IOMSClient
    {
        IDMSContract proxyToDispatcherDMS;
        ModelGDATMS gdaTMS;

        private IMSClient imsClient;
        private IMSClient IMSClient
        {
            get
            {
                if (imsClient == null)
                {
                    NetTcpBinding binding = new NetTcpBinding();
                    binding.CloseTimeout = TimeSpan.FromMinutes(10);
                    binding.OpenTimeout = TimeSpan.FromMinutes(10);
                    binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
                    binding.SendTimeout = TimeSpan.FromMinutes(10);
                    binding.MaxReceivedMessageSize = Int32.MaxValue;
                    imsClient = new IMSClient(new EndpointAddress("net.tcp://localhost:6090/IncidentManagementSystemService"), binding);
                }
                return imsClient;
            }
            set { imsClient = value; }
        }

        private SCADAClient scadaClient;
        private SCADAClient ScadaClient
        {
            get
            {
                NetTcpBinding binding = new NetTcpBinding();
                binding.CloseTimeout = TimeSpan.FromMinutes(10);
                binding.OpenTimeout = TimeSpan.FromMinutes(10);
                binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
                binding.SendTimeout = TimeSpan.FromMinutes(10);
                binding.MaxReceivedMessageSize = Int32.MaxValue;

                if (scadaClient == null)
                {
                    scadaClient = new SCADAClient(new EndpointAddress("net.tcp://localhost:4000/SCADAService"), binding);
                }
                return scadaClient;
            }
            set { scadaClient = value; }
        }

        #region IOMSClient CIMAdapter Methods

        /// <summary>
        /// Called by ModelLabs(CIMAdapter) when Static data changes
        /// </summary>
        /// <param name="d">Delta</param>
        /// <returns></returns>
        public bool UpdateSystem(Delta d)
        {
            Console.WriteLine("Update System started." + d.Id);
            //Enlist(d);
            return true;
        }

        public void ClearNMSDB()
        {
            using (NMSAdoNet ctx = new NMSAdoNet())
            {
                var tableNames = ctx.Database.SqlQuery<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME NOT LIKE '%Migration%'").ToList();
                foreach (var tableName in tableNames)
                {
                    ctx.Database.ExecuteSqlCommand(string.Format("DELETE FROM {0}", tableName));
                }

                ctx.SaveChanges();
            }
        }

        #endregion

        #region  IOMSClient DispatcherApp Methods

        public TMSAnswerToClient GetNetwork()
        {
            // ako se ne podignu svi servisi na DMSu, ovde pada
            List<Element> listOfDMSElement = new List<Element>();
            try
            {
                listOfDMSElement = proxyToDispatcherDMS.GetAllElements();
            }
            catch (Exception e) { }

            List<ResourceDescription> resourceDescriptionFromNMS = new List<ResourceDescription>();
            List<ResourceDescription> descMeas = new List<ResourceDescription>();

            gdaTMS.GetExtentValues(ModelCode.BREAKER).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.CONNECTNODE).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ENERGCONSUMER).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ENERGSOURCE).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ACLINESEGMENT).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.DISCRETE).ForEach(u => resourceDescriptionFromNMS.Add(u));
            gdaTMS.GetExtentValues(ModelCode.ANALOG).ForEach(u => resourceDescriptionFromNMS.Add(u));

            int GraphDeep = proxyToDispatcherDMS.GetNetworkDepth();

            try
            {
                Command c = MappingEngineTransactionManager.Instance.MappCommand(TypeOfSCADACommand.ReadAll, "", 0, 0);

                do
                {
                    try
                    {
                        if (ScadaClient.State == CommunicationState.Created)
                        {
                            ScadaClient.Open();
                        }

                        if (ScadaClient.Ping())
                            break;
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e);
                        Console.WriteLine("GetNetwork() -> SCADA is not available yet.");
                        NetTcpBinding binding = new NetTcpBinding();
                        binding.CloseTimeout = TimeSpan.FromMinutes(10);
                        binding.OpenTimeout = TimeSpan.FromMinutes(10);
                        binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
                        binding.SendTimeout = TimeSpan.FromMinutes(10);
                        binding.MaxReceivedMessageSize = Int32.MaxValue;
                        if (ScadaClient.State == CommunicationState.Faulted)
                            ScadaClient = new SCADAClient(new EndpointAddress("net.tcp://localhost:4000/SCADAService"), binding);
                    }
                    Thread.Sleep(500);
                } while (true);
                Console.WriteLine("GetNetwork() -> SCADA is available.");

                Response r = ScadaClient.ExecuteCommand(c);
                descMeas = MappingEngineTransactionManager.Instance.MappResult(r);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            do
            {
                try
                {
                    if (IMSClient.State == CommunicationState.Created)
                    {
                        IMSClient.Open();
                    }

                    if (IMSClient.Ping())
                        break;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                    Console.WriteLine("GetNetwork() -> IMS is not available yet.");
                    if (IMSClient.State == CommunicationState.Faulted)
                    {
                        NetTcpBinding binding = new NetTcpBinding();
                        binding.CloseTimeout = TimeSpan.FromMinutes(10);
                        binding.OpenTimeout = TimeSpan.FromMinutes(10);
                        binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
                        binding.SendTimeout = TimeSpan.FromMinutes(10);
                        binding.MaxReceivedMessageSize = Int32.MaxValue;
                        IMSClient = new IMSClient(new EndpointAddress("net.tcp://localhost:6090/IncidentManagementSystemService"), binding);
                    }
                }
                Thread.Sleep(1000);
            } while (true);

            var crews = IMSClient.GetCrews();
            var incidentReports = IMSClient.GetAllReports();

            TMSAnswerToClient answer = new TMSAnswerToClient(resourceDescriptionFromNMS, listOfDMSElement, GraphDeep, descMeas, crews, incidentReports);
            return answer;
        }

        public List<Source> GetSources()
        {
            List<Source> listOfDMSSources = new List<Source>();
            try
            {
                listOfDMSSources = proxyToDispatcherDMS.GetAllSources();
            }
            catch (Exception e) { }

            return listOfDMSSources;
        }

        public void SendCommandToSCADA(TypeOfSCADACommand command, string mrid, CommandTypes commandtype, float value)
        {
            try
            {
                Command c = MappingEngineTransactionManager.Instance.MappCommand(command, mrid, commandtype, value);

                // to do: ping
                Response r = scadaClient.ExecuteCommand(c);
                //Response r = SCADAClientInstance.ExecuteCommand(c);

            }
            catch (Exception e)
            { }
        }

        public void SendCrew(IncidentReport report, Crew crew)
        {
            proxyToDispatcherDMS.SendCrewToDms(report, crew);
            return;
        }

        private ScadaDelta GetDeltaForSCADA(Delta d)
        {
            // zasto je ovo bitno, da ima measurement direction?? 
            // po tome odvajas measuremente od ostatka?
            List<ResourceDescription> rescDesc = d.InsertOperations.Where(u => ((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(u.Id) == DMSType.ANALOG) || ((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(u.Id) == DMSType.DISCRETE)).ToList();
            ScadaDelta scadaDelta = new ScadaDelta();

            foreach (ResourceDescription rd in rescDesc)
            {
                ScadaElement element = new ScadaElement();
                if (rd.ContainsProperty(ModelCode.MEASUREMENT_TYPE))
                {
                    string type = rd.GetProperty(ModelCode.MEASUREMENT_TYPE).ToString();
                    if (type == "Analog")
                    {
                        element.Type = DeviceTypes.ANALOG;
                        element.UnitSymbol = ((UnitSymbol)rd.GetProperty(ModelCode.MEASUREMENT_UNITSYMB).AsEnum()).ToString();
                        element.WorkPoint = rd.GetProperty(ModelCode.ANALOG_NORMVAL).AsFloat();
                    }
                    else if (type == "Discrete")
                    {
                        element.Type = DeviceTypes.DIGITAL;
                    }
                }

                element.ValidCommands = new List<CommandTypes>() { CommandTypes.CLOSE, CommandTypes.OPEN };
                element.ValidStates = new List<OMSSCADACommon.States>() { OMSSCADACommon.States.CLOSED, OMSSCADACommon.States.OPEN };

                if (rd.ContainsProperty(ModelCode.IDOBJ_MRID))
                {
                    //element.Name = rd.GetProperty(ModelCode.IDOBJ_NAME).ToString();
                    element.Name = rd.GetProperty(ModelCode.IDOBJ_MRID).ToString();
                }
                scadaDelta.InsertOps.Add(element);
            }
            return scadaDelta;
        }

        public List<List<SwitchStateReport>> GetElementStateReportsForMrID(string mrID)
        {
            return IMSClient.GetElementStateReportsForMrID(mrID);
        }

        public List<List<IncidentReport>> GetReportsForMrID(string mrID)
        {
            return IMSClient.GetReportsForMrID(mrID);
        }

        public List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date)
        {
            return IMSClient.GetReportsForSpecificDateSortByBreaker(mrids, date);
        }

        public List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids)
        {
            return IMSClient.GetAllReportsSortByBreaker(mrids);
        }

        public List<IncidentReport> GetAllIncidentReports()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
