using System;
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
using System.Net.Security;
using System.Runtime.CompilerServices;
using Games.Object.RPG;
using Games.Object.Visual;


namespace DeadlyOnline.Server
{
    class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            Console.Write("-----------------------------------------------------\n" +
                          "*                                                   *\n" +
                          "*    Deadly Online Server For Windows 10            *\n" +
                          "*                            Version Name 'Norm'    *\n" +
                          "*                                                   *\n" +
                          "-----------------------------------------------------\n");

            var server = new Server();
            while (server.IsAlive)
            {
                await Task.Delay(1);
            }
        }
    }
}
