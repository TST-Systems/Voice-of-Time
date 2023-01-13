namespace VoTCore.Package.SData
{
    public class SData_Int : SData<Int32>
    {
        public SData_Int(int data) 
            : base(data, BodyType.INT_SDATA)
        {
        }
    }
}
