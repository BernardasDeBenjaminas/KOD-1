using System;
using System.Linq;

namespace Logic
{
	public class MatrixG
	{
		private readonly int[][] _matrix;
		private readonly int _rows; // Number of rows in '_matrix'.
		private readonly int _cols; // Number of columns in '_matrix'.
		private readonly bool _isTransformableToH;

		// When converting to a standard (H) array.
		private int _hColFirst;	// Denotes the first column of the H to be matrix.
		private int _hColLast;  // Denotes the last column of the H to be matrix.

		public MatrixG(int[][] matrix)
		{
			_matrix = matrix;
			_rows = _matrix[0].GetUpperBound(0);
			_cols = _matrix.GetUpperBound(0);
			_isTransformableToH = IsTransformableToMatrixH();
		}

		public int[] Encode(int[] vector)
		{
			var result = new int[_cols + 1];

			if (_rows != vector.GetUpperBound(0))
				throw new ArgumentException("\nDimension mismatch!\nNumber of rows and columns differ!");

			for (var c = 0; c <= _cols; c++)
			{
				for (var r = 0; r <= _rows; r++)
				{
					result[c] += _matrix[c][r] * vector[r];
				}
				result[c] %= 2;
			}

			return result;
		}

		public int[] Decode(int[] vector)
		{
			// Todo: complete this function.
			// Todo: have a variable for keeping a mapping between words and their encoded variants.
		}

		public bool IsTransformableToMatrixH()
		{
			var location = 0;
			var answer = false;

			for (var c = 0; c <= _cols; c++)
			{
				if (ColumnContainsOnlyOne(_matrix[c], location))
				{
					location++;
					if (location == _rows + 1)
					{
						// Used when generating a standard (H) matrix.
						_hColLast = c; 
						_hColFirst = c - _rows;

						answer = true;
						break;
					}
				}
				else
				{
					location = 0;
				}
			}
			return answer;
		}

		public MatrixH GetMatrixH()
		{
			if (!_isTransformableToH)
				throw new ArgumentException("\nThe current matrix is not transformable to an H matrix.");

			var standardMatrix = CreateStandardMatrix(_cols - _rows);
			var separatedOtherMatrix = SeparateOtherMatrix();
			var matrix = CombineMatrices(standardMatrix, separatedOtherMatrix);

			return new MatrixH(matrix);
		}

		private int[][] CombineMatrices(int[][] standardMatrix, int[][] otherMatrix)
		{
			var numberOfColsInStandard = standardMatrix.GetUpperBound(0) + 1;
			var numberOfColsInOther = otherMatrix.GetUpperBound(0) + 1;
			var numberOfCols = numberOfColsInStandard + numberOfColsInOther;
			var matrix = new int[numberOfCols][];
			var col = 0;

			for (var c = 0; c < numberOfColsInOther; c++)
			{
				matrix[col] = otherMatrix[c];
				col++;
			}

			for (var c = 0; c < numberOfColsInStandard; c++)
			{
				matrix[col] = standardMatrix[c];
				col++;
			}

			return matrix;
		}

		private int[][] CreateStandardMatrix(int dimension)
		{
			// Create a matrix with default values.
			var matrix = new int[3][];
			for (var i = 0; i < dimension; i++)
				matrix[i] = new int[3];

			for (int r = 0, c = 0; r < dimension; r++)
			{
				matrix[r][c] = 1;
				c++;
			}

			return matrix;
		}


		private int[][] SeparateOtherMatrix()
		{
			var numberOfColums = _cols - _rows;

			var matrix = new int[numberOfColums][];
			var col = 0;
			for (var c = 0; c <= _cols; c++)
			{
				if (c < _hColFirst || c > _hColLast)
				{
					matrix[col] = _matrix[c];
					col++;
				}
			}

			col = 0;
			var transformedMatrix = new int[_rows + 1][];
			// We have to take a row and turn it 90 degrees.
			var rowToBeTurned = new int[numberOfColums];
			for (var r = 0; r <= _rows; r++)
			{
				for (var c = 0; c <= numberOfColums - 1; c++)
				{
					// From each 'c' column I take the 'r' row.
					// In result, 'rowToBeTurned' contains the row of what was a column in 'matrix'.
					rowToBeTurned[c] = matrix[c][r];
				}
				// Since we're working in binary no other transformations need to be applied (no inverting).
				transformedMatrix[col] = rowToBeTurned;
				rowToBeTurned = new int[numberOfColums];
				col++;
			}

			return transformedMatrix;
		}

		/// <summary>
		/// Returns 'True' if an array of ints contain a '1' in the specified position.
		/// </summary>
		/// <param name="column">Array of int to search through.</param>
		/// <param name="position">The position in which a '1' is supposed to be.</param>
		/// <returns></returns>
		private bool ColumnContainsOnlyOne(int[] column, int position)
		{
			if (column.Count(n => n == 1) != 1)
				return false;

			return column[position] == 1;
		}

		public void Display()
		{
			ConsoleHelper.WriteMatrix(_matrix);
		}
	}
}
