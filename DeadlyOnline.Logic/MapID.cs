namespace DeadlyOnline.Logic
{
    /// <summary>
    /// マップの識別番号
    /// </summary>
    public enum MapID
	{
		/// <summary>
		/// ワールドマップ(プレイヤー単体)
		/// </summary>
		WorldMap = 0,
		/// <summary>
		/// 広場(非戦闘エリア)
		/// </summary>
		Plaza = 1,
		/// <summary>
		/// 草原の道(戦闘可能エリア)
		/// </summary>
		MeadowRoad = 2,
	}
}
