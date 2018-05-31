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
    /// A builder used to create <see cref="GorgonStreamOutCall"/> objects.
    /// </summary>
    public sealed class GorgonStreamOutCallBuilder
    {
        #region Variables.
        // The worker call used to build up the object.
        private readonly GorgonStreamOutCall _workerCall;
        #endregion

        #region Methods.
        /// <summary>
        /// Function to assign a list of samplers to a shader on the pipeline.
        /// </summary>
        /// <param name="samplers">The samplers to assign.</param>
        /// <returns>The fluent interface for this builder.</returns>
        public GorgonStreamOutCallBuilder SamplerStates(IReadOnlyList<GorgonSamplerState> samplers)
        {
            StateCopy.CopySamplers(_workerCall.D3DState.PsSamplers, samplers);
            return this;
        }

        /// <summary>
        /// Function to assign a sampler to a pixel shader on the pipeline.
        /// </summary>
        /// <param name="sampler">The sampler to assign.</param>
        /// <param name="index">[Optional] The index of the sampler.</param>
        /// <returns>The fluent interface for this builder.</returns>
        public GorgonStreamOutCallBuilder SamplerState(GorgonSamplerStateBuilder sampler, int index = 0)
        {
            return SamplerState(sampler.Build(), index);
        }

        /// <summary>
        /// Function to assign a sampler to a pixel shader on the pipeline.
        /// </summary>
        /// <param name="sampler">The sampler to assign.</param>
        /// <param name="index">[Optional] The index of the sampler.</param>
        /// <returns>The fluent interface for this builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="index"/> parameter is less than 0, or greater than/equal to <see cref="GorgonSamplerStates.MaximumSamplerStateCount"/>.</exception>
        public GorgonStreamOutCallBuilder SamplerState(GorgonSamplerState sampler, int index = 0)
        {
            if ((index < 0) || (index >= GorgonSamplerStates.MaximumSamplerStateCount))
            {
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(Resources.GORGFX_ERR_INVALID_SAMPLER_INDEX, GorgonSamplerStates.MaximumSamplerStateCount));
            }

            _workerCall.D3DState.PsSamplers[index] = sampler;
            return this;
        }

        /// <summary>
        /// Function to set the pipeline state for this draw call.
        /// </summary>
        /// <param name="pipelineState">The pipeline state to assign.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="pipelineState"/> parameter is <b>null</b>.</exception>
        public GorgonStreamOutCallBuilder PipelineState(GorgonStreamOutPipelineState pipelineState)
        {
            _workerCall.PipelineState = new GorgonStreamOutPipelineState(pipelineState ?? throw new ArgumentNullException(nameof(pipelineState)));
            return this;
        }

        /// <summary>
        /// Function to set the pipeline state for this draw call.
        /// </summary>
        /// <param name="pipelineState">The pipeline state to assign.</param>
        /// <returns>The fluent builder interface.</returns>
        public GorgonStreamOutCallBuilder PipelineState(GorgonStreamOutPipelineStateBuilder pipelineState)
        {
            return PipelineState(pipelineState?.Build());
        }

        /// <summary>
        /// Function to set a vertex buffer binding for the draw call in slot 0.
        /// </summary>
        /// <param name="layout">The input layout to use.</param>
        /// <param name="binding">The vertex buffer binding to set.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="layout"/> parameter is <b>null</b>.</exception>
        public GorgonStreamOutCallBuilder VertexBuffer(GorgonInputLayout layout, in GorgonVertexBufferBinding binding)
        {
            if (_workerCall.D3DState.VertexBuffers == null)
            {
                _workerCall.D3DState.VertexBuffers = new GorgonVertexBufferBindings();
            }

            _workerCall.D3DState.VertexBuffers.Clear();
            _workerCall.D3DState.VertexBuffers[0] = binding;
            _workerCall.D3DState.VertexBuffers.InputLayout = layout ?? throw new ArgumentNullException(nameof(layout));
            return this;
        }

        /// <summary>
        /// Function to set a constant buffer for a pixel shader.
        /// </summary>
        /// <param name="shaderType">The shader stage to use.</param>
        /// <param name="constantBuffer">The constant buffer to assign.</param>
        /// <param name="slot">The slot for the constant buffer.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="slot"/> is less than 0, or greater than/equal to <see cref="GorgonConstantBuffers.MaximumConstantBufferCount"/>.</exception>
        public GorgonStreamOutCallBuilder ConstantBuffer(ShaderType shaderType, GorgonConstantBufferView constantBuffer, int slot = 0)
        {
            if ((slot < 0) || (slot >= GorgonConstantBuffers.MaximumConstantBufferCount))
            {
                throw new ArgumentOutOfRangeException(nameof(slot), string.Format(Resources.GORGFX_ERR_CBUFFER_SLOT_INVALID, 0));
            }

            _workerCall.D3DState.PsConstantBuffers[slot] = constantBuffer;
            return this;
        }

        /// <summary>
        /// Function to set the constant buffers for a pixel shader.
        /// </summary>
        /// <param name="constantBuffers">The constant buffers to copy.</param>
        /// <param name="startSlot">[Optional] The starting slot to use when copying the list.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="startSlot"/> is less than 0, or greater than/equal to <see cref="GorgonConstantBuffers.MaximumConstantBufferCount"/>.</exception>
        public GorgonStreamOutCallBuilder ConstantBuffers(IReadOnlyList<GorgonConstantBufferView> constantBuffers, int startSlot = 0)
        {
            if ((startSlot < 0) || (startSlot >= GorgonConstantBuffers.MaximumConstantBufferCount))
            {
                throw new ArgumentOutOfRangeException(nameof(startSlot), string.Format(Resources.GORGFX_ERR_CBUFFER_SLOT_INVALID, GorgonConstantBuffers.MaximumConstantBufferCount));
            }

            StateCopy.CopyConstantBuffers(_workerCall.D3DState.PsConstantBuffers, constantBuffers, startSlot);
            return this;
        }

        /// <summary>
        /// Function to assign a single pixel shader resource view to the draw call.
        /// </summary>
        /// <param name="resourceView">The shader resource view to assign.</param>
        /// <param name="slot">[Optional] The slot used to asign the view.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="slot"/> is less than 0, or greater than/equal to <see cref="GorgonShaderResourceViews.MaximumShaderResourceViewCount"/>.</exception>
        public GorgonStreamOutCallBuilder ShaderResource(GorgonShaderResourceView resourceView, int slot = 0)
        {
            if ((slot < 0) || (slot >= GorgonShaderResourceViews.MaximumShaderResourceViewCount))
            {
                throw new ArgumentOutOfRangeException(nameof(slot), string.Format(Resources.GORGFX_ERR_SRV_SLOT_INVALID, GorgonShaderResourceViews.MaximumShaderResourceViewCount));
            }

            _workerCall.D3DState.PsSrvs[slot] = resourceView;
            return this;
        }

        /// <summary>
        /// Function to assign the list of pixel shader resource views to the draw call.
        /// </summary>
        /// <param name="resourceViews">The shader resource views to copy.</param>
        /// <param name="startSlot">[Optional] The starting slot to use when copying the list.</param>
        /// <returns>The fluent builder interface .</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="startSlot"/> is less than 0, or greater than/equal to <see cref="GorgonShaderResourceViews.MaximumShaderResourceViewCount"/>.</exception>
        public GorgonStreamOutCallBuilder ShaderResources(IReadOnlyList<GorgonShaderResourceView> resourceViews, int startSlot = 0)
        {
            if ((startSlot < 0) || (startSlot >= GorgonShaderResourceViews.MaximumShaderResourceViewCount))
            {
                throw new ArgumentOutOfRangeException(nameof(startSlot), string.Format(Resources.GORGFX_ERR_SRV_SLOT_INVALID, GorgonShaderResourceViews.MaximumShaderResourceViewCount));
            }

            StateCopy.CopySrvs(_workerCall.D3DState.PsSrvs, resourceViews, startSlot);
            return this;
        }

        /// <summary>
        /// Function to return the draw call.
        /// </summary>
        /// <returns>The draw call created or updated by this builder.</returns>
        public GorgonStreamOutCall Build()
        {
            var final = new GorgonStreamOutCall();
            final.SetupConstantBuffers();
            final.SetupSamplers();
            final.SetupViews();
            
            if (final.D3DState.VertexBuffers == null)
            {
                final.D3DState.VertexBuffers = new GorgonVertexBufferBindings();
            }

            final.D3DState.VertexBuffers.InputLayout = _workerCall.InputLayout;
            final.D3DState.VertexBuffers[0] = _workerCall.VertexBufferBinding;

            // Copy over the available constants.
            StateCopy.CopyConstantBuffers(final.D3DState.PsConstantBuffers, _workerCall.D3DState.PsConstantBuffers, 0);

            // Copy over samplers.
            StateCopy.CopySamplers(final.D3DState.PsSamplers, _workerCall.D3DState.PsSamplers);

            // Copy over shader resource views.
            StateCopy.CopySrvs(final.D3DState.PsSrvs, _workerCall.D3DState.PsSrvs, 0);

            final.PipelineState = new GorgonStreamOutPipelineState(_workerCall.PipelineState);

            return final;
        }

        /// <summary>
        /// Function to reset the builder to the specified draw call state.
        /// </summary>
        /// <param name="drawCall">[Optional] The specified draw call state to copy.</param>
        /// <returns>The fluent builder interface.</returns>
        public GorgonStreamOutCallBuilder ResetTo(GorgonStreamOutCall drawCall = null)
        {
            if (drawCall == null)
            {
                return Clear();
            }

            VertexBuffer(drawCall.InputLayout, drawCall.VertexBufferBinding);
            
            // Copy over the available constants.
            ConstantBuffers(drawCall.D3DState.PsConstantBuffers);
            SamplerStates(drawCall.D3DState.PsSamplers);
            ShaderResources(drawCall.D3DState.PsSrvs);
            
            _workerCall.PipelineState = new GorgonStreamOutPipelineState(drawCall.PipelineState.PipelineState);
            
            return this;
        }

        /// <summary>
        /// Function to clear the builder to a default state.
        /// </summary>
        /// <returns>The fluent builder interface.</returns>
        public GorgonStreamOutCallBuilder Clear()
        {
            _workerCall.D3DState.VertexBuffers.Clear();
            
            _workerCall.D3DState.PsConstantBuffers.Clear();
            _workerCall.D3DState.PsSamplers.Clear();
            _workerCall.D3DState.PsSrvs.Clear();

            _workerCall.D3DState.PipelineState.Clear();

            return this;
        }
        #endregion

        #region Constructor/Finalizer.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonDrawCallBuilder"/> class.
        /// </summary>
        public GorgonStreamOutCallBuilder()
        {
            _workerCall = new GorgonStreamOutCall
                          {
                              PipelineState = new GorgonStreamOutPipelineState(new GorgonPipelineState())
                          };
            _workerCall.SetupConstantBuffers();
            _workerCall.SetupSamplers();
            _workerCall.SetupViews();
            _workerCall.D3DState.VertexBuffers = new GorgonVertexBufferBindings();
        }
        #endregion
    }
}