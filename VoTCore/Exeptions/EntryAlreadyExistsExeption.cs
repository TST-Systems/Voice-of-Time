namespace VoTCore.Exeptions
{
    public class EntryAlreadyExistsExeption : Exception
    {
        public EntryAlreadyExistsExeption()               : base("Error: Entry already Exists") { }
        public EntryAlreadyExistsExeption(string message) : base($"Error: Entry already Exists\n{message}") { }
    }
}
