using DNP3ConfigParser.Configuration;
using System.Xml.Linq;

namespace DNP3ConfigParser.Parsers
{
    public abstract class UniversalConfigurationParser
    {
        protected XDocument configFile;

        public UniversalConfiguration Configuration;

        public abstract void Parse();
    }
}
