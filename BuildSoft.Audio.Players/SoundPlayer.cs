using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BuildSoft.Audio.Players
{
	public abstract class SoundPlayer
	{
		protected Uri FileUri{ get; set; }

		protected string _aliasName;

		public string FileLocalPath => FileUri.LocalPath;
		public string FileAbsolutePath => FileUri.AbsolutePath;

		protected SoundPlayer(string path, string alias)
		{
			FileUri = InternalMethods.ResolveUri(path);
			if (FileUri is null)
			{
				ThrowHelper.ThrowArgumentException();
			}
			_aliasName = alias;
		}

		public abstract void Load();
		public abstract Task LoadAsync();
		public abstract void Play();
		public abstract void PlayLooping();
		public abstract void Stop();
		public abstract void Close();
		public abstract void SeekStart();
		public abstract void SeekEnd();
		public abstract void Seek(int ms);

		[DllImport("winmm.dll")]
		protected static extern int mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);
	}

	internal enum Mode { None, Playing, Stopped, Paused }
}
