﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2015 Michael Winsor
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
// Created: Monday, July 20, 2015 11:01:04 PM
// 
#endregion

using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using Gorgon.Input;

namespace Gorgon.Native
{
	/// <summary>
	/// Native Win32 API calls.
	/// </summary>
	[SuppressUnmanagedCodeSecurity]
	class Win32API
	{
		#region Properties.
		/// <summary>
		/// Property to return the number of function keys on the keyboard.
		/// </summary>
		public static int FunctionKeyCount
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to return the keyboard type.
		/// </summary>
		public static KeyboardType KeyboardType
		{
			get;
			private set;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to retrieve keyboard type information.
		/// </summary>
		/// <param name="nTypeFlag">The type of info.</param>
		/// <returns>The requested information.</returns>
		[DllImport("User32.dll", CharSet = CharSet.Ansi), SuppressUnmanagedCodeSecurity]
		private static extern int GetKeyboardType(int nTypeFlag);

		/// <summary>
		/// Function to get the state of a key.
		/// </summary>
		/// <param name="nVirtKey">Virtual key code to retrieve.</param>
		/// <returns>A bit mask containing the state of the virtual key.</returns>
		[DllImport("User32.dll"), SuppressUnmanagedCodeSecurity]
		public static extern short GetKeyState(Keys nVirtKey);

		/// <summary>
		/// Function to retrieve the scan code for a virtual key.
		/// </summary>
		/// <param name="uCode">Virtual key code</param>
		/// <param name="uMapType">Mapping type.</param>
		/// <returns>The scan code.</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto), SuppressUnmanagedCodeSecurity]
		public static extern int MapVirtualKey(Keys uCode, int uMapType);
		#endregion

		#region Constructor.
		/// <summary>
		/// Initializes static members of the <see cref="Win32API"/> class.
		/// </summary>
		static Win32API()
		{
			Marshal.PrelinkAll(typeof(Win32API));

			int keyboardType = GetKeyboardType(0);

			switch (keyboardType)
			{
				case 1:
					KeyboardType = KeyboardType.XT;
					break;
				case 2:
					KeyboardType = KeyboardType.OlivettiICO;
					break;
				case 3:
					KeyboardType = KeyboardType.AT;
					break;
				case 4:
					KeyboardType = KeyboardType.Enhanced;
					break;
				case 5:
					KeyboardType = KeyboardType.Nokia1050;
					break;
				case 6:
					KeyboardType = KeyboardType.Nokia9140;
					break;
				case 7:
					KeyboardType = KeyboardType.Japanese;
					break;
				case 81:
					KeyboardType = KeyboardType.USB;
					break;
				default:
					KeyboardType = KeyboardType.Unknown;
					break;
			}

			FunctionKeyCount = GetKeyboardType(2);
		}
		#endregion
	}
}