using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurvePlus.Components.Subdivide
{
    public class MidEdge : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MidEdge class.
        /// </summary>
        public MidEdge()
          : base("Mid Edge Polyline", "Mid Edge",
              "Creates a new polyline from the midpoints of an existing polyline.",
              "Curve", "Util")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "The source polyline", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Close", "C", "Optionally close the output polyline from an open input", GH_ParamAccess.item, false);
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

            bool close = false;
            DA.GetData(1, ref close);

            if (polyline.GetSegments().Count() < 2)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Insufficient segments. The polyline must contain at least 2 segments.");
                return;
            }

            if ((polyline.GetSegments().Count() == 2)&(close))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Cannot be closed. The polyline must contain at least 3 segments to be closed.");
            }


            Polyline output = polyline.Midedge(close);

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
                return Properties.Resources.CP_MidEdge_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2b8c35b6-717c-49fc-99f9-97df5e192437"); }
        }
    }
}