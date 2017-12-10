using System;
using Logic;

namespace KOD_1
{
	class Program
	{
		static void Main(string[] args)
		{
			var matrix = new int[3, 6]
			{ 
				{1,0,0,1,0,1},
				{0,1,0,1,1,0}, 
				{0,0,1,0,1,1}
			};
			var matrixH = new MatrixH(matrix);
			
			var vector = new int[6] { 0,1,1,0,0,0 };
			var result = matrixH.CalculateSyndrome(vector);

			foreach (var number in result)
				Console.Write(number);
			Console.WriteLine();



			var matrix2 = new int[2, 5]
			{
				{1,0,1,1,0},
				{0,1,0,1,1}
			};
			var matrixG = new MatrixG(matrix2);

			var vector2 = new int[2] {1,1};
			var result2 = matrixG.EncodeVector(vector2);
			
			foreach (var number in result2)
				Console.Write(number);
			Console.WriteLine();

			Console.ReadKey();
		}
	}
}
