using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components.Utilities
{
    public class OffsetByPoints : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the OffsetByPoints class.
        /// </summary>
        public OffsetByPoints()
          : base("Offset By Points", "OffsetPts",
              "Offset a polyline by vertex parameters",
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
            pManager.AddCurveParameter("Polyline", "P", "The source polyline", GH_ParamAccess.item);
            pManager.AddNumberParameter("Parameter", "T", "A unitized offset parameter between 0, the original point, and 1 the area center.", GH_ParamAccess.item, 0.5);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "The new polyline", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            if (!DA.GetData(0, ref curve)) return;
            NurbsCurve nurbs = curve.ToNurbsCurve();
            Polyline polyline = nurbs.Points.ControlPolygon();

            double t = 0.5;
            DA.GetData(1, ref t);

            Polyline output = polyline.OffsetByParameter(t);

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
                return Properties.Resources.CP_OffsetCp_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d83f66ff-eb77-44cd-bbc3-0518e69daec5"); }
        }
    }
}