using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
	/// <summary>
	/// Klasė, pagrinde naudojama konvertavimui tarp 'string' (simbolių eilutė) ir 'IList[byte]' (sąrašas) tipo kintamųjų.
	/// </summary>
	public static class Converter
	{
		/// <summary>
		/// Paverčia dešimtainių skaičių sąrašą į dvejetainių simbolių eilutę.
		/// </summary>
		/// <param name="vector">Sąrašas iš dešimtainių skaičių.</param>
		/// <returns></returns>
		public static string DecimalVectorToBinaryString(IList<byte> vector)
		{
			var text = new StringBuilder();
			foreach (var number in vector)
			{
				var binaryNumber = Convert.ToString(value: number, toBase: 2).PadLeft(totalWidth: 8, paddingChar: '0');
				text.Append(binaryNumber);
			}
			return text.ToString();
		}

		/// <summary>
		/// Paverčia dvejetainę simbolių eilutę į sąrašą iš dvejetainių skaičių.
		/// </summary>
		/// <param name="binaryString">Bet kokio ilgio simbolių eilutė iš 0 ir 1.</param>
		/// <returns>Sąrašas, sudarytas iš 0 ir 1.</returns>
		public static IList<byte> BinaryStringToBinaryVector(string binaryString)
		{
			var binaryVector = new List<byte>();

			foreach (var bit in binaryString)
			{
				if (bit == '0')
					binaryVector.Add(0);

				else if (bit == '1')
					binaryVector.Add(1);

				else
					throw new ArgumentException("Tekstas privalo būti sudarytas tik iš 0 ir 1.");
			}

			return binaryVector;
		}

		/// <summary>
		/// Paverčia sąrašą iš 0 ir 1 į sąrašą iš dešimtainių skaičių.
		/// </summary>
		/// <param name="binaryVector">Sąrašas sudarytas iš 0 ir 1.</param>
		/// <returns>Sąrašas, sudarytas iš dešimtainių skaičių.</returns>
		public static IList<byte> BinaryVectorToDecimalVector(IList<byte> binaryVector)
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

		/// <summary>
		/// Paverčia simbolių eilutę iš 0 ir 1 į sąrašą dešimtainių skaičių.
		/// </summary>
		/// <param name="binaryText">Simbolių eilutė, sudaryta iš 0 ir 1.</param>
		/// <returns></returns>
		public static IList<byte> BinaryStringToDecimalVector(string binaryText)
		{
			var vector = new List<byte>();

			for (var i = 0; i < binaryText.Length;)
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
			}

			return vector;
		}
	}
}
