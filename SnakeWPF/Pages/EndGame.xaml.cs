using System.Windows;
using System.Windows.Controls;

namespace SnakeWPF.Pages
{
	/// <summary>
	/// Логика взаимодействия для EndGame.xaml
	/// </summary>
	public partial class EndGame : Page
	{
		public EndGame()
		{
			InitializeComponent();
			name.Content = MainWindow.mainWindow.ViewModelUserSettings.Name;
			top.Content = MainWindow.mainWindow.ViewModelGames.Top;
			glasses.Content = $"{MainWindow.mainWindow.ViewModelGames.SnakesPlayers.Points.Count - 3} glasses";
			MainWindow.mainWindow.receivingUdpClient.Close();
			MainWindow.mainWindow.tRec.Abort();
			MainWindow.mainWindow.ViewModelGames = null;
		}
		private void OpenHome(object sender, RoutedEventArgs e)
		{
			MainWindow.mainWindow.OpenPage(MainWindow.mainWindow.Home);
		}
	}
}
