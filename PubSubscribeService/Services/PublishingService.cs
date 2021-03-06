﻿using IMSContract;
using PubSubContract;
using System;
using System.Collections.Generic;
using System.Threading;
using OMSSCADACommon;
using DMSCommon;

namespace PubSubscribeService.Services
{
    public class PublishingService : IPublishing
    {
        public PublishingService()
        {
        }

        public void PublishDigitalUpdate(string mrid, States state)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, mrid, state, 0, true);

                Thread thread = new Thread(threadObj.PublishDigitalDelta);
                thread.Start();
            }
        }

        public void PublishAnalogUpdate(string mrid, float value)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, mrid, 0, value, false);

                Thread thread = new Thread(threadObj.PublishAnalogDelta);
                thread.Start();
            }
        }

        public void PublishCrewUpdate(UIUpdateModel update)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, update);

                Thread thread = new Thread(threadObj.PublishCrewDelta);
                thread.Start();
            }
        }

        public void PublishIncident(IncidentReport report)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, report);

                Thread thread = new Thread(threadObj.PublishIncidentDelta);
                thread.Start();
            }
        }

        public void PublishCallIncident(UIUpdateModel call)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, call, true);

                Thread thread = new Thread(threadObj.PublishCallIncidentDelta);
                thread.Start();
            }
        }

        public void PublishUIBreakers(bool IsIncident, long incidentBreaker)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, IsIncident, incidentBreaker);

                Thread thread = new Thread(threadObj.PublishUIBreakersDelta);
                thread.Start();
            }
        }

        public void PublishEnergizationChange(List<UIUpdateModel> update)
        {
            foreach (IPublishing subscriber in PubSubscribeDB.Subscribers)
            {
                PublishThreadData threadObj = new PublishThreadData(subscriber, update);

                Thread thread = new Thread(threadObj.PublishEnergizationChange);
                thread.Start();
            }
        }
    }

    internal class PublishThreadData
    {
        private IPublishing subscriber;

        private string mrid;
        private States state;
        private float value;
        private List<UIUpdateModel> analogDeltaUpdate;
        private UIUpdateModel update;
        private List<UIUpdateModel> updateList;
        private IncidentReport report;
        private UIUpdateModel call;
        private bool Isincident;
        private long incidentBreaker;

        // for publishing digital, and analog delta
        public PublishThreadData(IPublishing subscriber, string mrid, States state, float value, bool isDigital)
        {
            this.subscriber = subscriber;
            this.mrid = mrid;

            if (isDigital)
                this.state = state;
            else
                this.value = value;
        }

        // for publishing crew update
        public PublishThreadData(IPublishing subscriber, UIUpdateModel update)
        {
            this.subscriber = subscriber;
            this.update = update;
        }

        public PublishThreadData(IPublishing subscriber, List<UIUpdateModel> update)
        {
            this.subscriber = subscriber;
            this.updateList = update;
        }

        // for publishing incident
        public PublishThreadData(IPublishing subscriber, IncidentReport report)
        {
            this.subscriber = subscriber;
            this.report = report;
        }

        // for publishing call incident
        public PublishThreadData(IPublishing subscriber, UIUpdateModel call, bool isCall)
        {
            this.subscriber = subscriber;
            this.call = call;
        }

        // for publishing ui breakers
        public PublishThreadData(IPublishing subscriber, bool IsIncident, long incidentBreaker)
        {
            this.subscriber = subscriber;
            this.Isincident = IsIncident;
            this.incidentBreaker = incidentBreaker;
        }

        public IPublishing Subscriber
        {
            get
            {
                return subscriber;
            }

            set
            {
                subscriber = value;
            }
        }

        public string MRID 
        {
            get
            {
                return mrid;
            }

            set
            {
                mrid = value;
            }
        }

        public States State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
            }
        }

        public float Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public List<UIUpdateModel> AnalogDeltaUpdate
        {
            get
            {
                return analogDeltaUpdate;
            }

            set
            {
                analogDeltaUpdate = value;
            }
        }
        public UIUpdateModel CrewUpdate { get => update; set => update = value; }
        public IncidentReport Report { get => report; set => report = value; }
        public UIUpdateModel Call { get => call; set => call = value; }
        public bool IsIncident { get => Isincident; set => Isincident = value; }
        public long IncidentBreaker { get => incidentBreaker; set => incidentBreaker = value; }

        public void PublishDigitalDelta()
        {
            try
            {
                subscriber.PublishDigitalUpdate(MRID, State);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }

        public void PublishAnalogDelta()
        {
            try
            {
                subscriber.PublishAnalogUpdate(MRID, Value);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }

        public void PublishCrewDelta()
        {
            try
            {
                subscriber.PublishCrewUpdate(CrewUpdate);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }

        public void PublishIncidentDelta()
        {
            try
            {
                subscriber.PublishIncident(Report);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }

        public void PublishCallIncidentDelta()
        {
            try
            {
                subscriber.PublishCallIncident(Call);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }

        public void PublishUIBreakersDelta()
        {
            try
            {
                subscriber.PublishUIBreakers(IsIncident, IncidentBreaker);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }

        public void PublishEnergizationChange()
        {
            try
            {
                subscriber.PublishEnergizationChange(updateList);
            }
            catch (Exception e)
            {
                PubSubscribeDB.RemoveSubsriber(subscriber);
            }
        }
    }
}
