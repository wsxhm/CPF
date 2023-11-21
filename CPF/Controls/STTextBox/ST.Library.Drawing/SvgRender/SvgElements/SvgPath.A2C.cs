using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
/*
 * Note: This code was translated from https://github.com/fontello/svgpath/blob/master/lib/a2c.js
 * Time: 2022-02-24
 */
namespace ST.Library.Drawing.SvgRender
{
    partial class SvgPath
    {
        // Convert an arc to a sequence of cubic bezier curves
        private const double TAU = Math.PI * 2;

        /* eslint-disable space-infix-ops */
        // Calculate an angle between two unit vectors
        //
        // Since we measure angle between radii of circular arcs,
        // we can use simplified math (without length normalization)
        //
        private static double UnitVectorAngle(double x1, double y1, double x2, double y2) {
            var sign = (x1 * y2 - y1 * x2 < 0) ? -1 : 1;
            var dot = x1 * x2 + y1 * y2;

            // Add this to work with arbitrary vectors:
            // dot /= Math.Sqrt(x1 * x1 + y1 * y1) * Math.Sqrt(x2 * x2 + y2 * y2);

            // rounding errors, e.g. -1.0000000000000002 can screw up this
            if (dot > 1.0) { dot = 1.0F; }
            if (dot < -1.0) { dot = -1.0F; }

            return sign * Math.Acos(dot);
        }
        // Convert from endpoint to center parameterization,
        // see http://www.w3.org/TR/SVG11/implnote.html#ArcImplementationNotes
        //
        // Return [cx, cy, theta, delta_theta]
        //
        private static double[] GetArcCenter(double x1, double y1, double x2, double y2, double rx, double ry, bool isLarge, bool isSweep, double sin_phi, double cos_phi) {
            // Step 1.
            //
            // Moving an ellipse so origin will be the middlepoint between our two
            // points. After that, rotate it to line up ellipse axes with coordinate
            // axes.
            //
            var x1p = cos_phi * (x1 - x2) / 2 + sin_phi * (y1 - y2) / 2;
            var y1p = -sin_phi * (x1 - x2) / 2 + cos_phi * (y1 - y2) / 2;

            var rx_sq = rx * rx;
            var ry_sq = ry * ry;
            var x1p_sq = x1p * x1p;
            var y1p_sq = y1p * y1p;

            // Step 2.
            //
            // Compute coordinates of the centre of this ellipse (cx', cy')
            // in the new coordinate system.
            //
            double radicant = (rx_sq * ry_sq) - (rx_sq * y1p_sq) - (ry_sq * x1p_sq);

            if (radicant < 0) {
                // due to rounding errors it might be e.g. -1.3877787807814457e-17
                radicant = 0;
            }

            radicant /= (rx_sq * y1p_sq) + (ry_sq * x1p_sq);
            radicant = Math.Sqrt(radicant) * (isLarge == isSweep ? -1 : 1);

            var cxp = radicant * rx / ry * y1p;
            var cyp = radicant * -ry / rx * x1p;

            // Step 3.
            //
            // Transform back to get centre coordinates (cx, cy) in the original
            // coordinate system.
            //
            var cx = cos_phi * cxp - sin_phi * cyp + (x1 + x2) / 2;
            var cy = sin_phi * cxp + cos_phi * cyp + (y1 + y2) / 2;

            // Step 4.
            //
            // Compute angles (theta, delta_theta).
            //
            var v1x = (x1p - cxp) / rx;
            var v1y = (y1p - cyp) / ry;
            var v2x = (-x1p - cxp) / rx;
            var v2y = (-y1p - cyp) / ry;

            var theta = UnitVectorAngle(1, 0, v1x, v1y);
            var delta_theta = UnitVectorAngle(v1x, v1y, v2x, v2y);

            if (!isSweep && delta_theta > 0) {
                delta_theta -= TAU;
            }
            if (isSweep && delta_theta < 0) {
                delta_theta += TAU;
            }

            return new double[] { cx, cy, theta, delta_theta };
        }
        //
        // Approximate one unit arc segment with bezier curves,
        // see http://math.stackexchange.com/questions/873224
        //
        private static double[] ApproximateUnitArc(double theta, double delta_theta) {
            var alpha = Math.Tan(delta_theta / 4) * 4 / 3;
            var x1 = Math.Cos(theta);
            var y1 = Math.Sin(theta);
            var x2 = Math.Cos(theta + delta_theta);
            var y2 = Math.Sin(theta + delta_theta);

            return new double[] { x1, y1, x1 - y1 * alpha, y1 + x1 * alpha, x2 + y2 * alpha, y2 - x2 * alpha, x2, y2 };
        }
        /// <summary>
        /// Convert an arc to a sequence of cubic bezier curves
        /// </summary>
        /// <param name="ptF">Start point</param>
        /// <param name="rx">Ellipse X Radius</param>
        /// <param name="ry">Ellipse Y Radius</param>
        /// <param name="angle">xAxisRotate</param>
        /// <param name="isLarge">LargeArcFlag</param>
        /// <param name="isSweep">SweepFlag</param>
        /// <param name="x">End X</param>
        /// <param name="y">End Y</param>
        /// <returns>The cubic bezier curve points list</returns>
        public static List<double[]> ArcToBezier(PointF ptF, double rx, double ry, double angle, bool isLarge, bool isSweep, double x, double y) {
            List<double[]> lst = new List<double[]>();
            var sin_phi = Math.Sin(angle * TAU / 360);
            var cos_phi = Math.Cos(angle * TAU / 360);

            // Make sure radii are valid
            var x1p = cos_phi * (ptF.X - x) / 2 + sin_phi * (ptF.Y - y) / 2;
            var y1p = -sin_phi * (ptF.X - x) / 2 + cos_phi * (ptF.Y - y) / 2;

            if (x1p.Equals(0) && y1p.Equals(0)) {
                // we're asked to draw line to itself
                return lst;
            }

            if (rx.Equals(0) || ry.Equals(0)) {
                // one of the radii is zero
                return lst;
            }


            // Compensate out-of-range radii
            //
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);

            var lambda = (x1p * x1p) / (rx * rx) + (y1p * y1p) / (ry * ry);
            if (lambda > 1) {
                rx *= Math.Sqrt(lambda);
                ry *= Math.Sqrt(lambda);
            }


            // Get center parameters (cx, cy, theta1, delta_theta)
            //
            var cc = GetArcCenter(ptF.X, ptF.Y, x, y, rx, ry, isLarge, isSweep, sin_phi, cos_phi);

            var result = lst;
            var theta1 = cc[2];
            var delta_theta = cc[3];

            // Split an arc to multiple segments, so each segment
            // will be less than τ/4 (= 90°)
            //
            var segments = Math.Max(Math.Ceiling(Math.Abs(delta_theta) / (TAU / 4)), 1);
            delta_theta /= segments;

            for (var i = 0; i < segments; i++) {
                lst.Add(ApproximateUnitArc(theta1, delta_theta));
                theta1 += delta_theta;
            }

            // We have a bezier approximation of a unit circle,
            // now need to transform back to the original ellipse
            foreach (var p in lst) {
                for (var i = 0; i < p.Length; i += 2) {
                    double temp_x = p[i], temp_y = p[i + 1];
                    temp_x *= rx;
                    temp_y *= ry;

                    var xp = cos_phi * temp_x - sin_phi * temp_y;
                    var yp = sin_phi * temp_x + cos_phi * temp_y;
                    p[i] = xp + cc[0];
                    p[i + 1] = yp + cc[1];
                }
            }
            return lst;
        }
    }
}
