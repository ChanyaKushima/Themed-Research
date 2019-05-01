using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Games.Objects;
using Games.Objects.RPG;
using Games.Objects.Visual;

namespace DeadlyOnline.Client
{
	using DeadlyOnline.Logic;

	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		TcpClient Client;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void CommandAccept()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			NetworkStream stream = Client.GetStream();
			

			while (true)
			{
				try
				{
					
				}
				catch (Exception)
				{

					throw;
				}
			}
		}
	}
}
