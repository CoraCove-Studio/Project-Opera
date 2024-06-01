using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;

namespace BEngine
{

    [Serializable]
    public class BEComponentSettings
    {

        [Tooltip("Remove Component When a Game runs. As this component stores lots of data.")]
        public bool RemoveComponent = true;

        [Space]
        
        // [Header("Networking")]
        // [Tooltip("How many minutes Unity should wait for Blender's message from a Server.")]
        // [Range(1, 50)]
        // public float timeOutMinutes = 3; 

        [Space]

        public BESettingsIcon icon = new BESettingsIcon();

        [Tooltip("It Runs Blender with GUI. It works only with RunBlender Mode")]
        public bool BackgroundMode = true;

    }

}