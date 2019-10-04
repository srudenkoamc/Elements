using System;
using System.Collections.Generic;
using Elements.ElementTypes;
using Elements.Geometry;
using Elements.Interfaces;
using Newtonsoft.Json;

namespace Elements
{
    /// <summary>
    /// A wall defined by a planar curve, a height, and a thickness.
    /// </summary>
    /// <example>
    /// <code source="../../test/Examples/WallExample.cs"/>
    /// </example>
    public class StandardWall : Wall, IHasOpenings
    {
        private List<Opening> _openings = new List<Opening>();

        /// <summary>
        /// The center line of the wall.
        /// </summary>
        public Line CenterLine { get; }

        /// <summary>
        /// An array of openings in the wall.
        /// </summary>
        public List<Opening> Openings
        {
            get{return _openings;}
            protected set{_openings = value;}
        }

        /// <summary>
        /// Extrude to both sides?
        /// </summary>
        public override bool BothSides => true;

        /// <summary>
        /// Construct a wall along a line.
        /// </summary>
        /// <param name="centerLine">The center line of the wall.</param>
        /// <param name="elementType">The wall type of the wall.</param>
        /// <param name="height">The height of the wall.</param>
        /// <param name="openings">A collection of Openings in the wall.</param>
        /// <param name="transform">The transform of the wall.
        /// This transform will be concatenated to the transform created to describe the wall in 2D.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the height of the wall is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the Z components of wall's start and end points are not the same.</exception>
        public StandardWall(Line centerLine, WallType elementType, double height, List<Opening> openings = null, Transform transform = null)
        {
            if (height <= 0.0)
            {
                throw new ArgumentOutOfRangeException($"The wall could not be created. The height of the wall provided, {height}, must be greater than 0.0.");
            }

            if (centerLine.Start.Z != centerLine.End.Z)
            {
                throw new ArgumentException("The wall could not be created. The Z component of the start and end points of the wall's center line must be the same.");
            }

            this.CenterLine = centerLine;
            this.Height = height;
            this.ElementType = elementType;
            if(openings != null)
            {
                this._openings = openings;
            }
            
            // Construct a transform whose X axis is the centerline of the wall.
            // The wall is described as if it's lying flat in the XY plane of that Transform.
            var d = centerLine.Direction();
            var z = d.Cross(Vector3.ZAxis);
            var wallTransform = new Transform(centerLine.Start, d, z);
            this.Transform = wallTransform;
            if(transform != null) 
            {
                wallTransform.Concatenate(transform);
            }

            this.Profile = new Profile(Polygon.Rectangle(Vector3.Origin, new Vector3(centerLine.Length(), height)));
            this.ExtrudeDepth = this.Thickness();
            this.ExtrudeDirection = Vector3.ZAxis;
        }

        [JsonConstructor]
        internal StandardWall(Line centerLine, Guid elementTypeId, double height, List<Opening> openings = null, Transform transform = null)
        {
            if (height <= 0.0)
            {
                throw new ArgumentOutOfRangeException($"The wall could not be created. The height of the wall provided, {height}, must be greater than 0.0.");
            }

            if (centerLine.Start.Z != centerLine.End.Z)
            {
                throw new ArgumentException("The wall could not be created. The Z component of the start and end points of the wall's center line must be the same.");
            }

            this.CenterLine = centerLine;
            this.Height = height;
            this._elementTypeId = elementTypeId;
            if(openings != null)
            {
                this._openings = openings;
            }
            
            // Construct a transform whose X axis is the centerline of the wall.
            // The wall is described as if it's lying flat in the XY plane of that Transform.
            var d = centerLine.Direction();
            var z = d.Cross(Vector3.ZAxis);
            var wallTransform = new Transform(centerLine.Start, d, z);
            this.Transform = wallTransform;
            if(transform != null) 
            {
                wallTransform.Concatenate(transform);
            }

            this.Profile = new Profile(Polygon.Rectangle(Vector3.Origin, new Vector3(centerLine.Length(), height)));
            this.ExtrudeDirection = Vector3.ZAxis;
        }

        /// <summary>
        /// Set the wall type.
        /// </summary>
        public override void SetReference(WallType type)
        {
            this.ElementType = type;
            this._elementTypeId = this.ElementType.Id;
            this.ExtrudeDepth = this.Thickness();
        }
    }
}