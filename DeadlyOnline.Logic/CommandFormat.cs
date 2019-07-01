namespace DeadlyOnline.Logic
{
    public enum CommandFormat
    {
        None = 0,
        CreateAccount = 1,
        Login = 2,
        Logout = 3,
        DataRequest = 4,
        DataUpdate = 5,
        MapMove = 10,

        Result = 100000,
        Debug = 9999999,
    }
}

