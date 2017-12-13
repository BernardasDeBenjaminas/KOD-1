using System;
using System.Collections.Generic;

namespace Logic
{
	public class Enigma
	{
		private readonly int _rows;	// n
		private readonly int _cols; // k
		private readonly MatrixG _matrixG; // G
		private readonly MatrixH _matrixH;

		// CONSTRUCTOR
		
		/// <summary>
		/// Creates the history's most sophisticated encoding machine.
		/// I have spent many years studying its inner-workings and subtleties.
		/// All of my gathered knowledged has been recreated in this class.
		/// Don't show it to the brits.
		/// </summary>
		/// <param name="length">Length of the generating matrix's vector.</param>
		/// <param name="dimension">Number of vectors in the generating matrix.</param>
		/// <param name="matrixG">You can provide the generating matrix yourself if so please
		///									  - otherwise it will be created for you.</param>
		public Enigma(int length, int dimension, int[][] matrixG = null)
		{
			_matrixG = new MatrixG(length, dimension, matrixG);
			_matrixH = _matrixG.GetMatrixH();
		}

		
		// PUBLIC

		public int[] Encode(int[] vector)
		{
			return _matrixG.Encode(vector);
		}

		public int[] Decode(int[] vector)
		{
			var one = _matrixH.Decode(vector);
			return _matrixG.Decode(one);
		}

		/// <summary>
		/// Get a list of positions in which changes had been made when sending the vector through the channel.
		/// </summary>
		/// <param name="before">Vector, before it was sent through the channel.</param>
		/// <param name="after">Vector, after it was sent through the channel.</param>
		/// <returns>A list of positions.</returns>
		public List<int> FindChangedPositions(int[] before, int[] after)
		{
			if (before.GetUpperBound(0) != after.GetUpperBound(0))
				throw new ArgumentException("\nThe vectors have to be the same length!");

			var length = before.GetUpperBound(0) + 1;
			var list = new List<int>();

			for (var c = 0; c < length; c++)
				if (before[c] != after[c])
					list.Add(c);

			return list;
		}

		// PRIVATE
	}
}
