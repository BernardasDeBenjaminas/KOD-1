using System;
using System.Collections.Generic;
using Logic;

namespace KOD_1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Test1();
			Test2();

			Console.ReadKey();
		}

		private static void Test1()
		{
			Console.WriteLine();
			ConsoleHelper.WriteInformation($"--- Started testing in {nameof(Test1)}.");

			var results = new List<string>();
			var supposed = new List<string>();

			// new G = 1 0 1 1 0
			//         0 1 0 1 1
			//
			var matrix = new int[5][];
			matrix[0] = new int[2] { 1, 0 };
			matrix[1] = new int[2] { 0, 1 };
			matrix[2] = new int[2] { 1, 0 };
			matrix[3] = new int[2] { 1, 1 };
			matrix[4] = new int[2] { 0, 1 };

			Console.WriteLine("Printing G matrix.");
			var matrixG = new MatrixG(matrix);
			matrixG.Display();

			// H = 1 0 1 0 0
			//     1 1 0 1 0
			//     0 1 0 0 1
			Console.WriteLine("Printing H matrix.");
			var newMatrixH = matrixG.GetMatrixH();
			newMatrixH.Display();

			// Test leaders = 0 0 0 0 0
			//                0 0 0 0 1
			//                0 0 0 1 0
			//                0 0 1 0 0
			//                0 1 0 0 0
			//                1 0 0 0 0
			//                0 1 1 0 0
			var leader1 = new int[5] { 0, 0, 0, 0, 0 };
			var leader2 = new int[5] { 0, 0, 0, 0, 1 };
			var leader3 = new int[5] { 0, 0, 0, 1, 0 };
			var leader4 = new int[5] { 0, 0, 1, 0, 0 };
			var leader5 = new int[5] { 0, 1, 0, 0, 0 };
			var leader6 = new int[5] { 1, 0, 0, 0, 0 };
			var leader7 = new int[5] { 0, 1, 1, 0, 0 };
			var leader8 = new int[5] { 1, 1, 0, 0, 0 };



			// Output should be
			// Syndromes = 0 0 0
			//			   0 0 1
			//			   0 1 0
			//			   1 0 0
			//			   0 1 1
			//			   1 1 0
			//			   1 1 1
			//			   1 0 1
			var syndrome1 = newMatrixH.CalculateSyndrome(leader1);
			var syndrome2 = newMatrixH.CalculateSyndrome(leader2);
			var syndrome3 = newMatrixH.CalculateSyndrome(leader3);
			var syndrome4 = newMatrixH.CalculateSyndrome(leader4);
			var syndrome5 = newMatrixH.CalculateSyndrome(leader5);
			var syndrome6 = newMatrixH.CalculateSyndrome(leader6);
			var syndrome7 = newMatrixH.CalculateSyndrome(leader7);
			var syndrome8 = newMatrixH.CalculateSyndrome(leader8);

			var result1 = string.Join("", syndrome1);
			var result2 = string.Join("", syndrome2);
			var result3 = string.Join("", syndrome3);
			var result4 = string.Join("", syndrome4);
			var result5 = string.Join("", syndrome5);
			var result6 = string.Join("", syndrome6);
			var result7 = string.Join("", syndrome7);
			var result8 = string.Join("", syndrome8);

			results.Clear();
			results = new List<string> { result1, result2, result3, result4, result5, result6, result7,result8 };
			supposed.Clear();
			supposed = new List<string> { "000", "001" ,"010" ,"100", "011" ,"110" ,"111", "101" };

			for (var i = 0; i < results.Count; i++)
			{
				if (results[i] != supposed[i])
					ConsoleHelper.WriteError($"Failed test in {nameof(Test1)} - syndromes do not match.");
			}

			ConsoleHelper.WriteInformation($"--- Finished testing in {nameof(Test1)}.");
		}

		private static void Test2()
		{
			//Console.WriteLine();
			//ConsoleHelper.WriteInformation($"--- Started testing in {nameof(Test2)}.");

			//// G = 1 0 1 1 0
			////     0 1 0 1 1
			//var matrix = new int[5][];
			//matrix[0] = new int[2] { 1, 0 };
			//matrix[1] = new int[2] { 0, 1 };
			//matrix[2] = new int[2] { 1, 0 };
			//matrix[3] = new int[2] { 1, 1 };
			//matrix[4] = new int[2] { 0, 1 };

			//var matrixG = new MatrixG(matrix);
			////matrixG.Display();

			//// H = 1 0 1 0 0
			////     1 1 0 1 0
			////     0 1 0 0 1
			//var newMatrixH = matrixG.GetMatrixH();
			////newMatrixH.Display();

			//ConsoleHelper.WriteInformation($"--- Finished testing in {nameof(Test2)}.");
		}
	}
}
