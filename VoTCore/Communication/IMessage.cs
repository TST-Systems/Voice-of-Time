using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Communication
{
    public interface IMessage
    {
        Int16  TypeOfMessage   { get; }
        string MessageString   { get; }
        long   AuthorID        { get; }
        long   DateOfCreation  { get; }
    }
}
