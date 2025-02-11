﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Streetcode.BLL.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Streetcode.BLL.Resources.ErrorResources", typeof(ErrorResources).Assembly);
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
        ///   Looks up a localized string similar to Cannot find a {0} with corresponding id: {1}.
        /// </summary>
        internal static string CantFindByIdError {
            get {
                return ResourceManager.GetString("CantFindByIdError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find a {0} by a streetcode id: {1}.
        /// </summary>
        internal static string CantFindByStreetcodeIdError {
            get {
                return ResourceManager.GetString("CantFindByStreetcodeIdError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find any {0} by Url: {1}.
        /// </summary>
        internal static string CantFindByURLError {
            get {
                return ResourceManager.GetString("CantFindByURLError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find any {0}.
        /// </summary>
        internal static string CantFindError {
            get {
                return ResourceManager.GetString("CantFindError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot convert {0} to {1}.
        /// </summary>
        internal static string ConvertationError {
            get {
                return ResourceManager.GetString("ConvertationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to create a {0}.
        /// </summary>
        internal static string FailCreateError {
            get {
                return ResourceManager.GetString("FailCreateError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to delete a {0}.
        /// </summary>
        internal static string FailDeleteError {
            get {
                return ResourceManager.GetString("FailDeleteError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to update a {0}.
        /// </summary>
        internal static string FailUpdateError {
            get {
                return ResourceManager.GetString("FailUpdateError", resourceCulture);
            }
        }
    }
}
