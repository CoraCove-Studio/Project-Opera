using UnityEngine;
using System;


namespace BEngine
{

    [Serializable]
    public class BESettingsIcon
    {
        [Range(0, 5)]
        public float IconSize = 1f;
        public Vector3 IconOffset = Vector3.zero;
        public Color IconColor = new Color(1, 0.5f, 0, 0.5f);
    }
    
}