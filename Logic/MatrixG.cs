using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class MatrixG
	{
		// This is where I'll store words and their encoded meanings.
		//		Key - encoded;
		//		Value - original.
		private readonly Dictionary<string, int[]> _translations = new Dictionary<string, int[]>(); 
		private readonly int[][] _matrix; // The main matrix.
		private readonly int _rows;	// Number of rows in '_matrix'.
		private readonly int _cols; // Number of columns in '_matrix'.


		// CONSTRUCTOR

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

			_rows = dimension;
			_cols = length;

			if (matrix != null)
			{
				_matrix = CheckIfProperMatrixGiven(matrix) ? matrix : GenerateMatrix(length, dimension);
			}

			FillInnerTable();
		}


		// PUBLIC 

		/// <summary>
		/// Encodes a vector and adds it to it's inner table.
		/// </summary>
		/// <param name="vector">The vector to be encoded.</param>
		/// <returns>An encoded vector.</returns>
		public int[] Encode(int[] vector)
		{
			if (vector.GetUpperBound(0) + 1 != _rows)
				throw new ArgumentException("\nThe number of columns in the vector must match the number of rows in the matrix!");

			var result = new int[_cols];
			for (var c = 0; c < _cols; c++)
			{
				var matrixCol = GetColumn(_matrix, c);
				result[c] = MultiplyVectors(matrixCol, vector);
			}

			// Add the encoded vector to the table.
			var key = string.Join("", result);
			if (!_translations.ContainsKey(key))
				_translations.Add(key, vector);

			return result;
		}

		/// <summary>
		/// Looks in to its inner table to see if the passed in vector has been encoded before and if it has - it returns it.
		/// </summary>
		/// <param name="vector">The vector to be decoded.</param>
		/// <returns>The decoded vector if one is found, else raises an exception.</returns>
		public int[] Decode(int[] vector)
		{
			if (vector.GetUpperBound(0) + 1 != _cols)
				throw new ArgumentException("\nThe passed in vector is too long to have been coded with this instance of the matrix!");

			var key = string.Join("", vector);

			if (!_translations.ContainsKey(key))
				throw new ArgumentException("\nThe passed in vector was never encoded in the first place!");

			return _translations[key];
		}

		/// <summary>
		/// Returns the 'H' matrix of the current 'G' matrix.
		/// </summary>
		/// <returns>The 'H' matrix of the current matrix.</returns>
		public MatrixH GetMatrixH()
		{
			var standardMatrix = GenerateStandardMatrix(_cols - _rows);
			var separatedMatrix = SeparateOtherMatrix();
			var twistedMatrix = TwistMatrix(separatedMatrix);
			var combinedMatrix = CombineMatrices(twistedMatrix, standardMatrix);
			return new MatrixH(combinedMatrix);
		}

		/// <summary>
		/// Displays vectors that were used for encoding and the encoding outcome.
		/// </summary>
		public void DisplayInnerTable()
		{
			foreach (var item in _translations)
			{
				var encoded = string.Join("", item.Key);
				var original = string.Join("", item.Value);
				Console.WriteLine($"{original} becomes {encoded}.");
			}
		}

		/// <summary>
		/// Prints out the matrix to the console.
		/// </summary>
		public void DisplayMatrix()
		{
			ConsoleHelper.WriteInformation("The 'G' matrix.");
			ConsoleHelper.WriteMatrix(_matrix);
		}


		// PRIVATE

		/// <summary>
		/// Fills the '_translations' variable with all of the possible words that can be encoded so that we could decode any given vector.
		/// </summary>
		private void FillInnerTable()
		{
			while (_translations.Count < Math.Pow(2, _rows))
			{
				// Generate a random vector.
				var word = GenerateVector(_rows);
				// Encode it.
				var translation = Encode(word);
				// Convert the encoded vector to a string.
				var key = string.Join("", translation);
				// If a key doesn't exist - we add it.
				if (!_translations.ContainsKey(key))
					_translations.Add(key, word);
			}
		}

		/// <summary>
		/// Checks to see whether a standard matrix can be found inside.
		/// </summary>
		/// <param name="matrix"></param>
		/// <returns>'true' if can be transformed, 'false' else.</returns>
		private bool IsTransformableToMatrixH(int[][] matrix)
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

		/// <summary>
		/// Multiplies 2 vectors using mod 2.
		/// </summary>
		/// <param name="vector1">The first vector to be multiplied.</param>
		/// <param name="vector2">The second vector to be multiplied.</param>
		/// <returns>An int that is the multiplication result in mod 2.</returns>
		private int MultiplyVectors(int[] vector1, int[] vector2)
		{
			if (vector1.GetUpperBound(0) != vector2.GetUpperBound(0))
				throw new ArgumentException("\nVectors have to be the same length!");

			var length = vector1.GetUpperBound(0) + 1;
			var result = 0;

			for (var c = 0; c < length; c++)
				result += vector1[c] * vector2[c];
			result %= 2;

			return result;
		}

		/// <summary>
		/// Check if a given matrix is useable.
		/// </summary>
		/// <param name="matrix">A matrix to be checked.</param>
		/// <returns>'true' if the matrix is proper, 'false' if not.</returns>
		private bool CheckIfProperMatrixGiven(int[][] matrix)
		{
			var length = matrix[0].GetUpperBound(0) + 1;
			var dimension = matrix.GetUpperBound(0) + 1;

			if (length != _cols)
				throw new ArgumentException("\nThe length of vectors in the given matrix " +
				                            "does not match the length provided to the constructor.");

			if (dimension != _rows)
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

		/// <summary>
		/// Separates the standard matrix from the 'other' matrix in the '_matrix' and returns the 'other' matrix.
		/// </summary>
		/// <returns>The 'other' matrix.</returns>
		private int[][] SeparateOtherMatrix()
		{
			// Todo: it will not work if the standard matrix is not at the beginning of the matrix.
			var matrix = new int[_rows][];

			// Since the standard matrix is a square we take the number of columns and subtract the square dimension.
			var numberOfColumns = _cols - _rows;
			var index = 0; // For the new row.

			for (var r = 0; r < _rows; r++)
			{
				var row = new int[numberOfColumns];
				for (var c = _rows; c < _cols; c++)
				{
					row[index] = _matrix[r][c];
					index++;
				}
				matrix[r] = row;
				index = 0;
			}

			return matrix;
		}

		/// <summary>
		/// Twists the matrix in such a way that the rows become columns and vice versa.
		/// </summary>
		/// <param name="matrix">The matrix to be twisted.</param>
		/// <returns>A twisted matrix.</returns>
		private int[][] TwistMatrix(int[][] matrix)
		{
			var rows = matrix.GetUpperBound(0) + 1;
			var cols = matrix[0].GetUpperBound(0) + 1;
			var twisted = new int[cols][];

			for (var c = 0; c < cols; c++)
			{
				var twistedCol = new int[rows];
				for (var r = 0; r < rows; r++)
				{
					twistedCol[r] = matrix[r][c];
				}
				twisted[c] = twistedCol;
			}
			return twisted;
		}

		/// <summary>
		/// Generates a vector of random weight but specified length.
		/// </summary>
		/// <param name="length">Length of the vector to be generated.</param>
		/// <returns>A new vector.</returns>
		private int[] GenerateVector(int length)
		{
			var vector = new int[length];
			var generator = new Random(DateTime.Now.Millisecond);

			for (var c = 0; c < length; c++)
				vector[c] = generator.Next(0, 2);

			return vector;
		}
	}
}
