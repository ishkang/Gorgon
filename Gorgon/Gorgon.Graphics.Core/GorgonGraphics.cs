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
// Created: Tuesday, July 19, 2011 8:55:06 AM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gorgon.Core;
using Gorgon.Diagnostics;
using Gorgon.Graphics.Core.Properties;
using Gorgon.Math;
using Gorgon.Native;
using SharpDX.Mathematics.Interop;
using DX = SharpDX;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DXGI =  SharpDX.DXGI;

namespace Gorgon.Graphics.Core
{
    /// <summary>
    /// The primary object for the Gorgon Graphics system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is used to initialize the functionality available for rendering hardware accelerated graphics for applications. It is also used in the initialization of other objects used to create graphics. 
    /// </para>
    /// <para>
    /// Typically, a graphics object is assigned to a single <see cref="IGorgonVideoAdapterInfo"/> to provide access to the functionality of that video adapter. If the system has more than once video adapter 
    /// installed then access to subsequent devices can be given by creating a new instance of this object with the appropriate <see cref="IGorgonVideoAdapterInfo"/>.
    /// </para>
    /// <para>
    /// <note type="tip">
    /// <para>
    /// To determine what devices are attached to the system, use a <see cref="EnumerateAdapters"/> method to retreive a list of applicable video adapters. This will contain a list of 
    /// <see cref="IGorgonVideoAdapterInfo"/> objects suitable for construction of the graphics object.
    /// </para>
    /// </note>
    /// </para>
    /// <para>
    /// When creating a graphics object, the user can choose which feature set they will support for a given <see cref="IGorgonVideoAdapterInfo"/> so that older devices may be used. The actual feature set 
    /// support is provided by the <see cref="IGorgonVideoAdapterInfo.SupportedFeatureLevel"/> on the <see cref="IGorgonVideoAdapterInfo"/> interface.
    /// </para>
    /// <para>
    /// This object is quite simple in its functionality. It provides some state assignment, and a means to submit a <see cref="GorgonDrawCallBase">draw call</see> so that graphics information can be 
    /// rendered.
    /// </para>
    /// <para><h3>Rendering</h3></para>
    /// <para>
    /// Through the use of <see cref="GorgonDrawCallBase">draw call types</see>, this object will send data in a stateless fashion. This differs from Direct 3D and other traditional APIs where states are 
    /// set until they're changed (stateful). The approach provided by this object avoids a common problem called state-leakage where a state may have been set prior to drawing, but was forgotten about. 
    /// This can lead to artifacts or can disable rendering entirely and consequently can be quite difficult to track. 
    /// </para>
    /// <para>
    /// When a draw call is sent, it carries all of the required state information (with the exception of a view resource types). This ensures that if a draw call doesn't need a state at a specific time, 
    /// it will be reset to a sensible default (as defined by the developer). 
    /// </para>
    /// <para>
    /// When drawing, Gorgon will determine the minimum required state to send with the final draw call, ensuring no redundant states are set. This type of rendering provides a performance gain since it will 
    /// only set the absolute minimum unique state it needs when the draw call is actually sent to the GPU. This means the user can set the state for a draw call as much as they want without that state being 
    /// sent to the GPU.
    /// </para>
    /// <para>
    /// <h3>Debugging Support</h3>
    /// </para>
    /// <para>
    /// Applications can enable Direct 3D debugging by setting to the <see cref="IsDebugEnabled"/> property to <b>true</b>. This will allow developers to examine underlying failures when rendering using 
    /// Direct 3D. Gorgon also provides memory tracking for any underlying Direct 3D objects when the <see cref="IsObjectTrackingEnabled"/> is set to <b>true</b>. This is useful if a 
    /// <see cref="IDisposable.Dispose"/> call was forgotten by the developer.
    /// </para>
    /// <para>
    /// However, it is not enough to just set these flags to <b>true</b> to enable debugging. Users must also use the DirectX control panel (<c>Debug -> Graphics -> DirectX Control Panel</c>) provided by 
    /// Visual Studio in order to turn on debugging. Finally, the user must then turn on Native debugging in the Project properties of their application (under the <b>Debug</b> tab) so that any debug 
    /// output can be seen in the Output window while running the application.
    /// </para>
    /// <para>
    /// If using a <b>DEBUG</b> compiled version of Gorgon (recommended for development), then the <see cref="IsDebugEnabled"/> property will automatically be set to <b>true</b>.
    /// </para>
    /// <para>
    /// <h3>Requirements</h3>
    /// </para>
    /// <para>
    /// This object enforces a minimum of <b>Windows 10, Build 16299 (aka Fall Creators Update)</b>. If this object is instanced on a lower version of operating system, an exception will be thrown. It also 
    /// requires a minimum Direct 3D version of 11.5 (which itself requires a Direct3D 12.0 compliant video card).
    /// </para>
    /// <para>
    /// <h3>Some rationale for Windows 10 is warranted here.</h3>
    /// </para>
    /// Since this requirement may prove somewhat unpopular, here's some reasoning behind it:
    /// <para>
    /// <list type="bullet">
    ///     <item>The author feels that Windows 10 adoption is significant enough to warrant its use as a baseline version.</item>
    ///     <item>Direct 3D 12 support is only available on Windows 10. While Gorgon only uses Direct 3D 11.4, moving to 12 outright may happen in the future and this will help facilitate that.</item>
    ///     <item>Windows 10 (specifically the Fall Creators Update, build 16299 in conjunction with .NET 4.7) provides new WinForms DPI-aware functionality. This one is kind of a big deal.</item>
    ///     <item>The author actually likes Windows 10. Your opinion may vary.</item>
    /// </list>
    /// So that's the reasoning for now. Users who don't wish to use Windows 10 are free to use other, potentially better, libraries/engines such as 
    /// <a target="_blank" href="http://www.monogame.net/">MonoGame</a> or <a target="_blank" href="https://unity3d.com/">Unity</a> which should work fine on older versions.
    /// </para>
    /// </remarks>
    /// <seealso cref="IGorgonVideoAdapterInfo"/>
    /// <seealso cref="GorgonDrawCall"/>
    /// <seealso cref="GorgonDrawIndexedCall"/>
    /// <seealso cref="GorgonDrawInstancedCall"/>
    /// <seealso cref="GorgonDrawIndexedInstancedCall"/>
    public sealed class GorgonGraphics
        : IDisposable
    {
        #region Constants.
        /// <summary>
        /// The name of the shader file data used for include files that wish to use the include shader.
        /// </summary>
        public const string BlitterShaderIncludeFileName = "__Gorgon_TextureBlitter_Shader__";
        #endregion

        #region Variables.
        // The log interface used to log debug messages.
        private readonly IGorgonLog _log;

        // The video adapter to use for this graphics object.
        private IGorgonVideoAdapterInfo _videoAdapter;

        // The last used draw call.
        private GorgonDrawCallBase _currentDrawCall;

        // Pipeline state cache.
        private readonly List<GorgonPipelineState> _stateCache = new List<GorgonPipelineState>();

        // A group of pipeline state caches that applications can use for caching their own pipeline states.
        private readonly Dictionary<string, IGorgonPipelineStateGroup> _groupedCache = new Dictionary<string, IGorgonPipelineStateGroup>(StringComparer.OrdinalIgnoreCase);

        // Synchronization lock for creating new pipeline cache entries.
        private readonly object _stateCacheLock = new object();

        // The list of cached scissor rectangles to keep allocates sane.
        private DX.Rectangle[] _cachedScissors = new DX.Rectangle[1];

        // The current depth/stencil view.
        private GorgonDepthStencilView _depthStencilView;

        // Flag to indicate that the depth/stencil view has been changed.
        private bool _depthStencilChanged;

        // The list of render targets currently assigned.
        private readonly GorgonRenderTargetViews _renderTargets;

        // The states used for texture samplers.  Used as a transitional buffer between D3D11 and Gorgon.
        private readonly D3D11.SamplerState[] _samplerStates = new D3D11.SamplerState[GorgonSamplerStates.MaximumSamplerStateCount];

        // The texture blitter used to render a single texture.
        private readonly Lazy<TextureBlitter> _textureBlitter;

        // An unordered access view buffer (other shader stages).
        private D3D11.UnorderedAccessView[] _uavBuffer = new D3D11.UnorderedAccessView[0];

        // A buffer for uav counters (other shader stages).
        private int[] _uavCounters = new int[0];

        // A buffer used to apply stream out buffers.
        private D3D11.StreamOutputBufferBinding[] _streamOutBuffer;
        
        // The list of support options for a given buffer format.
        private readonly Dictionary<BufferFormat, GorgonFormatSupportInfo> _formatSupport = new Dictionary<BufferFormat, GorgonFormatSupportInfo>();

        // The D3D 11.4 device context.
        private D3D11.DeviceContext4 _d3DDeviceContext;

        // The D3D 11.4 device.
        private D3D11.Device5 _d3DDevice;

        // The DXGI adapter.
        private DXGI.Adapter4 _dxgiAdapter;

        // The DXGI factory
        private DXGI.Factory5 _dxgiFactory;
        #endregion

        #region Properties.
        /// <summary>
        /// Property to return the Direct 3D 11.4 device context for this graphics instance.
        /// </summary>
        internal D3D11.DeviceContext4 D3DDeviceContext => _d3DDeviceContext;

        /// <summary>
        /// Property to return the Direct 3D 11.4 device for this graphics instance.
        /// </summary>
        internal D3D11.Device5 D3DDevice => _d3DDevice;

        /// <summary>
        /// Property to return the selected DXGI video adapter for this graphics instance.
        /// </summary>
        internal DXGI.Adapter4 DXGIAdapter => _dxgiAdapter;

        /// <summary>
        /// Property to return the DXGI factory used to create DXGI objects.
        /// </summary>
        internal DXGI.Factory5 DXGIFactory => _dxgiFactory;

        /// <summary>
        /// Property to return the support available to each format.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will return the options available to a <see cref="BufferFormat"/>.
        /// </para>
        /// <para>
        /// The format support and compute shader/uav support value returned will be a bit mask of values from the <see cref="BufferFormatSupport"/> and the <see cref="ComputeShaderFormatSupport"/> 
        /// enumeration respectively.
        /// </para>
        /// </remarks>
        public IReadOnlyDictionary<BufferFormat, GorgonFormatSupportInfo> FormatSupport => _formatSupport;

        /// <summary>
        /// Property to return the list of cached pipeline states.
        /// </summary>
        public IReadOnlyList<GorgonPipelineState> CachedPipelineStates
        {
            get
            {
                lock (_stateCacheLock)
                {
                    return _stateCache;
                }
            }
        }

        /// <summary>
        /// Property to set or return the video adapter to use for this graphics interface.
        /// </summary>
        public IGorgonVideoAdapterInfo VideoAdapter => _videoAdapter;

        /// <summary>
        /// Property to set or return whether object tracking is disabled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will enable SharpDX's object tracking to ensure references are destroyed upon application exit.
        /// </para>
        /// <para>
        /// The default value for DEBUG mode is <b>true</b>, and for RELEASE it is set to <b>false</b>.  Disabling object tracking will give a slight performance increase.
        /// </para>
        /// <para>
        /// <note type="important">
        /// <para>
        /// This flag <i>must</i> be set prior to creating any <see cref="GorgonGraphics"/> object, or else the flag will not take effect.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public static bool IsObjectTrackingEnabled
        {
            get => DX.Configuration.EnableObjectTracking;
            set => DX.Configuration.EnableObjectTracking = value;
        }

        /// <summary>
        /// Property to set or return whether debug output is enabled for the underlying graphics API.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will enable debug output for the underlying graphics API that Gorgon uses to render (Direct 3D 11 at this time). When this is enabled, all functionality will have debugging information that will 
        /// output to the debug output console (Output window in Visual Studio) if the <c>Debug -> Enable Native Debugging</c> is turned on in the application project settings <i>and</i> the DirectX control panel 
        /// is set up to debug the application under Direct 3D 10/11(/12 for Windows 10) application list.
        /// </para>
        /// <para>
        /// When Gorgon is compiled in DEBUG mode, this flag defaults to <b>true</b>, otherwise it defaults to <b>false</b>.
        /// </para>
        /// <para>
        /// <note type="important">
        /// <para>
        /// This flag <i>must</i> be set prior to creating any <see cref="GorgonGraphics"/> object, or else the flag will not take effect.
        /// </para>
        /// <para>
        /// The D3D11 SDK Layers DLL must be installed in order for this flag to work. If it is not, then the application may crash.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public static bool IsDebugEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Property to set or return the currently active depth/stencil view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is read only, but can be assigned via one of the <see cref="O:Gorgon.Graphics.Core.GorgonGraphics.SetRenderTarget"/> or 
        /// <see cref="SetRenderTargets(GorgonRenderTargetView[], GorgonDepthStencilView)"/> methods.
        /// </para>
        /// <para>
        /// If a render target is bound using <see cref="SetRenderTarget(GorgonRenderTargetView)"/> and it has an attached depth/stencil view, then that view will automatically be assigned to this value.
        /// </para>
        /// </remarks>
        /// <seealso cref="O:Gorgon.Graphics.Core.GorgonGraphics.SetRenderTarget"/>
        /// <seealso cref="SetRenderTargets"/>
        /// <seealso cref="GorgonDepthStencilView"/>
        public GorgonDepthStencilView DepthStencilView
        {
            get => _depthStencilView;
            private set
            {
                if (_depthStencilView == value)
                {
                    return;
                }

                _depthStencilView = value;
                _depthStencilChanged = true;
            }
        }

        /// <summary>
        /// Property to return the current list of render targets assigned to the renderer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When a render target is assigned to this list, the depth/stencil buffer will be replaced with the associated depth/stencil for the assigned <see cref="GorgonRenderTargetView"/>. If the 
        /// <see cref="GorgonRenderTargetView"/> does not have an associated depth/stencil buffer, then the current <see cref="DepthStencilView"/> buffer will be set to <b>null</b>. 
        /// If a user wishes to assign their own depth/stencil buffer, then they must assign the <see cref="DepthStencilView"/> after assigning a <see cref="GorgonRenderTargetView"/>.
        /// </para>
        /// <para>
        /// Unlike most states/resources, this property is stateful, that is, it will retain its state until changed by the user and will not be overwritten by a draw call.
        /// </para>
        /// <para>
        /// This list is read only. To assign a render target, use the <see cref="O:Gorgon.Graphics.Core.GorgonGraphics.SetRenderTarget"/> or the 
        /// <see cref="SetRenderTargets(GorgonRenderTargetView[], GorgonDepthStencilView)"/> methods.
        /// </para>
        /// </remarks>
        /// <seealso cref="SetRenderTargets(GorgonRenderTargetView[], GorgonDepthStencilView)"/>
        /// <seealso cref="O:Gorgon.Graphics.Core.GorgonGraphics.SetRenderTarget"/>
        /// <seealso cref="GorgonRenderTargetView"/>
        public IReadOnlyList<GorgonRenderTargetView> RenderTargets => _renderTargets;

        /// <summary>
        /// Property to return the scissor rectangles currently active for rendering.
        /// </summary>
        public GorgonMonitoredValueTypeArray<DX.Rectangle> ScissorRectangles
        {
            get;
        }

        /// <summary>
        /// Property to return the viewports used to render to the <see cref="RenderTargets"/>.
        /// </summary>
        public GorgonMonitoredValueTypeArray<DX.ViewportF> Viewports
        {
            get;
        }

        /// <summary>
        /// Property to return the actual supported <see cref="FeatureSet"/> from the device.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A user may request a lower <see cref="FeatureSet"/> than what is supported by the device to allow the application to run on older video adapters that lack support for newer functionality. 
        /// This requested feature set will be returned by this property if supported by the device. 
        /// </para>
        /// <para>
        /// If the user does not request a feature set, or has specified one higher than what the video adapter supports, then the highest feature set supported by the video adapter 
        /// (indicated by the <see cref="IGorgonVideoAdapterInfo.SupportedFeatureLevel"/> property in the <see cref="IGorgonVideoAdapterInfo"/> class) will be returned.
        /// </para>
        /// </remarks>
        /// <seealso cref="FeatureSet"/>
        public FeatureSet RequestedFeatureSet
        {
            get;
            private set;
        }
        #endregion

        #region Methods.
        /// <summary>
        /// Function to validate the parameters for the set/get data methods.
        /// </summary>
        /// <param name="usage">The usage for the resource.</param>
        /// <param name="srcSize">The total size of the data being uploaded.</param>
        /// <param name="bufferSize">The total size of the resource receiving the data.</param>
        /// <param name="sourceIndex">The index/offset to start reading from.</param>
        /// <param name="sourceCount">The number of items/bytes to read.</param>
        /// <param name="destOffset">The offset, in bytes, to start writing at.</param>
        /// <param name="destSize">The number of bytes to write.</param>
        private void ValidateSetData(ResourceUsage usage, int srcSize, int bufferSize, int sourceIndex, int sourceCount, int destOffset, int destSize)
        {
            if (sourceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            }

            if (destOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(destOffset));
            }

            if (sourceIndex + sourceCount > srcSize)
            {
                throw new ArgumentException(string.Format(Resources.GORGFX_ERR_DATA_OFFSET_COUNT_IS_TOO_LARGE, sourceIndex, sourceCount));
            }

            if (destOffset + destSize > bufferSize)
            {
                throw new ArgumentException(string.Format(Resources.GORGFX_ERR_DATA_OFFSET_COUNT_IS_TOO_LARGE, destOffset, destSize));
            }

            if (usage == ResourceUsage.Immutable)
            {
                throw new NotSupportedException(Resources.GORGFX_ERR_BUFFER_CANT_UPDATE_IMMUTABLE_OR_DYNAMIC);
            }
        }

        /// <summary>
        /// Function to uplaod data into a D3D 11 resource via a pointer and a map operation.
        /// </summary>
        /// <param name="source">The pointer to the data to upload.</param>
        /// <param name="dest">The D3D 11 resource that will receive the data.</param>
        /// <param name="subResource">The sub resource on the resource to write into.</param>
        /// <param name="sourceSize">The amount of data to copy from the pointer, in bytes.</param>
        /// <param name="destOffset">The offset, in bytes, within the resource to start writing at.</param>
        /// <param name="staging"><b>true</b> if the resource is a staging resource, or <b>false</b> if it is not.</param>
        /// <param name="copyMode">The flags used to determine how to upload the data.</param>
        private void MapBuffer(IntPtr source, D3D11.Resource dest, int subResource, int sourceSize, int destOffset, bool staging, CopyMode copyMode)
        {
            D3D11.MapMode mapMode = D3D11.MapMode.Write;

            if (!staging)
            {
                switch (copyMode)
                {
                    case CopyMode.None:
                    case CopyMode.Discard:
                        mapMode = D3D11.MapMode.WriteDiscard;
                        break;
                    case CopyMode.NoOverwrite:
                        mapMode = D3D11.MapMode.WriteNoOverwrite;
                        break;
                }
            }

            DX.DataBox box = D3DDeviceContext.MapSubresource(dest, subResource, mapMode, D3D11.MapFlags.None);
            DirectAccess.MemoryCopy(box.DataPointer + destOffset, source, sourceSize);
            D3DDeviceContext.UnmapSubresource(dest, subResource);
        }

        /// <summary>
        /// Function to retrieve the multi sample maximum quality level support for a given format.
        /// </summary>
        /// <param name="format">The DXGI format support to evaluate.</param>
        /// <returns>A <see cref="GorgonMultisampleInfo"/> value containing the max count and max quality level.</returns>
        private GorgonMultisampleInfo GetMultisampleSupport(DXGI.Format format)
        {
            try
            {
                for (int count = D3D11.Device.MultisampleCountMaximum; count >= 1; count = count / 2)
                {
                    int quality = D3DDevice.CheckMultisampleQualityLevels1(format, count, D3D11.CheckMultisampleQualityLevelsFlags.None);

                    if ((quality < 1) || (count == 1))
                    {
                        continue;
                    }

                    return new GorgonMultisampleInfo(count, quality - 1);
                }
            }
            catch (DX.SharpDXException sdEx)
            {
                _log.Print($"ERROR: Could not retrieve a multisample quality level max for format: [{format}]. Exception: {sdEx.Message}", LoggingLevel.Verbose);
            }

            return GorgonMultisampleInfo.NoMultiSampling;
        }

        /// <summary>
        /// Function to create the Direct 3D device and Adapter for use with Gorgon.
        /// </summary>
        /// <param name="adapterInfo">The adapter to use.</param>
        /// <param name="requestedFeatureLevel">The requested feature set for the device.</param>
        private void CreateDevice(IGorgonVideoAdapterInfo adapterInfo, D3D.FeatureLevel requestedFeatureLevel)
        {
            D3D11.DeviceCreationFlags flags = IsDebugEnabled ? D3D11.DeviceCreationFlags.Debug : D3D11.DeviceCreationFlags.None;

            using (DXGI.Factory2 factory2 = new DXGI.Factory2(IsDebugEnabled))
            {
                _dxgiFactory = factory2.QueryInterface<DXGI.Factory5>();

                using (DXGI.Adapter adapter = (adapterInfo.VideoDeviceType == VideoDeviceType.Hardware ? _dxgiFactory.GetAdapter1(adapterInfo.Index) : _dxgiFactory.GetWarpAdapter()))
                {
                    _dxgiAdapter = adapter.QueryInterface<DXGI.Adapter4>();

                    using (D3D11.Device device = new D3D11.Device(_dxgiAdapter, flags, requestedFeatureLevel)
                                                 {
                                                     DebugName = $"'{adapterInfo.Name}' D3D11.4 {(adapterInfo.VideoDeviceType == VideoDeviceType.Software ? "Software Adapter" : "Adapter")}"
                                                 })
                    {
                        _d3DDevice = device.QueryInterface<D3D11.Device5>();
                        RequestedFeatureSet = (FeatureSet)_d3DDevice.FeatureLevel;

                        _d3DDeviceContext = device.ImmediateContext.QueryInterface<D3D11.DeviceContext4>();

		                _log.Print($"Direct 3D 11.4 device created for video adapter '{adapterInfo.Name}' at feature set [{RequestedFeatureSet}]", LoggingLevel.Simple);
                    }
                }
            }
        }


        /// <summary>
        /// Function to initialize the selected video adapter for rendering using Direct 3D 11.4.
        /// </summary>
        /// <param name="adapterInfo">The information for the adapter to use.</param>
        /// <param name="featureSet">The feature set for the adapter.</param>
        private void InitializeVideoAdapter(IGorgonVideoAdapterInfo adapterInfo, FeatureSet featureSet)
        {
            CreateDevice(adapterInfo, (D3D.FeatureLevel)featureSet);

            IEnumerable<BufferFormat> formats = (BufferFormat[])Enum.GetValues(typeof(BufferFormat));

            // Get support values for each format.
            foreach (BufferFormat format in formats)
            {
                DXGI.Format dxgiFormat = (DXGI.Format)format;

                // NOTE: NV12 seems to come back as value of -92093664, no idea what the extra flags might be, the documentation for D3D doesn't
                //       specify the flags, and this value is present in SharpDX too.
                D3D11.FormatSupport formatSupport = D3DDevice.CheckFormatSupport(dxgiFormat);
                D3D11.ComputeShaderFormatSupport computeSupport = D3DDevice.CheckComputeShaderFormatSupport(dxgiFormat);

                _formatSupport[format] = new GorgonFormatSupportInfo(format, formatSupport, computeSupport, GetMultisampleSupport(dxgiFormat));
            }

            _videoAdapter = adapterInfo;
        }

        /// <summary>
        /// Function to merge the previous draw call vertex buffers with new ones.
        /// </summary>
        /// <param name="vertexBuffers">The vertex buffers to merge in.</param>
        /// <param name="currentChanges">The current changes on the pipeline.</param>
        /// <returns>A <see cref="PipelineResourceChange"/> indicating whether or not the state has changed.</returns>
        private PipelineResourceChange MergeVertexBuffers(GorgonVertexBufferBindings vertexBuffers, PipelineResourceChange currentChanges)
        {
            if (_currentDrawCall.VertexBuffers?.InputLayout != vertexBuffers?.InputLayout)
            {
                currentChanges |= PipelineResourceChange.InputLayout;
            }

            if (_currentDrawCall.VertexBuffers == vertexBuffers)
            {
                if ((_currentDrawCall.VertexBuffers == null)
                    || (!_currentDrawCall.VertexBuffers.IsDirty))
                {
                    return currentChanges;
                }

                return currentChanges | PipelineResourceChange.VertexBuffers;
            }

            // If we're tranferring into an uninitialized vertex buffer list, then allocate new vertex buffers and copy.
            if ((vertexBuffers != null)
                && (_currentDrawCall.VertexBuffers == null))
            {
                _currentDrawCall.VertexBuffers = new GorgonVertexBufferBindings(vertexBuffers.InputLayout);
                vertexBuffers.CopyTo(_currentDrawCall.VertexBuffers);
                return currentChanges | PipelineResourceChange.VertexBuffers;
            }

            // If we're removing a set of vertex buffers, then get rid of our current set as well.
            if (vertexBuffers == null)
            {
                _currentDrawCall.VertexBuffers = null;
                return currentChanges | PipelineResourceChange.VertexBuffers;
            }

            ref (int StartSlot, int Count) newItems = ref vertexBuffers.GetDirtyItems();

            // There's nothing going on in this list, so we can just leave now.
            if (newItems.Count == 0)
            {
                return currentChanges;
            }

            int endSlot = newItems.StartSlot + newItems.Count;

            if ((currentChanges & PipelineResourceChange.InputLayout) == PipelineResourceChange.InputLayout)
            {
                _currentDrawCall.VertexBuffers.InputLayout = vertexBuffers.InputLayout;
            }

            for (int i = newItems.StartSlot; i < endSlot; ++i)
            {
                _currentDrawCall.VertexBuffers[i] = vertexBuffers[i];
            }

            if (_currentDrawCall.VertexBuffers.IsDirty)
            {
                currentChanges |= PipelineResourceChange.VertexBuffers;
            }

            return currentChanges;
        }

        /// <summary>
        /// Function to merge the previous draw call stream output buffers with new ones.
        /// </summary>
        /// <param name="streamOutBuffers">The stream output buffers to merge in.</param>
        /// <param name="currentChanges">The current changes on the pipeline.</param>
        /// <returns>A <see cref="PipelineResourceChange"/> indicating whether or not the state has changed.</returns>
        private PipelineResourceChange MergeStreamOutBuffers(GorgonStreamOutBindings streamOutBuffers, PipelineResourceChange currentChanges)
        {
            if (_currentDrawCall.StreamOutBuffers == streamOutBuffers)
            {
                if ((_currentDrawCall.StreamOutBuffers == null)
                    || (!_currentDrawCall.StreamOutBuffers.IsDirty))
                {
                    return currentChanges;
                }

                return currentChanges | PipelineResourceChange.StreamOut;
            }

            // If we're tranferring into an uninitialized stream output buffer list, then allocate new stream output buffers and copy.
            if ((streamOutBuffers != null)
                && (_currentDrawCall.StreamOutBuffers == null))
            {
                _currentDrawCall.StreamOutBuffers = new GorgonStreamOutBindings();
                streamOutBuffers.CopyTo(_currentDrawCall.StreamOutBuffers);
                return currentChanges | PipelineResourceChange.StreamOut;
            }

            // If we're removing a set of stream output buffers, then get rid of our current set as well.
            if (streamOutBuffers == null)
            {
                _currentDrawCall.StreamOutBuffers = null;
                return currentChanges | PipelineResourceChange.StreamOut;
            }

            ref (int StartSlot, int Count) newItems = ref streamOutBuffers.GetDirtyItems();

            // There's nothing going on in this list, so we can just leave now.
            if (newItems.Count == 0)
            {
                return currentChanges;
            }

            int endSlot = newItems.StartSlot + newItems.Count;

            for (int i = newItems.StartSlot; i < endSlot; ++i)
            {
                _currentDrawCall.StreamOutBuffers[i] = streamOutBuffers[i];
            }

            if (_currentDrawCall.StreamOutBuffers.IsDirty)
            {
                currentChanges |= PipelineResourceChange.StreamOut;
            }

            return currentChanges;
        }

        /// <summary>
        /// Function to merge the previous shader constant buffers with new ones.
        /// </summary>
        /// <param name="shaderType">The type of shader to work with.</param>
        /// <param name="buffers">The constant buffers to merge in.</param>
        /// <param name="currentChanges">The current changes on the pipeline.</param>
        /// <returns>A <see cref="PipelineResourceChange"/> indicating whether or not the state has changed.</returns>
        private PipelineResourceChange MergeConstantBuffers(ShaderType shaderType, GorgonConstantBuffers buffers, PipelineResourceChange currentChanges)
        {
            ref (int StartSlot, int Count) newItems = ref buffers.GetDirtyItems();

            if (newItems.Count == 0)
            {
                return currentChanges;
            }

            PipelineResourceChange desiredStateBit;
            GorgonConstantBuffers destBuffers;

            switch (shaderType)
            {
                case ShaderType.Vertex:
                    desiredStateBit = PipelineResourceChange.VertexShaderConstantBuffers;
                    destBuffers = _currentDrawCall.VertexShaderConstantBuffers;
                    break;
                case ShaderType.Pixel:
                    desiredStateBit = PipelineResourceChange.PixelShaderConstantBuffers;
                    destBuffers = _currentDrawCall.PixelShaderConstantBuffers;
                    break;
                case ShaderType.Geometry:
                    desiredStateBit = PipelineResourceChange.GeometryShaderConstantBuffers;
                    destBuffers = _currentDrawCall.GeometryShaderConstantBuffers;
                    break;
                case ShaderType.Hull:
                    desiredStateBit = PipelineResourceChange.HullShaderConstantBuffers;
                    destBuffers = _currentDrawCall.HullShaderConstantBuffers;
                    break;
                case ShaderType.Domain:
                    desiredStateBit = PipelineResourceChange.DomainShaderConstantBuffers;
                    destBuffers = _currentDrawCall.DomainShaderConstantBuffers;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return CopyToLastDrawCall(newItems.StartSlot,
                                      newItems.StartSlot + newItems.Count,
                                      destBuffers,
                                      buffers,
                                      currentChanges,
                                      desiredStateBit);

            // Local functions are neat.
            PipelineResourceChange CopyToLastDrawCall(int newStart,
                                                      int newEnd,
                                                      GorgonConstantBuffers lastDrawConstants,
                                                      GorgonConstantBuffers newBuffers,
                                                      PipelineResourceChange changes,
                                                      PipelineResourceChange desiredBit)
            {
                for (int i = newStart; i < newEnd; ++i)
                {
                    lastDrawConstants[i] = newBuffers[i];
                }

                if (lastDrawConstants.IsDirty)
                {
                    changes |= desiredBit;
                }

                return changes;
            }
        }

        /// <summary>
        /// Function to merge the previous shader resource views with new ones.
        /// </summary>
        /// <param name="shaderType">The type of shader to work with.</param>
        /// <param name="srvs">The shader resource views to merge in.</param>
        /// <param name="currentChanges">The current changes on the pipeline.</param>
        /// <returns>A <see cref="PipelineResourceChange"/> indicating whether or not the state has changed.</returns>
        private PipelineResourceChange MergeShaderResources(ShaderType shaderType, GorgonShaderResourceViews srvs, PipelineResourceChange currentChanges)
        {
            ref (int StartSlot, int Count) newItems = ref srvs.GetDirtyItems();

            if (newItems.Count == 0)
            {
                return currentChanges;
            }

            PipelineResourceChange desiredStateBit;
            GorgonShaderResourceViews destSrvs;

            // Ensure that no input is currently bound as an output.
#if DEBUG
            ValidateSrvBinding(shaderType, srvs, newItems.StartSlot, newItems.StartSlot + newItems.Count);
#endif

            switch (shaderType)
            {
                case ShaderType.Vertex:
                    desiredStateBit = PipelineResourceChange.VertexShaderResources;
                    destSrvs = _currentDrawCall.VertexShaderResourceViews;
                    break;
                case ShaderType.Pixel:
                    desiredStateBit = PipelineResourceChange.PixelShaderResources;
                    destSrvs = _currentDrawCall.PixelShaderResourceViews;
                    break;
                case ShaderType.Geometry:
                    desiredStateBit = PipelineResourceChange.GeometryShaderResources;
                    destSrvs = _currentDrawCall.GeometryShaderResourceViews;
                    break;
                case ShaderType.Hull:
                    desiredStateBit = PipelineResourceChange.HullShaderResources;
                    destSrvs = _currentDrawCall.HullShaderResourceViews;
                    break;
                case ShaderType.Domain:
                    desiredStateBit = PipelineResourceChange.DomainShaderResources;
                    destSrvs = _currentDrawCall.DomainShaderResourceViews;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return CopyToLastDrawCall(newItems.StartSlot,
                                      newItems.Count + newItems.StartSlot,
                                      destSrvs,
                                      srvs,
                                      currentChanges,
                                      desiredStateBit);

            // Local functions are neat.
            PipelineResourceChange CopyToLastDrawCall(int newStart,
                                                      int newEnd,
                                                      GorgonShaderResourceViews lastDrawSrvs,
                                                      GorgonShaderResourceViews newSrvs,
                                                      PipelineResourceChange changes,
                                                      PipelineResourceChange desiredBit)
            {
                for (int i = newStart; i < newEnd; ++i)
                {
                    lastDrawSrvs[i] = newSrvs[i];
                }

                if (lastDrawSrvs.IsDirty)
                {
                    changes |= desiredBit;
                }

                return changes;
            }
        }

        /// <summary>
        /// Function to merge the unordered access views.
        /// </summary>
        /// <param name="uavs">The unordered access views to merge.</param>
        /// <param name="currentChanges">The current changes on the pipeline.</param>
        /// <returns>A <see cref="PipelineResourceChange"/> indicating whether or not the state has changed and a flag indicating that render targets were unbound.</returns>
        private PipelineResourceChange MergeUavs(GorgonUavBindings uavs, PipelineResourceChange currentChanges)
        {
            ref (int StartSlot, int Count) newItems = ref uavs.GetDirtyItems();

            if (newItems.Count == 0)
            {
                return currentChanges;
            }

            // Unbind any render targets at this point.
            int newEnd = newItems.Count + newItems.StartSlot;

#if DEBUG
            ref (int, int Count) rtvItems = ref _renderTargets.GetDirtyItems();

            for (int i = 0; (i < rtvItems.Count) && (newItems.Count > 0); ++i)
            {
                GorgonRenderTargetView rtv = _renderTargets[i];

                if ((rtv == null) || ((rtv.Texture.Info.Binding & TextureBinding.UnorderedAccess) != TextureBinding.UnorderedAccess))
                {
                    continue;
                }

                for (int j = newItems.StartSlot; j < newEnd; ++j)
                {
                    GorgonUavBinding uavBinding = uavs[i];

                    if ((uavBinding.Uav == null) || (uavBinding.Uav.Resource != rtv.Texture))
                    {
                        continue;
                    }

                    throw new GorgonException(GorgonResult.CannotBind, string.Format(Resources.GORGFX_ERR_CONFLICT_UAV_RTV, uavBinding.Uav.Resource.Name, i));
                }
            }
#endif

            UnbindUavInputs(ref newItems, _currentDrawCall?.UnorderedAccessViews, false);

            for (int i = newItems.StartSlot; i < newEnd; ++i)
            {
                _renderTargets[i] = null;
                _currentDrawCall.UnorderedAccessViews[i] = uavs[i];
            }

            if (_currentDrawCall.UnorderedAccessViews.IsDirty)
            {
                currentChanges |= PipelineResourceChange.Uavs;
            }

            return currentChanges;
        }

        /// <summary>
        /// Function to merge the previous shader samplers with new ones.
        /// </summary>
        /// <param name="shaderType">The type of shader to work with.</param>
        /// <param name="samplers">The shader samplers to merge in.</param>
        /// <param name="currentChanges">The current changes on the pipeline.</param>
        /// <returns>A <see cref="PipelineResourceChange"/> indicating whether or not the state has changed.</returns>
        private PipelineResourceChange MergeShaderSamplers(ShaderType shaderType, GorgonSamplerStates samplers, PipelineResourceChange currentChanges)
        {
            ref (int StartSlot, int Count) newItems = ref samplers.GetDirtyItems();

            if (newItems.Count == 0)
            {
                return currentChanges;
            }

            PipelineResourceChange desiredStateBit;
            GorgonSamplerStates destSamplers;

            switch (shaderType)
            {
                case ShaderType.Vertex:
                    desiredStateBit = PipelineResourceChange.VertexShaderSamplers;
                    destSamplers = _currentDrawCall.VertexShaderSamplers;
                    break;
                case ShaderType.Pixel:
                    desiredStateBit = PipelineResourceChange.PixelShaderSamplers;
                    destSamplers = _currentDrawCall.PixelShaderSamplers;
                    break;
                case ShaderType.Geometry:
                    desiredStateBit = PipelineResourceChange.GeometryShaderSamplers;
                    destSamplers = _currentDrawCall.GeometryShaderSamplers;
                    break;
                case ShaderType.Hull:
                    desiredStateBit = PipelineResourceChange.HullShaderSamplers;
                    destSamplers = _currentDrawCall.HullShaderSamplers;
                    break;
                case ShaderType.Domain:
                    desiredStateBit = PipelineResourceChange.DomainShaderSamplers;
                    destSamplers = _currentDrawCall.DomainShaderSamplers;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return CopyToLastDrawCall(newItems.StartSlot,
                                      newItems.Count + newItems.StartSlot,
                                      destSamplers,
                                      samplers,
                                      currentChanges,
                                      desiredStateBit);

            // Local functions are neat.
            PipelineResourceChange CopyToLastDrawCall(int newStart,
                                                      int newEnd,
                                                      GorgonSamplerStates lastDrawSamplers,
                                                      GorgonSamplerStates newSamplers,
                                                      PipelineResourceChange changes,
                                                      PipelineResourceChange desiredBit)
            {
                for (int i = newStart; i < newEnd; ++i)
                {
                    lastDrawSamplers[i] = newSamplers[i];
                }

                if (lastDrawSamplers.IsDirty)
                {
                    changes |= desiredBit;
                }

                return changes;
            }
        }

        /// <summary>
        /// Function to compare this pipeline state with another pipeline state.
        /// </summary>
        /// <param name="state">The state to compare.</param>
        /// <returns>The states that have been changed between this state and the other <paramref name="state"/>.</returns>
        private PipelineStateChange GetPipelineStateChange(GorgonPipelineState state)
        {
            PipelineStateChange pipelineFlags = PipelineStateChange.None;

            if ((_currentDrawCall.PipelineState == null) && (state == null))
            {
                return pipelineFlags;
            }

            if (((_currentDrawCall.PipelineState == null) && (state != null))
                || (state == null))
            {
                pipelineFlags |= PipelineStateChange.BlendState
                                 | PipelineStateChange.DepthStencilState
                                 | PipelineStateChange.RasterState
                                 | PipelineStateChange.PixelShader
                                 | PipelineStateChange.VertexShader
                                 | PipelineStateChange.GeometryShader
                                 | PipelineStateChange.HullShader
                                 | PipelineStateChange.DomainShader;

                _currentDrawCall.PipelineState = state;
                return pipelineFlags;
            }

            if (_currentDrawCall.PipelineState.Info.PixelShader != state.Info.PixelShader)
            {
                pipelineFlags |= PipelineStateChange.PixelShader;
            }

            if (_currentDrawCall.PipelineState.Info.VertexShader != state.Info.VertexShader)
            {
                pipelineFlags |= PipelineStateChange.VertexShader;
            }


            if (_currentDrawCall.PipelineState.Info.GeometryShader != state.Info.GeometryShader)
            {
                pipelineFlags |= PipelineStateChange.GeometryShader;
            }

            if (_currentDrawCall.PipelineState.Info.HullShader != state.Info.HullShader)
            {
                pipelineFlags |= PipelineStateChange.HullShader;
            }

            if (_currentDrawCall.PipelineState.Info.DomainShader != state.Info.DomainShader)
            {
                pipelineFlags |= PipelineStateChange.DomainShader;
            }

            if (_currentDrawCall.PipelineState.D3DRasterState != state.D3DRasterState)
            {
                pipelineFlags |= PipelineStateChange.RasterState;
            }

            if (_currentDrawCall.PipelineState.D3DDepthStencilState != state.D3DDepthStencilState)
            {
                pipelineFlags |= PipelineStateChange.DepthStencilState;
            }

            if (_currentDrawCall.PipelineState.D3DBlendState != state.D3DBlendState)
            {
                pipelineFlags |= PipelineStateChange.BlendState;
            }

            if (pipelineFlags != PipelineStateChange.None)
            {
                _currentDrawCall.PipelineState = state;
            }

            return pipelineFlags;
        }

        /// <summary>
        /// Function to merge the current and previous draw call in order to reduce state change.
        /// </summary>
        /// <param name="sourceDrawCall">The draw call that is currently being executed.</param>
        /// <returns>The changes to the resources and the state for the pipeline.</returns>
        private (PipelineResourceChange, PipelineStateChange) MergeDrawCall(GorgonDrawCallBase sourceDrawCall)
        {
            if (_currentDrawCall == null)
            {
                _currentDrawCall = new GorgonDrawCallBase
                                   {
                                       PrimitiveType = PrimitiveType.None
                                   };
            }

            PipelineResourceChange stateChanges = PipelineResourceChange.None;

            if (_currentDrawCall.PrimitiveType != sourceDrawCall.PrimitiveType)
            {
                _currentDrawCall.PrimitiveType = sourceDrawCall.PrimitiveType;
                stateChanges |= PipelineResourceChange.PrimitiveTopology;
            }

            if (_currentDrawCall.IndexBuffer != sourceDrawCall.IndexBuffer)
            {
                _currentDrawCall.IndexBuffer = sourceDrawCall.IndexBuffer;
                stateChanges |= PipelineResourceChange.IndexBuffer;
            }

            if (_currentDrawCall.BlendSampleMask != sourceDrawCall.BlendSampleMask)
            {
                _currentDrawCall.BlendSampleMask = sourceDrawCall.BlendSampleMask;
                stateChanges |= PipelineResourceChange.BlendSampleMask;
            }

            if (_currentDrawCall.DepthStencilReference != sourceDrawCall.DepthStencilReference)
            {
                _currentDrawCall.DepthStencilReference = sourceDrawCall.DepthStencilReference;
                stateChanges |= PipelineResourceChange.DepthStencilReference;
            }

            if (!_currentDrawCall.BlendFactor.Equals(sourceDrawCall.BlendFactor))
            {
                _currentDrawCall.BlendFactor = sourceDrawCall.BlendFactor;
                stateChanges |= PipelineResourceChange.BlendFactor;
            }

            stateChanges |= MergeUavs(sourceDrawCall.UnorderedAccessViews, stateChanges);
            stateChanges |= MergeVertexBuffers(sourceDrawCall.VertexBuffers, stateChanges);
            stateChanges |= MergeStreamOutBuffers(sourceDrawCall.StreamOutBuffers, stateChanges);

            stateChanges |= MergeConstantBuffers(ShaderType.Pixel, sourceDrawCall.PixelShaderConstantBuffers, stateChanges);
            stateChanges |= MergeShaderResources(ShaderType.Pixel, sourceDrawCall.PixelShaderResourceViews, stateChanges);
            stateChanges |= MergeShaderSamplers(ShaderType.Pixel, sourceDrawCall.PixelShaderSamplers, stateChanges);

            stateChanges |= MergeConstantBuffers(ShaderType.Vertex, sourceDrawCall.VertexShaderConstantBuffers, stateChanges);
            stateChanges |= MergeShaderSamplers(ShaderType.Vertex, sourceDrawCall.VertexShaderSamplers, stateChanges);
            stateChanges |= MergeShaderResources(ShaderType.Vertex, sourceDrawCall.VertexShaderResourceViews, stateChanges);

            stateChanges |= MergeConstantBuffers(ShaderType.Geometry, sourceDrawCall.GeometryShaderConstantBuffers, stateChanges);
            stateChanges |= MergeShaderResources(ShaderType.Geometry, sourceDrawCall.GeometryShaderResourceViews, stateChanges);
            stateChanges |= MergeShaderSamplers(ShaderType.Geometry, sourceDrawCall.GeometryShaderSamplers, stateChanges);

            stateChanges |= MergeConstantBuffers(ShaderType.Hull, sourceDrawCall.HullShaderConstantBuffers, stateChanges);
            stateChanges |= MergeShaderResources(ShaderType.Hull, sourceDrawCall.HullShaderResourceViews, stateChanges);
            stateChanges |= MergeShaderSamplers(ShaderType.Hull, sourceDrawCall.HullShaderSamplers, stateChanges);

            stateChanges |= MergeConstantBuffers(ShaderType.Domain, sourceDrawCall.DomainShaderConstantBuffers, stateChanges);
            stateChanges |= MergeShaderResources(ShaderType.Domain, sourceDrawCall.DomainShaderResourceViews, stateChanges);
            stateChanges |= MergeShaderSamplers(ShaderType.Domain, sourceDrawCall.DomainShaderSamplers, stateChanges);

            return (stateChanges, GetPipelineStateChange(sourceDrawCall.PipelineState));
        }

        /// <summary>
        /// Function to initialize a <see cref="GorgonPipelineState" /> object with Direct 3D 11 state objects by creating new objects for the unassigned values.
        /// </summary>
        /// <param name="info">The information used to create the pipeline state.</param>
        /// <param name="blendState">An existing blend state to use.</param>
        /// <param name="depthStencilState">An existing depth/stencil state to use.</param>
        /// <param name="rasterState">An existing rasterizer state to use.</param>
        /// <returns>A new <see cref="GorgonPipelineState"/>.</returns>
        private GorgonPipelineState InitializePipelineState(IGorgonPipelineStateInfo info,
                                                            D3D11.BlendState1 blendState,
                                                            D3D11.DepthStencilState depthStencilState,
                                                            D3D11.RasterizerState1 rasterState)
        {
            GorgonPipelineState result = new GorgonPipelineState(info, _stateCache.Count)
                         {
                             D3DBlendState = blendState,
                             D3DDepthStencilState = depthStencilState,
                             D3DRasterState = rasterState
                         };

            if ((rasterState == null) && (info.RasterState != null))
            {
                result.D3DRasterState = new D3D11.RasterizerState1(_d3DDevice, info.RasterState.ToRasterStateDesc1())
                                        {
                                            DebugName = "Gorgon D3D11RasterState"
                                        };
            }

            if ((depthStencilState == null) && (info.DepthStencilState != null))
            {
                result.D3DDepthStencilState = new D3D11.DepthStencilState(_d3DDevice, info.DepthStencilState.ToDepthStencilStateDesc())
                                              {
                                                  DebugName = "Gorgon D3D11DepthStencilState"
                                              };
            }

            if ((blendState != null) || (info.BlendStates == null) || (info.BlendStates.Count == 0))
            {
                return result;
            }

            int maxStates = info.BlendStates.Count.Min(D3D11.OutputMergerStage.SimultaneousRenderTargetCount);

            D3D11.BlendStateDescription1 desc = new D3D11.BlendStateDescription1
                       {
                           IndependentBlendEnable = info.IsIndependentBlendingEnabled,
                           AlphaToCoverageEnable = info.IsAlphaToCoverageEnabled
                       };

            for (int i = 0; i < maxStates; ++i)
            {
                desc.RenderTarget[i] = info.BlendStates[i].ToRenderTargetBlendStateDesc1();
            }

            result.D3DBlendState = new D3D11.BlendState1(_d3DDevice, desc)
                                   {
                                       DebugName = "Gorgon D3D11BlendState"
                                   };

            return result;
        }

        /// <summary>
        /// Function to build up a <see cref="GorgonPipelineState"/> object with Direct 3D 11 state objects by either creating new objects, or inheriting previous ones.
        /// </summary>
        /// <param name="newState">The new state to initialize.</param>
        /// <returns>An existing pipeline state if no changes are found, or a new pipeline state otherwise.</returns>
        private (GorgonPipelineState state, bool isNew) SetupPipelineState(IGorgonPipelineStateInfo newState)
        {
            // Existing states.
            D3D11.DepthStencilState depthStencilState = null;
            D3D11.BlendState1 blendState = null;
            D3D11.RasterizerState1 rasterState = null;
            (GorgonPipelineState State, bool IsNew) result;

            IGorgonPipelineStateInfo newStateInfo = newState;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _stateCache.Count; ++i)
            {
                int blendStateEqualCount = 0;
                PipelineStateChange inheritedState = PipelineStateChange.None;
                GorgonPipelineState cachedState = _stateCache[i];
                IGorgonPipelineStateInfo cachedStateInfo = _stateCache[i].Info;

                if (cachedStateInfo.PixelShader == newStateInfo.PixelShader)
                {
                    inheritedState |= PipelineStateChange.PixelShader;
                }

                if (cachedStateInfo.VertexShader == newStateInfo.VertexShader)
                {
                    inheritedState |= PipelineStateChange.VertexShader;
                }

                if (cachedStateInfo.GeometryShader == newStateInfo.GeometryShader)
                {
                    inheritedState |= PipelineStateChange.GeometryShader;
                }

                if (cachedStateInfo.HullShader == newStateInfo.HullShader)
                {
                    inheritedState |= PipelineStateChange.HullShader;
                }

                if (cachedStateInfo.DomainShader == newStateInfo.DomainShader)
                {
                    inheritedState |= PipelineStateChange.DomainShader;
                }

                if ((cachedStateInfo.RasterState != null) &&
                    (cachedStateInfo.RasterState.Equals(newStateInfo.RasterState)))
                {
                    rasterState = cachedState.D3DRasterState;
                    inheritedState |= PipelineStateChange.RasterState;
                }

                if ((cachedStateInfo.DepthStencilState != null) &&
                    (cachedStateInfo.DepthStencilState.Equals(newStateInfo.DepthStencilState)))
                {
                    depthStencilState = cachedState.D3DDepthStencilState;
                    inheritedState |= PipelineStateChange.DepthStencilState;
                }

                if (ReferenceEquals(newStateInfo.BlendStates, cachedStateInfo.BlendStates))
                {
                    blendState = cachedState.D3DBlendState;
                    inheritedState |= PipelineStateChange.BlendState;
                }
                else
                {
                    if ((newStateInfo.BlendStates != null)
                        && (cachedStateInfo.BlendStates != null)
                        && (newStateInfo.BlendStates.Count == cachedStateInfo.BlendStates.Count))
                    {
                        for (int j = 0; j < newStateInfo.BlendStates.Count; ++j)
                        {
                            if (cachedStateInfo.BlendStates[j]?.Equals(newStateInfo.BlendStates[j]) ?? false)
                            {
                                blendStateEqualCount++;
                            }
                        }

                        if (blendStateEqualCount == newStateInfo.BlendStates.Count)
                        {
                            blendState = cachedState.D3DBlendState;
                            inheritedState |= PipelineStateChange.BlendState;
                        }
                    }
                }

                // We've copied all the states, so just return the existing pipeline state.
                // ReSharper disable once InvertIf
                if (inheritedState == (PipelineStateChange.VertexShader
                                       | PipelineStateChange.PixelShader
                                       | PipelineStateChange.GeometryShader
                                       | PipelineStateChange.HullShader
                                       | PipelineStateChange.DomainShader
                                       | PipelineStateChange.BlendState
                                       | PipelineStateChange.RasterState
                                       | PipelineStateChange.DepthStencilState))
                {
                    result.State = _stateCache[i];
                    result.IsNew = false;
                    return result;
                }
            }

            // Setup any uninitialized states.
            result.State = InitializePipelineState(newState, blendState, depthStencilState, rasterState);
            result.IsNew = true;
            return result;
        }

        /// <summary>
        /// Function to bind an index buffer to the pipeline.
        /// </summary>
        /// <param name="indexBuffer">The index buffer to bind.</param>
        private void SetIndexbuffer(GorgonIndexBuffer indexBuffer)
        {
            D3D11.Buffer buffer = null;
            DXGI.Format format = DXGI.Format.Unknown;

            if (indexBuffer != null)
            {
                buffer = indexBuffer.NativeBuffer;
                format = (DXGI.Format)indexBuffer.IndexFormat;
            }

            D3DDeviceContext.InputAssembler.SetIndexBuffer(buffer, format, 0);
        }

        /// <summary>
        /// Function to apply a pipeline state to the pipeline.
        /// </summary>
        /// <param name="state">A <see cref="GorgonPipelineState"/> to apply to the pipeline.</param>
        /// <param name="changes">The changes to the pipeline state to apply.</param>
        /// <remarks>
        /// <para>
        /// This is responsible for setting all the states for a pipeline at once. This has the advantage of ensuring that duplicate states do not get set so that performance is not impacted. 
        /// </para>
        /// </remarks>
        private void ApplyPipelineState(GorgonPipelineState state, PipelineStateChange changes)
        {
            if (changes == PipelineStateChange.None)
            {
                return;
            }

            if ((changes & PipelineStateChange.RasterState) == PipelineStateChange.RasterState)
            {
                D3DDeviceContext.Rasterizer.State = state?.D3DRasterState;
            }

            if ((changes & PipelineStateChange.DepthStencilState) == PipelineStateChange.DepthStencilState)
            {
                D3DDeviceContext.OutputMerger.DepthStencilState = state?.D3DDepthStencilState;
            }

            if ((changes & PipelineStateChange.BlendState) == PipelineStateChange.BlendState)
            {
                D3DDeviceContext.OutputMerger.BlendState = state?.D3DBlendState;
            }

            if ((changes & PipelineStateChange.VertexShader) == PipelineStateChange.VertexShader)
            {
                D3DDeviceContext.VertexShader.Set(state?.Info.VertexShader?.NativeShader);
            }

            if ((changes & PipelineStateChange.PixelShader) == PipelineStateChange.PixelShader)
            {
                D3DDeviceContext.PixelShader.Set(state?.Info.PixelShader?.NativeShader);
            }

            if ((changes & PipelineStateChange.GeometryShader) == PipelineStateChange.GeometryShader)
            {
                D3DDeviceContext.GeometryShader.Set(state?.Info.GeometryShader?.NativeShader);
            }

            if ((changes & PipelineStateChange.HullShader) == PipelineStateChange.HullShader)
            {
                D3DDeviceContext.HullShader.Set(state?.Info.HullShader?.NativeShader);
            }

            if ((changes & PipelineStateChange.DomainShader) == PipelineStateChange.DomainShader)
            {
                D3DDeviceContext.DomainShader.Set(state?.Info.DomainShader?.NativeShader);
            }
        }

        /// <summary>
        /// Function to assign viewports.
        /// </summary>
        private unsafe void SetViewports()
        {
            if (!Viewports.IsDirty)
            {
                return;
            }

            ref (int Start, int Count) viewports = ref Viewports.GetDirtyItems();
            RawViewportF* rawViewports = stackalloc RawViewportF[viewports.Count];

            for (int i = 0; i < viewports.Count; ++i)
            {
                rawViewports[i] = Viewports[i];
            }

            D3DDeviceContext.Rasterizer.SetViewports(rawViewports, viewports.Count);
        }

        /// <summary>
        /// Function to assign scissor rectangles.
        /// </summary>
        private void SetScissorRects()
        {
            // If there's been no change to the scissor rectangles, then we do nothing as the state should be the same as last time.
            if (!ScissorRectangles.IsDirty)
            {
                return;
            }

            ref (int Start, int Count) scissors = ref ScissorRectangles.GetDirtyItems();

            if (scissors.Count != _cachedScissors.Length)
            {
                if ((scissors.Count == 0) && (_cachedScissors.Length != 1))
                {
                    _cachedScissors = new[]
                                      {
                                          DX.Rectangle.Empty
                                      };
                }
                else if (scissors.Count != 0)
                {
                    _cachedScissors = new DX.Rectangle[scissors.Count];
                }
            }

            for (int i = 0; i < _cachedScissors.Length; ++i)
            {
                _cachedScissors[i] = ScissorRectangles[i];
            }

            D3DDeviceContext.Rasterizer.SetScissorRectangles(_cachedScissors);
        }

        /// <summary>
        /// Function to assign the vertex buffers.
        /// </summary>
        /// <param name="vertexBuffers">The vertex buffers to assign.</param>
        private void SetVertexBuffers(GorgonVertexBufferBindings vertexBuffers)
        {
            if (vertexBuffers == null)
            {
                D3DDeviceContext.InputAssembler.SetVertexBuffers(0);
                return;
            }

            ref (int StartSlot, int Count) bindings = ref vertexBuffers.GetDirtyItems();
            D3DDeviceContext.InputAssembler.SetVertexBuffers(bindings.StartSlot, vertexBuffers.Native);
        }

        /// <summary>
        /// Function to assign the stream output buffers.
        /// </summary>
        /// <param name="streamOutBuffers">The stream output buffers to assign.</param>
        private void SetStreamOutBuffers(GorgonStreamOutBindings streamOutBuffers)
        {
            if (streamOutBuffers == null)
            {
                if (_streamOutBuffer != null)
                {
                    Array.Clear(_streamOutBuffer, 0, _streamOutBuffer.Length);
                }
                D3DDeviceContext.StreamOutput.SetTargets(null);
                return;
            }

            (int, int Count) bindings = streamOutBuffers.GetDirtyItems();

            if ((_streamOutBuffer == null) || (_streamOutBuffer.Length != bindings.Count))
            {
                _streamOutBuffer = new D3D11.StreamOutputBufferBinding[bindings.Count];
            }

            for (int i = 0; i < bindings.Count; ++i)
            {
                _streamOutBuffer[i] = streamOutBuffers.Native[i];
            }

            D3DDeviceContext.StreamOutput.SetTargets(_streamOutBuffer);
        }

        /// <summary>
        /// Function to unbind a render target that is bound as a shader input.
        /// </summary>
        /// <param name="rtBindings">The render target view bindings.</param>
        private void UnbindRtvInputs(ref (int Start, int Count) rtBindings)
        {
            // This can happen quite easily due to how we're handling draw calls (i.e. stateless).  So we won't log anything here and we'll just unbind for the time being.
            // This may have a small performance penalty.

            if (_currentDrawCall == null)
            {
                return;
            }

            ref (int, int) psSrvBindings = ref _currentDrawCall.PixelShaderResourceViews.GetDirtyItems();
            ref (int, int) gsSrvBindings = ref _currentDrawCall.GeometryShaderResourceViews.GetDirtyItems();
            ref (int, int) vsSrvBindings = ref _currentDrawCall.VertexShaderResourceViews.GetDirtyItems();
            ref (int, int) hsSrvBindings = ref _currentDrawCall.HullShaderResourceViews.GetDirtyItems();
            ref (int, int) dsSrvBindings = ref _currentDrawCall.DomainShaderResourceViews.GetDirtyItems();

            // Unbind any depth/stencil bound as input.
            if ((_depthStencilView != null)
                && ((_depthStencilView.Texture.Info.Binding & TextureBinding.ShaderResource) == TextureBinding.ShaderResource)
                && (_depthStencilView.Flags == DepthStencilViewFlags.None))
            {
                UnbindFromShader(ShaderType.Pixel, _depthStencilView.Texture, ref psSrvBindings, _currentDrawCall.PixelShaderResourceViews);
                UnbindFromShader(ShaderType.Geometry, _depthStencilView.Texture, ref gsSrvBindings, _currentDrawCall.GeometryShaderResourceViews);
                UnbindFromShader(ShaderType.Vertex, _depthStencilView.Texture, ref vsSrvBindings, _currentDrawCall.VertexShaderResourceViews);
                UnbindFromShader(ShaderType.Hull, _depthStencilView.Texture, ref hsSrvBindings, _currentDrawCall.HullShaderResourceViews);
                UnbindFromShader(ShaderType.Domain, _depthStencilView.Texture, ref dsSrvBindings, _currentDrawCall.DomainShaderResourceViews);
            }

            for (int i = 0; i < rtBindings.Start + rtBindings.Count; ++i)
            {
                GorgonRenderTargetView view = _renderTargets[i];

                if ((view == null) || ((view.Texture.Info.Binding & TextureBinding.ShaderResource) != TextureBinding.ShaderResource))
                {
                    continue;
                }

                UnbindFromShader(ShaderType.Pixel, view.Texture, ref psSrvBindings, _currentDrawCall.PixelShaderResourceViews);
                UnbindFromShader(ShaderType.Geometry, view.Texture, ref gsSrvBindings, _currentDrawCall.GeometryShaderResourceViews);
                UnbindFromShader(ShaderType.Vertex, view.Texture, ref vsSrvBindings, _currentDrawCall.VertexShaderResourceViews);
                UnbindFromShader(ShaderType.Hull, view.Texture, ref hsSrvBindings, _currentDrawCall.HullShaderResourceViews);
                UnbindFromShader(ShaderType.Domain, view.Texture, ref dsSrvBindings, _currentDrawCall.DomainShaderResourceViews);
            }

            void UnbindFromShader(ShaderType shaderType, GorgonTexture renderTarget, ref (int Start, int Count) bindings, GorgonShaderResourceViews srvs)
            {
                if (bindings.Count == 0)
                {
                    return;
                }

                bool unbound = false;
                for (int i = bindings.Start; i < bindings.Start + bindings.Count; ++i)
                {
                    GorgonShaderResourceView srv = srvs[i];

                    if ((srv == null) || (renderTarget != srv.Resource))
                    {
                        continue;
                    }

                    srvs[i] = null;
                    unbound = true;
                }

                if (unbound)
                {
                    SetShaderResourceViews(shaderType, srvs);
                }
            }
        }

        /// <summary>
        /// Function to assign the render targets.
        /// </summary>
        private void SetRenderTargetAndDepthViews()
        {
            if ((!_renderTargets.IsDirty) && (!_depthStencilChanged))
            {
                return;
            }

            ref (int StartSlot, int Count) bindings = ref _renderTargets.GetDirtyItems();

#if DEBUG
            if (_currentDrawCall != null)
            {
                ref (int, int Count) uavBindings = ref _currentDrawCall.UnorderedAccessViews.GetDirtyItems();

                // If we attempt to bind a UAV with the resource already being bound, then that's a validation error.
                for (int i = 0; i < bindings.Count; ++i)
                {
                    GorgonRenderTargetView rtv = _renderTargets[i];

                    if ((rtv == null) || ((rtv.Texture.Info.Binding & TextureBinding.UnorderedAccess) != TextureBinding.UnorderedAccess))
                    {
                        continue;
                    }

                    for (int j = 0; j < uavBindings.Count; ++j)
                    {
                        GorgonUavBinding uavBinding = _currentDrawCall.UnorderedAccessViews[j];

                        if ((uavBinding.Uav == null) || (uavBinding.Uav.Resource != rtv.Texture))
                        {
                            continue;
                        }

                        throw new GorgonException(GorgonResult.CannotBind, string.Format(Resources.GORGFX_ERR_CONFLICT_RTV_UAV, rtv.Texture.Name, j));
                    }
                }
            }

            GorgonRenderTargetViews.ValidateDepthStencilView(DepthStencilView, RenderTargets.FirstOrDefault(item => item != null));
#endif

            // If we have any resources assigned to an RTV/DSV output, and they're already assigned to shader resource inputs, then we need to unbind them.
            UnbindRtvInputs(ref bindings);

            D3DDeviceContext.OutputMerger.SetTargets(DepthStencilView?.Native, bindings.Count, _renderTargets.Native);
        }

        /// <summary>
        /// Function to assign the constant buffers to the resource list.
        /// </summary>
        /// <param name="shaderType">The type of shader to set the resources on.</param>
        /// <param name="buffers">The constant buffers to assign.</param>
        private void SetShaderConstantBuffers(ShaderType shaderType, GorgonConstantBuffers buffers)
        {
            ref (int StartSlot, int Count) bindings = ref buffers.GetDirtyItems();

            switch (shaderType)
            {
                case ShaderType.Pixel:
                    D3DDeviceContext.PixelShader.SetConstantBuffers(bindings.StartSlot, bindings.Count, buffers.Native);
                    break;
                case ShaderType.Vertex:
                    D3DDeviceContext.VertexShader.SetConstantBuffers(bindings.StartSlot, bindings.Count, buffers.Native);
                    break;
                case ShaderType.Geometry:
                    D3DDeviceContext.GeometryShader.SetConstantBuffers(bindings.StartSlot, bindings.Count, buffers.Native);
                    break;
                case ShaderType.Hull:
                    D3DDeviceContext.HullShader.SetConstantBuffers(bindings.StartSlot, bindings.Count, buffers.Native);
                    break;
                case ShaderType.Domain:
                    D3DDeviceContext.DomainShader.SetConstantBuffers(bindings.StartSlot, bindings.Count, buffers.Native);
                    break;
            }
        }

        /// <summary>
        /// Function to bind unordered access views to the resource list.
        /// </summary>
        /// <param name="uavs">The unordered access views to bind.</param>
        private void SetUavs(GorgonUavBindings uavs)
        {
            ref (int StartSlot, int Count) bindings = ref uavs.GetDirtyItems();

            if (_uavBuffer.Length != bindings.Count)
            {
                _uavBuffer = new D3D11.UnorderedAccessView[bindings.Count];
                _uavCounters = new int[_uavBuffer.Length];
            }

            for (int i = 0; i < _uavBuffer.Length; ++i)
            {
                _uavBuffer[i] = uavs.Native[i];
                _uavCounters[i] = uavs.Counts[i];
            }

            D3DDeviceContext.OutputMerger.SetUnorderedAccessViews(bindings.StartSlot, _uavBuffer, _uavCounters);
        }

        /// <summary>
        /// Function to validate a shader resource input to ensure the resource it is connected with is not bound to a UAV, RTV or DSV output.
        /// </summary>
        /// <param name="shaderType">The type of shader being bound.</param>
        /// <param name="srvs">The list of shader resource views.</param>
        /// <param name="start">The starting index to validate from.</param>
        /// <param name="end">The ending index to validate to.</param>
        private void ValidateSrvBinding(ShaderType shaderType, GorgonShaderResourceViews srvs, int start, int end)
        {
            ref (int, int Count) rtBindings = ref _renderTargets.GetDirtyItems();
            ref (int Start, int Count) uavBindings = ref _currentDrawCall.UnorderedAccessViews.GetDirtyItems();

            for (int i = start; i < end; ++i)
            {
                GorgonShaderResourceView srv = srvs[i];

                if (srv == null)
                {
                    continue;
                }

                // Check to see if we're bound as a DSV.
                if (srv.Resource == DepthStencilView?.Texture)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    throw new GorgonException(GorgonResult.CannotBind, string.Format(Resources.GORGFX_ERR_CONFLICT_SRV_DSV, srv.Resource.Name, shaderType));
                }

                for (int j = 0; j < rtBindings.Count; ++j)
                {
                    GorgonRenderTargetView rtv = _renderTargets[j];

                    // If we're trying to bind a shader resource while it's held as a render target, then reset the render target to null.
                    if ((rtv == null) || (rtv.Texture != srv.Resource))
                    {
                        continue;
                    }

                    throw new GorgonException(GorgonResult.CannotBind, string.Format(Resources.GORGFX_ERR_CONFLICT_SRV_RTV, srv.Resource.Name, shaderType));
                }

                // Evaluate unordered access views.
                for (int j = uavBindings.Start; j < uavBindings.Start + uavBindings.Count; ++j)
                {
                    GorgonUavBinding uav = _currentDrawCall.UnorderedAccessViews[j];

                    if ((uav.Uav == null)
                        || (uav.Uav.Resource != srv.Resource))
                    {
                        continue;
                    }

                    throw new GorgonException(GorgonResult.CannotBind, string.Format(Resources.GORGFX_ERR_CONFLICT_SRV_UAV, srv.Resource.Name, j, shaderType));
                }
            }
        }

        /// <summary>
        /// Function to assign the shader resource views to the resource list.
        /// </summary>
        /// <param name="shaderType">The type of shader to set the resources on.</param>
        /// <param name="srvs">The shader resource views to assign.</param>
        private void SetShaderResourceViews(ShaderType shaderType, GorgonShaderResourceViews srvs)
        {
            ref (int StartSlot, int Count) bindings = ref srvs.GetDirtyItems();

            switch (shaderType)
            {
                case ShaderType.Pixel:
                    D3DDeviceContext.PixelShader.SetShaderResources(bindings.StartSlot, bindings.Count, srvs.Native);
                    break;
                case ShaderType.Vertex:
                    D3DDeviceContext.VertexShader.SetShaderResources(bindings.StartSlot, bindings.Count, srvs.Native);
                    break;
                case ShaderType.Geometry:
                    D3DDeviceContext.GeometryShader.SetShaderResources(bindings.StartSlot, bindings.Count, srvs.Native);
                    break;
                case ShaderType.Hull:
                    D3DDeviceContext.HullShader.SetShaderResources(bindings.StartSlot, bindings.Count, srvs.Native);
                    break;
                case ShaderType.Domain:
                    D3DDeviceContext.DomainShader.SetShaderResources(bindings.StartSlot, bindings.Count, srvs.Native);
                    break;
            }
        }

        /// <summary>
        /// Function to assign the shader samplers to the resource list.
        /// </summary>
        /// <param name="shaderType">The type of shader to set the resources on.</param>
        /// <param name="samplers">The samplers to assign.</param>
        private void SetShaderSamplers(ShaderType shaderType, GorgonSamplerStates samplers)
        {
            ref (int StartSlot, int Count) bindings = ref samplers.GetDirtyItems();

            for (int i = bindings.StartSlot; i < bindings.StartSlot + bindings.Count; ++i)
            {
                GorgonSamplerState state = samplers[i];

                if (state == null)
                {
                    _samplerStates[i - bindings.StartSlot] = null;
                    continue;
                }

                if (state.Native == null)
                {
                    state.Native = SamplerStateFactory.GetSamplerState(this, state, _log);
                }

                _samplerStates[i - bindings.StartSlot] = state.Native;
            }

            switch (shaderType)
            {
                case ShaderType.Pixel:
                    D3DDeviceContext.PixelShader.SetSamplers(bindings.StartSlot, bindings.Count, _samplerStates);
                    break;
                case ShaderType.Vertex:
                    D3DDeviceContext.VertexShader.SetSamplers(bindings.StartSlot, bindings.Count, _samplerStates);
                    break;
                case ShaderType.Geometry:
                    D3DDeviceContext.GeometryShader.SetSamplers(bindings.StartSlot, bindings.Count, _samplerStates);
                    break;
                case ShaderType.Hull:
                    D3DDeviceContext.HullShader.SetSamplers(bindings.StartSlot, bindings.Count, _samplerStates);
                    break;
                case ShaderType.Domain:
                    D3DDeviceContext.DomainShader.SetSamplers(bindings.StartSlot, bindings.Count, _samplerStates);
                    break;
            }
        }

        /// <summary>
        /// Function to apply states that can be changed per draw call.
        /// </summary>
        /// <param name="drawCall">The draw call containing the direct states to change.</param>
        /// <param name="newState">The current pipeline state settings for the new draw call.</param>
        /// <param name="resourceChanges">The resource changes to apply.</param>
        /// <param name="stateChanges">The pipeline state changes to apply.</param>
        private void ApplyPerDrawStates(GorgonDrawCallBase drawCall,
                                        GorgonPipelineState newState,
                                        PipelineResourceChange resourceChanges,
                                        PipelineStateChange stateChanges)
        {
            if ((resourceChanges & PipelineResourceChange.PrimitiveTopology) == PipelineResourceChange.PrimitiveTopology)
            {
                D3DDeviceContext.InputAssembler.PrimitiveTopology = (D3D.PrimitiveTopology)drawCall.PrimitiveType;
            }

            // Bind the scissor rectangles.
            SetScissorRects();
            // Bind the active viewports.
            SetViewports();

            if ((resourceChanges & PipelineResourceChange.Uavs) == PipelineResourceChange.Uavs)
            {
                SetUavs(drawCall.UnorderedAccessViews);
            }

            if ((resourceChanges & PipelineResourceChange.InputLayout) == PipelineResourceChange.InputLayout)
            {
                D3DDeviceContext.InputAssembler.InputLayout = drawCall.VertexBuffers?.InputLayout.D3DInputLayout;
            }

            if ((resourceChanges & PipelineResourceChange.StreamOut) == PipelineResourceChange.StreamOut)
            {
                SetStreamOutBuffers(drawCall.StreamOutBuffers);
            }

            if ((resourceChanges & PipelineResourceChange.VertexBuffers) == PipelineResourceChange.VertexBuffers)
            {
                SetVertexBuffers(drawCall.VertexBuffers);
            }

            if ((resourceChanges & PipelineResourceChange.IndexBuffer) == PipelineResourceChange.IndexBuffer)
            {
                SetIndexbuffer(drawCall.IndexBuffer);
            }

            if ((resourceChanges & PipelineResourceChange.VertexShaderConstantBuffers) == PipelineResourceChange.VertexShaderConstantBuffers)
            {
                SetShaderConstantBuffers(ShaderType.Vertex, drawCall.VertexShaderConstantBuffers);
            }

            if ((resourceChanges & PipelineResourceChange.PixelShaderConstantBuffers) == PipelineResourceChange.PixelShaderConstantBuffers)
            {
                SetShaderConstantBuffers(ShaderType.Pixel, drawCall.PixelShaderConstantBuffers);
            }

            if ((resourceChanges & PipelineResourceChange.GeometryShaderConstantBuffers) == PipelineResourceChange.GeometryShaderConstantBuffers)
            {
                SetShaderConstantBuffers(ShaderType.Geometry, drawCall.GeometryShaderConstantBuffers);
            }

            if ((resourceChanges & PipelineResourceChange.HullShaderConstantBuffers) == PipelineResourceChange.HullShaderConstantBuffers)
            {
                SetShaderConstantBuffers(ShaderType.Hull, drawCall.HullShaderConstantBuffers);
            }

            if ((resourceChanges & PipelineResourceChange.DomainShaderConstantBuffers) == PipelineResourceChange.DomainShaderConstantBuffers)
            {
                SetShaderConstantBuffers(ShaderType.Domain, drawCall.DomainShaderConstantBuffers);
            }

            if ((resourceChanges & PipelineResourceChange.VertexShaderResources) == PipelineResourceChange.VertexShaderResources)
            {
                SetShaderResourceViews(ShaderType.Vertex, drawCall.VertexShaderResourceViews);
            }

            if ((resourceChanges & PipelineResourceChange.PixelShaderResources) == PipelineResourceChange.PixelShaderResources)
            {
                SetShaderResourceViews(ShaderType.Pixel, drawCall.PixelShaderResourceViews);
            }

            if ((resourceChanges & PipelineResourceChange.GeometryShaderResources) == PipelineResourceChange.GeometryShaderResources)
            {
                SetShaderResourceViews(ShaderType.Geometry, drawCall.GeometryShaderResourceViews);
            }

            if ((resourceChanges & PipelineResourceChange.HullShaderResources) == PipelineResourceChange.HullShaderResources)
            {
                SetShaderResourceViews(ShaderType.Hull, drawCall.HullShaderResourceViews);
            }

            if ((resourceChanges & PipelineResourceChange.DomainShaderResources) == PipelineResourceChange.DomainShaderResources)
            {
                SetShaderResourceViews(ShaderType.Domain, drawCall.DomainShaderResourceViews);
            }

            if ((resourceChanges & PipelineResourceChange.VertexShaderSamplers) == PipelineResourceChange.VertexShaderSamplers)
            {
                SetShaderSamplers(ShaderType.Vertex, drawCall.VertexShaderSamplers);
            }

            if ((resourceChanges & PipelineResourceChange.PixelShaderSamplers) == PipelineResourceChange.PixelShaderSamplers)
            {
                SetShaderSamplers(ShaderType.Pixel, drawCall.PixelShaderSamplers);
            }

            if ((resourceChanges & PipelineResourceChange.GeometryShaderSamplers) == PipelineResourceChange.GeometryShaderSamplers)
            {
                SetShaderSamplers(ShaderType.Geometry, drawCall.GeometryShaderSamplers);
            }

            if ((resourceChanges & PipelineResourceChange.HullShaderSamplers) == PipelineResourceChange.HullShaderSamplers)
            {
                SetShaderSamplers(ShaderType.Hull, drawCall.HullShaderSamplers);
            }

            if ((resourceChanges & PipelineResourceChange.DomainShaderSamplers) == PipelineResourceChange.DomainShaderSamplers)
            {
                SetShaderSamplers(ShaderType.Domain, drawCall.DomainShaderSamplers);
            }

            if ((resourceChanges & PipelineResourceChange.BlendFactor) == PipelineResourceChange.BlendFactor)
            {
                D3DDeviceContext.OutputMerger.BlendFactor = drawCall.BlendFactor.ToRawColor4();
            }

            if ((resourceChanges & PipelineResourceChange.BlendSampleMask) == PipelineResourceChange.BlendSampleMask)
            {
                D3DDeviceContext.OutputMerger.BlendSampleMask = drawCall.BlendSampleMask;
            }

            if ((resourceChanges & PipelineResourceChange.DepthStencilReference) == PipelineResourceChange.DepthStencilReference)
            {
                D3DDeviceContext.OutputMerger.DepthStencilReference = drawCall.DepthStencilReference;
            }

            if (stateChanges != PipelineStateChange.None)
            {
                ApplyPipelineState(newState, stateChanges);
            }
        }

        /// <summary>
        /// Function to force UAVs that are bound as inputs to be unbound from the input stages.
        /// </summary>
        /// <param name="uavBindings">The binding range.</param>
        /// <param name="uavs">The list of bindings.</param>
        /// <param name="forceUnbind"><b>true</b> to force the slots to unbind from the GPU, or <b>false</b> to delay the unbinding until later.</param>
        private void UnbindUavInputs(ref (int Start, int Count) uavBindings, GorgonUavBindings uavs, bool forceUnbind)
        {
            if ((uavs == null) || (_currentDrawCall == null))
            {
                return;
            }

            ref (int, int) psSrvBindings = ref _currentDrawCall.PixelShaderResourceViews.GetDirtyItems();
            ref (int, int) gsSrvBindings = ref _currentDrawCall.GeometryShaderResourceViews.GetDirtyItems();
            ref (int, int) vsSrvBindings = ref _currentDrawCall.VertexShaderResourceViews.GetDirtyItems();
            ref (int, int) hsSrvBindings = ref _currentDrawCall.HullShaderResourceViews.GetDirtyItems();
            ref (int, int) dsSrvBindings = ref _currentDrawCall.DomainShaderResourceViews.GetDirtyItems();

            for (int i = uavBindings.Start; i < uavBindings.Start + uavBindings.Count; ++i)
            {
                GorgonUavBinding uavBinding = uavs[i];

                if ((uavBinding.Uav == null) || (!uavBinding.Uav.Resource.IsShaderResource))
                {
                    continue;
                }

                UnbindFromShader(ShaderType.Pixel, uavBinding.Uav.Resource, ref psSrvBindings, _currentDrawCall.PixelShaderResourceViews);
                UnbindFromShader(ShaderType.Geometry, uavBinding.Uav.Resource, ref gsSrvBindings, _currentDrawCall.GeometryShaderResourceViews);
                UnbindFromShader(ShaderType.Vertex, uavBinding.Uav.Resource, ref vsSrvBindings, _currentDrawCall.VertexShaderResourceViews);
                UnbindFromShader(ShaderType.Hull, uavBinding.Uav.Resource, ref hsSrvBindings, _currentDrawCall.HullShaderResourceViews);
                UnbindFromShader(ShaderType.Domain, uavBinding.Uav.Resource, ref dsSrvBindings, _currentDrawCall.DomainShaderResourceViews);
            }

            void UnbindFromShader(ShaderType shaderType, GorgonGraphicsResource resource, ref (int Start, int Count) bindings, GorgonShaderResourceViews srvs)
            {
                bool unbound = false;

                if (bindings.Count == 0)
                {
                    return;
                }

                for (int i = bindings.Start; i < bindings.Start + bindings.Count; ++i)
                {
                    GorgonShaderResourceView srv = srvs[i];

                    if ((srv == null) || (resource != srv.Resource))
                    {
                        continue;
                    }

                    srvs[i] = null;
                    unbound = true;
                }

                if ((forceUnbind) && (unbound))
                {
                    SetShaderResourceViews(shaderType, srvs);
                }
            }
        }

        

        /// <summary>
        /// Function to validate the compute pipeline state against the graphics pipeline state.
        /// </summary>
        internal void ValidateComputeWork(GorgonUavBindings uavBindings, ref (int Start, int Count) uavRange)
        {
            // Unbind any UAVs bound as input.
            if (uavRange.Count > 0)
            {
                UnbindUavInputs(ref uavRange, uavBindings, true);
            }
        }

        /// <summary>
        /// Function to assign a single render target to the first slot.
        /// </summary>
        /// <param name="renderTarget">The render target view to assign.</param>
        /// <remarks>
        /// <para>
        /// This will assign a render target in slot 0 of the <see cref="RenderTargets"/> list. If the <paramref name="renderTarget"/> has an associated depth/stencil buffer, then that will be assigned to the 
        /// <see cref="DepthStencilView"/>. If it does not, the <see cref="DepthStencilView"/> will be set to <b>null</b>.
        /// </para>
        /// <para>
        /// When a render target is set, the first scissor rectangle in the <see cref="ScissorRectangles"/> list and the first viewport in the <see cref="Viewports"/> list will be reset to the size of the 
        /// render target. The user is responsible for restoring these to their intended values after assigning the target.
        /// </para>
        /// <para>
        /// <note type="information">
        /// <para>
        /// If the <see cref="RenderTargets"/> list contains other render target views at different slots, they will be unbound.
        /// </para>
        /// </note>
        /// </para>
        /// <para>
        /// If a user wishes to assign their own <see cref="GorgonDepthStencilView"/>, then use the <see cref="SetRenderTarget(GorgonRenderTargetView, GorgonDepthStencilView)"/> overload.
        /// </para>
        /// <para>
        /// If multiple render targets need to be assigned at the same time, use the <see cref="SetRenderTargets(GorgonRenderTargetView[], GorgonDepthStencilView)"/> overload.
        /// </para>
        /// </remarks>
        /// <seealso cref="SetRenderTarget(GorgonRenderTargetView, GorgonDepthStencilView)"/>
        /// <seealso cref="SetRenderTargets(GorgonRenderTargetView[], GorgonDepthStencilView)"/>
        /// <seealso cref="RenderTargets"/>
        /// <seealso cref="DepthStencilView"/>
        /// <seealso cref="ScissorRectangles"/>
        /// <seealso cref="Viewports"/>
        public void SetRenderTarget(GorgonRenderTargetView renderTarget)
        {
            SetRenderTarget(renderTarget, renderTarget?.DepthStencilView);
        }

        /// <summary>
        /// Function to assign a render target to the first slot and a custom depth/stencil view.
        /// </summary>
        /// <param name="renderTarget">The render target view to assign.</param>
        /// <param name="depthStencil">The depth/stencil view to assign.</param>
        /// <remarks>
        /// <para>
        /// This will assign a render target in slot 0 of the <see cref="RenderTargets"/> list. Any associated depth/stencil buffer for the render target will be ignored and the value assigned to 
        /// <paramref name="depthStencil"/> will be used instead.
        /// </para>
        /// <para>
        /// <note type="information">
        /// <para>
        /// If the <see cref="RenderTargets"/> list contains other render target views at different slots, they will be unbound.
        /// </para>
        /// </note>
        /// </para>
        /// <para>
        /// If a user wishes to use the associated <see cref="GorgonDepthStencilView"/> for the <paramref name="renderTarget"/>, then use the <see cref="SetRenderTarget(GorgonRenderTargetView)"/> overload.
        /// </para>
        /// <para>
        /// If multiple render targets need to be assigned at the same time, use the <see cref="SetRenderTargets(GorgonRenderTargetView[], GorgonDepthStencilView)"/> overload.
        /// </para>
        /// <para>
        /// When binding a <see cref="GorgonDepthStencilView"/>, the resource must be of the same type as other resources for other views in this list. If they do not match, an exception will be thrown.
        /// </para>
        /// <para>
        /// The <see cref="GorgonDepthStencilView"/> values for the <paramref name="depthStencil"/> (such as array (or depth) index and array (or depth) count) must be the same as the other views in this list. 
        /// If they are not, an exception will be thrown. Mip slices may be different. An exception will also be raised if the resources assigned to the <paramref name="renderTarget"/> do not have the same 
        /// array/depth count.
        /// </para>
        /// <para>
        /// If the <see cref="GorgonRenderTargetView">GorgonRenderTargetViews</see> are attached to resources with multisampling enabled through <see cref="GorgonMultisampleInfo"/>, then the 
        /// <see cref="GorgonMultisampleInfo"/> of the resource attached to the <see cref="GorgonDepthStencilView"/> being assigned must match, or an exception will be thrown.
        /// </para>
        /// <para>
        /// When a render target is set, the first scissor rectangle in the <see cref="ScissorRectangles"/> list and the first viewport in the <see cref="Viewports"/> list will be reset to the size of the 
        /// render target. The user is responsible for restoring these to their intended values after assigning the target.
        /// </para>
        /// <para>
        /// <note type="information">
        /// <para>
        /// The exceptions raised when validating a view against other views in this list are only thrown when Gorgon is compiled as <b>DEBUG</b>.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        /// <seealso cref="SetRenderTarget(GorgonRenderTargetView)"/>
        /// <seealso cref="SetRenderTargets(GorgonRenderTargetView[], GorgonDepthStencilView)"/>
        /// <seealso cref="RenderTargets"/>
        /// <seealso cref="DepthStencilView"/>
        /// <seealso cref="ScissorRectangles"/>
        /// <seealso cref="RenderTargets"/>
        public void SetRenderTarget(GorgonRenderTargetView renderTarget, GorgonDepthStencilView depthStencil)
        {
            if ((_renderTargets[0] == renderTarget) && (depthStencil == DepthStencilView))
            {
                return;
            }

            _renderTargets.Clear();
            _renderTargets[0] = renderTarget;

            if (_renderTargets[0] != null)
            {
                ScissorRectangles[0] = _renderTargets[0].Bounds;
                Viewports[0] = new DX.ViewportF(0, 0, _renderTargets[0].Bounds.Width, _renderTargets[0].Bounds.Height);
            }
            else
            {
                ScissorRectangles[0] = DX.Rectangle.Empty;
                Viewports[0] = default(DX.ViewportF);
            }

            DepthStencilView = depthStencil;
            SetRenderTargetAndDepthViews();
        }

        /// <summary>
        /// Function to assign multiple render targets to the first slot and a custom depth/stencil view.
        /// </summary>
        /// <param name="renderTargets">The list of render target views to assign.</param>
        /// <param name="depthStencil">The depth/stencil view to assign.</param>
        /// <remarks>
        /// <para>
        /// This will assign multiple render targets to the corresponding slots in the <see cref="RenderTargets"/> list. Any associated depth/stencil buffer for the render target will be ignored and the value 
        /// assigned to <paramref name="depthStencil"/> will be used instead.
        /// </para>
        /// <para>
        /// If a user wishes to use the associated <see cref="GorgonDepthStencilView"/> for the <paramref name="renderTargets"/>, then use the <see cref="SetRenderTarget(GorgonRenderTargetView)"/> overload.
        /// </para>
        /// <para>
        /// When binding a <see cref="GorgonDepthStencilView"/>, the resource must be of the same type as other resources for other views in this list. If they do not match, an exception will be thrown.
        /// </para>
        /// <para>
        /// The <see cref="GorgonDepthStencilView"/> values for the <paramref name="depthStencil"/> (such as array (or depth) index and array (or depth) count) must be the same as the other views in this list. 
        /// If they are not, an exception will be thrown. Mip slices may be different. An exception will also be raised if the resources assigned to the <paramref name="renderTargets"/> do not have the same 
        /// array/depth count.
        /// </para>
        /// <para>
        /// If the <see cref="GorgonRenderTargetView">GorgonRenderTargetViews</see> are attached to resources with multisampling enabled through <see cref="GorgonMultisampleInfo"/>, then the 
        /// <see cref="GorgonMultisampleInfo"/> of the resource attached to the <see cref="GorgonDepthStencilView"/> being assigned must match, or an exception will be thrown.
        /// </para>
        /// <para>
        /// The format for the <paramref name="renderTargets"/> and <paramref name="depthStencil"/> may differ from the formats of other views passed in.
        /// </para>
        /// <para>
        /// When a render target is set, the first scissor rectangle in the <see cref="ScissorRectangles"/> list and the first viewport in the <see cref="Viewports"/> list will be reset to the size of the 
        /// first render target. The user is responsible for restoring these to their intended values after assigning the targets.
        /// </para>
        /// <para>
        /// <note type="information">
        /// <para>
        /// The exceptions raised when validating a view against other views in this list are only thrown when Gorgon is compiled as <b>DEBUG</b>.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        /// <seealso cref="SetRenderTarget(GorgonRenderTargetView)"/>
        /// <seealso cref="SetRenderTarget(GorgonRenderTargetView, GorgonDepthStencilView)"/>
        /// <seealso cref="RenderTargets"/>
        /// <seealso cref="DepthStencilView"/>
        /// <seealso cref="ScissorRectangles"/>
        /// <seealso cref="Viewports"/>
        public void SetRenderTargets(GorgonRenderTargetView[] renderTargets, GorgonDepthStencilView depthStencil = null)
        {
            _renderTargets.Clear();

            if ((renderTargets == null)
                || (renderTargets.Length == 0))
            {
                ScissorRectangles[0] = DX.Rectangle.Empty;
                DepthStencilView = depthStencil;
                SetRenderTargetAndDepthViews();
                return;
            }

            for (int i = 0; i < renderTargets.Length.Min(_renderTargets.Count); ++i)
            {
                _renderTargets[i] = renderTargets[i];
            }

            if (_renderTargets[0] != null)
            {
                ScissorRectangles[0] = renderTargets[0].Bounds;
                Viewports[0] = new DX.ViewportF(0, 0, renderTargets[0].Bounds.Width, renderTargets[0].Bounds.Height);
            }
            else
            {
                ScissorRectangles[0] = DX.Rectangle.Empty;
                Viewports[0] = default(DX.ViewportF);
            }

            if ((!_renderTargets.IsDirty) && (DepthStencilView == depthStencil))
            {
                return;
            }

            DepthStencilView = depthStencil;
            SetRenderTargetAndDepthViews();
        }

        /// <summary>
        /// Function to clear the cached pipeline states.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will destroy any previously cached pipeline states. Because of this, any states that were previously created must be re-created using the <seealso cref="GetPipelineState"/> method.
        /// </para>
        /// </remarks>
        public void ClearStateCache()
        {
            if (D3DDeviceContext != null)
            {
                ClearState();
            }

            lock (_stateCacheLock)
            {
                // Ensure that all groups lose their reference to the cached pipeline states first.
                foreach (KeyValuePair<string, IGorgonPipelineStateGroup> cacheGroup in _groupedCache)
                {
                    cacheGroup.Value.Invalidate();
                }

                // Wipe out the state cache.
                // ReSharper disable once ForCanBeConvertedToForeach
                for (int i = 0; i < _stateCache.Count; ++i)
                {
                    _stateCache[i].D3DRasterState?.Dispose();
                    _stateCache[i].D3DDepthStencilState?.Dispose();
                    _stateCache[i].D3DBlendState?.Dispose();
                }

                _stateCache.Clear();

                SamplerStateFactory.ClearCache();
            }
        }

        /// <summary>
        /// Function to submit a <see cref="GorgonDrawIndexedInstancedCall"/> to the GPU using a <see cref="GorgonBuffer"/> to pass in variable sized arguments.
        /// </summary>
        /// <param name="drawCall">The draw call to submit.</param>
        /// <param name="indirectArgs">The buffer containing the draw call arguments to pass.</param>
        /// <param name="argumentOffset">[Optional] The offset, in bytes, within the buffer to start reading the arguments from.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="drawCall"/>, or the <paramref name="indirectArgs"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="argumentOffset"/> parameter is less than 0.</exception>
        /// <exception cref="GorgonException">Thrown if the <paramref name="indirectArgs"/> was not created with the <see cref="IGorgonBufferInfo.IndirectArgs"/> flag set to <b>true</b>.</exception>
        /// <remarks>
        /// <para>
        /// This allows submitting a <see cref="GorgonDrawIndexedInstancedCall"/> with variable arguments without having to perform a read back of that data from the GPU and therefore avoid a stall. 
        /// </para>
        /// <para>
        /// Like the <see cref="SubmitStreamOut"/> method, this is useful when a shader generates an arbitrary amount of data within a buffer. To get the size, or the data itself out of the buffer will 
        /// cause a stall when swtiching back to the CPU. This is obviously not good for performance. So, to counter this, this method will pass the buffer with the arguments for the draw call straight 
        /// through without having to get the CPU to read the data back, thus avoiding the stall.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// <para>
        /// For performance reasons, any exceptions thrown from this method will only be thrown when Gorgon is compiled as DEBUG.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        /// <seealso cref="GorgonDrawIndexedInstancedCall"/>
        public void SubmitIndirect(GorgonDrawIndexedInstancedCall drawCall, GorgonBuffer indirectArgs, int argumentOffset = 0)
        {
            drawCall.ValidateObject(nameof(drawCall));
            indirectArgs.ValidateObject(nameof(indirectArgs));

#if DEBUG
            if (!indirectArgs.Info.IndirectArgs)
            {
                throw new GorgonException(GorgonResult.AccessDenied, string.Format(Resources.GORGFX_ERR_BUFFER_IS_NOT_INDIRECTARGS, indirectArgs.Name));
            }
#endif

            // Merge this draw call with our previous one (if available).
            (PipelineResourceChange ChangedResources, PipelineStateChange ChangedStates) = MergeDrawCall(drawCall);

            ApplyPerDrawStates(_currentDrawCall, drawCall.PipelineState, ChangedResources, ChangedStates);

            D3DDeviceContext.DrawIndexedInstancedIndirect(indirectArgs.NativeBuffer, argumentOffset);
        }

        /// <summary>
        /// Function to submit a <see cref="GorgonDrawInstancedCall"/> to the GPU using a <see cref="GorgonBuffer"/> to pass in variable sized arguments.
        /// </summary>
        /// <param name="drawCall">The draw call to submit.</param>
        /// <param name="indirectArgs">The buffer containing the draw call arguments to pass.</param>
        /// <param name="argumentOffset">[Optional] The offset, in bytes, within the buffer to start reading the arguments from.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="drawCall"/>, or the <paramref name="indirectArgs"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="argumentOffset"/> parameter is less than 0.</exception>
        /// <exception cref="GorgonException">Thrown if the <paramref name="indirectArgs"/> was not created with the <see cref="IGorgonBufferInfo.IndirectArgs"/> flag set to <b>true</b>.</exception>
        /// <remarks>
        /// <para>
        /// This allows submitting a <see cref="GorgonDrawInstancedCall"/> with variable arguments without having to perform a read back of that data from the GPU and therefore avoid a stall. 
        /// </para>
        /// <para>
        /// Like the <see cref="SubmitStreamOut"/> method, this is useful when a shader generates an arbitrary amount of data within a buffer. To get the size, or the data itself out of the buffer will 
        /// cause a stall when swtiching back to the CPU. This is obviously not good for performance. So, to counter this, this method will pass the buffer with the arguments for the draw call straight 
        /// through without having to get the CPU to read the data back, thus avoiding the stall.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// <para>
        /// For performance reasons, any exceptions thrown from this method will only be thrown when Gorgon is compiled as DEBUG.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        /// <seealso cref="GorgonDrawIndexedInstancedCall"/>
        public void SubmitIndirect(GorgonDrawInstancedCall drawCall, GorgonBuffer indirectArgs, int argumentOffset = 0)
        {
            drawCall.ValidateObject(nameof(drawCall));
            indirectArgs.ValidateObject(nameof(indirectArgs));

#if DEBUG
            if (!indirectArgs.Info.IndirectArgs)
            {
                throw new GorgonException(GorgonResult.AccessDenied, string.Format(Resources.GORGFX_ERR_BUFFER_IS_NOT_INDIRECTARGS, indirectArgs.Name));
            }
#endif

            // Merge this draw call with our previous one (if available).
            (PipelineResourceChange ChangedResources, PipelineStateChange ChangedStates) = MergeDrawCall(drawCall);

            ApplyPerDrawStates(_currentDrawCall, drawCall.PipelineState, ChangedResources, ChangedStates);

            D3DDeviceContext.DrawInstancedIndirect(indirectArgs.NativeBuffer, argumentOffset);
        }

        /// <summary>
        /// Function to submit a <see cref="GorgonDrawCallBase"/> to the GPU.
        /// </summary>
        /// <param name="drawCall">The draw call to submit.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="drawCall"/> parameter is <b>null</b>.</exception>
        /// <remarks>
        /// <para>
        /// This method sends a series of state changes and resource bindings to the GPU. However, unlike the <see cref="O:Gorgon.Graphics.Core.GorgonGraphics.Submit"/> commands, this command uses 
        /// pre-processed data from the vertex and stream out stages. This means that the <see cref="GorgonVertexBuffer"/> attached to the draw call must have been assigned to the 
        /// <see cref="GorgonDrawCallBase.StreamOutBuffers"/> and had data deposited into it from the stream out stage. After that, it should be removed from the 
        /// <see cref="GorgonDrawCallBase.StreamOutBuffers"/> and assigned to the <see cref="GorgonDrawCallBase.VertexBuffers"/> on the <paramref name="drawCall"/> passed to this method.
        /// </para>
        /// <para>
        /// To render data with this method, the <see cref="GorgonVertexBuffer"/> being rendered must be at slot 0 in the <see cref="GorgonDrawCallBase.VertexBuffers"/> list on the 
        /// <paramref name="drawCall"/> passed to the method. This buffer must be created with the <see cref="VertexIndexBufferBinding.StreamOut"/> flag set.
        /// </para>
        /// <para>
        /// Draw calls with a start and count property (for indices, vertices, etc...) will work with this method, but those properties are ignored because the actual size of the data being sent is unknown 
        /// at the application level. The GPU will track the size of the buffer being rendered. The <see cref="IGorgonPipelineStateInfo.VertexShader"/> of the <see cref="GorgonDrawCallBase.PipelineState"/> 
        /// will be ignored as well as the vertex data being passed is already processed by a vertex shader.
        /// </para>
        /// <para>
        /// This method does not support the use of a <see cref="GorgonIndexBuffer"/>, if one is bound to the draw call, it will be unbound and a warning will go to the log.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// <para>
        /// For performance reasons, any exceptions thrown from this method will only be thrown when Gorgon is compiled as DEBUG.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public void SubmitStreamOut(GorgonDrawCallBase drawCall)
        {
            drawCall.ValidateObject(nameof(drawCall));

            // Merge this draw call with our previous one (if available).
            (PipelineResourceChange ChangedResources, PipelineStateChange ChangedStates) = MergeDrawCall(drawCall);

            // Unbind the index buffer if one is present.
            if (_currentDrawCall.IndexBuffer != null)
            {
                _log.Print("The SubmitStreamOut method does not support the use of index buffers. The current index buffer will be reset to NULL.",
                           LoggingLevel.Verbose);
                _currentDrawCall.IndexBuffer = null;
                ChangedResources |= PipelineResourceChange.IndexBuffer;
            }

#if DEBUG
            if (_currentDrawCall.PipelineState.Info.VertexShader != null)
            {
                _log.Print("The SubmitStreamOut method has a vertex shader bound to its pipeline state. This may have unintended effects on the rendered geometry.",
                           LoggingLevel.Verbose);
            }
#endif

            ApplyPerDrawStates(_currentDrawCall, drawCall.PipelineState, ChangedResources, ChangedStates);

            D3DDeviceContext.DrawAuto();
        }

        /// <summary>
        /// Function to submit a <see cref="GorgonDrawIndexedCall"/> to the GPU.
        /// </summary>
        /// <param name="drawCall">The draw call to submit.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="drawCall"/> parameter is <b>null</b>.</exception>
        /// <remarks>
        /// <para>
        /// This method sends a series of state changes and resource bindings to the GPU along with a command to render primitive data with a <see cref="GorgonIndexBuffer"/>.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// <para>
        /// For performance reasons, any exceptions thrown from this method will only be thrown when Gorgon is compiled as DEBUG.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        /// <seealso cref="GorgonIndexBuffer"/>
        public void Submit(GorgonDrawIndexedCall drawCall)
        {
            drawCall.ValidateObject(nameof(drawCall));

            // Merge this draw call with our previous one (if available).
            (PipelineResourceChange ChangedResources, PipelineStateChange ChangedStates) = MergeDrawCall(drawCall);

            ApplyPerDrawStates(_currentDrawCall, drawCall.PipelineState, ChangedResources, ChangedStates);

            D3DDeviceContext.DrawIndexed(drawCall.IndexCount, drawCall.IndexStart, drawCall.BaseVertexIndex);
        }

        /// <summary>
        /// Function to submit a <see cref="GorgonDrawCall"/> to the GPU.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="drawCall"/> parameter is <b>null</b>.</exception>
        /// <param name="drawCall">The draw call to submit.</param>
        /// <remarks>
        /// <para>
        /// This method sends a series of state changes and resource bindings to the GPU along with a command to render primitive data.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// <para>
        /// For performance reasons, any exceptions thrown from this method will only be thrown when Gorgon is compiled as DEBUG.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public void Submit(GorgonDrawCall drawCall)
        {
            drawCall.ValidateObject(nameof(drawCall));

            // Merge this draw call with our previous one (if available).
            (PipelineResourceChange ChangedResources, PipelineStateChange ChangedStates) = MergeDrawCall(drawCall);

            ApplyPerDrawStates(_currentDrawCall, drawCall.PipelineState, ChangedResources, ChangedStates);

            D3DDeviceContext.Draw(drawCall.VertexCount, drawCall.VertexStartIndex);
        }

        /// <summary>
        /// Function to submit a <see cref="GorgonDrawInstancedCall"/> to the GPU.
        /// </summary>
        /// <param name="drawCall">The draw call to submit.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="drawCall"/> parameter is <b>null</b>.</exception>
        /// <remarks>
        /// <para>
        /// This method sends a series of state changes and resource bindings to the GPU along with a command to render instanced primitive data.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// <para>
        /// For performance reasons, any exceptions thrown from this method will only be thrown when Gorgon is compiled as DEBUG.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public void Submit(GorgonDrawInstancedCall drawCall)
        {
            drawCall.ValidateObject(nameof(drawCall));

            // Merge this draw call with our previous one (if available).
            (PipelineResourceChange ChangedResources, PipelineStateChange ChangedStates) = MergeDrawCall(drawCall);

            ApplyPerDrawStates(_currentDrawCall, drawCall.PipelineState, ChangedResources, ChangedStates);

            D3DDeviceContext.DrawInstanced(drawCall.VertexCountPerInstance, drawCall.InstanceCount, drawCall.VertexStartIndex, drawCall.StartInstanceIndex);
        }

        /// <summary>
        /// Function to submit a <see cref="GorgonDrawIndexedInstancedCall"/> to the GPU.
        /// </summary>
        /// <param name="drawCall">The draw call to submit.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="drawCall"/> parameter is <b>null</b>.</exception>
        /// <remarks>
        /// <para>
        /// This method sends a series of state changes and resource bindings to the GPU along with a command to render instanced primitive data with a <see cref="GorgonIndexBuffer"/>.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// <para>
        /// For performance reasons, any exceptions thrown from this method will only be thrown when Gorgon is compiled as DEBUG.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        /// <seealso cref="GorgonIndexBuffer"/>
        public void Submit(GorgonDrawIndexedInstancedCall drawCall)
        {
            drawCall.ValidateObject(nameof(drawCall));

            // Merge this draw call with our previous one (if available).
            (PipelineResourceChange ChangedResources, PipelineStateChange ChangedStates) = MergeDrawCall(drawCall);

            ApplyPerDrawStates(_currentDrawCall, drawCall.PipelineState, ChangedResources, ChangedStates);

            D3DDeviceContext.DrawIndexedInstanced(drawCall.IndexCountPerInstance,
                                                  drawCall.InstanceCount,
                                                  drawCall.IndexStart,
                                                  drawCall.BaseVertexIndex,
                                                  drawCall.StartInstanceIndex);
        }

        /// <summary>
        /// Function to render a texture to the current <see cref="GorgonRenderTargetView"/>.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="x">The horizontal destination position to render into.</param>
        /// <param name="y">The vertical destination position to render into.</param>
        /// <param name="width">[Optional] The destination width to render.</param>
        /// <param name="height">[Optional] The destination height to render.</param>
        /// <param name="srcX">[Optional] The horizontal pixel position in the texture to start rendering from.</param>
        /// <param name="srcY">[Optional] The vertical pixel position in the texture to start rendering from.</param>
        /// <param name="color">[Optional] The color used to tint the diffuse of the texture.</param>
        /// <param name="clip">[Optional] <b>true</b> to clip the texture if the destination width or height are larger or smaller than the original texture size, or <b>false</b> to scale the texture to meet the new size.</param>
        /// <param name="blendState">[Optional] The blending state to apply when rendering.</param>
        /// <param name="samplerState">[Optional] The sampler state to apply when rendering.</param>
        /// <param name="pixelShader">[Optional] A pixel shader used to override the default pixel shader for the rendering.</param>
        /// <param name="pixelShaderConstants">[Optional] Pixel shader constants to apply when rendering with a custom pixel shader.</param>
        /// <remarks>
        /// <para>
        /// This will render a <see cref="GorgonTexture"/> to the current <see cref="GorgonRenderTargetView"/> in slot 0 of the <see cref="RenderTargets"/> list. This is used a quick means to send graphics 
        /// data to the display without having to set up a <see cref="GorgonDrawCallBase">draw call</see> and submitting it.
        /// </para>
        /// <para>
        /// There are many optional parameters for this method that can alter how the texture is rendered. If there are no extra parameters supplied beyond the <paramref name="texture"/> and position, then 
        /// the texture will render as-is at the desired location. This is sufficient for most use cases. However, this method provides a lot of functionality to allow a user to quickly test out various 
        /// effects when rendering a texture:
        /// <list type="bullet">
        ///     <item>
        ///         Providing a width and height will render the texture at that width and height, or will clip to that width and height depending on the state of the <paramref name="clip"/> parameter.
        ///     </item>
        ///     <item>
        ///         Providing the source offset parameters will make the texture start rendering from that pixel coordinate within the texture itself. Omitting these coordinates will mean 
        ///         that rendering starts at (0, 0).
        ///     </item>
        ///     <item>
        ///         Providing a <paramref name="color"/> will render the texture with a tint of the color specified to the diffuse channels of the texture. If omitted, then the 
        ///         <see cref="GorgonColor.White"/> color will be used.
        ///     </item>
        ///     <item>
        ///         Providing a <paramref name="clip"/> value of <b>true</b> will tell the renderer to clip to the width and height specified. If omitted, then the texture will scale to the width and 
        ///         height provided.
        ///     </item>
        ///     <item>
        ///         Providing a <paramref name="blendState"/> will define how the texture is rendered against the render target and can allow for transparency effects. If omitted, then the 
        ///         <see cref="GorgonBlendState.NoBlending"/> state will be used and the texture will be rendered as opaque.
        ///     </item>
        ///     <item>
        ///         Providing a <paramref name="samplerState"/> will define how to smooth a scaled texture. If omitted, then the <see cref="GorgonSamplerState.Default"/> will be used and the texture 
        ///         will be rendered with bilinear filtering.
        ///     </item>
        ///     <item>
        ///         If a <paramref name="pixelShader"/> is defined, then the texture will be rendered with the specified pixel shader instead of the default one. This will allow for a variety of effects 
        ///         to be applied to the texture while rendering. The companion parameter <paramref name="pixelShaderConstants"/> will also be used to control how the pixel shader renders data based on 
        ///         user input. If the <paramref name="pixelShader"/> parameter is omitted, then a default pixel shader is used and the <paramref name="pixelShaderConstants"/> parameter is ignored.
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso cref="GorgonTexture"/>
        /// <seealso cref="GorgonRenderTargetView"/>
        /// <seealso cref="RenderTargets"/>
        public void DrawTexture(GorgonTextureView texture,
                                int x,
                                int y,
                                int width = -1,
                                int height = -1,
                                int srcX = 0,
                                int srcY = 0,
                                GorgonColor? color = null,
                                bool clip = false,
                                GorgonBlendState blendState = null,
                                GorgonSamplerState samplerState = null,
                                GorgonPixelShader pixelShader = null,
                                GorgonConstantBuffers pixelShaderConstants = null)
        {
            DrawTexture(texture,
                        new DX.Rectangle(x, y, width == -1 ? texture.Width : width, height == -1 ? texture.Height : height),
                        new DX.Point(srcX, srcY),
                        color,
                        clip,
                        blendState,
                        samplerState,
                        pixelShader,
                        pixelShaderConstants);
        }

        /// <summary>
        /// Function to render a texture to the current <see cref="GorgonRenderTargetView"/>.
        /// </summary>
        /// <param name="texture">The texture to render.</param>
        /// <param name="destRect">The destination rectangle representing the coordinates to render into.</param>
        /// <param name="sourceOffset">The source horizontal and vertical offset within the source texture to start rendering from.</param>
        /// <param name="color">[Optional] The color used to tint the diffuse of the texture.</param>
        /// <param name="clip">[Optional] <b>true</b> to clip the texture if the destination width or height are larger or smaller than the original texture size, or <b>false</b> to scale the texture to meet the new size.</param>
        /// <param name="blendState">[Optional] The blending state to apply when rendering.</param>
        /// <param name="samplerState">[Optional] The sampler state to apply when rendering.</param>
        /// <param name="pixelShader">[Optional] A pixel shader used to override the default pixel shader for the rendering.</param>
        /// <param name="pixelShaderConstants">[Optional] Pixel shader constants to apply when rendering with a custom pixel shader.</param>
        /// <remarks>
        /// <para>
        /// This will render a <see cref="GorgonTexture"/> to the current <see cref="GorgonRenderTargetView"/> in slot 0 of the <see cref="RenderTargets"/> list. This is used a quick means to send graphics 
        /// data to the display without having to set up a <see cref="GorgonDrawCallBase">draw call</see> and submitting it.
        /// </para>
        /// <para>
        /// There are many optional parameters for this method that can alter how the texture is rendered. If there are no extra parameters supplied beyond the <paramref name="texture"/> and 
        /// <paramref name="destRect"/>, then the texture will render, scaled, to the destination rectangle. This is sufficient for most use cases. However, this method provides a lot of functionality to 
        /// allow a user to quickly test out various effects when rendering a texture:
        /// <list type="bullet">
        ///     <item>
        ///         Providing the <paramref name="sourceOffset"/> parameters will make the texture start rendering from that pixel coordinate within the texture itself. Omitting these coordinates will 
        ///         mean that rendering starts at (0, 0).
        ///     </item>
        ///     <item>
        ///         Providing a <paramref name="color"/> will render the texture with a tint of the color specified to the diffuse channels of the texture. If omitted, then the 
        ///         <see cref="GorgonColor.White"/> color will be used.
        ///     </item>
        ///     <item>
        ///         Providing a <paramref name="clip"/> value of <b>true</b> will tell the renderer to clip to the width and height specified. If omitted, then the texture will scale to the width and 
        ///         height provided.
        ///     </item>
        ///     <item>
        ///         Providing a <paramref name="blendState"/> will define how the texture is rendered against the render target and can allow for transparency effects. If omitted, then the 
        ///         <see cref="GorgonBlendState.NoBlending"/> state will be used and the texture will be rendered as opaque.
        ///     </item>
        ///     <item>
        ///         Providing a <paramref name="samplerState"/> will define how to smooth a scaled texture. If omitted, then the <see cref="GorgonSamplerState.Default"/> will be used and the texture 
        ///         will be rendered with bilinear filtering.
        ///     </item>
        ///     <item>
        ///         If a <paramref name="pixelShader"/> is defined, then the texture will be rendered with the specified pixel shader instead of the default one. This will allow for a variety of effects 
        ///         to be applied to the texture while rendering. The companion parameter <paramref name="pixelShaderConstants"/> will also be used to control how the pixel shader renders data based on 
        ///         user input. If the <paramref name="pixelShader"/> parameter is omitted, then a default pixel shader is used and the <paramref name="pixelShaderConstants"/> parameter is ignored.
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso cref="GorgonTexture"/>
        /// <seealso cref="GorgonRenderTargetView"/>
        /// <seealso cref="RenderTargets"/>
        public void DrawTexture(GorgonTextureView texture,
                                DX.Rectangle destRect,
                                DX.Point? sourceOffset = null,
                                GorgonColor? color = null,
                                bool clip = false,
                                GorgonBlendState blendState = null,
                                GorgonSamplerState samplerState = null,
                                GorgonPixelShader pixelShader = null,
                                GorgonConstantBuffers pixelShaderConstants = null)
        {
            TextureBlitter blitter = _textureBlitter.Value;

            blitter.Blit(texture,
                         destRect,
                         sourceOffset ?? DX.Point.Zero,
                         color ?? GorgonColor.White,
                         clip,
                         blendState,
                         samplerState,
                         pixelShader,
                         pixelShaderConstants);
        }

        /// <summary>
        /// Function to retrieve cached states, segregated by group names.
        /// </summary>
        /// <param name="groupName">The name of the grouping.</param>
        /// <returns>A <see cref="IGorgonPipelineStateGroup"/> that will contain the cached <see cref="GorgonPipelineState"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="groupName"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="groupName"/> parameter is empty.</exception>
        /// <remarks>
        /// <para>
        /// While the <see cref="GetPipelineState"/> method is much quicker than creating a pipeline state over and over, it still has a fair bit of overhead when calculating which cached 
        /// <see cref="GorgonPipelineState"/> to bring back (or whether to create a new one). 
        /// </para>
        /// <para>
        /// To counter this, applications may use this functionality to create groups of <see cref="GorgonPipelineState"/> objects so they will not need to create them over and over (and thus impair 
        /// performance) or hit the primary <see cref="CachedPipelineStates"/> list via the <see cref="GetPipelineState"/> method.  
        /// </para>
        /// <para>
        /// Note that when the <see cref="ClearStateCache"/> method is called, the cache groupings will be preserved, but any pipeline states contained within it will be cleared and must be recreated by 
        /// the application if they're needed again.
        /// </para>
        /// </remarks>
        /// <seealso cref="IGorgonPipelineStateGroup"/>
        /// <seealso cref="GorgonPipelineState"/>
        public IGorgonPipelineStateGroup GetPipelineStateGroup(string groupName)
        {
            IGorgonPipelineStateGroup cacheGroup;

            if (!_groupedCache.TryGetValue(groupName, out IGorgonPipelineStateGroup groupObject))
            {
                _groupedCache[groupName] = cacheGroup = new PipelineStateGroup(groupName);
            }
            else
            {
                cacheGroup = groupObject;
            }

            return cacheGroup;
        }

        /// <summary>
        /// Function to retrieve a pipeline state.
        /// </summary>
        /// <param name="info">Information used to define the pipeline state.</param>
        /// <returns>A new <see cref="GorgonPipelineState"/>, or an existing one if one was already created.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="info"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="info"/> has no <see cref="IGorgonPipelineStateInfo.VertexShader"/>.</exception>
        /// <exception cref="GorgonException">Thrown if a pipeline state requires a higher feature set than supported by the <see cref="VideoAdapter"/>.</exception>
        /// <remarks>
        /// <para>
        /// This method will create a new pipeline state, or retrieve an existing state if a cached state already exists that exactly matches the information passed to the <paramref name="info"/> parameter. 
        /// </para>
        /// <para>
        /// When a new pipeline state is created, sub-states like blending, depth/stencil, etc... may be reused from previously cached pipeline states and other uncached sub-states will be created anew. This 
        /// new pipeline state is then cached for reuse later in order to speed up the process of creating a series of states.
        /// </para>
        /// </remarks>
        /// <seealso cref="GorgonPipelineState"/>
        /// <seealso cref="IGorgonPipelineStateInfo"/>
        /// <seealso cref="IGorgonVideoAdapterInfo"/>
        public GorgonPipelineState GetPipelineState(IGorgonPipelineStateInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            (GorgonPipelineState State, bool IsNew) result;

            // Threads have to wait their turn.
            lock (_stateCacheLock)
            {
                result = SetupPipelineState(info);

                if (result.IsNew)
                {
                    _stateCache.Add(result.State);
                }
            }

            return result.State;
        }

        /// <summary>
        /// Function to clear the states for the graphics object.
        /// </summary>
        /// <param name="flush">[Optional] <b>true</b> to flush the queued graphics object commands, <b>false</b> to leave as is.</param>
        /// <remarks>
        /// <para>
        /// This method will reset all current states to an uninitialized state.
        /// </para>
        /// <para>
        /// If the <paramref name="flush"/> parameter is set to <b>true</b>, then any commands on the GPU that are pending will be flushed.
        /// </para>
        /// <para>
        /// <note type="warning">
        /// <para>
        /// This method will cause a significant performance hit if the <paramref name="flush"/> parameter is set to <b>true</b>, so its use is generally discouraged in performance sensitive situations.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public void ClearState(bool flush = false)
        {
            // Reset state on the device context.
            D3DDeviceContext.ClearState();

            if (flush)
            {
                D3DDeviceContext.Flush();
            }

            _renderTargets.Clear();
            ScissorRectangles.Clear();
            Viewports.Clear();
            _currentDrawCall?.Reset();
        }

        /// <summary>
        /// Function to retrieve information about the installed video adapters on the system.
        /// </summary>
        /// <param name="includeSoftwareDevice">[Optional] <b>true</b> to retrieve a software rendering device, or <b>false</b> to exclude it.</param>
        /// <param name="log">[Optional] The logging interface used to capture debug messages.</param>
        /// <returns>A list of installed adapters on the system.</returns>
        /// <remarks>
        /// <para>
        /// Use this to retrieve a list of video adapters available on the system. A video adapter may be a discreet video card, a device on the motherboard, or a software video adapter.
        /// </para>
        /// <para>
        /// This resulting list will contain <see cref="VideoAdapterInfo"/> objects which can then be passed to a <see cref="GorgonGraphics"/> instance. This allows applications or users to pick and choose which 
        /// adapter they wish to use for rendering.
        /// </para>
        /// <para>
        /// If the user specifies <b>true</b> for the <paramref name="includeSoftwareDevice"/> parameter, then the video adapter supplied will be much slower than an actual hardware video adapter. However, 
        /// this adapter can be helpful in debugging scenarios where issues with the hardware device driver may be causing incorrect rendering.
        /// </para>
        /// </remarks>
        public static IReadOnlyList<IGorgonVideoAdapterInfo> EnumerateAdapters(bool includeSoftwareDevice = false, IGorgonLog log = null) => VideoAdapterEnumerator.Enumerate(includeSoftwareDevice, log);

        /// <summary>
        /// Function to find a display mode supported by the Gorgon.
        /// </summary>
        /// <param name="output">The output to use when looking for a video mode.</param>
        /// <param name="videoMode">The <see cref="GorgonVideoMode"/> used to find the closest match.</param>
        /// <param name="suggestedMode">A <see cref="GorgonVideoMode"/> that is the nearest match for the provided video mode.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="output"/> parameter is <b>null</b>.</exception>
        /// <remarks>
        /// <para>
        /// Users may leave the <see cref="GorgonVideoMode"/> values at unspecified (either 0, or default enumeration values) to indicate that these values should not be used in the search.
        /// </para>
        /// <para>
        /// The following members in <see cref="GorgonVideoMode"/> may be skipped (if not listed, then this member must be specified):
        /// <list type="bullet">
        ///		<item>
        ///			<description><see cref="GorgonVideoMode.Width"/> and <see cref="GorgonVideoMode.Height"/>.  Both values must be set to 0 if not filtering by width or height.</description>
        ///		</item>
        ///		<item>
        ///			<description><see cref="GorgonVideoMode.RefreshRate"/> should be set to empty in order to skip filtering by refresh rate.</description>
        ///		</item>
        ///		<item>
        ///			<description><see cref="GorgonVideoMode.Scaling"/> should be set to <see cref="ModeScaling.Unspecified"/> in order to skip filtering by the scaling mode.</description>
        ///		</item>
        ///		<item>
        ///			<description><see cref="GorgonVideoMode.ScanlineOrder"/> should be set to <see cref="ModeScanlineOrder.Unspecified"/> in order to skip filtering by the scanline order.</description>
        ///		</item>
        /// </list>
        /// </para>
        /// <para>
        /// <note type="important">
        /// <para>
        /// The <see cref="GorgonVideoMode.Format"/> member must be one of the UNorm format types and cannot be set to <see cref="BufferFormat.Unknown"/>.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public void FindNearestVideoMode(IGorgonVideoOutputInfo output, ref GorgonVideoMode videoMode, out GorgonVideoMode suggestedMode)
        {
            suggestedMode = videoMode;
            
            using (DXGI.Output giOutput = _dxgiAdapter.GetOutput(output.Index))
            {
                using (DXGI.Output1 giOutput1 = giOutput.QueryInterface<DXGI.Output1>())
                {
                    DXGI.ModeDescription1 matchMode = videoMode.ToModeDesc1();

                    giOutput1.FindClosestMatchingMode1(ref matchMode, out DXGI.ModeDescription1 mode, D3DDevice);

                    suggestedMode =  mode.ToGorgonVideoMode();
                }
            }
        }

        /// <summary>
        /// Function to validate whether a buffer can have its data read or not.
        /// </summary>
        /// <param name="allowRead"><b>true</b> if the buffer allows reading, or <b>false</b> if not.</param>
        /// <param name="sourceIndex">The index within the buffer data to start reading from.</param>
        /// <param name="sourceCount">The number of items to read from the buffer.</param>
        /// <param name="resourceSize">The total size of the buffer.</param>
        private void ValidateGetData(bool allowRead, int sourceIndex, int sourceCount, int resourceSize)
        {
            if (!allowRead)
            {
                throw new GorgonException(GorgonResult.CannotRead, Resources.GORGFX_ERR_BUFFER_NOT_READABLE);
            }

            if (sourceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            }

            if (sourceIndex + sourceCount > resourceSize)
            {
                throw new ArgumentException(string.Format(Resources.GORGFX_ERR_DATA_OFFSET_COUNT_IS_TOO_LARGE, sourceIndex, sourceCount));
            }
        }

        public T[] GetData<T>(GorgonBufferBase sourceBuffer, int bufferByteOffset = 0, int? itemCount = null)
            where T : struct
        {
            sourceBuffer.ValidateObject(nameof(sourceBuffer));
            int typeSize = DirectAccess.SizeOf<T>();

            if (itemCount == null)
            {
                itemCount = (sourceBuffer.SizeInBytes / typeSize) - (bufferByteOffset / typeSize);
            }

            int sourceCount = itemCount.Value * typeSize;

            GorgonBufferBase stageBuffer;
            MapPointerData mapData = null;

            // We have requested a read, and we are allowed to directly read the buffer.
            if ((sourceBuffer.RequestedCpuReadable) && (sourceBuffer.IsCpuReadable))
            {
                stageBuffer = sourceBuffer;
            }
            else
            {
                stageBuffer = sourceBuffer.GetStaging();
            }

#if DEBUG
            ValidateGetData(sourceBuffer.RequestedCpuReadable, bufferByteOffset, sourceCount, stageBuffer.SizeInBytes);
#endif

            try
            {
                mapData = new MapPointerData(this, stageBuffer.D3DResource, D3D11.MapMode.Read, 0, bufferByteOffset, sourceCount);
                mapData.Lock();
                var result = new T[itemCount.Value];
                mapData.ReadRange(result, 0, result.Length);

                return result;
            }
            finally
            {
                mapData?.Dispose();

                if (stageBuffer != sourceBuffer)
                {
                    stageBuffer.Dispose();
                }
            }
        }

        public void GetDataRange<T>(GorgonBufferBase sourceBuffer, T[] destArray, int bufferByteOffset = 0, int? itemCount = null, int destIndex = 0)
            where T : struct
        {
            sourceBuffer.ValidateObject(nameof(sourceBuffer));
            sourceBuffer.ValidateObject(nameof(destArray));

            int typeSize = DirectAccess.SizeOf<T>();

            if (itemCount == null)
            {
                itemCount = (sourceBuffer.SizeInBytes / typeSize) - (bufferByteOffset / typeSize);
            }
            
            int sourceCount = itemCount.Value * typeSize;

            GorgonBufferBase stageBuffer;
            MapPointerData mapData = null;

            // We have requested a read, and we are allowed to directly read the buffer.
            if ((sourceBuffer.RequestedCpuReadable) && (sourceBuffer.IsCpuReadable))
            {
                stageBuffer = sourceBuffer;
            }
            else
            {
                stageBuffer = sourceBuffer.GetStaging();
            }

#if DEBUG
            if (destIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(destIndex));
            }

            if (destIndex + itemCount.Value > destArray.Length)
            {
                throw new ArgumentException(string.Format(Resources.GORGFX_ERR_DATA_OFFSET_COUNT_IS_TOO_LARGE, destIndex, sourceCount));
            }

            ValidateGetData(stageBuffer.RequestedCpuReadable, bufferByteOffset, sourceCount, stageBuffer.SizeInBytes);
#endif

            try
            {
                mapData = new MapPointerData(this, stageBuffer.D3DResource, D3D11.MapMode.Read, 0, bufferByteOffset, sourceCount);
                mapData.Lock();
                mapData.ReadRange(destArray, destIndex, itemCount.Value);
            }
            finally
            {
                mapData?.Dispose();

                if (stageBuffer != sourceBuffer)
                {
                    stageBuffer.Dispose();
                }
            }
        }

        /// <summary>
        /// Function to read a single value from a <see cref="GorgonBufferBase"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to return. Must be a primitive or value type.</typeparam>
        /// <param name="sourceBuffer">The buffer containing the data to read.</param>
        /// <param name="result">The value returned from the buffer.</param>
        /// <param name="bufferByteOffset">[Optional] The starting offset from the beginning of the buffer, in bytes, to start reading from.</param>
        public void GetValue<T>(GorgonBufferBase sourceBuffer, out T result, int bufferByteOffset = 0)
            where T : struct
        {
            sourceBuffer.ValidateObject(nameof(sourceBuffer));
            int typeSize = DirectAccess.SizeOf<T>();

            int sourceCount = typeSize;

            GorgonBufferBase stageBuffer;
            MapPointerData mapData = null;

            // We have requested a read, and we are allowed to directly read the buffer.
            if ((sourceBuffer.RequestedCpuReadable) && (sourceBuffer.IsCpuReadable))
            {
                stageBuffer = sourceBuffer;
            }
            else
            {
                stageBuffer = sourceBuffer.GetStaging();
            }

#if DEBUG
            ValidateGetData(sourceBuffer.RequestedCpuReadable, bufferByteOffset, sourceCount, stageBuffer.SizeInBytes);
#endif

            try
            {
                mapData = new MapPointerData(this, stageBuffer.D3DResource, D3D11.MapMode.Read, 0, bufferByteOffset, sourceCount);
                mapData.Lock();
                mapData.Read(out result);
            }
            finally
            {
                mapData?.Dispose();

                if (stageBuffer != sourceBuffer)
                {
                    stageBuffer.Dispose();
                }
            }
        }

        /// <summary>
        /// Function to upload data pointed to by a <see cref="IGorgonPointer"/> to an object that descends from a <see cref="GorgonBufferBase"/> type.
        /// </summary>
        /// <param name="sourceData">A pointer to memory that will be uploaded into the buffer.</param>
        /// <param name="buffer">A buffer that will receive the data.</param>
        /// <param name="sourceSize">[Optional] The number of bytes to read from <paramref name="sourceData"/>.</param>
        /// <param name="destOffset">[Optional] The offset, in bytes, in the <paramref name="buffer"/> to start writing into.</param>
        /// <param name="copyMode">[Optional] The type of locking mode to employ when copying data into the buffer.</param>
        public void SetDataRange(IGorgonPointer sourceData, GorgonBufferBase buffer, int? sourceSize = null, int destOffset = 0, CopyMode copyMode = CopyMode.None)
        {
            sourceData.ValidateObject(nameof(sourceData));
            buffer.ValidateObject(nameof(buffer));

            if (sourceSize == null)
            {
                sourceSize = (int)sourceData.Size;
            }

            int destSize = sourceSize.Value;

#if DEBUG
            ValidateSetData(buffer.Usage, (int)sourceData.Size, buffer.SizeInBytes, 0, sourceSize.Value, destOffset, destSize);
#endif

            if ((buffer.Usage == ResourceUsage.Dynamic) || (buffer.Usage == ResourceUsage.Staging))
            {
                MapBuffer(new IntPtr(sourceData.Address),
                          buffer.D3DResource,
                          0,
                          sourceSize.Value,
                          destOffset,
                          buffer.Usage == ResourceUsage.Staging,
                          copyMode);
                return;
            }

            D3DDeviceContext.UpdateSubresource1(buffer.D3DResource,
                                                0,
                                                new D3D11.ResourceRegion
                                                {
                                                    Left = destOffset,
                                                    Right = destOffset + destSize,
                                                    Top = 0,
                                                    Front = 0,
                                                    Back = 1,
                                                    Bottom = 1
                                                },
                                                new IntPtr(sourceData.Address),
                                                sourceSize.Value,
                                                0,
                                                (int)copyMode);
        }

        /// <summary>
        /// Function to upload data to an object that descends from a <see cref="GorgonBufferBase"/> type.
        /// </summary>
        /// <typeparam name="T">The type of data to upload, must be a primitive or value type.</typeparam>
        /// <param name="sourceData">An array of type <typeparamref name="T"/> to upload into the buffer.</param>
        /// <param name="destBuffer">A buffer that will receive the data.</param>
        /// <param name="sourceIndex">[Optional] The index to start reading from in the <paramref name="sourceData"/> array.</param>
        /// <param name="sourceCount">[Optional] The number of elements to read from the <paramref name="sourceData"/> array.</param>
        /// <param name="destOffset">[Optional] The offset, in bytes, in the <paramref name="destBuffer"/> to start writing into.</param>
        /// <param name="copyMode">[Optional] The type of locking mode to employ when copying data into the buffer.</param>
        public void SetDataRange<T>(T[] sourceData, GorgonBufferBase destBuffer, int sourceIndex = 0, int? sourceCount = null, int destOffset = 0, CopyMode copyMode = CopyMode.None)
            where T : struct
        {
            sourceData.ValidateObject(nameof(sourceData));
            destBuffer.ValidateObject(nameof(destBuffer));

            int elementSize = DirectAccess.SizeOf<T>();
            
            if (sourceCount == null)
            {
                sourceCount = sourceData.Length - sourceIndex;
            }

            int destSize = elementSize * sourceCount.Value;

#if DEBUG
            ValidateSetData(destBuffer.Usage, sourceData.Length, destBuffer.SizeInBytes, sourceIndex, sourceCount.Value, destOffset, destSize);
#endif

            using (var ptr = new GorgonPointerPinned<T>(sourceData, sourceIndex, sourceCount.Value))
            {
                if ((destBuffer.Usage == ResourceUsage.Dynamic) || (destBuffer.Usage == ResourceUsage.Staging))
                {
                    MapBuffer(new IntPtr(ptr.Address + sourceIndex * elementSize),
                              destBuffer.D3DResource,
                              0,
                              sourceCount.Value * elementSize,
                              destOffset,
                              destBuffer.Usage == ResourceUsage.Staging,
                              copyMode);
                    return;
                }

                D3DDeviceContext.UpdateSubresource1(destBuffer.D3DResource,
                                                    0,
                                                    new D3D11.ResourceRegion
                                                    {
                                                        Left = destOffset,
                                                        Right = destOffset + destSize,
                                                        Top = 0,
                                                        Front = 0,
                                                        Back = 1,
                                                        Bottom = 1
                                                    },
                                                    new IntPtr(ptr.Address + sourceIndex * elementSize),
                                                    sourceCount.Value * elementSize,
                                                    0,
                                                    (int)copyMode);
            }
        }

        /// <summary>
        /// Function to upload data to an object that descends from a <see cref="GorgonBufferBase"/> type.
        /// </summary>
        /// <typeparam name="T">The type of data to upload, must be a primitive or value type.</typeparam>
        /// <param name="sourceData">An array of type <typeparamref name="T"/> to upload into the buffer.</param>
        /// <param name="destBuffer">A buffer that will receive the data.</param>
        /// <param name="destOffset">[Optional] The offset, in bytes, in the <paramref name="destBuffer"/> to start writing into.</param>
        /// <param name="copyMode">[Optional] The type of locking mode to employ when copying data into the buffer.</param>
        public void SetValue<T>(ref T sourceData, GorgonBufferBase destBuffer, int destOffset = 0, CopyMode copyMode = CopyMode.None)
            where T : struct
        {
            destBuffer.ValidateObject(nameof(destBuffer));

            int elementSize = DirectAccess.SizeOf<T>();

#if DEBUG
            ValidateSetData(destBuffer.Usage, elementSize, destBuffer.SizeInBytes, 0, elementSize, destOffset, elementSize);
#endif

            using (var ptr = new GorgonPointerPinned<T>(sourceData))
            {
                if ((destBuffer.Usage == ResourceUsage.Dynamic) || (destBuffer.Usage == ResourceUsage.Staging))
                {
                    MapBuffer(new IntPtr(ptr.Address),
                              destBuffer.D3DResource,
                              0,
                              elementSize,
                              destOffset,
                              destBuffer.Usage == ResourceUsage.Staging,
                              copyMode);
                    return;
                }

                D3DDeviceContext.UpdateSubresource1(destBuffer.D3DResource,
                                                    0,
                                                    new D3D11.ResourceRegion
                                                    {
                                                        Left = destOffset,
                                                        Right = destOffset + elementSize,
                                                        Top = 0,
                                                        Front = 0,
                                                        Back = 1,
                                                        Bottom = 1
                                                    },
                                                    new IntPtr(ptr.Address),
                                                    elementSize,
                                                    0,
                                                    (int)copyMode);
            }
        }


        /// <summary>
        /// Function to copy the contents of a buffer into another buffer.
        /// </summary>
        /// <param name="sourceBuffer">The source buffer to read from.</param>
        /// <param name="destBuffer">The destination buffer that will receive the data.</param>
        /// <param name="sourceOffset">[Optional] Starting byte index to start copying from.</param>
        /// <param name="byteCount">[Optional] The number of bytes to copy.</param>
        /// <param name="destOffset">[Optional] The offset within the destination buffer.</param>
        /// <param name="copyMode">[Optional] Defines how data should be copied into the buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="sourceOffset"/> plus the <paramref name="byteCount"/> is less than 0 or larger than the size of this buffer.
        /// <para>-or-</para>
        /// <para>Thrown when the <paramref name="destOffset"/> plus the <paramref name="byteCount"/> is less than 0 or larger than the size of the destination <paramref name="destBuffer"/>.</para>
        /// </exception>
        /// <exception cref="GorgonException">Thrown when <paramref name="destBuffer"/> has a resource usage of <see cref="ResourceUsage.Immutable"/>.</exception>
        /// <remarks>
        /// <para>
        /// Use this method to copy the contents of this buffer into another.
        /// </para> 
        /// <para>
        /// The <paramref name="copyMode"/> flag defines how data will be copied into this buffer.  See the <see cref="CopyMode"/> enumeration for a description of the values.
        /// </para>
        /// <para>
        /// The source and destination buffer offsets must fit within their range of their allocated space, as must the <paramref name="byteCount"/>. Otherwise, an exception will be thrown. Also, the 
        /// destination buffer must not be <see cref="ResourceUsage.Immutable"/>.
        /// </para>
        /// <para>
        /// <note type="warning">
        /// <para>
        /// For performance reasons, this method will only throw exceptions when Gorgon is compiled as <b>DEBUG</b>.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        public void Copy(GorgonBufferBase sourceBuffer, GorgonBufferBase destBuffer, int sourceOffset = 0, int byteCount = 0, int destOffset = 0, CopyMode copyMode = CopyMode.None)
        {
            destBuffer.ValidateObject(nameof(destBuffer));

            sourceOffset = sourceOffset.Max(0);
            destOffset = destOffset.Max(0);

            if (byteCount < 1)
            {
                byteCount = sourceBuffer.SizeInBytes.Min(destBuffer.SizeInBytes);
            }

            int sourceByteIndex = sourceOffset + byteCount;
            int destByteIndex = destOffset + byteCount;

            sourceOffset.ValidateRange(nameof(sourceOffset), 0, sourceBuffer.SizeInBytes);
            destOffset.ValidateRange(nameof(destOffset), 0, destBuffer.SizeInBytes);
            sourceByteIndex.ValidateRange(nameof(byteCount), 0, sourceBuffer.SizeInBytes, maxInclusive: true);
            destByteIndex.ValidateRange(nameof(byteCount), 0, destBuffer.SizeInBytes, maxInclusive: true);

#if DEBUG
            if (destBuffer.NativeBuffer.Description.Usage == D3D11.ResourceUsage.Immutable)
            {
                throw new GorgonException(GorgonResult.AccessDenied, Resources.GORGFX_ERR_BUFFER_IS_IMMUTABLE);
            }
#endif
            D3DDeviceContext.CopySubresourceRegion1(destBuffer.D3DResource,
                                                             0,
                                                             destOffset,
                                                             0,
                                                             0,
                                                             sourceBuffer.D3DResource,
                                                             0,
                                                             new D3D11.ResourceRegion(sourceOffset, 0, 0, sourceByteIndex, 1, 1),
                                                             (int)copyMode);
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            D3D11.DeviceContext4 context = Interlocked.Exchange(ref _d3DDeviceContext, null);
            D3D11.Device5 device = Interlocked.Exchange(ref _d3DDevice, null);
            DXGI.Adapter4 adapter = Interlocked.Exchange(ref _dxgiAdapter, null);
            DXGI.Factory5 factory = Interlocked.Exchange(ref _dxgiFactory, null);

            if (context == null)
            {
                return;
            }

            // If we ever created a blitter on this interface, then we need to clean up the common data for all blitter instances.
            if (_textureBlitter.IsValueCreated)
            {
                _textureBlitter.Value.Dispose();
            }

            ClearStateCache();

            // Disconnect from the context.
            _log.Print($"Destroying GorgonGraphics interface for device '{_videoAdapter.Name}'...", LoggingLevel.Simple);

            // Reset the state for the context. This will ensure we don't have anything bound to the pipeline when we shut down.
            context.ClearState();
            context.Dispose();
            device.Dispose();
            adapter.Dispose();
            factory.Dispose();
        }
        #endregion

        #region Constructor/Destructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonGraphics"/> class.
        /// </summary>
        /// <param name="videoAdapterInfo">A <see cref="VideoAdapterInfo"/> to specify the video adapter to use for this instance.</param>
        /// <param name="featureSet">[Optional] The requested feature set for the video adapter used with this object.</param>
        /// <param name="log">[Optional] The log to use for debugging.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="videoAdapterInfo"/> parameter is <b>null</b>.</exception>
        /// <exception cref="GorgonException">Thrown when the <paramref name="featureSet"/> is unsupported.</exception>
        /// <remarks>
        /// <para>
        /// When the <paramref name="videoAdapterInfo"/> is set to <b>null</b>, Gorgon will use the first video adapter with feature set specified by <paramref name="featureSet"/>  
        /// will be used. If the feature set requested is higher than what any device in the system can support, then the first device with the highest feature set will be used.
        /// </para>
        /// <para>
        /// When specifying a feature set, the device with the closest matching feature set will be used. If the <paramref name="videoAdapterInfo"/> is specified, then that device will be used at the 
        /// requested <paramref name="featureSet"/>. If the requested <paramref name="featureSet"/> is higher than what the <paramref name="videoAdapterInfo"/> will support, then Gorgon will use the 
        /// highest feature of the specified <paramref name="videoAdapterInfo"/>. 
        /// </para>
        /// <para>
        /// If Gorgon is compiled in DEBUG mode, and <see cref="VideoAdapterInfo"/> is <b>null</b>, then it will attempt to find the most appropriate hardware video adapter, and failing that, will fall 
        /// back to a software device.
        /// </para>
        /// <para>
        /// <note type="important">
        /// <para>
        /// The Gorgon Graphics library only works on Windows 10 build 16299 (aka Fall Creators Update) or better. No lesser operating system version is supported.
        /// </para>
        /// </note>
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The following examples show the various ways the object can be configured:
        /// </para>
        /// <code lang="csharp">
        /// <![CDATA[
        /// // Create using the first video adapter with the highest feature set:
        /// var graphics = new GorgonGraphics();
        /// 
        /// // Create using a specific video adapter and use the highest feature set supported by that device:
        /// // Get a list of available video adapters.
        /// IReadOnlyList<IGorgonVideoAdapterInfo> videoAdapters = GorgonGraphics.EnumerateAdapters(false, log);
        ///
        /// // In real code, you should always check for more than 0 devices in the resulting list.
        /// var graphics = new GorgonGraphics(videoAdapters[0]);
        /// 
        /// // Create using the requested feature set and the first adapter that supports the nearest feature set requested:
        /// // If the device does not support 12.1, then the device with the nearest feature set (e.g. 12.0) will be used instead.
        /// var graphics = new GorgonGraphics(videoAdapters[0], FeatureSet.Level_12_1);
        /// 
        /// // Create using the requested device and the requested feature set:
        /// // If the device does not support 12.0, then the highest feature set supported by the device will be used (e.g. 10.1).
        /// IReadOnlyList<IGorgonVideoAdapterInfo> videoAdapters = GorgonGraphics.EnumerateAdapters(false, log);
        ///
        /// var graphics = new GorgonGraphics(videoAdapters[0], FeatureLevel.Level_12_0); 
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="IGorgonVideoAdapterInfo"/>
        public GorgonGraphics(IGorgonVideoAdapterInfo videoAdapterInfo, FeatureSet? featureSet = null, IGorgonLog log = null)
        {
            if (videoAdapterInfo == null)
            {
                throw new ArgumentNullException(nameof(videoAdapterInfo));
            }

            if (!Win32API.IsWindows10OrGreater(16299))
            {
                throw new GorgonException(GorgonResult.CannotCreate, Resources.GORGFX_ERR_INVALID_OS);
            }

            // If we've not specified a feature set, or the feature set exceeds the requested device feature set, then 
            // fall back to the device feature set.
            if ((featureSet == null) || (videoAdapterInfo.SupportedFeatureLevel < featureSet.Value))
            {
                featureSet = videoAdapterInfo.SupportedFeatureLevel;
            }

            // We only support feature set 12 and greater.
            if (!Enum.IsDefined(typeof(FeatureSet), featureSet.Value))
            {
                throw new GorgonException(GorgonResult.CannotCreate, string.Format(Resources.GORGFX_ERR_FEATURE_LEVEL_INVALID, featureSet));
            }

            _log = log ?? GorgonLogDummy.DefaultInstance;

            _log.Print("Gorgon Graphics initializing...", LoggingLevel.Simple);
            _log.Print($"Using video adapter '{videoAdapterInfo.Name}' at feature set [{featureSet.Value}] for Direct 3D 11.4.", LoggingLevel.Simple);
            
            InitializeVideoAdapter(videoAdapterInfo, featureSet.Value);

            ScissorRectangles = new GorgonMonitoredValueTypeArray<DX.Rectangle>(videoAdapterInfo.MaxScissorCount);
            Viewports = new GorgonMonitoredValueTypeArray<DX.ViewportF>(videoAdapterInfo.MaxViewportCount);

            
            _renderTargets = new GorgonRenderTargetViews();

            // Assign common sampler states to the factory cache.
            SamplerStateFactory.GetSamplerState(this, GorgonSamplerState.Default, _log);
            SamplerStateFactory.GetSamplerState(this, GorgonSamplerState.AnisotropicFiltering, _log);
            SamplerStateFactory.GetSamplerState(this, GorgonSamplerState.PointFiltering, _log);

            // Register texture blitter shader code to the shader factory so it can be used to include the blitter.
            GorgonShaderFactory.Includes[BlitterShaderIncludeFileName] = new GorgonShaderInclude(BlitterShaderIncludeFileName, Resources.GraphicsShaders);

            _textureBlitter = new Lazy<TextureBlitter>(() => new TextureBlitter(this), LazyThreadSafetyMode.ExecutionAndPublication);

            _log.Print("Gorgon Graphics initialized.", LoggingLevel.Simple);
        }

        /// <summary>
        /// Initializes the <see cref="GorgonGraphics"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Gorgon.Native.Win32API.DwmIsCompositionEnabled(System.Boolean@)")]
        static GorgonGraphics()
        {
            DX.Configuration.ThrowOnShaderCompileError = false;

#if DEBUG
            IsDebugEnabled = true;
#endif
        }
        #endregion
    }
}
