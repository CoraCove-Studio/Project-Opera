using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BEngine
{
    // a single catmull-rom curve

    [System.Serializable]
    public struct SplineSegment {
        public List<Vector3> Points;

        public SplineSegment( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, BECurveSettings curvSetts) {

            Points = new List<Vector3>();
            GenerateSplinePoints(p0, p1, p2, p3, curvSetts);

        }


        void GenerateSplinePoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, BECurveSettings curvSetts) {
            float segmentLength = Vector3.Distance(p1, p2);
            int loopLength = (int)Mathf.Floor(segmentLength / curvSetts.CurveStep) + 1;  // We add the last point also
            loopLength = Mathf.Max(loopLength, 4);

            for( int i = 0; i < loopLength; i++ ) {
                float t = i / ( loopLength - 1f );

                Points.Add(GetPoint(p0, p1, p2, p3, t, curvSetts.SplineAlpha));
            }
        }

        // Evaluates a point at the given t-value from 0 to 1
        public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, float splineAlpha) {
            // calculate knots
            const float k0 = 0;
            float k1 = GetKnotInterval( p0, p1, splineAlpha);
            float k2 = GetKnotInterval( p1, p2, splineAlpha) + k1;
            float k3 = GetKnotInterval( p2, p3, splineAlpha) + k2;

            // evaluate the point
            float u = Mathf.LerpUnclamped( k1, k2, t );
            Vector3 A1 = Remap( k0, k1, p0, p1, u );
            Vector3 A2 = Remap( k1, k2, p1, p2, u );
            Vector3 A3 = Remap( k2, k3, p2, p3, u );
            Vector3 B1 = Remap( k0, k2, A1, A2, u );
            Vector3 B2 = Remap( k1, k3, A2, A3, u );

            return Remap( k1, k2, B1, B2, u );
        }

        static Vector3 Remap( float a, float b, Vector3 c, Vector3 d, float u ) {
            return Vector3.LerpUnclamped( c, d, ( u - a ) / ( b - a ) );
        }

        float GetKnotInterval( Vector3 a, Vector3 b, float splineAlpha) {
            return Mathf.Pow( Vector3.SqrMagnitude( a - b ), 0.5f * splineAlpha );
        }

    }
}