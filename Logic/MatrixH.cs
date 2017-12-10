using System;
using System.Linq;

namespace Logic
{
	public class MatrixH
	{
		private static int[][] _matrix;
		private readonly int _rows; // Number of rows in '_matrix'.
		private readonly int _cols; // Number of columns in '_matrix'.

		public MatrixH(int[][] matrix)
		{
			_matrix = matrix;
			_rows = _matrix[0].GetUpperBound(0);
			_cols = _matrix.GetUpperBound(0);
		}

		public int[] CalculateSyndrome(int[] vector)
		{
			var result = new int[_rows + 1];

			// No. of columns in vector must be equal to the no. of columns in matrix. 
			if (_cols != vector.GetUpperBound(0))
				throw new ArgumentException("\nDimension mismatch!\nNumber of columns differ!");

			for (var r = 0; r <= _rows; r++)
			{
				for (var c = 0; c <= _cols; c++)
				{
					result[r] += _matrix[c][r] * vector[c];
				}
				result[r] %= 2;
			}
			
			return result;
		}

		public int CalculateWeight(int[] vector)
		{
			return vector.Count(number => number != 0);
		}

		public void Display()
		{
			ConsoleHelper.WriteMatrix(_matrix);
		}
	}
}
