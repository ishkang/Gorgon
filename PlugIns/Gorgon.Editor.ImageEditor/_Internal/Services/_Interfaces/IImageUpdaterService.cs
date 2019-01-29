﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2019 Michael Winsor
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
// Created: January 21, 2019 8:58:05 AM
// 
#endregion

using DX = SharpDX;
using Gorgon.Graphics.Imaging;
using Gorgon.UI;

namespace Gorgon.Editor.ImageEditor
{
    /// <summary>
    /// Provides functionality to update an image.
    /// </summary>
    internal interface IImageUpdaterService
    {
        /// <summary>
        /// Function to copy an image onto another image, using the supplied alignment.
        /// </summary>
        /// <param name="srcImage">The image to copy.</param>
        /// <param name="destImage">The destination image.</param>
        /// <param name="startMip">The starting mip map level to copy.</param>
        /// <param name="startArrayOrDepth">The starting array index for 2D images, or depth slice for 3D images.</param>
        /// <param name="alignment">The alignment of the image, relative to the source image.</param>
        void CopyTo(IGorgonImage srcImage, IGorgonImage destImage, int startMip, int startArrayOrDepth, Alignment alignment);

        /// <summary>
        /// Function to resize the image to fit within the width and height specified.
        /// </summary>
        /// <param name="resizeImage">The image tor resize.</param>
        /// <param name="newSize">The new size for the image.</param>
        /// <param name="filter">The filter to apply when resizing.</param>
        /// <param name="preserveAspect"><b>true</b> to preserve the aspect ratio of the image, <b>false</b> to ignore it.</param>
        void Resize(IGorgonImage resizeImage, DX.Size2 newSize, ImageFilter filter, bool preserveAspect);

        /// <summary>
        /// Function to crop an image.
        /// </summary>
        /// <param name="cropImage">The image to crop.</param>
        /// <param name="destSize">The new size of the image.</param>
        /// <param name="alignment">The location to start cropping from.</param>
        void CropTo(IGorgonImage cropImage, DX.Size2 destSize, Alignment alignment);
    }
}