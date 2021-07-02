using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components
{
    public class CloseCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ConnectTwoCurves class.
        /// </summary>
        public CloseCurve()
          : base("Close Curve", "Close",
              "Closes a curve by adding an additional span",
              "Curve", "Util")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Open curve to close", GH_ParamAccess.item);
            pManager.AddNumberParameter("Factor", "F", "The bulge factor for Tangency and Curvature modes", GH_ParamAccess.item, 0.5);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Continuity", "C", "Blend Continuity Type", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;

            Param_Integer param = (Param_Integer)pManager[2];
            foreach (Extensions.BlendContinuityModes value in Enum.GetValues(typeof(Extensions.BlendContinuityModes)))
            {
                param.AddNamedValue(value.ToString(), (int)value);
            }

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Closed Curve", "C", "A closed curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            if (!DA.GetData(0, ref curve)) return;

            double f = 0.5;
            DA.GetData(1, ref f);

            int continuity = 0;
            DA.GetData(2, ref continuity);

            Curve output = curve.CloseCurve(f, (Extensions.BlendContinuityModes) continuity);

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
                return Properties.Resources.CP_CloseCurve_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("607e693e-61a7-43d7-b2a6-f247c6ad518b"); }
        }
    }
}