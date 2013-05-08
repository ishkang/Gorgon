// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#joyGetThreshold(System.Int32,System.Int32&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#joyConfigChanged(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#joyGetNumDevs()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#joyGetDevCaps(System.Int32,GorgonLibrary.Native.JOYCAPS&,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#joyGetPos(System.Int32,GorgonLibrary.Native.JOYINFO&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#joyGetPosEx(System.Int32,GorgonLibrary.Native.JOYINFOEX&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#GetRawInputData(System.IntPtr,GorgonLibrary.Native.RawInputCommand,System.IntPtr,System.Int32&,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#GetRawInputDeviceList(System.IntPtr,System.Int32&,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#GetRawInputDeviceInfo(System.IntPtr,System.Int32,System.IntPtr,System.Int32&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#RegisterRawInputDevices(GorgonLibrary.Native.RAWINPUTDEVICE[],System.Int32,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1414:MarkBooleanPInvokeArgumentsWithMarshalAs", Scope = "member", Target = "GorgonLibrary.Native.Win32API.#RegisterRawInputDevices(GorgonLibrary.Native.RAWINPUTDEVICE[],System.Int32,System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Scope = "member", Target = "GorgonLibrary.Input.Raw.RawInputData.#_rawData")]
