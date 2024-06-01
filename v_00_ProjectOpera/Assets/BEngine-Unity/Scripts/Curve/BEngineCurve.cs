using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BEngine {

    [DisallowMultipleComponent]
    public class BEngineCurve : MonoBehaviour
    {

        public BECurveSettings CurveSettings = new BECurveSettings();

        public List<CurveData> Curves = new List<CurveData>();


        void Awake()
        {
            if (CurveSettings.RemoveComponent) {
                Destroy(this);
            }
        }


        public void GenerateAllSplines() {
            foreach (var curve in Curves) {
                    curve.GenerateSplineSegments(curve.IsClosed, CurveSettings);
            }
        }


        public void ClearAllSplines() {
            foreach (var curve in Curves) {
                    curve.CurveSplineSegments = null;
            }
        }

    }
}
