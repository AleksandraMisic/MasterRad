using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMSSCADACommon.Responses;

namespace OMSSCADACommon.Commands
{
    public class ReadSingleDigital : Command
    {
        public override Response Execute()
        {
            return this.Receiver.ReadSingleDigital(this.Id);
        }
    }
}
