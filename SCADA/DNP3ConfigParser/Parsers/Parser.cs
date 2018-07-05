using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DNP3ConfigParser.Parsers
{
    public abstract class Parser
    {
        protected XmlDocument configuration;

        public abstract void Parse();
    }
}
