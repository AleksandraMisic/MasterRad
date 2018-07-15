using IMSContract;
using IncidentManagementSystem.Model;
using OMSCommon;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IncidentManagementSystem.Service
{
    public class IMSServiceHost
    {
        private ServiceHost svc = null;

        public void Start()
        {
            //Database.SetInitializer<IncidentContext>(new DropCreateDatabaseIfModelChanges<IncidentContext>());

            LoadCrews();

            svc = new ServiceHost(typeof(IMSService));
            svc.AddServiceEndpoint(typeof(IIMSContract),
                NetTcpBindingCreator.Create(),
                new Uri("net.tcp://localhost:6000/IMSService"));
          
            svc.Open();
            Console.WriteLine("IMSService ready and waiting for requests.");
        }

        public void Stop()
        {
            svc.Close();
            Console.WriteLine("IMSService has stopped.");
        }

        private void LoadCrews()
        {
            List<Crew> crews = new List<Crew>();
            Crew c1 = new Crew() { Id = "1", CrewName = "Adam Smith", Type = CrewType.Investigation };
            Crew c2 = new Crew() { Id = "2", CrewName = "Danny Phillips", Type = CrewType.Investigation };
            Crew c3 = new Crew() { Id = "3", CrewName = "Anna Davis", Type = CrewType.Investigation };
            Crew c4 = new Crew() { Id = "4", CrewName = "Mark Crow ", Type = CrewType.Repair };
            Crew c5 = new Crew() { Id = "5", CrewName = "Jullie Stephenson", Type = CrewType.Repair };
            Crew c6 = new Crew() { Id = "6", CrewName = "David Phill", Type = CrewType.Repair };
            crews.Add(c1);
            crews.Add(c2);
            crews.Add(c3);
            crews.Add(c4);
            crews.Add(c5);
            crews.Add(c6);

            using (var ctx = new IncidentContext())
            {
                foreach (Crew c in crews)
                {
                    try
                    {
                        if (!ctx.Crews.Any(e => e.Id == c.Id))
                        {
                            ctx.Crews.Add(c);
                            ctx.SaveChanges();
                        }
                    }
                    catch (Exception e) { }
                }
            }
        }
    }
}
