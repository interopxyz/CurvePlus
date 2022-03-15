using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components.Spiral
{
    public class AddPathSpiral : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddPathSpiral class.
        /// </summary>
        public AddPathSpiral()
          : base("Spiral Rail", "SpiralRail",
              "Creates a spiral along a rail curve by pitch, turn count, and two radi",
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
            pManager.AddCurveParameter("Curve", "C", "The rail curve for the spiral", GH_ParamAccess.item);
            pManager.AddAngleParameter("Angle", "A", "RotationAngle", GH_ParamAccess.item, Math.PI / 4);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Turns", "T", "The number of turns in the spiral", GH_ParamAccess.item, 2);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Base Radius", "B", "Radius at the base of the spiral", GH_ParamAccess.item, 1);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Peak Radius", "P", "Radius at the peak of the spiral", GH_ParamAccess.item, 1);
            pManager[4].Optional = true;
            pManager.AddIntegerParameter("Sample Point", "S", "Number of sample points per turn", GH_ParamAccess.item, 100);
            pManager[5].Optional = true;

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
            Curve railCurve = null;
            if (!DA.GetData(0, ref railCurve)) return;
            railCurve.Domain = new Interval(0, 1);

            Plane plane = Plane.WorldYZ;
            railCurve.PerpendicularFrameAt(0, out plane);

            double pitch = Math.PI / 4;
            DA.GetData(1, ref pitch);
            plane.Rotate(pitch, plane.ZAxis);

            Point3d radiusPoint = plane.Origin + plane.XAxis;

            double turnCount = 2;
            DA.GetData(2, ref turnCount);

            double radius0 = 1;
            DA.GetData(3, ref radius0);

            double radius1 = 1;
            DA.GetData(4, ref radius1);

            int pointsPerTurn = 100;
            DA.GetData(5, ref pointsPerTurn);

            Curve output = NurbsCurve.CreateSpiral(railCurve,0,1,radiusPoint,pitch,turnCount,radius0,radius1,pointsPerTurn);

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
                return Properties.Resources.CP_SpiralCurve_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d25e638d-0677-4306-89e7-caadf0786013"); }
        }
    }
}