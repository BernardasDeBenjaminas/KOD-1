using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Logic;

namespace Scenario2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _errorMessage = string.Empty;    // Naudojamas klaidos žinutėms atvaizduoti.
		private Validator _validator = new Validator(); // Naudojamas vartotojo įvesties validavimui.

		private int _rows = -1; // '_matrixG' dimensija (k).
		private int _cols = -1; // '_matrixG' ilgis (n).
		private MatrixG _matrixG; // G matrica (vartotojo įvesta arba kompiuterio sugeneruota).
		private MatrixH _matrixH; // H matrica (gauta iš '_matrixG').

		private string _textToSend;       // Tekstas, kurį siųsime kanalu.
		private Channel _channel;         // Kanalas, kuriuo siųsiu vektorius.
		private double _errorChance = -1; // Tikimybė kanale įvykti klaidai (p). -1, nes 0 yra leidžiama reikšmė.

		private bool _entersOwnMatrix;        // Ar vartotojas pats įves generuojančią matricą?
		private List<List<byte>> _tempMatrix; // Laikina matrica, kurią naudosiu vartotojui pačiam suvedinėjant G matricą.

		// Naudojami statistikai.
		private bool _shouldCountersBeNulled;   // Ar reikia pradėti statistiką skaičiuoti iš naujo? (Pvz.: jei pakeitė tekstą siuntimui)
		private int _timesSentPlain;			// Kiek kartų buvo siųstas tas pats neužkoduotas tekstas.
		private int _timesSendEncoded;			// Kiek kartų buvo siųstas tas pats užkoduotas tektas.
		private int _totalMismatchCountPlain;   // Kiek kartų iš viso buvo padaryta klaidų siunčiant koduotą tekstą.
		private int _totalMismatchCountEncoded; // Kiek kartų iš viso buvo padaryta klaidų siunčiant koduotą tekstą. 


		#region Kintamųjų surinkimui iš vartotojo.

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas įveda bet kokį simbolį į klaidos tikimybės langelį.
		/// </summary>
		private void InputErrorChance_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			try
			{
				_errorChance = _validator.ValidateErrorProbability(input);
				_channel = new Channel(_errorChance);
				// Nes negalima toliau pildyti statistikos su naujai pateikta klaidos tikimybe.
				_shouldCountersBeNulled = true;
			}
			catch (Exception ex) { _errorMessage = ex.Message; }

			ShowErrorMessage();
		}

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas įveda bet kokį simbolį į matricos ilgio langelį.
		/// </summary>
		private void InputCols_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			try { _cols = _validator.ValidateNumberOfCols(input); }
			catch (Exception ex) { _errorMessage = ex.Message; }

			ShowErrorMessage();
		}

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas įveda bet kokį simbolį į kodo dimensijos langelį.
		/// </summary>
		private void InputRows_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			try { _rows = _validator.ValidateNumberOfRows(input); }
			catch (Exception ex) { _errorMessage = ex.Message; }

			ShowErrorMessage();
		}

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas padeda varnelę ant "Įvesiu savo matricą" langelį.
		/// </summary>
		private void CheckBoxOwnMatrix_Checked(object sender, RoutedEventArgs e)
		{
			var value = ((CheckBox)sender).IsChecked;

			if (value.HasValue && value.Value)
				_entersOwnMatrix = true;

			else
				_entersOwnMatrix = false;
		}

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas paspaudžia "Pateikti" mygtuką.
		/// </summary>
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

			// Paslepiame kodo ilgį.
			InputCols.Visibility = Visibility.Hidden;
			LabelCols.Visibility = Visibility.Hidden;

			// Paslepiame kodo dimensiją.
			InputRows.Visibility = Visibility.Hidden;
			LabelRows.Visibility = Visibility.Hidden;

			// Paslepiame kas liko.
			CheckBoxOwnMatrix.Visibility = Visibility.Hidden;     // Varnelė savo matricos pateikimui.
			ButtonSubmitVariables.Visibility = Visibility.Hidden; // Mygtukas visko pateikimui.

			if (_entersOwnMatrix)
			{
				LabelAdviceInputMatrixRow.Visibility = Visibility.Visible;
				LetUserEnterGMatrix();
			}
			else
			{
				_matrixG = new MatrixG(_cols, _rows);
				_matrixH = _matrixG.GetMatrixH();
				// Paslepiame nebeaktualius langelius ir parodome aktualius.
				HideOldFieldsAndShowNewOnes();
			}
		}

		#region Jeigu vartotojas nori pats įvesti matricą.

		/// <summary>
		/// Parodo laukus, atsakingus už norimos matricos įvedimą.
		/// </summary>
		private void LetUserEnterGMatrix()
		{
			_tempMatrix = new List<List<byte>>(_rows);

			InputMatrixRow.Visibility = Visibility.Visible;
			LabelInputMatrixRow.Visibility = Visibility.Visible;

			var row = _tempMatrix.Count + 1;
			LabelInputMatrixRow.Content = $"Įveskite {row}-ąjį vektorių: ";
		}

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas paspaudžia "Enter" klavišą įvedęs vektorių matricai.
		/// </summary>
		private void InputMatrixRow_KeyUp(object sender, KeyEventArgs e)
		{
			// Reiškia matrica jau sukurta - nebereikia čia nieko daryti.
			if (_matrixG != null)
				return;

			if (e.Key != Key.Enter)
				return;

			try
			{
				// Patikriname ar įvestas vektorius tinkamas matricai.
				var row = _validator.ValidateGMatrixRow(InputMatrixRow.Text);
				_tempMatrix.Add(row);
				InputMatrixRow.Text = string.Empty;
				LabelInputMatrixRow.Content = $"Įveskite {_tempMatrix.Count + 1}-ąjį vektorių: ";

				// Jeigu jau turime pakankamai matricos eilučių.
				if (_tempMatrix.Count == _rows)
				{
					try
					{
						_matrixG = new MatrixG(length: _cols, dimension: _rows, matrix: _tempMatrix);
						_matrixH = _matrixG.GetMatrixH();
						// Paslepiame nebeaktualius langelius ir parodome aktualius.
						HideOldFieldsAndShowNewOnes();
					}
					catch (Exception ex)
					{
						_errorMessage = ex.Message;
						ShowErrorMessage();
						// Nepavyko sukurti matricos iš paduotų vektorių tad išvalome esamą matricą.
						_tempMatrix.Clear();
						LabelInputMatrixRow.Content = $"Įveskite {_tempMatrix.Count + 1}-ąjį vektorių: ";
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

		#endregion


		#endregion

		#region Pasiruošimas teksto siuntimui kanalu.

		/// <summary>
		///  Paslepia matricos kintamųjų surinkimo langelius ir parodo teksto siuntimui kanalu reikalingus langelius.
		/// </summary>
		private void HideOldFieldsAndShowNewOnes()
		{
			HideErrorMessage();

			// Paslepiame kodo ilgį.
			InputCols.Visibility = Visibility.Hidden;
			LabelCols.Visibility = Visibility.Hidden;

			// Paslepiame kodo dimensiją.
			InputRows.Visibility = Visibility.Hidden; 
			LabelRows.Visibility = Visibility.Hidden; 

			// Paslepiame matricos vektoriaus įvedimo laukus.
			LabelInputMatrixRow.Visibility = Visibility.Hidden;
			InputMatrixRow.Visibility = Visibility.Hidden;

			// Paslepiame kas liko.
			CheckBoxOwnMatrix.Visibility = Visibility.Hidden;     // Varnelė savo matricos pateikimui.
			ButtonSubmitVariables.Visibility = Visibility.Hidden; // Mygtukas visko pateikimui.

			// Parodome teksto siuntimui aktualius laukus.
			LabelInputTextToSend.Visibility = Visibility.Visible;
			InputTextToSend.Visibility = Visibility.Visible;
			ButtonSendText.Visibility = Visibility.Visible;

			// Parodome patarimus.
			LabelAdvice1.Visibility = Visibility.Visible;
			LabelAdvice2.Visibility = Visibility.Visible;
			LabelAdvice3.Visibility = Visibility.Visible;
		}

		#endregion

		#region Teksto siuntimas kanalu.

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas paspaudžia teksto siuntimo kanalu mygtuką.
		/// </summary>
		private async void ButtonSendText_Click(object sender, RoutedEventArgs e)
		{
			if (_textToSend != InputTextToSend.Text)
			{
				_textToSend = InputTextToSend.Text;
				_shouldCountersBeNulled = true;
			}

			if (_shouldCountersBeNulled)
			{
				NullStatisticCounters();
				_shouldCountersBeNulled = false;
			}

			// Tekstas siuntimui.
			LabelSymbolCount.Content = $"Simbolių skaičius: {_textToSend.Length}";
			LabelSymbolCount.Visibility = Visibility.Visible;

			// Siųstas be kodavimo.
			PrepareResultsOfSentPlain(originalText: _textToSend,
									  receivedText: await Task.Run(() => SendPlainText(_textToSend)));

			// Siųstas su kodavimu.
			PrepareResultsOfSentEncoded(originalText: _textToSend,
										receivedText: await Task.Run(() => SendEncodedText(_textToSend)));
		}

		#region Nekoduoto.

		/// <summary>
		/// Išsiunčia neužkoduotą tekstą kanalu.
		/// </summary>
		/// <param name="text">Tekstas, kurį norima siųsti kanalu.</param>
		/// <returns>Iš kanalo grįžęs rezultatas.</returns>
		private string SendPlainText(string text)
		{
			var binaryText = ConvertTextToBinary(text);
			var textAsVector = ConvertStringToByteList(binaryText);
			var deformed = _channel.SendVectorThrough(textAsVector);
			return ConvertBinaryVectorToText(deformed);
		}

		/// <summary>
		/// Parodo visą turimą informaciją bei statistiką apie neužkoduoto teksto siuntimą kanalu.
		/// </summary>
		/// <param name="originalText">Tekstas, kuris buvo išsiųstas kanalu.</param>
		/// <param name="receivedText">Tektas, kuris buvo gautas iš kanalo.</param>
		private void PrepareResultsOfSentPlain(string originalText, string receivedText)
		{
			LabelSendWithoutCoding.Visibility = Visibility.Visible;

			TextBoxSendWithoutCoding.Text = receivedText;
			TextBoxSendWithoutCoding.Visibility = Visibility.Visible;

			var numberOfErrors1 = CalculateNumberOfDifferences(originalText, receivedText);
			_totalMismatchCountPlain += numberOfErrors1;
			_timesSentPlain++;

			LabelMismatchCountPlain.Content = $"Nesutapimų skaičius: {numberOfErrors1}";
			LabelMismatchCountPlain.Visibility = Visibility.Visible;

			LabelMismatchCountAveragePlain.Content = $"Vidurkis: {_totalMismatchCountPlain / _timesSentPlain}";
			LabelMismatchCountAveragePlain.Visibility = Visibility.Visible;
		}

		#endregion

		#region Koduoto.

		/// <summary>
		/// Išsiunčia užkoduotą tekstą kanalu.
		/// </summary>
		/// <param name="text">Tekstas, kurį norima siųsti kanalu.</param>
		/// <returns>Iš kanalo grįžęs rezultatas.</returns>
		private string SendEncodedText(string text)
		{
			var textAsVector = new List<byte>();

			// 1. Konvertuojame tekstą į dvejetainę simbolių eilutę.
			var textInBinary = ConvertTextToBinary(text);

			// 2. Daliname į tokio ilgio dalis, kad sutaptų su kodo dimensija.
			for (var c = 0; c < textInBinary.Length;)
			{
				var toEncodeAsString = string.Empty;
				for (var r = 0; r < _rows; r++)
				{
					// Jeigu pasibaigė tekstas, tačiau mums vis tiek reikia simbolių.
					if (c == textInBinary.Length)
					{
						// Pridedame nulį priekyje, kad nepasikeistų dvejetainio skaičiaus reikšmė.
						toEncodeAsString = '0' + toEncodeAsString;
					}
					else
					{
						toEncodeAsString += textInBinary[c];
						c++;
					}
				}

				// 3. Užkoduojame generuojančia matrica.
				var toEncodeAsList = ConvertStringToByteList(toEncodeAsString);
				var encoded = _matrixG.Encode(toEncodeAsList);

				// 4. Siunčiame kanalu.
				var deformed = _channel.SendVectorThrough(encoded);

				// 5. Atkoduojame 'Step-by-Step' algoritmu.
				var decoded = _matrixH.Decode(deformed);

				// 6. Atkoduojame generuojančia matrica.
				var fullyDecoded = _matrixG.Decode(decoded);

				// 7. Viską sudedame į vieną ilgą vektorių.
				foreach (var bit in fullyDecoded)
					textAsVector.Add(bit);
			}

			return ConvertBinaryVectorToText(textAsVector);
		}

		/// <summary>
		/// Parodo visą turimą informaciją bei statistiką apie užkoduoto teksto siuntimą kanalu.
		/// </summary>
		/// <param name="originalText">Tekstas, kuris buvo išsiųstas kanalu.</param>
		/// <param name="receivedText">Tektas, kuris buvo gautas iš kanalo.</param>
		private void PrepareResultsOfSentEncoded(string originalText, string receivedText)
		{
			LabelSendWithCoding.Visibility = Visibility.Visible;

			TextBoxSendWithCoding.Text = receivedText;
			TextBoxSendWithCoding.Visibility = Visibility.Visible;

			var numberOfErrors2 = CalculateNumberOfDifferences(originalText, receivedText);
			_totalMismatchCountEncoded += numberOfErrors2;
			_timesSendEncoded++;

			LabelMistmatchCountEncoded.Content = $"Nesutapimų skaičius: {numberOfErrors2}";
			LabelMistmatchCountEncoded.Visibility = Visibility.Visible;

			LabelMismatchCountAverageEncoded.Content = $"Vidurkis: {_totalMismatchCountEncoded / _timesSendEncoded}";
			LabelMismatchCountAverageEncoded.Visibility = Visibility.Visible;
		}

		#endregion

		/// <summary>
		/// Išvalome už statistiką atsakingus skaitiklius.
		/// </summary>
		private void NullStatisticCounters()
		{
			_timesSentPlain = 0;
			_timesSendEncoded = 0;
			_totalMismatchCountPlain = 0;
			_totalMismatchCountEncoded = 0;
		}

		#endregion

		#region Pagalbiniai metodai.

		/// <summary>
		/// Parodo klaidos pranešimą.
		/// </summary>
		private void ShowErrorMessage()
		{
			ErrorMessageLabel.Content = _errorMessage;
			_errorMessage = string.Empty;
		}

		/// <summary>
		/// Paslepia klaidos pranešimą.
		/// </summary>
		private void HideErrorMessage()
		{
			ErrorMessageLabel.Content = string.Empty;
		}

		/// <summary>
		/// Apskaičiuojama keliose pozicijose nesutampa tekstas.
		/// </summary>
		/// <param name="original">Tekstas, su kuriuo bus lyginama.</param>
		/// <param name="changed">Tekstas, kurio simboliai bus lyginami.</param>
		/// <returns>Jeigu 'original = "ab"' ir 'changed = "aa"' - grąžins 1.</returns>
		private int CalculateNumberOfDifferences(string original, string changed)
		{
			return original.Where((character, index) => index < changed.Length
			                                            && character != changed[index])
				.Count();
		}

		/// <summary>
		/// Konvertuoja dvejetainį vektorių atgal į tekstą pagal ASCII koduotę.
		/// </summary>
		/// <param name="vector">Dvejetainis vektorius.</param>
		/// <returns>Tekstą.</returns>
		private string ConvertBinaryVectorToText(List<byte> vector)
		{
			var textInBinary = ConvertByteListToString(vector);
			var decodedTextAsList = new List<byte>();

			for (var i = 0; i < textInBinary.Length;)
			{
				// Susiskaidome į vektorių su 8 bitais.
				var byteAsBinaryString = string.Empty;
				for (var c = 0; c < 8; c++)
				{
					// Jeigu turimas tekstas pasibaigė, tačiau pildomas vektorius
					//		nėra 8 simbolių ilgio - nedarome nieko.
					if (i == textInBinary.Length)
						c++;

					else
					{
						byteAsBinaryString += textInBinary[i];
						i++;
					}
				}

				// Konvertuojame iš dvejetainės į dešimtainę.
				var decimalNumber = Convert.ToByte(value: byteAsBinaryString, fromBase: 2);
				decodedTextAsList.Add(decimalNumber);
			}

			return Encoding.UTF8.GetString(decodedTextAsList.ToArray());
		}

		/// <summary>
		/// Paduotą tekstą konvertuoja į tekstą sudarytą iš dvejetainių skaičių pagal ASCII kodavimą.
		/// </summary>
		/// <param name="text">Tekstas, kurį norima konvertuoti.</param>
		/// <returns>Tekstą, sudarytą iš 0 ir 1.</returns>
		private string ConvertTextToBinary(string text)
		{
			// 'textAsBytes' reikšmės bus maždaug 89, 112, 201, 5, ...
			var textAsBytes = Encoding.UTF8.GetBytes(text);
			var stringBuilder = new StringBuilder();

			// Paverčiame į simbolių eilutę iš nulių ir vienetų.
			foreach (var word in textAsBytes)
			{
				// '@' simbolis naudojamas, kad kompiliatorius suprastų, jog 'byte' yra kintamasis.
				var @byte = Convert.ToString(value: word, toBase: 2)
								   .PadLeft(totalWidth: 8, paddingChar: '0');
				stringBuilder.Append(@byte);
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Simbolių eilutę iš 0 ir 1 paverčia į vektorių.
		/// </summary>
		/// <param name="vector">Simbolių eilutė, kurią norima paversti į vektorių.</param>
		/// <returns>Vektorių, sudarytą iš 0 ir 1.</returns>
		private static List<byte> ConvertStringToByteList(string vector)
		{
			var list = new List<byte>();

			foreach (var bit in vector)
				list.Add(bit == '0' ? (byte)0 : (byte)1);

			return list;
		}

		/// <summary>
		/// Vektorių iš 0 ir 1 paverčia į simbolių eilutę.
		/// </summary>
		/// <param name="vector">Vektorius, kurį norima paversti į simbolių eilutę.</param>
		/// <returns>Simbolių eilutę, sudarytą iš 0 ir 1.</returns>
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

		#endregion
	}
}
