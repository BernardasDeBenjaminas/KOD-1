using System;

namespace Logic
{
	/// <summary>
	/// Kūnas F2, kuris gali atlikti sudėties bei daugybos veiksmus moduliu 2.
	/// </summary>
	public static class Field
	{
		/// <summary>
		/// Sudeda du vektorius kartu moduliu 2.
		/// </summary>
		/// <param name="vector1">Pirmasis vektorius sudėčiai.</param>
		/// <param name="vector2">Antrasis vektorius sudėčiai.</param>
		/// <returns>Vektorius, kuris yra pateiktų vektorių sumos rezultatas.</returns>
		public static int[] Add(int[] vector1, int[] vector2)
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
		public static int Multiply(int[] vector1, int[] vector2)
		{
			if (vector1.GetUpperBound(0) != vector2.GetUpperBound(0))
				throw new ArgumentException("\nVektoriai privalo būti vienodo ilgio.");

			var length = vector1.GetUpperBound(0) + 1;
			var result = 0;

			for (var c = 0; c < length; c++)
				result += vector1[c] * vector2[c];
			result %= 2;

			return result;
		}
	}
}
