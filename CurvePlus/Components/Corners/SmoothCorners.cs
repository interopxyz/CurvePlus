using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components
{
    public class SmoothCorners : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SnubCorners class.
        /// </summary>
        public SmoothCorners()
          : base("Smooth Corners", "Smooth Param",
              "Smooth the corners of a segmented curve by unitized parameter",
              "Curve", "Util")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary | GH_Exposure.obscure; }
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to Smooth Corners", GH_ParamAccess.item);
            pManager.AddNumberParameter("Parameter", "T", "A unitized curve parameter between 0-1", GH_ParamAccess.item, 0.5);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Blend Continuity", "B", "Blend Continuity Type", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("Close", "C", "If true, the open ends of the curve will be closed with a blend",GH_ParamAccess.item, false);
            pManager[3].Optional = true;


            Param_Integer param = (Param_Integer)pManager[2];
            foreach (Rhino.Geometry.BlendContinuity value in Enum.GetValues(typeof(Rhino.Geometry.BlendContinuity)))
            {
                param.AddNamedValue(value.ToString(), (int)value);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Compound Curve", "C", "The smoothed polycurve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            if (!DA.GetData(0, ref curve)) return;

            double t = 0.5;
            DA.GetData(1, ref t);

            int continuity = 0;
            DA.GetData(2, ref continuity);

            bool closed = false;
            DA.GetData(3, ref closed);

            Curve output = curve.SmoothCorner(t, (BlendContinuity) continuity, closed);

            DA.SetData(0, output);
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
                return Properties.Resources.CP_BlendCornersT_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d6d9b934-83b2-452d-ab0c-87fc73a03ac5"); }
        }
    }
}