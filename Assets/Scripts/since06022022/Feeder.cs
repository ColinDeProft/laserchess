using System;
using System.Collections;
using UnityEngine;

namespace since06022022
{
    public class Feeder : MonoBehaviour
    {
        private Emitter emitterScript;
        [SerializeField] public int masterColor;
        private Material rayMat1;
        private Material rayMat2;

        private void Start()
        {
            emitterScript = transform.parent.Find(Constants.EmitterName).GetComponent<Emitter>();
            rayMat1 = Resources.Load("Materials/" + Constants.RayMat1Name) as Material;
            rayMat2 = Resources.Load("Materials/" + Constants.RayMat2Name) as Material;
            Material masterMat = masterColor > 0 ? rayMat1 : rayMat2;
            emitterScript.Activate(masterMat);
        }

        public void NotifyFeederAfterPieceMoved()
        {
            emitterScript.NotifyLasersAfterPieceMoved();
        }
        
    }
}