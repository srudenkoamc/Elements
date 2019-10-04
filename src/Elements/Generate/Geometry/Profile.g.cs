//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.0.24.0 (Newtonsoft.Json v9.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------

namespace Elements.Geometry
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
    
    /// <summary>A profile comprised of an external boundary and one or several holes.</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.0.24.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Profile 
    {
        /// <summary>An identifier for the entity which must be unique within the system.</summary>
        [Newtonsoft.Json.JsonProperty("Id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public  System.Guid Id { get; internal set; }
    
        /// <summary>The entity's name.</summary>
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.AllowNull)]
        public  string Name { get; internal set; }
    
        /// <summary>The perimeter of the profile.</summary>
        [Newtonsoft.Json.JsonProperty("Perimeter", Required = Newtonsoft.Json.Required.AllowNull)]
        public  Polygon Perimeter { get; internal set; }
    
        /// <summary>A collection of Polygons representing voids in the profile.</summary>
        [Newtonsoft.Json.JsonProperty("Voids", Required = Newtonsoft.Json.Required.AllowNull)]
        public  IList<Polygon> Voids { get; internal set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new Newtonsoft.Json.JsonConverter[] { new ModelConverter() });
        }
    
        public static Profile FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Profile>(data, new Newtonsoft.Json.JsonConverter[] { new ModelConverter() });
        }
    
    }
}