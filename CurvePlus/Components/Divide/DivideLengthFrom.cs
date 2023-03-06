﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CurvePlus.Components.Divide
{
    public class DivideLengthFrom : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DivideLengthFrom class.
        /// </summary>
        public DivideLengthFrom()
          : base("Divide Length From", "DivLenFrom",
              "Divide a curve with a preset distance between points from a parameter along the curve",
              "Curve", "Division")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to divide", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "L", "Length of segments", GH_ParamAccess.item);
            pManager.AddNumberParameter("Parameter", "t", "Parameter along the curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Division points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Tangents", "T", "Tangent vectors at division points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Parameters", "t", "Parameter values at division points", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Curve input = null;
            if (!DA.GetData(0, ref input)) return;
            Curve curve = input.DuplicateCurve();

            double distance = 1;
            if (!DA.GetData(1, ref distance)) return;

            double parameter = 0.5;
            if (!DA.GetData(2, ref parameter)) return;

            List<Point3d> points = new List<Point3d>();
            List<Vector3d> tangents = new List<Vector3d>();
            List<double> parameters = new List<double>();

            if (parameter <= curve.Domain.Min)
            {
                curve.DivideCurveByLength(distance, out points, out tangents, out parameters);
            }
            else if (parameter >= curve.Domain.Max)
            {
                curve.Reverse();
                curve.DivideCurveByLength(distance, out points, out tangents, out parameters);
            }
            else
            {
                List<Point3d> pts = new List<Point3d>();
                List<Vector3d> tns = new List<Vector3d>();
                List<double> prs = new List<double>();

                Curve[] curves = curve.Split(parameter);
                curves[0].Reverse();
                curves[0].DivideCurveByLength(distance, out List<Point3d> pA, out List<Vector3d> vA, out List<double> tA);

                pA.Reverse();
                vA.Reverse();
                tA.Reverse();
                points.AddRange(pA);
                tangents.AddRange(vA);
                parameters.AddRange(tA);

                if (curves.Length > 1)
                {
                    curves[1].DivideCurveByLength(distance, out List<Point3d> pB, out List<Vector3d> vB, out List<double> tB);

                    pB.RemoveAt(0);
                    vB.RemoveAt(0);
                    tB.RemoveAt(0);
                    points.AddRange(pB);
                    tangents.AddRange(vB);
                    parameters.AddRange(tB);
                }
            }

            DA.SetDataList(0, points);
            DA.SetDataList(1, tangents);
            DA.SetDataList(2, parameters);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D1056B21-7132-4125-9985-CA0D9DC87E40"); }
        }
    }
}