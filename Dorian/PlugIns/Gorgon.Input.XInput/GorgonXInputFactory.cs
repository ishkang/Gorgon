#region MIT.
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
// Created: Friday, July 15, 2011 6:22:54 AM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using GorgonLibrary.Collections;
using GorgonLibrary.Diagnostics;
using GorgonLibrary.Input;
using XI = SharpDX.XInput;
using Forms = System.Windows.Forms;

namespace GorgonLibrary.Input.XInput
{
	/// <summary>
	/// Object representing the main interface to the input library.
	/// </summary>
	internal class GorgonXInputFactory
		: GorgonInputFactory
	{
		#region Methods.
		/// <summary>
		/// Function to enumerate the pointing devices on the system.
		/// </summary>
		/// <returns>A list of pointing device names.</returns>
		protected override IEnumerable<GorgonInputDeviceInfo> EnumeratePointingDevices()
		{
			return new GorgonInputDeviceInfo[] {};
		}

		/// <summary>
		/// Function to enumerate the keyboard devices on the system.
		/// </summary>
		/// <returns>A list of keyboard device names.</returns>
		protected override IEnumerable<GorgonInputDeviceInfo> EnumerateKeyboardDevices()
		{
			return new GorgonInputDeviceInfo[] {};
		}

		/// <summary>
		/// Function to enumerate the joystick devices attached to the system.
		/// </summary>
		/// <returns>A list of joystick device names.</returns>
		protected override IEnumerable<GorgonInputDeviceInfo> EnumerateJoysticksDevices()
		{
			XI.UserIndex[] controllerIndex = ((XI.UserIndex[])Enum.GetValues(typeof(XI.UserIndex))).OrderBy((item) => item).ToArray();
			XI.Controller[] controllers = new XI.Controller[controllerIndex.Length];
			IList<GorgonXInputDeviceInfo> devices = null;

			devices = new List<GorgonXInputDeviceInfo>();

			// Enumerate all controllers.
			for (int i = 0; i < controllerIndex.Length; i++)
			{
				XI.Capabilities caps = default(XI.Capabilities);

				controllers[i] = new XI.Controller(controllerIndex[i]);
				if (controllers[i].IsConnected)
				{
					if (controllerIndex[i] != XI.UserIndex.Any)
					{
						caps = controllers[i].GetCapabilities(XI.DeviceQueryType.Any);
						devices.Add(new GorgonXInputDeviceInfo(string.Format("{0}", i + 1) + ": XInput " + caps.SubType.ToString() + " Controller", caps.SubType.ToString(), "XInput_" + controllerIndex[i].ToString(), controllers[i], i));
					}
				}
				else
					devices.Add(new GorgonXInputDeviceInfo(string.Format("{0}", i + 1) + ": XInput Disconnected Controller ", "Disconnected Controller", "XInput_" + controllerIndex[i].ToString(), controllers[i], i));
			}

			return devices.OrderBy((item) => item.Name);
		}

		/// <summary>
		/// Function to enumerate device types for which there is no class wrapper and will return data in a custom property collection.
		/// </summary>
		/// <returns>
		/// A list of custom HID types.
		/// </returns>
		protected override IEnumerable<GorgonInputDeviceInfo> EnumerateCustomHIDs()
		{
			return new GorgonInputDeviceInfo[] {};
		}

		/// <summary>
		/// Creates the custom HID impl.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="hidName">Name of the hid.</param>
		/// <returns></returns>
		protected override GorgonCustomHID CreateCustomHIDImpl(Forms.Control window, GorgonInputDeviceInfo hidName)
		{
			throw new NotImplementedException("This plug-in only contains XBOX 360 controller devices.");
		}

		/// <summary>
		/// Creates the keyboard impl.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="keyboardName">Name of the keyboard.</param>
		/// <returns></returns>
		protected override GorgonKeyboard CreateKeyboardImpl(Forms.Control window, GorgonInputDeviceInfo keyboardName)
		{
			throw new NotImplementedException("This plug-in only contains XBOX 360 controller devices.");
		}

		/// <summary>
		/// Creates the pointing device impl.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="pointingDeviceName">Name of the pointing device.</param>
		/// <returns></returns>
		protected override GorgonPointingDevice CreatePointingDeviceImpl(Forms.Control window, GorgonInputDeviceInfo pointingDeviceName)
		{
			throw new NotImplementedException("This plug-in only contains XBOX 360 controller devices.");
		}

		/// <summary>
		/// Function to create a joystick interface.
		/// </summary>
		/// <param name="window">Window to bind with.</param>
		/// <param name="joystickName">A <see cref="GorgonLibrary.Input.GorgonInputDeviceInfo">GorgonInputDeviceInfo</see> object containing the joystick information.</param>
		/// <returns>A new joystick interface.</returns>
		/// <remarks>Pass NULL to the <paramref name="window"/> parameter to use the <see cref="P:GorgonLibrary.Gorgon.ApplicationForm">Gorgon application form</see>.</remarks>
		/// <exception cref="System.ArgumentNullException">The <paramRef name="joystickInfo"/> is NULL.</exception>
		protected override GorgonJoystick CreateJoystickImpl(Forms.Control window, GorgonInputDeviceInfo joystickName)
		{
			GorgonJoystick joystick = null;
			GorgonXInputDeviceInfo deviceName = joystickName as GorgonXInputDeviceInfo;

			if (deviceName == null)
				throw new ArgumentNullException("joystickInfo");

			joystick = new XInputController(this, deviceName.Index, joystickName.Name, window, deviceName.Controller);
			joystick.Enabled = true;

			return joystick;			
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonXInputFactory"/> class.
		/// </summary>
		public GorgonXInputFactory()
			: base("Gorgon.Input.XInput")
		{
		}
		#endregion
	}
}