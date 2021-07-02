using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components
{
    public class WeightControlPoint : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the WeightControlPoint class.
        /// </summary>
        public WeightControlPoint()
          : base("Weight Control Points", "WeightPts",
              "Weight control points",
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
            pManager.AddCurveParameter("Curve", "C", "A nurbs curve", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Indices", "I", "Control point indices", GH_ParamAccess.list);
            pManager.AddNumberParameter("Weights", "W", "Control point weights", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "The modified curve", GH_ParamAccess.item);
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

            List<int> indices = new List<int>();
            if (!DA.GetDataList(1, indices)) return;

            List<double> weights = new List<double>();
            if (!DA.GetDataList(2, weights)) return;

            for(int i=weights.Count;i<indices.Count;i++)
            {
                weights.Add(weights[weights.Count - 1]);
            }

            for(int i = 0; i < indices.Count; i++)
            {
                int index = indices[i];
                Point3d p = nurbs.Points[index].Location;
                nurbs.Points.SetPoint(index, new Point3d(p.X, p.Y, p.Z), weights[i]);
            }

            DA.SetData(0, nurbs);
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
                return Properties.Resources.CP_WeightControlPoint_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e43eb3e8-2cd1-4f70-8b68-ecd57cce6a44"); }
        }
    }
}