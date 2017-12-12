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
			Test0(); // Test the creation of 'matrixG'.
			//Test1(); // Test syndromes.
			//Test2(); // Test encoding and decoding.

			Console.ReadKey();
		}

		//private static void Test1()
		//{
		//	Console.WriteLine();
		//	ConsoleHelper.WriteInformation($"--- Started testing in {nameof(Test1)}.");

		//	var results = new List<string>();
		//	var supposed = new List<string>();

		//	// new G = 1 0 1 1 0
		//	//         0 1 0 1 1
		//	//
		//	var matrix = new int[5][];
		//	matrix[0] = new int[2] { 1, 0 };
		//	matrix[1] = new int[2] { 0, 1 };
		//	matrix[2] = new int[2] { 1, 0 };
		//	matrix[3] = new int[2] { 1, 1 };
		//	matrix[4] = new int[2] { 0, 1 };

		//	var matrixG = new MatrixG(matrix);
		//	//matrixG.Display();

		//	// H = 1 0 1 0 0
		//	//     1 1 0 1 0
		//	//     0 1 0 0 1
		//	var newMatrixH = matrixG.GetMatrixH();
		//	//newMatrixH.Display();

		//	// Test leaders = 0 0 0 0 0
		//	//                0 0 0 0 1
		//	//                0 0 0 1 0
		//	//                0 0 1 0 0
		//	//                0 1 0 0 0
		//	//                1 0 0 0 0
		//	//                0 1 1 0 0
		//	var leader1 = new int[5] { 0, 0, 0, 0, 0 };
		//	var leader2 = new int[5] { 0, 0, 0, 0, 1 };
		//	var leader3 = new int[5] { 0, 0, 0, 1, 0 };
		//	var leader4 = new int[5] { 0, 0, 1, 0, 0 };
		//	var leader5 = new int[5] { 0, 1, 0, 0, 0 };
		//	var leader6 = new int[5] { 1, 0, 0, 0, 0 };
		//	var leader7 = new int[5] { 0, 1, 1, 0, 0 };
		//	var leader8 = new int[5] { 1, 1, 0, 0, 0 };



		//	// Output should be
		//	// Syndromes = 0 0 0
		//	//			   0 0 1
		//	//			   0 1 0
		//	//			   1 0 0
		//	//			   0 1 1
		//	//			   1 1 0
		//	//			   1 1 1
		//	//			   1 0 1
		//	var syndrome1 = newMatrixH.CalculateSyndrome(leader1);
		//	var syndrome2 = newMatrixH.CalculateSyndrome(leader2);
		//	var syndrome3 = newMatrixH.CalculateSyndrome(leader3);
		//	var syndrome4 = newMatrixH.CalculateSyndrome(leader4);
		//	var syndrome5 = newMatrixH.CalculateSyndrome(leader5);
		//	var syndrome6 = newMatrixH.CalculateSyndrome(leader6);
		//	var syndrome7 = newMatrixH.CalculateSyndrome(leader7);
		//	var syndrome8 = newMatrixH.CalculateSyndrome(leader8);

		//	var result1 = string.Join("", syndrome1);
		//	var result2 = string.Join("", syndrome2);
		//	var result3 = string.Join("", syndrome3);
		//	var result4 = string.Join("", syndrome4);
		//	var result5 = string.Join("", syndrome5);
		//	var result6 = string.Join("", syndrome6);
		//	var result7 = string.Join("", syndrome7);
		//	var result8 = string.Join("", syndrome8);

		//	results.Clear();
		//	results = new List<string> { result1, result2, result3, result4, result5, result6, result7,result8 };
		//	supposed.Clear();
		//	supposed = new List<string> { "000", "001" ,"010" ,"100", "011" ,"110" ,"111", "101" };

		//	ConsoleHelper.WriteWarning("Testing syndromes..");

		//	for (var i = 0; i < results.Count; i++)
		//	{
		//		if (results[i] != supposed[i])
		//			ConsoleHelper.WriteError($"Failed test in {nameof(Test1)} - Syndromes are not calculated properly.");
		//	}

		//	ConsoleHelper.WriteInformation($"--- Finished testing in {nameof(Test1)}.");
		//}

		//private static void Test2()
		//{
		//	Console.WriteLine();
		//	ConsoleHelper.WriteInformation($"--- Started testing in {nameof(Test2)}.");
			
		//	var matrix = new int[5][];
		//	matrix[0] = new int[2] { 1, 0 };
		//	matrix[1] = new int[2] { 0, 1 };
		//	matrix[2] = new int[2] { 1, 0 };
		//	matrix[3] = new int[2] { 1, 1 };
		//	matrix[4] = new int[2] { 0, 1 };

		//	// G = 1 0 1 1 0
		//	//     0 1 0 1 1
		//	var matrixG = new MatrixG(matrix);
		//	//matrixG.Display();

		//	var messageToEncode1 = new int[2] {0,0};
		//	var messageToEncode2 = new int[2] {1,0};
		//	var messageToEncode3 = new int[2] {0,1};
		//	var messageToEncode4 = new int[2] {1,1};

		//	var encodedMessage1 = matrixG.Encode(messageToEncode1);
		//	var encodedMessage2 = matrixG.Encode(messageToEncode2);
		//	var encodedMessage3 = matrixG.Encode(messageToEncode3);
		//	var encodedMessage4 = matrixG.Encode(messageToEncode4);

		//	var encoded1 = string.Join("", encodedMessage1);
		//	var encoded2 = string.Join("", encodedMessage2);
		//	var encoded3 = string.Join("", encodedMessage3);
		//	var encoded4 = string.Join("", encodedMessage4);

		//	var results = new List<string> { encoded1, encoded2, encoded3, encoded4 };
		//	var supposed = new List<string> { "00000", "10110", "01011", "11101" };

		//	ConsoleHelper.WriteWarning("Testing encoding..");

		//	for (var i = 0; i < results.Count; i++)
		//	{
		//		if (results[i] != supposed[i])
		//			ConsoleHelper.WriteError($"Failed test in {nameof(Test2)} - Messages are not encoded properly.");
		//	}


		//	// H = 1 0 1 0 0
		//	//     1 1 0 1 0
		//	//     0 1 0 0 1
		//	var matrixH = matrixG.GetMatrixH();
		//	//matrixH.Display();

		//	var decodedMessage1 = matrixH.Decode(encodedMessage1);
		//	var decodedMessage2 = matrixH.Decode(encodedMessage2);
		//	var decodedMessage3 = matrixH.Decode(encodedMessage3);
		//	var decodedMessage4 = matrixH.Decode(encodedMessage4);

		//	var decoded1 = string.Join("", decodedMessage1);
		//	var decoded2 = string.Join("", decodedMessage2);
		//	var decoded3 = string.Join("", decodedMessage3);
		//	var decoded4 = string.Join("", decodedMessage4);

		//	results.Clear();
		//	results = new List<string>{ decoded1, decoded2, decoded3, decoded4 };

		//	ConsoleHelper.WriteWarning("Testing decoding..");

		//	for (var i = 0; i < results.Count; i++)
		//	{
		//		if (results[i] != supposed[i])
		//			ConsoleHelper.WriteError($"Failed test in {nameof(Test2)} - Messages are not decoded properly.");
		//	}

		//	ConsoleHelper.WriteInformation($"--- Finished testing in {nameof(Test2)}.");
		//}

		private static void Test0()
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

			
			var matrixH = matrixG.GetMatrixH();
			matrixG.DisplayMatrix();
			matrixH.DisplayMatrix();
		}
	}
}
