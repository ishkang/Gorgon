#region LGPL.
// 
// Gorgon.
// Copyright (C) 2007 Michael Winsor
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
// Created: Friday, October 26, 2007 11:55:49 PM
// 
#endregion

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("You were framed")]
[assembly: AssemblyDescription("Example showing how to use the framework to make life a little easier.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Michael Winsor")]
[assembly: AssemblyProduct("Gorgon Framework Example.")]
[assembly: AssemblyCopyright("Copyright © Michael Winsor 2007")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]
[assembly: FileIOPermission(SecurityAction.RequestMinimum, AllFiles = FileIOPermissionAccess.AllAccess)]
[assembly: ReflectionPermission(SecurityAction.RequestMinimum, Flags = ReflectionPermissionFlag.MemberAccess)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("9db6bac3-0b0d-437f-b7e8-2fa6ce2bb33d")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersionAttribute("1.0.*")]
[assembly: AssemblyFileVersionAttribute("1.0.0.0")]
