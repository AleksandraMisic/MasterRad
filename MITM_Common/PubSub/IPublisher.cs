﻿using MITM_Common.MITM_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MITM_Common.PubSub
{
    [ServiceContract]
    public interface IPublisher
    {
        [OperationContract]
        void ReturnConnectionInfo(ConnectionInfoStruct connectionInfo);
    }
}
