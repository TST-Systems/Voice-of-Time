internal class VOTP
{
    private VOTPHeaderV1 head;
    private FileMessage body;
    private object serialized;

    public VOTP(object serialized)
    {
        this.serialized = serialized;
    }

    public VOTP(VOTPHeaderV1 head, FileMessage body)
    {
        this.head = head;
        this.body = body;
    }

    internal object Serialize()
    {
        throw new NotImplementedException();
    }
}