﻿using DMSCommon;
using DMSCommon.Model;
using DMSCommon.TreeGraph;
using DMSCommon.TreeGraph.Tree;
using DMSContract;
using DMSService;
using FTN.Common;
using IMSContract;
using OMSCommon;
using OMSSCADACommon.Commands;
using OMSSCADACommon.Responses;
using SCADAContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using TransactionManagerContract;

namespace DMS.Hosts
{
    public class DMSServiceHost : IDisposable
    {
        private List<ServiceHost> hosts = null;

        private List<Source> _sources = new List<Source>();
        private List<Consumer> _consumers = new List<Consumer>();
        private List<ACLine> _aclines = new List<ACLine>();
        private List<Switch> _switches = new List<Switch>();
        private List<Node> _connecnodes = new List<Node>();
        private List<ResourceDescription> terminals = new List<ResourceDescription>();
        private List<ResourceDescription> switches = new List<ResourceDescription>();
        private List<ResourceDescription> nodes = new List<ResourceDescription>();
        private List<ResourceDescription> aclineSeg = new List<ResourceDescription>();
        private List<ResourceDescription> energyConsumers = new List<ResourceDescription>();
        private List<ResourceDescription> energySources = new List<ResourceDescription>();
        private List<ResourceDescription> discreteMeasurements = new List<ResourceDescription>();
        private List<ResourceDescription> analogMeasurements = new List<ResourceDescription>();

        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
        private ModelGdaDMS gda = new ModelGdaDMS();

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

        private ServiceHost scadaHost;

        private static Tree<Element> tree;
        private static DMSServiceHost instance = null;

        public static bool areHostsStarted = false;
        public static bool isNetworkInitialized = false;

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

        public static DMSServiceHost Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DMSServiceHost();
                }
                return instance;
            }
        }

        public static int updatesCount = 0;

        private DMSServiceHost()
        {
            InitializeHosts();
        }

        #region Properties
        public List<Source> Sources { get => _sources; set => _sources = value; }
        public List<Consumer> Consumers { get => _consumers; set => _consumers = value; }
        public List<ACLine> Aclines { get => _aclines; set => _aclines = value; }
        public List<Switch> Switches { get => _switches; set => _switches = value; }
        public List<Node> ConnecNodes { get => _connecnodes; set => _connecnodes = value; }

        public Tree<Element> Tree
        {
            get
            {
                Console.WriteLine("GetNetwork getter");
                return tree;
            }
            set
            {
                if (value != null)
                {
                    tree = value;
                }
            }
        }

        public List<ResourceDescription> TerminalsRD { get => terminals; set => terminals = value; }
        public List<ResourceDescription> SwitchesRD { get => switches; set => switches = value; }
        public List<ResourceDescription> NodesRD { get => nodes; set => nodes = value; }
        public List<ResourceDescription> AclineSegRD { get => aclineSeg; set => aclineSeg = value; }
        public List<ResourceDescription> EnergyConsumersRD { get => energyConsumers; set => energyConsumers = value; }
        public List<ResourceDescription> EnergySourcesRD { get => energySources; set => energySources = value; }
        public List<ResourceDescription> DiscreteMeasurementsRD { get => discreteMeasurements; set => discreteMeasurements = value; }
        public List<ResourceDescription> AnalogMeasurementsRD { get => analogMeasurements; set => analogMeasurements = value; }

        public ModelGdaDMS Gda { get => gda; set => gda = value; }
        #endregion

        public void Start()
        {
            string message = string.Empty;
            
            StartHosts();
            Tree = InitializeNetwork(new Delta());

            while (!isNetworkInitialized)
            {
                Console.WriteLine("Not Initialized network");
                Thread.Sleep(100);
            }
            
            StartScadaHost();
            
            if (hosts.Select(h => h.State == CommunicationState.Opened).ToList().Count == hosts.Count)
            {
                areHostsStarted = true;
                message = "The Distribution Management System has started.";
                Console.WriteLine("\n{0}", message);
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
            }
        }

        /// <summary>
        /// Getting Network Static Data from NMS (.data), and Dynamic data from Scada, in order to create Network DMS Tree.
        /// Called initialy on system Init, and later in transaction, if .data changes
        /// </summary>
        /// <returns></returns>
        public Tree<Element> InitializeNetwork(Delta delta)
        {
            Console.WriteLine("DMSService()-> InitializeNetwork Called");
            Tree<Element> retVal = new Tree<Element>();
            List<long> eSources = new List<long>();

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
                    Console.WriteLine("InitializeNetwork() -> SCADA is not available yet.");
                    if (ScadaProxy.State == CommunicationState.Faulted)
                        ScadaProxy = new SCADAProxy();
                }
                Thread.Sleep(300);
            } while (true);

            Console.WriteLine("InitializeNetwork() -> SCADA is available.");

            Response response = null;
            // get dynamic data
            response = ScadaProxy.ExecuteCommand(new ReadAll());

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
                    Console.WriteLine("InitializeNetwork() -> IMS is not available yet.");
                    if (IMSProxy.State == CommunicationState.Faulted)
                    {
                        IMSProxy = new IMSProxy();
                    }
                }

                Thread.Sleep(1000);
            } while (true);

            Console.WriteLine("InitializeNetwork() -> IMS is available.");

            List<IncidentReport> reports = imsProxy.GetAllReports();
            List<SwitchStateReport> elementStates = imsProxy.GetAllElementStateReports();

            // if there is no insert operations it means it is system initialization,
            // and DMS should obtain the static data from NMS           
            if (delta.InsertOperations.Count == 0)
            {
                ClearAllLists();
                //Ovo je kod koji je komunikacija DMS-a sa NMS-om   
                Gda.GetExtentValuesExtended(ModelCode.TERMINAL).ForEach(ter => TerminalsRD.Add(ter));
                Gda.GetExtentValuesExtended(ModelCode.CONNECTNODE).ForEach(n => NodesRD.Add(n));
                Gda.GetExtentValuesExtended(ModelCode.BREAKER).ForEach(n => SwitchesRD.Add(n));
                Gda.GetExtentValuesExtended(ModelCode.ACLINESEGMENT).ForEach(n => AclineSegRD.Add(n));
                Gda.GetExtentValuesExtended(ModelCode.ENERGCONSUMER).ForEach(n => EnergyConsumersRD.Add(n));
                Gda.GetExtentValuesExtended(ModelCode.ENERGSOURCE).ForEach(n => EnergySourcesRD.Add(n));
                Gda.GetExtentValuesExtended(ModelCode.DISCRETE).ForEach(n => DiscreteMeasurementsRD.Add(n));
                Gda.GetExtentValuesExtended(ModelCode.ANALOG).ForEach(n => AnalogMeasurementsRD.Add(n));

                //Posto smo uveli bazu RD-na napunicemo liste na DMS-u iz cloud baze
                //using (NMSAdoNet ctx = new NMSAdoNet())
                //{
                //    List<PropertyValue> propValues = (List<PropertyValue>)ctx.PropertyValue.ToList();
                //    List<Property> properties = ctx.Property.ToList();
                //    properties.ForEach(x => x.PropertyValue = ctx.PropertyValue.Where(y => y.Id == x.IdDB).FirstOrDefault());
                //    if (properties.Count > 0)
                //    {
                //        foreach (ResourceDescription rd in ctx.ResourceDescription)
                //        {
                //            List<Property> rdProp = (List<Property>)properties.Where(x => x.ResourceDescription_Id == rd.IdDb).ToList();
                //            ResourceDescription res = new ResourceDescription(rd.Id, rdProp);
                //            DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(res.Id);
                //            switch (type)
                //            {
                //                case DMSType.CONNECTNODE:
                //                    NodesRD.Add(res);
                //                    break;
                //                case DMSType.ENERGSOURCE:
                //                    EnergySourcesRD.Add(res);
                //                    break;
                //                case DMSType.ACLINESEGMENT:
                //                    AclineSegRD.Add(res);
                //                    break;
                //                case DMSType.BREAKER:
                //                    SwitchesRD.Add(res);
                //                    break;
                //                case DMSType.ENERGCONSUMER:
                //                    EnergyConsumersRD.Add(res);
                //                    break;
                //                case DMSType.TERMINAL:
                //                    TerminalsRD.Add(res);
                //                    break;
                //                case DMSType.DISCRETE:
                //                    DiscreteMeasurementsRD.Add(res);
                //                    break;
                //                case DMSType.ANALOG:
                //                    break;
                //                default:
                //                    break;
                //            }
                //        }
                //    }
                //}
            }
            // it means this is an update from TransactionManager
            else
            {
                foreach (ResourceDescription resource in delta.InsertOperations)
                {
                    DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(resource.Id);
                    if (type == DMSType.ACLINESEGMENT)
                        AclineSegRD.Add(resource);
                    else if (type == DMSType.CONNECTNODE)
                        NodesRD.Add(resource);
                    else if (type == DMSType.BREAKER)
                        SwitchesRD.Add(resource);
                    else if (type == DMSType.ENERGCONSUMER)
                        EnergyConsumersRD.Add(resource);
                    else if (type == DMSType.TERMINAL)
                        TerminalsRD.Add(resource);
                    else if (type == DMSType.ENERGSOURCE)
                        EnergySourcesRD.Add(resource);
                    else if (type == DMSType.DISCRETE)
                        DiscreteMeasurementsRD.Add(resource);
                }
                foreach (ResourceDescription resource in delta.UpdateOperations)
                {
                    DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(resource.Id);
                    if (type == DMSType.ACLINESEGMENT)
                    {
                        var ac = AclineSegRD.FirstOrDefault(a => a.GetProperty(ModelCode.IDOBJ_MRID) == resource.GetProperty(ModelCode.IDOBJ_MRID));
                        if (ac != null)
                            ac.Update(resource);
                    }
                    else if (type == DMSType.CONNECTNODE)
                    {
                        var cn = NodesRD.FirstOrDefault(c => c.GetProperty(ModelCode.IDOBJ_MRID) == resource.GetProperty(ModelCode.IDOBJ_MRID));
                        if (cn != null)
                            cn.Update(resource);
                    }
                    else if (type == DMSType.BREAKER)
                    {
                        var sw = SwitchesRD.FirstOrDefault(c => c.GetProperty(ModelCode.IDOBJ_MRID) == resource.GetProperty(ModelCode.IDOBJ_MRID));
                        if (sw != null)
                            sw.Update(resource);
                    }
                    else if (type == DMSType.ENERGCONSUMER)
                    {
                        var ec = EnergyConsumersRD.FirstOrDefault(c => c.GetProperty(ModelCode.IDOBJ_MRID) == resource.GetProperty(ModelCode.IDOBJ_MRID));
                        if (ec != null)
                            ec.Update(resource);
                    }
                    else if (type == DMSType.TERMINAL)
                    {
                        var ter = TerminalsRD.FirstOrDefault(c => c.GetProperty(ModelCode.IDOBJ_MRID) == resource.GetProperty(ModelCode.IDOBJ_MRID));
                        if (ter != null)
                            ter.Update(resource);
                    }
                    else if (type == DMSType.ENERGSOURCE)
                    {
                        var es = EnergySourcesRD.FirstOrDefault(c => c.GetProperty(ModelCode.IDOBJ_MRID) == resource.GetProperty(ModelCode.IDOBJ_MRID));
                        if (es != null)
                            es.Update(resource);
                    }
                }
            }

            EnergySourcesRD.ForEach(x => eSources.Add(x.Id));
            if (eSources.Count == 0)
            {
                Console.WriteLine("InitializeNetwork Done");
                isNetworkInitialized = true;
                return new Tree<Element>();
            }

            ClearListsForNTreeAlgorith();
            List<long> terminals = new List<long>();
            List<NodeLink> links = new List<NodeLink>();
            string mrid = "";

            TerminalsRD.ForEach(x => terminals.Add(x.Id));
            Console.WriteLine("terminalsRD.Count= {0}, terminals.count={1}", TerminalsRD.Count, terminals.Count);

            // Pocetak algoritma za formiranje stabla
            // obtaining all ES and connecting them with CNs. there is only one ES currently
            foreach (long source in eSources)
            {
                mrid = GetMrid(DMSType.ENERGSOURCE, source);
                Source ESource = new Source(source, 0, mrid);

                // ES and CN linked by terminal 
                long term = GetTerminalConnectedWithBranch(source);
                if (term != 0)
                {
                    long connNode = GetConnNodeConnectedWithTerminal(term);
                    if (connNode != 0)
                    {
                        Console.WriteLine("connNode!=0");
                        mrid = GetMrid(DMSType.CONNECTNODE, connNode);
                        Node n = new Node(connNode, mrid, ESource, term);
                        ESource.End2 = n.ElementGID;
                        ConnecNodes.Add(n);
                        Sources.Add(ESource);
                        //dodavanje ES u koren stabla i prvog childa
                        retVal.AddRoot(Sources[0].ElementGID, Sources[0]);
                        retVal.AddChild(n.Parent, n.ElementGID, n);
                    }
                }
                terminals.Remove(term);
            }

            // Obrada od pocetnog CN ka svim ostalima. Iteracija po terminalima
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int count = 0;

            while (terminals.Count != 0)
            {
                Console.WriteLine("terminals.Count!=0");
                Node n;
                try
                {
                    n = ConnecNodes.ElementAt(count);
                }
                catch (Exception e)
                {
                    Console.WriteLine("problem1");
                    Console.WriteLine(e.Message);
                    return new Tree<Element>();
                }

                List<long> terms = GetTerminalsConnectedWithConnNode(n.ElementGID);

                if (terms.Contains(n.UpTerminal))
                {
                    Console.WriteLine("contains");
                    terms.Remove(n.UpTerminal);
                }
                foreach (long item in terms)
                {
                    long branch = GetBranchConnectedWithTerminal(item);

                    DMSType mc;
                    if (branch != 0)
                    {
                        Console.WriteLine("branch!=0");
                        mc = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(branch);
                        mrid = GetMrid(mc, branch);
                    }
                    else
                        continue;

                    List<long> branchTerminals = GetTerminalsConnectedWithBranch(branch);
                    if (branchTerminals.Contains(item))
                    {
                        Console.WriteLine("branchTerminals.Contains");
                        branchTerminals.Remove(item);
                    }

                    if (mc.Equals(DMSType.ACLINESEGMENT))
                    {
                        Console.WriteLine("mc equas acline");
                        ACLine acline = new ACLine(branch, mrid);
                        acline.End1 = n.ElementGID;
                        n.Children.Add(acline.ElementGID);
                        long downNodegid = GetConnNodeConnectedWithTerminal(branchTerminals[0]);
                        mrid = GetMrid(DMSType.CONNECTNODE, downNodegid);
                        Node downNode = new Node(downNodegid, mrid, acline, branchTerminals[0]);
                        acline.End2 = downNode.ElementGID;
                        Aclines.Add(acline);
                        ConnecNodes.Add(downNode);
                        terminals.Remove(branchTerminals[0]);

                        retVal.AddChild(n.ElementGID, acline.ElementGID, acline);
                        retVal.AddChild(acline.ElementGID, downNode.ElementGID, downNode);
                    }
                    else if (mc.Equals(DMSType.BREAKER))
                    {
                        Switch sw = new Switch(branch, mrid);

                        if (response != null)
                        {
                            var psr = SwitchesRD.Where(m => m.GetProperty(ModelCode.IDOBJ_MRID).AsString() == mrid).FirstOrDefault();
                            var meas = DiscreteMeasurementsRD.Where(m => m.GetProperty(ModelCode.MEASUREMENT_PSR).AsLong() == psr.Id).FirstOrDefault();

                            if (meas != null)
                            {
                                var res = (DigitalVariable)response.Variables.Where(v => v.Id == meas.GetProperty(ModelCode.IDOBJ_MRID).AsString()).FirstOrDefault();

                                if (res != null)
                                {
                                    if (res.State == OMSSCADACommon.States.OPEN)
                                    {
                                        sw = new Switch(branch, mrid, SwitchState.Open) { UnderSCADA = true };
                                    }
                                    else
                                    {
                                        sw = new Switch(branch, mrid, SwitchState.Closed) { UnderSCADA = true };
                                    }
                                }

                                sw.UnderSCADA = true;
                            }
                            else
                            {
                                SwitchStateReport elementStateReport = elementStates.Where(s => s.MrID == sw.MRID).LastOrDefault();

                                if (elementStateReport != null)
                                {
                                    sw = new Switch(branch, mrid, (SwitchState)elementStateReport.State) { UnderSCADA = false };
                                }
                                else
                                {
                                    sw = new Switch(branch, mrid, SwitchState.Closed) { UnderSCADA = false };
                                }
                            }
                        }
                        else
                        {
                            sw.UnderSCADA = false;

                            SwitchStateReport elementStateReport = elementStates.Where(s => s.MrID == sw.MRID).LastOrDefault();

                            if (elementStateReport != null)
                            {
                                sw = new Switch(branch, mrid, (SwitchState)elementStateReport.State) { UnderSCADA = false };
                            }
                            else
                            {
                                sw = new Switch(branch, mrid, SwitchState.Closed) { UnderSCADA = false };
                            }
                        }

                        foreach (IncidentReport report in reports)
                        {
                            if (report.MrID == sw.MRID && report.IncidentState != IncidentState.REPAIRED)
                            {
                                sw.Incident = true;

                                if (sw.UnderSCADA)
                                {
                                    sw.CanCommand = false;
                                }

                                break;
                            }
                            else if (report.MrID == sw.MRID && report.IncidentState == IncidentState.REPAIRED)
                            {
                                if (sw.State == SwitchState.Open)
                                {
                                    if (sw.UnderSCADA)
                                    {
                                        sw.CanCommand = true;
                                    }
                                }
                            }
                        }

                        sw.End1 = n.ElementGID;
                        n.Children.Add(sw.ElementGID);
                        long downNodegid = GetConnNodeConnectedWithTerminal(branchTerminals[0]);
                        mrid = GetMrid(DMSType.CONNECTNODE, downNodegid);
                        Node downNode = new Node(downNodegid, mrid, sw, branchTerminals[0]);
                        sw.End2 = downNode.ElementGID;
                        Switches.Add(sw);
                        ConnecNodes.Add(downNode);
                        terminals.Remove(branchTerminals[0]);

                        retVal.AddChild(n.ElementGID, sw.ElementGID, sw);
                        retVal.AddChild(sw.ElementGID, downNode.ElementGID, downNode);
                    }
                    else if (mc.Equals(DMSType.ENERGCONSUMER))
                    {
                        Consumer consumer = new Consumer(branch, mrid);
                        consumer.End1 = n.ElementGID;
                        n.Children.Add(consumer.ElementGID);
                        Consumers.Add(consumer);

                        if (response != null)
                        {
                            var psr = EnergyConsumersRD.Where(m => m.GetProperty(ModelCode.IDOBJ_MRID).AsString() == mrid).FirstOrDefault();
                            var meas = AnalogMeasurementsRD.Where(m => m.GetProperty(ModelCode.MEASUREMENT_PSR).AsLong() == psr.Id).FirstOrDefault();

                            if (meas != null)
                            {
                                consumer.UnderSCADA = true;
                            }
                            else
                            {
                                consumer.UnderSCADA = false;
                            }
                        }

                        retVal.AddChild(n.ElementGID, consumer.ElementGID, consumer);
                    }
                    else break;

                    terminals.Remove(item);
                }
                count++;
            }

            watch.Stop();
            updatesCount += 1;

            if (retVal.Roots.Count > 0)
            {
                var gid = retVal.Roots[0];
                Source source = (Source)retVal.Data.Where(e => e.Value.ElementGID == gid).FirstOrDefault().Value;

                if (source != null)
                {
                    Node node = (Node)retVal.Data.Where(e => e.Value.ElementGID == source.End2).FirstOrDefault().Value;

                    if (node != null)
                    {
                        EnergizationAlgorithm.TraceDown(node, new List<UIUpdateModel>(), true, true, retVal);
                    }
                }
            }

            Console.WriteLine("\nNewtork Initialization finished in {0} sec", watch.ElapsedMilliseconds / 1000);
            isNetworkInitialized = true;
            return retVal;
        }

        private void ClearAllLists()
        {
            this.Aclines.Clear();
            this.ConnecNodes.Clear();
            this.Consumers.Clear();
            this.Switches.Clear();
            this.Sources.Clear();
            this.TerminalsRD.Clear();
            this.SwitchesRD.Clear();
            this.EnergyConsumersRD.Clear();
            this.NodesRD.Clear();
            this.AclineSegRD.Clear();
            this.EnergySourcesRD.Clear();
        }
        private void ClearListsForNTreeAlgorith()
        {
            this.Aclines.Clear();
            this.ConnecNodes.Clear();
            this.Consumers.Clear();
            this.Switches.Clear();
            this.Sources.Clear();
        }

        #region GetRelatedMethods

        private string GetMrid(DMSType mc, long branch)
        {
            string mrid = "";
            if (mc == DMSType.ACLINESEGMENT)
            {
                foreach (ResourceDescription resD in AclineSegRD)
                {
                    if (resD.Id == branch)
                    {
                        mrid = resD.GetProperty(ModelCode.IDOBJ_MRID).PropertyValue.StringValue;
                        return mrid;
                    }
                }

            }
            else if (mc == DMSType.BREAKER)
            {
                foreach (ResourceDescription resD in SwitchesRD)
                {
                    if (resD.Id == branch)
                    {
                        mrid = resD.GetProperty(ModelCode.IDOBJ_MRID).PropertyValue.StringValue;
                        return mrid;
                    }
                }
            }
            else if (mc == DMSType.ENERGCONSUMER)
            {
                foreach (ResourceDescription resD in EnergyConsumersRD)
                {
                    if (resD.Id == branch)
                    {
                        mrid = resD.GetProperty(ModelCode.IDOBJ_MRID).PropertyValue.StringValue;
                        return mrid;
                    }
                }
            }
            else if (mc == DMSType.CONNECTNODE)
            {
                foreach (ResourceDescription resD in NodesRD)
                {
                    if (resD.Id == branch)
                    {
                        mrid = resD.GetProperty(ModelCode.IDOBJ_MRID).PropertyValue.StringValue;
                        return mrid;
                    }
                }
            }

            else if (mc == DMSType.ENERGSOURCE)
            {
                foreach (ResourceDescription resD in EnergySourcesRD)
                {
                    if (resD.Id == branch)
                    {
                        mrid = resD.GetProperty(ModelCode.IDOBJ_MRID).PropertyValue.StringValue;
                        return mrid;
                    }
                }
            }
            return mrid;
        }
        private long GetConnNodeConnectedWithTerminal(long terminal)
        {
            long connNode = 0;
            foreach (ResourceDescription resD in TerminalsRD)
            {
                if (resD.Id == terminal)
                {
                    connNode = resD.GetProperty(ModelCode.TERMINAL_CONNECTNODE).PropertyValue.LongValue;
                    return connNode;
                }
            }
            return 0;

        }

        private long GetTerminalConnectedWithBranch(long branch)
        {
            long term = 0;
            foreach (ResourceDescription resD in TerminalsRD)
            {
                if (resD.GetProperty(ModelCode.TERMINAL_CONDEQUIP).PropertyValue.LongValue == branch)
                {
                    term = resD.Id;
                    break;
                }
            }
            return term;
        }

        private List<long> GetTerminalsConnectedWithConnNode(long connNode)
        {
            List<long> ret = new List<long>();
            TerminalsRD.ForEach(x => x.Properties
                .FindAll(y => y.PropertyValue.LongValue == connNode)
                .ForEach(g => ret.Add(x.Id)));

            return ret;
        }

        private long GetBranchConnectedWithTerminal(long terminal)
        {
            long branch = 0;
            foreach (ResourceDescription resD in TerminalsRD)
            {
                if (resD.Id == terminal)
                {
                    branch = resD.GetProperty(ModelCode.TERMINAL_CONDEQUIP).PropertyValue.LongValue;
                    return branch;
                }
            }
            return 0;
        }

        private List<long> GetTerminalsConnectedWithBranch(long branch)
        {
            List<long> terms = new List<long>();
            foreach (ResourceDescription resD in TerminalsRD)
            {
                if (resD.GetProperty(ModelCode.TERMINAL_CONDEQUIP).PropertyValue.LongValue == branch)
                {
                    terms.Add(resD.Id);
                }
            }
            return terms;

        }

        #endregion

        public void Dispose()
        {
            CloseHosts();
            GC.SuppressFinalize(this);
        }

        private void InitializeHosts()
        {
            hosts = new List<ServiceHost>();

            ServiceHost dmsHost = new ServiceHost(typeof(DMSService));
            dmsHost.Description.Name = "DMSService";
            dmsHost.AddServiceEndpoint(typeof(IDMSContract), NetTcpBindingCreator.Create(), new
            Uri("net.tcp://localhost:5000/DMSService"));
            dmsHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            dmsHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            hosts.Add(dmsHost);

            ServiceHost transactionHost = new ServiceHost(typeof(DMSTransactionService));
            transactionHost.Description.Name = "DMSTransactionService";
            transactionHost.AddServiceEndpoint(typeof(IDistributedTransaction), NetTcpBindingCreator.Create(), new
            Uri("net.tcp://localhost:5001/DMSTransactionService"));
            transactionHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            transactionHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            hosts.Add(transactionHost);

            scadaHost = new ServiceHost(typeof(DMSSCADAService));
            scadaHost.Description.Name = "DMSSCADAService";
            scadaHost.AddServiceEndpoint(typeof(IDMSSCADAContract), NetTcpBindingCreator.Create(), new
            Uri("net.tcp://localhost:5002/DMSSCADAService"));
            scadaHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            scadaHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            ServiceHost callService = new ServiceHost(typeof(DMSCallService));
            callService.Description.Name = "DMSCallService";
            callService.AddServiceEndpoint(typeof(IDMSCallContract), NetTcpBindingCreator.Create(), new
            Uri("net.tcp://localhost:5003/DMSCallService"));
            callService.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            callService.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            hosts.Add(callService);
        }

        private void StartHosts()
        {
            if (hosts == null || hosts.Count == 0)
            {
                throw new Exception("DMS Services can not be opend because it is not initialized.");
            }

            string message = string.Empty;

            try
            {
                foreach (ServiceHost host in hosts)
                {
                    host.Open();

                    message = string.Format("The WCF service {0} is ready.", host.Description.Name);
                    Console.WriteLine(message);
                    CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

                    message = "Endpoints:";
                    Console.WriteLine(message);
                    CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

                    foreach (Uri uri in host.BaseAddresses)
                    {
                        Console.WriteLine(uri);
                        CommonTrace.WriteTrace(CommonTrace.TraceInfo, uri.ToString());
                    }

                    Console.WriteLine("\n");
                }

                message = string.Format("Trace level: {0}", CommonTrace.TraceLevel);
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void StartScadaHost()
        {
            string message = string.Empty;

            // hosts.Add(scadaHost);

            scadaHost.Open();

            message = string.Format("The WCF service {0} is ready.", scadaHost.Description.Name);
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

            message = "Endpoints:";
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

            foreach (Uri uri in scadaHost.BaseAddresses)
            {
                Console.WriteLine(uri);
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, uri.ToString());
            }
        }

        public void CloseHosts()
        {
            if (hosts == null || hosts.Count == 0)
            {
                throw new Exception("DMS Services can not be closed because it is not initialized.");
            }

            foreach (ServiceHost host in hosts)
            {
                host.Close();
            }

            string message = "The DMS Service is closed.";
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
            Console.WriteLine("\n\n{0}", message);
        }
    }
}