using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CurvePlus.Components
{
    public class RadialHexagon : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RadialHexagon class.
        /// </summary>
        public RadialHexagon()
          : base("Radial Hexagon Grid", "RadHexGrid",
              "2D Radial Hexagon Grid",
              "Vector", "Grid")
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

            int countR = radial+2;
            int countP = polar*4;

            double stepR = 1.0 / radial;
            double stepP = 1.0 / countP;

            double shift = Math.PI * 2 * stepP / 2.0;
                           
            List<Curve> cells = new List<Curve>();

            List<List<Point3d>> points = new List<List<Point3d>>();

            for (int i = 0; i < countR; i++)
            {
                points.Add(new List<Point3d>());
                for (int j = 0; j < countP; j++)
                {
                    double x = (radius + size * i) * Math.Sin(shift+Math.PI * 2 * stepP * j);
                    double y = (radius + size * i) * Math.Cos(shift+Math.PI * 2 * stepP * j);

                    Point3d point = plane.PointAt(x, y);
                    points[i].Add(point);
                }
            }

            for (int i = 0; i < countR - 2; i++)
            {
                for (int j = 0; j < countP; j+=4)
                {
                    int ua = (i + 1);
                    int ub = (i + 2);

                    int bump = (i + 1) % 2;
                    bump *= 2;
                    int va = (j + bump) % countP;
                    int vb = (j + bump+ 1) % countP;
                    int vc = (j + bump + 2) % countP;
                    int vd = (j + bump + 3) % countP;
                    Polyline cell = new Polyline();
                    cell.Add(points[i][vb]);
                    cell.Add(points[i][vc]);
                    cell.Add(points[ua][vd]);
                    cell.Add(points[ub][vc]);
                    cell.Add(points[ub][vb]);
                    cell.Add(points[ua][va]);
                    cell.Add(points[i][vb]);

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
                return Properties.Resources.CP_Grid_Hexagon_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("efd76cb1-d44a-4aa5-bb2a-aa58bb14206d"); }
        }
    }
}