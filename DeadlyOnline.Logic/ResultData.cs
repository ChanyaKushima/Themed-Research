namespace DeadlyOnline.Logic
{
	using System;

	[Serializable]
	public struct ResultData{
		public static readonly ResultData Empty = new ResultData();

		public ResultDataFormat DataFormat { get; set; }
		public object Data{ get; set; }
	}
}
