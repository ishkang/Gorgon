#region MIT.
// 
// Gorgon.
// Copyright (C) 2007 Michael Winsor
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
// Created: Saturday, June 16, 2007 3:27:46 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Flobbster.Windows.Forms;

namespace GorgonLibrary.Graphics.Tools.PropBag
{
	/// <summary>
	/// Interface for the an object that has a property bag.
	/// </summary>
	public interface IPropertyBagObject
	{
		#region Properties.
		/// <summary>
		/// Property to return the property bag for the object.
		/// </summary>
		PropertyBag PropertyBag
		{
			get;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to set a value for the property.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		void SetValue(object sender, PropertySpecEventArgs e);

		/// <summary>
		/// Function to set a value for the property.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event parameters.</param>
		void GetValue(object sender, PropertySpecEventArgs e);
		#endregion
	}
}
