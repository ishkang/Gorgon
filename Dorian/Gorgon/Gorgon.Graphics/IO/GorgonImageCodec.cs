﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2013 Michael Winsor
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
// Created: Wednesday, February 6, 2013 5:59:03 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WIC = SharpDX.WIC;
using GorgonLibrary.Native;
using GorgonLibrary.Math;
using GorgonLibrary.Graphics;

namespace GorgonLibrary.IO
{
	/// <summary>
	/// Filter to be applied to an image that's been stretched or shrunk.
	/// </summary>
	public enum ImageFilter
	{
		/// <summary>
		/// The output pixel is assigned the value of the pixel that the point falls within. No other pixels are considered.
		/// </summary>
		Point = WIC.BitmapInterpolationMode.NearestNeighbor,
		/// <summary>
		/// The output pixel values are computed as a weighted average of the nearest four pixels in a 2x2 grid.
		/// </summary>
		Linear = WIC.BitmapInterpolationMode.Linear,
		/// <summary>
		/// Destination pixel values are computed as a weighted average of the nearest sixteen pixels in a 4x4 grid.
		/// </summary>
		Cubic = WIC.BitmapInterpolationMode.Cubic,
		/// <summary>
		/// Destination pixel values are computed as a weighted average of the all the pixels that map to the new pixel.
		/// </summary>
		Fant = WIC.BitmapInterpolationMode.Fant
	}

	/// <summary>
	/// Filter for dithering an image when it is downsampled to a lower bit depth.
	/// </summary>
	public enum ImageDithering
	{
		/// <summary>
		/// No dithering.
		/// </summary>
		None = WIC.BitmapDitherType.None,
		/// <summary>
		/// A 4x4 ordered dither algorithm.
		/// </summary>
		Dither4x4 = WIC.BitmapDitherType.Ordered4x4,
		/// <summary>
		/// An 8x8 ordered dither algorithm.
		/// </summary>
		Dither8x8 = WIC.BitmapDitherType.Ordered8x8,
		/// <summary>
		/// A 16x16 ordered dither algorithm.
		/// </summary>
		Dither16x16 = WIC.BitmapDitherType.Ordered16x16,
		/// <summary>
		/// An 4x4 spiral dither algorithm.
		/// </summary>
		Spiral4x4 = WIC.BitmapDitherType.Spiral4x4,
		/// <summary>
		/// An 8x8 spiral dither algorithm.
		/// </summary>
		Spiral8x8 = WIC.BitmapDitherType.Spiral8x8,
		/// <summary>
		/// A 4x4 dual spiral dither algorithm.
		/// </summary>
		DualSpiral4x4 = WIC.BitmapDitherType.DualSpiral4x4,
		/// <summary>
		/// An 8x8 dual spiral dither algorithm.
		/// </summary>
		DualSpiral8x8 = WIC.BitmapDitherType.DualSpiral8x8,
		/// <summary>
		/// An error diffusion algorithm.
		/// </summary>
		ErrorDiffusion = WIC.BitmapDitherType.ErrorDiffusion
	}

	/// <summary>
	/// Flags to control how pixel conversion should be handled.
	/// </summary>
	[Flags]
	public enum ImageBitFlags
	{
		/// <summary>
		/// No modifications to conversion process.
		/// </summary>
		None = 0,
		/// <summary>
		/// Set a known opaque alpha value in the alpha channel.
		/// </summary>
		OpaqueAlpha = 1,
		/// <summary>
		/// Enables specific legacy format conversion cases.
		/// </summary>
		Legacy = 2
	}

	/// <summary>
	/// A codec to reading and/or writing image data.
	/// </summary>
	/// <remarks>A codec allows for reading and/or writing of data in an encoded format.  Users may inherit from this object to define their own 
	/// image formats, or use one of the predefined image codecs available in Gorgon.
	/// <para>The codec accepts and returns a <see cref="GorgonLibrary.Graphics.GorgonImageData">GorgonImageData</see> type, which is filled from or read into the encoded file.</para>
	/// </remarks>
	public abstract class GorgonImageCodec
		: INamedObject
	{
		#region Properties.
		/// <summary>
		/// Property to return the common file name extension(s) for a codec.
		/// </summary>
		public IList<string> CodecCommonExtensions
		{
			get;
			protected set;
		}

		/// <summary>
		/// Property to return the friendly description of the format.
		/// </summary>
		public abstract string CodecDescription
		{
			get;
		}

		/// <summary>
		/// Property to return the abbreviated name of the codec (e.g. PNG).
		/// </summary>
		public abstract string Codec
		{
			get;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to copy (or update in-place) a line with opaque alpha substituion (if required).
		/// </summary>
		/// <param name="src">The pointer to the source data.</param>
		/// <param name="srcPitch">The pitch of the source data.</param>
		/// <param name="dest">The pointer to the destination data.</param>
		/// <param name="destPitch">The pitch of the destination data.</param>
		/// <param name="format">Format of the destination buffer.</param>
		/// <param name="bitFlags">Image bit conversion control flags.</param>
		/// <remarks>Use this method to copy a single scanline of an image and (optionally) set an opaque constant alpha value.
		/// <para>This overload is for languages that don't support unsafe code (e.g. Visual Basic .NET).</para>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="format"/> parameter is Unknown.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="src"/> or the <paramref name="dest"/> parameter is IntPtr.Zero.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="srcPitch"/> or the <paramref name="destPitch"/> parameter is less than 0.</exception>
		/// </remarks>
		protected unsafe void CopyScanline(IntPtr src, int srcPitch, IntPtr dest, int destPitch, BufferFormat format, ImageBitFlags bitFlags)
		{
			CopyScanline(src.ToPointer(), srcPitch, dest.ToPointer(), destPitch, format, bitFlags);
		}

		/// <summary>
		/// Function to expand a 16BPP line in an image to a 32BPP RGBA line.
		/// </summary>
		/// <param name="src">The pointer to the source data.</param>
		/// <param name="srcPitch">The pitch of the source data.</param>
		/// <param name="srcFormat">Format to convert from.</param>
		/// <param name="dest">The pointer to the destination data.</param>
		/// <param name="destPitch">The pitch of the destination data.</param>
		/// <param name="bitFlags">Image bit conversion control flags.</param>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="srcFormat"/> is not a 16 BPP format.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="src"/> or the <paramref name="dest"/> parameter is IntPtr.Zero.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="srcPitch"/> or the <paramref name="destPitch"/> parameter is less than 0.</exception>
		/// <remarks>Use this to expand a 16 BPP (B5G6R5 or B5G5R5A1 format) into a 32 BPP R8G8B8A8 (normalized unsigned integer) format.
		/// <para>This overload is for languages that don't support unsafe code (e.g. Visual Basic .NET).</para>
		/// </remarks>
		protected unsafe void Expand16BPPScanline(IntPtr src, int srcPitch, BufferFormat srcFormat, IntPtr dest, int destPitch, ImageBitFlags bitFlags)
		{
			Expand16BPPScanline(src.ToPointer(), srcPitch, srcFormat, dest.ToPointer(), destPitch, bitFlags);
		}

		/// <summary>
		/// Function to expand a 16BPP scan line in an image to a 32BPP RGBA line.
		/// </summary>
		/// <param name="src">The pointer to the source data.</param>
		/// <param name="srcPitch">The pitch of the source data.</param>
		/// <param name="srcFormat">Format to convert from.</param>
		/// <param name="dest">The pointer to the destination data.</param>
		/// <param name="destPitch">The pitch of the destination data.</param>
		/// <param name="bitFlags">Image bit conversion control flags.</param>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="srcFormat" /> is not a 16 BPP format.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="src"/> or the <paramref name="dest"/> parameter is NULL.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="srcPitch"/> or the <paramref name="destPitch"/> parameter is less than 0.</exception>
		/// <remarks>
		/// Use this to expand a 16 BPP (B5G6R5 or B5G5R5A1 format) into a 32 BPP R8G8B8A8 (normalized unsigned integer) format.
		/// </remarks>
		protected unsafe void Expand16BPPScanline(void* src, int srcPitch, BufferFormat srcFormat, void* dest, int destPitch, ImageBitFlags bitFlags)
		{
			var srcPtr = (ushort*)src;
			var destPtr = (uint*)dest;

			if ((srcFormat != BufferFormat.B5G5R5A1_UIntNormal) && (srcFormat != BufferFormat.B5G6R5_UIntNormal))
			{
				throw new ArgumentException("The format '" + srcFormat.ToString() + "' is not a standard 16BPP format (B5G6R5 or B5G5R5A1).", "srcFormat");
			}

			if ((src == null) || (dest == null))
			{
				throw new ArgumentNullException("The source/destination pointer must not be NULL.");
			}

			if ((srcPitch <= 0) || (destPitch <= 0))
			{
				throw new ArgumentOutOfRangeException("The source/destination pitch must be greater than 0");
			}

			for (int srcCount = 0, destCount = 0; ((srcCount < srcPitch) && (destCount < destPitch)); srcCount += 2, destCount += 4)
			{
				ushort srcPixel = *(srcPtr++);
				uint R = 0, G = 0, B = 0, A = 0;

				if (srcFormat == BufferFormat.B5G6R5_UIntNormal)
				{
					R = (uint)((srcPixel & 0xF800) >> 11);
					G = (uint)((srcPixel & 0x07E0) >> 5);
					B = (uint)(srcPixel & 0x001F);

					R = ((R << 3) | (R >> 2));
					G = ((G << 2) | (G >> 4)) << 8;
					B = ((B << 3) | (B >> 2)) << 16;
					A = 0xFF000000;
				}
				else if (srcFormat == BufferFormat.B5G5R5A1_UIntNormal)
				{
					R = (uint)((srcPixel & 0x7C00) >> 10);
					G = (uint)((srcPixel & 0x03E0) >> 5);
					B = (uint)(srcPixel & 0x001F);

					R = ((R << 3) | (R >> 2));
					G = ((G << 3) | (G >> 2)) << 8;
					B = ((B << 3) | (B >> 2)) << 16;

					A = ((bitFlags & ImageBitFlags.OpaqueAlpha) == ImageBitFlags.OpaqueAlpha) ? 0xFF000000 : (((srcPixel & 0x8000) != 0) ? 0xFF000000 : 0);
				}

				*(destPtr++) = R | G | B | A;
			}
		}

		/// <summary>
		/// Function to copy (or update in-place) with bits swizzled to match another format.
		/// </summary>
		/// <param name="src">The pointer to the source data.</param>
		/// <param name="srcPitch">The pitch of the source data.</param>
		/// <param name="dest">The pointer to the destination data.</param>
		/// <param name="destPitch">The pitch of the destination data.</param>
		/// <param name="format">Format of the destination buffer.</param>
		/// <param name="bitFlags">Image bit conversion control flags.</param>
        /// <exception cref="System.ArgumentException">Thrown when the <paramref name="format"/> parameter is Unknown.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="src"/> or the <paramref name="dest"/> parameter is NULL.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="srcPitch"/> or the <paramref name="destPitch"/> parameter is less than 0.</exception>
        /// <remarks>Use this method to copy a single scanline and swizzle the bits of an image and (optionally) set an opaque constant alpha value.</remarks>
		protected unsafe void SwizzleScanline(void* src, int srcPitch, void* dest, int destPitch, BufferFormat format, ImageBitFlags bitFlags)
		{
            int size = srcPitch.Min(destPitch);
            uint r = 0, g = 0, b = 0, a = 0, pixel = 0;

            if ((src == null) || (dest == null))
            {
                throw new ArgumentNullException("The source/destination pointer must not be NULL.");
            }

            if ((srcPitch <= 0) || (destPitch <= 0))
            {
                throw new ArgumentOutOfRangeException("The source/destination pitch must be greater than 0");
            }

            if (format == BufferFormat.Unknown)
            {
                throw new ArgumentException("The format is unknown", "format");
            }

            var srcPtr = (uint*)src;
            var destPtr = (uint*)dest;

            switch (format)
            {
                case BufferFormat.R10G10B10A2:
                case BufferFormat.R10G10B10A2_UInt:
                case BufferFormat.R10G10B10A2_UIntNormal:
                case BufferFormat.R10G10B10_XR_BIAS_A2_UIntNormal:
                    for (int i = 0; i < size; i += 4)
                    {
                        if (src != dest)
                        {
                            pixel = *(srcPtr++);
                        }
                        else
                        {
                            pixel = *(destPtr);
                        }

                        r = (uint)((pixel & 0x3FF00000) >> 20);
                        g = (uint)(pixel & 0x000FFC00);
                        b = (uint)((pixel & 0x000003FF) << 20);
                        a = ((bitFlags & ImageBitFlags.OpaqueAlpha) == ImageBitFlags.OpaqueAlpha) ? 0xC0000000 : pixel & 0xC0000000;

                        *destPtr = r | g | b | a;
                        destPtr++;
                    }
                    return;
                case BufferFormat.R8G8B8A8:
                case BufferFormat.R8G8B8A8_UIntNormal:
                case BufferFormat.R8G8B8A8_UIntNormal_sRGB:
                case BufferFormat.B8G8R8A8_UIntNormal:
                case BufferFormat.B8G8R8X8_UIntNormal:
                case BufferFormat.B8G8R8A8:
                case BufferFormat.B8G8R8A8_UIntNormal_sRGB:
                case BufferFormat.B8G8R8X8:
                case BufferFormat.B8G8R8X8_UIntNormal_sRGB:
                    for (int i = 0; i < size; i += 4)
                    {
                        if (src != dest)
                        {
                            pixel = *(srcPtr++);
                        }
                        else
                        {
                            pixel = *(destPtr);
                        }

                        r = (uint)((pixel & 0xFF0000) >> 16);
                        g = (uint)(pixel & 0x00FF00);
                        b = (uint)((pixel & 0x0000FF) << 16);
                        a = ((bitFlags & ImageBitFlags.OpaqueAlpha) == ImageBitFlags.OpaqueAlpha) ? 0xFF000000 : pixel & 0xFF000000;

                        *destPtr = r | g | b | a;
                        destPtr++;
                    }
                    return;
            }

            if (src != dest)
            {
                DirectAccess.MemoryCopy(dest, src, size);
            }
        }


		/// <summary>
		/// Function to copy (or update in-place) a line with opaque alpha substituion (if required).
		/// </summary>
		/// <param name="src">The pointer to the source data.</param>
		/// <param name="srcPitch">The pitch of the source data.</param>
		/// <param name="dest">The pointer to the destination data.</param>
		/// <param name="destPitch">The pitch of the destination data.</param>
		/// <param name="format">Format of the destination buffer.</param>
		/// <param name="bitFlags">Image bit conversion control flags.</param>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="format"/> parameter is Unknown.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="src"/> or the <paramref name="dest"/> parameter is NULL.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="srcPitch"/> or the <paramref name="destPitch"/> parameter is less than 0.</exception>
		/// <remarks>Use this method to copy a single scanline of an image and (optionally) set an opaque constant alpha value.</remarks>
		protected unsafe void CopyScanline(void* src, int srcPitch, void* dest, int destPitch, BufferFormat format, ImageBitFlags bitFlags)
		{
			if ((src == null) || (dest == null))
			{
				throw new ArgumentNullException("The source/destination pointer must not be NULL.");
			}

			if ((srcPitch <= 0) || (destPitch <= 0))
			{
				throw new ArgumentOutOfRangeException("The source/destination pitch must be greater than 0");
			}

			if (format == BufferFormat.Unknown)
			{
				throw new ArgumentException("The format is unknown", "format");
			}

			int size = (src == dest) ? destPitch : (srcPitch.Min(destPitch));

			if ((bitFlags & ImageBitFlags.OpaqueAlpha) == ImageBitFlags.OpaqueAlpha)
			{
				// Do a straight copy.
				switch (format)
				{
					case BufferFormat.R32G32B32A32:
					case BufferFormat.R32G32B32A32_Float:
					case BufferFormat.R32G32B32A32_UInt:
					case BufferFormat.R32G32B32A32_Int:
						{
							uint alpha = (format == BufferFormat.R32G32B32_Float) ? 0x3F800000
												: ((format == BufferFormat.R32G32B32_Int) ? 0x7FFFFFFF : 0xFFFFFFFF);

							var srcPtr = (uint*)src;
							var destPtr = (uint*)dest;

							for (int i = 0; i < size; i += 16)
							{
								// If not in-place copy, then copy from the source.
								if (src != dest)
								{
									*destPtr = *srcPtr;
									srcPtr += 4;
								}

								*destPtr += 3;
								*(destPtr++) = alpha;
							}
						}
						return;
					case BufferFormat.R16G16B16A16:
					case BufferFormat.R16G16B16A16_Float:
					case BufferFormat.R16G16B16A16_UIntNormal:
					case BufferFormat.R16G16B16A16_UInt:
					case BufferFormat.R16G16B16A16_IntNormal:
					case BufferFormat.R16G16B16A16_Int:
						{
							ushort alpha = 0xFFFF;

							if (format == BufferFormat.R16G16B16A16_Float)
							{
								alpha = 0x3C00;
							}
							else if ((format == BufferFormat.R16G16B16A16_IntNormal) || (format == BufferFormat.R16G16B16A16_Int))
							{
								alpha = 0x7FFF;
							}

							var srcPtr = (ushort*)src;
							var destPtr = (ushort*)dest;

							for (int i = 0; i < size; i += 8)
							{
								// If not in-place copy, then copy from the source.
								if (src != dest)
								{
									*destPtr = *srcPtr;
									srcPtr += 4;
								}
								*destPtr += 3;
								*(destPtr++) = alpha;
							}
						}
						return;
					case BufferFormat.R10G10B10A2:
					case BufferFormat.R10G10B10A2_UIntNormal:
					case BufferFormat.R10G10B10A2_UInt:
					case BufferFormat.R10G10B10_XR_BIAS_A2_UIntNormal:
						{
							var srcPtr = (uint*)src;
							var destPtr = (uint*)dest;

							for (int i = 0; i < size; i += 4)
							{
								// If not in-place copy, then copy from the source.
								if (src != dest)
								{
									*destPtr = (*srcPtr) & 0x3FFFFFFF;
									srcPtr++;
								}
								*destPtr |= 0xC0000000;
								destPtr++;
							}
						}
						return;
					case BufferFormat.R8G8B8A8:
					case BufferFormat.R8G8B8A8_UIntNormal:
					case BufferFormat.R8G8B8A8_UIntNormal_sRGB:
					case BufferFormat.R8G8B8A8_UInt:
					case BufferFormat.R8G8B8A8_IntNormal:
					case BufferFormat.R8G8B8A8_Int:
					case BufferFormat.B8G8R8A8_UIntNormal:
					case BufferFormat.B8G8R8A8:
					case BufferFormat.B8G8R8A8_UIntNormal_sRGB:
						{
							uint alpha = ((format == BufferFormat.R8G8B8A8_Int) || (format == BufferFormat.R8G8B8A8_IntNormal)) ? 0x7F000000 : 0xFF000000;

							var srcPtr = (uint*)src;
							var destPtr = (uint*)dest;

							for (int i = 0; i < size; i += 4)
							{
								// If not in-place copy, then copy from the source.
								if (src != dest)
								{
									*destPtr = (*srcPtr) & 0xFFFFFF;
									srcPtr++;
								}
								*destPtr |= alpha;
								destPtr++;
							}
						}
						return;
					case BufferFormat.B5G5R5A1_UIntNormal:
						{
							var srcPtr = (ushort*)src;
							var destPtr = (ushort*)dest;

							for (int i = 0; i < size; i += 2)
							{
								// If not in-place copy, then copy from the source.
								if (src != dest)
								{
									*destPtr = (ushort)((*srcPtr) & 0x7FFF);
									srcPtr++;
								}
								*destPtr |= 0x8000;
								destPtr++;
							}
						}
						return;
					case BufferFormat.A8_UIntNormal:
						DirectAccess.FillMemory(dest, 0xFF, size);
						return;
				}
			}

			// Copy if not doing an in-place update.
			if (dest != src)
			{
				DirectAccess.MemoryCopy(dest, src, size);
			}
		}

		/// <summary>
		/// Function to load an image from a stream.
		/// </summary>
		/// <param name="stream">Stream containing the data to load.</param>
		/// <returns>The image data that was in the stream.</returns>
		protected internal abstract GorgonImageData LoadFromStream(Stream stream);

		/// <summary>
		/// Function to persist image data to a stream.
		/// </summary>
		/// <param name="imageData"><see cref="GorgonLibrary.Graphics.GorgonImageData">Gorgon image data</see> to persist.</param>
		/// <param name="stream">Stream that will contain the data.</param>
		protected internal abstract void SaveToStream(GorgonImageData imageData, Stream stream);

        /// <summary>
        /// Function to determine if this codec can read the file or not.
        /// </summary>
        /// <param name="stream">Stream used to read the file information.</param>
        /// <returns>
        /// TRUE if the codec can read the file, FALSE if not.
        /// </returns>
        /// <exception cref="System.IO.IOException">Thrown when the <paramref name="stream"/> is write-only or if the stream cannot perform seek operations.</exception>
		/// <exception cref="System.IO.EndOfStreamException">Thrown when an attempt to read beyond the end of the stream is made.</exception>
		public abstract bool CanBeRead(System.IO.Stream stream);

		/// <summary>
        /// Function to read file meta data.
        /// </summary>
        /// <param name="stream">Stream used to read the metadata.</param>
        /// <returns>
        /// The image meta data as a <see cref="GorgonLibrary.Graphics.IImageSettings">IImageSettings</see> value.
        /// </returns>
        /// <exception cref="System.IO.IOException">Thrown when the <paramref name="stream"/> is write-only or if the stream cannot perform seek operations.
		/// <para>-or-</para>
		/// <para>Thrown if the file is corrupt or can't be read by the codec.</para>
		/// </exception>
		/// <exception cref="System.IO.EndOfStreamException">Thrown when an attempt to read beyond the end of the stream is made.</exception>
		public abstract IImageSettings GetMetaData(System.IO.Stream stream);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Gorgon " + Codec + " Image Codec";
        }
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonImageCodec" /> class.
		/// </summary>
		protected GorgonImageCodec()
		{
			CodecCommonExtensions = new string[] { };
		}
		#endregion

		#region INamedObject Members
		/// <summary>
		/// Property to return the name of this object.
		/// </summary>
		string INamedObject.Name
		{
			get 
			{
				return Codec;
			}
		}
		#endregion
	}
}