using System;
using UnityEngine;

namespace since06022022
{
    public class Laser : MonoBehaviour
    {
        private GameObject currentRay;
        private RayManager rayManager;
        private LaserRay currentRayScript;
        private RaycastHit hit;
        private float currentRayDistance;
        private Material currentMat;
        private GameObject parentPiece;
        private float previousParentRotation;

        private void Start()
        {
            rayManager = GameObject.Find(Constants.RayManagerPath).GetComponent<RayManager>();
            currentRayDistance = 0;
            parentPiece = transform.parent.parent.gameObject;
            previousParentRotation = parentPiece.transform.eulerAngles.y;
        }

        public void SetCurrentMaterial(Material mat)
        {
            currentMat = mat;
        }

        public void ShootRay()
        {
            Physics.Raycast(transform.position, transform.forward, out hit, 100.0f);
            //print("ray from " + transform.parent.gameObject.GetInstanceID() + " on " + hit.collider.name);
            if (hit.collider.name == "Receiver")
            {
                GetNewRay(hit);
                currentRayScript.SetPosition(transform.position + transform.forward);
                StartCoroutine(currentRayScript.Propagate());
            }
        }
        
        private void GetNewRay(RaycastHit hit)
        {
            currentRay = rayManager.GiveRay();
            currentRay.transform.rotation = transform.rotation;
            currentRayDistance = hit.distance;
            currentRayScript = currentRay.GetComponent<LaserRay>();
            currentRayScript.SetMaterial(currentMat);
        }

        public void UpdateAfterPieceMoved()
        {
            Physics.Raycast(transform.position, transform.forward, out hit, 100.0f);
            Vector3 hitPoint = hit.point;
            Boolean colliderIsReceiver = hit.collider.name == "Receiver";
            
            if (currentRayScript) // I currently have a ray colliding a receiver
            {
                if (hit.distance < currentRayDistance)
                {
                    if (Math.Abs(previousParentRotation - parentPiece.transform.eulerAngles.y) < 0.001)
                        // my parent didnt rotate : new obstacle breaks the ray
                    {
                        // break the ray in two parts
                        
                        // second part (current ray) dissipates behind obstacle
                        currentRayScript.SetLrStartPos(hitPoint + transform.forward);
                        StartCoroutine(currentRayScript.Propagate()); // if target moved
                        BreakRay();
                    
                        // first part is a new ray
                        GetNewRay(hit);
                        currentRayScript.SetPosition(hitPoint);
                        currentRayScript.SetLrStartPos(transform.position + transform.forward);
                    
                        // dissipate first part too if no connection to keep
                        if (!colliderIsReceiver) BreakRay();
                    }
                    else // my parent did rotate : must dissipate current ray, eventually shoot
                    {
                        BreakRay();
                        if (colliderIsReceiver) ShootRay();
                    }
                }
                else
                {
                    if (hit.distance > currentRayDistance)
                    {
                        if (Math.Abs(previousParentRotation - parentPiece.transform.eulerAngles.y) < 0.001)
                            // my parent didnt rotate : previous target not there anymore
                        {
                            StartCoroutine(currentRayScript.Propagate());
                            if (!colliderIsReceiver) BreakRay();
                        }
                        else
                            // my parent did rotate : must dissipate current ray, eventually shoot
                        {
                            StartCoroutine(currentRayScript.Propagate()); // if target moved
                            BreakRay();
                            if (colliderIsReceiver) ShootRay();
                        }
                    }
                    else // still current target but may have been rotated (so no more receiver)
                    {
                        if (!colliderIsReceiver) BreakRay();
                    }
                }
            }
            else // I don't have a ray, may need to shoot one
            {
                if (colliderIsReceiver) ShootRay();
            }
            currentRayDistance = hit.distance;
            previousParentRotation = parentPiece.transform.eulerAngles.y;
        }
        
        public void BreakRay()
        {
            if(currentRayScript) StartCoroutine(currentRayScript.Dissipate());
            currentRay = null;
            currentRayScript = null;
        }

    }
}