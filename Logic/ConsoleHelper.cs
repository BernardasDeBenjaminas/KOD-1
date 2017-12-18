using System;

namespace Logic
{
	/// <summary>
	/// Klasė, kurios pagalba į konsolės langą spausdinamas spalvotas tekstas.
	/// </summary>
	public static class ConsoleHelper
	{
		/// <summary>
		/// Spausdina duotą tekstą į konsolės langą raudona spalva.
		/// </summary>
		/// <param name="message">Žinutė, kurią norima spausdinti.</param>
		public static void WriteError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine();
		}

		/// <summary>
		/// Spausdina duotą tekstą į konsolės langą balta spalva.
		/// </summary>
		/// <param name="message">Žinutė, kurią norima spausdinti.</param>
		public static void WriteInformation(string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		/// <summary>
		/// Spausdina duotą žinutę bei matricą į konsolė langą balta spalva.
		/// </summary>
		/// <param name="matrixName">Matricos pavadinimas.</param>
		/// <param name="matrix">Matrica, kurią norima spausdinti.</param>
		public static void WriteMatrix(string matrixName, MatrixG matrix)
		{
			var innerMatrix = matrix.Matrix;

			var message = $"{matrixName} = ";

			for (var c = 0; c <= innerMatrix.GetUpperBound(0); c++)
				if (innerMatrix[c] != null)
					message += string.Join(" ", innerMatrix[c]) + "\n    ";

			WriteInformation(message);
		}
		/// <summary>
		/// Spausdina duotą žinutę bei matricą į konsolė langą balta spalva.
		/// </summary>
		/// <param name="matrixName">Matricos pavadinimas.</param>
		/// <param name="matrix">Matrica, kurią norima spausdinti.</param>
		public static void WriteMatrix(string matrixName, MatrixH matrix)
		{
			var innerMatrix = matrix.Matrix;

			var message = $"{matrixName} = ";

			for (var c = 0; c <= innerMatrix.GetUpperBound(0); c++)
				if (innerMatrix[c] != null)
					message += string.Join(" ", innerMatrix[c]) + "\n    ";

			WriteInformation(message);
		}

		/// <summary>
		/// Spausdina duotą žinutę bei pateiktą vektorių į konsolės langą.
		/// Atsižvelgiant į 'errorVector' narius bus skirtingomis spalvomis spausdinami 'originalVector' nariai.
		/// Jeigu errorVector = 01, o originalVector = 11, tuomet bus atspausdinta žalias vienetas ir raudonas vienetas.
		/// </summary>
		/// <param name="introMessage">Žinutė, kurią norima spausdinti.</param>
		/// <param name="errorVector">Vektorius, kurio reikšmės lems kokia spalva bus spausdinami 'originalVector' nariai.</param>
		/// <param name="originalVector">Vektorius, kurio narius norima spausdinti spalvotai.</param>
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
