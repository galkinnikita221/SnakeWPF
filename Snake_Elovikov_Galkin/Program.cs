using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Snake_Elovikov_Galkin
{
	internal class Program
	{
		public static List<Leaders> leaders = new List<Leaders>();
		public static List<ViewModelUserSettings> remoteIPAddress = new List<ViewModelUserSettings>();
		public static List<ViewModelGames> viewModelGames = new List<ViewModelGames>();
		public static int localPort = 5001;
		public static int maxSpeed = 15;
		static void Main(string[] args)
		{

		}
		private static void Send()
		{
			UdpClient sender = new UdpClient();
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(User.IPAddress), int.Parse(User.Port));
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(viewModelGames.Find(x => x.IdSnake == User.IdSnake)));
				sender.Send(bytes, bytes.Length, endPoint);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"Отправил данные пользователю: {User.IPAddress}:{User.Port}");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Возникло исключение: {ex.ToString()}\n{ex.Message}");
			}
			finally
			{
				sender.Close();
			}
		}
	}
}
