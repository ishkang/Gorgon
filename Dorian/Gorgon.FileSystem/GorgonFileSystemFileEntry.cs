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
// Created: Monday, June 27, 2011 8:59:25 AM
// 
#endregion

using System;
using System.IO;

namespace GorgonLibrary.FileSystem
{
	/// <summary>
	/// A file entry corresponding to a file on the physical file system.
	/// </summary>
	public class GorgonFileSystemFileEntry
		: GorgonNamedObject
	{
		#region Properties.
		/// <summary>
		/// Property to return the file system that owns this file.
		/// </summary>
		public GorgonFileSystemProvider Provider
		{
			get;
			internal set;
		}

		/// <summary>
		/// Property to return the full path to the file in the virtual file system.
		/// </summary>
		public string FullPath
		{
			get
			{
				if (Directory == null)
					return Name;
				return Directory.FullPath + Name;
			}
		}

		/// <summary>
		/// Property to return the path to the file on the physical file system.
		/// </summary>
		public string PhysicalFileSystemPath
		{
			get;
			internal set;
		}

		/// <summary>
		/// Property to return the mount point that holds the file.
		/// </summary>
		public string MountPoint
		{
			get;
			internal set;
		}

		/// <summary>
		/// Property to return the directory that owns this file.
		/// </summary>
		public GorgonFileSystemDirectory Directory
		{
			get;
			internal set;
		}

		/// <summary>
		/// Property to return the extension
		/// </summary>
		public string Extension
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to return the filename without the extension.
		/// </summary>
		public string BaseFilename
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to return the uncompressed size of the file in bytes.
		/// </summary>
		public long Size
		{
			get;
			internal set;
		}

		/// <summary>
		/// Property to return the file creation date.
		/// </summary>
		public DateTime CreateDate
		{
			get;
			internal set;
		}

		/// <summary>
		/// Property to return the offset of the file within a packed file.
		/// </summary>
		/// <remarks>This will always return 0 for a folder file system.</remarks>
		public long Offset
		{
			get;
			internal set;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to read the file.
		/// </summary>
		/// <returns>An array of bytes containing the file data.</returns>
		public byte[] Read()
		{
			return Provider.ReadFile(this);
		}

		/// <summary>
		/// Function to write to the file.
		/// </summary>
		/// <param name="data">The data to write to the file.</param>
		public void Write(byte[] data)
		{
			Provider.WriteFile(this, data);
		}

		/// <summary>
		/// Function to open a stream to the file on the physical file system.
		/// </summary>
		/// <param name="writeable">TRUE to write to the file, FALSE to make read-only.</param>
		/// <returns>The open <see cref="GorgonLibrary.FileSystem.GorgonFileSystemStream"/> file stream object.</returns>
		public GorgonFileSystemStream OpenStream(bool writeable)
		{
			return Provider.OpenStream(this, writeable);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonFileSystemFileEntry"/> class.
		/// </summary>
		/// <param name="provider">The file system provider that owns this file.</param>
		/// <param name="directory">The directory that holds this file.</param>
		/// <param name="fileName">The file name of the file.</param>
		/// <param name="mountPoint">The mount point that holds the file.</param>
		/// <param name="physicalPath">Path to the file on the physical file system.</param>
		/// <param name="fileSize">Size of the file in bytes.</param>
		/// <param name="offset">Offset of the file within a packed file.</param>
		/// <param name="createDate">Create date for the file.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="directory"/> or <paramref name="provider"/> parameter is NULL (or Nothing in VB.NET).</exception>
		internal GorgonFileSystemFileEntry(GorgonFileSystemProvider provider, GorgonFileSystemDirectory directory, string fileName, string mountPoint, string physicalPath, long fileSize, long offset, DateTime createDate)
			: base(GorgonUtility.RemoveIllegalFilenameChars(fileName))
		{
			GorgonUtility.AssertParamString(mountPoint, "mountPoint");

			if (provider == null)
				throw new ArgumentNullException("provider");
			if (directory == null)
				throw new ArgumentNullException("directory");

			Provider = provider;
			Directory = directory;
			Extension = Path.GetExtension(Name);
			BaseFilename = Path.GetFileNameWithoutExtension(Name);
			if (string.IsNullOrEmpty(physicalPath))
			{
				PhysicalFileSystemPath = Name;
				MountPoint = mountPoint;
			}
			else
			{
				MountPoint = mountPoint;
				PhysicalFileSystemPath = physicalPath;
			}

			if (string.IsNullOrEmpty(BaseFilename)) 
				throw new IOException("There was no file name.");

			Offset = offset;
			Size = fileSize;
			CreateDate = createDate;
		}
		#endregion
	}
}
