using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Logic;
using Microsoft.Win32;

namespace Scenario3
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int _rows = -1; // '_matrixG' dimensija (k).
		private int _cols = -1; // '_matrixG' ilgis (n).
		private MatrixG _matrixG; // G matrica (vartotojo įvesta arba kompiuterio sugeneruota).
		private MatrixH _matrixH; // H matrica (gauta iš '_matrixG').

		private bool _entersOwnMatrix;        // Ar vartotojas pats įves generuojančią matricą?
		private List<List<byte>> _tempMatrix; // Laikina matrica, kurią naudosiu vartotojui pačiam suvedinėjant G matricą.

		private string _pathToSaveEncodedImage;	// Kelias iki paveikslėlio, gauto siunčiant koduotą paveikslėlį kanalu.
		private string _pathToSavePlainImage;	// Kelias iki paveikslėlio, gauto siunčiant nekoduotą paveikslėlį kanalu.
		private string _pathToOpenImage;		// Kelias iki paveikslėlio, kurį siųsime kanalu.
		private Channel _channel;				// Kanalas, kuriuo siųsiu vektorius.
		private double _errorChance = -1;		// Tikimybė kanale įvykti klaidai (p). -1, nes 0 yra leidžiama reikšmė.

		private string _errorMessage = string.Empty;			 // Naudojamas klaidos žinutėms atvaizduoti.
		private readonly Validator _validator = new Validator(); // Naudojamas vartotojo įvesties validavimui.

		private bool _areImageFieldsShow; // Naudojamas parodyti visas tris nuotraukas.


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
			}
			catch (Exception ex)
			{
				_errorChance = -1;
				_errorMessage = ex.Message;
			}

			ShowErrorMessage();
		}

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas įveda bet kokį simbolį į matricos ilgio langelį.
		/// </summary>
		private void InputCols_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			try
			{
				_cols = _validator.ValidateNumberOfCols(input);
			}
			catch (Exception ex)
			{
				_cols = -1;
				_errorMessage = ex.Message;
			}

			ShowErrorMessage();
		}

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas įveda bet kokį simbolį į kodo dimensijos langelį.
		/// </summary>
		private void InputRows_TextChanged(object sender, TextChangedEventArgs e)
		{
			var input = ((TextBox)sender).Text;

			try
			{
				_rows = _validator.ValidateNumberOfRows(input);
			}
			catch (Exception ex)
			{
				_rows = -1;
				_errorMessage = ex.Message;
			}

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
				LetUserEnterGMatrix();
			}
			else
			{
				_matrixG = new MatrixG(_cols, _rows);
				_matrixH = _matrixG.GetMatrixH();
				// Paslepiame nebeaktualius langelius ir parodome aktualius.
				HideInputFieldsAndShowChooseImage();
			}
		}

		#region Jeigu vartotojas pats nori įvesti matricą.

		/// <summary>
		/// Parodo laukus, atsakingus už norimos matricos įvedimą.
		/// </summary>
		private void LetUserEnterGMatrix()
		{
			_tempMatrix = new List<List<byte>>(_rows);

			InputMatrixRow.Visibility = Visibility.Visible;
			LabelInputMatrixRow.Visibility = Visibility.Visible;
			LabelAdviceInputMatrixRow.Visibility = Visibility.Visible;

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
						HideInputFieldsAndShowChooseImage();
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

		#region Pasiruošimas nuotraukos siuntimui kanalu.

		/// <summary>
		///  Paslepia matricos kintamųjų surinkimo langelius ir parodo teksto siuntimui kanalu reikalingus langelius.
		/// </summary>
		private void HideInputFieldsAndShowChooseImage()
		{
			HideErrorMessage();

			// Paslepiame kodo ilgį.
			InputCols.Visibility = Visibility.Hidden;
			LabelCols.Visibility = Visibility.Hidden;

			// Paslepiame kodo dimensiją.
			InputRows.Visibility = Visibility.Hidden;
			LabelRows.Visibility = Visibility.Hidden;

			// Paslepiame matricos vektoriaus įvedimo laukus.
			LabelAdviceInputMatrixRow.Visibility = Visibility.Hidden;
			LabelInputMatrixRow.Visibility = Visibility.Hidden;
			InputMatrixRow.Visibility = Visibility.Hidden;

			// Paslepiame kas liko.
			CheckBoxOwnMatrix.Visibility = Visibility.Hidden;     // Varnelė savo matricos pateikimui.
			ButtonSubmitVariables.Visibility = Visibility.Hidden; // Mygtukas visko pateikimui.

			ButtonChooseImageToSend.Visibility = Visibility.Visible; // Mygtukas paveikslėlio pasirinkimui.

			// Patarimai.
			LabelAdvice1.Visibility = Visibility.Visible;
			LabelAdvice2.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Parodo paveikslėlių langelius ir jų etiketes.
		/// </summary>
		private void ShowImageFields()
		{
			LabelImageToSend.Visibility = Visibility.Visible;
			ImageToSend.Visibility = Visibility.Visible;

			LabelImagePlain.Visibility = Visibility.Visible;
			ImagePlain.Visibility = Visibility.Visible;

			LabelImageEncoded.Visibility = Visibility.Visible;
			ImageEncoded.Visibility = Visibility.Visible;
		}

		#endregion

		#region Nuotraukos siuntimas kanalu.

		/// <summary>
		/// Funkcija iškviečiama, kuomet vartotojas paspausdžia mygtuką pasirinkt nuotrauką, kurią nori siųsti kanalu.
		/// </summary>
		private void ButtonChooseImageToSend_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog window = new OpenFileDialog();

			if (window.ShowDialog() == true)
				_pathToOpenImage = window.FileName;

			try
			{
				// Ar nuotrauką galime atidaryti?
				var test = new BitmapImage(new Uri(_pathToOpenImage));
				if (_areImageFieldsShow)
				{
					ShowImageFields();
					_areImageFieldsShow = true;
				}
				_pathToSaveEncodedImage = GenerateImagePaths("encoded");
				_pathToSavePlainImage = GenerateImagePaths("plain");
				SendImagesThroughTheChannel();
			}
			catch (Exception ex)
			{
				_pathToSaveEncodedImage = string.Empty;
				_pathToSavePlainImage = string.Empty;
				_pathToOpenImage = string.Empty;
				_errorMessage = ex.Message;
				ShowErrorMessage();
			}
		}

		/// <summary>
		/// Parengia vartotojo pasirinktą nuotrauką siuntimui koduotu ir nekoduotu būdu.
		/// </summary>
		private void SendImagesThroughTheChannel()
		{
			ButtonChooseImageToSend.IsEnabled = false;

			// Nuotrauka, siųsta be kodavimo.
			ImagePlain.Source = null;
			ImagePlain.Visibility = Visibility.Hidden;
			LabelImagePlain.Visibility = Visibility.Hidden;
			ImagePlain.Source = SendImageAsPlain(pathToOpenImage: _pathToOpenImage, pathWhereToSaveResult: _pathToSavePlainImage);
			LabelImagePlain.Visibility = Visibility.Visible;
			ImagePlain.Visibility = Visibility.Visible;

			// Nuotrauka, siųsta su kodavimu.
			ImageEncoded.Source = null;
			ImageEncoded.Visibility = Visibility.Hidden;
			LabelImageEncoded.Visibility = Visibility.Hidden;
			ImageEncoded.Source = SendImageAsEncoded(pathToOpenImage: _pathToOpenImage, pathWhereToSaveResult: _pathToSaveEncodedImage);
			LabelImageEncoded.Visibility = Visibility.Visible;
			ImageEncoded.Visibility = Visibility.Visible;

			// Nuotrauka siuntimui.
			ImageToSend.Source = null;
			ImageToSend.Visibility = Visibility.Hidden;
			LabelImageToSend.Visibility = Visibility.Hidden;
			ImageToSend.Source = new BitmapImage(new Uri(_pathToOpenImage));
			LabelImageToSend.Visibility = Visibility.Visible;
			ImageToSend.Visibility = Visibility.Visible;

			ButtonChooseImageToSend.IsEnabled = true;
		}


		/// <summary>
		/// Išsiunčia neužkoduotą paveikslėlį nesaugiu kanalu.
		/// </summary>
		/// <param name="pathToOpenImage">Kelias iki paveikslėlio, kurį reikės siųsti kanalu.</param>
		/// <param name="pathWhereToSaveResult">Kelias, kur galima išsaugoti iš kanalo gautą paveikslėlį.</param>
		/// <returns>Adresą į kanale iškraipytą neužkoduotą paveikslėlį.</returns>
		private BitmapImage SendImageAsPlain(string pathToOpenImage, string pathWhereToSaveResult)
		{
			// 1. Atsidarome paveikslėlį ir gauname sąrašą dešimtainių skaitmenų: 58, 49, 112, ...
			var imageAsDecimalVector = File.ReadAllBytes(pathToOpenImage);

			// 2. Konvertuojame sąrašą į simbolių eilutę iš 0 ir 1.
			var imageAsBinaryString = Converter.DecimalVectorToBinaryString(imageAsDecimalVector);

			// 3. Konvertuojame simbolių eilutę į dvejetainį sąrašą.
			var imageAsBinaryVector = Converter.BinaryStringToBinaryVector(imageAsBinaryString);

			// 4. Vektoriaus pradžioje yra laikoma kontrolinė informacija, tad jos negalime siųsti kanalu.
			//		1104 = (14 baitų (bitmap header) + 124 baitai (DIB header)) * 8 bitai.
			IList<byte> vectorToSend = new List<byte>();
			for (var i = 1104; i < imageAsBinaryVector.Count; i++)
				vectorToSend.Add(imageAsBinaryVector[i]);

			// 5. Siunčiame vektorių iškraipymui
			var deformedBinaryVector = _channel.SendVectorThrough(vectorToSend);

			// 6.Sujungiame atgal į vieną kontrolinę informaciją ir iškraipytąjį vektorių.
			vectorToSend.Clear();
			for (var i = 0; i < 1104; i++)
				vectorToSend.Add(imageAsBinaryVector[i]);

			foreach (byte bit in deformedBinaryVector)
				vectorToSend.Add(bit);

			// 7. Paruošiame naująjį vektoriui konvertavimui į paveikslėlį.
			var resultAsDecimalVector = Converter.BinaryVectorToDecimalVector(vectorToSend);

			// 8. Konvertuojame į paveikslėlį.
			var imageConverter = new ImageConverter();
			Bitmap bitmap = (Bitmap)imageConverter.ConvertFrom(resultAsDecimalVector.ToArray());
			bitmap.Save(pathWhereToSaveResult);

			return new BitmapImage(new Uri(pathWhereToSaveResult));
		}


		/// <summary>
		/// Išsiunčia užkoduotą paveikslėlį nesaugiu kanalu.
		/// </summary>
		/// <param name="pathToOpenImage">Kelias iki paveikslėlio, kurį reikės siųsti kanalu.</param>
		/// <param name="pathWhereToSaveResult">Kelias, kur galima išsaugoti iš kanalo gautą paveikslėlį.</param>
		/// <returns>Adresą į kanale iškraipytą užkoduotą paveikslėlį.</returns>
		private BitmapImage SendImageAsEncoded(string pathToOpenImage, string pathWhereToSaveResult)
		{
			// 1. Atsidarome paveikslėlį ir gauname sąrašą dešimtainių skaitmenų: 58, 49, 112, ...
			var imageAsDecimalVector = File.ReadAllBytes(pathToOpenImage);

			// 2. Konvertuojame sąrašą į simbolių eilutę iš 0 ir 1.
			var imageAsBinaryString = Converter.DecimalVectorToBinaryString(imageAsDecimalVector);

			// 3. Dvejetainę simbolių eilutę daliname dalimis po aštuonis, dalį kovertuojame į dvejetainį vektorių, 
			//		jį iškraipome, rezultatą paverčiame atgal į dvejetainę simbolių eilutę ir grąžiname čia.
			var deformedImageAsBinaryString = SendBinaryStringThroughChannel(imageAsBinaryString);

			// 4. Konvertuojame dvejetainį tekstą atgal į dešimtainį vektorių.
			var deformedImageAsDecimalVector = Converter.BinaryStringToDecimalVector(deformedImageAsBinaryString);

			// 5. Paverčiame atgal į paveikslėlį ir išsaugome.
			var imageConverter = new ImageConverter();
			Bitmap bitmap = (Bitmap)imageConverter.ConvertFrom(deformedImageAsDecimalVector.ToArray());
			bitmap.Save(pathWhereToSaveResult);

			return new BitmapImage(new Uri(pathWhereToSaveResult));
		}

		/// <summary>
		/// Iškraipo paduotą simbolių eilutę (0 gali tapti 1 ir atvirkščiai).
		/// </summary>
		/// <param name="binaryString">Simbolių eilutė, sudaryta iš 0 ir 1.</param>
		/// <returns>Iškraipytą simbolių eilutę.</returns>
		private string SendBinaryStringThroughChannel(string binaryString)
		{
			var stringBuilder = new StringBuilder();

			for (var c = 0; c < binaryString.Length;)
			{
				// 1. Daliname į tokio ilgio dalis, kad sutaptų su kodo dimensija.
				var toEncodeAsString = string.Empty;
				for (var r = 0; r < _rows; r++)
				{
					// Jeigu pasibaigė tekstas, tačiau mums vis tiek reikia simbolių.
					if (c == binaryString.Length)
					{
						// Pridedame nulį priekyje, kad nepasikeistų dvejetainio skaičiaus reikšmė.
						toEncodeAsString = '0' + toEncodeAsString;
					}
					else
					{
						toEncodeAsString += binaryString[c];
						c++;
					}
				}

				// 2. Užkoduojame generuojančia matrica.
				var toEncodeAsList = Converter.BinaryStringToBinaryVector(toEncodeAsString);
				var encoded = _matrixG.Encode(toEncodeAsList);

				// 1104 = (14 baitų (bitmap header) + 124 baitai (DIB header)) * 8 bitai.
				// Jeigu tai kontrolinė informacija tuomet jos nesiunčiame kanalu.
				var deformed = c >= 1104
					? _channel.SendVectorThrough(encoded)
					: encoded;

				// 5. Atkoduojame 'Step-by-Step' algoritmu.
				var decoded = _matrixH.Decode(deformed);

				// 6. Atkoduojame generuojančia matrica.
				var fullyDecoded = _matrixG.Decode(decoded);

				// 7. Viską sudedame į vieną ilgą vektorių.
				foreach (var bit in fullyDecoded)
					stringBuilder.Append(bit);
			}

			return stringBuilder.ToString();
		}

		#endregion

		#region Pagalbiniai metodai.

		/// <summary>
		/// Parodo klaidos pranešimą.
		/// </summary>
		private void ShowErrorMessage()
		{
			LabelErrorMessage.Content = _errorMessage;
			_errorMessage = string.Empty;
		}

		/// <summary>
		/// Paslepia klaidos pranešimą.
		/// </summary>
		private void HideErrorMessage()
		{
			LabelErrorMessage.Content = string.Empty;
		}

		/// <summary>
		/// Sugeneruoja pavadinimus paveikslėliams, kuriuos išsaugosime.
		/// </summary>
		/// <param name="forWhichImage">Žodis, kuris bus pavadinime, kad būtų galima atskirti tarp encoded ir plain.</param>
		/// <returns>Kelias iki paveikslėlio.</returns>
		private static string GenerateImagePaths(string forWhichImage)
		{
			var directoryPath = Directory.GetCurrentDirectory();
			var allFilesInDirectory = Directory.GetFiles(directoryPath);

			for (var i = 0; i < 100000; i++)
			{
				var encodedName = Path.Combine(directoryPath, $"{forWhichImage}_{i}.bmp");

				// Patikriname ar toks failas jau egzistuoja.
				var match = allFilesInDirectory.FirstOrDefault(f => f == encodedName);

				// Nėra tokio failo.
				if (match == default(string))
				{
					return encodedName;
				}
			}

			// Todo: nenori pakeisti šito?
			throw new Exception("Ups.");
		}

		#endregion
	}
}