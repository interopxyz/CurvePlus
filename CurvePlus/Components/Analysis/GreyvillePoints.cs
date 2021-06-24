using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurvePlus.Components.Analysis
{
    public class GreyvillePoints : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GreyvillePoints class.
        /// </summary>
        public GreyvillePoints()
          : base("Greyville Points", "Greyville",
              "Description",
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
            pManager.AddPointParameter("Points", "P", "The greyville points of the curve", GH_ParamAccess.list);
            pManager.AddNumberParameter("Parameters", "T", "The greyville parameters of the curve", GH_ParamAccess.list);
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

            List<Point3d> points = nurbs.GrevillePoints(true).ToList();
            List<double> parameters = nurbs.GrevilleParameters().ToList();


            DA.SetDataList(0, points);
            DA.SetDataList(1, parameters);
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
                return Properties.Resources.CP_GreyvillePoints_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a79ce08b-5ca6-4d75-aeab-d735a5acaa18"); }
        }
    }
}