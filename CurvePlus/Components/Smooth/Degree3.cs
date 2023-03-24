﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components.Smooth
{
    public class Degree3 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CatmullClark class.
        /// </summary>
        public Degree3()
          : base("Degree 3 Smooth", "Deg3",
              "Degree 3 Polyline Smoothing",
              "Curve", "Smooth")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "Polyline to subdived and smooth", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Iterations", "I", "The number of smoothing iterations", GH_ParamAccess.item, 1);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "New Polyline", GH_ParamAccess.item);
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

            int iterations = 1;
            DA.GetData(1, ref iterations);

            DA.SetData(0, polyline.Degree3Smoothing(iterations));
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
                return Properties.Resources.Deg3_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3e8cd4ec-2ba7-4041-b185-fe98cd311e27"); }
        }
    }
}