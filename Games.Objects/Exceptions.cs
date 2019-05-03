using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace Games.Objects
{
	/// <summary>
	/// ファイルの文法が異常だった時に出す例外
	/// </summary>
	[Serializable]
	public class FileGrammarException : Exception
	{
		public FileGrammarException() { }
		public FileGrammarException(string message) : base(message) { }
		public FileGrammarException(string message, Exception inner) : base(message, inner) { }
		protected FileGrammarException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// IDが見つからなかった時にスローされる例外
	/// </summary>
	[Serializable]
	public class IDNotFoundException : Exception
	{
		public IDNotFoundException() { }
		public IDNotFoundException(string message) : base(message) { }
		public IDNotFoundException(string message, Exception inner) : base(message, inner) { }
		protected IDNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
