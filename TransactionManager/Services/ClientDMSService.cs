﻿using System;
using System.Collections.Generic;
using DMSCommon.Model;
using DMSContract;
using DMSContract.Proxies;
using IMSContract;
using TransactionManagerContract.ClientDMS;

namespace TransactionManager.Services
{
    public class ClientDMSService : IDMSContract
    {
        private DMSProxy dMSProxy;

        public ClientDMSService()
        {
            dMSProxy = new DMSProxy();
        }

        public List<ACLine> GetAllACLines()
        {
            throw new NotImplementedException();
        }

        public List<Consumer> GetAllConsumers()
        {
            throw new NotImplementedException();
        }

        public List<Element> GetAllElements()
        {
            throw new NotImplementedException();
        }

        public List<Node> GetAllNodes()
        {
            throw new NotImplementedException();
        }

        public List<Source> GetAllSources()
        {
            List<Source> listOfDMSSources = new List<Source>();
            try
            {
                listOfDMSSources = dMSProxy.GetAllSources();
            }
            catch (Exception e) { }

            return listOfDMSSources;
        }

        public List<Switch> GetAllSwitches()
        {
            throw new NotImplementedException();
        }

        public List<Element> GetNetwork(string mrid)
        {
            List<Element> listOfDMSElements = new List<Element>();
            try
            {
                listOfDMSElements = dMSProxy.GetNetwork(mrid);
            }
            catch (Exception e) { }

            return listOfDMSElements;
        }

        public int GetNetworkDepth()
        {
            throw new NotImplementedException();
        }

        public Source GetTreeRoot()
        {
            throw new NotImplementedException();
        }

        public void SendCrew(IncidentReport report, Crew crew)
        {
            throw new NotImplementedException();
        }
    }
}
