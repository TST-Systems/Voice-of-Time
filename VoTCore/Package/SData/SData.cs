using VoTCore.Package.Interfaces;

namespace VoTCore.Package.SData
{
    public abstract class SData<T> : IVOTPBody
    {
        public T? Data { get; }

        public BodyType Type { get; }

        protected SData(T? data, BodyType type)
        {
            Data = data;
            Type = type;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not SData<T> their) return false;

            if (Data == null && their.Data != null) return false;
            if (!(Data == null && their.Data == null) && !Data.Equals(their.Data)) return false;
            if (Type != their.Type) return false;

            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
