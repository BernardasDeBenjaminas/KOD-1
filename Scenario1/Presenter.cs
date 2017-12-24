using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Logic;

namespace Scenario1
{
	/// <summary>
	/// Įjungia 1-ojo scenarijaus vartotojo sąsają.
	/// </summary>
	public class Presenter
	{
		private string _errorMessage = string.Empty; // Naudojamas klaidos žinutėms atvaizduoti.

		private int _rows; // '_matrixG' dimensija (k).
		private int _cols; // '_matrixG' ilgis (n).

		private List<List<byte>> _tempMatrix; // Laikina matrica, kurią naudosiu vartotojui pačiam suvedinėjant G matricą.
		private MatrixG _matrixG; // G matrica (vartotojo įvesta arba kompiuterio sugeneruota).
		private MatrixH _matrixH; // H matrica (gauta iš '_matrixG').

		private Channel _channel; // Kanalas, kuriuo siųsiu vektorius.
		private double _errorProbability = -1; // Tikimybė kanale įvykti klaidai (p). -1, nes 0 yra leidžiama reikšmė.
		private List<byte> _errorVector; // Klaidos vektorius.

		private List<byte> _originalVector;	// Vartotojo žodis, kurį siųsime kanalu.
		private List<byte> _encodedVector;   // '_originalVector'  užkoduotas G matrica.
		private List<byte> _distortedVector; // '_encodedVector'   išsiųstas kanalu (galimai iškraipytas).
		private List<byte> _decodedVector;   // '_distortedVector' dekoduotas H matrica.
		private List<byte> _receivedVector;  // '_decodedVector'   dekoduotas G matrica.


		// PUBLIC
		/// <summary>
		/// Įjungia vartotojo sąsają.
		/// </summary>
		public void Start()
		{
			_errorProbability = GetErrorProbabilty();
			_channel = new Channel(_errorProbability);
			_cols = GetNumberOfCols();
			_rows = GetNumberOfRows();

			if (AskYesOrNoQuestion("Ar norite įvesti generuojančią matricą patys (jeigu ne - ji bus sugeneruota už jus)?"))
				LetUserEnterGMatrix();
			else
				_matrixG = new MatrixG(_cols, _rows);

			_matrixH = _matrixG.GetMatrixH();

			while (true)
			{
				_originalVector = GetVectorToSend();
				_encodedVector = _matrixG.Encode(_originalVector);
				_distortedVector = _channel.SendVectorThrough(_encodedVector);
				_errorVector = _channel.FindDifferences(_encodedVector, _distortedVector);

				if (AskYesOrNoQuestion("Ar norite keisti iš kanalo gautą vektorių?"))
					LetUserEnterErrorVector();

				_decodedVector = _matrixH.Decode(_distortedVector);
				_receivedVector = _matrixG.Decode(_decodedVector);

				if (!AskYesOrNoQuestion("Ar norite siųsti dar vieną vektorių?"))
					break;

				_originalVector = null;
				_encodedVector = null;
				_distortedVector = null;
				_errorVector = null;
				_decodedVector = null;
				_receivedVector = null;
			}
		}


		// PRIVATE
		/// <summary>
		/// Per vartotojo sąsają priima G matricos ilgį (n).
		/// </summary>
		/// <returns>G matricos ilgį (n).</returns>
		private int GetNumberOfCols()
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.Write("Įveskite kodo ilgį (n): ");
				var input = Console.ReadLine();

				if (int.TryParse(input, out var cols))
				{
					if (cols < 2)
						_errorMessage = "Reikšmė privalo būti didesnė už 1!";

					else
					{
						_errorMessage = string.Empty;
						return cols;
					}
				}
				else
					_errorMessage = "Galimos reikšmės tik sveikieji skaičiai!";

				Console.Clear();
			}
		}

		/// <summary>
		/// Per vartotojo sąsają priima G matricos dimensiją (k).
		/// </summary>
		/// <returns>G matricos dimensiją (k).</returns>
		private int GetNumberOfRows()
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.Write("Įveskite dimensiją (k): ");
				var input = Console.ReadLine();

				if (int.TryParse(input, out var rows))
				{
					if (rows < 1)
						_errorMessage = "Reikšmė privalo būti didesnė už 0!";

					else if (rows == _cols)
						_errorMessage = $"Reikšmė negali sutapti su ilgiu ({_cols})!";

					else if (rows > _cols)
						_errorMessage = "Reikšmė negali būti didesnė už kodo ilgį!";

					else
					{
						_errorMessage = string.Empty;
						return rows;
					}
				}

				else
					_errorMessage = "Galimos reikšmės tik sveikieji skaičiai!";

				Console.Clear();
			}
		}

		/// <summary>
		/// Per vartotojo sąsają priima tikimybę klaidai įvykti (p) siunčiant vektorių kanalu.
		/// </summary>
		/// <returns>Tikimybę įvykti klaidai kanale (p).</returns>
		private double GetErrorProbabilty()
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.Write("Įveskite klaidos tikimybę (p): ");
				var input = Console.ReadLine();

				if (double.TryParse(input, out var probability))
				{
					if (probability > 1 || probability < 0)
						_errorMessage = "Reikšmė privalo būti intervale [0;1] (ar įvedėte skaičių su kableliu?)!";

					else
					{
						_errorMessage = string.Empty;
						return probability;
					}
				}
				else
					_errorMessage = "Leidžiama įvedimo forma: #.#### (taškas, ne kablelis)!";
				
					Console.Clear();
			}
		}

		/// <summary>
		/// Per vartotojo sąsają priimta vektorių siuntimui kanalu.
		/// </summary>
		/// <returns>Vektorius, kurį turėsime siųsti kanalu (m).</returns>
		private List<byte> GetVectorToSend()
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.Write("Įveskite žodį, kurį norite siųsti kanalu: ");
				var input = Console.ReadLine();

				if (Regex.IsMatch(input, "^[0,1]{1,}$"))
				{
					if (input.Length != _rows)

						_errorMessage = $"Vektoriaus ilgis privalo būti lygus {_rows}.";
					else
					{
						_errorMessage = string.Empty;
						return StringToIntArrayVector(input);
					}
				}

				else
					_errorMessage = "Leidžiami simboliai yra tik '0' ir '1'.";
			}
		}

		/// <summary>
		/// Per vartotojo sąsają priima atsakymą į užduotą klausimą.
		/// </summary>
		/// <param name="question">Klausimas, kurį norime užduoti vartotojui.</param>
		/// <returns>Grąžinama 'true', jeigu atsakė 'taip' - antraip grąžinama 'false'.</returns>
		private bool AskYesOrNoQuestion(string question)
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.Write($"{question} (y/n): ");
				var input = Console.ReadLine();

				if (input.ToLower() != "y" && input.ToLower() != "n")
					_errorMessage = "Įveskite 'y', jeigu norite keisti iš kanalo gautą vektorių, antraip 'n'.";

				else
				{
					_errorMessage = string.Empty;
					return input.ToLower() == "y";
				}
			}
		}

		/// <summary>
		/// (!) Leidžia vartotojui įvesti savo norimą klaidos vektorių. (!)
		/// Per vartotojo sąsaja priima stulpelių numerius, kuriais pasinaudodami keisime 
		/// '_errorVector' bei '_distortedVector' atitinkamų stulpelių reikšmes.
		/// </summary>
		private void LetUserEnterErrorVector()
		{
			while (true)
			{
				DisplayCurrentInformation();

				Console.WriteLine("Spauskite skaičių, kuris atitiktų stulpelio numerį (paspaudus '1' pasikeis pirmoji pozicija).");
				Console.WriteLine("Spauskite 'n', jeigu norite pabaigti keitimą.");
				var input = Console.ReadKey(true).KeyChar + "";

				if (input == "n")
				{
					_errorMessage = string.Empty;
					return;
				}

				if (int.TryParse(input, out var col))
				{
					if (col > _cols || col < 1)
						_errorMessage = $"Skaičius privalo būti tarp [1;{_cols}]!";

					else
					{
						_errorVector[col - 1] = (byte) (_errorVector[col - 1] ^ 1);
						_distortedVector[col - 1] = (byte) (_distortedVector[col - 1] ^ 1);
					}
				}
				else
					_errorMessage = "Galimos reikšmės tik sveikieji skaičiai!";
			}
		}

		/// <summary>
		/// (!) Leidžia vartotojui įvesti savo norimą G matricą. (!)
		/// Per vartotojo sąsaja priima vektorius, kuriais pasinaudodami keisime
		/// '_tempMatrix' ir sukursime '_matrixG'.
		/// </summary>
		private void LetUserEnterGMatrix()
		{
			_tempMatrix = new List<List<byte>>(_rows);
			
			for (var r = 0; r < _rows; r++)
			{
				DisplayCurrentInformation();

				Console.Write($"Įveskite {r + 1}-ąjį vektorių: ");
				var input = Console.ReadLine();

				if (Regex.IsMatch(input, "^[0,1]{1,}$"))
				{
					if (input.Length != _cols)
					{
						_errorMessage = $"Vektoriaus ilgis privalo būti lygus {_cols}.";
					}
					else
					{
						_tempMatrix.Add(StringToIntArrayVector(input));
						_errorMessage = string.Empty;
						continue;
					}
				}
				else
				{
					_errorMessage = "Leidžiami simboliai yra tik '0' ir '1'.";
				}
				r--;
			}
			try
			{
				_matrixG = new MatrixG(_cols, _rows, _tempMatrix);
			}
			catch (Exception e)
			{
				_errorMessage = e.Message;
				LetUserEnterGMatrix();
			}
		}

		/// <summary>
		/// Konvertuoja 'string' tipo vektorių į 'int[]' tipą.
		/// </summary>
		/// <param name="vector">Vektorius, kurį norime konvertuoti.</param>
		/// <returns>Konvertuotą vektorių.</returns>
		private List<byte> StringToIntArrayVector(string vector)
		{
			var length = vector.Length;
			var row = new List<byte>(length);
			for (var c = 0; c < length; c++)
			{
				row.Add((byte)char.GetNumericValue(vector[c]));
			}
			return row;
		}

		/// <summary>
		/// Į vartotojo sąsają išveda visą turimą informaciją apie vektorius ir matricas.
		/// </summary>
		private void DisplayCurrentInformation()
		{
			Console.Clear();

			if (_errorProbability != -1)
				ConsoleHelper.WriteInformation($"Klaidos tikimybė (p): {_errorProbability}");

			if (_cols != 0)
				ConsoleHelper.WriteInformation($"Ilgis (n): {_cols}");

			if (_rows != 0)
				ConsoleHelper.WriteInformation($"Dimensija (k): {_rows}");

			Console.WriteLine();

			if (_matrixG == null && _tempMatrix != null)
			{
				var message = "G = ";

				for (var c = 0; c < _tempMatrix.Count; c++)
					if (_tempMatrix[c] != null)
						message += string.Join(" ", _tempMatrix[c]) + "\n    ";

				ConsoleHelper.WriteInformation(message);
			}

			if (_matrixG != null)
				ConsoleHelper.WriteMatrix("G", _matrixG);

			if (_matrixH != null)
				ConsoleHelper.WriteMatrix("H", _matrixH);

			if (_originalVector != null)
				ConsoleHelper.WriteInformation($"Siunčiamas žodis (m)  = {string.Join(" ", _originalVector)}");

			if (_encodedVector != null)
				ConsoleHelper.WriteInformation($"Užkoduotas žodis (c)  = {string.Join(" ", _encodedVector)}");

			if (_distortedVector != null && _errorVector != null)
				ConsoleHelper.WriteChanges($"Gautas vektorius (y)  =", _errorVector, _distortedVector);

			if (_decodedVector != null)
				ConsoleHelper.WriteInformation($"Dekoduotas žodis (c') = {string.Join(" ", _decodedVector)}");

			if (_receivedVector != null)
				ConsoleHelper.WriteInformation($"Galutinis žodis  (m') = {string.Join(" ", _receivedVector)}");

			if (_errorMessage != string.Empty)
				ConsoleHelper.WriteError(_errorMessage);

			Console.WriteLine();
		}
	}
}
