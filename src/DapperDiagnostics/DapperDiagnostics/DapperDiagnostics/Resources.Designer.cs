﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DapperDiagnostics {
    using System;
    using System.Reflection;
    
    
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DapperDiagnostics.Resources", typeof(Resources).GetTypeInfo().Assembly);
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
        ///   Looks up a localized string similar to There are more properties in the param argument then there are in the SQL. This could result in additional, unused information being sent over the wire..
        /// </summary>
        internal static string MissingParametersDescription {
            get {
                return ResourceManager.GetString("MissingParametersDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The values are featured on the param argument, but not in the SQL: &apos;{0}&apos;.
        /// </summary>
        internal static string MissingParametersMessageFormat {
            get {
                return ResourceManager.GetString("MissingParametersMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQL is missing parameters.
        /// </summary>
        internal static string MissingParametersTitle {
            get {
                return ResourceManager.GetString("MissingParametersTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All parameters listed in the SQL must be in the params argument properties.
        /// </summary>
        internal static string MissingParamsPropertyDescription {
            get {
                return ResourceManager.GetString("MissingParamsPropertyDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing properties from param: &apos;{0}&apos;.
        /// </summary>
        internal static string MissingParamsPropertyMessageFormat {
            get {
                return ResourceManager.GetString("MissingParamsPropertyMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Params argument is missing properties.
        /// </summary>
        internal static string MissingParamsPropertyTitle {
            get {
                return ResourceManager.GetString("MissingParamsPropertyTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value names in the select statement don&apos;t match the properties on the return type..
        /// </summary>
        internal static string ReturnObjectMismatch_Properties_Description {
            get {
                return ResourceManager.GetString("ReturnObjectMismatch_Properties_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returned columns &apos;{0}&apos; don&apos;t have a matching property on &apos;{1}&apos;.
        /// </summary>
        internal static string ReturnObjectMismatch_Properties_MessageFormat {
            get {
                return ResourceManager.GetString("ReturnObjectMismatch_Properties_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The return type is a scalar, but more than one column is being returned..
        /// </summary>
        internal static string ReturnObjectMismatch_Scalar_Description {
            get {
                return ResourceManager.GetString("ReturnObjectMismatch_Scalar_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Return type is scalar, but {0} columns are being returned..
        /// </summary>
        internal static string ReturnObjectMismatch_Scalar_MessageFormat {
            get {
                return ResourceManager.GetString("ReturnObjectMismatch_Scalar_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The return type doesn&apos;t match the sql select statement.
        /// </summary>
        internal static string ReturnObjectMismatchTitle {
            get {
                return ResourceManager.GetString("ReturnObjectMismatchTitle", resourceCulture);
            }
        }
    }
}