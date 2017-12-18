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
		public readonly int[][] Matrix; // Pagrindinė matrica.
		private readonly int _rows; // 'Matrix' dimensija (k).
		private readonly int _cols; // 'Matrix' ilgis (n).


		// CONSTRUCTOR

		/// <summary>
		/// Apiformina pateiktą kontrolinę matricą tipo 'MatrixH' objektu.
		/// </summary>
		/// <param name="matrix">Kontrolinė matrica, kuris bus apiforminta.</param>
		public MatrixH(int[][] matrix)
		{
			if (CheckIfProperMatrixGiven(matrix))
			{
				Matrix = matrix;
				_rows = matrix.GetUpperBound(0) + 1;
				_cols = matrix[0].GetUpperBound(0) + 1;

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
		public int[] Decode(int[] vector)
		{
			var result = Clone(vector); 

			for (var c = 0; c < _cols; c++)
			{
				// 1. 'tuple' vektorius sudarytas iš nulių išskyrus vieną poziciją 
				//	  (mūsų atveju pažymėtą 'c' kintamuoju, kuri turės vienetą).
				var tuple = new int[_cols];
				tuple[c] = 1;

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
				var addedVectors = AddVectors(tuple, result);
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
		private int[] GetSyndrome(int[] vector)
		{
			if (vector.GetUpperBound(0) + 1 != _cols)
				throw new ArgumentException("\nPaduoto vektoriaus ilgis privalo sutapti su matricos ilgiu.");

			var syndrome = new int[_rows];
			for (var r = 0; r < _rows; r++)
			{
				var row1 = Matrix[r];
				syndrome[r] = MultiplyVectors(row1, vector);
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

			// Labai paprasta optimizacija.
			if (weight == 0)
			{
				leaders.Add(new string('0', length));
				return leaders;
			}

			for (var i = 0; i < numberOfPossibleLeaders; i++)
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

		// Todo: pavogta iš 'Presenter'.
		/// <summary>
		/// Konvertuoja 'string' tipo vektorių į 'int[]' tipą.
		/// </summary>
		/// <param name="vector">Vektorius, kurį norime konvertuoti.</param>
		/// <returns>Konvertuotą vektorių.</returns>
		private static int[] StringToIntArrayVector(string vector)
		{
			var length = vector.Length;
			var row = new int[length];
			for (var c = 0; c < length; c++)
			{
				row[c] = (int) char.GetNumericValue(vector[c]);
			}
			return row;
		}

		/// <summary>
		/// Suskaičiuoja kiek pateiktame vektoriuje yra vienetų.
		/// </summary>
		/// <param name="vector">Vektorius, kuriame reikia skaičiuoti vienetus.</param>
		/// <returns>Vektoriaus svoris.</returns>
		private static int GetWeight(int[] vector)
		{
			return vector.Count(n => n == 1);
		}

		/// <summary>
		/// Sudeda du vektorius kartu moduliu 2.
		/// </summary>
		/// <param name="vector1">Pirmasis vektorius sudėčiai.</param>
		/// <param name="vector2">Antrasis vektorius sudėčiai.</param>
		/// <returns>Vektorius, kuris yra pateiktų vektorių sumos rezultatas.</returns>
		private static int[] AddVectors(int[] vector1, int[] vector2)
		{
			if (vector1.GetUpperBound(0) != vector2.GetUpperBound(0))
				throw new ArgumentException("\nVektoriai privalo būti vienodo ilgio.");

			var length = vector1.GetUpperBound(0) + 1;
			var result = new int[length];

			for (var c = 0; c < length; c++)
				result[c] = (vector1[c] + vector2[c]) % 2;

			return result;
		}

		/// <summary>
		/// Sudaugina du vektorius kartu moduliu 2.
		/// </summary>
		/// <param name="vector1">Pirmasis vektorius daugybai.</param>
		/// <param name="vector2">Antrasis vektorius daugybai.</param>
		/// <returns>Skaičius, kuris yra pateiktų vektorių sandaugos rezultatas.</returns>
		private static int MultiplyVectors(int[] vector1, int[] vector2)
		{
			// Todo: currently, the implementation matches the one found in 'MatrixG'.
			if (vector1.GetUpperBound(0) != vector2.GetUpperBound(0))
				throw new ArgumentException("\nVektoriai privalo būti vienodo ilgio.");

			var length = vector1.GetUpperBound(0) + 1;
			var result = 0;

			for (var c = 0; c < length; c++)
				result += vector1[c] * vector2[c];
			result %= 2;

			return result;
		}

		/// <summary>
		/// Grąžina vektorių su identiškomis reikšmėmis pateiktam klonavimui vektoriui.
		/// </summary>
		/// <param name="vector">Vektorius, kurio reikšmes reikia nukopijuoti į atskirą vektorių.</param>
		/// <returns>Vektorius su identiškomis reikšmėmis pateiktam vektoriui.</returns>
		public int[] Clone(int[] vector)
		{
			var length = vector.GetUpperBound(0) + 1;
			var newVector = new int[length];

			for (var c = 0; c < length; c++)
				newVector[c] = vector[c];

			return newVector;
		}

		/// <summary>
		/// Patikrina ar paduota matrica tinkama naudojimui.
		/// </summary>
		/// <param name="matrix">Matrica patikrinimui.</param>
		/// <returns>Grąžina 'true' jeigu matrica tinkama - antraip 'false'.</returns>
		private bool CheckIfProperMatrixGiven(int[][] matrix)
		{
			if (matrix == null)
				throw new ArgumentNullException(nameof(matrix), "\nPaduota matrica yra neinicializuota (null).");

			for (var r = 0; r < matrix.GetUpperBound(0) + 1; r++)
			{
				try
				{
					var test = matrix[r];
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
