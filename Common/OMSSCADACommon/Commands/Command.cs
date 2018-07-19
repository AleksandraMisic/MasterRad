using OMSSCADACommon.Commands;
using System;
using System.Runtime.Serialization;
using OMSSCADACommon.Responses;

namespace OMSSCADACommon.Commands
{
    [DataContract]
    [KnownType(typeof(ReadAll))]
    [KnownType(typeof(WriteSingleDigital))]
    [KnownType(typeof(WriteSingleAnalog))]
    [KnownType(typeof(ReadSingleDigital))]
    [KnownType(typeof(ReadSingleAnalog))]
    public abstract class Command
    {
        [IgnoreDataMember]
        public ICommandReceiver Receiver { get; set; }

        [DataMember]
        public string Id { get; set; }

        public abstract Response Execute();
    }
}
