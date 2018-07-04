using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNP3TCPDriver.ApplicationLayer
{
    public enum FunctionCodes : Byte
    {
        //Request codes
        CONFIRM             = 0x00,
        READ                = 0x01,
        WRITE               = 0x02,
        SELECT              = 0x03,
        OPERATE             = 0x04,
        DIRECT_OPERATE      = 0x05,
        DIRETCT_OPERATE_NR  = 0x06,
        IMMED_FREEYE        = 0x07,
        IMMED_FREEYE_NR     = 0x08,
        FREEZE_CLEAR        = 0x09,
        FREEZE_CLEAR_NR     = 0x0A,
        FREEZA_AT_TIME      = 0x0B,
        FREEYE_AT_TIME_NR   = 0x0C,
        COLD_RESTART        = 0x0D,
        WARM_RESTART        = 0x0E,
        INITIALIZE_DATA     = 0x0F,
        INITIALIZE_APPL     = 0x10,
        START_APPL          = 0x11,
        STOP_APPL           = 0x12,
        SAVE_CONFIG         = 0x13,
        ENABLE_UNSOLICITED  = 0x14,
        DISABLE_UNSOLICITED = 0x15,
        ASSIGN_CLASS        = 0x16,
        DELAY_MEASURE       = 0x17,
        RECORD_CURRENT_TIME = 0x18,
        OPEN_FILE           = 0x19,
        CLOSE_FILE          = 0x1A, 
        DELETE_FILE         = 0x1B,
        GET_FILE_INFO       = 0x1C,
        AUTHENTICATE_FILE   = 0x1D,
        ABORT_FILE          = 0x1E,
        ACTIVATE_CONFIG     = 0x1F,
        AUTHENTICATE_REQ    = 0x20,
        AUTHENTICATE_ERR    = 0x21,

        //Response codes 
        RESPONSE                = 0x81,
        UNSOLICITED_RESPONSE    = 0x82,
        AUTHENTICATE_RESP       = 0x83
    }
}
