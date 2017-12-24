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
				[0] = new List<byte> {1, 0, 1, 1, 0},
				[1] = new List<byte> {0, 1, 0, 1, 1}
			};
			var matrixG = new MatrixG(length: matrix[0].Count, 
									  dimension: matrix.Count, 
									  matrix: matrix);

			matrix = new List<List<byte>>(3)
			{
				[0] = new List<byte> {1, 0, 1, 0, 0},
				[1] = new List<byte> {1, 1, 0, 1, 0},
				[2] = new List<byte> {0, 1, 0, 0, 1}
			};
			var matrixH = new MatrixH(matrix);
			#endregion

			var text = "Some sample text.";
			var textAsArray = Encoding.ASCII.GetBytes(text);

			var wordLength = 2;
			for (var a = 0; a < textAsArray.Length; a += 0)
			{
				var toEncode = new List<byte>(wordLength);

				for (var i = 0; i < wordLength; i++)
				{
					toEncode.Add(textAsArray[a]);
					a++;
				}

				var encoded = matrixG.Encode(toEncode);
			}

			var revertedText = Encoding.ASCII.GetString(textAsArray);

			Console.WriteLine(revertedText);
			Console.ReadKey();
		}

		private static List<byte> CombineVectors(List<byte> vector1, List<byte> vector2)
		{
			var combined = new List<byte>(vector1.Count + vector2.Count);

			foreach (var element in vector1)
				combined.Add(element);

			foreach (var element in vector2)
				combined.Add(element);

			return combined;
		}
	}
}
