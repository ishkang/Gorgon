﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2013 Michael Winsor
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
// Created: Saturday, October 12, 2013 10:08:08 PM
// 
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using GorgonLibrary.Math;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// A brush used to draw glyphs using a linear gradient value.
	/// </summary>
	public class GorgonGlyphLinearGradientBrush
		: GorgonGlyphBrush
	{
		#region Properties.
		/// <summary>
		/// Property to return the type of brush.
		/// </summary>
		public override GlyphBrushType BrushType
		{
			get
			{
				return GlyphBrushType.LinearGradient;
			}
		}

		/// <summary>
		/// Property to set or return the wrapping mode for the gradient fill.
		/// </summary>
		public WrapMode WrapMode
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the region for a single gradient run.
		/// </summary>
		public RectangleF GradientRegion
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the starting color to use for the gradient.
		/// </summary>
		public GorgonColor StartColor
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the ending color to use for the gradient.
		/// </summary>
		public GorgonColor EndColor
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the angle, in degrees, for the gradient.
		/// </summary>
		public float Angle
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return whether to scale the angle.
		/// </summary>
		public bool ScaleAngle
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return whether to enable or disable gamma correction.
		/// </summary>
		public bool GammaCorrection
		{
			get;
			set;
		}

		/// <summary>
		/// Property to return the linear colors for the gradient.
		/// </summary>
		public IList<GorgonColor> LinearColors 
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to set or return the interpolation colors for the gradient.
		/// </summary>
		public IList<GorgonColor> InterpolationColors
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to set or return the interpolation weights for the gradient.
		/// </summary>
		public IList<float> InterpolationWeights
		{
			get;
			private set;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to convert this brush to the equivalent GDI+ brush type.
		/// </summary>
		/// <returns>
		/// The GDI+ brush type for this object.
		/// </returns>
		internal override Brush ToGDIBrush()
		{
			var interpColors = new ColorBlend(InterpolationColors.Count.Max(InterpolationWeights.Count).Max(1));

			for (int i = 0; i < interpColors.Colors.Length; i++)
			{
				if (i < InterpolationColors.Count)
				{
					interpColors.Colors[i] = InterpolationColors[i];
				}

				if (i < InterpolationWeights.Count)
				{
					interpColors.Positions[i] = InterpolationWeights[i];
				}
			}

			var result = new LinearGradientBrush(GradientRegion, StartColor, EndColor, Angle, ScaleAngle)
			             {
				             GammaCorrection = GammaCorrection,
							 WrapMode = WrapMode
			             };

			if (LinearColors.Count > 0)
			{
				result.LinearColors = LinearColors.Select(item => item.ToColor()).ToArray();
			}

			if (interpColors.Colors.Length > 1)
			{
				result.InterpolationColors = interpColors;
			}

			return result;
		}

		/// <summary>
		/// Function to write the brush elements out to a chunked file.
		/// </summary>
		/// <param name="chunk">Chunk writer used to persist the data.</param>
		internal override void Write(IO.GorgonChunkWriter chunk)
		{
			chunk.Begin("BRSHDATA");
			chunk.Write(BrushType);
			chunk.Write(WrapMode);
			chunk.Write(GammaCorrection);
			chunk.Write(Angle);
			chunk.Write(ScaleAngle);
			chunk.Write(StartColor);
			chunk.Write(EndColor);

			chunk.Write(LinearColors.Count);
			
			foreach (GorgonColor color in LinearColors)
			{
				chunk.Write(color);
			}

			chunk.Write(InterpolationColors.Count);

			foreach (GorgonColor color in InterpolationColors)
			{
				chunk.Write(color);
			}

			chunk.Write(InterpolationWeights.Count);

			foreach (float weight in InterpolationWeights)
			{
				chunk.Write(weight);
			}

			chunk.End();
		}

		/// <summary>
		/// Function to read the brush elements in from a chunked file.
		/// </summary>
		/// <param name="chunk">Chunk reader used to read the data.</param>
		internal override void Read(IO.GorgonChunkReader chunk)
		{
			InterpolationWeights.Clear();
			InterpolationColors.Clear();
			LinearColors.Clear();

			WrapMode = chunk.Read<WrapMode>();
			GammaCorrection = chunk.ReadBoolean();
			Angle = chunk.ReadFloat();
			ScaleAngle = chunk.ReadBoolean();
			StartColor = chunk.Read<GorgonColor>();
			EndColor = chunk.Read<GorgonColor>();

			int counter = chunk.ReadInt32();

			for (int i = 0; i < counter; i++)
			{
				LinearColors.Add(chunk.Read<GorgonColor>());
			}

			counter = chunk.ReadInt32();

			for (int i = 0; i < counter; i++)
			{
				InterpolationColors.Add(chunk.Read<GorgonColor>());
			}

			counter = chunk.ReadInt32();

			for (int i = 0; i < counter; i++)
			{
				InterpolationWeights.Add(chunk.ReadFloat());
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonGlyphPathGradientBrush"/> class.
		/// </summary>
		public GorgonGlyphLinearGradientBrush()
		{
			InterpolationColors = new List<GorgonColor>();
			InterpolationWeights = new List<float>();
			LinearColors = new List<GorgonColor>();
		}
		#endregion
	}
}