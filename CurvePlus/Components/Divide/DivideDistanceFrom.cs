using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CurvePlus.Components.Divide
{
    public class DivideDistanceFrom : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DivideDistanceFrom class.
        /// </summary>
        public DivideDistanceFrom()
          : base("Divide Distance From", "DivDistFrom",
              "Divide a curve into segments with a preset length from a parameter along the curve",
              "Curve", "Division")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to divide", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Distance between points", GH_ParamAccess.item);
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
                points = curve.DivideCurveByDistance(distance);
            }
            else if (parameter >= curve.Domain.Max)
            {
                curve.Reverse();
                points = curve.DivideCurveByDistance(distance);
                points.Reverse();
            }
            else
            {
                List<Point3d> pA= new List<Point3d>();
                List<Point3d> pB = new List<Point3d>();

                Curve[] curves = curve.Split(parameter);
                curves[0].Reverse();
                pA = curves[0].DivideCurveByDistance(distance);

                pA.Reverse();
                points.AddRange(pA);

                if (curves.Length > 1)
                {
                    pB = curves[1].DivideCurveByDistance(distance);

                    pB.RemoveAt(0);
                    points.AddRange(pB);
                }
            }

            foreach(Point3d p in points)
            {
                curve.ClosestPoint(p, out double t);
                parameters.Add(t);
                tangents.Add(curve.TangentAt(t));
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
                return Properties.Resources.CP_DivideDistance_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("95628501-E0A7-437F-B9A3-C2BED97B152A"); }
        }
    }
}