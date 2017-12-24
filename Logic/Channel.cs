using System;
using System.Collections.Generic;

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
		public List<byte> SendVectorThrough(List<byte> vector)
		{
			var result = Clone(vector);
			var length = result.Count;

			for (var c = 0; c < length; c++)
			{
				if (RandomGenerator.NextDouble() <= ChanceOfError)
					result[c] = (byte) (result[c] ^ 1); // ^ = XOR.
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
		public List<byte> FindDifferences(List<byte> vector1, List<byte> vector2)
		{
			var length = vector1.Count;

			if (length != vector2.Count)
				throw new ArgumentException("The vectors have to be the same length!");

			var answer = new List<byte>(length);
			for (var c = 0; c < length; c++)
			{
				if (vector1[c] != vector2[c])
					answer.Add(1);
				else
					answer.Add(0);
			}
			return answer;
		}

		/// <summary>
		/// Grąžina pateikto vektoriaus kopiją.
		/// </summary>
		/// <param name="vector">Vektorius, kurio reikšmes norima nukopijuoti.</param>
		/// <returns>Vektorius su identiškomis reikšmėmis.</returns>
		private List<byte> Clone(List<byte> vector)
		{
			var length = vector.Count;
			var newVector = new List<byte>(length);

			for (var c = 0; c < length; c++)
				newVector.Add(vector[c]);

			return newVector;
		}
	}
}
