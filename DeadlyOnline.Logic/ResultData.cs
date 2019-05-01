namespace DeadlyOnline.Logic
{
	public struct ResultData{
		public static readonly ResultData Empty = new ResultData();

		public DataFormat DataFormat { get; set; }
		public object Data{ get; set; }
	}
}
