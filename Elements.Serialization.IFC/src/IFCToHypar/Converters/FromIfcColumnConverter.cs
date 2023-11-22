﻿using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements.Serialization.IFC.IFCToHypar.RepresentationsExtraction;
using IFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elements.Serialization.IFC.IFCToHypar.Converters
{
    internal class FromIfcColumnConverter : IFromIfcProductConverter
    {
        public GeometricElement ConvertToElement(IfcProduct ifcProduct, RepresentationData repData, List<string> constructionErrors)
        {
            if (ifcProduct is not IfcColumn ifcColumn)
            {
                return null;
            }

            if (repData.Extrude == null)
            {
                constructionErrors.Add($"#{ifcProduct.StepId}: Conversion of IfcColumn without extrude or mapped item representation to Column is not supported.");
                return null;
            }

            var result = new Column(repData.ExtrudeTransform.Origin,
                                    repData.Extrude.Height,
                                    null,
                                    repData.Extrude.Profile,
                                    0,
                                    0,
                                    0,
                                    transform: repData.Transform,
                                    isElementDefinition: false,
                                    id: IfcGuid.FromIfcGUID(ifcColumn.GlobalId),
                                    name: ifcColumn.Name)
            {
                RepresentationInstances = repData.RepresentationInstances
            };
            return result;
        }

        public bool CanConvert(IfcProduct ifcProduct)
        {
            return ifcProduct is IfcColumn;
        }
    }
}
