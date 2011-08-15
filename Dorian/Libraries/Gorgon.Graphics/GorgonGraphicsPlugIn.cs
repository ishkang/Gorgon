﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2011 Michael Winsor
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
// Created: Tuesday, July 19, 2011 8:42:01 AM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorgonLibrary;
using GorgonLibrary.PlugIns;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// The plug-in interface for a graphics object.
	/// </summary>
	public abstract class GorgonGraphicsPlugIn
		: GorgonPlugIn, IDisposable
	{
		#region Variables.
		private bool _disposed = false;			// Flag to indicate that the object was disposed.
		private IDisposable _graphics = null;	// Graphics interface.
		#endregion

		#region Methods.
		/// <summary>
		/// Function to perform the actual creation of the graphics object.
		/// </summary>
		/// <returns>A new renderer object.</returns>
		/// <remarks>Implementors must use this to create the graphics object from the plug-in.</remarks>
		protected abstract GorgonGraphics CreateGraphics();

		/// <summary>
		/// Function to create a new graphics object.
		/// </summary>
		/// <returns>A new graphics object.</returns>
		internal GorgonGraphics GetGraphics()
		{
			GorgonGraphics graphics = CreateGraphics();
			_graphics = graphics as IDisposable;
			return graphics;
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonGraphicsPlugIn"/> class.
		/// </summary>
		/// <param name="description">Optional description of the plug-in.</param>
		/// <remarks>Objects that implement this base class should pass in a hard coded description on the base constructor.</remarks>
		protected GorgonGraphicsPlugIn(string description)
			: base(description)
		{
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_graphics != null)
						_graphics.Dispose();
				}

				_disposed = true;
				_graphics = null;
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
