using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components.Analysis
{
    public class CurveSpans : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CurveSpans class.
        /// </summary>
        public CurveSpans()
          : base("Curve Spans", "Spans",
              "Returns the curve span domains",
              "Curve", "Analysis")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "A nurbs curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntervalParameter("Domains", "D", "The span domains of the curve", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            if(!DA.GetData(0, ref curve))return;
            NurbsCurve nurbs = curve.ToNurbsCurve();

            int count = nurbs.SpanCount;

            List<Interval> domains = new List<Interval>();
            for(int i = 0; i < count; i++)
            {
                domains.Add(nurbs.SpanDomain(i));
            }

            DA.SetDataList(0, domains);
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
                return Properties.Resources.CP_CurveSpans_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3e14cf7e-f623-4e0c-8a49-997ff5bcbd9a"); }
        }
    }
}