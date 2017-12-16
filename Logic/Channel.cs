using System;

namespace Logic
{
	public class Channel
	{
		public double ChanceOfError { get; set; }
		public Random RandomGenerator { get; set; }

		// CONSTRUCTOR

		/// <summary>
		/// Returns an object which simulates an unsafe channel for information sending.
		/// </summary>
		/// <param name="chanceOfError">A value in the range of [0;1].</param>
		/// <param name="randomGenerator">Random number generator based on the 'chanceOfError' probability.</param>
		public Channel(double chanceOfError, Random randomGenerator = null)
		{
			if (chanceOfError < 0 || chanceOfError > 1)
				throw new ArgumentException("\nThe chance of error must be a value in the range of [0;1]!");

			ChanceOfError = chanceOfError;
			RandomGenerator = randomGenerator ?? new Random(DateTime.Now.Millisecond);
		}


		// PUBLIC

		/// <summary>
		/// Returns a vector with modified values based on the probability in 'ChanceOfError'.
		/// </summary>
		/// <param name="vector">Vector to be sent through a potentially unsafe channel.</param>
		/// <returns>A modified vector.</returns>
		public int[] SendVectorThrough(int[] vector)
		{
			var result = Clone(vector);
			var length = result.GetUpperBound(0) + 1;

			for (var c = 0; c < length; c++)
			{
				if (RandomGenerator.NextDouble() <= ChanceOfError)
					result[c] = result[c] ^ 1; // 0 would become 1 and 1 would become 0.
			}

			return result;
		}

		/// <summary>
		/// 0 - it matches, 1 - it doesn't match.
		/// </summary>
		/// <param name="original"></param>
		/// <param name="changed"></param>
		/// <returns></returns>
		public int[] FindChanges(int[] original, int[] changed)
		{

			var length = original.GetUpperBound(0) + 1;

			if (length != changed.GetUpperBound(0) + 1)
				throw new ArgumentException("The vectors have to be the same length!");

			var answer = new int[length];
			for (var c = 0; c < length; c++)
			{
				answer[c] = original[c] != changed[c]
					? answer[c] = 1
					: answer[c] = 0;
			}
			return answer;
		}


		// PRIVATE

		/// <summary>
		/// Returns a shallow copy of a passed in vector.
		/// </summary>
		/// <param name="vector">Vector whose values need to be copied without modifying the original.</param>
		/// <returns>A vector with identical values.</returns>
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
