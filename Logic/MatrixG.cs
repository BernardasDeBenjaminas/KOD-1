using System;

namespace Logic
{
	public class MatrixG
	{
		private static int[,] _matrix;

		public MatrixG(int[,] matrix)
		{
			_matrix = matrix;
		}

		public int[] EncodeVector(int[] vector)
		{
			var rows = _matrix.GetUpperBound(0);
			var cols = _matrix.GetUpperBound(1);
			var result = new int[cols + 1];

			if (rows != vector.GetUpperBound(0))
				throw new ArgumentException("\nDimension mismatch!\nNumber of rows and columns differ!");

			for (var c = 0; c <= cols; c++)
			{
				for (var r = 0; r <= rows; r++)
				{
					result[c] += _matrix[r, c] * vector[r];
				}
				result[c] %= 2;
			}

			return result;
		}
	}
}
