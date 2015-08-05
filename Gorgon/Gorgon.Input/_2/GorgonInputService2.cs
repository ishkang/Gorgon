﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2015 Michael Winsor
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
// Created: Saturday, July 18, 2015 4:37:48 PM
// 
#endregion

using System.Collections.Generic;
using System.Windows.Forms;

namespace Gorgon.Input
{
	/// <inheritdoc/>
	public abstract class GorgonInputService2
		: IGorgonInputService
	{
		#region Properties.
		/// <summary>
		/// Property to return the router used to pass event data to the appropriate devices.
		/// </summary>
		/// <remarks>
		/// Implementors of an input service must use the method provided by the <see cref="GorgonInputDeviceEventRouting"/> type to ensure that events received for the device are forwarded to the correct 
		/// <see cref="IGorgonInputDevice"/>. Failure to do so will keep the device from updating its state and from triggering any of its own events.
		/// </remarks>
		protected GorgonInputDeviceEventRouting EventRouter
		{
			get;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to acquire or unacquire a device.
		/// </summary>
		/// <param name="device">The device being acquire or unacquired.</param>
		/// <param name="acquisitionState"><b>true</b> if the device is being acquired, <b>false</b> if the device is being unacquired.</param>
		/// <remarks>
		/// Plug in implementors will use this method to perform any set up or tear down of functionality required when the device becomes acquired or unacquired respectively. This method will be called when the 
		/// <see cref="IGorgonInputDevice.IsAcquired"/> property is set.
		/// </remarks>
		protected abstract internal void AcquireDevice(IGorgonInputDevice device, bool acquisitionState);

		/// <summary>
		/// Function to register a device when it binds with a window.
		/// </summary>
		/// <param name="device">The device that is being bound to the window.</param>
		/// <param name="deviceInfo">Information about the device being bound to the window.</param>
		/// <param name="parentForm">The parent form for the window.</param>
		/// <param name="window">The window that the device is being bound with.</param>
		/// <param name="exclusive"><b>true</b> if the device is being registered as exclusive, <b>false</b> if not.</param>
		/// <remarks>
		/// Plug in implementors will use this method to ensure that required functionality is present when a device is bound to a window. This method will be called when the <see cref="IGorgonInputDevice.BindWindow"/> 
		/// method is called.
		/// </remarks>
		protected internal abstract void RegisterDevice(IGorgonInputDevice device, IGorgonInputDeviceInfo2 deviceInfo, Form parentForm, Control window, ref bool exclusive);

		/// <summary>
		/// Function to unregister a device when it unbinds from a window.
		/// </summary>
		/// <param name="device">The device that is being unbound from the window.</param>
		/// <remarks>
		/// Plug in implementors will use this method to ensure that any clean up required for functionality is present when a device is unbound from a window. This method will be called when the 
		/// <see cref="IGorgonInputDevice.UnbindWindow"/>  method is called.
		/// </remarks>
		protected internal abstract void UnregisterDevice(IGorgonInputDevice device);

		/// <summary>
		/// Function to perform enumeration of keyboard devices.
		/// </summary>
		/// <returns>A list of <see cref="IGorgonKeyboardInfo2"/> items containing information about each keyboard.</returns>
		/// <remarks>
		/// <para>
		/// This is intended for use by input plug in developers. Developers may enumerate their devices in whichever mechanism is provided by the underlying input system and return a list of 
		/// <see cref="IGorgonKeyboardInfo2"/> items.
		/// </para>
		/// </remarks>
		protected abstract IReadOnlyList<IGorgonKeyboardInfo2> OnEnumerateKeyboards();

		/// <summary>
		/// Function to perform enumeration of mice or other pointing devices.
		/// </summary>
		/// <returns>A list of <see cref="IGorgonMouseInfo2"/> items containing information about each mouse.</returns>
		/// <remarks>
		/// <para>
		/// This is intended for use by input plug in developers. Developers may enumerate their devices in whichever mechanism is provided by the underlying input system and return a list of 
		/// <see cref="IGorgonMouseInfo2"/> items.
		/// </para>
		/// </remarks>
		protected abstract IReadOnlyList<IGorgonMouseInfo2> OnEnumerateMice();

		/// <summary>
		/// Function to perform enumeration of joysticks or other gaming devices.
		/// </summary>
		/// <returns>A list of <see cref="IGorgonMouseInfo2"/> items containing information about each joystick.</returns>
		/// <remarks>
		/// <para>
		/// This is intended for use by input plug in developers. Developers may enumerate their devices in whichever mechanism is provided by the underlying input system and return a list of 
		/// <see cref="IGorgonJoystickInfo2"/> items.
		/// </para>
		/// </remarks>
		protected abstract IReadOnlyList<IGorgonJoystickInfo2> OnEnumerateJoysticks();
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonInputService2"/> class.
		/// </summary>
		protected GorgonInputService2()
		{
			EventRouter = new GorgonInputDeviceEventRouting();
		}
		#endregion

		#region IGorgonInputService Members
		/// <inheritdoc/>
		public IReadOnlyList<IGorgonKeyboardInfo2> EnumerateKeyboards()
		{
			return OnEnumerateKeyboards();
		}

		/// <inheritdoc/>
		public IReadOnlyList<IGorgonMouseInfo2> EnumerateMice()
		{
			return OnEnumerateMice();
		}

		/// <inheritdoc/>
		public IReadOnlyList<IGorgonJoystickInfo2> EnumerateJoysticks()
		{
			return OnEnumerateJoysticks();
		}
		#endregion
	}
}