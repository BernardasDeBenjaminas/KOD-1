using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class MatrixH
	{
		private static int[][] _matrix;
		private readonly int _rows; // Number of rows in '_matrix'.
		private readonly int _cols; // Number of columns in '_matrix'.
		private readonly Random _randomGenerator;

		// The main table:
		//	string	- leader's syndrome;
		//	int		- leader's weight.
		public Dictionary<string, int> _table;

		public MatrixH(int[][] matrix)
		{
			_matrix = matrix;
			_rows = _matrix[0].GetUpperBound(0);
			_cols = _matrix.GetUpperBound(0);
			_randomGenerator = new Random(DateTime.Now.Millisecond);
			FillTable();
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

		/// <summary>
		/// Generates coset leaders, calculates their syndromes and weights and fills the internal '_table' with acquired information.
		/// </summary>
		private void FillTable()
		{
			var numberOfSyndromes = (int) Math.Pow(2, _rows + 1);

			// Generate coset leaders of weight 1.
			GenerateCosetLeadersOne(numberOfSyndromes, out var array, out var syndromes);

			// Generate the rest of coset leaders (weight > 1).
			// Todo: what about weight 3?
			// An extra + 1 because we have to count in the zero vector.
			var index = _cols + 1 + 1; 
			for (; index < numberOfSyndromes; index++)
			{
				// Generate a leader of weight 2.
				var generatedLeader = GenerateCosetLeader(_cols + 1, 2);
				// Calculate its syndrome.
				var syndrome = string.Join("", CalculateSyndrome(generatedLeader));
				// If we don't already have it - we store the syndrome.
				if (!syndromes.Contains(syndrome))
				{
					syndromes.Add(syndrome);
					// Add the syndrome's leader.
					array[index] = generatedLeader;
					// Stop the cycle if we have enough syndromes.
					if (syndromes.Count == numberOfSyndromes)
						break;
				}
				else
				{
					index--;
				}
			}

			// Fill the <syndrome, weight> table.
			FillTheTable(array, syndromes);
		}

		/// <summary>
		/// Fills the internal '_table' with syndromes and weights.
		/// </summary>
		/// <param name="leaders">An uninitialized 2D array for storing leaders.</param>
		/// <param name="syndromes">An uninitialized list of strings for storing syndromes.</param>
		private void FillTheTable(int[][] leaders, List<string> syndromes)
		{
			_table = new Dictionary<string, int>();

			for (var i = 0; i < syndromes.Count; i++)
				_table.Add(syndromes[i], CalculateWeight(leaders[i]));
		}

		/// <summary>
		/// Generates a row of numbers of specified length and weight.
		/// Example: if length is 4 and weight is 1, the function may return 0010.
		/// Example: if length is 5 and weight is 2, the function may return 10010.
		/// </summary>
		/// <param name="length">Length of the leader.</param>
		/// <param name="weight">How many '1' does the leader have to have.</param>
		/// <returns></returns>
		private int[] GenerateCosetLeader(int length, int weight)
		{
			var array = new int[length];
			var positionsUsed = new List<int>(); // Used to keep track in which positions we already inserted '1'.
			var onesUsed = 0; // Used to count how many '1' we insert.

			for (var i = 0; i < length; i++)
			{
				// Generate a random position.
				var position = _randomGenerator.Next(0, length);
				if (positionsUsed.Contains(position))
					continue;

				// Insert the '1' at a randomly generated position.
				array[position] = 1;
				// Mark the position as used.
				positionsUsed.Add(position); 
				// Increment the number of '1' used.
				onesUsed++;

				// If we inserted enough '1'.
				if (onesUsed == weight)
					break;

				// If the cycle has ended but we did not insert enough '1'.
				if (i == length - 2 && onesUsed != weight)
					i = 0;
			}

			return array;
		}

		/// <summary>
		/// Generates coset leaders of up to weight 1. Stores them in the 'array' variable and their syndromes in 'syndromes'.
		/// </summary>
		/// <param name="numberOfSyndromes">The total number of syndromes including other weights.</param>
		/// <param name="array">Coset leaders will be placed here.</param>
		/// <param name="syndromes">Coset leader syndromes will be placed here.</param>
		private void GenerateCosetLeadersOne(int numberOfSyndromes, out int[][] array, out List<string> syndromes)
		{
			syndromes = new List<string>();
			array = new int[numberOfSyndromes][];

			// The first element must always be a zero vector!
			array[0] = new int[_cols + 1]; 
			// Add the zero vector's syndrome to the list.
			syndromes.Add(string.Join("", CalculateSyndrome(array[0])));
			// Because the first position was used for the zero vector.
			var index = 1; 

			for (; index <= _cols + 1; index++)
			{
				var leader = new int[_cols + 1];
				// Insert the '1' in an array made up of only zeroes.
				leader[index - 1] = 1; 
				// Calculate the syndrome and add it.
				var syndrome = string.Join("", CalculateSyndrome(leader));
				syndromes.Add(syndrome); 
				// Store the leader.
				array[index] = leader; 
			}
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
