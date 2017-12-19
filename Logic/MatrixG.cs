using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class MatrixG
	{
		public readonly int[][] Matrix; // Pagrindinė matrica.
		private readonly int _rows;	    // 'Matrix' dimensija (k).
		private readonly int _cols;     // 'Matrix' ilgis (n).

		/// <summary>
		/// Grąžina generuojančią matricą (viduje jau yra standartinė matrica).
		/// </summary>
		/// <param name="length">Matricos ilgis.</param>
		/// <param name="dimension">Matricos dimensija.</param>
		/// <param name="matrix">Vartotojo norima matrica naudojimui (jeigu nebus pateikta - kodas pats sugeneruos ją).</param>
		public MatrixG(int length, int dimension, int[][] matrix = null)
		{
			if (length < 2)
				throw new ArgumentException("\nMatricos ilgis privalo būti 2 arba daugiau.");

			if (dimension < 1)
				throw new ArgumentException("\nMatricos dimensija privalo būti 1 arba daugiau.");

			if (length == dimension)
				throw new ArgumentException("\nMatricos ilgis negali sutapti sutapti su jos dimensija.");

			if (length < dimension)
				throw new ArgumentException("\nMatricos ilgis negali būti mažesnis už jos dimensiją.");

			_rows = dimension;
			_cols = length;

			if (matrix != null && CheckIfProperMatrixGiven(matrix))
				Matrix = matrix;
			else
				Matrix = GenerateMatrix(length, dimension);
		}


		// PUBLIC 
		/// <summary>
		/// Užkoduoja vektorių pasinaudodamas generuojančia matrica.
		/// </summary>
		/// <param name="vector">Vektorius, kurį reikia užkoduoti.</param>
		/// <returns>Generuojančia matrica užkoduotas vektorius.</returns>
		public int[] Encode(int[] vector)
		{
			if (vector.GetUpperBound(0) + 1 != _rows)
				throw new ArgumentException("\nVektoriaus ilgis privalo sutapti su matricos dimensija.");

			var result = new int[_cols];
			for (var c = 0; c < _cols; c++)
			{
				var matrixCol = GetColumnAsVector(Matrix, c);
				result[c] = Field.Multiply(matrixCol, vector);
			}
			return result;
		}

		/// <summary>
		/// Dekoduoja vektorių.
		/// </summary>
		/// <param name="vector">Vektorius, kurį reikia dekoduoti.</param>
		/// <returns>Generuojančia matrica dekoduotas vektorius.</returns>
		public int[] Decode(int[] vector)
		{
			if (vector.GetUpperBound(0) + 1 != _cols)
				throw new ArgumentException("\nPateikto vektoriaus ilgis privalo sutapti su matricos ilgiu.");

			var decoded = new int[_rows];

			// Kokia esamos matricos dimensija, tiek ir paimame bitų.
			for (var c = 0; c < _rows; c++)
				decoded[c] = vector[c];

			return decoded;
		}

		/// <summary>
		/// Grąžina iš generuojančios matricos gautą kontrolinę (H) matricą.
		/// </summary>
		/// <returns>Grąžina kontrolinę matricą.</returns>
		public MatrixH GetMatrixH()
		{
			var standardMatrix = GenerateStandardMatrix(_cols - _rows);
			var separatedMatrix = SeparateOtherMatrix();
			var twistedMatrix = TwistMatrix(separatedMatrix);
			var combinedMatrix = CombineMatrices(twistedMatrix, standardMatrix);
			return new MatrixH(combinedMatrix);
		}


		// PRIVATE
		/// <summary>
		/// Patikrina ar pateiktą matricą galima paversti į kontrolinę matricą.
		/// ! Standartinė matrica privalo prasidėti su pirmuoju stulpeliu !
		/// </summary>
		/// <param name="matrix">Matrica, kurią reikia patikrinti.</param>
		/// <returns>Grąžina 'true' jeigu standarinė matrica yra pačioje pradžioje arba pačioje pabaigoje - antraip 'false'.</returns>
		private bool IsTransformableToMatrixH(int[][] matrix)
		{
			var numberOfCols = matrix[0].GetUpperBound(0) + 1;
			var numberOfRows = matrix.GetUpperBound(0) + 1;
			var result = false;
			// Seksime kurioje stulpelio vietoje yra vienetas.
			var position = 0;

			for (var c = 0; c < numberOfCols; c++)
			{
				var column = GetColumnAsVector(matrix, c);
				if (VectorContainsOnlyOne(column, position))
				{
					// Kitame stulpelyje '1' turės būti eilute žemiau.
					position++;

					if (position == numberOfRows)
					{
						result = true;
						break;
					}
				}

				else
					break;
			}
			return result;
		}

		/// <summary>
		/// Patikrina ar vartotojo pateikta matrica yra tinkama naudojimui.
		/// </summary>
		/// <param name="matrix">Matrica, kurią reikia patikrinti.</param>
		/// <returns>Grąžina 'true' jeigu matrica tinkanti - antraip 'false'.</returns>
		private bool CheckIfProperMatrixGiven(int[][] matrix)
		{
			var length = matrix[0].GetUpperBound(0) + 1;
			var dimension = matrix.GetUpperBound(0) + 1;

			if (length != _cols)
				throw new ArgumentException("\nPaduotos matricos ilgis nesutampa su paduotu matricos ilgio argumentu konstruktoriui.");

			if (dimension != _rows)
				throw new ArgumentException("\nPaduotos matricos dimensija nesutampa su paduotu dimensijos argumentu konstruktoriui.");

			for (var r = 0; r < dimension; r++)
			{
				try
				{
					var test = matrix[r];
				}
				catch (Exception)
				{
					throw new ArgumentException("\nPaduota matrica turi neinicializuotų (null) eilučių.");
				}
			}

			if (!IsTransformableToMatrixH(matrix))
				throw new ArgumentException("\nNepavyko surasti pateiktoje matricoje standartinės formos matricos." +
				                            "\nPatikrinkite ar ji prasideda nuo kairiausio stulpelio.");

			return true;
		}

		/// <summary>
		/// Sugeneruoja matricą nurodytų matmenų (su viduje esančia standartine matrica).
		/// </summary>
		/// <param name="length">Matricos ilgis.</param>
		/// <param name="dimension">Matricos dimensija</param>
		/// <returns>Sugeneruotą matricą.</returns>
		private int[][] GenerateMatrix(int length, int dimension)
		{
			var standardMatrix = GenerateStandardMatrix(dimension);
			var randomMatrix = GenerateRandomMatrix(rows: dimension, cols: length - dimension);
			return CombineMatrices(standardMatrix, randomMatrix);
		}

		/// <summary>
		/// Sujungia dvi matricas į vieną.
		/// </summary>
		/// <param name="matrix1">Pirmoji matrica sujungimui.</param>
		/// <param name="matrix2">Antroji matrica sujungimui.</param>
		/// <returns>Matrica, gauta sujungus pateiktas matricas.</returns>
		private int[][] CombineMatrices(int[][] matrix1, int[][] matrix2)
		{
			if (matrix1.GetUpperBound(0) != matrix2.GetUpperBound(0))
				throw new ArgumentException("\nMatricos dimensijos turi sutapti.");

			var numberOfRows = matrix1.GetUpperBound(0) + 1;
			var numberOfCols = matrix1[0].GetUpperBound(0) + 1 + matrix2[0].GetUpperBound(0) + 1;
			var matrix = new int[numberOfRows][];

			for (var r = 0; r < numberOfRows; r++)
			{
				var row = new int[numberOfCols];
				var colIndex = 0;

				// Kopijuojame reikšmes iš pirmos matricos.
				for (var c1 = 0; c1 <= matrix1[0].GetUpperBound(0); c1++)
				{
					row[colIndex] = matrix1[r][c1];
					colIndex++;
				}
				// Kopijuojame reikšmes iš antros matricos.
				for (var c2 = 0; c2 <= matrix2[0].GetUpperBound(0); c2++)
				{
					row[colIndex] = matrix2[r][c2];
					colIndex++;
				}
				
				matrix[r] = row;
			}

			return matrix;
		}

		/// <summary>
		/// Sugeneruoja atsitiktinę matricą nurodytų matmenų.
		/// </summary>
		/// <param name="rows">Matricos eilučių skaičius.</param>
		/// <param name="cols">Matricos stulpelių skaičius.</param>
		/// <returns>Sugeneruota matrica.</returns>
		private int[][] GenerateRandomMatrix(int rows, int cols)
		{
			var randomGenerator = new Random(DateTime.Now.Millisecond);
			var matrix = new int[rows][];

			for (var r = 0; r < rows; r++)
			{
				var vector = new int[cols];

				for (var c = 0; c < cols; c++)
					vector[c] = randomGenerator.Next(0, 2);

				matrix[r] = vector;
			}

			return matrix;
		}

		/// <summary>
		/// Sugeneruoja nurodyto dydžio standartinę matricą.
		/// </summary>
		/// <param name="size">Matricos eilučių/stulpelių skaičius.</param>
		/// <returns>Sugeneruotą standartinę matricą.</returns>
		private int[][] GenerateStandardMatrix(int size)
		{
			var matrix = new int[size][];

			for (var r = 0; r < size; r++)
			{
				var row = new int[size];
				row[r] = 1;
				matrix[r] = row;
			}

			return matrix;
		}

		/// <summary>
		/// Patikrina ar duotame vektoriuje yra tik vienas '1' ir ar jis nurodytoje pozicijoje.
		/// </summary>
		/// <param name="vector">Vektorius, kurį reikia patikrinti.</param>
		/// <param name="position">Pozicija, kurioje turėtų būti '1'.</param>
		/// <returns>Grąžina 'true' jeigu tik nurodytoje pozicijoje yra '1' - antraip 'false'.</returns>
		private bool VectorContainsOnlyOne(int[] vector, int position)
		{
			// Patikrina ar yra tik vienas '1'.
			if (vector.Count(n => n == 1) != 1)
				return false;

			// Patikrina ar jis nurodytoje pozicijoje.
			return vector[position] == 1;
		}

		/// <summary>
		/// Grąžina iš nurodytos matricos nurodytą stulpelį.
		/// </summary>
		/// <param name="matrix">Matrica iš kurios reikės išimti stulpelį.</param>
		/// <param name="columnNumber">Numeris norimo stulpelio (jeigu reikia pirmojo, paduodate 0).</param>
		/// <returns>Nurodytą stulpelį kaip vektorių.</returns>
		private int[] GetColumnAsVector(int[][] matrix, int columnNumber)
		{
			var numberOfRows = matrix.GetUpperBound(0) + 1;
			var column = new int[numberOfRows];

			for (var r = 0; r < numberOfRows; r++)
				column[r] = matrix[r][columnNumber];

			return column;
		}

		/// <summary>
		/// Atskiria standartinę matricą nuo 'kitos' ir grąžina ją (tą 'kitą').
		/// </summary>
		/// <returns>Ne standartinę matricos dalį.</returns>
		private int[][] SeparateOtherMatrix()
		{
			var matrix = new int[_rows][];

			// Kadangi standartinės matricos k = n, tai mes žinome, jog standartinė matrica pasibaigia k stulpelyje.
			var numberOfColumns = _cols - _rows;
			var index = 0;

			for (var r = 0; r < _rows; r++)
			{
				var row = new int[numberOfColumns];
				for (var c = _rows; c < _cols; c++)
				{
					row[index] = Matrix[r][c];
					index++;
				}
				matrix[r] = row;
				index = 0;
			}

			return matrix;
		}

		/// <summary>
		/// Transponuoja matricą.
		/// Stulpeliai tampa eilutėmis ir atvirkščiai.
		/// </summary>
		/// <param name="matrix">Matricą, kurią reikia transponuoti.</param>
		/// <returns>Transponuota matrica.</returns>
		private int[][] TwistMatrix(int[][] matrix)
		{
			var rows = matrix.GetUpperBound(0) + 1;
			var cols = matrix[0].GetUpperBound(0) + 1;
			var twisted = new int[cols][];

			for (var c = 0; c < cols; c++)
			{
				var twistedCol = new int[rows];

				for (var r = 0; r < rows; r++)
					twistedCol[r] = matrix[r][c];

				twisted[c] = twistedCol;
			}
			return twisted;
		}
	}
}
