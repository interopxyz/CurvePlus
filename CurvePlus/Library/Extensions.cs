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

        public static List<Polyline> Fan(this Polyline input)
        {
            Point3d center = input.CenterPoint();
            List<Polyline> output = new List<Polyline>();
            Line[] segments = input.GetSegments();

            foreach (Line line in segments)
            {
                Polyline polyline = new Polyline();
                polyline.Add(center);
                polyline.Add(line.From);
                polyline.Add(line.To);
                polyline.Add(center);

                output.Add(polyline);
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
                List<NurbsCurve> curves = new List<NurbsCurve>();
                curves.Add(nurbsA);

                curves.Add(NurbsCurve.CreateBlendCurve(nurbsA, nurbsA, (BlendContinuity)continuity, factor, factor).ToNurbsCurve());

                return NurbsCurve.JoinCurves(curves)[0];
            }

        }

        public static Curve SmoothCorner(this Curve curve, double t, Rhino.Geometry.BlendContinuity continuity = BlendContinuity.Tangency)
        {
            if (t == 0) return curve.DuplicateCurve();
            t = t / 2.0;

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

                if (!input.IsClosed)
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

                if (!input.IsClosed)
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
