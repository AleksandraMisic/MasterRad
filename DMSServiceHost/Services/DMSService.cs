using DMS.Hosts;
using DMSCommon.Model;
using DMSContract;
using IMSContract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS
{
    public class DMSService : IDMSContract
    {
        public List<Element> GetAllElements()
        {
            List<Element> retVal = new List<Element>();
            try
            {
                foreach (Element e in DMSServiceHost.Instance.Tree.Data.Values)
                {
                    retVal.Add(e);
                }
                return retVal;
            }
            catch (Exception)
            {
                return new List<Element>();
            }
        }

        public List<Source> GetAllSources()
        {
            List<Source> pom = new List<Source>();
            try
            {
                foreach (var item in DMSServiceHost.Instance.Tree.Data.Values)
                {
                    if (item is Source)
                    {
                        pom.Add((Source)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {
                return new List<Source>();
            }
        }

        public List<Consumer> GetAllConsumers()
        {
            List<Consumer> pom = new List<Consumer>();
            try
            {
                foreach (var item in DMSServiceHost.Instance.Tree.Data.Values)
                {
                    if (item is Consumer)
                    {
                        pom.Add((Consumer)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {
                return new List<Consumer>();
            }
        }

        public List<Switch> GetAllSwitches()
        {
            List<Switch> pom = new List<Switch>();
            try
            {
                foreach (var item in DMSServiceHost.Instance.Tree.Data.Values)
                {
                    if (item is Switch)
                    {
                        pom.Add((Switch)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {
                return new List<Switch>();
            }
        }

        public List<ACLine> GetAllACLines()
        {
            List<ACLine> pom = new List<ACLine>();
            try
            {
                foreach (var item in DMSServiceHost.Instance.Tree.Data.Values)
                {
                    if (item is ACLine)
                    {
                        pom.Add((ACLine)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {
                return new List<ACLine>();
            }
        }

        public List<Node> GetAllNodes()
        {
            List<Node> pom = new List<Node>();
            try
            {
                foreach (var item in DMSServiceHost.Instance.Tree.Data.Values)
                {
                    if (item is Node)
                    {
                        pom.Add((Node)item);
                    }
                }
                return pom;
            }
            catch (Exception)
            {
                return new List<Node>();
            }
        }

        public int GetNetworkDepth()
        {
            try
            {
                return DMSServiceHost.Instance.Tree.Links.Max(x => x.Value.Depth) + 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public Source GetTreeRoot()
        {
            try
            {
                Source s = (Source)DMSServiceHost.Instance.Tree.Data[DMSServiceHost.Instance.Tree.Roots[0]];
                return s;
            }
            catch (Exception)
            {
                return new Source();
            }
        }

        public List<Element> GetNetwork(string mrid)
        {
            Element element;
            List<Element> list = new List<Element>();

            if((element = DMSServiceHost.Instance.Tree.Data.Values.Where(e => e.MRID == mrid).FirstOrDefault()) != null)
            {
                TraceDownNetwork(element as Branch, list);
            }

            return list;
        }

        private void TraceDownNetwork(Branch branch, List<Element> list)
        {
            list.Add(branch);

            if (branch.End2 != 0)
            {
                Element element;
                DMSServiceHost.Instance.Tree.Data.TryGetValue(branch.End2, out element);

                Node node = element as Node;
                list.Add(node);

                foreach (long id in node.Children)
                {
                    DMSServiceHost.Instance.Tree.Data.TryGetValue(id, out element);

                    TraceDownNetwork(element as Branch, list);
                }
            }
        }

        public void SendCrew(IncidentReport report, Crew crew)
        {
            throw new NotImplementedException();
        }
    }
}