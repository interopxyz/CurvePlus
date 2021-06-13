using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace CurvePlus.Components
{
    
    public class BoundingRectangle : GH_Component
    {
        bool isUnion = false;
        /// <summary>
        /// Initializes a new instance of the BoundingRectangle class.
        /// </summary>
        public BoundingRectangle()
          : base("Bounding Rectangle", "Bound Rect",
              "Solve oriented geometry bounding rectangle",
              "Curve", "Primitive")
        {
            isUnion = false;
            SetMessage();
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
            pManager.AddGeometryParameter("Geometry", "G", "Geometry to Contain", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "P", "Orientation Plane", GH_ParamAccess.item, Plane.WorldXY);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddRectangleParameter("Rectangle", "R", "The bounding rectangle", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IGH_GeometricGoo> geometries = new List<IGH_GeometricGoo>();
            if (!DA.GetDataList(0, geometries)) return;

            Plane plane = Plane.WorldXY;
            DA.GetData(1, ref plane);

            List<Rectangle3d> rectangles = new List<Rectangle3d>();
            List<BoundingBox> boxes = new List<BoundingBox>();

            foreach(IGH_GeometricGoo geoGoo in geometries)
            {
                GeometryBase geometry;
                bool isGeo = geoGoo.CastTo<GeometryBase>(out geometry);

                Point3d point;
                bool isPoint = geoGoo.CastTo<Point3d>(out point);

                BoundingBox box = new BoundingBox();
                if (isGeo) box = geometry.GetBoundingBox(plane);
                if (isPoint) box = new BoundingBox(point, point);
                if (isGeo | isPoint)
                {
                    boxes.Add(box);
                    rectangles.Add(new Rectangle3d(plane, box.Min, box.Max));
                }
            }

            if (isUnion)
            {
                if (boxes.Count > 0)
                {
                    BoundingBox unionBox = boxes[0];
                    foreach(BoundingBox bbox in boxes)
                    {
                        unionBox.Union(bbox);
                    }
                    DA.SetDataList(0, new List<Rectangle3d> { new Rectangle3d(plane, unionBox.Min, unionBox.Max) });
                }
            }
            else
            {
                DA.SetDataList(0, rectangles);
            }


        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, "Union", SetUnionMode, true, isUnion);
        }

        public void SetUnionMode(Object sender, EventArgs e)
        {
            isUnion = !isUnion;
            SetMessage();
            this.ExpireSolution(true);
        }

        public void SetMessage()
        {
            if(isUnion)
            {
                Message = "Union";
            }
            else
            {
                Message = "Per Object";
            }
        }

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetBoolean("unioned", isUnion);

            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            isUnion = reader.GetBoolean("unioned");

            SetMessage();
            return base.Read(reader);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2625b22f-bb17-4451-958b-d4a057c47ef8"); }
        }
    }
}