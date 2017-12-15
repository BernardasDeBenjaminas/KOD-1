using System;
using System.Text.RegularExpressions;
using Logic;

namespace Scenario1
{
	public class Presenter
	{
		private int _rows;
		private int _cols;
		private int[][] _tempMatrix;
		private MatrixG _matrixG;
		private string _errorMessage = string.Empty;

		// CONSTRUCTOR

		public Presenter() { }


		// PUBLIC
		public void Start()
		{
			_cols = GetNumberOfCols();
			_rows = GetNumberOfRows();
			_matrixG = GetMatrix();
			DisplayCurrentInformation();
		}


		// PRIVATE
		// Todo: add comments and summary in Lithuanian.
		private int GetNumberOfCols()
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.Write("Įveskite kodo ilgį (n): ");
				var input = Console.ReadLine();

				if (int.TryParse(input, out var cols))
				{
					if (cols < 1)
						_errorMessage = "Reikšmė privalo būti teigiamas skaičius!";

					else
					{
						_errorMessage = string.Empty;
						return cols;
					}
				}
				else
					_errorMessage = "Galimos reikšmės tik sveikieji skaičiai!";

				Console.Clear();
			}
		}

		private int GetNumberOfRows()
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.Write("Įveskite dimensiją (k): ");
				var input = Console.ReadLine();

				if (int.TryParse(input, out var rows))
				{
					if (rows < 1)
						_errorMessage = "Reikšmė privalo būti teigiamas skaičius!";

					else if (rows > _cols)
						_errorMessage = "Dimensija negali būti mažesnė už kodo ilgį!";

					else
					{
						_errorMessage = string.Empty;
						return rows;
					}
				}

				else
					_errorMessage = "Galimos reikšmės tik sveikieji skaičiai!";

				Console.Clear();
			}
		}

		private MatrixG GetMatrix()
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.Write("Ar norite įvesti generuojančią matricą patys (jeigu ne - ji bus sugeneruota už jus)? (y/n): ");
				var input = Console.ReadLine();

				if (input?.ToLower() != "y" && input?.ToLower() != "n")
					_errorMessage = "Įveskite 'y', jeigu norite patys įvesti matricą, antraip 'n'.";

				else
				{
					_errorMessage = string.Empty;
					
					return input.ToLower() == "y" 
						? GetUserInputedMatrix()     // The user will input a personal matrix.
						: new MatrixG(_cols, _rows); // The matrix will be generated randomly.
				}
			}
		}

		private MatrixG GetUserInputedMatrix()
		{
			_tempMatrix = new int[_rows][];	

			for (var r = 0; r < _rows; r++)
			{
				DisplayCurrentInformation();

				Console.Write($"Įveskite {r + 1}-ąjį vektorių: ");
				var input = Console.ReadLine();

				if (Regex.IsMatch(input, "^[0,1]{1,}$"))
				{
					if (input.Length != _cols)
					{
						_errorMessage = $"Vektoriaus ilgis privalo būti lygus {_cols}.";
					}
					else
					{
						var row = new int[_cols];
						for (var c = 0; c < _cols; c++)
						{
							row[c] = (int)char.GetNumericValue(input[c]);
						}
						_tempMatrix[r] = row;
						_errorMessage = string.Empty;
						continue;
					}
				}
				else
				{
					_errorMessage = "Leidžiami simboliai yra tik '0' ir '1'.";
				}
				r--;
			}
			try
			{
				return new MatrixG(_cols, _rows, _tempMatrix);
			}
			catch (Exception e)
			{
				_errorMessage = e.Message;
				return GetUserInputedMatrix();
			}
			
		}

		private void DisplayCurrentInformation()
		{
			Console.Clear();

			if (_cols != 0)
				ConsoleHelper.WriteInformation($"Ilgis (n): {_cols}.");

			if (_rows != 0)
				ConsoleHelper.WriteInformation($"Dimensija (k): {_rows}.");

			if (_matrixG == null && _tempMatrix != null)
			{
				var message = "G = ";

				for (var c = 0; c <= _tempMatrix.GetUpperBound(0); c++)
					if (_tempMatrix[c] != null)
						message += string.Join(" ", _tempMatrix[c]) + "\n    ";

				ConsoleHelper.WriteInformation(message);
			}

			if (_matrixG != null)
				ConsoleHelper.WriteMatrix(_matrixG);

			if (_errorMessage != string.Empty)
				ConsoleHelper.WriteError(_errorMessage);

			Console.WriteLine();
		}
	}
}
