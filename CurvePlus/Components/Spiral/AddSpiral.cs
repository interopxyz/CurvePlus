using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components
{
    public class AddSpiral : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddSpiral class.
        /// </summary>
        public AddSpiral()
          : base("Spiral", "Spiral",
              "Creates a spiral from base plane, pitch, turn count, and two radi",
              "Curve", "Spline")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary | GH_Exposure.obscure; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Base Plane", "F", "The base plane for the spiral", GH_ParamAccess.item, Plane.WorldXY);
            pManager[0].Optional = true;
            pManager.AddAngleParameter("Angle", "A", "Pitch angle", GH_ParamAccess.item, Math.PI / 4);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Turns", "T", "The number of turns in the spiral", GH_ParamAccess.item, 2);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Base Radius", "B", "Radius at the base of the spiral", GH_ParamAccess.item, 1);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Peak Radius", "P", "Radius at the peak of the spiral", GH_ParamAccess.item, 1);
            pManager[4].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Spiral", "S", "The spiral curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            DA.GetData(0, ref plane);

            Point3d axisStart = plane.Origin;
            Vector3d axisDir = plane.ZAxis;
            Point3d radiusPoint = plane.Origin+plane.XAxis;

            double pitch = Math.PI/4;
            DA.GetData(1, ref pitch);

            double turnCount = 2;
            DA.GetData(2, ref turnCount);

            double radius0 = 1;
            DA.GetData(3, ref radius0);

            double radius1 = 1;
            DA.GetData(4, ref radius1);

            Curve output = NurbsCurve.CreateSpiral(axisStart, axisDir, radiusPoint, pitch, turnCount, radius0, radius1);

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
                return Properties.Resources.CP_SpiralPlane_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("312a18f0-3da1-467e-b19f-7ac542a10932"); }
        }
    }
}