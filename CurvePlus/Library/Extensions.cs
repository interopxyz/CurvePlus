using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurvePlus
{
    public static class Extensions
    {
        public enum BlendContinuityModes { Position,Tangency,Curvature};

        public static List<Polyline> TriangularFan(this Polyline input)
        {
            Point3d center = input.CenterPoint();
            List<Polyline> output = new List<Polyline>();
            Line[] segments = input.GetSegments();

            foreach (Line line in segments)
            {
                Polyline polyline = new Polyline
                {
                    center,
                    line.From,
                    line.To,
                    center
                };

                output.Add(polyline);
            }
            return output;
        }

        public static List<Polyline> QuadrangularFan(this Polyline input)
        {
            Point3d center = input.CenterPoint();
            List<Polyline> output = new List<Polyline>();
            Line[] segments = input.GetSegments();

            for(int i =0;i<segments.Count()-1;i++)
            {
                Line lineA = segments[i];
                Line lineB = segments[i+1];
                Polyline polyline = new Polyline
                {
                    center,
                    (lineA.From + lineA.To) / 2.0,
                    lineA.To,
                    (lineB.From + lineB.To) / 2.0,
                    center
                };

                output.Add(polyline);
            }

            if (input.IsClosedWithinTolerance(Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance))
            {
                Line lineA = segments[segments.Count() - 1];
                Line lineB = segments[0];
                Polyline polyline = new Polyline
                {
                    center,
                    (lineA.From + lineA.To) / 2.0,
                    lineA.To,
                    (lineB.From + lineB.To) / 2.0,
                    center
                };

                output.Add(polyline);
            }
            return output;
        }

        public static Polyline OffsetByParameter(this Polyline polyline, double t)
        {
            Polyline output = new Polyline();
            List<Point3d> points = polyline.ToList();
            Point3d center = polyline.CenterPoint();

            foreach(Point3d point in points)
            {
                output.Add(point + (center - point) * t);
            }

            return output;
        }

        public static Polyline Midedge(this Polyline polyline, bool closeOutput = false)
        {
            Polyline output = new Polyline();
            Line[] segments = polyline.GetSegments();

            if (polyline.IsClosedWithinTolerance(Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)) closeOutput = true;

            foreach (Line line in segments)
            {
                output.Add((line.From + line.To) / 2.0);
            }

            if (closeOutput)
            {
                Line line = segments[0];
                output.Add((line.From + line.To) / 2.0);
            }
            return output;

        }

        public static List<Polyline> RemovePointsByIndex(this Polyline polyline, List<int> indicies, bool collapse = true)
        {
            List<Polyline> output = new List<Polyline>();
            List<Point3d> points = polyline.ToList();

            if (collapse)
            {
                int[] arrIndices = indicies.ToArray();
                Array.Sort(arrIndices);
                Array.Reverse(arrIndices);

                foreach (int index in arrIndices)
                {
                    points.RemoveAt(index);
                }

                Polyline pline = new Polyline(points);

                output.Add(pline);
            }
            else
            {
                Polyline pline = new Polyline();
                
                for(int i = 0;i<points.Count;i++)
                {
                    if (indicies.Contains(i))
                    {
                        if (pline.Count > 1) { 
                            output.Add(new Polyline(pline));
                            pline = new Polyline();
                        }
                    }
                    else
                    {
                        pline.Add(points[i]);
                    }

                }
                if (pline.Count > 1) { output.Add(pline); }
                
            }

            return output;
        }

        public static List<Polyline> Triangulate(this Polyline polyline)
        {
            List<Polyline> output = new List<Polyline>();
            
            if (!polyline.IsClosedWithinTolerance(Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)) polyline.Add(polyline[0]);
            polyline[polyline.Count - 1] = polyline[0];
            MeshFace[] faces = polyline.TriangulateClosedPolyline();

            foreach(MeshFace face in faces)
            {
                Polyline pline = new Polyline
                {
                    polyline[face.A],
                    polyline[face.B],
                    polyline[face.C],
                    polyline[face.A]
                };
                output.Add(pline);
            }

            return output;
        }

        public static List<Polyline> RemoveSegmentsByIndex(this Polyline polyline,List<int> indicies, bool collapse = true)
        {
            List<Polyline> output = new List<Polyline>();
            List<Curve> nSegments = polyline.ToNurbsCurve().DuplicateSegments().ToList();
            List<Line> segments = polyline.GetSegments().ToList();

            int[] arrIndices = indicies.ToArray();
            Array.Sort(arrIndices);
            arrIndices.Reverse();

            foreach(int index in arrIndices)
            {
                segments.RemoveAt(index);
                nSegments.RemoveAt(index);
            }

            if (collapse)
            {
                List<int> ptIndicies = new List<int>();
                foreach(int index in indicies)
                {
                    if (!ptIndicies.Contains(index)) ptIndicies.Add(index);
                    if (!ptIndicies.Contains(index + 1)) ptIndicies.Add(index + 1);
                }

                output.AddRange( polyline.RemovePointsByIndex(ptIndicies,true));
            }
            else
            {
                List<Curve> curves = NurbsCurve.JoinCurves(nSegments).ToList();
                foreach(Curve curve in curves)
                {
                    output.Add(curve.ToNurbsCurve().Points.ControlPolygon());
                }
            }

            return output;
        }

        public static Curve CloseCurve(this Curve curve, double factor, BlendContinuityModes continuity = BlendContinuityModes.Position)
        {

            NurbsCurve nurbsA = curve.ToNurbsCurve();
            nurbsA.Domain = new Interval(0, 1.0);

            if (curve.IsClosed)
            {
                return nurbsA;
            }
            else
            {
                NurbsCurve nurbsB = nurbsA.DuplicateCurve().ToNurbsCurve();
                nurbsB.Reverse();
                List<NurbsCurve> curves = new List<NurbsCurve>
                {
                    nurbsA,
                    NurbsCurve.CreateBlendCurve(nurbsA, nurbsA, (BlendContinuity)continuity, factor, factor).ToNurbsCurve()
                };

                return NurbsCurve.JoinCurves(curves)[0];
            }

        }

        public static Curve SmoothCorner(this Curve curve, double t, Rhino.Geometry.BlendContinuity continuity = BlendContinuity.Tangency)
        {
            if (t == 0) return curve.DuplicateCurve();
            t /= 2.0;

            NurbsCurve nurbs = curve.ToNurbsCurve();

            Curve[] segments = nurbs.DuplicateSegments();
            int count = segments.Length;

            List<NurbsCurve> curves = new List<NurbsCurve>();

            for (int i = 0; i < count; i++)
            {
                int j = (i + 1) % count;

                NurbsCurve nurbsA = segments[i].DuplicateCurve().ToNurbsCurve();
                nurbsA.Reverse();
                nurbsA.Domain = new Interval(0, 1.0);
                NurbsCurve nurbsB = segments[j].DuplicateCurve().ToNurbsCurve();
                nurbsB.Domain = new Interval(0, 1.0);

                NurbsCurve blendCurve = NurbsCurve.CreateBlendCurve(nurbsA, t, true, continuity, nurbsB, t, true, continuity).ToNurbsCurve();

                curves.Add(blendCurve);

            }

            if (t < 0.5)
            {
                for (int i = 0; i < count; i++)
                {
                    NurbsCurve nurbsA = segments[i].DuplicateCurve().ToNurbsCurve();
                    nurbsA.Reverse();
                    nurbsA.Domain = new Interval(0, 1.0);

                    Curve[] trimmed = nurbsA.Split(new List<double> { t, 1.0 - t });
                    curves.Add(trimmed[1].ToNurbsCurve());
                }

            }

            Curve output = NurbsCurve.JoinCurves(curves)[0];

            return output;

        }

        public static Curve SmoothCornerByDistance(this Curve curve, double distance, Rhino.Geometry.BlendContinuity continuity = BlendContinuity.Tangency)
        {
            if (distance == 0) return curve.DuplicateCurve();

            NurbsCurve nurbs = curve.ToNurbsCurve();

            Curve[] segments = nurbs.DuplicateSegments();
            int count = segments.Length;

            List<NurbsCurve> curves = new List<NurbsCurve>();

            for (int i = 0; i < count; i++)
            {
                int j = (i + 1) % count;

                NurbsCurve nurbsA = segments[i].DuplicateCurve().ToNurbsCurve();
                nurbsA.Reverse();
                nurbsA.Domain = new Interval(0, 1.0);

                NurbsCurve nurbsB = segments[j].DuplicateCurve().ToNurbsCurve();
                nurbsB.Domain = new Interval(0, 1.0);

                NurbsCurve nurbsC = segments[i].DuplicateCurve().ToNurbsCurve();
                nurbsC.Domain = new Interval(0, 1.0);

                double ta = 0.5;
                if(distance*2<nurbsA.GetLength())nurbsA.LengthParameter(distance, out ta);

                double tb = 0.5;
                if (distance * 2 < nurbsB.GetLength()) nurbsB.LengthParameter(distance, out tb);

                double tc = 0.5;
                if (distance * 2 < nurbsC.GetLength()) nurbsC.LengthParameter(distance, out tc);

                NurbsCurve blendCurve = NurbsCurve.CreateBlendCurve(nurbsA, ta, true, continuity, nurbsB, tb, true, continuity).ToNurbsCurve();

                curves.Add(blendCurve);

                if((ta+tc)<1.0)
                {
                    Curve[] trimmed = nurbsA.Split(new List<double> { ta, 1.0-tc });
                    curves.Add(trimmed[1].ToNurbsCurve());
                }

            }

            Curve output = NurbsCurve.JoinCurves(curves)[0];

            return output;

        }


        public static Polyline Snub(this Polyline input, double t)
        {

            if (t < 0.0) t = 0.0;
            if (t > 1.0) t = 1.0;

            Line[] segments = input.GetSegments();
            List<Point3d> points = new List<Point3d>();

            if (t < 1.0)
            {
                foreach (Line line in segments)
                {
                    NurbsCurve crv = line.ToNurbsCurve();
                    points.Add(crv.PointAtNormalizedLength(t / 2.0));
                    points.Add(crv.PointAtNormalizedLength(1.0 - t / 2.0));
                }

                if (!input.IsClosedWithinTolerance(Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance))
                {
                    points[0] = input[0];
                    points[points.Count - 1] = input[input.Count - 1];
                }
                else
                {
                    points.Add(points[0]);
                }

                return new Polyline(points);
            }
            else
            {
                foreach (Line line in segments)
                {
                    NurbsCurve crv = line.ToNurbsCurve();
                    points.Add(crv.PointAtNormalizedLength(0.5));
                }

                if (!input.IsClosedWithinTolerance(Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance))
                {
                    points.Insert(0, input[0]);
                    points.Add(input[input.Count - 1]);
                }
                else
                {
                    points.Add(points[0]);
                }

                return new Polyline(points);
            }
        }


    }
}
