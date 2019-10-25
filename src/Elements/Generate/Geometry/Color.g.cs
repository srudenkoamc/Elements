//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.0.24.0 (Newtonsoft.Json v9.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------

using Elements.GeoJSON;
using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elements.Geometry
{
    #pragma warning disable // Disable all warnings

    /// <summary>An RGBA color.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.24.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Color 
    {
        /// <summary>The red component of the color between 0.0 and 1.0.</summary>
        [Newtonsoft.Json.JsonProperty("Red", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Range(0.0D, 1.0D)]
        public  double Red { get; internal set; }
    
        /// <summary>The green component of the color between 0.0 and 1.0..</summary>
        [Newtonsoft.Json.JsonProperty("Green", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Range(0.0D, 1.0D)]
        public  double Green { get; internal set; }
    
        /// <summary>The blue component of the color between 0.0 and 1.0..</summary>
        [Newtonsoft.Json.JsonProperty("Blue", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Range(0.0D, 1.0D)]
        public  double Blue { get; internal set; }
    
        /// <summary>The alpha component of the color between 0.0 and 1.0..</summary>
        [Newtonsoft.Json.JsonProperty("Alpha", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Range(0.0D, 1.0D)]
        public  double Alpha { get; internal set; }
    
    
    }
}