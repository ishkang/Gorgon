﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2012 Michael Winsor
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
// Created: Sunday, October 14, 2012 4:37:30 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Simplex;

namespace Gorgon.Core
{
    /// <summary>
    /// A random number generator for floating point and integer values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class expands upon the functionality of the <see cref="Random"/> class by providing <see cref="float"/> random numbers, and ranges for <see cref="double"/> and <see cref="float"/> random numbers.
    /// </para>
    /// <para>
    /// It also provides a simplex noise implementation for generation of repeatable random noise.
    /// </para> 
    /// <para>
    /// The simplex noise functionality uses the Noise.cs functionality built by Benjamin Ward, which was based on another implementation by Heikki Törmälä.  Both of which are derived from the original Java 
    /// implementation by Stefan Gustavson.
    /// <list type="bullet">
    ///     <item>
    ///         <term>Benjamin Ward</term>
    ///         <description><a target="_blank" href="https://github.com/WardBenjamin/SimplexNoise">https://github.com/WardBenjamin/SimplexNoise</a></description>
    ///     </item>
    ///     <item>
    ///         <term>Heikki Törmälä</term>
    ///         <description><a target="_blank" href="https://github.com/Xpktro/simplexnoise/blob/master/SimplexNoise/Noise.cs">https://github.com/Xpktro/simplexnoise/blob/master/SimplexNoise/Noise.cs</a></description>
    ///     </item>
    ///     <item>
    ///         <term>Stefan Gustavson</term>
    ///         <description><a target="_blank" href="http://staffwww.itn.liu.se/~stegu/aqsis/aqsis-newnoise/">http://staffwww.itn.liu.se/~stegu/aqsis/aqsis-newnoise/</a></description>
    ///     </item>
    /// </list> 
    /// </para> 
    /// </remarks>
    /// <seealso cref="Random"/>
    public static class GorgonRandom
    {
        #region Variables.
		// Seed used to generate random numbers.
		private static int _seed;
		// Random number generator.
		private static Random _rnd;
		#endregion

		#region Properties.
		/// <summary>
		/// Property to set or return the random seed value.
		/// </summary>
		public static int Seed
		{
			get => _seed;
			set
			{
			    Noise.Seed = value;
				_seed = value;
				_rnd = new Random(_seed);
			}
		}

		/// <summary>
		/// Property to set or return the Simplex noise permutation array.
		/// </summary>
		/// <remarks>
		/// This is used to modify the random values generated by the Simplex noise algorithm.
		/// </remarks>
		public static IReadOnlyList<byte> SimplexPermutations
		{
		    get => Noise.Perm;
			set
			{
			    if (value == null)
			    {
			        return;
			    }

                Noise.Perm = value.ToArray();
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to generate 1 dimensional simplex noise.
		/// </summary>
		/// <param name="value">The <see cref="float"/> value to use to generate the simplex noise value.</param>
		/// <returns>A <see cref="float"/> representing the simplex noise value.</returns>
		/// <remarks>
		/// <para>
		/// Simplex noise values similar to Perlin noise but with fewer artifacts and better performance. 
		/// </para>
		/// <para>
		/// This produces predictable random numbers based on the seed <paramref name="value"/> passed to the method. 
		/// </para>
		/// </remarks>
		public static float SimplexNoise(float value)
		{
		    return 1.0f + (Noise.Generate(value) - 1.0f);
		}

        /// <summary>
        /// Function to generate 2 dimensional simplex noise.
        /// </summary>
        /// <param name="x">The horizontal value to use to generate the simplex noise value.</param>
        /// <param name="y">The vertical value to use to generate the simplex noise value.</param>
        /// <returns>A <see cref="float"/> representing the simplex noise value.</returns>
        /// <remarks>
        /// <para>
        /// Simplex noise values similar to Perlin noise but with fewer artifacts and better performance. 
        /// </para>
        /// <para>
        /// This produces predictable random numbers based on the seed value passed to the method. 
        /// </para>
        /// </remarks>
        public static float SimplexNoise(float x, float y)
		{
		    return 1.0f + (Noise.Generate(x, y) - 1.0f);
		}

        /// <summary>
        /// Function to generate 3 dimensional simplex noise.
        /// </summary>
        /// <param name="x">The horizontal value to use to generate the simplex noise value.</param>
        /// <param name="y">The vertical value to use to generate the simplex noise value.</param>
        /// <param name="z">The depth value to use to generate the simplex noise value.</param>
		/// <returns>A <see cref="float"/> representing the simplex noise value.</returns>
        /// <remarks>
        /// <para>
        /// Simplex noise values similar to Perlin noise but with fewer artifacts and better performance. 
        /// </para>
        /// <para>
        /// This produces predictable random numbers based on the seed values passed to the method. 
        /// </para>
        /// </remarks>
        public static float SimplexNoise(float x, float y, float z)
		{
		    return 1.0f + (Noise.Generate(x, y, z) - 1.0f);
		}

        /// <summary>
        /// Function to generate 4 dimensional simplex noise.
        /// </summary>
        /// <param name="x">The horizontal value to use to generate the simplex noise value.</param>
        /// <param name="y">The vertical value to use to generate the simplex noise value.</param>
        /// <param name="z">The depth value to use to generate the simplex noise value.</param>
        /// <param name="w">The w value to use to generate the simplex noise value.</param>
		/// <returns>A <see cref="float"/> representing the simplex noise value.</returns>
        /// <remarks>
        /// <para>
        /// Simplex noise values similar to Perlin noise but with fewer artifacts and better performance. 
        /// </para>
        /// <para>
        /// This produces predictable random numbers based on the seed values passed to the method. 
        /// </para>
        /// </remarks>
        public static float SimplexNoise(float x, float y, float z, float w)
		{
		    return SimplexNoise(x, y, z);
		}
        
		/// <summary>
		/// Function to return a random <see cref="float"/> number.
		/// </summary>
		/// <param name="start">The starting value for the random number.</param>
		/// <param name="end">The ending value for the random number range.  This value is inclusive.</param>
		/// <returns>A random <see cref="float"/> value between <paramref name="start"/> to <paramref name="end"/>.</returns>
		/// <remarks>
		/// This overload generates a random <see cref="float"/> number between the range of <paramref name="start"/> and <paramref name="end"/>.
		/// </remarks>
		public static float RandomSingle(float start, float end)
		{
			return start < end
				       ? (float)_rnd.NextDouble() * (end - start) + start
				       : (float)_rnd.NextDouble() * (start - end) + end;
		}

		/// <summary>
		/// Function to return a random <see cref="float"/> number.
		/// </summary>
		/// <param name="maxValue">The highest number for random values, this value is inclusive.</param>
		/// <returns>A random <see cref="float"/> value.</returns>.
		/// <remarks>
		/// This overload generates a random <see cref="float"/> number between the range of 0 and <paramref name="maxValue"/>.
		/// </remarks>
		public static float RandomSingle(float maxValue)
		{
			return RandomSingle(0, maxValue);
		}

		/// <summary>
		/// Function to return a random <see cref="float"/> number.
		/// </summary>
		/// <returns>A random <see cref="float"/> value between 0.0f and 1.0f.</returns>
		public static float RandomSingle()
		{
			return (float)_rnd.NextDouble();
		}

		/// <summary>
		/// Function to return a non-negative random <see cref="int"/>.
		/// </summary>
		/// <param name="start">Starting value for the random number.</param>
		/// <param name="end">Ending value for the random number range.  This value is not inclusive.</param>
		/// <returns>The random <see cref="int"/> value within the range of <paramref name="start"/> to <paramref name="end"/>.</returns>
		/// <remarks>
		/// This overload generates a random <see cref="int"/> number between the range of <paramref name="start"/> and <paramref name="end"/>-1.
		/// </remarks>
		public static int RandomInt32(int start, int end)
		{
			return _rnd.Next(start, end);
		}

		/// <summary>
		/// Function to return a non-negative random <see cref="int"/>.
		/// </summary>
		/// <param name="maxValue">The highest number for random values, this value is not inclusive.</param>
		/// <returns>A random number</returns>.
		/// <remarks>
		/// This overload generates a random <see cref="int"/> number between the range of 0 and <paramref name="maxValue"/>-1.
		/// </remarks>
		public static int RandomInt32(int maxValue)
		{
			return _rnd.Next(maxValue);
		}

		/// <summary>
		/// Function to return a non-negative random <see cref="int"/>.
		/// </summary>
		/// <returns>A random <see cref="int"/> value between 0 and <see cref="int.MaxValue"/>-1.</returns>
		public static int RandomInt32()
		{
			return _rnd.Next();
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes the <see cref="GorgonRandom" /> class.
		/// </summary>
		static GorgonRandom()
		{
			Seed = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
		}
		#endregion
	}
}
