using System;
using Logic;

namespace KOD_1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var matrix = new int[6][];
			matrix[0] = new int[3]{1,0,0};
			matrix[1] = new int[3]{0,1,0};
			matrix[2] = new int[3]{0,0,1};
			matrix[3] = new int[3]{1,1,0};
			matrix[4] = new int[3]{0,1,1};
			matrix[5] = new int[3]{1,0,1};
			//var matrix = new int[3, 6]
			//{ 
			//	{1,0,0,1,0,1},
			//	{0,1,0,1,1,0}, 
			//	{0,0,1,0,1,1}
			//};
			var matrixH = new MatrixH(matrix);
			
			var vector = new int[6] { 1,0,0,0,1,1 };
			var result = matrixH.CalculateSyndrome(vector);
			
			ConsoleHelper.WriteInformation("Testing MatrixH.");
			ConsoleHelper.WriteCheck("100011", vector, "010", result);
			Console.WriteLine();
			Console.WriteLine();

			// G = 10110
			//     01011

			//// new G = 1 1 0 1 0 0
			////         0 1 1 0 1 0
			////         1 0 1 0 0 1
			//var matrix2 = new int[6][];
			//matrix2[0] = new int[3]{1,0,1};
			//matrix2[1] = new int[3]{1,1,0};
			//matrix2[2] = new int[3]{0,1,1};
			//matrix2[3] = new int[3]{1,0,0};
			//matrix2[4] = new int[3]{0,1,0};
			//matrix2[5] = new int[3]{0,0,1};

			//// new G = 1 0 1 0 1
			////         0 1 1 1 0
			//var matrix2 = new int[5][];
			//matrix2[0] = new int[2]{1,0};
			//matrix2[1] = new int[2]{0,1};
			//matrix2[2] = new int[2]{1,1};
			//matrix2[3] = new int[2]{0,1};
			//matrix2[4] = new int[2]{1,0};

			// new G = 1 0 0 0 0 1 1
			//         0 1 0 0 1 0 1
			//         0 0 1 0 1 1 0
			//         0 0 0 1 1 1 1
			var matrix2 = new int[7][];
			matrix2[0] = new int[4]{1,0,0,0};
			matrix2[1] = new int[4]{0,1,0,0};
			matrix2[2] = new int[4]{0,0,1,0};
			matrix2[3] = new int[4]{0,0,0,1};
			matrix2[4] = new int[4]{0,1,1,1};
			matrix2[5] = new int[4]{1,0,1,1};
			matrix2[6] = new int[4]{1,1,0,1};
			var matrixG = new MatrixG(matrix2);

			//var vector2 = new int[2] {1,0};
			//var result2 = matrixG.EncodeVector(vector2);

			ConsoleHelper.WriteInformation("Testing MatrixG.");
			matrixG.Display();
			//ConsoleHelper.WriteCheck("10", vector2, "10110", result2);

			Console.Write("Is the matrix convertable to an H? ANSWER: ");
			Console.WriteLine(matrixG.IsTransformableToMatrixH());

			var newMatrixH = matrixG.GetMatrixH();
			newMatrixH.Display();

			Console.WriteLine("");


			Console.ReadKey();
		}
	}
}
