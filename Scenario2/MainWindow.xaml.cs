using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Logic;

namespace Scenario2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<List<byte>> _matrix;
		private List<List<byte>> _tempMatrix;
		private MatrixG _matrixG;
		private MatrixH _matrixH;
		private Channel _channel;

		private string _matrixRow;

		private int _rows = -1;
		private int _cols = -1;
		private bool _entersOwnMatrix = false;
		private double _errorChance = -1;

		private readonly Validator _validator = new Validator();
		private string _errorMessage;


		private void InputErrorChance_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			try
			{
				_errorChance = _validator.ValidateErrorProbability(input);
				_channel = new Channel(_errorChance);
			}
			catch (Exception ex) { _errorMessage = ex.Message; }

			ShowErrorMessage();
		}

		private void InputCols_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			try                  { _cols = _validator.ValidateNumberOfCols(input); }
			catch (Exception ex) { _errorMessage = ex.Message; }

			ShowErrorMessage();
		}

		private void InputRows_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			try                  { _rows = _validator.ValidateNumberOfRows(input); }
			catch (Exception ex) { _errorMessage = ex.Message; }

			ShowErrorMessage();
		}

		private void CheckBoxOwnMatrix_Checked(object sender, RoutedEventArgs e)
		{
			var value = ((CheckBox) sender).IsChecked;

			if (value.HasValue && value.Value)
				_entersOwnMatrix = true;

			else
				_entersOwnMatrix = false;
		}

		private void ButtonSubmitVariables_Click(object sender, RoutedEventArgs e)
		{
			var inputIsValid = false;

			if (_errorChance == -1)
				_errorMessage = "Neteisingai įvesta klaidos tikimybė (p).";

			else if (_cols == -1)
				_errorMessage = "Neteisingai įvestas matricos ilgis (n).";

			else if (_rows == -1)
				_errorMessage = "Neteisingai įvesta matricos dimensija (k).";

			else
				inputIsValid = true;

			if (!inputIsValid)
			{
				ShowErrorMessage();
				return;
			}

			if (_entersOwnMatrix)
				LetUserEnterGMatrix();
			else
			{
				_matrixG = new MatrixG(_cols, _rows);
				_matrixH = _matrixG.GetMatrixH();
			}

			LabelCols.Visibility = Visibility.Hidden;
			InputCols.Visibility = Visibility.Hidden;

			LabelRows.Visibility = Visibility.Hidden;
			InputRows.Visibility = Visibility.Hidden;

			CheckBoxOwnMatrix.Visibility = Visibility.Hidden;
			ButtonSubmitVariables.Visibility = Visibility.Hidden;
			
			LabelTextToSend.Visibility = Visibility.Visible;
			InputTextToSend.Visibility = Visibility.Visible;

			LabelSentWithoutCoding.Visibility = Visibility.Visible;
			TextBlockSendWithoutCoding.Visibility = Visibility.Visible;

			LabelSentWithCoding.Visibility = Visibility.Visible;
			TextBlockSendWithCoding.Visibility = Visibility.Visible;
		}



		private void ShowErrorMessage()
		{
			ErrorMessageLabel.Content = _errorMessage;
			_errorMessage = string.Empty;
		}


		// Todo: move to its own class?
		private void LetUserEnterGMatrix()
		{
			_tempMatrix = new List<List<byte>>(_rows);


			var row = _tempMatrix.Count + 1;

			LabelEnterMatrixRow.Visibility = Visibility.Visible;
			LabelEnterMatrixRow.Content = $"Įveskite {row}-ąjį vektorių: ";
			InputMatrixRow.Visibility = Visibility.Visible;
		}

		private void InputMatrixRow_TextChanged(object sender, TextChangedEventArgs e)
		{
			_matrixRow = ((TextBox) sender).Text;
		}

		private static List<byte> StringToByteListVector(string vector)
		{
			var length = vector.Length;
			var row = new List<byte>(length);
			for (var c = 0; c < length; c++)
			{
				row.Add((byte)char.GetNumericValue(vector[c]));
			}
			return row;
		}

		private void InputMatrixRow_KeyUp(object sender, KeyEventArgs e)
		{
			if (_matrixG != null)
				return;

			if (e.Key != Key.Enter)
				return;

			// Paimame įvestą tekstą.
			try
			{
				var row = _validator.ValidateGMatrixRow(_matrixRow);
				_tempMatrix.Add(row);
				LabelEnterMatrixRow.Content = $"Įveskite {_tempMatrix.Count + 1}-ąjį vektorių: ";

				// Jeigu jau turimę matricą.
				if (_tempMatrix.Count == _rows)
				{
					try
					{
						_matrixG = new MatrixG(length: _cols, dimension: _rows, matrix: _tempMatrix);
						_matrixH = _matrixG.GetMatrixH();

						LabelEnterMatrixRow.Visibility = Visibility.Hidden;
						InputMatrixRow.Visibility = Visibility.Hidden;

						LabelTextToSend.Visibility = Visibility.Visible;
						InputTextToSend.Visibility = Visibility.Visible;

					}
					catch (Exception ex)
					{
						_errorMessage = ex.Message;
						ShowErrorMessage();
						_tempMatrix.Clear();
						LabelEnterMatrixRow.Content = $"Įveskite {_tempMatrix.Count + 1}-ąjį vektorių: ";
					}
				}
			}
			// Jeigu buvo įvestas netinkamas vektorius.
			catch (Exception ex)
			{
				_errorMessage = ex.Message;
				ShowErrorMessage();
			}
		}

		private void InputTextToSend_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			SendText(((TextBox)sender).Text);
			SendTextUnencoded(((TextBox)sender).Text);
		}

		private void SendTextUnencoded(string text)
		{
			// This will contain something like: 89, 112, 201, 5, ...
			var textAsBytes = Encoding.ASCII.GetBytes(text);
			var stringBuilder = new StringBuilder();

			// 1. Convert the text into a string made up of binary symbols.
			foreach (var word in textAsBytes)
			{
				var @byte = Convert.ToString(value: word, toBase: 2)
								   .PadLeft(totalWidth: 8, paddingChar: '0');
				stringBuilder.Append(@byte);
			}

			var textToSend = ConvertStringToByteList(stringBuilder.ToString());
				var deformed = _channel.SendVectorThrough(textToSend);

			var textInBinary = ConvertByteListToString(deformed);
			var index = 0;
			var decodedTextAsList = new List<byte>();
			// 8. Convert it back in to numbers.
			for (var i = 0; i < textInBinary.Length;)
			{
				// 8.1 Put it in to groups of 8 bits.
				var byteAsBinaryString = string.Empty;
				for (var c = 0; c < 8; c++)
				{
					if (i == textInBinary.Length)
					{
						//byteAsBinaryString += '0';
						c++;
					}
					else
					{
						byteAsBinaryString += textInBinary[i];
						i++;
					}
				}

				// 8.2 Convert it to a decimal number.
				var byteAsDecimalString = Convert.ToByte(byteAsBinaryString, 2);
				decodedTextAsList.Add(byteAsDecimalString);
			}

			var revertedText = Encoding.ASCII.GetString(decodedTextAsList.ToArray());
			TextBlockSendWithoutCoding.Text = revertedText;
		}

		private void SendText(string text)
		{
			// This will contain something like: 89, 112, 201, 5, ...
			var textAsBytes = Encoding.ASCII.GetBytes(text);
			var stringBuilder = new StringBuilder();

			// 1. Convert the text into a string made up of binary symbols.
			foreach (var word in textAsBytes)
			{
				var @byte = Convert.ToString(value: word, toBase: 2)
								   .PadLeft(totalWidth: 8, paddingChar: '0');
				stringBuilder.Append(@byte);
			}

			// 2. Split it into length that matches the dimension of the matrix.
			var addedExtra = 0;
			var textInBinary = stringBuilder.ToString();
			stringBuilder = stringBuilder.Clear();
			for (var c = 0; c < textInBinary.Length;)
			{
				var toEncodeAsString = string.Empty;
				for (var r = 0; r < _rows; r++)
				{
					if (c == textInBinary.Length)
					{
						toEncodeAsString += '0';
						addedExtra++;
					}
					else
					{
						toEncodeAsString += textInBinary[c];
						c++; // Move to the next bit.	
					}
				}

				// 3. Encode the word.
				var toEncodeAsList = ConvertStringToByteList(toEncodeAsString);
				var encoded = _matrixG.Encode(toEncodeAsList);

				// 4. Send it through the channel.
				var deformed = _channel.SendVectorThrough(encoded);

				// 5. Decode the vector.
				var decoded = _matrixH.Decode(deformed);

				// 6. Get the original word.
				var fullyDecoded = _matrixG.Decode(decoded);

				// 7. Put it all into a string.
				stringBuilder.Append(ConvertByteListToString(fullyDecoded));
			}

			textInBinary = stringBuilder.ToString();
			var index = 0;
			var decodedTextAsList = new List<byte>();
			// 8. Convert it back in to numbers.
			for (var i = 0; i < textInBinary.Length;)
			{
				// 8.1 Put it in to groups of 8 bits.
				var byteAsBinaryString = string.Empty;
				for (var c = 0; c < 8; c++)
				{
					if (i == textInBinary.Length)
					{
						//byteAsBinaryString += '0';
						c++;
					}
					else
					{
						byteAsBinaryString += textInBinary[i];
						i++;
					}
				}

				// 8.2 Convert it to a decimal number.
				var byteAsDecimalString = Convert.ToByte(byteAsBinaryString, 2);
				decodedTextAsList.Add(byteAsDecimalString);
			}

			var revertedText = Encoding.ASCII.GetString(decodedTextAsList.ToArray());
			TextBlockSendWithCoding.Text = revertedText;
		}

		private static List<byte> ConvertStringToByteList(string vector)
		{
			var list = new List<byte>();
			foreach (var bit in vector)
			{
				list.Add(bit == '0' ? (byte)0 : (byte)1);
			}
			return list;
		}

		private static string ConvertByteListToString(List<byte> vector)
		{
			var result = string.Empty;
			foreach (var bit in vector)
			{
				if (bit == 0)
					result += '0';
				else
					result += '1';
			}
			return result;
		}
	}
}
