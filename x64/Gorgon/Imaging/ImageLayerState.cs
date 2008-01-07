#region LGPL.
// 
// Gorgon.
// Copyright (C) 2006 Michael Winsor
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Created: Wednesday, May 17, 2006 10:50:26 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpUtilities.Utility;
using DX = SlimDX;
using D3D9 = SlimDX.Direct3D9;
using GorgonLibrary.Graphics;

namespace GorgonLibrary.Internal
{
	/// <summary>
	/// Object representing the various renderstates.
	/// </summary>
	public class ImageLayerStates
	{
		#region Variables.
		private int _imageLayerIndex = 0;								// Image layer index.
		private ImageOperations _colorOperation;						// Color operation.
		private ImageOperationArguments _colorArgument0;				// Color operation argument 0.
		private ImageOperationArguments _colorArgument1;				// Color operation argument 1.
		private ImageOperationArguments _colorArgument2;				// Color operation argument 2.
		private ImageOperations _alphaOperation;						// Alpha operation.
		private ImageOperationArguments _alphaArgument0;				// Alpha operation argument 0.
		private ImageOperationArguments _alphaArgument1;				// Alpha operation argument 1.
		private ImageOperationArguments _alphaArgument2;				// Alpha operation argument 2.
		private Color _constantColor;									// Constant color.
		private ImageFilters _imageMagFilters;							// Image filters.
		private ImageFilters _imageMinFilters;							// Image filters.
		private Color _borderColor;										// Image border color.
		private ImageAddressing _Uaddress;								// Horizontal addressing.
		private ImageAddressing _Vaddress;								// Vertical addressing.
		private D3D9.Device _device = null;								// D3D device object.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to return whether the device is ready or not.
		/// </summary>
		private bool DeviceReady
		{
			get
			{
				if (Device == null)
					return false;

				return !Gorgon.Screen.DeviceNotReset;
			}
		}

		/// <summary>
		/// Property to return the device object.
		/// </summary>
		private D3D9.Device Device
		{
			get
			{
				if (Gorgon.Screen == null)
					return null;

				if (_device == null)
					_device = Gorgon.Screen.Device;

				return _device;
			}
		}

		/// <summary>
		/// Property to set or return the alpha operation.
		/// </summary>
		public ImageOperations AlphaOperation
		{
			get
			{
				return _alphaOperation;
			}
			set
			{
				if (_alphaOperation == value)
					return;
				SetAlphaOperation(value);
			}
		}

		/// <summary>
		/// Property to set or return alpha argument 0.
		/// </summary>
		public ImageOperationArguments AlphaOperationArgument0
		{
			get
			{
				return _alphaArgument0;
			}
			set
			{
				if (_alphaArgument0 == value)
					return;
				SetAlphaOperationArgument0(value);
			}
		}

		/// <summary>
		/// Property to set or return alpha argument 1.
		/// </summary>
		public ImageOperationArguments AlphaOperationArgument1
		{
			get
			{
				return _alphaArgument1;
			}
			set
			{
				if (_alphaArgument1 == value)
					return;
				SetAlphaOperationArgument1(value);
			}
		}

		/// <summary>
		/// Property to set or return alpha argument 2.
		/// </summary>
		public ImageOperationArguments AlphaOperationArgument2
		{
			get
			{
				return _alphaArgument2;
			}
			set
			{
				if (_alphaArgument2 == value)
					return;
				SetAlphaOperationArgument2(value);
			}
		}

		/// <summary>
		/// Property to set or return the color operation.
		/// </summary>
		public ImageOperations ColorOperation
		{
			get
			{
				return _colorOperation;
			}
			set
			{
				if (_colorOperation == value)
					return;
				SetColorOperation(value);
			}
		}

		/// <summary>
		/// Property to set or return color argument 0.
		/// </summary>
		public ImageOperationArguments ColorOperationArgument0
		{
			get
			{
				return _colorArgument0;
			}
			set
			{
				if (_colorArgument0 == value)
					return;
				SetColorOperationArgument0(value);
			}
		}

		/// <summary>
		/// Property to set or return color argument 1.
		/// </summary>
		public ImageOperationArguments ColorOperationArgument1
		{
			get
			{
				return _colorArgument1;
			}
			set
			{
				if (_colorArgument1 == value)
					return;
				SetColorOperationArgument1(value);
			}
		}

		/// <summary>
		/// Property to set or return color argument 2.
		/// </summary>
		public ImageOperationArguments ColorOperationArgument2
		{
			get
			{
				return _colorArgument2;
			}
			set
			{
				if (_colorArgument2 == value)
					return;
				SetColorOperationArgument2(value);
			}
		}

		/// <summary>
		/// Property to set or return the constant color.
		/// </summary>
		public Color ConstantColor
		{
			get
			{
				return _constantColor;
			}
			set
			{
				if (_constantColor == value)
					return;
				SetConstantColor(value);
			}
		}

		/// <summary>
		/// Property to set or return the minification filter.
		/// </summary>
		public ImageFilters MinificationFilter
		{
			get
			{
				return _imageMinFilters;
			}
			set
			{
				if (_imageMinFilters == value)
					return;
				SetImageMinFilter(value);
			}
		}

		/// <summary>
		/// Property to set or return the magnification filter.
		/// </summary>
		public ImageFilters MagnificationFilter
		{
			get
			{
				return _imageMagFilters;
			}
			set
			{
				if (_imageMagFilters == value)
					return;
				SetImageMagFilter(value);
			}
		}

		/// <summary>
		/// Property to set or return the border color.
		/// </summary>
		public Color BorderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				if (_borderColor == value)
					return;
				SetBorderColor(value);
			}
		}

		/// <summary>
		/// Property to set or return the horizontal addressing mode.
		/// </summary>
		public ImageAddressing HorizontalAddressing
		{
			get
			{
				return _Uaddress;
			}
			set
			{
				if (_Uaddress == value)
					return;
				SetUAddressing(value);
			}
		}

		/// <summary>
		/// Property to set or return the vertical addressing mode.
		/// </summary>
		public ImageAddressing VerticalAddressing
		{
			get
			{
				return _Vaddress;
			}
			set
			{
				if (_Vaddress == value)
					return;
				SetVAddressing(value);
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to set a alpha operation.
		/// </summary>		
		/// <param name="value">Operation to use.</param>
		private void SetAlphaOperation(ImageOperations value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.AlphaOperation, Converter.Convert(value));
			_alphaOperation = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the first alpha operation argument.
		/// </summary>		
		/// <param name="value">Operation argument to use.</param>
		private void SetAlphaOperationArgument0(ImageOperationArguments value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.AlphaArg0, Converter.Convert(value));
			_alphaArgument0 = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the second alpha operation argument.
		/// </summary>		
		/// <param name="value">Operation argument to use.</param>
		private void SetAlphaOperationArgument1(ImageOperationArguments value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.AlphaArg1, Converter.Convert(value));
			_alphaArgument1 = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the third alpha operation argument.
		/// </summary>		
		/// <param name="value">Operation argument to use.</param>
		private void SetAlphaOperationArgument2(ImageOperationArguments value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.AlphaArg2, Converter.Convert(value));
			_alphaArgument2 = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set a color operation.
		/// </summary>		
		/// <param name="value">Operation to use.</param>
		private void SetColorOperation(ImageOperations value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.ColorOperation, Converter.Convert(value));
			_colorOperation = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the first color operation argument.
		/// </summary>		
		/// <param name="value">Operation argument to use.</param>
		private void SetColorOperationArgument0(ImageOperationArguments value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.ColorArg0, Converter.Convert(value));
			_colorArgument0 = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the second color operation argument.
		/// </summary>		
		/// <param name="value">Operation argument to use.</param>
		private void SetColorOperationArgument1(ImageOperationArguments value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.ColorArg1, Converter.Convert(value));
			_colorArgument1 = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the third color operation argument.
		/// </summary>		
		/// <param name="value">Operation argument to use.</param>
		private void SetColorOperationArgument2(ImageOperationArguments value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.ColorArg2, Converter.Convert(value));
			_colorArgument2 = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the constant color.
		/// </summary>		
		/// <param name="value">Color to use.</param>
		private void SetConstantColor(Color value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetTextureStageState(_imageLayerIndex, D3D9.TextureStage.Constant, value.ToArgb());
			_constantColor = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set an image magnification filter.
		/// </summary>
		/// <param name="value">Filter to use.</param>
		private void SetImageMagFilter(ImageFilters value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetSamplerState(_imageLayerIndex, D3D9.SamplerState.MagFilter, Converter.Convert(value));
			_imageMagFilters = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set an image minification filter.
		/// </summary>		
		/// <param name="value">Filter to use.</param>
		private void SetImageMinFilter(ImageFilters value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetSamplerState(_imageLayerIndex, D3D9.SamplerState.MinFilter, Converter.Convert(value));
			_imageMinFilters = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the border color.
		/// </summary>
		/// <param name="value">Border color</param>
		private void SetBorderColor(Color value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetSamplerState(_imageLayerIndex, D3D9.SamplerState.BorderColor, value.ToArgb());
			_borderColor = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the horizontal addressing mode.
		/// </summary>
		/// <param name="value">Addressing mode.</param>
		private void SetUAddressing(ImageAddressing value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetSamplerState(_imageLayerIndex, D3D9.SamplerState.AddressU, Converter.Convert(value));
			_Uaddress = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the vertical addressing mode.
		/// </summary>
		/// <param name="value">Addressing mode.</param>
		private void SetVAddressing(ImageAddressing value)
		{
			DX.DirectXException.EnableExceptions = false;
			if (DeviceReady)
				Device.SetSamplerState(_imageLayerIndex, D3D9.SamplerState.AddressV, Converter.Convert(value));
			_Vaddress = value;
			DX.DirectXException.EnableExceptions = true;
		}

		/// <summary>
		/// Function to set the render states.
		/// </summary>
		public void SetStates()
		{
			// Set values for each stage.
			SetColorOperationArgument0(_colorArgument0);
			SetColorOperationArgument1(_colorArgument1);
			SetColorOperationArgument2(_colorArgument2);
			SetAlphaOperationArgument0(_alphaArgument0);
			SetAlphaOperationArgument1(_alphaArgument1);
			SetAlphaOperationArgument2(_alphaArgument2);
			SetColorOperation(_colorOperation);
			SetAlphaOperation(_alphaOperation);
			SetConstantColor(_constantColor);
			SetImageMagFilter(_imageMagFilters);
			SetImageMinFilter(_imageMinFilters);
			SetBorderColor(_borderColor);
			SetUAddressing(_Uaddress);
			SetVAddressing(_Vaddress);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ImageLayerStates(int index)
		{
			// Set defaults.
			_imageLayerIndex = index;
			_colorArgument0 = ImageOperationArguments.Current;
			_colorArgument1 = ImageOperationArguments.Texture;
			_colorArgument2 = ImageOperationArguments.Current;
			_alphaArgument0 = ImageOperationArguments.Current;
			_alphaArgument1 = ImageOperationArguments.Diffuse;
			_alphaArgument2 = ImageOperationArguments.Current;
			_colorOperation = ImageOperations.Disable;
			_alphaOperation = ImageOperations.Disable;
			_imageMagFilters = ImageFilters.Point;
			_imageMinFilters = ImageFilters.Point;
			_borderColor = Color.Black;
			_Uaddress = ImageAddressing.Wrapping;
			_Vaddress = ImageAddressing.Wrapping;
		}
		#endregion
	}
}

