using Newtonsoft.Json;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SnakeWPF.Pages
{
	/// <summary>
	/// Логика взаимодействия для Home.xaml
	/// </summary>
	public partial class Home : Page
	{
		public Home()
		{
			InitializeComponent();
		}
		private void StartGame(object sender, RoutedEventArgs e)
		{
			if (MainWindow.mainWindow.receivingUdpClient != null)
			{
				MainWindow.mainWindow.receivingUdpClient.Close();
			}
			if (MainWindow.mainWindow.tRec != null)
			{
				MainWindow.mainWindow.tRec.Abort();
			}
			IPAddress UserIPAddress;
			if (!IPAddress.TryParse(ip.Text, out UserIPAddress))
			{
				MessageBox.Show("Please use the IP addres in the format X.X.X.X");
				return;
			}
			int UserPort;
			if (!int.TryParse(port.Text, out UserPort))
			{
				MessageBox.Show("Please use the port as a number.");
				return;
			}
			MainWindow.mainWindow.ViewModelUserSettings.IPAddress = ip.Text;
			MainWindow.mainWindow.ViewModelUserSettings.Port = port.Text;
			MainWindow.mainWindow.ViewModelUserSettings.Name = name.Text;
			MainWindow.mainWindow.StartReceiver();
			MainWindow.mainWindow.Send("/start|" + JsonConvert.SerializeObject(MainWindow.mainWindow.ViewModelUserSettings));
		}
	}
}
