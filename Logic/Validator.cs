using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logic
{
	// Todo: add summaries.
	public class Validator
	{
		private double _errorProbability = -1;
		private int _rows = -1;
		private int _cols = -1;

		public double ValidateErrorProbability(string input)
		{
			if (double.TryParse(input, out var probability))
			{
				if (probability > 1 || probability < 0)
					throw new ArgumentException("Reikšmė privalo būti intervale [0;1] (ar įvedėte skaičių su kableliu?).");

				_errorProbability = probability;
				return probability;
			}

			throw new ArgumentException("Leidžiama įvedimo forma: #.#### (taškas, ne kablelis).");
		}

		public int ValidateNumberOfCols(string input)
		{
			if (int.TryParse(input, out var cols))
			{
				if (cols < 2)
					throw new ArgumentException("Reikšmė privalo būti didesnė už 1.");

				_cols = cols;
				return cols;
			}
			
			throw new ArgumentException("Galimos reikšmės tik sveikieji skaičiai.");
		}

		public int ValidateNumberOfRows(string input)
		{
			if (_cols == -1)
				throw new ArgumentException("Iš pirmo privalote kviesti dimensijos patikrinimo metodą.");

			if (int.TryParse(input, out var rows))
			{
				if (rows < 1)
					throw new ArgumentException("Reikšmė privalo būti didesnė už 0.");

				if (rows == _cols)
					throw new ArgumentException($"Reikšmė negali sutapti su ilgiu (n = {_cols}).");

				if (rows > _cols)
					throw new ArgumentException($"Reikšmė negali būti didesnė už kodo ilgį (n = {_cols}).");

				_rows = rows;
				return rows;
			}

			throw new ArgumentException("Galimos reikšmės tik sveikieji skaičiai.");
		}

		public List<byte> ValidateVectorToSend(string input)
		{
			if (Regex.IsMatch(input, "^[0,1]{1,}$"))
			{
				if (input.Length != _rows)
					throw new ArgumentException($"Vektoriaus ilgis privalo būti lygus {_rows}.");
				
				return StringToByteListVector(input);
			}

			throw new ArgumentException("Leidžiami simboliai yra tik '0' ir '1'.");
		}

		public bool ValidateYesOrNoAnswer(string input)
		{
			if (input.ToLower() != "y" && input.ToLower() != "n")
				throw new ArgumentException("Įveskite 'y', jeigu norite keisti iš kanalo gautą vektorių, antraip 'n'.");

			return input.ToLower() == "y";
		}

		public List<byte> ValidateGMatrixRow(string input)
		{
			if (Regex.IsMatch(input, "^[0,1]{1,}$"))
			{
				if (input.Length != _cols)
					throw new ArgumentException($"Vektoriaus ilgis privalo būti lygus {_cols}.");

				return StringToByteListVector(input);
			}

			throw new ArgumentException("Leidžiami simboliai yra tik '0' ir '1'.");
		}

		public int ValidateErrorVectorColumn(string input)
		{
			if (int.TryParse(input, out var col))
			{
				if (col > _cols || col < 1)
					throw new ArgumentException($"Skaičius privalo būti tarp [1;{_cols}].");

				return col;
			}
			
			throw new ArgumentException("Galimos reikšmės tik sveikieji skaičiai.");
		}


		/// <summary>
		/// Konvertuoja 'string' tipo vektorių į 'int[]' tipą.
		/// </summary>
		/// <param name="vector">Vektorius, kurį norime konvertuoti.</param>
		/// <returns>Konvertuotą vektorių.</returns>
		private List<byte> StringToByteListVector(string vector)
		{
			var length = vector.Length;
			var row = new List<byte>(length);
			for (var c = 0; c < length; c++)
			{
				row.Add((byte)char.GetNumericValue(vector[c]));
			}
			return row;
		}
	}
}
