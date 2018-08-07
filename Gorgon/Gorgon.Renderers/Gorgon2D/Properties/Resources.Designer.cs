﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Gorgon.Renderers.Properties {
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    internal class Resources {
        
        private static ResourceManager resourceMan;
        
        private static CultureInfo resourceCulture;
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if (ReferenceEquals(resourceMan, null)) {
                    ResourceManager temp = new ResourceManager("Gorgon.Renderers.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #define REJECT_ALPHA(alpha) if (alphaTestEnabled) clip((alpha &lt;= alphaTestValueHi &amp;&amp; alpha &gt;= alphaTestValueLow) ? -1 : 1);
        ///#define RANGE_BW(colorValue) (colorValue &lt; oneBitRange.x || colorValue &gt; oneBitRange.y) ? 0.0f : 1.0f;
        ///
        ///// Our default texture and sampler.
        ///Texture2DArray _gorgonTexture : register(t0);
        ///SamplerState _gorgonSampler : register(s0);
        ///
        ///// Additional effect texture buffer.
        ///Texture2D _gorgonEffectTexture : register(t1);
        ///
        ///// Our default sprite vertex.
        ///struct GorgonSpriteVertex
        ///{
        ///  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string BasicSprite {
            get {
                return ResourceManager.GetString("BasicSprite", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // This is adopted from the nvidia film grain shader.
        ///
        /// To learn more about shading, shaders, and to bounce ideas off other shader
        ///    authors and users, visit the NVIDIA Shader Library Forums at:
        ///
        ///    http://developer.nvidia.com/forums/
        ///
        ///#GorgonInclude &quot;Gorgon2DShaders&quot;
        ///
        /// Additional effect texture buffer.
        ///Texture2D _filmEffectTexture : register(t1);
        ///SamplerState _gorgonFilmGrainSampler : register(s1);		// Sampler used for film grain random texture.
        ///
        ///cbuffer FilmGrainTiming : regist [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string FilmGrain {
            get {
                return ResourceManager.GetString("FilmGrain", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1-bit Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_1BIT {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_1BIT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Draws an image with 1-bit color information (2 colors)..
        /// </summary>
        internal static string GOR2D_EFFECT_1BIT_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_1BIT_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Burn/Dodge Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_BURN_DODGE {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_BURN_DODGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applies a burn or dodge processing to the image being rendered..
        /// </summary>
        internal static string GOR2D_EFFECT_BURN_DODGE_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_BURN_DODGE_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Displacement Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_DISPLACEMENT {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_DISPLACEMENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Displaces pixels on a background image by weighting the pixels from another image over top..
        /// </summary>
        internal static string GOR2D_EFFECT_DISPLACEMENT_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_DISPLACEMENT_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Old Film Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_FILM {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_FILM", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Draws images to make it appear as though those images are from an old film reel..
        /// </summary>
        internal static string GOR2D_EFFECT_FILM_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_FILM_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gaussian Blur Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_GAUSS_BLUR {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_GAUSS_BLUR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Blurs a texture using a gaussian blurring technique and sends the blurred data to the effect output..
        /// </summary>
        internal static string GOR2D_EFFECT_GAUSS_BLUT_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_GAUSS_BLUT_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Grayscale Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_GRAYSCALE {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_GRAYSCALE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Renders sprites, primitives, etc... as grayscale..
        /// </summary>
        internal static string GOR2D_EFFECT_GRAYSCALE_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_GRAYSCALE_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invert Color Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_INVERT {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_INVERT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inverts the colors of an image..
        /// </summary>
        internal static string GOR2D_EFFECT_INVERT_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_INVERT_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Posterize Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_POSTERIZE {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_POSTERIZE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Performs a reduction in colors for an image..
        /// </summary>
        internal static string GOR2D_EFFECT_POSTERIZE_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_POSTERIZE_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sharpen/Emboss Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_SHARPEMBOSS {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_SHARPEMBOSS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sharpens or embosses an image..
        /// </summary>
        internal static string GOR2D_EFFECT_SHARPEMBOSS_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_SHARPEMBOSS_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sobel Edge Detection Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_SOBEL {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_SOBEL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Renders the edges of an image using a Sobel filter..
        /// </summary>
        internal static string GOR2D_EFFECT_SOBEL_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_SOBEL_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wave Effect.
        /// </summary>
        internal static string GOR2D_EFFECT_WAVE {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_WAVE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Causes a waving effect on a texture..
        /// </summary>
        internal static string GOR2D_EFFECT_WAVE_DESC {
            get {
                return ResourceManager.GetString("GOR2D_EFFECT_WAVE_DESC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Begin() method must be called prior to calling this method..
        /// </summary>
        internal static string GOR2D_ERR_BEGIN_NOT_CALLED {
            get {
                return ResourceManager.GetString("GOR2D_ERR_BEGIN_NOT_CALLED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot change the render target, depth/stencil or viewport during a Begin/End rendering block..
        /// </summary>
        internal static string GOR2D_ERR_CANNOT_CHANGE_STATE_INSIDE_BEGIN {
            get {
                return ResourceManager.GetString("GOR2D_ERR_CANNOT_CHANGE_STATE_INSIDE_BEGIN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The constant buffer slot must be 0 or less than {0}..
        /// </summary>
        internal static string GOR2D_ERR_CBUFFER_SLOT_INVALID {
            get {
                return ResourceManager.GetString("GOR2D_ERR_CBUFFER_SLOT_INVALID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The kernel size must be between 3 and 81 (inclusive)..
        /// </summary>
        internal static string GOR2D_ERR_EFFECT_BLUR_KERNEL_SIZE_INVALID {
            get {
                return ResourceManager.GetString("GOR2D_ERR_EFFECT_BLUR_KERNEL_SIZE_INVALID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The format [{0}] is not supported for render targets..
        /// </summary>
        internal static string GOR2D_ERR_EFFECT_BLUR_RENDER_TARGET_FORMAT_NOT_SUPPORTED {
            get {
                return ResourceManager.GetString("GOR2D_ERR_EFFECT_BLUR_RENDER_TARGET_FORMAT_NOT_SUPPORTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The format [{0}] is not supported for render targets..
        /// </summary>
        internal static string GOR2D_ERR_EFFECT_DISPLACEMENT_UNSUPPORTED_FORMAT {
            get {
                return ResourceManager.GetString("GOR2D_ERR_EFFECT_DISPLACEMENT_UNSUPPORTED_FORMAT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The sampler index must be 0 or less than {1}..
        /// </summary>
        internal static string GOR2D_ERR_INVALID_SAMPLER_INDEX {
            get {
                return ResourceManager.GetString("GOR2D_ERR_INVALID_SAMPLER_INDEX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not initialize the 2D renderer because there was no render target assigned to slot 0 on the graphics interface..
        /// </summary>
        internal static string GOR2D_ERR_NO_RTV {
            get {
                return ResourceManager.GetString("GOR2D_ERR_NO_RTV", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Begin() was called previously. Call End() prior to calling Begin() again..
        /// </summary>
        internal static string GOR2D_ERR_RENDER_ALREADY_STARTED {
            get {
                return ResourceManager.GetString("GOR2D_ERR_RENDER_ALREADY_STARTED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The shader resource slot must be 0 or less than {0}..
        /// </summary>
        internal static string GOR2D_ERR_SRV_SLOT_INVALID {
            get {
                return ResourceManager.GetString("GOR2D_ERR_SRV_SLOT_INVALID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static Bitmap Icons {
            get {
                object obj = ResourceManager.GetObject("Icons", resourceCulture);
                return ((Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #GorgonInclude &quot;Gorgon2DShaders&quot;
        ///
        ///// A texture array to allow access to the diffuse, normal map, and specular map.
        ///Texture2DArray _gorgonDiffSpecNormTexture : register(t0);
        ///
        ///// Texture and sampler used for normal mapping.
        ///Texture2D _gorgonNormalTexture : register(t1);
        ///SamplerState _gorgonNormalSampler : register(s1);
        ///
        ///// Texture and sampler used for specular mapping.
        ///Texture2D _gorgonSpecTexture : register(t2);
        ///SamplerState _gorgonSpecSampler : register(s2);
        ///
        ///// Output data from our vertex shad [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Lighting {
            get {
                return ResourceManager.GetString("Lighting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static Bitmap White_2x2 {
            get {
                object obj = ResourceManager.GetObject("White_2x2", resourceCulture);
                return ((Bitmap)(obj));
            }
        }
    }
}