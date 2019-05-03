namespace DeadlyOnline.Logic
{
	using System;

	[Serializable]
	public readonly struct ResultData
	{
		public static readonly ResultData Empty = new ResultData();

		public ResultDataFormat DataFormat { get; }
		public object Data { get; }

		public ResultData(ResultDataFormat dataFormat, object data = null)
		{
			DataFormat = dataFormat;
			Data = data;
		}
	}
}
