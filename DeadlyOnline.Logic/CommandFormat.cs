namespace DeadlyOnline.Logic
{
    public enum CommandFormat
    {
        None = 0,
        CreateAccount_c = 1,
        Login_c = 2,
        Logout_c = 3,
        DataRequest_c = 4,
        DataUpdate_e = 5,
        MapMove_e = 10,
        MapRequest_c=20,
        MapTransfer_s=21,

        Result = 100000,
        Debug = 9999999,
    }
}

