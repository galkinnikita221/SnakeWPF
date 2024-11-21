﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json;

namespace Snake_Elovikov_Galkin
{
	internal class Program
	{
		// Еловиков Степан и Галкин Никита ИСП-21-2
		public static List<Leaders> leaders = new List<Leaders>();
		public static List<ViewModelUserSettings> remoteIPAddress = new List<ViewModelUserSettings>();
		public static List<ViewModelGames> viewModelGames = new List<ViewModelGames>();
		public static int localPort = 5001;
		public static int maxSpeed = 15;
		static void Main(string[] args)
		{
			try
			{
				Thread tRec = new Thread(new ThreadStart(Receiver));
				tRec.Start();
				Thread tTime = new Thread(Timer);
				tTime.Start();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Возникло исключение: {ex.ToString()}\n{ex.Message}");
			}
		}
		private static void Send()
		{
			foreach (ViewModelUserSettings User in remoteIPAddress)
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
		public static void Receiver()
		{
			UdpClient receivingUdpClient = new UdpClient(localPort);
			IPEndPoint RemoteIpEndPoint = null;
			try
			{
				Console.WriteLine("Команды сервера:");
				while(true)
				{
					byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
					string returnData = Encoding.UTF8.GetString(receiveBytes);
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"Получил команду: {returnData.ToString()}");
					if (returnData.ToString().Contains("/start"))
					{
						string[] dataMessage = returnData.ToString().Split('|');
						ViewModelUserSettings viewModelUserSettings = JsonConvert.DeserializeObject<ViewModelUserSettings>(dataMessage[1]);
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine($"Подключился пользователь: {viewModelUserSettings.IPAddress}:{viewModelUserSettings.Port}");
						remoteIPAddress.Add(viewModelUserSettings);
						viewModelGames[viewModelUserSettings.IdSnake].IdSnake = viewModelUserSettings.IdSnake;
					}
					else
					{
						string[] dataMessage = returnData.ToString().Split('|');
						ViewModelUserSettings viewModelUserSettings = JsonConvert.DeserializeObject<ViewModelUserSettings>(dataMessage[1]);
						int IdPlayer = -1;
						IdPlayer = remoteIPAddress.FindIndex(x => x.IPAddress == viewModelUserSettings.IPAddress && x.Port == viewModelUserSettings.Port);
						if (IdPlayer != -1)
						{
							if (dataMessage[0] == "Up" && viewModelGames[IdPlayer].SnakesPlayers.direction != Snakes.Direction.Down)
								viewModelGames[IdPlayer].SnakesPlayers.direction = Snakes.Direction.Up;
							else if (dataMessage[0] == "Down" && viewModelGames[IdPlayer].SnakesPlayers.direction != Snakes.Direction.Up)
								viewModelGames[IdPlayer].SnakesPlayers.direction = Snakes.Direction.Down;
							else if (dataMessage[0] == "Left" && viewModelGames[IdPlayer].SnakesPlayers.direction != Snakes.Direction.Right)
								viewModelGames[IdPlayer].SnakesPlayers.direction = Snakes.Direction.Left;
							else if (dataMessage[0] == "Right" && viewModelGames[IdPlayer].SnakesPlayers.direction != Snakes.Direction.Left)
								viewModelGames[IdPlayer].SnakesPlayers.direction = Snakes.Direction.Right;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Возникло исключение: {ex.ToString()}\n{ex.Message}");
			}
		}
		public static int AddSnake()
		{
			ViewModelGames viewModelGamesPlayer = new ViewModelGames();
			viewModelGamesPlayer.SnakesPlayers = new Snakes()
			{
				Points = new List<Snakes.Point>()
				{
					new Snakes.Point() {X = 30, Y = 10},
					new Snakes.Point() {X = 20, Y = 10},
					new Snakes.Point() {X = 10, Y = 10}
				},
				direction = Snakes.Direction.Start
			};
			viewModelGamesPlayer.Points = new Snakes.Point(new Random().Next(10, 783), new Random().Next(10, 410));
			return viewModelGames.FindIndex(x => x == viewModelGamesPlayer);
		}
		public static void Timer()
		{
			while (true)
			{
				Thread.Sleep(100);
				List<ViewModelGames> RemoteSnakes = viewModelGames.FindAll(x => x.SnakesPlayers.GameOver);
				if (RemoteSnakes.Count > 0)
				{
					foreach (ViewModelGames DeadSnake in RemoteSnakes)
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine($"Отключён пользователь: {remoteIPAddress.Find(x => x.IdSnake == DeadSnake.IdSnake).IPAddress} : {remoteIPAddress.Find(x => x.IdSnake == DeadSnake.IdSnake).Port}");
						remoteIPAddress.RemoveAll(x => x.IdSnake == DeadSnake.IdSnake);
					}
					viewModelGames.RemoveAll(x => x.SnakesPlayers.GameOver);
				}
				foreach (ViewModelUserSettings User in remoteIPAddress)
				{
					Snakes Snake = viewModelGames.Find(x => x.IdSnake == User.IdSnake).SnakesPlayers;
					for (int i = Snake.Points.Count - 1; i >= 0; i--)
					{
						if (i != 0)
							Snake.Points[i] = Snake.Points[i - 1];
						else
						{
							int Speed = 10 + (int)Math.Round(Snake.Points.Count / 20f);
							if (Speed > maxSpeed) Speed = maxSpeed;
							if (Snake.direction == Snakes.Direction.Right)
								Snake.Points[i] = new Snakes.Point() { X = Snake.Points[i].X + Speed, Y = Snake.Points[i].Y };
							else if (Snake.direction == Snakes.Direction.Down)
								Snake.Points[i] = new Snakes.Point() { X = Snake.Points[i].X, Y = Snake.Points[i].Y + Speed };
							else if (Snake.direction == Snakes.Direction.Up)
								Snake.Points[i] = new Snakes.Point() { X = Snake.Points[i].X, Y = Snake.Points[i].Y - Speed };
							else if (Snake.direction == Snakes.Direction.Left)
								Snake.Points[i] = new Snakes.Point() { X = Snake.Points[i].X - Speed, Y = Snake.Points[i].Y };
						}
					}
					if (Snake.Points[0].X <= 0 || Snake.Points[0].X >= 793)
						Snake.GameOver = true;
					else if (Snake.Points[0].Y <= 0 || Snake.Points[0].Y >= 420)
						Snake.GameOver = true;
				}
			}
		}
		public static void SaveLeaders()
		{
			string json = JsonConvert.SerializeObject(leaders);
			StreamWriter SW = new StreamWriter("./leaders.txt");
			SW.WriteLine(json);
			SW.Close();
		}
		public static void LoadLeaders()
		{
			if (File.Exists("./leaders.txt"))
			{
				StreamReader SR = new StreamReader("./leaders.txt");
				string json = SR.ReadLine();
				SR.Close();
				if (!string.IsNullOrEmpty(json))
					leaders = JsonConvert.DeserializeObject<List<Leaders>>(json);
				else
					leaders = new List<Leaders>();
			}
			else
				leaders = new List<Leaders>();
		}
	}
}
