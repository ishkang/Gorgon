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
// Created: Thursday, December 15, 2011 10:43:47 AM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D3D = SharpDX.Direct3D11;
using GorgonLibrary.Native;
using GorgonLibrary.Diagnostics;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// Primitive type.
	/// </summary>
	public enum PrimitiveType
	{
		/// <summary>
		/// Unknown.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// A list of points.
		/// </summary>
		PointList = 1,
		/// <summary>
		/// A list of lines.
		/// </summary>
		LineList = 2,
		/// <summary>
		/// A strip of lines.
		/// </summary>
		LineStrip = 3,
		/// <summary>
		/// A list of triangles.
		/// </summary>
		TriangleList = 4,
		/// <summary>
		/// A strip of triangles.
		/// </summary>
		TriangleStrip = 5,
		/// <summary>
		/// A list of lines including adjacency information.
		/// </summary>
		LineListWithAdjacency = 10,
		/// <summary>
		/// A strip of lines including adjacency information.
		/// </summary>
		LineStripWithAdjacency = 11,
		/// <summary>
		/// A list of triangles including adjacency information.
		/// </summary>
		TriangleListWithAdjacency = 12,
		/// <summary>
		/// A strip of triangles including adjacency information.
		/// </summary>
		TriangleStripWithAdjacency = 13,
		/// <summary>
		/// A patch list with 1 control point.
		/// </summary>
		PatchListWith1ControlPoints = 33,
		/// <summary>
		/// A patch list with 2 control points.
		/// </summary>
		PatchListWith2ControlPoints = 34,
		/// <summary>
		/// A patch list with 3 control points.
		/// </summary>
		PatchListWith3ControlPoints = 35,
		/// <summary>
		/// A patch list with 4 control points.
		/// </summary>
		PatchListWith4ControlPoints = 36,
		/// <summary>
		/// A patch list with 5 control points.
		/// </summary>
		PatchListWith5ControlPoints = 37,
		/// <summary>
		/// A patch list with 6 control points.
		/// </summary>
		PatchListWith6ControlPoints = 38,
		/// <summary>
		/// A patch list with 7 control points.
		/// </summary>
		PatchListWith7ControlPoints = 39,
		/// <summary>
		/// A patch list with 8 control points.
		/// </summary>
		PatchListWith8ControlPoints = 40,
		/// <summary>
		/// A patch list with 9 control points.
		/// </summary>
		PatchListWith9ControlPoints = 41,
		/// <summary>
		/// A patch list with 10 control points.
		/// </summary>
		PatchListWith10ControlPoints = 42,
		/// <summary>
		/// A patch list with 11 control points.
		/// </summary>
		PatchListWith11ControlPoints = 43,
		/// <summary>
		/// A patch list with 12 control points.
		/// </summary>
		PatchListWith12ControlPoints = 44,
		/// <summary>
		/// A patch list with 13 control points.
		/// </summary>
		PatchListWith13ControlPoints = 45,
		/// <summary>
		/// A patch list with 14 control points.
		/// </summary>
		PatchListWith14ControlPoints = 46,
		/// <summary>
		/// A patch list with 15 control points.
		/// </summary>
		PatchListWith15ControlPoints = 47,
		/// <summary>
		/// A patch list with 16 control points.
		/// </summary>
		PatchListWith16ControlPoints = 48,
		/// <summary>
		/// A patch list with 17 control points.
		/// </summary>
		PatchListWith17ControlPoints = 49,
		/// <summary>
		/// A patch list with 18 control points.
		/// </summary>
		PatchListWith18ControlPoints = 50,
		/// <summary>
		/// A patch list with 19 control points.
		/// </summary>
		PatchListWith19ControlPoints = 51,
		/// <summary>
		/// A patch list with 20 control points.
		/// </summary>
		PatchListWith20ControlPoints = 52,
		/// <summary>
		/// A patch list with 21 control points.
		/// </summary>
		PatchListWith21ControlPoints = 53,
		/// <summary>
		/// A patch list with 22 control points.
		/// </summary>
		PatchListWith22ControlPoints = 54,
		/// <summary>
		/// A patch list with 23 control points.
		/// </summary>
		PatchListWith23ControlPoints = 55,
		/// <summary>
		/// A patch list with 24 control points.
		/// </summary>
		PatchListWith24ControlPoints = 56,
		/// <summary>
		/// A patch list with 25 control points.
		/// </summary>
		PatchListWith25ControlPoints = 57,
		/// <summary>
		/// A patch list with 26 control points.
		/// </summary>
		PatchListWith26ControlPoints = 58,
		/// <summary>
		/// A patch list with 27 control points.
		/// </summary>
		PatchListWith27ControlPoints = 59,
		/// <summary>
		/// A patch list with 28 control points.
		/// </summary>
		PatchListWith28ControlPoints = 60,
		/// <summary>
		/// A patch list with 29 control points.
		/// </summary>
		PatchListWith29ControlPoints = 61,
		/// <summary>
		/// A patch list with 30 control points.
		/// </summary>
		PatchListWith30ControlPoints = 62,
		/// <summary>
		/// A patch list with 31 control points.
		/// </summary>
		PatchListWith31ControlPoints = 63,
		/// <summary>
		/// A patch list with 32 control points.
		/// </summary>
		PatchListWith32ControlPoints = 64,
	}

	/// <summary>
	/// Manages the input bindings such as the vertex/index buffer, input layout and primitive types.
	/// </summary>
	public sealed class GorgonInputGeometry
	{
		#region Variables.
		private PrimitiveType _primitiveType = PrimitiveType.Unknown;		// Primitive type to use.
		private GorgonInputLayout _inputLayout = null;						// The current input layout.
		private GorgonGraphics _graphics = null;							// Current graphics interface.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to set or return the primtive type.
		/// </summary>
		public PrimitiveType PrimitiveType
		{
			get
			{
				return _primitiveType;
			}
			set
			{
				if (_primitiveType != value)
				{
					_primitiveType = value;
					_graphics.Context.InputAssembler.PrimitiveTopology = (SharpDX.Direct3D.PrimitiveTopology)value;
				}
			}
		}

		/// <summary>
		/// Property to set or return the input layout.
		/// </summary>
		public GorgonInputLayout Layout
		{
			get
			{
				return _inputLayout;
			}
			set
			{
				if (_inputLayout != value)
				{
					_inputLayout = value;
					if (_inputLayout != null)
						_graphics.Context.InputAssembler.InputLayout = _inputLayout.Convert(_graphics.D3DDevice);
					else
						_graphics.Context.InputAssembler.InputLayout = null;
				}
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to create a index buffer.
		/// </summary>
		/// <param name="size">Size of the buffer, in bytes.</param>
		/// <param name="is32bit">TRUE to indicate that we're using 32 bit indices, FALSE to use 16 bit indices </param>
		/// <param name="usage">Usage of the buffer.</param>
		/// <returns>A new index buffer.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="size"/> parameter is less than 1.</exception>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="usage"/> parameter is set to Staging or Immutable.
		/// </exception>
		public GorgonIndexBuffer CreateIndexBuffer(int size, bool is32bit, BufferUsage usage)
		{
			if ((usage == BufferUsage.Staging) || (usage == BufferUsage.Immutable))
				throw new ArgumentException("A index buffer cannot be used as a staging or immutable buffer.", "usage");

			return CreateIndexBuffer(size, usage, is32bit, null);
		}

		/// <summary>
		/// Function to create a index buffer.
		/// </summary>
		/// <param name="usage">Usage of the buffer.</param>
		/// <param name="is32bit">TRUE to indicate that we're using 32 bit indices, FALSE to use 16 bit indices </param>
		/// <param name="data">Data used to initialize the buffer.</param>
		/// <typeparam name="T">Type of data used to populate the buffer.</typeparam>
		/// <returns>A new index buffer.</returns>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="usage"/> parameter is set to Staging.
		/// <para>-or-</para>
		/// <para>Thrown when the usage parameter is set to Immutable and the <paramref name="data"/> is NULL (Nothing in VB.Net) or empty.</para>
		/// </exception>
		/// <remarks>If creating an immutable index buffer, be sure to pre-populate it via the initialData parameter.</remarks>
		public GorgonIndexBuffer CreateIndexBuffer<T>(BufferUsage usage, bool is32bit, IList<T> data)
			where T : struct
		{
			int size = data.Count * DirectAccess.SizeOf<T>();

			using (GorgonDataStream dataStream = new GorgonDataStream(size))
			{
				for (int i = 0; i < data.Count; i++)
					dataStream.Write<T>(data[i]);
				dataStream.Position = 0;
				return CreateIndexBuffer(size, usage, is32bit, dataStream);
			}
		}

		/// <summary>
		/// Function to create a index buffer.
		/// </summary>
		/// <param name="size">Size of the buffer, in bytes.</param>
		/// <param name="usage">Usage of the buffer.</param>
		/// <param name="is32bit">TRUE to indicate that we're using 32 bit indices, FALSE to use 16 bit indices </param>
		/// <param name="initialData">Initial data to populate the index buffer with.</param>
		/// <returns>A new index buffer.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="size"/> parameter is less than 1.</exception>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="usage"/> parameter is set to Staging.
		/// <para>-or-</para>
		/// <para>Thrown when the usage parameter is set to Immutable and the <paramref name="initialData"/> is NULL (Nothing in VB.Net).</para>
		/// </exception>
		/// <remarks>If creating an immutable index buffer, be sure to pre-populate it via the initialData parameter.</remarks>
		public GorgonIndexBuffer CreateIndexBuffer(int size, BufferUsage usage, bool is32bit, GorgonDataStream initialData)
		{
			if (size < 1)
				throw new ArgumentOutOfRangeException("size", "A index buffer needs at least 1 byte.");

			if (usage == BufferUsage.Staging)
				throw new ArgumentException("A index buffer cannot be used as a staging buffer.", "usage");

			if ((usage == BufferUsage.Immutable) && ((initialData == null) || (initialData.Length == 0)))
				throw new ArgumentException("Cannot create an immutable buffer without initial data to populate it.", "usage");

			GorgonIndexBuffer buffer = new GorgonIndexBuffer(_graphics, usage, size, is32bit);
			buffer.Initialize(initialData);

			_graphics.TrackedObjects.Add(buffer);
			return buffer;
		}

		/// <summary>
		/// Function to create a vertex buffer.
		/// </summary>
		/// <param name="size">Size of the buffer, in bytes.</param>
		/// <param name="usage">Usage of the buffer.</param>
		/// <returns>A new vertex buffer.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="size"/> parameter is less than 1.</exception>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="usage"/> parameter is set to Staging or Immutable.
		/// </exception>
		public GorgonVertexBuffer CreateVertexBuffer(int size, BufferUsage usage)
		{
			if ((usage == BufferUsage.Staging) || (usage == BufferUsage.Immutable))
				throw new ArgumentException("A vertex buffer cannot be used as a staging or immutable buffer.", "usage");

			return CreateVertexBuffer(size, usage, null);
		}

		/// <summary>
		/// Function to create a vertex buffer.
		/// </summary>
		/// <param name="usage">Usage of the buffer.</param>
		/// <param name="data">Data used to initialize the buffer.</param>
		/// <typeparam name="T">Type of data used to populate the buffer.</typeparam>
		/// <returns>A new vertex buffer.</returns>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="usage"/> parameter is set to Staging.
		/// <para>-or-</para>
		/// <para>Thrown when the usage parameter is set to Immutable and the <paramref name="data"/> is NULL (Nothing in VB.Net) or empty.</para>
		/// </exception>
		/// <remarks>If creating an immutable vertex buffer, be sure to pre-populate it via the initialData parameter.</remarks>
		public GorgonVertexBuffer CreateVertexBuffer<T>(BufferUsage usage, IList<T> data)
			where T : struct
		{
			GorgonDebug.AssertNull<IList<T>>(data, "data");
			int size = data.Count * DirectAccess.SizeOf<T>();

			using (GorgonDataStream dataStream = new GorgonDataStream(size))
			{
				for (int i = 0; i < data.Count; i++)
					dataStream.Write<T>(data[i]);
				dataStream.Position = 0;
				return CreateVertexBuffer(size, usage, dataStream);
			}
		}

		/// <summary>
		/// Function to create a vertex buffer.
		/// </summary>
		/// <param name="size">Size of the buffer, in bytes.</param>
		/// <param name="usage">Usage of the buffer.</param>
		/// <param name="initialData">Initial data to populate the vertex buffer with.</param>
		/// <returns>A new vertex buffer.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="size"/> parameter is less than 1.</exception>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="usage"/> parameter is set to Staging.
		/// <para>-or-</para>
		/// <para>Thrown when the usage parameter is set to Immutable and the <paramref name="initialData"/> is NULL (Nothing in VB.Net).</para>
		/// </exception>
		/// <remarks>If creating an immutable vertex buffer, be sure to pre-populate it via the initialData parameter.</remarks>
		public GorgonVertexBuffer CreateVertexBuffer(int size, BufferUsage usage, GorgonDataStream initialData)
		{
			if (size < 1)
				throw new ArgumentOutOfRangeException("size", "A vertex buffer needs at least 1 byte.");

			if (usage == BufferUsage.Staging)
				throw new ArgumentException("A vertex buffer cannot be used as a staging buffer.", "usage");

			if ((usage == BufferUsage.Immutable) && ((initialData == null) || (initialData.Length == 0)))
				throw new ArgumentException("Cannot create an immutable buffer without initial data to populate it.", "usage");

			GorgonVertexBuffer buffer = new GorgonVertexBuffer(_graphics, usage, size);
			buffer.Initialize(initialData);

			_graphics.TrackedObjects.Add(buffer);
			return buffer;
		}

		/// <summary>
		/// Function to create an input layout object from a predefined type.
		/// </summary>
		/// <param name="name">Name of the input layout.</param>
		/// <param name="type">Type to evaluate.</param>
		/// <param name="shader">The shader that holds the input layout signature.</param>
		/// <returns>The input layout object to create.</returns>
		/// <exception cref="System.ArgumentException">Thrown when then <paramref name="name"/> parameter is an empty string.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="shader"/> parameter is NULL (Nothing in VB.Net).
		/// <para>-or-</para>
		/// <para>Thrown when the <paramref name="type"/> parameter is NULL.</para>
		/// <para>-or-</para>
		/// <para>Thrown when the name parameter is NULL.</para>
		/// </exception>
		/// <remarks>The shader parameter is used to compare input layout on the shader side with the input layout.  If the layout is mismatched, a warning will appear in the debug output.
		/// <para>Note that any shader can be used with the input layout as long as the shader contains the same layout for the input, i.e. there is no need to create a new layout for each shader if the element layouts are identical.</para>
		/// </remarks>
		public GorgonInputLayout CreateInputLayout(string name, Type type, GorgonShader shader)
		{
			GorgonInputLayout layout = null;

			GorgonDebug.AssertNull<Type>(type, "type");
			GorgonDebug.AssertNull<GorgonShader>(shader, "shader");

			layout = new GorgonInputLayout(_graphics, name, shader);
			layout.GetLayoutFromType(type);

			_graphics.TrackedObjects.Add(layout);

			return layout;
		}

		/// <summary>
		/// Function to create an input layout object.
		/// </summary>
		/// <param name="name">Name of the input layout.</param>
		/// <param name="shader">The shader that holds the input layout signature.</param>
		/// <returns>The input layout object to create.</returns>
		/// <exception cref="System.ArgumentException">Thrown when then name parameter is an empty string.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="shader"/> parameter is NULL (Nothing in VB.Net).
		/// <para>-or-</para>
		/// <para>Thrown when the <paramref name="name"/> parameter is NULL.</para>
		/// </exception>
		/// <remarks>The shader parameter is used to compare input layout on the shader side with the input layout.  If the layout is mismatched, a warning will appear in the debug output.
		/// <para>Note that any shader can be used with the input layout as long as the shader contains the same layout for the input, i.e. there is no need to create a new layout for each shader if the element layouts are identical.</para>
		/// </remarks>
		public GorgonInputLayout CreateInputLayout(string name, GorgonShader shader)
		{
			GorgonInputLayout layout = null;

			GorgonDebug.AssertNull<GorgonShader>(shader, "shader");
			layout = new GorgonInputLayout(_graphics, name, shader);

			_graphics.TrackedObjects.Add(layout);
			return layout;
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonInputGeometry"/> class.
		/// </summary>
		internal GorgonInputGeometry(GorgonGraphics graphics)
		{
			_graphics = graphics;
			PrimitiveType = PrimitiveType.TriangleList;
		}
		#endregion
	}
}
