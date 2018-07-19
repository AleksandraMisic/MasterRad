using FTN.Common;
using FTN.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionManagerContract.ClientNMS;

namespace TransactionManager.Services
{
    public class ClientNMSService : IClientNMS
    {
        private ModelResourcesDesc modelResourcesDesc = null;
        private NetworkModelGDAProxy gdaQueryProxy = null;

        public ClientNMSService()
        {
            modelResourcesDesc = new ModelResourcesDesc();
            gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
        }

        public ResourceDescription GetStaticDataForElement(long gid)
        {
            List<ModelCode> properties;

            try
            {
                properties = modelResourcesDesc.GetAllPropertyIds(ModelCodeHelper.ExtractTypeFromGlobalId(gid));
                return gdaQueryProxy.GetValues(gid, properties);
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
