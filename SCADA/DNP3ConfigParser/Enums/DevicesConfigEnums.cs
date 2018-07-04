using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3ConfigParser.Enums.DeviceConfig
{
    public enum DNPLevels
    {
        NONE = 0,
        LEVEL_1,
        LEVEL_2,
        LEVEL_3,
        LEVEL_4
    }

    public enum FunctionBlocks
    {
        SELF_ADDRESS = 0,
        DATA_SET,
        FILE_TRANSFER,
        VIRTUAL_TERMINAL,
        MAPPING_TO_IEC61850_OBJECT_MODELS,
        FUNCTION_CODE_31,
        SECURE_AUTHENTICATION
    }

    public enum MethodsForSettingConfigParams
    {
        // to do
    }

    public enum DNP3XMLFilesOnLine
    {
        RD_dnpDP,
        RD_dnpDPcap,
        RD_dnpDPcfg
    }

    public enum DNP3XMLFilesOffLine
    {
        // to do
    }

    public enum SupportedConnections
    {
        SERIAL,
        IP_NETWORKING
    }
}
