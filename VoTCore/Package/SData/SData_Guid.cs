namespace VoTCore.Package.SData
{
    public class SData_Guid : SData<Guid>
    {
        public SData_Guid(Guid data)
            : base(data, BodyType.SDATA_GUID)
        {
        }
    }
}
