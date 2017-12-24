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

namespace Scenario2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int _rows;
		private int _cols;
		private double _errorChance;

		private string _errorMessage;

		//public MainWindow()
		//{
		//	InitializeComponent();
		//	ErrorMessageLabel = new Label();
		//}


		private void ColsInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			if (int.TryParse(input, out var cols))
			{
				if (cols < 2)
					_errorMessage = "Reikšmė privalo būti didesnė už 1!";

				else
				{
					_errorMessage = string.Empty;
					_cols = cols;
				}
			}
			else
				_errorMessage = "Galimos reikšmės tik sveikieji skaičiai!";

			ShowErrorMessage();
		}

		private void RowsInput_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void ErrorChanceInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			//throw new NotImplementedException();
		}

		private void ShowErrorMessage()
		{
			ErrorMessageLabel.Content = _errorMessage;
		}
	}
}
