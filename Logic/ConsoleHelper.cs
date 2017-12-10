using System;

namespace Logic
{
	public static class ConsoleHelper
	{
		public static void WriteError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine();
		}

		public static void WriteCheck(string original, int[] input, string result, int[] output)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"{original} must be {result}.");

			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach (var number in input) Console.Write(number);
			Console.Write(" outputs ");
			foreach (var number in output) Console.Write(number);

			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static void WriteInformation(string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static void WriteMatrix(int[][] matrix)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			var cols = matrix.GetUpperBound(0);
			var rows = matrix[0].GetUpperBound(0);
			for (var r = 0; r <= rows; r++)
			{
				for (var c = 0; c <= cols; c++)
				{
					Console.Write(matrix[c][r]);
				}
				Console.WriteLine();
			}
		}
	}
}
