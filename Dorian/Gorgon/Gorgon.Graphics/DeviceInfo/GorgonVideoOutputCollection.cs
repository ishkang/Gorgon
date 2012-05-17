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
// Created: Sunday, July 24, 2011 9:41:28 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorgonLibrary.Collections;
using GI = SharpDX.DXGI;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// A collection of video outputs for a video device.
	/// </summary>
	public class GorgonVideoOutputCollection
		: IList<GorgonVideoOutput>
	{
		#region Variables.
		private GorgonVideoDevice _videoDevice = null;			// Video device that owns the outputs in this collection.
		private List<GorgonVideoOutput> _outputs = null;		// List of outputs.
		#endregion

		#region Methods.
		/// <summary>
		/// Function to clear the outputs from the list.
		/// </summary>
		internal void ClearOutputs()
		{
			foreach (var item in _outputs)
				((IDisposable)item).Dispose();

			_outputs.Clear();
		}

		/// <summary>
		/// Function to retrieve the outputs for an adapter.
		/// </summary>
		internal void Refresh(SharpDX.Direct3D11.Device device)
		{
			ClearOutputs();

			Gorgon.Log.Print("Retrieving outputs for video device '{0}'...", Diagnostics.LoggingLevel.Simple, _videoDevice.Name);

			foreach(var giOutput in _videoDevice.GIAdapter.Outputs)
			{
				GorgonVideoOutput output = new GorgonVideoOutput(_videoDevice, giOutput);

				GI.ModeDescription findMode = GorgonVideoMode.Convert(new GorgonVideoMode(output.OutputBounds.Width, output.OutputBounds.Height, BufferFormat.R8G8B8A8_UIntNormal, 60, 1));
				GI.ModeDescription result = default(GI.ModeDescription);

				// Get the default video mode.
				output.GIOutput.GetClosestMatchingMode(device, findMode, out result);
				output.DefaultVideoMode = GorgonVideoMode.Convert(result);

				_outputs.Add(output);

				Gorgon.Log.Print("Found output {0}.", Diagnostics.LoggingLevel.Simple, output.Name);
				Gorgon.Log.Print("===================================================================", Diagnostics.LoggingLevel.Verbose);
				Gorgon.Log.Print("Output bounds: ({0}x{1})-({2}x{3})", Diagnostics.LoggingLevel.Verbose, output.OutputBounds.Left, output.OutputBounds.Top, output.OutputBounds.Right, output.OutputBounds.Bottom);
				Gorgon.Log.Print("Monitor handle: 0x{0}", Diagnostics.LoggingLevel.Verbose, output.Handle.FormatHex());
				Gorgon.Log.Print("Attached to desktop: {0}", Diagnostics.LoggingLevel.Verbose, output.IsAttachedToDesktop);
				Gorgon.Log.Print("Monitor rotation: {0}\u00B0", Diagnostics.LoggingLevel.Verbose, output.Rotation);
				Gorgon.Log.Print("===================================================================", Diagnostics.LoggingLevel.Verbose);

				output.VideoModes.Refresh(device);
			}

			Gorgon.Log.Print("Found {0} outputs for video device '{1}'.", Diagnostics.LoggingLevel.Simple, _outputs.Count, _videoDevice.Name);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonVideoOutputCollection"/> class.
		/// </summary>
		/// <param name="videoDevice">Video device that owns the outputs in this collection.</param>
		internal GorgonVideoOutputCollection(GorgonVideoDevice videoDevice)
		{
			if (videoDevice == null)
				throw new ArgumentNullException("videoDevice");

			_outputs = new List<GorgonVideoOutput>();
			_videoDevice = videoDevice;
		}
		#endregion

		#region IList<GorgonVideoOutput> Members
		#region Properties.
		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <returns>
		/// The element at the specified index.
		///   </returns>
		///   
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
		///   </exception>
		public GorgonVideoOutput this[int index]
		{
			get
			{
				return _outputs[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		/// <returns>
		/// The index of <paramref name="item"/> if found in the list; otherwise, -1.
		/// </returns>
		public int IndexOf(GorgonVideoOutput item)
		{
			return _outputs.IndexOf(item);
		}

		/// <summary>
		/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
		/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
		///   </exception>
		void IList<GorgonVideoOutput>.Insert(int index, GorgonVideoOutput item)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
		///   </exception>
		void IList<GorgonVideoOutput>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}
		#endregion
		#endregion

		#region ICollection<GorgonVideoOutput> Members
		#region Properties.
		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		///   </returns>
		public int Count
		{
			get 
			{
				return _outputs.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
		///   </returns>
		public bool IsReadOnly
		{
			get 
			{
				return true;
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		///   </exception>
		void ICollection<GorgonVideoOutput>.Add(GorgonVideoOutput item)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		///   </exception>
		void ICollection<GorgonVideoOutput>.Clear()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
		/// </returns>
		public bool Contains(GorgonVideoOutput item)
		{
			return _outputs.Contains(item);
		}

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="arrayIndex">Index of the array.</param>
		public void CopyTo(GorgonVideoOutput[] array, int arrayIndex)
		{
			_outputs.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">
		/// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		///   </exception>
		bool ICollection<GorgonVideoOutput>.Remove(GorgonVideoOutput item)
		{
			throw new NotImplementedException();
		}
		#endregion
		#endregion

		#region IEnumerable<GorgonVideoOutput> Members
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<GorgonVideoOutput> GetEnumerator()
		{
			foreach (var item in _outputs)
				yield return item;
		}

		#endregion

		#region IEnumerable Members
		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}
