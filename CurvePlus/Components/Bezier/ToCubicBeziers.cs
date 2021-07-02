using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components.Bezier
{
    public class ToCubicBeziers : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ToCubicBeziers class.
        /// </summary>
        public ToCubicBeziers()
          : base("To Cubic Bezier Spans", "To Cubic Beziers",
              "Fits a list of cubic bezier spans to a Nurbs Curve",
              "Curve", "Bezier")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to convert to bezier spans", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance Tolerance", "D", "Max fitting error", GH_ParamAccess.item, Rhino.RhinoMath.SqrtEpsilon);
            pManager[1].Optional = false;
            pManager.AddAngleParameter("Kink Tolerance", "K", "The angle tolerance at a kink", GH_ParamAccess.item, 0.0);
            pManager[2].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Bezier Curves", "B", "A list of cubic Bezier segments", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            if (!DA.GetData(0, ref curve)) return;

            double distance = Rhino.RhinoMath.SqrtEpsilon;
            DA.GetData(1, ref distance);

            double angle = 0.0;
            DA.GetData(2, ref angle);

            BezierCurve[] beziers = BezierCurve.CreateCubicBeziers(curve,distance,angle);

            List<Curve> output = new List<Curve>();

            foreach (BezierCurve bezier in beziers)
            {
                output.Add(bezier.ToNurbsCurve());
            }


            DA.SetDataList(0, output);
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
                return Properties.Resources.CP_BezierCubic_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("44f6e345-e235-4dcb-b82a-5c7440863f68"); }
        }
    }
}