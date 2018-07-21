
namespace PCCommon
{
    public interface IIndustryProtocolHandler
    {
        byte[] PackData();

        void UnpackData(byte[] data, int length);     
    }
}
