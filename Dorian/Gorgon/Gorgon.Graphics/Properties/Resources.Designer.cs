﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GorgonLibrary.Graphics.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GorgonLibrary.Graphics.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to These image settings are not compatible with texture settings..
        /// </summary>
        internal static string GOR_GFX_IMAGE_SETTINGS_NOT_TEXTURE_SETTINGS {
            get {
                return ResourceManager.GetString("GOR_GFX_IMAGE_SETTINGS_NOT_TEXTURE_SETTINGS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The buffer is already locked..
        /// </summary>
        internal static string GORGFX_ALREADY_LOCKED {
            get {
                return ResourceManager.GetString("GORGFX_ALREADY_LOCKED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This buffer is immutable and this cannot be updated..
        /// </summary>
        internal static string GORGFX_BUFFER_IMMUTABLE {
            get {
                return ResourceManager.GetString("GORGFX_BUFFER_IMMUTABLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The size of the buffers do not match..
        /// </summary>
        internal static string GORGFX_BUFFER_SIZE_MISMATCH {
            get {
                return ResourceManager.GetString("GORGFX_BUFFER_SIZE_MISMATCH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Color Value: Red={0}, Green={1}, Blue={2}, Alpha={3}..
        /// </summary>
        internal static string GORGFX_COLOR_TOSTR {
            get {
                return ResourceManager.GetString("GORGFX_COLOR_TOSTR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Must supply a known feature level..
        /// </summary>
        internal static string GORGFX_FEATURE_LEVEL_UNKNOWN {
            get {
                return ResourceManager.GetString("GORGFX_FEATURE_LEVEL_UNKNOWN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image pitch information.  Width={0} bytes, Size={1} bytes.  Format is compressed. Block count width: {2}, Block count height: {3}.
        /// </summary>
        internal static string GORGFX_FMTPITCH_COMPRESSED_TOSTR {
            get {
                return ResourceManager.GetString("GORGFX_FMTPITCH_COMPRESSED_TOSTR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Image pitch information.  Width={0} bytes, Size={1} bytes..
        /// </summary>
        internal static string GORGFX_FMTPITCH_TOSTR {
            get {
                return ResourceManager.GetString("GORGFX_FMTPITCH_TOSTR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The index is out of range.  The index value [{0}] must be be 0 or less than {1}..
        /// </summary>
        internal static string GORGFX_INDEX_OUT_OF_RANGE {
            get {
                return ResourceManager.GetString("GORGFX_INDEX_OUT_OF_RANGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Gorgon Graphics interface requires Windows Vista Service Pack 2 or greater..
        /// </summary>
        internal static string GORGFX_INVALID_OS {
            get {
                return ResourceManager.GetString("GORGFX_INVALID_OS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The usage for this object must be set to &apos;Default&apos;..
        /// </summary>
        internal static string GORGFX_NOT_DEFAULT_USAGE {
            get {
                return ResourceManager.GetString("GORGFX_NOT_DEFAULT_USAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A buffer with a usage of &apos;{0}&apos; cannot be locked..
        /// </summary>
        internal static string GORGFX_USAGE_CANT_LOCK {
            get {
                return ResourceManager.GetString("GORGFX_USAGE_CANT_LOCK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Gorgon_2_x_Logo_Small {
            get {
                object obj = ResourceManager.GetObject("Gorgon_2_x_Logo_Small", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
