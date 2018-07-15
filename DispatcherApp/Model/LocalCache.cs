using DMSCommon.Model;
using IMSContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherApp.Model
{
    public class LocalCache
    {
        public static Dictionary<string, Source> Sources;
        public static List<IncidentReport> LatestIncidents;

        static LocalCache()
        {
            Sources = new Dictionary<string, Source>();
            LatestIncidents = new List<IncidentReport>();
        }
    }
}
