﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2017 Michael Winsor
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
// Created: July 22, 2017 10:31:48 AM
// 
#endregion

using System;
using System.IO;
using Gorgon.Core;
using Gorgon.Diagnostics;
using Gorgon.Graphics.Core.Properties;
using Gorgon.Graphics.Imaging;
using Gorgon.Graphics.Imaging.Codecs;
using Gorgon.Math;
using SharpDX.Mathematics.Interop;
using DXGI = SharpDX.DXGI;
using DX = SharpDX;
using D3D11 = SharpDX.Direct3D11;

namespace Gorgon.Graphics.Core
{
    /// <summary>
    /// Provides a read/write (unordered access) view for a <see cref="GorgonTexture2D"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type of view allows for unordered access to a <see cref="GorgonTexture2D"/>. The texture must have been created with the <see cref="TextureBinding.ReadWriteView"/> flag in its 
    /// <see cref="IGorgonTexture2DInfo.Binding"/> property.
    /// </para>
    /// <para>
    /// The unordered access allows a shader to read/write any part of a <see cref="GorgonGraphicsResource"/> by multiple threads without memory contention. This is done through the use of 
    /// <a target="_blank" href="https://msdn.microsoft.com/en-us/library/windows/desktop/ff476334(v=vs.85).aspx">atomic functions</a>.
    /// </para>
    /// <para>
    /// These types of views are most useful for <see cref="GorgonComputeShader"/> shaders, but can also be used by a <see cref="GorgonPixelShader"/> by passing a list of these views in to a 
    /// <see cref="GorgonDrawCallCommon">draw call</see>.
    /// </para>
    /// <para>
    /// <note type="warning">
    /// <para>
    /// Unordered access views do not support <see cref="GorgonTexture2D"/> textures with <see cref="GorgonMultisampleInfo">multisampling</see> enabled.
    /// </para>
    /// </note>
    /// </para>
    /// </remarks>
    /// <seealso cref="GorgonGraphicsResource"/>
    /// <seealso cref="GorgonTexture2D"/>
    /// <seealso cref="GorgonComputeShader"/>
    /// <seealso cref="GorgonPixelShader"/>
    /// <seealso cref="GorgonDrawCallCommon"/>
    /// <seealso cref="GorgonMultisampleInfo"/>
    public sealed class GorgonTexture2DReadWriteView
        : GorgonReadWriteView, IGorgonTexture2DInfo
    {
        #region Variables.
        // Rectangles used for clearing the view.
        private RawRectangle[] _clearRects;
        #endregion

        #region Properties.
        /// <summary>
        /// Property to return the format used to interpret this view.
        /// </summary>
        public BufferFormat Format
        {
            get;
        }

        /// <summary>
        /// Property to return information about the <see cref="Format"/> used by this view.
        /// </summary>
        public GorgonFormatInfo FormatInformation
        {
            get;
        }

        /// <summary>
        /// Property to return the index of the first mip map in the resource to view.
        /// </summary>
        public int MipSlice
        {
            get;
        }

        /// <summary>
        /// Property to return the first array index to use in the view.
        /// </summary>
        public int ArrayIndex
        {
            get;
        }

        /// <summary>
        /// Property to return the number of array indices to use in the view.
        /// </summary>
        public int ArrayCount
        {
            get;
        }

        /// <summary>
        /// Property to return whether the texture is a texture cube or not.
        /// </summary>
        public bool IsCubeMap => Texture?.IsCubeMap ?? false;

        /// <summary>
        /// Property to return the texture that is bound to this view.
        /// </summary>
        public GorgonTexture2D Texture { get; private set; }

        /// <summary>
        /// Property to return the bounding rectangle for the view.
        /// </summary>
        /// <remarks>
        /// This value is the full bounding rectangle of the first mip map level for the texture associated with the view.
        /// </remarks>
        public DX.Rectangle Bounds
        {
            get;
        }

        /// <summary>
        /// Property to return the width of the texture in pixels.
        /// </summary>
        /// <remarks>
        /// This value is the full width of the first mip map level for the texture associated with the view.
        /// </remarks>
        public int Width => Texture?.Width ?? 0;

        /// <summary>
        /// Property to return the height of the texture in pixels.
        /// </summary>
        /// <remarks>
        /// This value is the full height of the first mip map level for the texture associated with the view.
        /// </remarks>
        public int Height => Texture?.Height ?? 0;

        /// <summary>
        /// Property to return the name of the texture.
        /// </summary>
        string IGorgonNamedObject.Name => Texture?.Name ?? string.Empty;

        /// <summary>
        /// Property to return the number of mip-map levels for the texture.
        /// </summary>
        int IGorgonTexture2DInfo.MipLevels => Texture?.MipLevels ?? 0;

        /// <summary>
        /// Property to return the multisample quality and count for this texture.
        /// </summary>
        /// <remarks>
        /// This value is defaulted to <see cref="GorgonMultisampleInfo.NoMultiSampling"/>.
        /// </remarks>
        GorgonMultisampleInfo IGorgonTexture2DInfo.MultisampleInfo => GorgonMultisampleInfo.NoMultiSampling;

        /// <summary>
        /// Property to return the flags to determine how the texture will be bound with the pipeline when rendering.
        /// </summary>
        public TextureBinding Binding => Texture?.Binding ?? TextureBinding.None;
        #endregion

        #region Methods.
        /// <summary>
        /// Function to retrieve the view description for a 2D texture.
        /// </summary>
        /// <param name="texture">Texture to build a view description for.</param>
        /// <returns>The shader view description.</returns>
        private D3D11.UnorderedAccessViewDescription1 GetDesc2D(GorgonTexture2D texture)
        {
            return new D3D11.UnorderedAccessViewDescription1
                   {
                       Format = (DXGI.Format)Format,
                       Dimension = texture.ArrayCount > 1
                                       ? D3D11.UnorderedAccessViewDimension.Texture2DArray
                                       : D3D11.UnorderedAccessViewDimension.Texture2D,
                       Texture2DArray =
                       {
                           MipSlice =  MipSlice,
                           FirstArraySlice = ArrayCount,
                           ArraySize = ArrayIndex,
                           PlaneSlice = 0
                       }
                   };
        }

        /// <summary>
        /// Function to perform the creation of a specific kind of view.
        /// </summary>
        /// <returns>The view that was created.</returns>
        private protected override D3D11.ResourceView OnCreateNativeView()
        {
            D3D11.UnorderedAccessViewDescription1 desc = GetDesc2D(Texture);
            
            Graphics.Log.Print($"Creating D3D11 2D texture unordered access view for {Texture.Name}.", LoggingLevel.Verbose);

            try
            {
                // Create our SRV.
                Native = new D3D11.UnorderedAccessView1(Resource.Graphics.D3DDevice, Resource.D3DResource, desc)
                         {
                             DebugName = $"'{Texture.Name}'_D3D11UnorderedAccessView1_2D"
                         };

                Graphics.Log.Print($"Unordered Access 2D View '{Texture.Name}': {Texture.ResourceType} -> Mip slice: {MipSlice}, Array Index: {ArrayIndex}, Array Count: {ArrayCount}",
                                   LoggingLevel.Verbose);
            }
            catch (DX.SharpDXException sDXEx)
            {
                if ((uint)sDXEx.ResultCode.Code == 0x80070057)
                {
                    throw new GorgonException(GorgonResult.CannotCreate,
                                              string.Format(Resources.GORGFX_ERR_VIEW_CANNOT_CAST_FORMAT,
                                                            Texture.Format,
                                                            Format));
                }

                throw;
            }

            return Native;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Texture = null;
            base.Dispose();
        }

        /// <summary>
        /// Function to return the width of the texture at the current <see cref="MipSlice"/> in pixels.
        /// </summary>
        /// <param name="mipLevel">The mip level to evaluate.</param>
        /// <returns>The width of the mip map level assigned to <see cref="MipSlice"/> for the texture associated with the view.</returns>
        public int GetMipWidth(int mipLevel)
        {
            mipLevel = mipLevel.Min(Texture.MipLevels).Max(MipSlice);
            return Width >> mipLevel;
        }

        /// <summary>
        /// Function to return the height of the texture at the current <see cref="MipSlice"/> in pixels.
        /// </summary>
        /// <param name="mipLevel">The mip level to evaluate.</param>
        /// <returns>The height of the mip map level assigned to <see cref="MipSlice"/> for the texture associated with the view.</returns>
        public int GetMipHeight(int mipLevel)
        {
            mipLevel = mipLevel.Min(Texture.MipLevels).Max(MipSlice);

            return Height >> mipLevel;
        }

        /// <summary>
        /// Function to clear the contents of the texture for this view.
        /// </summary>
        /// <param name="color">Color to use when clearing the texture unordered access view.</param>
        /// <param name="rectangles">[Optional] Specifies which regions on the view to clear.</param>
        /// <remarks>
        /// <para>
        /// This will clear the texture unordered access view to the specified <paramref name="color"/>.  If a specific region should be cleared, one or more <paramref name="rectangles"/> should be passed 
        /// to the method.
        /// </para>
        /// <para>
        /// If the <paramref name="rectangles"/> parameter is <b>null</b>, or has a zero length, the entirety of the view is cleared.
        /// </para>
        /// <para>
        /// If this method is called with a 3D texture bound to the view, and with regions specified, then the regions are ignored.
        /// </para>
        /// </remarks>
        public void Clear(GorgonColor color, DX.Rectangle[] rectangles)
        {
            if ((rectangles == null) || (rectangles.Length == 0))
            {
                Clear(color.Red, color.Green, color.Blue, color.Alpha);
                return;
            }

            if ((_clearRects == null) || (_clearRects.Length < rectangles.Length))
            {
                _clearRects = new RawRectangle[rectangles.Length];
            }

            for (int i = 0; i < rectangles.Length; ++i)
            {
                _clearRects[i] = rectangles[i];
            }

            Resource.Graphics.D3DDeviceContext.ClearView(Native, color.ToRawColor4(), _clearRects, rectangles.Length);
        }

        /// <summary>
        /// Function to create a new texture that is bindable to the GPU as an unordered access resource.
        /// </summary>
        /// <param name="graphics">The graphics interface to use when creating the target.</param>
        /// <param name="info">The information about the texture.</param>
        /// <param name="initialData">[Optional] Initial data used to populate the texture.</param>
        /// <returns>A new <see cref="GorgonTexture2DReadWriteView"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="graphics"/>, or <paramref name="info"/> parameter is <b>null</b>.</exception>
        /// <remarks>
        /// <para>
        /// This is a convenience method that will create a <see cref="GorgonTexture2D"/> and a <see cref="GorgonTexture2DReadWriteView"/> as a single object that users can use to apply a texture as an unordered  
        /// access resource. This helps simplify creation of a texture by executing some prerequisite steps on behalf of the user.
        /// </para>
        /// <para>
        /// Since the <see cref="GorgonTexture2D"/> created by this method is linked to the <see cref="GorgonTexture2DReadWriteView"/> returned, disposal of either one will dispose of the other on your behalf. If 
        /// the user created a <see cref="GorgonTexture2DReadWriteView"/> from the <see cref="GorgonTexture2D.GetReadWriteView"/> method on the <see cref="GorgonTexture2D"/>, then it's assumed the user knows 
        /// what they are doing and will handle the disposal of the texture and view on their own.
        /// </para>
        /// <para>
        /// If an <paramref name="initialData"/> image is provided, and the width/height/depth is not the same as the values in the <paramref name="info"/> parameter, then the image data will be cropped to
        /// match the values in the <paramref name="info"/> parameter. Things like array count, and mip levels will still be taken from the <paramref name="initialData"/> image parameter.
        /// </para>
        /// </remarks>
        /// <seealso cref="GorgonTexture2D"/>
        public static GorgonTexture2DReadWriteView CreateTexture(GorgonGraphics graphics, IGorgonTexture2DInfo info, IGorgonImage initialData = null)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            var newInfo = new GorgonTexture2DInfo(info)
                          {
                              Usage = info.Usage == ResourceUsage.Staging ? ResourceUsage.Default : info.Usage,
                              Binding = (((info.Binding & TextureBinding.ReadWriteView) != TextureBinding.ReadWriteView)
                                             ? (info.Binding | TextureBinding.ReadWriteView)
                                             : info.Binding) & ~TextureBinding.DepthStencil 
                          };

            if (initialData != null)
            {
                if ((initialData.Info.Width > info.Width)
                    || (initialData.Info.Height > info.Height))
                {
                    initialData = initialData.Expand(info.Width, info.Height, 1);
                }

                if ((initialData.Info.Width < info.Width)
                    || (initialData.Info.Height < info.Height))
                {
                    initialData = initialData.Crop(new DX.Rectangle(0, 0, info.Width, info.Height), 1);
                }
            }

            GorgonTexture2D texture = initialData == null
                                          ? new GorgonTexture2D(graphics, newInfo)
                                          : initialData.ToTexture2D(graphics,
                                                                    new GorgonTextureLoadOptions
                                                                    {
                                                                        Usage = newInfo.Usage,
                                                                        Binding = newInfo.Binding,
                                                                        MultisampleInfo = newInfo.MultisampleInfo,
                                                                        Name = newInfo.Name
                                                                    });

            GorgonTexture2DReadWriteView result = texture.GetReadWriteView();
            result.OwnsResource = true;

            return result;
        }

        /// <summary>
        /// Function to load a texture from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="graphics">The graphics interface that will own the texture.</param>
        /// <param name="stream">The stream containing the texture image data.</param>
        /// <param name="codec">The codec that is used to decode the the data in the stream.</param>
        /// <param name="size">[Optional] The size of the image in the stream, in bytes.</param>
        /// <param name="options">[Optional] Options used to further define the texture.</param>
        /// <returns>A new <see cref="GorgonTexture2DReadWriteView"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="graphics"/>, <paramref name="stream"/>, or the <paramref name="codec"/> parameter is <b>null</b>.</exception>
        /// <exception cref="IOException">Thrown if the <paramref name="stream"/> is write only.</exception>
        /// <exception cref="EndOfStreamException">Thrown if reading the image would move beyond the end of the <paramref name="stream"/>.</exception>
        /// <remarks>
        /// <para>
        /// This will load an <see cref="IGorgonImage"/> from a <paramref name="stream"/> and put it into a <see cref="GorgonTexture2D"/> object and return a <see cref="GorgonTexture2DReadWriteView"/>.
        /// </para>
        /// <para>
        /// If the <paramref name="size"/> option is specified, then the method will read from the stream up to that number of bytes, so it is up to the user to provide an accurate size. If it is omitted 
        /// then the <c>stream length - stream position</c> is used as the total size.
        /// </para>
        /// <para>
        /// If specified, the <paramref name="options"/>parameter will define how Gorgon and shaders should handle the texture.  The <see cref="GorgonTextureLoadOptions"/> type contains the following:
        /// <list type="bullet">
        ///		<item>
        ///			<term>Binding</term>
        ///			<description>When defined, will indicate the <see cref="TextureBinding"/> that defines how the texture will be bound to the graphics pipeline. If it is omitted, then the binding will be 
        ///         <see cref="TextureBinding.ShaderResource"/>.</description>
        ///		</item>
        ///		<item>
        ///			<term>Usage</term>
        ///			<description>When defined, will indicate the preferred usage for the texture. If it is omitted, then the usage will be set to <see cref="ResourceUsage.Default"/>.</description>
        ///		</item>
        ///		<item>
        ///			<term>Multisample info</term>
        ///			<description>When defined (i.e. not <b>null</b>), defines the multisampling to apply to the texture. If omitted, then the default is <see cref="GorgonMultisampleInfo.NoMultiSampling"/>.</description>
        ///		</item>
        /// </list>
        /// </para>
        /// <para>
        /// Since the <see cref="GorgonTexture2D"/> created by this method is linked to the <see cref="GorgonTexture2DReadWriteView"/> returned, disposal of either one will dispose of the other on your behalf. If 
        /// the user created a <see cref="GorgonTexture2DReadWriteView"/> from the <see cref="GorgonTexture2D.GetShaderResourceView"/> method on the <see cref="GorgonTexture2D"/>, then it's assumed the user knows 
        /// what they are doing and will handle the disposal of the texture and view on their own.
        /// </para>
        /// </remarks>
        public static GorgonTexture2DReadWriteView FromStream(GorgonGraphics graphics, Stream stream, IGorgonImageCodec codec, long? size = null, GorgonTextureLoadOptions options = null)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (codec == null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            if (!stream.CanRead)
            {
                throw new IOException(Resources.GORGFX_ERR_STREAM_WRITE_ONLY);
            }

            if (size == null)
            {
                size = stream.Length - stream.Position;
            }

            if ((stream.Length - stream.Position) < size)
            {
                throw new EndOfStreamException();
            }

            using (IGorgonImage image = codec.LoadFromStream(stream, size))
            {
                GorgonTexture2D texture = image.ToTexture2D(graphics, options);
                GorgonTexture2DReadWriteView view =  texture.GetReadWriteView();
                view.OwnsResource = true;
                return view;
            }
        }

        /// <summary>
        /// Function to load a texture from a file.
        /// </summary>
        /// <param name="graphics">The graphics interface that will own the texture.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="codec">The codec that is used to decode the the data in the stream.</param>
        /// <param name="options">[Optional] Options used to further define the texture.</param>
        /// <returns>A new <see cref="GorgonTexture2DReadWriteView"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="graphics"/>, <paramref name="filePath"/>, or the <paramref name="codec"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="filePath"/> parameter is empty.</exception>
        /// <remarks>
        /// <para>
        /// This will load an <see cref="IGorgonImage"/> from a file on disk and put it into a <see cref="GorgonTexture2D"/> object and return a <see cref="GorgonTexture2DReadWriteView"/>.
        /// </para>
        /// <para>
        /// If specified, the <paramref name="options"/>parameter will define how Gorgon and shaders should handle the texture.  The <see cref="GorgonTextureLoadOptions"/> type contains the following:
        /// <list type="bullet">
        ///		<item>
        ///			<term>Binding</term>
        ///			<description>When defined, will indicate the <see cref="TextureBinding"/> that defines how the texture will be bound to the graphics pipeline. If it is omitted, then the binding will be 
        ///         <see cref="TextureBinding.ShaderResource"/>.</description>
        ///		</item>
        ///		<item>
        ///			<term>Usage</term>
        ///			<description>When defined, will indicate the preferred usage for the texture. If it is omitted, then the usage will be set to <see cref="ResourceUsage.Default"/>.</description>
        ///		</item>
        ///		<item>
        ///			<term>Multisample info</term>
        ///			<description>When defined (i.e. not <b>null</b>), defines the multisampling to apply to the texture. If omitted, then the default is <see cref="GorgonMultisampleInfo.NoMultiSampling"/>.</description>
        ///		</item>
        /// </list>
        /// </para>
        /// <para>
        /// Since the <see cref="GorgonTexture2D"/> created by this method is linked to the <see cref="GorgonTexture2DReadWriteView"/> returned, disposal of either one will dispose of the other on your behalf. If 
        /// the user created a <see cref="GorgonTexture2DReadWriteView"/> from the <see cref="GorgonTexture2D.GetShaderResourceView"/> method on the <see cref="GorgonTexture2D"/>, then it's assumed the user knows 
        /// what they are doing and will handle the disposal of the texture and view on their own.
        /// </para>
        /// </remarks>
        public static GorgonTexture2DReadWriteView FromFile(GorgonGraphics graphics, string filePath, IGorgonImageCodec codec, GorgonTextureLoadOptions options = null)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentEmptyException(nameof(filePath));
            }

            if (codec == null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            using (IGorgonImage image = codec.LoadFromFile(filePath))
            {
                GorgonTexture2D texture = image.ToTexture2D(graphics, options);
                GorgonTexture2DReadWriteView view = texture.GetReadWriteView();
                view.OwnsResource = true;
                return view;
            }
        }
        #endregion

        #region Constructor/Finalizer.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonTexture2DReadWriteView"/> class.
        /// </summary>
        /// <param name="texture">The texture to view.</param>
        /// <param name="format">The format for the view.</param>
        /// <param name="formatInfo">Information about the format.</param>
        /// <param name="firstMipLevel">The first mip level to view.</param>
        /// <param name="arrayIndex">The first array index to view.</param>
        /// <param name="arrayCount">The number of array indices to view.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="texture"/>, or the <paramref name="formatInfo"/> parameter is <b>null</b>.</exception>
        internal GorgonTexture2DReadWriteView(GorgonTexture2D texture,
                                  BufferFormat format,
                                  GorgonFormatInfo formatInfo,
                                  int firstMipLevel,
                                  int arrayIndex,
                                  int arrayCount)
            : base(texture)
        {
            FormatInformation = formatInfo ?? throw new ArgumentNullException(nameof(formatInfo));
            Format = format;
            Texture = texture;
            Bounds = new DX.Rectangle(0, 0, Width, Height);
            MipSlice = firstMipLevel;
            ArrayIndex = arrayIndex;
            ArrayCount = arrayCount;
        }
        #endregion
    }
}