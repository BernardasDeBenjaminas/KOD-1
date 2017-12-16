using System;

namespace Logic
{
	public class Channel
	{
		public double ChanceOfError { get; set; }
		public Random RandomGenerator { get; set; }

		/// <summary>
		/// Grąžina informacijos siuntimui simuliuojantį kanalą, kuris gali iškraipyti informaciją.
		/// </summary>
		/// <param name="chanceOfError">Tikimybė, jog kanale įvyks klaida.</param>
		public Channel(double chanceOfError)
		{
			if (chanceOfError < 0 || chanceOfError > 1)
				throw new ArgumentException("\nKlaidos tikimybė privalo būti skaičius iš šios aibės - [0;1]!");

			ChanceOfError = chanceOfError;
			RandomGenerator = new Random(DateTime.Now.Millisecond);
		}


		/// <summary>
		/// Grąžina, atsižvelgiant į klaidos tikimybę, iškraipytą vektorių.
		/// </summary>
		/// <param name="vector">Vektorius, kurį norima siųsti kanalu.</param>
		/// <returns>Potencialiai iškraipytas vektorius (0 tampa 1 ir atvirkščiai).</returns>
		public int[] SendVectorThrough(int[] vector)
		{
			var result = Clone(vector);
			var length = result.GetUpperBound(0) + 1;

			for (var c = 0; c < length; c++)
			{
				if (RandomGenerator.NextDouble() <= ChanceOfError)
					result[c] = result[c] ^ 1; // ^ = XOR.
			}

			return result;
		}

		/// <summary>
		/// (!) Grąžinamas vektorius, kuris parodo kuriose pozicijose nesutampa reikšmės tarp pateiktų vektorių. (!)
		/// Pateikus 010 ir 110 būtų grąžinama 100.
		/// 0 - jeigu reikšmės stulpeliuose sutampa, 1 jeigu nesutampa.
		/// </summary>
		/// <param name="vector1">Pirmasis vektorius palyginimui.</param>
		/// <param name="vector2">Antrasis vektorius palyginimui.</param>
		/// <returns>Vektorių sudarytą iš 0 ir 1.</returns>
		public int[] FindDifferences(int[] vector1, int[] vector2)
		{
			var length = vector1.GetUpperBound(0) + 1;

			if (length != vector2.GetUpperBound(0) + 1)
				throw new ArgumentException("The vectors have to be the same length!");

			var answer = new int[length];
			for (var c = 0; c < length; c++)
			{
				answer[c] = vector1[c] != vector2[c]
					? answer[c] = 1
					: answer[c] = 0;
			}
			return answer;
		}


		/// <summary>
		/// Grąžina pateikto vektoriaus kopiją.
		/// </summary>
		/// <param name="vector">Vektorius, kurio reikšmes norima nukopijuoti.</param>
		/// <returns>Vektorius su identiškomis reikšmėmis.</returns>
		private int[] Clone(int[] vector)
		{
			// Todo: currently, the implementation matches the one found in 'MatrixH'.
			var length = vector.GetUpperBound(0) + 1;
			var newVector = new int[length];

			for (var c = 0; c < length; c++)
				newVector[c] = vector[c];

			return newVector;
		}
	}
}
