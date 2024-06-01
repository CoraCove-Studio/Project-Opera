using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BEngine {


    [System.Serializable]
    public class BECurveSettings
    {

        [Header("Main Settings")]

        [Tooltip("Remove Component When a Game runs. As this component stores lots of data.")]
        public bool RemoveComponent = true;

        [Space]

        public CurveType CurveDataType = CurveType.PolyLine;
        public bool SnapNewPoints = false;

        [Header("Spline Settings")]
        [Min(0.0001f)]
        public float CurveStep = 1;

        [Range(0.001f, 1f)]
        public float EndDistMultiplier = 0.02f;
        [Range(0, 1)]

        public float SplineAlpha = 0.5f;

    }

}