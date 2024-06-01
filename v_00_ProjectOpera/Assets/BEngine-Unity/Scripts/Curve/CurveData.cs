using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BEngine {

    public enum CurveType {
        PolyLine,
        Spline
        // Bezier
    }

    [System.Serializable]
    public class CurveData
    {

        public List<CurvePoint> CurvePoints = new List<CurvePoint>();

        [HideInInspector]
        private List<SplineSegment> _CurveSplineSegments;
        public List<SplineSegment> CurveSplineSegments {get {return _CurveSplineSegments;} set {_CurveSplineSegments = value;}}

        public bool IsClosed = false;

        Vector3 GetPoint( int i ) => CurvePoints[i].position;

        public void GenerateSplineSegments(bool IsClosed, BECurveSettings curvSetts) {
            if (curvSetts.CurveDataType == CurveType.Spline) {
                CurveSplineSegments = new List<SplineSegment>();

                if(CurvePoints.Count > 1) {
                    int loopNumb;

                    if (IsClosed) {
                        loopNumb = CurvePoints.Count;
                    } else {
                        loopNumb = CurvePoints.Count - 1;
                    }

                    for( int i = 0; i < loopNumb; i++ ) {

                        var newSplineSegment = CreateSplineSegment( i, IsClosed, curvSetts);
                        CurveSplineSegments.Add(newSplineSegment);
                    }
                }
            }
            else {
                CurveSplineSegments = null;
            }
        }


        SplineSegment CreateSplineSegment( int pointID, bool isClosed, BECurveSettings curvSetts) {
            SplineSegment newSpline;

            int id0, id2, id3;
            id0 = pointID - 1;
            // id1 = pointID;
            id2 = pointID + 1;
            id3 = pointID + 2;

            Vector3 newPos1, newPos2, newPos3;
            
            if (id0 < 0) {

                if (isClosed) {
                    newPos1 = GetPoint(CurvePoints.Count - 1);
                }
                else {
                    newPos1 = Vector3.Normalize(GetPoint(0) - GetPoint(1));
                    float xLength = Vector3.Distance(GetPoint(0), GetPoint(1));
                    newPos1 = GetPoint(0) + (newPos1 * xLength * curvSetts.EndDistMultiplier);
                }

                // newSpline = new CatmullRomWikiCurve( newPos1, GetPoint(0), GetPoint(1), GetPoint(2), alpha, Step );
            }
            else {
                newPos1 = GetPoint(id0);
            }
            
            if (id2 > CurvePoints.Count - 1) {
                newPos2 = GetPoint(0);
            }
            else {
                newPos2 = GetPoint(id2);
            }

            if (id3 > CurvePoints.Count - 1) {

                if (isClosed) {
                    if (id3 == CurvePoints.Count + 1) {
                        newPos3 = GetPoint(1);
                    }
                    else {
                        newPos3 = GetPoint(0);
                    }
                }
                else {
                    newPos3 = Vector3.Normalize(GetPoint(pointID + 1) - GetPoint(pointID));
                    float x2Length = Vector3.Distance(GetPoint(pointID+1), GetPoint(pointID));
                    newPos3 = GetPoint(pointID + 1) + (newPos3 * x2Length * curvSetts.EndDistMultiplier);
                }

                // newSpline = new CatmullRomWikiCurve( GetPoint(pointID-1), GetPoint(pointID), GetPoint(pointID+1), newPos3, alpha, Step );
            }
            else {
                newPos3 = GetPoint(id3);
            }
            // else {
            // 	newSpline = new CatmullRomWikiCurve( GetPoint(pointID-1), GetPoint(pointID), GetPoint(pointID+1), GetPoint(pointID+2), alpha, Step );
            // }

            newSpline = new SplineSegment( newPos1, GetPoint(pointID), newPos2, newPos3, curvSetts);

            return newSpline;
        }


        public List<Vector3> GetPoints(bool isClosed, CurveType CurveDataType) {

            List<Vector3> allPoints = new List<Vector3>();

            // if (CurveDataType == CurveType.PolyLine) {
                foreach (var curvePoint in CurvePoints) {
                    allPoints.Add(curvePoint.position);
                }

                if (isClosed) {
                    allPoints.Add(CurvePoints[0].position);
                }
            // }
            // else if (CurveDataType == CurveType.Spline) {
            //     foreach (var splineSegment in CurveSplineSegments) {
            //         allPoints.AddRange(splineSegment.Points);
            //     }
            // }


            return allPoints;
        }

    }
}
