using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components.Utilities
{
    public class RemovePoints : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RemovePoints class.
        /// </summary>
        public RemovePoints()
          : base("Cull Points", "CullPts",
              "Cull points from a polyline by indices",
              "Curve", "Util")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "The source polyline", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Indices", "I", "The indices of the points to remove.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Collapse", "C", "If true, a single polyline will be returned with the points removed. If false, a list of polylines will be returned that are broken at the specified indices", GH_ParamAccess.item, false);
            pManager[2].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Polylines", "P", "A list of polylines",GH_ParamAccess.list);
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

            List<int> indices = new List<int>();
            if (!DA.GetDataList(1, indices)) return;

            bool collapse = false;
            DA.GetData(2, ref collapse);

            List<Polyline> polylines = new List<Polyline>();
            polylines = polyline.RemovePointsByIndex(indices, collapse);

            DA.SetDataList(0, polylines);
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
                return Properties.Resources.CP_CullPoint_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("df7d6462-4b46-4d33-8364-b1a526a4789d"); }
        }
    }
}