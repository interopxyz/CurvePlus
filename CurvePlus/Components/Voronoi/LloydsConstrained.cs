using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Geometry.Voronoi;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurvePlus.Components.Voronoi
{
    public class LloydsConstrained : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the LloydsConstrained class.
        /// </summary>
        public LloydsConstrained()
          : base("Lloyds Voronoi Constrained", "Lloyds2",
              "An implementation of Lloyd's algorithm which iteratively runs a Voronoi solution which for each run uses the previous run's cell area centroids as the new seed points constrained to a surface.",
              "Mesh", "Triangulation")
        {
        }

        /// <summary>
        /// Set Exposure level for the component.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "The points for the initial voronoi solution", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Trimmed Surface", "S", "A planar trimmed surface for containment", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Iterations", "I", "The number of solution iterations to run", GH_ParamAccess.item, 1);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Cells", "C", "The polyline cells of the voronoi diagram", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            if (!DA.GetDataList(0, points)) return;

            Brep brep = null;
            if (!DA.GetData(1, ref brep)) return;
            bool isPlanar = brep.Surfaces[0].IsPlanar();

            if (!isPlanar)
            {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Surface must be planar");
                    return;
            }

            int iterations = 1;
            if (!DA.GetData(2, ref iterations)) return;

            List<Polyline> CellsOut = new List<Polyline>();

            double Dt = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            BoundingBox bbox = brep.GetBoundingBox(true);

            Node2List corners = new Node2List();

            corners.Append(new Node2(bbox.Min.X, bbox.Min.Y));
            corners.Append(new Node2(bbox.Max.X, bbox.Min.Y));
            corners.Append(new Node2(bbox.Max.X, bbox.Max.Y));
            corners.Append(new Node2(bbox.Min.X, bbox.Max.Y));


            for (int i = 0; i < iterations; i++)
            {

                Node2List nodes = new Node2List();
                foreach (Point3d Pt in points)
                {
                    nodes.Append(new Node2(Pt.X, Pt.Y));
                }


                List<Cell2> CS = Solver.Solve_Connectivity(nodes, Grasshopper.Kernel.Geometry.Delaunay.Solver.Solve_Connectivity(nodes, Dt, false), corners);

                List<Polyline> Pts = new List<Polyline>();
                List<Point3d> centers = new List<Point3d>();

                foreach (Cell2 Cls in CS)
                {
                    Polyline Pline = Cls.ToPolyline();

                    Curve pCrv = Pline.ToNurbsCurve();
                    List<Curve> Projected = Curve.ProjectToBrep(pCrv, brep, -Vector3d.ZAxis, Dt).ToList();

                    foreach (Curve SubCrv in Projected)
                    {
                        Polyline TPline = new Polyline();
                        SubCrv.TryGetPolyline(out TPline);

                        if (!SubCrv.IsClosed)
                        {
                            TPline.Add(TPline[0]);
                        }

                        centers.Add(TPline.CenterPoint());
                        Pts.Add(TPline);
                    }
                }

                points = centers;
                CellsOut = Pts;
            }


            DA.SetDataList(0, CellsOut);
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
                return Properties.Resources.CP_LloydsConstrained_01;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bf2244b5-f5c7-4a72-8c69-a8a089afc475"); }
        }
    }
}