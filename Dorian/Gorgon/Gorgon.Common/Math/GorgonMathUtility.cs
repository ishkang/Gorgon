#region MIT.
// 
// Gorgon.
// Copyright (C) 2008 Michael Winsor
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Created: Saturday, April 19, 2008 11:44:35 AM
// 
#endregion

using System;
using SlimMath;

namespace GorgonLibrary.Math
{
	/// <summary>
	/// Utility class for mathematics.
	/// </summary>
	/// <remarks>
	/// This just provides a convenient interface for commonly used mathematical functions and constants.
	/// </remarks>
	public static class GorgonMathUtility
	{
		#region Constants.
		private const float degConvert = ((float)System.Math.PI / 180.0f);		// Constant containing the value used to convert degrees to radians.
		private const float radConvert = (180.0f / (float)System.Math.PI);		// Constant containing the value used to convert radians to degrees.

		/// <summary>
		/// Constant containing the value of PI.
		/// </summary>
		public const float PI = (float)System.Math.PI;
		#endregion

		#region Methods.
		/// <summary>
		/// Function to take e raised to the power passed in.
		/// </summary>
		/// <param name="power">Value to take e from.</param>
		/// <returns><c>e</c> raised to the power specified.</returns>
		public static float Exp(float power)
		{
			return (float)System.Math.Exp(power);
		}

		/// <summary>
		/// Function to raise a value to a specified power.
		/// </summary>
		/// <param name="value">Value to raise.</param>
		/// <param name="power">Power to raise up to.</param>
		/// <returns>The value raised to the power.</returns>
		public static float Pow(float value, float power)
		{
			return (float)System.Math.Pow(value, power);
		}

		/// <summary>
		/// Function to round a floating point value to an integer.
		/// </summary>
		/// <param name="value">Value to round.</param>
		/// <returns>Rounded value.</returns>
		public static int RoundInt(float value)
		{
			return (int)System.Math.Round(value);
		}

		/// <summary>
		/// Function to round a floating point value to an integer.
		/// </summary>
		/// <param name="value">Value to round.</param>
		/// <returns>Rounded value.</returns>
		public static float Round(float value)
		{
			return (float)System.Math.Round(value);
		}

		/// <summary>
		/// Function to round a floating point value.
		/// </summary>
		/// <param name="value">Value to round.</param>
		/// <param name="decimalPlaceCount">Number of decimal places to return.</param>
		/// <returns>Rounded value.</returns>
		public static float Round(float value, int decimalPlaceCount)
		{
			return (float)System.Math.Round(value, decimalPlaceCount);
		}

		/// <summary>
		/// Function to round a floating point value.
		/// </summary>
		/// <param name="value">Value to round.</param>
		/// <param name="decimalPlaceCount">Number of decimal places to return.</param>
		/// <param name="rounding">Determines how to round mid point numbers.</param>
		/// <returns>Rounded value.</returns>
		public static float Round(float value, int decimalPlaceCount, MidpointRounding rounding)
		{
			return (float)System.Math.Round(value, decimalPlaceCount, rounding);
		}

		/// <summary>
		/// Function to return the cosine of an angle in radians.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <returns>Cosine.</returns>
		public static float Cos(float radians)
		{
			return (float)System.Math.Cos(radians);
		}

		/// <summary>
		/// Function to return the sine of an angle in radians.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <returns>Sine.</returns>
		public static float Sin(float radians)
		{
			return (float)System.Math.Sin(radians);
		}

		/// <summary>
		/// Function to return the tangent of an angle in radians.
		/// </summary>
		/// <param name="radians">Angle in radians.</param>
		/// <returns>Tangent.</returns>
		public static float Tan(float radians)
		{
			return (float)System.Math.Tan(radians);
		}

		/// <summary>
		/// Function to return the arc cosine in radians.
		/// </summary>
		/// <param name="cosine">Cosine value to convert.</param>
		/// <returns>Angle in radians.</returns>
		public static float ACos(float cosine)
		{
			return (float)System.Math.Acos(cosine);
		}

		/// <summary>
		/// Function to return the arc sine in radians.
		/// </summary>
		/// <param name="sine">Sine value to convert.</param>
		/// <returns>Angle in radians.</returns>
		public static float ASin(float sine)
		{
			return (float)System.Math.Asin(sine);
		}

		/// <summary>
		/// Function to return the arc tangent in radians.
		/// </summary>
		/// <param name="y">Sin value to convert.</param>
		/// <param name="x">Cosine value to convert.</param>
		/// <returns>Angle in radians.</returns>
		public static float ATan(float y, float x)
		{
			return (float)System.Math.Atan2(y, x);
		}

		/// <summary>
		/// Function to return the square root of a number.
		/// </summary>
		/// <param name="sqvalue">Number to get the square root of.</param>
		/// <returns>Square root of the number.</returns>
		public static float Sqrt(float sqvalue)
		{
			return (float)System.Math.Sqrt(sqvalue);
		}

		/// <summary>
		/// Function to return the inverse square root of a number.
		/// </summary>
		/// <param name="sqvalue">Number to get the inverse square root of.</param>
		/// <returns>Inverted square root.</returns>
		public static float InverseSqrt(float sqvalue)
		{
			return 1.0f / Sqrt(sqvalue);
		}

		/// <summary>
		/// Function to return the absolute value of a number.
		/// </summary>
		/// <param name="number">Number to get the absolute value of.</param>
		/// <returns>Absolute value of the number.</returns>
		public static double Abs(double number)
		{
			return System.Math.Abs(number);
		}

		/// <summary>
		/// Function to return the absolute value of a number.
		/// </summary>
		/// <param name="number">Number to get the absolute value of.</param>
		/// <returns>Absolute value of the number.</returns>
		public static float Abs(float number)
		{
			return System.Math.Abs(number);
		}

		/// <summary>
		/// Function to return the absolute value of a number.
		/// </summary>
		/// <param name="number">Number to get the absolute value of.</param>
		/// <returns>Absolute value of the number.</returns>
		public static int Abs(int number)
		{
			return System.Math.Abs(number);
		}

		/// <summary>
		/// Function to return if two double precision numbers are equal within a threshold.
		/// </summary>
		/// <param name="value1">First double value to compare.</param>
		/// <param name="value2">Second double value to compare.</param>
		/// <param name="delta">Delta to account for error between the two values.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualDouble(double value1, double value2, double delta)
		{
			return (Abs(value2 - value1) <= delta);
		}

		/// <summary>
		/// Function to return if two double precision numbers are equal within a threshold.
		/// </summary>
		/// <param name="value1">First double value to compare.</param>
		/// <param name="value2">Second double value to compare.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualDouble(double value1, double value2)
		{
			return EqualDouble(value1, value2, 0.0);
		}

		/// <summary>
		/// Function to return if two floating point numbers are equal within a threshold.
		/// </summary>
		/// <param name="value1">First float value to compare.</param>
		/// <param name="value2">Second float value to compare.</param>
		/// <param name="epsilon">Tolerance of floating point error.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualFloat(float value1, float value2, float epsilon)
		{
			return (Abs(value2 - value1) <= epsilon);
		}

		/// <summary>
		/// Function to return if two floating point numbers are equal.
		/// </summary>
		/// <param name="value1">First float value to compare.</param>
		/// <param name="value2">Second float value to compare.</param>		
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualFloat(float value1, float value2)
		{
			return EqualFloat(value1, value2, 1e-6f);
		}

		/// <summary>
		/// Function to return if two 2D vectors are equal.
		/// </summary>
		/// <param name="value1">First vector value to compare.</param>
		/// <param name="value2">Second vector value to compare.</param>
		/// <param name="epsilon">Tolerance of floating point error.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualVector2(Vector2 value1, Vector2 value2, float epsilon)
		{
			return EqualFloat(value1.X, value2.X, epsilon) && EqualFloat(value1.Y, value1.Y, epsilon);
		}

		/// <summary>
		/// Function to return if two 2D vectors are equal.
		/// </summary>
		/// <param name="value1">First vector value to compare.</param>
		/// <param name="value2">Second vector value to compare.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualVector2(Vector2 value1, Vector2 value2)
		{
			return EqualVector2(value1, value2, 1e-6f);
		}

		/// <summary>
		/// Function to return if two 3D vectors are equal.
		/// </summary>
		/// <param name="value1">First vector value to compare.</param>
		/// <param name="value2">Second vector value to compare.</param>
		/// <param name="epsilon">Tolerance of floating point error.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualVector3(Vector3 value1, Vector3 value2, float epsilon)
		{
			return EqualFloat(value1.X, value2.X, epsilon) && EqualFloat(value1.Y, value1.Y, epsilon) & EqualFloat(value1.Z, value2.Z, epsilon);
		}

		/// <summary>
		/// Function to return if two 3D vectors are equal.
		/// </summary>
		/// <param name="value1">First vector value to compare.</param>
		/// <param name="value2">Second vector value to compare.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualVector3(Vector3 value1, Vector3 value2)
		{
			return EqualVector3(value1, value2, 1e-6f);
		}

		/// <summary>
		/// Function to return if two 4D vectors are equal.
		/// </summary>
		/// <param name="value1">First vector value to compare.</param>
		/// <param name="value2">Second vector value to compare.</param>
		/// <param name="epsilon">Tolerance of floating point error.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualVector4(Vector4 value1, Vector4 value2, float epsilon)
		{
			return EqualFloat(value1.X, value2.X, epsilon) && EqualFloat(value1.Y, value1.Y, epsilon) & EqualFloat(value1.Z, value2.Z, epsilon) & EqualFloat(value1.W, value2.W, epsilon);
		}

		/// <summary>
		/// Function to return if two 4D vectors are equal.
		/// </summary>
		/// <param name="value1">First vector value to compare.</param>
		/// <param name="value2">Second vector value to compare.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool EqualVector4(Vector4 value1, Vector4 value2)
		{
			return EqualVector4(value1, value2, 1e-6f);
		}

		/// <summary>
		/// Function to convert degrees into radians.
		/// </summary>
		/// <param name="degrees">Degrees</param>
		/// <returns>Radian equivilant of degrees.</returns>
		public static float Radians(float degrees)
		{
			return degConvert * degrees;
		}

		/// <summary>
		/// Function to convert radians into degrees.
		/// </summary>
		/// <param name="radians">Radians</param>
		/// <returns>Degree equivilant of radians.</returns>
		public static float Degrees(float radians)
		{
			return radConvert * radians;
		}

		/// <summary>
		/// Function to return the highest of two values.
		/// </summary>
		/// <returns>Highest of the values supplied.</returns>
		public static float Max(float value1, float value2)
		{
			if (value1 > value2)
				return value1;

			return value2;
		}

		/// <summary>
		/// Function to return the lowest of two values.
		/// </summary>
		/// <returns>Highest of the values supplied.</returns>
		public static float Min(float value1, float value2)
		{
			if (value1 < value2)
				return value1;

			return value2;
		}

		/// <summary>
		/// Function to return the highest of two values.
		/// </summary>
		/// <returns>Highest of the values supplied.</returns>
		public static short Max(short value1, short value2)
		{
			if (value1 > value2)
				return value1;

			return value2;
		}

		/// <summary>
		/// Function to return the lowest of two values.
		/// </summary>
		/// <returns>Highest of the values supplied.</returns>
		public static short Min(short value1, short value2)
		{
			if (value1 < value2)
				return value1;

			return value2;
		}

		/// <summary>
		/// Function to return the highest of two values.
		/// </summary>
		/// <returns>Highest of the values supplied.</returns>
		public static int Max(int value1, int value2)
		{
			if (value1 > value2)
				return value1;

			return value2;
		}

		/// <summary>
		/// Function to return the lowest of two values.
		/// </summary>
		/// <returns>Highest of the values supplied.</returns>
		public static int Min(int value1, int value2)
		{
			if (value1 < value2)
				return value1;

			return value2;
		}
		#endregion
	}
}
