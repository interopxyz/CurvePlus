﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components.Polygons
{
    public class InscribedPolygon : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RegularPolygon class.
        /// </summary>
        public InscribedPolygon()
          : base("Inscribed Polygon", "InPgon",
              "A regular inscribed polygon",
              "Curve", "Primitive")
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
            pManager.AddPlaneParameter("Plane", "P", "Polygon base plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager[0].Optional = true;
            pManager.AddNumberParameter("Radius", "R", "The distance from the center to the corner of the polygon", GH_ParamAccess.item, 1);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Sides", "S", "The number of sides of the regular polygon. Must be more than 2", GH_ParamAccess.item, 3);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Polygon", "P", "Regular polygon curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            DA.GetData(0, ref plane);

            double radius = 1;
            DA.GetData(1, ref radius);

            int sides= 3;
            DA.GetData(2, ref sides);
            if (sides < 3)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Minimum number of sides is 3.");
                sides = 3;
            }

            if (radius <= 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Radius must be greater than 0");
                return;
            }

            Circle circle = new Circle(plane, radius);

            Curve polygon = Polyline.CreateInscribedPolygon(circle, sides).ToNurbsCurve();
            DA.SetData(0, polygon);
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
                return Properties.Resources.CP_Polygon_Inscribed_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e815b78d-d2cc-4411-b8f5-f15e555baeec"); }
        }
    }
}