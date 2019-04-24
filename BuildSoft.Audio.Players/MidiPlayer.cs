using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Threading;

namespace BuildSoft.Audio.Players
{
	public class MidiPlayer : SoundPlayer, IDisposable
	{
		private static ulong objCnt = 0;
		private static readonly string BaseAlias = "midiplayer";

		private Dispatcher dispatcher;

		public bool IsLoadCompleted { get; set; }

		#region constructor(s) and destructor

		public MidiPlayer(string path) : this(path, BaseAlias + objCnt)
		{
		}

		public MidiPlayer(string path, string alias) : base(path, alias)
		{
			dispatcher = Dispatcher.CurrentDispatcher;
			objCnt++;
		}
		~MidiPlayer()
		{
			Dispose(false);
		}

		#endregion

		#region public methods

		public override void Load()
		{
			if (IsLoadCompleted) { return; };
			string cmd = $@"open ""{FileUri.LocalPath}"" type sequencer alias {_aliasName}";
			if (mciSendString(cmd, null, 0, IntPtr.Zero) != 0)
			{
				ThrowHelper.ThrowArgumentException();
			}
			cmd = $"set {_aliasName} time format milliseconds";
			if (mciSendString(cmd, null, 0, IntPtr.Zero) != 0)
			{
				ThrowHelper.ThrowArgumentException();
			}
			IsLoadCompleted = true;
		}

		public override async Task LoadAsync()
		{
			await Task.Run(() => dispatcher.Invoke(Load));
		}


		public override void Play()
		{
			Load();
			mciSendString($"play {_aliasName} notify", null, 0, IntPtr.Zero);
		}

		public override void PlayLooping()
		{
			Load();
			mciSendString($"play {_aliasName} notify repeat", null, 0, IntPtr.Zero);
		}

		public override void Stop()
		{
			mciSendString($"stop {_aliasName}", null, 0, IntPtr.Zero);
		}

		public override void Close()
		{
			if (IsLoadCompleted)
			{
				mciSendString($"close {_aliasName}", null, 0, IntPtr.Zero);
				IsLoadCompleted = false;
			}
		}

		public override void Seek(int ms)
		{
			mciSendString($"seek {_aliasName} ", null, 0, IntPtr.Zero);
		}

		public override void SeekStart()
		{
			mciSendString($"seek {_aliasName} to start", null, 0, IntPtr.Zero);
		}

		public override void SeekEnd()
		{
			mciSendString($"seek {_aliasName} to end", null, 0, IntPtr.Zero);
		}

		#endregion

		#region private methods

		private string GetMode()
		{
			StringBuilder buffer = new StringBuilder(256);
			mciSendString($"status {_aliasName} mode", buffer, buffer.Capacity, IntPtr.Zero);
			return buffer.ToString();
		}

		private string GetPosition()
		{
			StringBuilder buffer = new StringBuilder(256);
			mciSendString($"status {_aliasName} position", buffer, buffer.Capacity, IntPtr.Zero);
			return buffer.ToString();
		}

		private string GetLength()
		{
			StringBuilder buffer = new StringBuilder(256);
			mciSendString($"status {_aliasName} length", buffer, buffer.Capacity, IntPtr.Zero);
			return buffer.ToString();
		}

		private int _mciSendCmd(string cmd)
		{
			return mciSendString($"{cmd} {_aliasName}", null, 0, IntPtr.Zero);
		}

		#endregion

		#region Dispose Methods

		private bool disposed = false;

		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (IsLoadCompleted)
				{
					mciSendString($"close {_aliasName}", null, 0, IntPtr.Zero);
				}
				if (disposing)
				{
					GC.ReRegisterForFinalize(this);
					GC.SuppressFinalize(this);
				}
				disposed = true;
			}
		}

		#endregion

	}
}
