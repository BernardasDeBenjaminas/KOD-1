using System;
using System.Linq;

namespace Logic
{
	public class MatrixH
	{
		private static int[,] _matrix;

		public MatrixH(int[,] matrix)
		{
			_matrix = matrix;
		}

		public int[] CalculateSyndrome(int[] vector)
		{
			var cols = vector.GetUpperBound(0);
			var rows = _matrix.GetUpperBound(0);
			var result = new int[rows + 1];

			// No. of columns in vector must be equal to the no. of columns in matrix. 
			if (cols != _matrix.GetUpperBound(1))
				throw new ArgumentException("\nDimension mismatch!\nNumber of columns differ!");

			for (var r = 0; r <= rows; r++)
			{
				for (var c = 0; c <= cols; c++)
				{
					result[r] += _matrix[r, c] * vector[c];
				}
				result[r] %= 2;
			}
			
			return result;
		}

		public int CalculateWeight(int[] vector)
		{
			return vector.Count(number => number != 0);
		}
	}
}
