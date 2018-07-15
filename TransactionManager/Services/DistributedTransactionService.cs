using DMSCommon.Model;
using DMSContract;
using FTN.Common;
using FTN.ServiceContracts;
using IMSContract;
using OMSSCADACommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using SCADAContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using TransactionManagerContract;

namespace TransactionManager
{
    public class DistributedTransactionService : IDistributedTransaction, IOMSClient
    {
        // properties for providing communication infrastructure for 2PC protocol
        List<IDistributedTransaction> transactionProxys;
        List<DistributedTransactionCallback> transactionCallbacks;
        IDistributedTransaction proxyTransactionNMS;
        IDistributedTransaction proxyTransactionDMS;
        ITransactionSCADA proxyTransactionSCADA;
        DistributedTransactionCallback callBackTransactionNMS;
        DistributedTransactionCallback callBackTransactionDMS;
        DistributedTransactionCallback callBackTransactionSCADA;
        IDMSContract proxyToDispatcherDMS;

        ModelGDATMS gdaTMS;

        private IMSProxy imsProxy;
        private IMSProxy IMSProxy
        {
            get
            {
                if (imsProxy == null)
                {
                    imsProxy = new IMSProxy();
                }
                return imsProxy;
            }
            set { imsProxy = value; }
        }

        private SCADAProxy scadaProxy;
        private SCADAProxy ScadaProxy
        {
            get
            {
                if (scadaProxy == null)
                {
                    scadaProxy = new SCADAProxy();
                }
                return scadaProxy;
            }
            set { scadaProxy = value; }
        }

        public List<IDistributedTransaction> TransactionProxys { get => transactionProxys; set => transactionProxys = value; }
        public List<DistributedTransactionCallback> TransactionCallbacks { get => transactionCallbacks; set => transactionCallbacks = value; }
        public IDistributedTransaction ProxyTransactionNMS { get => proxyTransactionNMS; set => proxyTransactionNMS = value; }
        public IDistributedTransaction ProxyTransactionDMS { get => proxyTransactionDMS; set => proxyTransactionDMS = value; }
        public ITransactionSCADA ProxyTransactionSCADA { get => proxyTransactionSCADA; set => proxyTransactionSCADA = value; }
        public DistributedTransactionCallback CallBackTransactionNMS { get => callBackTransactionNMS; set => callBackTransactionNMS = value; }
        public DistributedTransactionCallback CallBackTransactionDMS { get => callBackTransactionDMS; set => callBackTransactionDMS = value; }
        public DistributedTransactionCallback CallBackTransactionSCADA { get => callBackTransactionSCADA; set => callBackTransactionSCADA = value; }

        public DistributedTransactionService()
        {
            TransactionProxys = new List<IDistributedTransaction>();
            TransactionCallbacks = new List<DistributedTransactionCallback>();

            InitializeChanels();

            gdaTMS = new ModelGDATMS();
        }

        private void InitializeChanels()
        {
            Console.WriteLine("InitializeChannels()");

            var binding = new NetTcpBinding();
            binding.CloseTimeout = TimeSpan.FromMinutes(10);
            binding.OpenTimeout = TimeSpan.FromMinutes(10);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.SendTimeout = TimeSpan.FromMinutes(10);
            binding.TransactionFlow = true;
            binding.MaxReceivedMessageSize = Int32.MaxValue;

            // duplex channel for NMS transaction
            CallBackTransactionNMS = new DistributedTransactionCallback();
            TransactionCallbacks.Add(CallBackTransactionNMS);
            DuplexChannelFactory<IDistributedTransaction> factoryTransactionNMS = new DuplexChannelFactory<IDistributedTransaction>(CallBackTransactionNMS,
                                                         binding,
                                                         new EndpointAddress("net.tcp://localhost:8018/NetworkModelTransactionService"));
            ProxyTransactionNMS = factoryTransactionNMS.CreateChannel();
            TransactionProxys.Add(ProxyTransactionNMS);

            // duplex channel for DMS transaction
            CallBackTransactionDMS = new DistributedTransactionCallback();
            TransactionCallbacks.Add(CallBackTransactionDMS);
            DuplexChannelFactory<IDistributedTransaction> factoryTransactionDMS = new DuplexChannelFactory<IDistributedTransaction>(CallBackTransactionDMS,
                                                            binding,
                                                            new EndpointAddress("net.tcp://localhost:8028/DMSTransactionService"));
            ProxyTransactionDMS = factoryTransactionDMS.CreateChannel();
            TransactionProxys.Add(ProxyTransactionDMS);

            // duplex channel for SCADA transaction
            CallBackTransactionSCADA = new DistributedTransactionCallback();
            TransactionCallbacks.Add(CallBackTransactionSCADA);
            DuplexChannelFactory<ITransactionSCADA> factoryTransactionSCADA = new DuplexChannelFactory<ITransactionSCADA>(CallBackTransactionSCADA,
                                                            binding,
                                                            new EndpointAddress("net.tcp://localhost:8078/SCADATransactionService"));
            ProxyTransactionSCADA = factoryTransactionSCADA.CreateChannel();

            // client channel for DMSDispatcherService
            ChannelFactory<IDMSContract> factoryDispatcherDMS = new ChannelFactory<IDMSContract>(binding, new EndpointAddress("net.tcp://localhost:8029/DMSDispatcherService"));
            proxyToDispatcherDMS = factoryDispatcherDMS.CreateChannel();

            //ProxyToNMSService = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
            //ProxyToNMSService.Open();
        }

        #region 2PC methods

        public void Enlist(Delta d)
        {
            Console.WriteLine("Transaction Manager calling enlist");
            foreach (IDistributedTransaction svc in TransactionProxys)
            {
                svc.Enlist();
            }

            ProxyTransactionSCADA.Enlist();

            while (true)
            {
                if (TransactionCallbacks.Where(k => k.AnswerForEnlist == TransactionAnswer.Unanswered).Count() > 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                else
                {
                    Prepare(d);
                    break;
                }
            }
        }

        public void Prepare(Delta deltaForNMS)
        {
            Console.WriteLine("Transaction Manager calling prepare");

            ScadaDelta deltaForScada = GetDeltaForSCADA(deltaForNMS);
            //Delta fixedGuidDeltaForDMS = ProxyToNMSService.GetFixedDelta(deltaForNMS);
            Delta fixedGuidDeltaForDMS = gdaTMS.GetFixedDelta(deltaForNMS);

            ProxyTransactionNMS.Prepare(deltaForNMS);
            ProxyTransactionDMS.Prepare(fixedGuidDeltaForDMS);
            ProxyTransactionSCADA.Prepare(deltaForScada);

            while (true)
            {
                if (TransactionCallbacks.Where(k => k.AnswerForPrepare == TransactionAnswer.Unanswered).Count() > 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                else if (TransactionCallbacks.Where(u => u.AnswerForPrepare == TransactionAnswer.Unprepared).Count() > 0)
                {
                    Rollback();
                    break;
                }
                Commit();
                break;
            }
        }

        private void Commit()
        {
            Console.WriteLine("Transaction Manager calling commit");
            foreach (IDistributedTransaction svc in TransactionProxys)
            {
                svc.Commit();
            }
            ProxyTransactionSCADA.Commit();
        }

        public void Rollback()
        {
            Console.WriteLine("Transaction Manager calling rollback");
            foreach (IDistributedTransaction svc in TransactionProxys)
            {
                svc.Rollback();
            }
            ProxyTransactionSCADA.Rollback();
        }

        #endregion

        #region IOMSClient CIMAdapter Methods

        /// <summary>
        /// Called by ModelLabs(CIMAdapter) when Static data changes
        /// </summary>
        /// <param name="d">Delta</param>
        /// <returns></returns>
        public bool UpdateSystem(Delta d)
        {
            Console.WriteLine("Update System started." + d.Id);
            Enlist(d);
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

        public TMSAnswerToClient GetNetwork(string mrid)
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
                        if (ScadaProxy.State == CommunicationState.Created)
                        {
                            ScadaProxy.Open();
                        }

                        if (ScadaProxy.Ping())
                            break;
                    }
                    catch (Exception e)
                    {
                        if (ScadaProxy.State == CommunicationState.Faulted)
                            ScadaProxy = new SCADAProxy();
                    }
                    Thread.Sleep(500);
                } while (true);
                Console.WriteLine("GetNetwork() -> SCADA is available.");

                Response r = ScadaProxy.ExecuteCommand(c);
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
                    if (IMSProxy.State == CommunicationState.Created)
                    {
                        IMSProxy.Open();
                    }

                    if (IMSProxy.Ping())
                        break;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                    Console.WriteLine("GetNetwork() -> IMS is not available yet.");
                    if (IMSProxy.State == CommunicationState.Faulted)
                    {
                        IMSProxy = new IMSProxy();
                    }
                }
                Thread.Sleep(1000);
            } while (true);

            var crews = IMSProxy.GetCrews();
            var incidentReports = IMSProxy.GetAllReports();

            TMSAnswerToClient answer = new TMSAnswerToClient(resourceDescriptionFromNMS, listOfDMSElement, GraphDeep, descMeas, crews, incidentReports);
            return answer;
        }

        public void SendCommandToSCADA(TypeOfSCADACommand command, string mrid, CommandTypes commandtype, float value)
        {
            try
            {
                Command c = MappingEngineTransactionManager.Instance.MappCommand(command, mrid, commandtype, value);

                // to do: ping
                Response r = scadaProxy.ExecuteCommand(c);
                //Response r = SCADAClientInstance.ExecuteCommand(c);

            }
            catch (Exception e)
            { }
        }

        public void SendCrew(IncidentReport report, Crew crew)
        {
            proxyToDispatcherDMS.SendCrew(report, crew);
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
            return IMSProxy.GetElementStateReportsForMrID(mrID);
        }

        public List<List<IncidentReport>> GetReportsForMrID(string mrID)
        {
            return IMSProxy.GetReportsForMrID(mrID);
        }

        public List<List<IncidentReport>> GetReportsForSpecificDateSortByBreaker(List<string> mrids, DateTime date)
        {
            return IMSProxy.GetReportsForSpecificDateSortByBreaker(mrids, date);
        }

        public List<List<IncidentReport>> GetAllReportsSortByBreaker(List<string> mrids)
        {
            return IMSProxy.GetAllReportsSortByBreaker(mrids);
        }

        public void Enlist()
        {
            throw new NotImplementedException();
        }

        void IDistributedTransaction.Commit()
        {
            throw new NotImplementedException();
        }

        public List<Source> GetAllSources()
        {
            List<Source> listOfDMSSources = new List<Source>();
            try
            {
                listOfDMSSources = proxyToDispatcherDMS.GetAllSources();
            }
            catch (Exception e) { }

            return listOfDMSSources;
        }

        public List<IncidentReport> GetAllIncidentReports()
        {
            return IMSProxy.GetAllReports();
        }
        #endregion
    }
}