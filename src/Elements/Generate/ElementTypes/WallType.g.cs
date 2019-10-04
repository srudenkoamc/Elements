//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.0.24.0 (Newtonsoft.Json v9.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------

namespace Elements.ElementTypes
{
    #pragma warning disable // Disable all warnings

    using Elements.ElementTypes;
    using Elements.GeoJSON;
    using Elements.Geometry;
    using Elements.Geometry.Solids;
    using Elements.Properties;
    using Elements.Serialization.JSON;
    using System;
    using System.Collections.Generic;
    
    /// <summary>A container for properties common to walls.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.24.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class WallType : ElementType
    {
        /// <summary>The material layers of the wall.</summary>
        [Newtonsoft.Json.JsonProperty("MaterialLayers", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public  IList<MaterialLayer> MaterialLayers { get; internal set; } = new List<MaterialLayer>();
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new Newtonsoft.Json.JsonConverter[] { new ModelConverter() });
        }
    
        public static WallType FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<WallType>(data, new Newtonsoft.Json.JsonConverter[] { new ModelConverter() });
        }
    
    }
}