using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class MatrixH
	{
		// '_translations' kintamajame laikysiu klasių lyderių svorius ir jų sindromus:
		//		Raktas  - klasės lyderio sindromas;
		//		Reikšmė - klasės lyderio svoris.
		private readonly Dictionary<string, int> _translations = new Dictionary<string, int>();
		public readonly List<List<byte>> Matrix; // Pagrindinė matrica.
		private readonly int _rows; // 'Matrix' dimensija (k).
		private readonly int _cols; // 'Matrix' ilgis (n).

		/// <summary>
		/// Apiformina pateiktą kontrolinę matricą tipo 'MatrixH' objektu.
		/// </summary>
		/// <param name="matrix">Kontrolinė matrica, kuris bus apiforminta.</param>
		public MatrixH(List<List<byte>> matrix)
		{
			if (CheckIfProperMatrixGiven(matrix))
			{
				Matrix = matrix;
				_rows = matrix.Count;
				_cols = matrix[0].Count;

				FillTranslationsTable();
			}
		}


		// PUBLIC
		/// <summary>
		/// Dekoduojamas vektorius naudojantis 'Step-by-Step' algoritmu.
		/// Grąžinamas vektorius, kurį galima gauti naudojantis generuojančia matrica.
		/// </summary>
		/// <param name="vector">Vektorius, kurį norima dekoduoti.</param>
		/// <returns>Dekoduotas vektorius.</returns>
		public List<byte> Decode(List<byte> vector)
		{
			var result = Clone(vector); 

			for (var c = 0; c < _cols; c++)
			{
				// 1. 'tuple' vektorius sudarytas iš nulių išskyrus vieną poziciją 
				//	  (mūsų atveju pažymėtą 'c' kintamuoju, kuri turės vienetą).
				var tuple = new List<byte>(_cols);
				for (var i = 0; i < c; i++)
					tuple.Add(0);
				tuple.Add(1);
				for (var i = c + 1; i < _cols; i++)
					tuple.Add(0);
				// 2. Apskaičiuojame 'vector'-iaus sindromą.
				//    Iš '_translations' kintamojo ištraukiame atitinkamo klasės lyderio svorį.
				var syndrome = string.Join("", GetSyndrome(result));
				var weight = _translations[syndrome];

				// 3. Jeigu klasės lyderio svoris lygus 0, reiškia šis žodis ir buvo siųstas.
				if (weight == 0)
				{
					break;
				}

				// 4. Jeigu ('vector' + 'tuple') sindromo svoris mažesnis už 'vector', 
				//    tuomet 'vector' = 'vector' + 'tuple'.
				var addedVectors = Field.Add(tuple, result);
				var newSyndrome = string.Join("", GetSyndrome(addedVectors));
				var newWeight = _translations[newSyndrome];
				if (newWeight < weight)
				{
					result = addedVectors;
				}

				// 5. c = c + 1
				//    'tuple' vektoriuje vienintelis vienetas dabar bus viena pozicija dešinėn pasislinkęs.
			}

			return result;
		}

		// PRIVATE
		/// <summary>
		/// Apskaičiuoja pateikto vektoriaus sindromą.
		/// </summary>
		/// <param name="vector">Vektorius, kurio sindromą norima apskaičiuoti.</param>
		/// <returns>Sindromas.</returns>
		private List<byte> GetSyndrome(List<byte> vector)
		{
			if (vector.Count != _cols)
				throw new ArgumentException("\nPaduoto vektoriaus ilgis privalo sutapti su matricos ilgiu.");

			var syndrome = new List<byte>(_rows);
			for (var r = 0; r < _rows; r++)
			{
				var row1 = Matrix[r];
				syndrome.Add(Field.Multiply(row1, vector));
			}
			return syndrome;
		}

		/// <summary>
		/// Užpildo '_translations' kintamąjį su visais galimais klasių lyderių svoriais ir jų sindromais.
		/// </summary>
		private void FillTranslationsTable()
		{
			var leadersNeeded = (int) Math.Pow(2, _rows);
			var weight = 0;
			// Nes būnant ciklo cikle 'break' sakinys leidžia ištrūkti tik iš vieno.
			var endThisNow = false;

			while (true)
			{
				// Pereiname per visus lyderius svorio 'weight'.
				var leaders = GetPotentialLeaders(_cols, weight);
				foreach (var leaderAsString in leaders)
				{
					var leaderAsArray = StringToIntArrayVector(leaderAsString);
					var syndromeAsArray = GetSyndrome(leaderAsArray);
					var syndromeAsString = string.Join("", syndromeAsArray);

					if (_translations.ContainsKey(key: syndromeAsString)) 
						continue;

					_translations.Add(key: syndromeAsString, value: GetWeight(leaderAsArray));

					if (_translations.Count == leadersNeeded)
					{
						endThisNow = true;
						break;
					}
				}
				if (endThisNow)
					break;

				// Neužteko svorio x lyderių tad dabar eisime per visus x + 1 svorio lyderius.
				weight++;
			}
		}

		/// <summary>
		/// Grąžina visus galimus nurodyto svorio bei ilgio vektorius.
		/// </summary>
		/// <param name="length">Vektorių ilgis.</param>
		/// <param name="weight">Vektorių svoris.</param>
		/// <returns>Sąrašą vektorių.</returns>
		private static List<string> GetPotentialLeaders(int length, int weight)
		{
			var numberOfPossibleLeaders = (int) Math.Pow(2, length);
			var leaders = new List<string>();

			// Optimizacija kuomet svoris = 0.
			if (weight == 0)
			{
				leaders.Add(new string('0', length));
				return leaders;
			}

			// Optimizacija kuomet svoris = 1.
			if (weight == 1)
			{
				for (var i = 0; i < length; i++)
				{
					var leader = new int[length];
					leader[i] = 1;
					leaders.Add(string.Join("", leader));
				}
				return leaders;
			}

			// Trejetas yra pirmasis dvejetainis skaičius, kurio svoris lygus dviems.
			for (var i = 3; i < numberOfPossibleLeaders; i++)
			{
				// Dešimtainį skaičių ('i') paverčia dvejetainiu ir iš priekio užpildo nuliais iki reikiamo ilgio.
				var leader = Convert.ToString(value: i, toBase: 2)
									.PadLeft(totalWidth: length, paddingChar: '0');
				// Jeigu lyderio svoris sutampa su norimu - pridedame jį.
				if (leader.Count(n => n == '1') == weight)
					leaders.Add(leader);
			}

			return leaders;
		}

		/// <summary>
		/// Konvertuoja 'string' tipo vektorių į 'byte[]' tipą.
		/// </summary>
		/// <param name="vector">Vektorius, kurį norime konvertuoti.</param>
		/// <returns>Konvertuotą vektorių.</returns>
		private static List<byte> StringToIntArrayVector(string vector)
		{
			var length = vector.Length;
			var row = new List<byte>(length);
			for (var c = 0; c < length; c++)
			{
				row.Add((byte)char.GetNumericValue(vector[c]));
			}
			return row;
		}

		/// <summary>
		/// Suskaičiuoja kiek pateiktame vektoriuje yra vienetų.
		/// </summary>
		/// <param name="vector">Vektorius, kuriame reikia skaičiuoti vienetus.</param>
		/// <returns>Vektoriaus svoris.</returns>
		private static int GetWeight(List<byte> vector)
		{
			return vector.Count(n => n == 1);
		}

		/// <summary>
		/// Grąžina vektorių su identiškomis reikšmėmis pateiktam klonavimui vektoriui.
		/// </summary>
		/// <param name="vector">Vektorius, kurio reikšmes reikia nukopijuoti į atskirą vektorių.</param>
		/// <returns>Vektorius su identiškomis reikšmėmis pateiktam vektoriui.</returns>
		public List<byte> Clone(List<byte> vector)
		{
			var length = vector.Count;
			var newVector = new List<byte>(length);

			for (var c = 0; c < length; c++)
				newVector.Add(vector[c]);

			return newVector;
		}

		/// <summary>
		/// Patikrina ar paduota matrica tinkama naudojimui.
		/// </summary>
		/// <param name="matrix">Matrica patikrinimui.</param>
		/// <returns>Grąžina 'true' jeigu matrica tinkama - antraip 'false'.</returns>
		private bool CheckIfProperMatrixGiven(List<List<byte>> matrix)
		{
			if (matrix == null)
				throw new ArgumentNullException(nameof(matrix), "\nPaduota matrica yra neinicializuota (null).");

			for (var r = 0; r < matrix.Count; r++)
			{
				try
				{
					var test = matrix[r].Count;
				}
				catch (Exception)
				{
					throw new ArgumentException("\nPaduota matrica turi neinicializuotų (null) eilučių.");
				}
			}

			return true;
		}
	}
}
