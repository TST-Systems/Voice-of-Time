﻿namespace VoTCore.Package.SData
{
    public class SData_Long : SData<Int64>
    {
        public SData_Long(long data) 
            : base(data, BodyType.SDATA_LONG)
        {
        }
    }
}