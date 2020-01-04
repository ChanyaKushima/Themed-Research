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
        PlayerMoved_e = 10,

        MapRequest_c = 20,
        MapTransfer_s = 21,

        EnteredPlayerInMap_s = 23,
        LeftPlayerInMap_s = 24,
        UpdatePlayerInMap_s = 25,

        MainPlayerDataTransfer_s = 31,
        PlayerDataTransfer_s = 41,
        EnemyDataRequest_c = 51,
        EnemyDataTransfer_s = 52,

        Result = 100000,
        Debug = 9999999,
    }
}

