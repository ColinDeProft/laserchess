using System;
using System.Collections;
using UnityEngine;

namespace since06022022
{
    public class Receiver : MonoBehaviour
    {
        private Emitter emitterScript;
        [SerializeField] public int masterColor;
        private Material rayMat1;
        private Material rayMat2;
        public int lastMasterColor;
        private GameObject light1;
        private GameObject light2;
        private GameObject lightWhite;
        [SerializeField] public int incomingLaserCount;
        private float confirmDuration;
        // anti-loop stuff
        // made to prevent perpetual moving rays, not to prevent "self-powered"
        [SerializeField] private Vector3 lastColPos;
        [SerializeField] private int lastColTriggerCount;
        private RayManager rayManager;
        public bool antiLoopState;
        public int id;
        private InGameActions inGameActions;
        private PieceManager pieceManager;

        private void Start()
        {
            masterColor = 0;
            emitterScript = transform.parent.Find(Constants.EmitterName).GetComponent<Emitter>();
            rayMat1 = Resources.Load("Materials/" + Constants.RayMat1Name) as Material;
            rayMat2 = Resources.Load("Materials/" + Constants.RayMat2Name) as Material;
            lastMasterColor = 0;
            light1 = transform.parent.Find(Constants.InsideLightGroupName).Find(Constants.InsideLight1Name).gameObject;
            light2 = transform.parent.Find(Constants.InsideLightGroupName).Find(Constants.InsideLight2Name).gameObject;
            lightWhite = transform.parent.Find(Constants.InsideLightGroupName).Find("White").gameObject;
            light1.GetComponent<Light>().color = Constants.Colors[0];
            light2.GetComponent<Light>().color = Constants.Colors[1];
            incomingLaserCount = 0;
            lastColPos = Vector3.zero;
            lastColTriggerCount = 0;
            rayManager = GameObject.Find(Constants.RayManagerPath).GetComponent<RayManager>();
            antiLoopState = false;
            id = GetInstanceID();
            inGameActions = Camera.main.GetComponent<InGameActions>();
            pieceManager = GameObject.Find(Constants.PieceManagerPath).GetComponent<PieceManager>();
        }

        private void OnTriggerEnter(Collider otherCol)
        {
            if (otherCol.GetComponent<LaserRay>())
            {
                Material otherRayMat = otherCol.GetComponent<LineRenderer>().material;
                masterColor = otherRayMat.mainTexture == rayMat1.mainTexture ? masterColor + 1 : masterColor - 1;
                incomingLaserCount++;
                NotifyEmitterAfterTrigger();

                Vector3 vec = lastColPos - otherCol.transform.position;
                float mag = vec.magnitude;
                if (mag < 0.2f && lastColPos != Vector3.zero) // TODO use constant names
                {
                    lastColTriggerCount++;
                }
                lastColPos = otherCol.transform.position;
                if (lastColTriggerCount == 2 && transform.parent.parent.tag != Constants.TileKingTag)
                { // same laser collided piece 3 times : avoid laser loop by removing piece if not king
                    //StartCoroutine(vibrate());
                    rayManager.ResetAllAntiLoop();
                    GameObject thisPiece = transform.parent.gameObject;
                    StartCoroutine(inGameActions.Warp(thisPiece, pieceManager.transform.gameObject, false));
                    pieceManager.TakeBackPiece(thisPiece);
                    print("Triggered antiloop from " + transform.parent.GetInstanceID());
                }
            }
        }

        private IEnumerator vibrate()
        {
            Transform model = transform.parent.Find("Model");
            Vector3 initialPosition = model.localPosition;
            var maxTrembleTime = 5f;
            var currentTime = 0.0f;
            var speed = 20f;
            var intensity = 0.2f;

            while(currentTime < maxTrembleTime)
            {
                currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, maxTrembleTime);

                model.localPosition = new Vector3(
                    intensity * Mathf.PerlinNoise(speed * Time.time, 1),
                    initialPosition.y,
                    intensity * Mathf.PerlinNoise(speed * Time.time, 3)
                    );

                yield return null; // wait until next frame
            }
            model.localPosition = initialPosition;
        }
        
        // TODO : fix issue with this & RayManager.TakeBackRay : 
        // ray can be made reusable before the method content executes, resulting in possibly wrong otherRayMat Color
        // possible sol, do all this in the laser end method not this triggerexit
        private void OnTriggerExit(Collider otherCol)
        {
            if (otherCol.GetComponent<LaserRay>())
            {
                Material otherRayMat = otherCol.GetComponent<LineRenderer>().material;
                masterColor = otherRayMat.mainTexture == rayMat1.mainTexture ? masterColor - 1 : masterColor + 1; // todo map laser texture with player color for adaptability?
                incomingLaserCount--;
                NotifyEmitterAfterTrigger();
            }
        }

        private void NotifyEmitterAfterTrigger()
        {
            if (lastMasterColor == 0)
            {
                light1.SetActive(masterColor > 0 ? true : false);
                light2.SetActive(masterColor < 0 ? true : false);
                lightWhite.SetActive(masterColor != 0 ? true : false);
                Material masterMat = masterColor > 0 ? rayMat1 : rayMat2;
                emitterScript.Activate(masterMat);
            }
            else
            {
                if (masterColor == 0)
                {
                    light1.SetActive(incomingLaserCount != 0 ? true : false);
                    light2.SetActive(incomingLaserCount != 0 ? true : false);
                    lightWhite.SetActive(incomingLaserCount != 0 ? true : false);
                    emitterScript.Deactivate();
                }
            }
            lastMasterColor = masterColor;
        }

        public void NotifyEmitterAfterPieceMoved()
        {
            if (masterColor != 0)
            {
                emitterScript.NotifyLasersAfterPieceMoved();
            }
        }

        public void ResetAntiLoop()
        {
            lastColPos = Vector3.zero;
            lastColTriggerCount = 0;
        }
        
        public int GetMasterColor()
        {
            return masterColor;
        }
        
        public Boolean IsOwnedBy1()
        {
            //return incomingLaserCount > 0 && masterColor >= 0;
            return masterColor > 0;
        }
        
        public Boolean IsOwnedBy2()
        {
            //return incomingLaserCount > 0 && masterColor <= 0;
            return masterColor < 0;
        }
        public Boolean IsNeutral()
        {
            return incomingLaserCount > 0 && masterColor == 0;
        }
    }
}