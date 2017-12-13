using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class MatrixH
	{
		// This is where I'll store coset leaders and their associated weights.
		//		Key - syndrome of coset leaders;
		//		Value - weight of coset leader.
		private readonly Dictionary<string, int> _translations = new Dictionary<string, int>();
		private readonly int[][] _matrix; // The main matrix.
		private readonly int _rows; // Number of rows in '_matrix'.
		private readonly int _cols; // Number of columns in '_matrix'.
		private readonly Random _randomGenerator = new Random();


		// CONSTRUCTOR

		/// <summary>
		/// Returns a new object of type 'MatrixH'
		/// </summary>
		/// <param name="matrix">A matrix to be used as the base.</param>
		public MatrixH(int[][] matrix)
		{
			if (CheckIfProperMatrixGiven(matrix))
			{
				_matrix = matrix;
				_rows = matrix.GetUpperBound(0) + 1;
				_cols = matrix[0].GetUpperBound(0) + 1;

				FillInnerTable();
			}
		}


		// PUBLIC

		/// <summary>
		/// Calculates the syndrome of a given vector.
		/// </summary>
		/// <param name="vector">Vector of which syndrome is to be calculted.</param>
		/// <returns>A syndrome.</returns>
		public int[] GetSyndrome(int[] vector)
		{
			if (vector.GetUpperBound(0) + 1 != _cols)
				throw new ArgumentException("\nThe length of the vector must match the number of columns in the matrix!");

			var syndrome = new int[_rows];
			for (var r = 0; r < _rows; r++)
			{
				var row1 = _matrix[r];
				syndrome[r] = MultiplyVectors(row1, vector);
			}
			return syndrome;
		}

		/// <summary>
		/// Uses the 'Step-by-Step' algorithm to decode a vector.
		/// </summary>
		/// <param name="vector">The vector to be decoded.</param>
		/// <returns>The decoded vector.</returns>
		public int[] Decode(int[] vector)
		{
			// Todo: make a shallow copy of the vector and use that.
			var result = new int[_cols];

			for (var c = 0; c < _cols; c++)
			{
				// Step 1.
				var tuple = new int[_cols];
				tuple[c] = 1;

				// Step 2.
				var syndrome = GetSyndrome(vector);
				var key = string.Join("", syndrome);
				var weight = _translations[key];

				// Step 3.
				if (weight == 0)
				{
					result = vector;
					break;
				}

				// Step 4.
				var changedRow = AddVectors(tuple, vector);
				var newSyndrome = GetSyndrome(changedRow);
				var newKey = string.Join("", newSyndrome);
				var newWeight = _translations[newKey];
				if (newWeight < weight)
				{
					vector = changedRow;
				}
			}

			return vector;
		}

		/// <summary>
		/// Prints out the matrix to the console.
		/// </summary>
		public void DisplayMatrix()
		{
			ConsoleHelper.WriteInformation("The 'H' matrix.");
			ConsoleHelper.WriteMatrix(_matrix);
		}


		// PRIVATE

		/// <summary>
		/// Fills the '_translations' table with coset leaders' syndromes and leaders weights;
		/// </summary>
		private void FillInnerTable()
		{
			// Todo: make it work with any weight.
			var numberOfLeaders = (int) Math.Pow(2, _rows);

			// Generate extra easy leader (weight = 0).
			var easyLeader = new int[_cols];
			var easySyndrome = GetSyndrome(easyLeader);
			_translations.Add(key: string.Join("", easySyndrome), value: GetWeight(easyLeader));

			// Generate easy leaders (weight = 1).

			// How many leaders will we need? count = Math.Pow(2, _rows) + 1
			// How many leaders can we generate of weight 0? count = 1
			// How many leaders can we generate of weight 1? count = _cols 
			// How many leaders can we generate of weight 2? count = Math.Pow(2, _cols - 1) - 1

			for (var c = _cols - 1; c >= 0; c--)
			{
				var leader = new int[_cols];
				leader[c] = 1;
				var syndrome = GetSyndrome(leader);
				_translations.Add(key: string.Join("", syndrome), value: GetWeight(leader));
			}

			// Generate hard leaders (weight > 1).
			while (numberOfLeaders > _translations.Count)
			{
				var leader = GenerateLeader(_cols, 2);
				var syndrome = GetSyndrome(leader);

				// If a randomly generated leader has a new syndrome - we save it.
				var key = string.Join("", syndrome);
				if (!_translations.ContainsKey(key))
					_translations.Add(key, GetWeight(leader));
			}
		}

		/// <summary>
		/// Generates a random vector of required length and weight.
		/// </summary>
		/// <param name="length">Length of the vector.</param>
		/// <param name="weight">Weight of the vector.</param>
		/// <returns>A new leader.</returns>
		private int[] GenerateLeader(int length, int weight)
		{
			var usedIndexes = new List<int>();
			var leader = new int[length];

			while (true)
			{
				// Generate a random index.
				var index = _randomGenerator.Next(0, length);
				// If the index was never used before..
				if (!usedIndexes.Contains(index))
				{
					// ..we use it.
					leader[index] = 1;
					// Mark the index as used.
					usedIndexes.Add(index);
					// Break if the leader has enough '1' in it.
					if (usedIndexes.Count == weight)
						break;
				}
			}

			return leader;
		}

		/// <summary>
		/// Returns the number of '1' found in the given vector.
		/// </summary>
		/// <param name="vector">A vector to be searched through.</param>
		/// <returns>Weight of a vector.</returns>
		private int GetWeight(int[] vector)
		{
			return vector.Count(n => n == 1);
		}

		/// <summary>
		/// Add 2 vectors together using mod 2.
		/// </summary>
		/// <param name="vector1">The first vector to be added.</param>
		/// <param name="vector2">The second vector to be added.</param>
		/// <returns>A vector that is the sum of 'vector1' and 'vector2' in mod 2.</returns>
		private int[] AddVectors(int[] vector1, int[] vector2)
		{
			if (vector1.GetUpperBound(0) != vector2.GetUpperBound(0))
				throw new ArgumentException("\nVectors have to be the same length.");

			var length = vector1.GetUpperBound(0) + 1;
			var result = new int[length];

			for (var c = 0; c < length; c++)
				result[c] = (vector1[c] + vector2[c]) % 2;

			return result;
		}

		/// <summary>
		/// Multiplies 2 vectors together using mod 2.
		/// </summary>
		/// <param name="vector1">The first vector to be multiplied.</param>
		/// <param name="vector2">The second vector to be multiplied.</param>
		/// <returns>An int that is the multiplication result in mod 2.</returns>
		private int MultiplyVectors(int[] vector1, int[] vector2)
		{
			// Todo: currently, the implementation matches the one found in 'MatrixG'.
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
			if (matrix == null)
				throw new ArgumentNullException(nameof(matrix), "\nThe passed in matrix cannot be null!");

			for (var r = 0; r < matrix.GetUpperBound(0) + 1; r++)
			{
				try
				{
					var test = matrix[r];
				}
				catch (Exception)
				{
					throw new ArgumentException("\nThe passed in matrix has null rows!");
				}
			}

			return true;
		}
	}
}
