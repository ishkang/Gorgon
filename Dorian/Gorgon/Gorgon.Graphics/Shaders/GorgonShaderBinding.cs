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
// Created: Tuesday, January 31, 2012 8:21:21 AM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Shaders = SharpDX.D3DCompiler;
using GorgonLibrary.Diagnostics;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// Used to manage shader bindings and shader buffers.
	/// </summary>
	public sealed class GorgonShaderBinding
	{
		#region Constants.
		/// <summary>
		/// Header for Gorgon binary shaders.
		/// </summary>
		internal const string BinaryShaderHeader = "GORBINSHD2.0";
		#endregion

		#region Variables.
		private GorgonGraphics _graphics = null;		// Graphics interface.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to return the current vertex shader states.
		/// </summary>
		public GorgonVertexShaderState VertexShader
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to return the current vertex shader states.
		/// </summary>
		public GorgonPixelShaderState PixelShader
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to return a list of include files for the shaders.
		/// </summary>
		public GorgonShaderIncludeCollection IncludeFiles
		{
			get;
			private set;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function clean up any resources within this interface.
		/// </summary>
		internal void CleanUp()
		{
			if (PixelShader != null)
				PixelShader.Dispose();
			if (VertexShader != null)
				VertexShader.Dispose();

			PixelShader = null;
			VertexShader = null;
		}

		/// <summary>
		/// Function to re-seat a shader after it's been altered.
		/// </summary>
		/// <param name="shader">Shader to re-seat.</param>
		internal void Reseat(GorgonShader shader)
		{
			if (shader is GorgonPixelShader)
			{
				if (PixelShader.Current == shader)
				{
					PixelShader.Current = null;
					PixelShader.Current = (GorgonPixelShader)shader;
				}
			}

			if (shader is GorgonVertexShader)
			{
				if (VertexShader.Current == shader)
				{
					VertexShader.Current = null;
					VertexShader.Current = (GorgonVertexShader)shader;
				}
			}
		}

		/// <summary>
		/// Function to re-seat a texture after it's been altered.
		/// </summary>
		/// <param name="texture">Texture to re-seat.</param>
		internal void Reseat(GorgonTexture texture)
		{
			PixelShader.Textures.ReSeat(texture);
			VertexShader.Textures.ReSeat(texture);
		}

		/// <summary>
		/// Function to unbind a texture from all shaders.
		/// </summary>
		/// <param name="texture">Texture to unbind.</param>
		internal void Unbind(GorgonTexture texture)
		{
			PixelShader.Textures.Unbind(texture);
			VertexShader.Textures.Unbind(texture);
		}

		/// <summary>
		/// Function to create an effect object.
		/// </summary>
		/// <typeparam name="T">Type of effect to create.</typeparam>
		/// <param name="name">Name of the effect.</param>
		/// <param name="passCount">Number of passes in the effect.</param>
		/// <returns>The new effect object.</returns>
		/// <remarks>Effects are used to simplify rendering with multiple passes when using a shader.</remarks>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="name"/> parameter is NULL (Nothing in VB.Net).</exception>
		/// <exception cref="System.ArgumentException">Thrown when the name parameter is an empty string.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="passCount"/> parameter is less than 0.</exception>
		public T CreateEffect<T>(string name, int passCount)
			where T : GorgonEffect
		{
			T effect = (T)Activator.CreateInstance(typeof(T), new object[] {_graphics, name, passCount});

			_graphics.AddTrackedObject(effect);

			return effect;
		}

		/// <summary>
		/// Function to create a constant buffer.
		/// </summary>
		/// <param name="size">Size of the buffer, in bytes.</param>
		/// <param name="allowCPUWrite">TRUE to allow the CPU to write to the buffer, FALSE to disallow.</param>
		/// <returns>A new constant buffer.</returns>
		public GorgonConstantBuffer CreateConstantBuffer(int size, bool allowCPUWrite)
		{
			return CreateConstantBuffer(size, allowCPUWrite, null);
		}

		/// <summary>
		/// Function to create a constant buffer.
		/// </summary>
		/// <param name="size">Size of the buffer, in bytes.</param>
		/// <param name="allowCPUWrite">TRUE to allow the CPU to write to the buffer, FALSE to disallow.</param>
		/// <param name="stream">Stream used to initialize the buffer.</param>
		/// <returns>A new constant buffer.</returns>
		public GorgonConstantBuffer CreateConstantBuffer(int size, bool allowCPUWrite, GorgonDataStream stream)
		{			
			GorgonConstantBuffer buffer = new GorgonConstantBuffer(_graphics, size, allowCPUWrite);
			buffer.Initialize(stream);
			_graphics.AddTrackedObject(buffer);
			return buffer;
		}

		/// <summary>
		/// Function to create a constant buffer and initializes it with data.
		/// </summary>
		/// <typeparam name="T">Type of data to pass to the constant buffer.</typeparam>
		/// <param name="value">Value to write to the buffer</param>
		/// <param name="allowCPUWrite">TRUE to allow the CPU to write to the buffer, FALSE to disallow.</param>
		/// <returns>A new constant buffer.</returns>
		public GorgonConstantBuffer CreateConstantBuffer<T>(T value, bool allowCPUWrite)
			where T : struct
		{
			using (GorgonDataStream stream = GorgonDataStream.ValueTypeToStream<T>(value))
			{
				GorgonConstantBuffer buffer = new GorgonConstantBuffer(_graphics, (int)stream.Length, allowCPUWrite);
				buffer.Initialize(stream);

				_graphics.AddTrackedObject(buffer);
				return buffer;
			}
		}

		/// <summary>
		/// Function to load a shader from a stream of data.
		/// </summary>
		/// <typeparam name="T">The shader type.  Must be inherited from <see cref="GorgonLibrary.Graphics.GorgonShader">GorgonShader</see>.</typeparam>
		/// <param name="name">Name of the shader object.</param>
		/// <param name="entryPoint">Entry point method to call in the shader.</param>
		/// <param name="stream">Stream to load the shader from.</param>
		/// <param name="size">Size of the shader, in bytes.</param>
		/// <returns>The new shader loaded from the data stream.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="stream"/>, <paramref name="name"/> or <paramref name="entryPoint"/> parameters are NULL (Nothing in VB.Net).
		/// </exception>
		/// <exception cref="System.ArgumentException">Thrown when the name or entryPoint parameters are empty.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="size"/> parameter is less than or equal to 0.</exception>
		/// <exception cref="System.TypeInitializationException">Thrown when the type of shader is unrecognized.</exception>
		/// <exception cref="GorgonLibrary.GorgonException">Thrown when there are compile errors in the shader.</exception>
		public T FromStream<T>(string name, string entryPoint, Stream stream, int size)
			where T : GorgonShader
		{
#if DEBUG
			return FromStream<T>(name, entryPoint, true, stream, size);
#else
			return FromStream<T>(name, entryPoint, false, stream, size);
#endif
		}

		/// <summary>
		/// Function to load a shader from a stream of data.
		/// </summary>
		/// <typeparam name="T">The shader type.  Must be inherited from <see cref="GorgonLibrary.Graphics.GorgonShader">GorgonShader</see>.</typeparam>
		/// <param name="name">Name of the shader object.</param>
		/// <param name="entryPoint">Entry point method to call in the shader.</param>
		/// <param name="isDebug">TRUE to apply debug information, FALSE to exclude it.</param>
		/// <param name="stream">Stream to load the shader from.</param>
		/// <param name="size">Size of the shader, in bytes.</param>
		/// <returns>The new shader loaded from the data stream.</returns>
		/// <remarks>The <paramref name="isDebug"/> parameter is only applicable to source code shaders.</remarks>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="stream"/>, <paramref name="name"/> or <paramref name="entryPoint"/> parameters are NULL (Nothing in VB.Net).
		/// </exception>
		/// <exception cref="System.ArgumentException">Thrown when the name or entryPoint parameters are empty.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="size"/> parameter is less than or equal to 0.</exception>
		/// <exception cref="System.TypeInitializationException">Thrown when the type of shader is unrecognized.</exception>
		/// <exception cref="GorgonLibrary.GorgonException">Thrown when there are compile errors in the shader.</exception>
		public T FromStream<T>(string name, string entryPoint, bool isDebug, Stream stream, int size)
			where T : GorgonShader
		{
			bool isBinary = false;
			GorgonShader shader = null;
			string sourceCode = string.Empty;
			byte[] shaderData = null;
			byte[] header = null;
			long streamPosition = 0;

			GorgonDebug.AssertNull<Stream>(stream, "stream");
			GorgonDebug.AssertParamString(name, "name");
			GorgonDebug.AssertParamString(entryPoint, "entryPoint");
			
#if DEBUG
			if ( size <= 0)
				throw new ArgumentOutOfRangeException("size", "The size must be greater than 0 bytes.");
#endif
			streamPosition = stream.Position;

			// Check for the binary header.  If we have it, load the file as a binary file.
			// Otherwise load it as source code.
			header = new byte[Encoding.UTF8.GetBytes(BinaryShaderHeader).Length];
			stream.Read(header, 0, header.Length);
			isBinary = (string.Compare(Encoding.UTF8.GetString(header), BinaryShaderHeader, true) == 0);
			if (isBinary)
				shaderData = new byte[size - BinaryShaderHeader.Length];
			else
			{
				stream.Position = streamPosition;
				shaderData = new byte[size];
			}

			stream.Read(shaderData, 0, shaderData.Length);

			if (isBinary)
			{
				shader = CreateShader<T>(name, entryPoint, string.Empty, isDebug);
				shader.D3DByteCode = new Shaders.ShaderBytecode(shaderData);
				shader.LoadShader();
			}
			else
			{
				sourceCode = Encoding.UTF8.GetString(shaderData);
				shader = CreateShader<T>(name, entryPoint, sourceCode, isDebug);
			}

			return (T)shader;
		}

		/// <summary>
		/// Function to load a shader from a file.
		/// </summary>
		/// <typeparam name="T">The shader type.  Must be inherited from <see cref="GorgonLibrary.Graphics.GorgonShader">GorgonShader</see>.</typeparam>
		/// <param name="name">Name of the shader object.</param>
		/// <param name="entryPoint">Entry point method to call in the shader.</param>
		/// <param name="fileName">File name and path to the shader file.</param>
		/// <returns>The new shader loaded from the file.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="name"/>, <paramref name="entryPoint"/> or <paramref name="fileName"/> parameters are NULL (Nothing in VB.Net).
		/// </exception>
		/// <exception cref="System.ArgumentException">Thrown when the name, entryPoint or fileName parameters are empty.</exception>
		/// <exception cref="System.TypeInitializationException">Thrown when the type of shader is unrecognized.</exception>
		/// <exception cref="GorgonLibrary.GorgonException">Thrown when there are compile errors in the shader.</exception>
		public T FromFile<T>(string name, string entryPoint, string fileName)
			where T : GorgonShader
		{
#if DEBUG
			return FromFile<T>(name, entryPoint, true, fileName);
#else
			return FromFile<T>(name, entryPoint, false, fileName);
#endif
		}

		/// <summary>
		/// Function to load a shader from a file.
		/// </summary>
		/// <typeparam name="T">The shader type.  Must be inherited from <see cref="GorgonLibrary.Graphics.GorgonShader">GorgonShader</see>.</typeparam>
		/// <param name="name">Name of the shader object.</param>
		/// <param name="entryPoint">Entry point method to call in the shader.</param>
		/// <param name="isDebug">TRUE to apply debug information, FALSE to exclude it.</param>
		/// <param name="fileName">File name and path to the shader file.</param>
		/// <returns>The new shader loaded from the file.</returns>
		/// <remarks>The <paramref name="isDebug"/> parameter is only applicable to source code shaders.</remarks>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="name"/>, <paramref name="entryPoint"/> or <paramref name="fileName"/> parameters are NULL (Nothing in VB.Net).
		/// </exception>
		/// <exception cref="System.ArgumentException">Thrown when the name, entryPoint or fileName parameters are empty.</exception>
		/// <exception cref="System.TypeInitializationException">Thrown when the type of shader is unrecognized.</exception>
		/// <exception cref="GorgonLibrary.GorgonException">Thrown when there are compile errors in the shader.</exception>
		public T FromFile<T>(string name, string entryPoint, bool isDebug, string fileName)
			where T : GorgonShader
		{
			FileStream stream = null;

			try
			{
				stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				return FromStream<T>(name, entryPoint, isDebug, stream, (int)stream.Length);
			}
			finally
			{
				if (stream != null)
					stream.Dispose();
				stream = null;
			}
		}

		/// <summary>
		/// Function to create a shader.
		/// </summary>
		/// <typeparam name="T">The shader type.  Must be inherited from <see cref="GorgonLibrary.Graphics.GorgonShader">GorgonShader</see>.</typeparam>
		/// <param name="name">Name of the shader.</param>
		/// <param name="entryPoint">Entry point for the shader.</param>
		/// <param name="sourceCode">Source code for the shader.</param>
		/// <param name="debug">TRUE to include debug information, FALSE to exclude.</param>
		/// <returns>A new vertex shader.</returns>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="name"/> or <paramref name="entryPoint"/> parameters are empty strings.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown when the name or entryPoint parameters are NULL (Nothing in VB.Net).</exception>
		/// <exception cref="System.TypeInitializationException">Thrown when the type of shader is unrecognized.</exception>
		/// <exception cref="GorgonLibrary.GorgonException">Thrown when there are compile errors in the shader.</exception>
		public T CreateShader<T>(string name, string entryPoint, string sourceCode, bool debug)
			where T : GorgonShader
		{
			GorgonShader shader = null;

			GorgonDebug.AssertParamString(name, "name");
			GorgonDebug.AssertParamString(entryPoint, "entryPoint");

			if (typeof(T) == typeof(GorgonVertexShader))
				shader = new GorgonVertexShader(_graphics, name, entryPoint);

			if (typeof(T) == typeof(GorgonPixelShader))
				shader = new GorgonPixelShader(_graphics, name, entryPoint);

			if (shader == null)
				throw new TypeInitializationException(typeof(T).FullName, null);

			shader.IsDebug = debug;
			if (!string.IsNullOrEmpty(sourceCode))
			{
				shader.SourceCode = sourceCode;
				shader.Compile();
			}
			_graphics.AddTrackedObject(shader);

			return (T)shader;
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonShaderBinding"/> class.
		/// </summary>
		/// <param name="graphics">The graphics.</param>
		internal GorgonShaderBinding(GorgonGraphics graphics)
		{
			IncludeFiles = new GorgonShaderIncludeCollection();
			VertexShader = new GorgonVertexShaderState(graphics);
			PixelShader = new GorgonPixelShaderState(graphics);
			_graphics = graphics;
		}
		#endregion
	}
}