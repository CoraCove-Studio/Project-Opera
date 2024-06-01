using UnityEngine;

namespace BEngine
{
    [System.Serializable]
    public class CurvePoint
    {
        public Vector3 position;

        public CurvePoint(Vector3 newPoint) {
            position = newPoint;

        }
        
    }
}