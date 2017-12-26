using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logic
{
	/// <summary>
	/// Naudojama validuoti vartotojo įvesčiai.
	/// </summary>
	public class Validator
	{
		private int _rows = -1; // 'Matrix' dimensija (k).
		private int _cols = -1; // 'Matrix' ilgis (n).


		/// <summary>
		/// Patikrina ar įvesta tinkama klaidos tikimybė.
		/// </summary>
		/// <param name="input">Vartotojo įvestas tekstas (klaidos tikimybė).</param>
		/// <returns>Įvestas skaičius, jeigu jis tinkamas.</returns>
		public double ValidateErrorProbability(string input)
		{
			if (double.TryParse(input, out var probability))
			{
				if (probability > 1 || probability < 0)
					throw new ArgumentException("Reikšmė privalo būti intervale [0;1] (ar įvedėte skaičių su kableliu?).");

				return probability;
			}

			throw new ArgumentException("Leidžiama įvedimo forma: #.#### (taškas, ne kablelis).");
		}

		/// <summary>
		/// Patikrina ar įvestas tinkamas kodo ilgis.
		/// </summary>
		/// <param name="input">Vartotojo įvestas tekstas (kodo ilgis).</param>
		/// <returns>Įvestas skaičius, jeigu jis tinkamas.</returns>
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

		/// <summary>
		/// Patikrina ar įvestas tinkama kodo dimensija.
		/// </summary>
		/// <param name="input">Vartotojo įvestas tekstas (kodo dimensija).</param>
		/// <returns>Tas pats skaičius, jeigu jis tinkamas.</returns>
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

		/// <summary>
		/// Patikrina ar įvestas tinkamas vektorius matricos sukūrimui.
		/// </summary>
		/// <param name="input">Vartotojo įvestas tekstas (vektorius).</param>
		/// <returns>Įvestas vektorius, jeigu jis tinkamas.</returns>
		public List<byte> ValidateGMatrixRow(string input)
		{
			if (input == null)
				throw new ArgumentException("Reikšmė negali būti tuščia.");

			if (Regex.IsMatch(input, "^[0,1]{1,}$"))
			{
				if (input.Length != _cols)
					throw new ArgumentException($"Vektoriaus ilgis privalo būti lygus {_cols}.");

				return StringToByteListVector(input);
			}

			throw new ArgumentException("Leidžiami simboliai yra tik '0' ir '1'.");
		}

		/// <summary>
		/// Patikrina ar įvestas tinkama raidė ar žodis.
		/// </summary>
		/// <param name="input">Vartotojo įvestas tekstas (atsakymas į klausimą).</param>
		/// <returns>'true' jeigu vartotojas atsakė 'taip' - antraip 'false'.</returns>
		public bool ValidateYesOrNoAnswer(string input)
		{
			if (input.ToLower() != "y" && input.ToLower() != "n")
				throw new ArgumentException("Įveskite 'y', jeigu norite, antraip 'n'.");

			return input.ToLower() == "y";
		}

		/// <summary>
		/// Patikrina ar įvestas tinkamas vektorius siuntimui kanalu.
		/// </summary>
		/// <param name="input">Vartotojo įvestas tekstas (vektorius).</param>
		/// <returns>Įvestas vektorius, jeigu jis tinkamas.</returns>
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

		/// <summary>
		/// Patikrina ar įvestas tinkamas stulpelio numeris.
		/// </summary>
		/// <param name="input">Vartotojo įvestas tekstas (stulpelio numeris).</param>
		/// <returns>Įvestas numeris, jeigu jis tinkamas.</returns>
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
		/// Konvertuoja 'string' tipo vektorių į 'List(byte)' tipą.
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
