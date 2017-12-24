using System;
using System.Collections.Generic;
using System.Text;
using Logic;

namespace ScenarioTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			#region Creation of matrices.
			var matrix = new List<List<byte>>(2)
			{
				new List<byte> {1, 0, 1, 1, 0}, 
				new List<byte> {0, 1, 0, 1, 1}
			};
			var matrixG = new MatrixG(length: matrix[0].Count, 
									  dimension: matrix.Count, 
									  matrix: matrix);

			matrix = new List<List<byte>>(3)
			{
				new List<byte> {1, 0, 1, 0, 0},
				new List<byte> {1, 1, 0, 1, 0},
				new List<byte> {0, 1, 0, 0, 1}
			};
			var matrixH = new MatrixH(matrix);
			#endregion

			var channel = new Channel(0.09);

			var text = "Some sample text.";
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
			var rows = 2;
			var textInBinary = stringBuilder.ToString();
			stringBuilder = stringBuilder.Clear();
			for (var c = 0; c < textInBinary.Length;)
			{
				var toEncodeAsString = string.Empty;
				for (var r = 0; r < rows; r++)
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
				var encoded = matrixG.Encode(toEncodeAsList);

				// 4. Send it through the channel.
				var deformed = channel.SendVectorThrough(encoded);

				// 5. Decode the vector.
				var decoded = matrixH.Decode(deformed);

				// 6. Get the original word.
				var fullyDecoded = matrixG.Decode(decoded);

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
			Console.WriteLine(revertedText);

			Console.ReadKey();
		}

		private static List<byte> ConvertStringToByteList(string vector)
		{
			var list = new List<byte>();
			foreach (var bit in vector)
			{
				list.Add(bit == '0' ? (byte) 0 : (byte) 1);
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
