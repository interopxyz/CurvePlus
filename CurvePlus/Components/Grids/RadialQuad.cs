using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components
{
    public class RadialQuad : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RadialQuad class.
        /// </summary>
        public RadialQuad()
          : base("Radial Quad Grid", "RadQuadGrid",
              "2D Radial Quad Grid",
              "Vector", "Grid")
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
            pManager.AddPlaneParameter("Plane", "P", "Base Plane for the grid", GH_ParamAccess.item, Plane.WorldXY);
            pManager[0].Optional = true;
            pManager.AddNumberParameter("Inner Radius", "R", "The inner radius of the grid", GH_ParamAccess.item, 1);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Size", "S", "Distance between concentric grid loops", GH_ParamAccess.item, 1);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Extent R", "Er", "Number of Grid Cells in the radial direction", GH_ParamAccess.item, 6);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("Extent P", "Ep", "Number of Grid Cells in the polar direction", GH_ParamAccess.item, 12);
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Cells", "C", "Grid Cell Outlines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.WorldXY;
            DA.GetData(0, ref plane);

            double radius = 1.0;
            DA.GetData(1, ref radius);

            double size = 1.0;
            DA.GetData(2, ref size);

            int radial = 6;
            DA.GetData(3, ref radial);

            int polar = 12;
            DA.GetData(4, ref polar);

            int countR = radial+1;
            int countP = polar;

            double stepR = 1.0 / radial;
            double stepP = 1.0 / polar;


            List<Curve> cells = new List<Curve>();

            List<List<Point3d>> points = new List<List<Point3d>>();

            for(int i = 0; i < countR; i++)
            {
                points.Add(new List<Point3d>());
                for (int j = 0; j < countP; j++)
                {
                    double x = (radius + size * i) * Math.Sin(Math.PI * 2 * stepP * j);
                    double y = (radius + size * i) * Math.Cos(Math.PI * 2 * stepP * j);

                    Point3d point = plane.PointAt(x, y);
                    points[i].Add(point);
                }
            }

            for (int i = 0; i < countR-1; i++)
            {
                for (int j = 0; j < countP; j++)
                {
                    int u = (i+1);
                    int v = (j+1) % countP;
                    Polyline cell = new Polyline();
                    cell.Add(points[i][j]);
                    cell.Add(points[i][v]);
                    cell.Add(points[u][v]);
                    cell.Add(points[u][j]);
                    cell.Add(points[i][j]);

                    cells.Add(cell.ToNurbsCurve());
                }
            }


            DA.SetDataList(0, cells);
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
                return Properties.Resources.CP_Grid_Quad_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b0b1339c-bde8-4212-8ab4-1fea1e69c65e"); }
        }
    }
}