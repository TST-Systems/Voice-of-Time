namespace Voice_of_Time_Server
{
    internal enum ChatState : byte
    {
        ADMIN     = 0b_0100_0000,
        MODERATOR = 0b_0010_0000,
        MEMBER    = 0b_0001_0000,
        INVITED   = 0b_0000_1000,
        BLOCKED   = 0b_0000_0100
    }
}