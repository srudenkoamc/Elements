﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Geometry;
using Elements.MathUtils;

namespace Elements.Spatial
{
    public class Grid1d
    {
        /// <summary>
        /// Child cells of this Grid. If null, this Grid is a complete cell with no subdivisions.
        /// </summary>
        public List<Grid1d> Cells { get; private set; }

        /// <summary>
        /// Numerical domain of this Grid
        /// </summary>
        public Domain1d Domain { get; }


        /// <summary>
        /// Returns true if this 1D Grid has no subdivisions / sub-grids. 
        /// </summary>
        public bool IsSingleCell => Cells == null || Cells.Count == 0;



        private readonly Curve curve;

        // we have to maintain an internal curve domain because subsequent subdivisions of a grid
        // based on a curve retain the entire curve; this domain allows us to map from the subdivided
        // domain back to the original curve.
        private Domain1d curveDomain;

        /// <summary>
        /// Default constructor with optional length parameter
        /// </summary>
        /// <param name="length">Length of the grid domain</param>
        public Grid1d(double length = 1.0) : this(new Domain1d(0, length))
        {

        }

        /// <summary>
        /// Construct a 1D grid from a numerical domain. The geometry will be assumed to lie along the X axis.
        /// </summary>
        /// <param name="domain"></param>
        public Grid1d(Domain1d domain)
        {
            Domain = domain;
            curve = new Line(new Vector3(domain.Min, 0, 0), new Vector3(Domain.Max, 0, 0));
            curveDomain = domain;
        }

        /// <summary>
        /// Construct a 1D grid from a curve.
        /// </summary>
        /// <param name="curve"></param>
        public Grid1d(Curve curve)
        {
            this.curve = curve;
            Domain = new Domain1d(0, curve.Length());
            curveDomain = Domain;
        }

        /// <summary>
        /// This constructor is only for internal use by subdivision / split methods. 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="domain"></param>
        /// <param name="curveDomain"></param>
        private Grid1d(Curve curve, Domain1d domain, Domain1d curveDomain)
        {
            this.curve = curve;
            this.curveDomain = curveDomain;
            Domain = domain;
        }

        /// <summary>
        /// Split the grid at a normalized parameter from 0 to 1 along its domain. 
        /// </summary>
        /// <param name="t"></param>
        public void SplitAtParameter(double t)
        {
            var pos = t.MapToDomain(Domain);
            SplitAtPosition(pos);
        }

        /// <summary>
        /// Split the grid at a list of normalized parameters from 0 to 1 along its domain.
        /// </summary>
        /// <param name="parameters"></param>
        public void SplitAtParameters(IEnumerable<double> parameters)
        {
            foreach (var t in parameters)
            {
                SplitAtParameter(t);
            }
        }

        /// <summary>
        /// Split the grid at a fixed position from the start or end
        /// </summary>
        /// <param name="pos">The length along the grid at which to split.</param>
        /// <param name="fromEnd">If true, measure the position from the end rather than the start</param>
        public void SplitAtPosition(double pos, bool fromEnd = false)
        {
            if (fromEnd)
            {
                pos = Domain.Max - pos;
            }

            if (IsSingleCell) // simple single split
            {
                var newDomains = Domain.SplitAt(pos);
                Cells = new List<Grid1d>
                {
                    new Grid1d(curve, newDomains[0], curveDomain),
                    new Grid1d(curve, newDomains[1], curveDomain)
                };
            }
            else
            {
                var index = FindCellIndexAtPosition(pos);
                var cellToSplit = Cells[index];
                cellToSplit.SplitAtPosition(pos);
                var newCells = cellToSplit.Cells;
                Cells.RemoveAt(index);
                Cells.InsertRange(index, newCells);
            }
            OnTopLevelGridChange();

        }

        /// <summary>
        /// Split the grid at a list of fixed positions from the start or end
        /// </summary>
        /// <param name="positions">The lengths along the grid at which to split.</param>
        /// <param name="fromEnd">If true, measure the position from the end rather than the start</param>
        public void SplitAtPositions(IEnumerable<double> positions, bool fromEnd = false)
        {
            foreach (var pos in positions)
            {
                SplitAtPosition(pos, fromEnd);
            }
        }



        /// <summary>
        /// Divide the grid into N even subdivisions. Grids that are already subdivided will fail. 
        /// </summary>
        /// <param name="n"></param>
        public void DivideByCount(int n)
        {
            if (!IsSingleCell)
            {
                throw new Exception("This grid already has subdivisions. Maybe you meant to select a subgrid to divide?");
            }

            var newDomains = Domain.DivideByCount(n);
            Cells = new List<Grid1d>(newDomains.Select(d => new Grid1d(curve, d, curveDomain)));
            OnTopLevelGridChange();
        }



        /// <summary>
        /// Divide 
        /// </summary>
        /// <param name="targetLength"></param>
        /// <param name="divisionMode"></param>
        public void DivideByApproximateLength(double targetLength, EvenDivisionMode divisionMode = EvenDivisionMode.Nearest)
        {
            var numDivisions = Domain.Length / targetLength;
            int roundedDivisions;
            switch (divisionMode)
            {
                case EvenDivisionMode.RoundUp:
                    roundedDivisions = (int)Math.Ceiling(numDivisions);
                    break;
                case EvenDivisionMode.RoundDown:
                    roundedDivisions = (int)Math.Floor(numDivisions);
                    break;
                case EvenDivisionMode.Nearest:
                default:
                    roundedDivisions = (int)Math.Round(numDivisions);
                    break;
            }
            DivideByCount(roundedDivisions);
        }

        /// <summary>
        /// Divide a grid by constant length subdivisions, starting from a position. 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="position"></param>
        public void DivideByFixedLengthFromPosition(double length, double position)
        {
            if (!Domain.Includes(position))
            {
                throw new Exception("Position is outside of the grid extents");
            }

            for (double p = position; Domain.Includes(p); p += length)
            {
                SplitAtPosition(p);
            }

            if (!Domain.Includes(position - length)) return;

            for (double p = position - length; Domain.Includes(p); p -= length)
            {
                SplitAtPosition(p);
            }
        }



        /// <summary>
        /// Divide a grid by constant length subdivisions, with a variable division mode to control how leftover
        /// space is handled. 
        /// </summary>
        /// <param name="length">The division length</param>
        /// <param name="divisionMode">How to handle leftover / partial remainder panels </param>
        /// <param name="sacrificialPanels">How many full length panels to sacrifice to make remainder panels longer.</param>
        public void DivideByFixedLength(double length, FixedDivisionMode divisionMode = FixedDivisionMode.RemainderAtEnd, int sacrificialPanels = 0)
        {
            var lengthToFill = Domain.Length;
            var maxPanelCount = (int)Math.Floor(lengthToFill / length) - sacrificialPanels;
            if (maxPanelCount < 1) return;



            var remainderSize = lengthToFill - maxPanelCount * length;
            if (remainderSize < 0.01)
            {
                DivideByCount(maxPanelCount);
                return;
            }
            switch (divisionMode)
            {
                case FixedDivisionMode.RemainderAtBothEnds:
                    for (double i = remainderSize / 2.0; i < lengthToFill; i += length)
                    {
                        SplitAtPosition(i);
                    }
                    break;
                case FixedDivisionMode.RemainderAtStart:
                    for (double i = remainderSize; i < lengthToFill; i += length)
                    {
                        SplitAtPosition(i);
                    }
                    break;
                case FixedDivisionMode.RemainderAtEnd:
                    for (int i = 1; i < maxPanelCount + 1; i++)
                    {
                        SplitAtPosition(i * length);
                    }
                    break;
                case FixedDivisionMode.RemainderNearMiddle:
                    // assumes we must have at least 2 full-size panels
                    int panelsOnLeft = maxPanelCount / 2; //integer division, on purpose
                    //make left panels
                    for (int i = 1; i <= panelsOnLeft; i++)
                    {
                        SplitAtPosition(i * length);
                    }
                    //make middle + right panels
                    for (double i = panelsOnLeft * length + remainderSize; i < lengthToFill; i += length)
                    {
                        SplitAtPosition(i);
                    }

                    break;
            }
        }



        /// <summary>
        /// Retrieve the grid cell (as a Grid1d) at a length along the domain. 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Grid1d FindCellAtPosition(double pos)
        {
            var index = FindCellIndexAtPosition(pos);
            if (index < 0) return this;
            return Cells[index];
        }

        /// <summary>
        /// Retrieve the index of the grid cell at a length along the domain. 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int FindCellIndexAtPosition(double pos)
        {
            if (!Domain.Includes(pos))
            {
                throw new Exception("Position was outside the domain of the grid");
            }

            if (IsSingleCell)
            {
                return -1;
            }

            else
            {
                var cellMatch = Cells.FirstOrDefault(c => c.Domain.Includes(pos));
                if (cellMatch != null)
                {
                    return Cells.IndexOf(cellMatch);
                }
                //if we are searching right at a cell boundary, return the one to the right;
                var edgeMatch = Cells.FirstOrDefault(c => c.Domain.Min == pos);
                if (edgeMatch != null)
                {
                    return Cells.IndexOf(cellMatch);
                }
                // we must be at the last cell — but let's just make sure
                var endMatch = Cells.FirstOrDefault(c => c.Domain.Max == pos);

                if (endMatch != null)
                {
                    return Cells.IndexOf(cellMatch);
                }
                throw new Exception("Something went wrong finding a cell at this position");
            }
        }

        /// <summary>
        /// Retrieve all grid segment cells recursively.
        /// For just top-level cells, get the Cells property.
        /// </summary>
        public List<Grid1d> GetCells()
        {
            List<Grid1d> resultCells = new List<Grid1d>();
            foreach (var cell in Cells)
            {
                if (cell.IsSingleCell)
                {
                    resultCells.Add(cell);
                }
                else
                {
                    resultCells.AddRange(cell.GetCells());
                }
            }
            return resultCells;

        }

        /// <summary>
        /// Retrieve geometric representation of a cell (currently just a line)
        /// </summary>
        /// <returns>A curve representing the extents of this grid / cell.</returns>
        public Curve GetCellGeometry()
        {
            if (curve == null)
            {
                //I don't think this should ever happen
                return new Line(new Vector3(Domain.Min, 0, 0), new Vector3(Domain.Max, 0, 0));
            }

            //TODO: support subcurve output / distance-based rather than parameter-based sampling.

            var t1 = Domain.Min.MapFromDomain(curveDomain);
            var t2 = Domain.Max.MapFromDomain(curveDomain);

            var x1 = curve.TransformAt(t1);
            var x2 = curve.TransformAt(t2);

            return new Line(x1.Origin, x2.Origin);

        }


        #region Events
        public delegate void Grid1dEventHandler(Grid1d sender, EventArgs e);

        public event Grid1dEventHandler TopLevelGridChange;

        protected virtual void OnTopLevelGridChange()
        {
            Grid1dEventHandler handler = TopLevelGridChange;
            handler?.Invoke(this, new EventArgs());
        }
        #endregion

    }

    /// <summary>
    /// Describe how a target length should be treated 
    /// </summary>
    public enum EvenDivisionMode
    {
        /// <summary>
        /// Closest match for a target length, can be greater or smaller in practice. 
        /// </summary>
        Nearest,
        /// <summary>
        /// Round up the count — Only divide into segments shorter than the target length
        /// </summary>
        RoundUp,
        /// <summary>
        /// Round down the count — Only divide into segments longer than the target length
        /// </summary>
        RoundDown
    }

    /// <summary>
    /// Different ways to handle the "remainder" when dividing an arbitrary length by a fixed size  
    /// </summary>
    public enum FixedDivisionMode
    {
        /// <summary>
        /// Take the remainder and split it across both ends of the grid
        /// </summary>
        RemainderAtBothEnds,
        /// <summary>
        /// Locate the remainder at the start of the grid
        /// </summary>
        RemainderAtStart,
        /// <summary>
        /// Locate the remainder at the end of the grid
        /// </summary>
        RemainderAtEnd,
        /// <summary>
        /// Locate the remainder at or near the middle of the grid.
        /// </summary>
        RemainderNearMiddle
    }



}
