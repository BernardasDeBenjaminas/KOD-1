using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Logic;
using Image = System.Windows.Controls.Image;

namespace Scenario3
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int _rows;
		private int _cols;
		private MatrixG _matrixG;
		private MatrixH _matrixH;
		private Channel _channel;

		public MainWindow()
		{
			InitializeComponent();

			#region Creation of matrices and channel.

			_channel = new Channel(0.05);

			_rows = 2;
			_cols = 10;

			var matrix = new List<List<byte>>(2)
			{
				new List<byte> {1, 0, 1, 1, 0},
				new List<byte> {0, 1, 0, 1, 1}
			};
			_matrixG = new MatrixG(length: matrix[0].Count,
				dimension: matrix.Count,
				matrix: matrix);

			matrix = new List<List<byte>>(3)
			{
				new List<byte> {1, 0, 1, 0, 0},
				new List<byte> {1, 1, 0, 1, 0},
				new List<byte> {0, 1, 0, 0, 1}
			};
			_matrixH = new MatrixH(matrix);
			#endregion


			var pathToOpen = "C:\\test(smaller).bmp";
			var pathToSaveEncoded = "C:\\Users\\Benas\\Desktop\\encoded.bmp";
			var pathToSavePlain = "C:\\Users\\Benas\\Desktop\\unencoded.bmp";
			ImageToSend.Source = new BitmapImage(new Uri(pathToOpen));

			// Encode and send image.
			SendImageAsEncoded(source: pathToOpen, destination: pathToSaveEncoded);

			// Don't encode and send image
			SendImageAsPlain(source: pathToOpen, destination: pathToSavePlain);

			// 6. Open the image.
			ImageEncoded.Source = new BitmapImage(new Uri(pathToSaveEncoded));
			ImagePlain.Source = new BitmapImage(new Uri(pathToSavePlain));
		}

		private void SendImageAsPlain(string source, string destination)
		{
			// 1. Open the file - contains stuff like: 58, 49, 112, ...
			var imageAsVector = File.ReadAllBytes(source);

			// 2. Convert the decimal vector to binary text.
			var imageAsBinaryString = ConvertDecimalVectorToBinaryString(imageAsVector);

			// 3. Convert the binary text to binary vector.
			var imageAsBinaryVector = ConvertBinaryStringToBinaryVector(imageAsBinaryString);

			// 4. The header of the image cannot be corrupted so we separate.
			IList<byte> vectorToSend = new List<byte>();
			for (var i = 1104; i < imageAsBinaryVector.Count; i++)
				vectorToSend.Add(imageAsBinaryVector[i]);

			var deformed = _channel.SendVectorThrough(vectorToSend);

			vectorToSend.Clear();
			for (var i = 0; i < 1104; i++)
				vectorToSend.Add(imageAsBinaryVector[i]);

			for (var i = 0; i < deformed.Count; i++)
				vectorToSend.Add(deformed[i]);

			var result = ConvertBinaryVectorToDecimalVector(vectorToSend);

			var _imageConverter = new ImageConverter();
			Bitmap bm = (Bitmap)_imageConverter.ConvertFrom(result.ToArray());
			bm.Save(destination);
		}

		private IList<byte> ConvertBinaryStringToBinaryVector(string binaryString)
		{
			var binaryVector = new List<byte>();

			foreach (var bit in binaryString)
			{
				if (bit == '0')
					binaryVector.Add(0);
				else if (bit == '1')
					binaryVector.Add(1);
				else
				{
					throw new ArgumentException("Tekstas privalo būti sudarytas tik iš 0 ir 1.");
				}
			}

			return binaryVector;
		}

		private void SendImageAsEncoded(string source, string destination)
		{
			// 1. Open the file - contains stuff like: 58, 49, 112, ...
			var imageAsVector = File.ReadAllBytes(source);

			// 2. Convert the vector to binary text.
			var imageAsBinaryString = ConvertDecimalVectorToBinaryString(imageAsVector);

			// 3. Send the text through the channel.
			var deformedImageAsBinaryString = SendBinaryStringThroughChannel(imageAsBinaryString);

			// 4. Convert binary text back to a vector.
			var deformedImageAsVector = ConvertBinaryStringToVector(deformedImageAsBinaryString);

			// 5. Save the vector as file.
			var _imageConverter = new ImageConverter();

			Bitmap bm = (Bitmap)_imageConverter.ConvertFrom(deformedImageAsVector.ToArray());

			if (bm != null && (bm.HorizontalResolution != (int)bm.HorizontalResolution ||
			                   bm.VerticalResolution != (int)bm.VerticalResolution))
			{
				// Correct a strange glitch that has been observed in the test program when converting 
				//  from a PNG file image created by CopyImageToByteArray() - the dpi value "drifts" 
				//  slightly away from the nominal integer value
				bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
					(int)(bm.VerticalResolution + 0.5f));
			}

			bm.Save(destination);
		}

		private IList<byte> ConvertBinaryVectorToDecimalVector(IList<byte> binaryVector)
		{
			var decimalVector = new List<byte>();

			for (var i = 0; i < binaryVector.Count;)
			{
				var binaryNumber = "";
				for (var c = 0; c < 8; c++)
				{
					if (i == binaryVector.Count)
					{
						binaryNumber = '0' + binaryNumber;
					}
					else
					{
						binaryNumber += binaryVector[i];
						i++;
					}
				}
				decimalVector.Add(Convert.ToByte(value: binaryNumber, fromBase: 2));
			}

			return decimalVector;
		}

		private IList<byte> ConvertBinaryStringToVector(string binaryText)
		{
			var vector = new List<byte>();

			for (var i = 0; i < binaryText.Length; i++)
			{
				var binaryNumber = "";
				for (var c = 0; c < 8; c++)
				{
					if (i != binaryText.Length)
					{
						binaryNumber += binaryText[i];
						i++;
					}
					else
					{
						binaryNumber = '0' + binaryNumber;
					}
				}
				var decimalNumber = Convert.ToByte(binaryNumber, 2);
				vector.Add(decimalNumber);

				// Kad ciklas neperšoktų vis po vieną.
				i--;
			}

			return vector;
		}

		private string ConvertDecimalVectorToBinaryString(IList<byte> vector)
		{
			var text = new StringBuilder();

			foreach (var number in vector)
			{
				var binaryNumber = Convert.ToString(value: number, toBase: 2).PadLeft(totalWidth: 8, paddingChar: '0');
				text.Append(binaryNumber);
			}

			return text.ToString();
		}

		private string SendBinaryStringThroughChannel(string binaryString)
		{
			var stringBuilder = new StringBuilder();

			// Nepradedame nuo 0, nes reikia išsaugoti kai kurią kontrolinę nuotraukos informaciją.
			for (var c = 0; c < binaryString.Length;)
			{
				// 2. Daliname į tokio ilgio dalis, kad sutaptų su kodo dimensija.
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

				// 3. Užkoduojame generuojančia matrica.
				var toEncodeAsList = ConvertStringToVector(toEncodeAsString);
				var encoded = _matrixG.Encode(toEncodeAsList);

				IList<byte> deformed;
				// Todo: explain magical number.
				// Todo: https://en.wikipedia.org/wiki/BMP_file_format
				deformed = c >= 1104 
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

		private List<byte> ConvertStringToVector(string vector)
		{
			var realVector = new List<byte>();

			foreach (var number in vector)
			{
				realVector.Add((byte)int.Parse(number + ""));
			}

			return realVector;
		}

		//// Todo: change name to better reflect function.
		//// Todo: the same in 'Scenario2'.
		//private string SendEncodedText(string text)
		//{
		//	var textAsVector = new List<byte>();

		//	// 1. Konvertuojame tekstą į dvejetainę simbolių eilutę.
		//	var textInBinary = ConvertTextToBinary(text);

		//	// 2. Daliname į tokio ilgio dalis, kad sutaptų su kodo dimensija.
		//	for (var c = 0; c < textInBinary.Length;)
		//	{
		//		var toEncodeAsString = string.Empty;
		//		for (var r = 0; r < _rows; r++)
		//		{
		//			// Jeigu pasibaigė tekstas, tačiau mums vis tiek reikia simbolių.
		//			if (c == textInBinary.Length)
		//			{
		//				// Pridedame nulį priekyje, kad nepasikeistų dvejetainio skaičiaus reikšmė.
		//				toEncodeAsString = '0' + toEncodeAsString;
		//			}
		//			else
		//			{
		//				toEncodeAsString += textInBinary[c];
		//				c++;
		//			}
		//		}

		//		// 3. Užkoduojame generuojančia matrica.
		//		var toEncodeAsList = ConvertStringToByteList(toEncodeAsString);
		//		var encoded = _matrixG.Encode(toEncodeAsList);

		//		// 4. Siunčiame kanalu.
		//		var deformed = _channel.SendVectorThrough(encoded);

		//		// 5. Atkoduojame 'Step-by-Step' algoritmu.
		//		var decoded = _matrixH.Decode(deformed);

		//		// 6. Atkoduojame generuojančia matrica.
		//		var fullyDecoded = _matrixG.Decode(decoded);

		//		// 7. Viską sudedame į vieną ilgą vektorių.
		//		foreach (var bit in fullyDecoded)
		//			textAsVector.Add(bit);
		//	}

		//	return ConvertBinaryVectorToText(textAsVector);
		//}

		//private string ConvertTextToBinary(string text)
		//{
		//	// 'textAsBytes' reikšmės bus maždaug 89, 112, 201, 5, ...
		//	// Todo: leave it at ASCII?
		//	var textAsBytes = Encoding.ASCII.GetBytes(text);
		//	var stringBuilder = new StringBuilder();

		//	// Paverčiame į simbolių eilutę iš nulių ir vienetų.
		//	foreach (var word in textAsBytes)
		//	{
		//		// '@' simbolis naudojamas, kad kompiliatorius suprastų, jog 'byte' yra kintamasis.
		//		var @byte = Convert.ToString(value: word, toBase: 2)
		//						   .PadLeft(totalWidth: 8, paddingChar: '0');
		//		stringBuilder.Append(@byte);
		//	}

		//	return stringBuilder.ToString();
		//}

		//// Todo: change elsewhere to IList.
		//private string ConvertBinaryVectorToText(IList<byte> vector)
		//{
		//	var textInBinary = ConvertByteListToString(vector);
		//	var decodedTextAsList = new List<byte>();

		//	for (var i = 0; i < textInBinary.Length;)
		//	{
		//		// Susiskaidome į vektorių su 8 bitais.
		//		var byteAsBinaryString = string.Empty;
		//		for (var c = 0; c < 8; c++)
		//		{
		//			// Jeigu turimas tekstas pasibaigė, tačiau pildomas vektorius
		//			//		nėra 8 simbolių ilgio - nedarome nieko.
		//			if (i == textInBinary.Length)
		//				c++;

		//			else
		//			{
		//				byteAsBinaryString += textInBinary[i];
		//				i++;
		//			}
		//		}

		//		// Konvertuojame iš dvejetainės į dešimtainę.
		//		var decimalNumber = Convert.ToByte(value: byteAsBinaryString, fromBase: 2);
		//		decodedTextAsList.Add(decimalNumber);
		//	}

		//	return Encoding.ASCII.GetString(decodedTextAsList.ToArray());
		//}

		//private static string ConvertByteListToString(IList<byte> vector)
		//{
		//	var result = string.Empty;
		//	foreach (var bit in vector)
		//	{
		//		if (bit == 0)
		//			result += '0';
		//		else
		//			result += '1';
		//	}
		//	return result;
		//}

		//// Todo: change argument name.
		//// Todo: change return type?
		//// Todo: the same in 'Scenario2'.
		//private static IList<byte> ConvertStringToByteList(string vector)
		//{
		//	var list = new List<byte>();

		//	foreach (var bit in vector)
		//		list.Add(bit == '0' ? (byte)0 : (byte)1);

		//	return list;
		//}
	}
}
