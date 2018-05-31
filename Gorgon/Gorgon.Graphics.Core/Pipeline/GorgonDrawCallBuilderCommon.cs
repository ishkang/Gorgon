﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2018 Michael Winsor
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
// Created: May 23, 2018 12:18:45 PM
// 
#endregion

using System;
using System.Collections.Generic;
using Gorgon.Graphics.Core.Properties;

namespace Gorgon.Graphics.Core
{
    /// <summary>
    /// Common functionality for the a draw call fluent builder.
    /// </summary>
    /// <typeparam name="TB">The type of builder.</typeparam>
    /// <typeparam name="TDc">The type of draw call.</typeparam>
    public abstract class GorgonDrawCallBuilderCommon<TB, TDc>
        where TB : GorgonDrawCallBuilderCommon<TB, TDc>
        where TDc : GorgonDrawCallCommon
    {
        #region Properties.
        /// <summary>
        /// Property to return the draw call being edited.
        /// </summary>
        protected TDc DrawCall
        {
            get;
        }
        #endregion

        #region Methods.
        /// <summary>
        /// Function to create a new draw call.
        /// </summary>
        /// <returns>A new draw call.</returns>
        protected abstract TDc OnCreate();

        /// <summary>
        /// Function to update the properties of the draw call from the working copy to the final copy.
        /// </summary>
        /// <param name="finalCopy">The object representing the finalized copy.</param>
        protected abstract void OnUpdate(TDc finalCopy);

        /// <summary>
        /// Function to reset the properties of the draw call to the draw call passed in.
        /// </summary>
        /// <param name="drawCall">The draw call to copy from.</param>
        /// <returns>The fluent builder interface.</returns>
        protected abstract TB OnResetTo(TDc drawCall);

        /// <summary>
        /// Function to clear the draw call.
        /// </summary>
        /// <returns>The fluent builder interface.</returns>
        protected abstract TB OnClear();

        /// <summary>
        /// Function to assign a list of samplers to a shader on the pipeline.
        /// </summary>
        /// <param name="shaderType">The type of shader to update.</param>
        /// <param name="samplers">The samplers to assign.</param>
        /// <returns>The fluent interface for this builder.</returns>
        public TB SamplerStates(ShaderType shaderType, IReadOnlyList<GorgonSamplerState> samplers)
        {
            switch (shaderType)
            {
                case ShaderType.Pixel:
                    StateCopy.CopySamplers(DrawCall.D3DState.PsSamplers, samplers);
                    break;
                case ShaderType.Vertex:
                    StateCopy.CopySamplers(DrawCall.D3DState.VsSamplers, samplers);
                    break;
                case ShaderType.Geometry:
                    StateCopy.CopySamplers(DrawCall.D3DState.GsSamplers, samplers);
                    break;
                case ShaderType.Domain:
                    StateCopy.CopySamplers(DrawCall.D3DState.DsSamplers, samplers);
                    break;
                case ShaderType.Hull:
                    StateCopy.CopySamplers(DrawCall.D3DState.VsSamplers, samplers);
                    break;
                case ShaderType.Compute:
                    StateCopy.CopySamplers(DrawCall.D3DState.CsSamplers, samplers);
                    break;
            }
            
            return (TB)this;
        }

        /// <summary>
        /// Function to assign a sampler to a shader on the pipeline.
        /// </summary>
        /// <param name="shaderType">The type of shader to update.</param>
        /// <param name="sampler">The sampler to assign.</param>
        /// <param name="index">[Optional] The index of the sampler.</param>
        /// <returns>The fluent interface for this builder.</returns>
        public TB SamplerState(ShaderType shaderType, GorgonSamplerStateBuilder sampler, int index = 0)
        {
            return SamplerState(shaderType, sampler.Build(), index);
        }

        /// <summary>
        /// Function to assign a sampler to a shader on the pipeline.
        /// </summary>
        /// <param name="shaderType">The type of shader to update.</param>
        /// <param name="sampler">The sampler to assign.</param>
        /// <param name="index">[Optional] The index of the sampler.</param>
        /// <returns>The fluent interface for this builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="index"/> parameter is less than 0, or greater than/equal to <see cref="GorgonSamplerStates.MaximumSamplerStateCount"/>.</exception>
        public TB SamplerState(ShaderType shaderType, GorgonSamplerState sampler, int index = 0)
        {
            if ((index < 0) || (index >= GorgonSamplerStates.MaximumSamplerStateCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(Resources.GORGFX_ERR_INVALID_SAMPLER_INDEX, GorgonSamplerStates.MaximumSamplerStateCount));
            }

            switch (shaderType)
            {
                case ShaderType.Pixel:
                    DrawCall.D3DState.PsSamplers[index] = sampler;
                    break;
                case ShaderType.Vertex:
                    DrawCall.D3DState.VsSamplers[index] = sampler;
                    break;
                case ShaderType.Geometry:
                    DrawCall.D3DState.GsSamplers[index] = sampler;
                    break;
                case ShaderType.Domain:
                    DrawCall.D3DState.DsSamplers[index] = sampler;
                    break;
                case ShaderType.Hull:
                    DrawCall.D3DState.HsSamplers[index] = sampler;
                    break;
                case ShaderType.Compute:
                    DrawCall.D3DState.CsSamplers[index] = sampler;
                    break;
            }

            return (TB)this;
        }

        /// <summary>
        /// Function to set the pipeline state for this draw call.
        /// </summary>
        /// <param name="pipelineState">The pipeline state to assign.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="pipelineState"/> parameter is <b>null</b>.</exception>
        public TB PipelineState(GorgonPipelineState pipelineState)
        {
            DrawCall.D3DState.PipelineState = new GorgonPipelineState(pipelineState ?? throw new ArgumentNullException(nameof(pipelineState)));
            return (TB)this;
        }

        /// <summary>
        /// Function to set the pipeline state for this draw call.
        /// </summary>
        /// <param name="pipelineState">The pipeline state to assign.</param>
        /// <returns>The fluent builder interface.</returns>
        public TB PipelineState(GorgonPipelineStateBuilder pipelineState)
        {
            return PipelineState(pipelineState?.Build());
        }

        /// <summary>
        /// Function to set a stream out binding for the draw call.
        /// </summary>
        /// <param name="binding">The stream out binding to use.</param>
        /// <param name="slot">[Optional] The slot for the binding.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="slot"/> parameter is less than 0, or greater than/equal to <see cref="GorgonStreamOutBindings.MaximumStreamOutCount"/>.</exception>
        public TB StreamOutBuffer(in GorgonStreamOutBinding binding, int slot = 0)
        {
            if ((slot < 0) || (slot >= GorgonStreamOutBindings.MaximumStreamOutCount))
            {
                throw new ArgumentOutOfRangeException(nameof(slot), string.Format(Resources.GORGFX_ERR_SO_SLOT_INVALID, GorgonStreamOutBindings.MaximumStreamOutCount));
            }

            if (DrawCall.D3DState.StreamOutBindings == null)
            {
                DrawCall.D3DState.StreamOutBindings = new GorgonStreamOutBindings();
            }

            DrawCall.D3DState.StreamOutBindings[slot] = binding;
            return (TB)this;
        }

        /// <summary>
        /// Function to set a list of stream out bindings for the draw call.
        /// </summary>
        /// <param name="bindings">The stream out bindings to use.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="bindings"/> parameter is larger than the <see cref="GorgonStreamOutBindings.MaximumStreamOutCount"/>.</exception>
        public TB StreamOutBuffers(IReadOnlyList<GorgonStreamOutBinding> bindings)
        {
            if (bindings.Count > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(bindings), string.Format(Resources.GORGFX_ERR_SO_SLOT_INVALID, GorgonStreamOutBindings.MaximumStreamOutCount));
            }

            if (DrawCall.D3DState.StreamOutBindings == null)
            {
                DrawCall.D3DState.StreamOutBindings = new GorgonStreamOutBindings();
            }

            DrawCall.D3DState.StreamOutBindings.Clear();
            if (bindings.Count == 0)
            {
                return (TB)this;
            }

            for (int i = 0; i < bindings.Count; ++i)
            {
                DrawCall.D3DState.StreamOutBindings[i] = bindings[i];
            }
            return (TB)this;
        }

        /// <summary>
        /// Function to set a vertex buffer binding for the draw call.
        /// </summary>
        /// <param name="layout">The input layout to use.</param>
        /// <param name="binding">The vertex buffer binding to set.</param>
        /// <param name="slot">[Optional] The slot for the binding.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="layout"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="slot"/> parameter is less than 0, or greater than/equal to <see cref="GorgonVertexBufferBindings.MaximumVertexBufferCount"/>.</exception>
        public TB VertexBuffer(GorgonInputLayout layout, in GorgonVertexBufferBinding binding, int slot = 0)
        {
            if ((slot < 0) || (slot >= GorgonVertexBufferBindings.MaximumVertexBufferCount))
            {
                throw new ArgumentOutOfRangeException(nameof(slot), string.Format(Resources.GORGFX_ERR_INVALID_VERTEXBUFFER_SLOT, GorgonVertexBufferBindings.MaximumVertexBufferCount));
            }

            if (DrawCall.D3DState.VertexBuffers == null)
            {
                DrawCall.D3DState.VertexBuffers = new GorgonVertexBufferBindings();
            }

            DrawCall.D3DState.VertexBuffers[slot] = binding;
            DrawCall.D3DState.VertexBuffers.InputLayout = layout ?? throw new ArgumentNullException(nameof(layout));
            return (TB)this;
        }

        /// <summary>
        /// Function to set the vertex buffer bindings for the draw call.
        /// </summary>
        /// <param name="layout">The input layout to use.</param>
        /// <param name="bindings">The vertex buffer bindings to set.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="layout"/> parameter is <b>null</b>.</exception>
        public TB VertexBufferBindings(GorgonInputLayout layout, IReadOnlyList<GorgonVertexBufferBinding> bindings)
        {
            StateCopy.CopyVertexBuffers(DrawCall.D3DState.VertexBuffers, bindings, layout ?? throw new ArgumentNullException(nameof(layout)));
            return (TB)this;
        }

        /// <summary>
        /// Function to set a constant buffer for a specific shader stage.
        /// </summary>
        /// <param name="shaderType">The shader stage to use.</param>
        /// <param name="constantBuffer">The constant buffer to assign.</param>
        /// <param name="slot">The slot for the constant buffer.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="slot"/> is less than 0, or greater than/equal to <see cref="GorgonConstantBuffers.MaximumConstantBufferCount"/>.</exception>
        public TB ConstantBuffer(ShaderType shaderType, GorgonConstantBufferView constantBuffer, int slot = 0)
        {
            if ((slot < 0) || (slot >= GorgonConstantBuffers.MaximumConstantBufferCount))
            {
                throw new ArgumentOutOfRangeException(nameof(slot), string.Format(Resources.GORGFX_ERR_CBUFFER_SLOT_INVALID, 0));
            }

            switch (shaderType)
            {
                case ShaderType.Pixel:
                    DrawCall.D3DState.PsConstantBuffers[slot] = constantBuffer;
                    break;
                case ShaderType.Vertex:
                    DrawCall.D3DState.VsConstantBuffers[slot] = constantBuffer;
                    break;
                case ShaderType.Geometry:
                    DrawCall.D3DState.GsConstantBuffers[slot] = constantBuffer;
                    break;
                case ShaderType.Domain:
                    DrawCall.D3DState.DsConstantBuffers[slot] = constantBuffer;
                    break;
                case ShaderType.Hull:
                    DrawCall.D3DState.HsConstantBuffers[slot] = constantBuffer;
                    break;
                case ShaderType.Compute:
                    DrawCall.D3DState.CsConstantBuffers[slot] = constantBuffer;
                    break;
            }

            return (TB)this;
        }

        /// <summary>
        /// Function to set the constant buffers for a specific shader stage.
        /// </summary>
        /// <param name="shaderType">The shader stage to use.</param>
        /// <param name="constantBuffers">The constant buffers to copy.</param>
        /// <param name="startSlot">[Optional] The starting slot to use when copying the list.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="startSlot"/> is less than 0, or greater than/equal to <see cref="GorgonConstantBuffers.MaximumConstantBufferCount"/>.</exception>
        public TB ConstantBuffers(ShaderType shaderType, IReadOnlyList<GorgonConstantBufferView> constantBuffers, int startSlot = 0)
        {
            if ((startSlot < 0) || (startSlot >= GorgonConstantBuffers.MaximumConstantBufferCount))
            {
                throw new ArgumentOutOfRangeException(nameof(startSlot), string.Format(Resources.GORGFX_ERR_CBUFFER_SLOT_INVALID, GorgonConstantBuffers.MaximumConstantBufferCount));
            }

            switch (shaderType)
            {
                case ShaderType.Pixel:
                    StateCopy.CopyConstantBuffers(DrawCall.D3DState.PsConstantBuffers, constantBuffers, startSlot);
                    break;
                case ShaderType.Vertex:
                    StateCopy.CopyConstantBuffers(DrawCall.D3DState.VsConstantBuffers, constantBuffers, startSlot);
                    break;
                case ShaderType.Geometry:
                    StateCopy.CopyConstantBuffers(DrawCall.D3DState.GsConstantBuffers, constantBuffers, startSlot);
                    break;
                case ShaderType.Domain:
                    StateCopy.CopyConstantBuffers(DrawCall.D3DState.DsConstantBuffers, constantBuffers, startSlot);
                    break;
                case ShaderType.Hull:
                    StateCopy.CopyConstantBuffers(DrawCall.D3DState.HsConstantBuffers, constantBuffers, startSlot);
                    break;
                case ShaderType.Compute:
                    StateCopy.CopyConstantBuffers(DrawCall.D3DState.CsConstantBuffers, constantBuffers, startSlot);
                    break;
            }

            return (TB)this;
        }

        /// <summary>
        /// Function to assign a single shader resource view to the draw call.
        /// </summary>
        /// <param name="shaderType">The shader stage to use.</param>
        /// <param name="resourceView">The shader resource view to assign.</param>
        /// <param name="slot">[Optional] The slot used to asign the view.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="slot"/> is less than 0, or greater than/equal to <see cref="GorgonShaderResourceViews.MaximumShaderResourceViewCount"/>.</exception>
        public TB ShaderResource(ShaderType shaderType, GorgonShaderResourceView resourceView, int slot = 0)
        {
            if ((slot < 0) || (slot >= GorgonShaderResourceViews.MaximumShaderResourceViewCount))
            {
                throw new ArgumentOutOfRangeException(nameof(slot), string.Format(Resources.GORGFX_ERR_SRV_SLOT_INVALID, GorgonShaderResourceViews.MaximumShaderResourceViewCount));
            }

            switch (shaderType)
            {
                case ShaderType.Pixel:
                    DrawCall.D3DState.PsSrvs[slot] = resourceView;
                    break;
                case ShaderType.Vertex:
                    DrawCall.D3DState.VsSrvs[slot] = resourceView;
                    break;
                case ShaderType.Geometry:
                    DrawCall.D3DState.GsSrvs[slot] = resourceView;
                    break;
                case ShaderType.Domain:
                    DrawCall.D3DState.DsSrvs[slot] = resourceView;
                    break;
                case ShaderType.Hull:
                    DrawCall.D3DState.HsSrvs[slot] = resourceView;
                    break;
                case ShaderType.Compute:
                    DrawCall.D3DState.CsSrvs[slot] = resourceView;
                    break;
            }

            return (TB)this;
        }

        /// <summary>
        /// Function to assign the list of shader resource views to the draw call.
        /// </summary>
        /// <param name="shaderType">The shader stage to use.</param>
        /// <param name="resourceViews">The shader resource views to copy.</param>
        /// <param name="startSlot">[Optional] The starting slot to use when copying the list.</param>
        /// <returns>The fluent builder interface .</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="startSlot"/> is less than 0, or greater than/equal to <see cref="GorgonShaderResourceViews.MaximumShaderResourceViewCount"/>.</exception>
        public TB ShaderResources(ShaderType shaderType, IReadOnlyList<GorgonShaderResourceView> resourceViews, int startSlot = 0)
        {
            if ((startSlot < 0) || (startSlot >= GorgonShaderResourceViews.MaximumShaderResourceViewCount))
            {
                throw new ArgumentOutOfRangeException(nameof(startSlot), string.Format(Resources.GORGFX_ERR_SRV_SLOT_INVALID, GorgonShaderResourceViews.MaximumShaderResourceViewCount));
            }

            switch (shaderType)
            {
                case ShaderType.Pixel:
                    StateCopy.CopySrvs(DrawCall.D3DState.PsSrvs, resourceViews, startSlot);
                    break;
                case ShaderType.Vertex:
                    StateCopy.CopySrvs(DrawCall.D3DState.VsSrvs, resourceViews, startSlot);
                    break;
                case ShaderType.Geometry:
                    StateCopy.CopySrvs(DrawCall.D3DState.GsSrvs, resourceViews, startSlot);
                    break;
                case ShaderType.Domain:
                    StateCopy.CopySrvs(DrawCall.D3DState.DsSrvs, resourceViews, startSlot);
                    break;
                case ShaderType.Hull:
                    StateCopy.CopySrvs(DrawCall.D3DState.HsSrvs, resourceViews, startSlot);
                    break;
                case ShaderType.Compute:
                    StateCopy.CopySrvs(DrawCall.D3DState.CsSrvs, resourceViews, startSlot);
                    break;
            }

            return (TB)this;
        }

        /// <summary>
        /// Function to assign a single read/write (unordered access) view to the draw call.
        /// </summary>
        /// <param name="resourceView">The shader resource view to assign.</param>
        /// <param name="slot">[Optional] The slot used to asign the view.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="slot"/> is less than 0, or greater than/equal to <see cref="GorgonShaderResourceViews.MaximumShaderResourceViewCount"/>.</exception>
        public TB ReadWriteView(in GorgonReadWriteViewBinding resourceView, int slot = 0)
        {
            if ((slot < 0) || (slot >= GorgonShaderResourceViews.MaximumShaderResourceViewCount))
            {
                throw new ArgumentOutOfRangeException(nameof(slot), string.Format(Resources.GORGFX_ERR_SRV_SLOT_INVALID, GorgonShaderResourceViews.MaximumShaderResourceViewCount));
            }

            DrawCall.D3DState.ReadWriteViews[slot] = resourceView;
            return (TB)this;
        }

        /// <summary>
        /// Function to assign the list of read/write (unordered access) views to the draw call.
        /// </summary>
        /// <param name="resourceViews">The shader resource views to copy.</param>
        /// <param name="startSlot">[Optional] The starting slot to use when copying the list.</param>
        /// <returns>The fluent builder interface .</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="startSlot"/> is less than 0, or greater than/equal to <see cref="GorgonShaderResourceViews.MaximumShaderResourceViewCount"/>.</exception>
        public TB ReadWriteViews(IReadOnlyList<GorgonReadWriteViewBinding> resourceViews, int startSlot = 0)
        {
            if ((startSlot < 0) || (startSlot >= GorgonShaderResourceViews.MaximumShaderResourceViewCount))
            {
                throw new ArgumentOutOfRangeException(nameof(startSlot), string.Format(Resources.GORGFX_ERR_SRV_SLOT_INVALID, GorgonShaderResourceViews.MaximumShaderResourceViewCount));
            }

            StateCopy.CopyReadWriteViews(DrawCall.D3DState.ReadWriteViews, resourceViews, startSlot);
            return (TB)this;
        }

        /// <summary>
        /// Function to return the draw call.
        /// </summary>
        /// <returns>The draw call created or updated by this builder.</returns>
        public TDc Build()
        {
            TDc final = OnCreate();
            final.SetupConstantBuffers();
            final.SetupSamplers();
            final.SetupViews();
            
            if (final.D3DState.VertexBuffers == null)
            {
                final.D3DState.VertexBuffers = new GorgonVertexBufferBindings();
            }

            if (final.D3DState.StreamOutBindings == null)
            {
                final.D3DState.StreamOutBindings = new GorgonStreamOutBindings();
            }
            
            StateCopy.CopyVertexBuffers(final.D3DState.VertexBuffers, DrawCall.VertexBufferBindings, DrawCall.InputLayout);
            StateCopy.CopyStreamOutBuffers(final.D3DState.StreamOutBindings, DrawCall.StreamOutBufferBindings);

            // Copy over the available constants.
            StateCopy.CopyConstantBuffers(final.D3DState.PsConstantBuffers, DrawCall.D3DState.PsConstantBuffers, 0);
            StateCopy.CopyConstantBuffers(final.D3DState.VsConstantBuffers, DrawCall.D3DState.VsConstantBuffers, 0);
            StateCopy.CopyConstantBuffers(final.D3DState.GsConstantBuffers, DrawCall.D3DState.GsConstantBuffers, 0);
            StateCopy.CopyConstantBuffers(final.D3DState.HsConstantBuffers, DrawCall.D3DState.HsConstantBuffers, 0);
            StateCopy.CopyConstantBuffers(final.D3DState.DsConstantBuffers, DrawCall.D3DState.DsConstantBuffers, 0);
            StateCopy.CopyConstantBuffers(final.D3DState.CsConstantBuffers, DrawCall.D3DState.CsConstantBuffers, 0);

            // Copy over samplers.
            StateCopy.CopySamplers(final.D3DState.PsSamplers, DrawCall.D3DState.PsSamplers);
            StateCopy.CopySamplers(final.D3DState.VsSamplers, DrawCall.D3DState.VsSamplers);
            StateCopy.CopySamplers(final.D3DState.GsSamplers, DrawCall.D3DState.GsSamplers);
            StateCopy.CopySamplers(final.D3DState.DsSamplers, DrawCall.D3DState.DsSamplers);
            StateCopy.CopySamplers(final.D3DState.HsSamplers, DrawCall.D3DState.HsSamplers);
            StateCopy.CopySamplers(final.D3DState.CsSamplers, DrawCall.D3DState.CsSamplers);

            // Copy over shader resource views.
            StateCopy.CopySrvs(final.D3DState.PsSrvs, DrawCall.D3DState.PsSrvs, 0);
            StateCopy.CopySrvs(final.D3DState.VsSrvs, DrawCall.D3DState.VsSrvs, 0);
            StateCopy.CopySrvs(final.D3DState.GsSrvs, DrawCall.D3DState.GsSrvs, 0);
            StateCopy.CopySrvs(final.D3DState.DsSrvs, DrawCall.D3DState.DsSrvs, 0);
            StateCopy.CopySrvs(final.D3DState.HsSrvs, DrawCall.D3DState.HsSrvs, 0);
            StateCopy.CopySrvs(final.D3DState.CsSrvs, DrawCall.D3DState.CsSrvs, 0);

            // Copy over unordered access views.
            StateCopy.CopyReadWriteViews(final.D3DState.ReadWriteViews, DrawCall.D3DState.ReadWriteViews, 0);

            final.D3DState.PipelineState = new GorgonPipelineState(DrawCall.PipelineState);

            OnUpdate(final);

            return final;
        }

        /// <summary>
        /// Function to reset the builder to the specified draw call state.
        /// </summary>
        /// <param name="drawCall">[Optional] The specified draw call state to copy.</param>
        /// <returns>The fluent builder interface.</returns>
        public TB ResetTo(TDc drawCall = null)
        {
            if (drawCall == null)
            {
                return Clear();
            }
            
            VertexBufferBindings(drawCall.InputLayout, drawCall.VertexBufferBindings);
            StreamOutBuffers(drawCall.StreamOutBufferBindings);

            // Copy over the available constants.
            ConstantBuffers(ShaderType.Pixel, drawCall.D3DState.PsConstantBuffers);
            ConstantBuffers(ShaderType.Vertex, drawCall.D3DState.VsConstantBuffers);
            ConstantBuffers(ShaderType.Geometry, drawCall.D3DState.GsConstantBuffers);
            ConstantBuffers(ShaderType.Domain, drawCall.D3DState.DsConstantBuffers);
            ConstantBuffers(ShaderType.Hull, drawCall.D3DState.HsConstantBuffers);
            ConstantBuffers(ShaderType.Compute, drawCall.D3DState.CsConstantBuffers);
            
            SamplerStates(ShaderType.Pixel, drawCall.D3DState.PsSamplers);
            SamplerStates(ShaderType.Vertex, drawCall.D3DState.VsSamplers);
            SamplerStates(ShaderType.Geometry, drawCall.D3DState.GsSamplers);
            SamplerStates(ShaderType.Domain, drawCall.D3DState.DsSamplers);
            SamplerStates(ShaderType.Hull, drawCall.D3DState.HsSamplers);
            SamplerStates(ShaderType.Compute, drawCall.D3DState.CsSamplers);

            ShaderResources(ShaderType.Pixel, drawCall.D3DState.PsSrvs);
            ShaderResources(ShaderType.Vertex, drawCall.D3DState.VsSrvs);
            ShaderResources(ShaderType.Geometry, drawCall.D3DState.GsSrvs);
            ShaderResources(ShaderType.Domain, drawCall.D3DState.DsSrvs);
            ShaderResources(ShaderType.Hull, drawCall.D3DState.HsSrvs);
            ShaderResources(ShaderType.Compute, drawCall.D3DState.CsSrvs);

            ReadWriteViews(drawCall.D3DState.ReadWriteViews);

            DrawCall.D3DState.PipelineState = new GorgonPipelineState(DrawCall.PipelineState);

            return OnResetTo(drawCall);
        }

        /// <summary>
        /// Function to clear the builder to a default state.
        /// </summary>
        /// <returns>The fluent builder interface.</returns>
        public TB Clear()
        {
            DrawCall.D3DState.VertexBuffers.Clear();
            DrawCall.D3DState.StreamOutBindings.Clear();
            
            DrawCall.D3DState.PsConstantBuffers.Clear();
            DrawCall.D3DState.VsConstantBuffers.Clear();
            DrawCall.D3DState.GsConstantBuffers.Clear();
            DrawCall.D3DState.HsConstantBuffers.Clear();
            DrawCall.D3DState.DsConstantBuffers.Clear();
            DrawCall.D3DState.CsConstantBuffers.Clear();

            DrawCall.D3DState.PsSamplers.Clear();
            DrawCall.D3DState.VsSamplers.Clear();
            DrawCall.D3DState.GsSamplers.Clear();
            DrawCall.D3DState.DsSamplers.Clear();
            DrawCall.D3DState.HsSamplers.Clear();
            DrawCall.D3DState.CsSamplers.Clear();

            DrawCall.D3DState.PsSrvs.Clear();
            DrawCall.D3DState.VsSrvs.Clear();
            DrawCall.D3DState.GsSrvs.Clear();
            DrawCall.D3DState.DsSrvs.Clear();
            DrawCall.D3DState.HsSrvs.Clear();
            DrawCall.D3DState.CsSrvs.Clear();

            DrawCall.D3DState.ReadWriteViews.Clear();

            DrawCall.D3DState.PipelineState.Clear();

            return OnClear();
        }
        #endregion

        #region Constructor/Finalizer.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonDrawCallBuilder"/> class.
        /// </summary>
        /// <param name="drawCall">The worker draw call.</param>
        private protected GorgonDrawCallBuilderCommon(TDc drawCall)
        {
            DrawCall = drawCall;
            drawCall.SetupConstantBuffers();
            drawCall.SetupSamplers();
            drawCall.SetupViews();
            DrawCall.D3DState.VertexBuffers = new GorgonVertexBufferBindings();
            DrawCall.D3DState.StreamOutBindings = new GorgonStreamOutBindings();
            DrawCall.D3DState.PipelineState = new GorgonPipelineState();
        }
        #endregion
    }
}