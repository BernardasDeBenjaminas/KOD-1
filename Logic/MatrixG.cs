using System;
using System.Linq;

namespace Logic
{
	public class MatrixG
	{
		private readonly int[][] _matrix; // The main matrix.
		private readonly int _length;	  // Length of the vectors.
		private readonly int _dimension;  // Number of the vectors.

		private readonly int _rows; // Number of rows in '_matrix'.
		private readonly int _cols; // Number of columns in '_matrix'.
		private readonly bool _isTransformableToH;

		// When converting to a standard (H) array (index means value of zero for the first column).
		private int _indexStandardColFirst; // The index of the column where the standard matrix begins.
		private int _indexStandardColLast;  // The index of the column where the standard matrix ends.

		/// <summary>
		/// Returns a new object of type 'MatrixG'.
		/// </summary>
		/// <param name="length">Length of the vectors.</param>
		/// <param name="dimension">Number of vectors.</param>
		/// <param name="matrix">(Optional) a created matrix to be used. If none is provided - a new one will be generated.</param>
		public MatrixG(int length, int dimension, int[][] matrix = null)
		{
			if (length <= 0)
				throw new ArgumentException("\nThe length cannot be zero or less.");

			if (dimension <= 0)
				throw new ArgumentException("\nThe dimension cannot be zero or less.");

			_length = length;
			_dimension = dimension;

			if (matrix != null)
			{
				_matrix = CheckIfProperMatrixGiven(matrix) ? matrix : GenerateMatrix(length, dimension);
			}
		}

		/// <summary>
		/// Check if a given matrix is useable.
		/// </summary>
		/// <param name="matrix">A matrix to be checked.</param>
		/// <returns>'true' is the matrix is proper, 'false' if not.</returns>
		private bool CheckIfProperMatrixGiven(int[][] matrix)
		{
			var length = matrix[0].GetUpperBound(0) + 1;
			var dimension = matrix.GetUpperBound(0) + 1;

			if (length != _length)
				throw new ArgumentException("\nThe length of vectors in the given matrix " +
				                            "does not match the length provided to the constructor.");

			if (dimension != _dimension)
				throw new ArgumentException("\nThe dimension (number of vectors) in the given matrix " +
				                            "does not match the dimension provided to the constructor.");

			for (var r = 0; r < dimension; r++)
			{
				try
				{
					var test = matrix[r];
				}
				catch (Exception)
				{
					throw new ArgumentException("\nThe provided matrix contains a null row.");
				}
			}

			if (!IsTransformableToMatrixH(matrix))
				throw new ArgumentException("\nThe provided matrix is not a standard G matrix " +
				                            "(can not be transformed to an H matrix).");

			return true;
		}

		/// <summary>
		/// Generates a matrix of specified dimensions with a standard matrix in it.
		/// </summary>
		/// <param name="length"></param>
		/// <param name="dimension"></param>
		/// <returns></returns>
		private int[][] GenerateMatrix(int length, int dimension)
		{
			// Todo: will it work with a 1x1?
			// Generate the standard matrix.
			int[][] standardMatrix = GenerateStandardMatrix(dimension);
			// Generate the random matrix.
			int[][] randomMatrix = GenerateRandomMatrix(rows: dimension, cols: length - dimension);
			// Return the combination of them.
			return CombineMatrices(standardMatrix, randomMatrix);
		}

		/// <summary>
		/// Combines two matrices together to form a new one.
		/// </summary>
		/// <param name="matrix1">The first matrix.</param>
		/// <param name="matrix2">The second matrix.</param>
		/// <returns>A new matrix.</returns>
		private int[][] CombineMatrices(int[][] matrix1, int[][] matrix2)
		{
			if (matrix1.GetUpperBound(0) != matrix2.GetUpperBound(0))
				throw new ArgumentException("\nThe dimensions of the matrices must be equal!");

			var numberOfRows = matrix1.GetUpperBound(0) + 1;
			var numberOfCols = matrix1[0].GetUpperBound(0) + 1 + matrix2[0].GetUpperBound(0) + 1;
			var matrix = new int[numberOfRows][];

			for (var r = 0; r < numberOfRows; r++)
			{
				// Create a new row.
				var row = new int[numberOfCols];
				// Store the current position when copying values.
				var colIndex = 0;
				// Copy values from the first matrix.
				for (var c1 = 0; c1 <= matrix1[0].GetUpperBound(0); c1++)
				{
					row[colIndex] = matrix1[r][c1];
					colIndex++;
				}
				// Copy the values from the second matrix.
				for (var c2 = 0; c2 <= matrix2[0].GetUpperBound(0); c2++)
				{
					row[colIndex] = matrix2[r][c2];
					colIndex++;
				}
				// Add the row to the matrix.
				matrix[r] = row;
			}

			return matrix;
		}

		/// <summary>
		/// Generates a matrix with random members between '1' and '0' of specified size.
		/// </summary>
		/// <param name="rows">Number of rows in the matrix.</param>
		/// <param name="cols">Number of cols in the matrix.</param>
		/// <returns>A new matrix.</returns>
		private int[][] GenerateRandomMatrix(int rows, int cols)
		{
			var randomGenerator = new Random(DateTime.Now.Millisecond);
			var matrix = new int[rows][];

			for (var r = 0; r < rows; r++)
			{
				// Create a new row.
				var vector = new int[cols];
				// Randomly generate values for it.
				for (var c = 0; c < cols; c++)
					vector[c] = randomGenerator.Next(0, 2);
				// Add the new row to the matrix.
				matrix[r] = vector;
			}

			return matrix;
		}

		/// <summary>
		/// Generates a new standard matrix.
		/// </summary>
		/// <param name="size">What size the matrix should be (example 2x2 would be with 'size' = 2).</param>
		/// <returns>A new standard matrix.</returns>
		private int[][] GenerateStandardMatrix(int size)
		{
			var matrix = new int[size][];
			for (var r = 0; r < size; r++)
			{
				// Create the new row.
				var row = new int[size];
				// Assign the only '1'.
				row[r] = 1;
				// Add the row to the matrix.
				matrix[r] = row;
			}

			return matrix;
		}

		// Todo: test.
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
			return null;
		}

		/// <summary>
		/// Checks to see whether a standard matrix can be found inside.
		/// </summary>
		/// <param name="matrix"></param>
		/// <returns>'true' if can be transformed, 'false' else.</returns>
		public bool IsTransformableToMatrixH(int[][] matrix)
		{
			// Todo: will not work if a given matrix has two columns in a row which can be the start of a standard matrix.
			var numberOfCols = matrix[0].GetUpperBound(0) + 1;
			var numberOfRows = matrix.GetUpperBound(0) + 1;
			// We mark the position in only which a '1' is supposed to be (elsewhere should be '0').
			var position = 0;
			var result = false;

			for (var c = 0; c < numberOfCols; c++)
			{
				var column = GetColumn(matrix, c);
				if (ColumnContainsOnlyOne(column, position))
				{
					// In the next column the '1' should be a row lower.
					position++;
					// When the standard matrix is found.
					if (position == numberOfRows)
					{
						// We use these when generating the H matrix.
						_indexStandardColFirst = c - numberOfRows + 1; // + 1 because 'numberOfRows' is not an index.
						_indexStandardColLast = c;
						result = true;
						break;
					}
				}
				else
				{
					// The standard array must always begin with a '1' in the first position (zero index).
					position = 0;
				}
			}
			return result;
		}

		// Todo: test.
		public MatrixH GetMatrixH()
		{
			if (!_isTransformableToH)
				throw new ArgumentException("\nThe current matrix is not transformable to an H matrix.");

			var standardMatrix = CreateStandardMatrix(_cols - _rows);
			var separatedOtherMatrix = SeparateOtherMatrix();
			var matrix = CombineMatrices(standardMatrix, separatedOtherMatrix);

			return new MatrixH(matrix);
		}

		//// Todo: test.
		//private int[][] CombineMatrices(int[][] standardMatrix, int[][] otherMatrix)
		//{
		//	var numberOfColsInStandard = standardMatrix.GetUpperBound(0) + 1;
		//	var numberOfColsInOther = otherMatrix.GetUpperBound(0) + 1;
		//	var numberOfCols = numberOfColsInStandard + numberOfColsInOther;
		//	var matrix = new int[numberOfCols][];
		//	var col = 0;

		//	for (var c = 0; c < numberOfColsInOther; c++)
		//	{
		//		matrix[col] = otherMatrix[c];
		//		col++;
		//	}

		//	for (var c = 0; c < numberOfColsInStandard; c++)
		//	{
		//		matrix[col] = standardMatrix[c];
		//		col++;
		//	}

		//	return matrix;
		//}

		// Todo: test.
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

		// Todo: test.
		private int[][] SeparateOtherMatrix()
		{
			var numberOfColums = _cols - _rows;

			var matrix = new int[numberOfColums][];
			var col = 0;
			for (var c = 0; c <= _cols; c++)
			{
				//if (c < _hColFirst || c > _hColLast)
				//{
				//	matrix[col] = _matrix[c];
				//	col++;
				//}
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
		/// Checks if an array of ints has only one '1' in it and if it's in the specified position.
		/// </summary>
		/// <param name="column">Array of int to search through.</param>
		/// <param name="position">The position in which a '1' is supposed to be.</param>
		/// <returns>'true' if in the whole array a '1' is only in the specified position, 'false' otherwise.</returns>
		private bool ColumnContainsOnlyOne(int[] column, int position)
		{
			// Check if the array contains only one '1'.
			if (column.Count(n => n == 1) != 1)
				return false;

			// Check if the only '1' is in the specified position.
			return column[position] == 1;
		}

		/// <summary>
		/// Gets a specified column from a specified matrix.
		/// </summary>
		/// <param name="matrix">The matrix from which to get the column.</param>
		/// <param name="columnNumber">Index of which column to get (if you need the first column, you pass a zero).</param>
		/// <returns></returns>
		private int[] GetColumn(int[][] matrix, int columnNumber)
		{
			var numberOfRows = matrix.GetUpperBound(0) + 1;
			var column = new int[numberOfRows];

			for (var r = 0; r < numberOfRows; r++)
				column[r] = matrix[r][columnNumber];

			return column;
		}
		

		public void Display()
		{
			ConsoleHelper.WriteMatrix(_matrix);
		}
	}
}
