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
// Created: September 4, 2018 10:48:10 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gorgon.Core;
using Gorgon.Editor.Metadata;
using Gorgon.Editor.Plugins;
using Gorgon.Editor.Properties;
using Gorgon.Editor.Services;
using Gorgon.IO;

namespace Gorgon.Editor.ViewModels
{
    /// <summary>
    /// A node for a file system directory.
    /// </summary>
    internal class FileExplorerDirectoryNodeVm
        : FileExplorerNodeCommon
    {
        #region Properties.
        /// <summary>
        /// Property to return whether to allow child node creation for this node.
        /// </summary>
        public override bool AllowChildCreation => true;

        /// <summary>
        /// Property to return the type of node.
        /// </summary>
        public override NodeType NodeType => NodeType.Directory;

        /// <summary>
        /// Property to return the full path to the node.
        /// </summary>
        public override string FullPath => Parent == null ? "/" : (Parent.FullPath + Name).FormatDirectory('/');

        /// <summary>
        /// Property to return the image name to use for the node type.
        /// </summary>
        public override string ImageName => "folder_20x20.png";

        /// <summary>
        /// Property to return whether or not the allow this node to be deleted.
        /// </summary>
        public override bool AllowDelete => true;

        /// <summary>Property to return whether this node represents content or not.</summary>
        public override bool IsContent => false;
        #endregion

        #region Methods.
        /// <summary>
        /// Function to allow a user to resolve a confict between files with the same name.
        /// </summary>
        /// <param name="sourceItem">The file being copied.</param>
        /// <param name="destItem">The destination file.</param>
        /// <param name="usePhysicalPath"><b>true</b> to display the physical path for the destination, or <b>false</b> to display the virtual path.</param>
        /// <returns>A <see cref="FileSystemConflictResolution"/> value that indicates how to proceed.</returns>
        private FileSystemConflictResolution ResolveExportConflict(FileSystemInfo sourceItem, FileSystemInfo destItem)
        {
            MessageResponse response = MessageDisplay.ShowConfirmation(string.Format(Resources.GOREDIT_CONFIRM_FILE_EXISTS, sourceItem.Name, destItem.FullName), toAll: true, allowCancel: true);

            switch (response)
            {
                case MessageResponse.Yes:
                    return FileSystemConflictResolution.Overwrite;
                case MessageResponse.YesToAll:
                    return FileSystemConflictResolution.OverwriteAll;
                case MessageResponse.No:
                    return FileSystemConflictResolution.Rename;
                case MessageResponse.NoToAll:
                    return FileSystemConflictResolution.RenameAll;
                default:
                    return FileSystemConflictResolution.Cancel;
            }
        }

        /// <summary>
        /// Function to allow a user to resolve a confict between files with the same name.
        /// </summary>
        /// <param name="sourceItem">The file being copied.</param>
        /// <param name="destItem">The destination file.</param>
        /// <param name="usePhysicalPath"><b>true</b> to display the physical path for the destination, or <b>false</b> to display the virtual path.</param>
        /// <returns>A <see cref="FileSystemConflictResolution"/> value that indicates how to proceed.</returns>
        private FileSystemConflictResolution ResolveConflict(FileSystemInfo sourceItem, FileSystemInfo destItem)
        {
            IFileExplorerNodeVm root = GetRoot();

            Debug.Assert(root != null, "Root is null");

            var rootDir = new DirectoryInfo(root.PhysicalPath);

            MessageResponse response = MessageDisplay.ShowConfirmation(string.Format(Resources.GOREDIT_CONFIRM_FILE_EXISTS, sourceItem.Name, destItem.ToFileSystemPath(rootDir)), toAll: true, allowCancel: true);
            
            switch (response)
            {
                case MessageResponse.Yes:
                    return FileSystemConflictResolution.Overwrite;
                case MessageResponse.YesToAll:
                    return FileSystemConflictResolution.OverwriteAll;
                case MessageResponse.No:
                    return FileSystemConflictResolution.Rename;
                case MessageResponse.NoToAll:
                    return FileSystemConflictResolution.RenameAll;
                default:
                    return FileSystemConflictResolution.Cancel;
            }
        }

        /// <summary>
        /// Function to delete the node.
        /// </summary>
        /// <param name="onDeleted">[Optional] A function to call when a node or a child node is deleted.</param>
        /// <param name="cancelToken">[Optional] A cancellation token used to cancel the operation.</param>
        /// <returns>A task for asynchronous operation.</returns>
        /// <remarks>
        /// <para>
        /// The <paramref name="onDeleted"/> parameter passes a file system information that contains name of the node being deleted, so callers can use that information for their own purposes.
        /// </para>
        /// </remarks>
        public override async Task DeleteNodeAsync(Action<FileSystemInfo> onDeleted = null, CancellationToken? cancelToken = null)
        {
            // Delete the physical objects first. If we fail here, our node will survive.
            // We do this asynchronously because deleting a directory with a lot of files may take a while.
            bool dirDeleted = await Task.Run(() => FileSystemService.DeleteDirectory(PhysicalPath, onDeleted, cancelToken ?? CancellationToken.None));

            // If, for some reason, our directory was not deleted, then do not remove the node.
            if (!dirDeleted)
            {
                return;
            }

            // Drop us from the parent list.
            // This will begin a chain reaction that will remove us from the UI.
            Parent.Children.Remove(this);
            Parent = null;
        }

        /// <summary>
        /// Function to rename the node.
        /// </summary>
        /// <param name="newName">The new name for the node.</param>
        /// <param name="projectItems">The list of items in the project.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="newName"/>, or the <paramref name="projectItems"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="newName"/> parameter is empty.</exception>
        public override void RenameNode(string newName, IDictionary<string, ProjectItemMetadata> projectItems)
        {
            if (newName == null)
            {
                throw new ArgumentNullException(nameof(newName));
            }

            if (projectItems == null)
            {
                throw new ArgumentNullException(nameof(projectItems));
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentEmptyException(nameof(newName));
            }

            // Remove the previous project item.
            projectItems.Remove(FullPath);

            PhysicalPath = FileSystemService.RenameDirectory(PhysicalPath, newName);
            Name = newName;

            // Update with the new path.
            if (Metadata != null)
            {
                projectItems[FullPath] = Metadata;
            }
            
            NotifyPropertyChanged(nameof(FullPath));
        }

        /// <summary>
        /// Function to copy this node to another node.
        /// </summary>
        /// <param name="newPath">The node that will receive the the copy of this node.</param>
        /// <param name="onCopy">[Optional] The method to call when a file is about to be copied.</param>
        /// <param name="cancelToken">[Optional] A token used to cancel the operation.</param>
        /// <returns>The new node for the copied node.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the the <paramref name="destNode"/> parameter is <b>null</b>.</exception>
        /// <exception cref="GorgonException">Thrown if the <paramref name="destNode"/> is unable to create child nodes.</exception>
        /// <remarks>
        /// <para>
        /// The <paramref name="onCopy" /> callback method sends the file system item being copied, the destination file system item, the current item #, and the total number of items to copy.
        /// </para>
        /// </remarks>
        public override async Task<IFileExplorerNodeVm> CopyNodeAsync(IFileExplorerNodeVm destNode, Action<FileSystemInfo, FileSystemInfo, int, int> onCopy, CancellationToken? cancelToken = null)
        {
            if (destNode == null)
            {
                throw new ArgumentNullException(nameof(destNode));
            }

            string name = Name;
            string newPath = destNode.PhysicalPath.FormatDirectory(Path.DirectorySeparatorChar);
            string newPhysicalPath = Path.Combine(newPath, Name);
            IFileExplorerNodeVm dupeNode = null;

            // Find out if this node already exists. We'll have to get rid of it.
            dupeNode = destNode.Children.FirstOrDefault(item => string.Equals(newPath, item.PhysicalPath, StringComparison.OrdinalIgnoreCase));

            try
            {
                var args = new CopyDirectoryArgs(PhysicalPath, newPath)
                {
                    OnCopyProgress = onCopy,
                    OnResolveConflict = ResolveConflict
                };
                bool copied = await FileSystemService.CopyDirectoryAsync(args, cancelToken ?? CancellationToken.None);

                if (!copied)
                {
                    return null;
                }                
            }
            catch (Exception ex)
            {
                MessageDisplay.ShowError(ex);
            }

            // This can happen if an exception occurs if we fail to create any directory.
            if (!Directory.Exists(newPhysicalPath))
            {
                return null;
            }

            if (dupeNode != null)
            {
                dupeNode.Parent.Children.Remove(dupeNode);
                dupeNode = null;
            }

            return new FileExplorerDirectoryNodeVm(this)
            {
                IsExpanded = false,
                Name = name,
                Parent = destNode,
                PhysicalPath = newPhysicalPath,
                Metadata = new ProjectItemMetadata(Metadata)
            };
        }

        /// <summary>
        /// Function to move this node to another node.
        /// </summary>
        /// <param name="destNode">The dest node.</param>
        /// <returns>The new node for the copied node.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="destNode"/> parameter is <b>null</b>.</exception>
        /// <exception cref="GorgonException">Thrown if the <paramref name="destNode"/> is incapable of creating child nodes.</exception>
        public override IFileExplorerNodeVm MoveNode(IFileExplorerNodeVm destNode)
        {
            if (destNode == null)
            {
                throw new ArgumentNullException(nameof(destNode));
            }

            if (destNode == null)
            {
                throw new GorgonException(GorgonResult.CannotCreate, string.Format(Resources.GOREDIT_ERR_NODE_CANNOT_CREATE_CHILDREN, destNode.Name));
            }

            if (string.Equals(Parent.FullPath, destNode.FullPath, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string newPath = Path.Combine(destNode.PhysicalPath, Name);

            IFileExplorerNodeVm result = new FileExplorerDirectoryNodeVm(this)
            {
                IsExpanded = false,
                Name = Name,
                Parent = destNode,
                PhysicalPath = newPath,
                Metadata = Metadata
            };

            FileSystemService.MoveDirectory(PhysicalPath, result.PhysicalPath);

            return result;
        }

        /// <summary>
        /// Function to export the contents of this node to the physical file system.
        /// </summary>
        /// <param name="destPath">The path to the directory on the physical file system that will receive the contents.</param>
        /// <param name="onCopy">[Optional] The method to call when a file is about to be copied.</param>
        /// <param name="cancelToken">[Optional] A token used to cancel the operation.</param>
        /// <returns>A task for asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the the <paramref name="destPath"/> parameter is <b>null</b>.</exception>
        /// <exception cref="GorgonException">Thrown if the <paramref name="destPath"/> is unable to create child nodes.</exception>
        /// <remarks>
        /// <para>
        /// The <paramref name="onCopy" /> callback method sends the file system item being copied, the destination file system item, the current item #, and the total number of items to copy.
        /// </para>
        /// </remarks>
        public override Task ExportAsync(string destPath, Action<FileSystemInfo, FileSystemInfo, int, int> onCopy, CancellationToken? cancelToken = null)
        {
            if (destPath == null)
            {
                throw new ArgumentNullException(nameof(destPath));
            }

            if (string.IsNullOrWhiteSpace(destPath))
            {
                throw new ArgumentEmptyException(nameof(destPath));
            }

            var args = new CopyDirectoryArgs(PhysicalPath, destPath)
            {
                OnCopyProgress = onCopy,
                OnResolveConflict = ResolveExportConflict
            };

            return FileSystemService.ExportDirectoryAsync(args, cancelToken ?? CancellationToken.None);
        }
        #endregion

        #region Constructor/Finalizer.
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExplorerDirectoryNodeVm"/> class.
        /// </summary>
        /// <param name="copy">The node to copy.</param>
        internal FileExplorerDirectoryNodeVm(FileExplorerDirectoryNodeVm copy)
            : base(copy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExplorerDirectoryNodeVm" /> class.
        /// </summary>
        public FileExplorerDirectoryNodeVm()
        {
        }
        #endregion
    }
}