using DMS.Hosts;
using DMSCommon;
using DMSCommon.Model;
using DMSContract;
using FTN.Common;
using IMSContract;
using OMSSCADACommon;
using PubSubscribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace DMSService
{
    public class DMSSCADAService : IDMSSCADAContract
    {
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

        public void ChangeOnSCADADigital(string mrID, OMSSCADACommon.States state)
        {
            ModelGdaDMS gda = new ModelGdaDMS();

            List<ResourceDescription> discreteMeasurements = gda.GetExtentValuesExtended(ModelCode.DISCRETE);
            ResourceDescription rdDMeasurement = discreteMeasurements.Where(r => r.GetProperty(ModelCode.IDOBJ_MRID).AsString() == mrID).FirstOrDefault();

            // if measurement exists here! if result is null it exists only on scada, but not in .data
            if (rdDMeasurement != null)
            {
                // find PSR element associated with measurement
                long rdAssociatedPSR = rdDMeasurement.GetProperty(ModelCode.MEASUREMENT_PSR).AsLong();

                List<UIUpdateModel> networkChange = new List<UIUpdateModel>();

                Element DMSElementWithMeas;
                Console.WriteLine("Change on scada Digital Instance.Tree");
                DMSServiceHost.Instance.Tree.Data.TryGetValue(rdAssociatedPSR, out DMSElementWithMeas);
                Switch sw = DMSElementWithMeas as Switch;

                if (sw != null)
                {
                    bool isIncident = false;
                    IncidentReport incident = new IncidentReport() { MrID = sw.MRID };
                    incident.Crewtype = CrewType.Investigation;

                    SwitchStateReport elementStateReport = new SwitchStateReport() { MrID = sw.MRID, Time = DateTime.UtcNow, State = (int)state };

                    if (state == OMSSCADACommon.States.OPEN)
                    {
                        isIncident = true;

                        sw.Incident = true;
                        sw.State = SwitchState.Open;
                        sw.IsEnergized = false;
                        networkChange.Add(new UIUpdateModel(sw.ElementGID, false, OMSSCADACommon.States.OPEN));

                        // treba mi objasnjenje sta se ovde radi? ne kotnam ove ScadaupdateModele sta se kad gde dodaje, sta je sta
                        // uopste, summary iznad tih propertija u dms modelu
                        Node n = (Node)DMSServiceHost.Instance.Tree.Data[sw.End2];
                        n.IsEnergized = false;
                        networkChange.Add(new UIUpdateModel(n.ElementGID, false));
                        // pojasnjenje mi treba, komentari u ovom algoritmu i slicno, da ne debagujem sve redom, nemam vremena sad za to xD 
                        networkChange = EnergizationAlgorithm.TraceDown(n, networkChange, false, false, DMSServiceHost.Instance.Tree);
                    }
                    else if (state == OMSSCADACommon.States.CLOSED)
                    {
                        sw.Incident = false;
                        sw.CanCommand = false;
                        sw.State = SwitchState.Closed;

                        // i ovde takodje pojasnjenje
                        if (EnergizationAlgorithm.TraceUp((Node)DMSServiceHost.Instance.Tree.Data[sw.End1], DMSServiceHost.Instance.Tree))
                        {
                            networkChange.Add(new UIUpdateModel(sw.ElementGID, true, OMSSCADACommon.States.CLOSED));
                            sw.IsEnergized = true;

                            Node n = (Node)DMSServiceHost.Instance.Tree.Data[sw.End2];
                            n.IsEnergized = true;
                            networkChange.Add(new UIUpdateModel(n.ElementGID, true));
                            networkChange = EnergizationAlgorithm.TraceDown(n, networkChange, true, false, DMSServiceHost.Instance.Tree);
                        }
                        else
                        {
                            networkChange.Add(new UIUpdateModel(sw.ElementGID, false, OMSSCADACommon.States.CLOSED));
                        }
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
                            Console.WriteLine("ProcessCrew() -> IMS is not available yet.");
                            if (IMSProxy.State == CommunicationState.Faulted)
                            {
                                IMSProxy = new IMSProxy();
                            }
                        }

                        Thread.Sleep(1000);
                    } while (true);

                    // report changed state of the element
                    IMSProxy.AddElementStateReport(elementStateReport);

                    // ni ovo ne kontam, tj. nemam vremena da kontam previse xD
                    Source s = (Source)DMSServiceHost.Instance.Tree.Data[DMSServiceHost.Instance.Tree.Roots[0]];
                    networkChange.Add(new UIUpdateModel(s.ElementGID, true));

                    Publisher publisher = new Publisher();
                    if (networkChange.Count > 0)
                    {
                        publisher.PublishUpdateDigital(mrID, state);
                    }
                    if (isIncident)
                    {
                        List<long> gids = new List<long>();
                        networkChange.ForEach(x => gids.Add(x.Gid));
                        List<long> listOfConsumersWithoutPower = gids.Where(x => (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(x) == DMSType.ENERGCONSUMER).ToList();
                        foreach (long gid in listOfConsumersWithoutPower)
                        {
                            ResourceDescription resDes = DMSServiceHost.Instance.Gda.GetValues(gid);
                            try { incident.LostPower += resDes.GetProperty(ModelCode.ENERGCONSUMER_PFIXED).AsFloat(); } catch { }
                        }
                        IMSProxy.AddReport(incident);
                        publisher.PublishIncident(incident);
                    }
                }
            }
            else
            {
                Console.WriteLine("ChangeOnScada()-> element with mrid={0} do not exist in OMS.", mrID);
            }
        }

        public void ChangeOnSCADAAnalog(string mrID, float value)
        {
            ModelGdaDMS gda = new ModelGdaDMS();

            List<ResourceDescription> analogMeasurements = gda.GetExtentValuesExtended(ModelCode.ANALOG);
            ResourceDescription rdDMeasurement = analogMeasurements.Where(r => r.GetProperty(ModelCode.IDOBJ_MRID).AsString() == mrID).FirstOrDefault();

            // if measurement exists here! if result is null it exists only on scada, but not in .data
            if (rdDMeasurement != null)
            {
                long measGid = rdDMeasurement.GetProperty(ModelCode.IDOBJ_GID).AsLong();


                // to do: cuvanje u bazi promene za analogne, bla bla. Inicijalno uopste nije bilo planirano da se propagiraju promene za analogne,
                // receno je da te vrednosti samo zakucamo :D, zato tu implementaciju ostavljam za svetlu buducnost! 

                // ovde sad mogu neke kalkulacije opasne da se racunaju, kao ako je ta neka vrednost to se npr. ne uklapa sa 
                // izracunatom vrednoscu za taj customer..ma bla bla...to nama ne treba xD
                
                List<UIUpdateModel> networkChange = new List<UIUpdateModel>();
                networkChange.Add(new UIUpdateModel() { Gid = measGid, AnValue = value });

                Publisher publisher = new Publisher();
                if (networkChange.Count > 0)
                {
                    publisher.PublishUpdateAnalog(mrID, value);
                }
            }
            else
            {
                Console.WriteLine("ChangeOnScada()-> element with mrid={0} do not exist in OMS.", mrID);
            }
        }
    }
}
