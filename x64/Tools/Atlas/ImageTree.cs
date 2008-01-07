#region LGPL.
// 
// Atlas.
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
// Created: Tuesday, November 13, 2007 1:53:13 AM
// 
// Code in this section was derived from the OpenTK project by Stefan Apostolopoulos:
// http://opentk.sourceforge.net/
#endregion

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Atlas
{
	/// <summary>
	/// Object used to represent an image node.
	/// </summary>
	public class ImageNode
	{
		#region Variables.
		private ImageNode _left = null;						// Left node.
		private ImageNode _right = null;					// Right node.
		private ImageNode _parent = null;					// Parent node.		
		private Rectangle _imageRect = Rectangle.Empty;		// Image rectangle.
		private string _imagePath = string.Empty;			// Path to the image.
		private bool _dontAdd = false;						// Flag to indicate that we should not add.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to set or return the image rectangle.
		/// </summary>
		public Rectangle ImageRect
		{
			get
			{
				return _imageRect;
			}
			set
			{
				_imageRect = value;
			}
		}

		/// <summary>
		/// Property to return the path to the image that occupies this area.
		/// </summary>
		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
			set
			{
				_imagePath = value;
			}
		}

		/// <summary>
		/// Property to return the image node.
		/// </summary>
		public ImageNode Parent
		{
			get
			{
				return _parent;
			}
		}

		/// <summary>
		/// Property to set or return the left child node.
		/// </summary>
		public ImageNode Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
			}
		}

		/// <summary>
		/// Property to set or return the right child node.
		/// </summary>
		public ImageNode Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;
			}
		}

		/// <summary>
		/// Property to return whether this node is a leaf node or not.
		/// </summary>
		public bool IsLeaf
		{
			get
			{
				return ((_right == null) && (_left == null));
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to add a node to this node.
		/// </summary>
		/// <param name="path">Path of the image.</param>
		/// <param name="dimensions">Dimensions of the image.</param>
		/// <param name="padding">Padding amount.</param>
		/// <returns>A new image node.</returns>
		public ImageNode Add(string path, Size dimensions)
		{
			Size delta = Size.Empty;		// Delta width & height.
			
			// If not a leaf node, then keep going.
			if (!IsLeaf)
			{
				ImageNode newNode = null;

				newNode = _left.Add(path, dimensions);

				return newNode ?? _right.Add(path, dimensions);
			}

			// Don't add if not empty.
			if (_dontAdd)
				return null;

			// Ensure we can fit this image.
			if ((dimensions.Width > _imageRect.Width) || (dimensions.Height  > _imageRect.Height))
				return null;

			// Exact fit.
			if ((dimensions.Width  == _imageRect.Width) && (dimensions.Height == _imageRect.Height))
			{
				_dontAdd = true;
				return this;
			}

			// Set left and right nodes.
			_left = new ImageNode(this);
			_right = new ImageNode(this);

			delta.Width = ImageRect.Width - dimensions.Width;
			delta.Height = ImageRect.Height - dimensions.Height;

			if (delta.Width > delta.Height)
			{
				_left.ImageRect = new Rectangle(ImageRect.Left, ImageRect.Top, dimensions.Width, ImageRect.Height);
				_right.ImageRect = new Rectangle(ImageRect.Left + dimensions.Width, ImageRect.Top, delta.Width, ImageRect.Height);
			}
			else
			{
				_left.ImageRect = new Rectangle(ImageRect.Left, ImageRect.Top, ImageRect.Width, dimensions.Height);
				_right.ImageRect = new Rectangle(ImageRect.Left, ImageRect.Top + dimensions.Height, ImageRect.Width, delta.Height);
			}
			
			return _left.Add(path, dimensions);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageNode"/> class.
		/// </summary>
		/// <param name="parent">The parent of this node, NULL for root.</param>
		public ImageNode(ImageNode parent)
		{
			_parent = parent;
		}
		#endregion
	}

	/// <summary>
	/// Object used to represent a spatial tree of images.
	/// </summary>
	public class ImageTree
	{
		#region Variables.
		private ImageNode _root = null;				// Root image node.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to return the root image node.
		/// </summary>
		public ImageNode Root
		{
			get
			{
				return _root;
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to add an image to the tree.
		/// </summary>
		/// <param name="imagePath">Source path to the image.</param>
		/// <param name="imageDims">Image dimensions.</param>
		/// <returns>A rectangle for the item.</returns>
		public Rectangle Add(string imagePath, Size imageDims)
		{
			ImageNode newNode = null;		// New node.

			if ((imageDims.Width > _root.ImageRect.Width) || (imageDims.Height > _root.ImageRect.Height))
				throw new OverflowException("The atlas texture is too small to support '" + imagePath + "'.");

			newNode = _root.Add(imagePath, imageDims);

			if (newNode == null)
				return Rectangle.Empty;

			return newNode.ImageRect;
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageTree"/> class.
		/// </summary>
		/// <param name="atlasSize">Size of the image atlas.</param>
		public ImageTree(Size atlasSize)
		{
			_root = new ImageNode(null);
			_root.ImagePath = string.Empty;
			_root.ImageRect = new Rectangle(new Point(0, 0), atlasSize);
		}
		#endregion
	}
}
