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

		public static void WriteWarning(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static void WriteMatrix(int[][] matrix)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine();

			var rows = matrix.GetUpperBound(0) + 1;
			var cols = matrix[0].GetUpperBound(0) + 1;
			for (var r = 0; r < rows; r++)
			{
				for (var c = 0; c < cols; c++)
				{
					Console.Write(matrix[r][c] + "  ");
				}
				Console.WriteLine();
				Console.WriteLine();
			}
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static void WriteMatrix(string matrixName, MatrixG matrix)
		{
			var innerMatrix = matrix.Matrix;

			var message = $"{matrixName} = ";

			for (var c = 0; c <= innerMatrix.GetUpperBound(0); c++)
				if (innerMatrix[c] != null)
					message += string.Join(" ", innerMatrix[c]) + "\n    ";

			WriteInformation(message);
		}

		public static void WriteMatrix(string matrixName, MatrixH matrix)
		{
			var innerMatrix = matrix.Matrix;

			var message = $"{matrixName} = ";

			for (var c = 0; c <= innerMatrix.GetUpperBound(0); c++)
				if (innerMatrix[c] != null)
					message += string.Join(" ", innerMatrix[c]) + "\n    ";

			WriteInformation(message);
		}

		public static void WriteChanges(string introMessage, int[] errorVector, int[] originalVector)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(introMessage);

			var length = errorVector.GetUpperBound(0) + 1;
			for (var c = 0; c < length; c++)
			{
				Console.ForegroundColor = errorVector[c] == 0 
					? ConsoleColor.Green 
					: ConsoleColor.Red;
				Console.Write(" " + originalVector[c]);
			}
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine();
		}
	}
}
