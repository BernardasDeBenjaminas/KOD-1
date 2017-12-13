using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Logic;

namespace KOD_1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			TestMatrixG();
			TestMatrixH();
			TestDecodingOfCorruptedVectors();

			Console.ReadKey();
		}


		private static void TestMatrixG()
		{
			// Todo: patikrinti, kuomet matrica G = (1 1 1).

			// G = 1 0 1 1 0
			//     0 1 0 1 1
			var matrix = new int[2][];
			matrix[0] = new int[5] {1,0,1,1,0};
			matrix[1] = new int[5] {0,1,0,1,1};

			var matrixG = new MatrixG(length: matrix[0].GetUpperBound(0) + 1, 
									  dimension: matrix.GetUpperBound(0) + 1, 
									  matrix: matrix);

			// ENCODING

			var toEncode1 = new int[2] {0,0};
			var toEncode2 = new int[2] {0,1};
			var toEncode3 = new int[2] {1,0};
			var toEncode4 = new int[2] {1,1};

			var encoded1 = string.Join("", matrixG.Encode(toEncode1));
			var encoded2 = string.Join("", matrixG.Encode(toEncode2));
			var encoded3 = string.Join("", matrixG.Encode(toEncode3));
			var encoded4 = string.Join("", matrixG.Encode(toEncode4));

			var encoded = new List<string> {encoded1, encoded2, encoded3, encoded4};
			var outcome = new List<string> {"00000", "01011", "10110", "11101"};
			for (var i = 0; i < encoded.Count; i++)
				if (encoded[i] != outcome[i])
					ConsoleHelper.WriteError("'MatrixG' encodes vectors improperly.");

			// DECODING

			var toDecode1 = new int[5] {0,0,0,0,0};
			var toDecode2 = new int[5] {0,1,0,1,1};
			var toDecode3 = new int[5] {1,0,1,1,0};
			var toDecode4 = new int[5] {1,1,1,0,1};

			var decode1 = string.Join("", matrixG.Decode(toDecode1));
			var decode2 = string.Join("", matrixG.Decode(toDecode2));
			var decode3 = string.Join("", matrixG.Decode(toDecode3));
			var decode4 = string.Join("", matrixG.Decode(toDecode4));

			var decoded = new List<string> {decode1, decode2, decode3, decode4};
			outcome = new List<string> {"00", "01", "10", "11"};
			for (var i = 0; i < decoded.Count; i++)
				if (decoded[i] != outcome[i])
					ConsoleHelper.WriteError("'MatrixG' decodes vectors improperly.");

			
			//var matrixH = matrixG.GetMatrixH();
			//matrixG.DisplayMatrix();
			//matrixH.DisplayMatrix();
		}

		private static void TestMatrixH()
		{
			// G = 1 1 0 1 0 0
			//     0 1 1 0 1 0
			//     1 0 1 0 0 1
			var matrix = new int[3][];
			matrix[0] = new int[6] {1,1,0,1,0,0};
			matrix[1] = new int[6] {0,1,1,0,1,0};
			matrix[2] = new int[6] {1,0,1,0,0,1};

			var matrixG = new MatrixG(
				length: matrix[0].GetUpperBound(0) + 1,
				dimension: matrix.GetUpperBound(0) + 1,
				matrix: matrix);

			// H = 1 0 0 1 0 1
			//     0 1 0 1 1 0
			//     0 0 1 0 1 1
			//var matrixH = matrixG.GetMatrixH();
			matrix = new int[3][];
			matrix[0] = new int[6] {1,0,0,1,0,1};
			matrix[1] = new int[6] {0,1,0,1,1,0};
			matrix[2] = new int[6] {0,0,1,0,1,1};

			// Todo: the GetMatrixH() cannot properly convert from this 'G'.
			var matrixH = new MatrixH(matrix);

			matrixG.DisplayMatrix();
			matrixH.DisplayMatrix();

			var toSyndrome1 = new int[6] {0,0,0,0,0,0};
			var toSyndrome2 = new int[6] {0,0,0,0,0,1};
			var toSyndrome3 = new int[6] {0,0,0,0,1,0};
			var toSyndrome4 = new int[6] {0,0,0,1,0,0};
			var toSyndrome5 = new int[6] {0,0,1,0,0,0};
			var toSyndrome6 = new int[6] {0,1,0,0,0,0};
			var toSyndrome7 = new int[6] {1,0,0,0,0,0};
			var toSyndrome8 = new int[6] {0,0,1,1,0,0};

			var syndrome1 = string.Join("", matrixH.GetSyndrome(toSyndrome1));
			var syndrome2 = string.Join("", matrixH.GetSyndrome(toSyndrome2));
			var syndrome3 = string.Join("", matrixH.GetSyndrome(toSyndrome3));
			var syndrome4 = string.Join("", matrixH.GetSyndrome(toSyndrome4));
			var syndrome5 = string.Join("", matrixH.GetSyndrome(toSyndrome5));
			var syndrome6 = string.Join("", matrixH.GetSyndrome(toSyndrome6));
			var syndrome7 = string.Join("", matrixH.GetSyndrome(toSyndrome7));
			var syndrome8 = string.Join("", matrixH.GetSyndrome(toSyndrome8));

			var syndromes = new List<string> { syndrome1, syndrome2, syndrome3, syndrome4, syndrome5, syndrome6, syndrome7, syndrome8 };
			var outcomes = new List<string> { "000", "101", "011", "110", "001", "010", "100", "111" };
			for (var i = 0; i < syndromes.Count; i++)
				if (syndromes[i] != outcomes[i])
					ConsoleHelper.WriteError("'MatrixH' calculates syndromes improperly.");


			var toEncode1 = new int[] {0,0,0};
			var toEncode2 = new int[] {1,0,0};
			var toEncode3 = new int[] {0,1,0};
			var toEncode4 = new int[] {0,0,1};
			var toEncode5 = new int[] {1,1,0};
			var toEncode6 = new int[] {0,1,1};
			var toEncode7 = new int[] {1,0,1};
			var toEncode8 = new int[] {1,1,1};

			var encoded1 = string.Join("", matrixG.Encode(toEncode1));
			var encoded2 = string.Join("", matrixG.Encode(toEncode2));
			var encoded3 = string.Join("", matrixG.Encode(toEncode3));
			var encoded4 = string.Join("", matrixG.Encode(toEncode4));
			var encoded5 = string.Join("", matrixG.Encode(toEncode5));
			var encoded6 = string.Join("", matrixG.Encode(toEncode6));
			var encoded7 = string.Join("", matrixG.Encode(toEncode7));
			var encoded8 = string.Join("", matrixG.Encode(toEncode8));
			var encoded = new List<string> { encoded1, encoded2, encoded3, encoded4, encoded5, encoded6, encoded7, encoded8 };
			outcomes = new List<string> { "000000", "110100", "011010", "101001", "101110", "110011", "011101", "000111" };
			for (var i = 0; i < encoded.Count; i++)
				if (encoded[i] != outcomes[i])
					ConsoleHelper.WriteError("'MatrixG' encodes vectors improperly.");


			var decoded1 = string.Join("", matrixG.Decode(matrixH.Decode(matrixG.Encode(toEncode1))));
			var decoded2 = string.Join("", matrixG.Decode(matrixH.Decode(matrixG.Encode(toEncode2))));
			var decoded3 = string.Join("", matrixG.Decode(matrixH.Decode(matrixG.Encode(toEncode3))));
			var decoded4 = string.Join("", matrixG.Decode(matrixH.Decode(matrixG.Encode(toEncode4))));
			var decoded5 = string.Join("", matrixG.Decode(matrixH.Decode(matrixG.Encode(toEncode5))));
			var decoded6 = string.Join("", matrixG.Decode(matrixH.Decode(matrixG.Encode(toEncode6))));
			var decoded7 = string.Join("", matrixG.Decode(matrixH.Decode(matrixG.Encode(toEncode7))));
			var decoded8 = string.Join("", matrixG.Decode(matrixH.Decode(matrixG.Encode(toEncode8))));
			var decoded = new List<string> { decoded1, decoded2, decoded3, decoded4, decoded5, decoded6, decoded7, decoded8 };
			outcomes = new List<string> { "000", "100" , "010", "001", "110", "011", "101", "111" };
			for (var i = 0; i < decoded.Count; i++)
				if (decoded[i] != outcomes[i])
					ConsoleHelper.WriteError("'MatrixH' decodes vectors improperly.");
		}

		private static void TestDecodingOfCorruptedVectors()
		{
			// G = 1 1 0 1 0 0
			//     0 1 1 0 1 0
			//     1 0 1 0 0 1
			var matrix = new int[3][];
			matrix[0] = new int[6] { 1, 1, 0, 1, 0, 0 };
			matrix[1] = new int[6] { 0, 1, 1, 0, 1, 0 };
			matrix[2] = new int[6] { 1, 0, 1, 0, 0, 1 };

			var matrixG = new MatrixG(
				length: matrix[0].GetUpperBound(0) + 1,
				dimension: matrix.GetUpperBound(0) + 1,
				matrix: matrix);

			// H = 1 0 0 1 0 1
			//     0 1 0 1 1 0
			//     0 0 1 0 1 1
			matrix = new int[3][];
			matrix[0] = new int[6] { 1, 0, 0, 1, 0, 1 };
			matrix[1] = new int[6] { 0, 1, 0, 1, 1, 0 };
			matrix[2] = new int[6] { 0, 0, 1, 0, 1, 1 };

			var matrixH = new MatrixH(matrix);

			// 1. Word to encode
			var clean = new int[3] {0,1,0};
			var dirty = new int[3] {0,1,0};
			// 2. Encoded word with 'G'
			var encoded = matrixG.Encode(clean);   // Encodes to 011010
			var corrupted = matrixG.Encode(dirty); // Encodes to 011010
			// 3. Corrupt it
			corrupted[5] = 1;
			// 4. Decode it
			var decoded = matrixH.Decode(corrupted);

			if (string.Join("", encoded) != string.Join("", decoded))
				ConsoleHelper.WriteError($"{string.Join("", encoded)} does not equal {string.Join("", decoded)}.");
			else
				ConsoleHelper.WriteInformation("Matches.");
		}
	}
}
