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
        const string VersionName = "Norm";

        [STAThread]
        static async Task Main(string[] args)
        {
            WriteFirstMessage();

            var server = new Server();
            while (server.IsAlive)
            {
                await Task.Delay(1);
            }
            Console.Write("\r                                                        ");
        }

        private static void WriteFirstMessage()
        {
            Console.Title = "Deadly Online Server ver. " + VersionName;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("\n\n\n" +
                          "             DDDDDD            oOOOOo            SSSSSs   \n" +
                          "            DD    dD         oO     OO          SS    Ss  \n" +
                          "           DD     DD        OO      OO           SSs      \n" +
                          "          DD     DD        OO      OO             SSs     \n" +
                          "         DD    dD          OO    oOO         Ss    SS     \n" +
                          "        DDDDDDD             OOOOO             SSSSS       \n" +
                          "\n\n");
            Console.ResetColor();

            Console.Write($"Welcome to the 'Deadly Online Server', whose version name is '{VersionName}.'\n" +
                          $"This server can be played 'Deadly Online' up to {Logic.Constants.ClientNumberMax} peoples.\n" +
                          $"This server will start soon. \n" +
                          $"If you found some issues where are in server or client, I want you to submit to ");

            
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(ServerHelper.IssuesPageUrl);
            Console.ResetColor();

            Console.Write($".\n" +
                          $"You can open the URL by 'issues' command.\n" +
                          $"                                                                           Thank you.\n" +
                          $"\n" +
                          $"--------------------------------------------------------------------------------------------\n");
        }
    }
}
