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
// Created: Thursday, December 15, 2011 12:49:30 PM
// 
#endregion

using D3DCompiler = SharpDX.D3DCompiler;
using D3D = SharpDX.Direct3D11;

namespace Gorgon.Graphics
{
	/// <summary>
	/// A pixel shader object.
	/// </summary>
	public class GorgonPixelShader
		: GorgonShader
	{
		#region Properties.
		/// <summary>
		/// Property to return the Direct3D pixel shader.
		/// </summary>
		internal D3D.PixelShader D3DShader
		{
			get;
		}

		/// <summary>
		/// Property to return the type of shader.
		/// </summary>
		public override ShaderType ShaderType => ShaderType.Pixel;
		#endregion

		#region Methods.
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
			D3DShader?.Dispose();
			base.Dispose();
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonShader" /> class.
		/// </summary>
		/// <param name="videoDevice">The video device used to create the shader.</param>
		/// <param name="name">The name for this shader.</param>
		/// <param name="isDebug"><b>true</b> if debug information is included in the byte code, <b>false</b> if not.</param>
		/// <param name="byteCode">The byte code for the shader.</param>
		internal GorgonPixelShader(IGorgonVideoDevice videoDevice, string name, bool isDebug, D3DCompiler.ShaderBytecode byteCode)
			: base(videoDevice, name, isDebug, byteCode)
		{
			D3DShader = new D3D.PixelShader(videoDevice.D3DDevice(), byteCode)
			            {
				            DebugName = name + " D3D11PixelShader"
			            };
		}
		#endregion
	}
}
