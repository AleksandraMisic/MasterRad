using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3ConfigParser.Enums.NetworkConfig
{
    public enum TypeOfEndpoint
    {
        TCP_Initiating,     // Masters only
        TCP_Listening,      // Outstations only
        TCP_Dual,           // required for Masters
        UDP_Datagram        // required
    }

    public enum AcceptsTCPConnectionsOrUDPDatagrams
    {
        ALLOWS_ALL,
        LIMITS_BASED_ON_IP_ADDRESS,
        LIMITS_BASED_ON_LIST_OF_IP_ADDRESSES,
        LIMITS_BASED_ON_WILDCARD_IP_ADDRESS,
        LIMITS_BASED_ON_LIST_OF_WILDCARD_IP_ADDRESSES,
        OTHER
    }

    public enum TCPListenPortNumber
    {
        NOT_APPLICABLE,
        FIXED_AT_20000,
        CONFIGURABLE_RANGE,
        CONFIGURABLE_SELECT_FROM,
        CONFIGURABLE_OTHER
    }

    public enum TCPListenPortNumberOfRemoteDevice
    {
        NOT_APPLICABLE,
        FIXED_AT_20000,
        CONFIGURABLE_RANGE,
        CONFIGURABLE_SELECT_FROM,
        CONFIGURABLE_OTHER
    }

    public enum TCPKeepAliveTimer
    {
        FIXED_AT,                   // in ms
        CONFIGURABLE_RANGE,
        CONFIGURABLE_SELECT_FROM,
        CONFIGURABLE_OTHER
    }

    public enum LocalUDPPort
    {
        // to do
    }

    public enum DestinationUDPPortForRequests       // Masters only
    {
        // to do
    }

    public enum DestinationUDPPortForInitialUnsolicitedNullResponses    // Outstations only
    {
        // to do
    }

    public enum DestinationUDPPortForResponses
    {
        // to do
    }

    public enum MultipleMasterConnections       // Oustations only
    {
        METHOD_1,       // based on IP addresses (required)
        METHOD_2,       // based on IP ports (recommended)
        METHOD_3        // browsing for static data (optional)
    }

    public enum TimeSynchronizationSupport
    {
        NOT_SUPPORTED,
        DNP3_LAN_PROCEDURE,     // function code 24
        DNP3_WRITE_TIME,        // not recommended over LAN
        OTHER
    }
}
