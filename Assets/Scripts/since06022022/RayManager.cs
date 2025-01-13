using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace since06022022
{
    public class RayManager : MonoBehaviour
    {
        private Queue<GameObject> availableRays;
        private LaserRay rayScript;
        public List<GameObject> movingRays;
        private List<Receiver> receivers;
        private List<Feeder> feeders;

        private void Start()
        {
            availableRays = new Queue<GameObject>();
            foreach (Transform ray in transform)
                availableRays.Enqueue(ray.gameObject);
            movingRays = new List<GameObject>();
            receivers = new List<Receiver>();
            feeders = new List<Feeder>();
            foreach (GameObject piece in GameObject.FindGameObjectsWithTag(Constants.PieceTag))
                receivers.Add(piece.transform.Find(Constants.ReceiverName).GetComponent<Receiver>());
            foreach (GameObject feederObject in GameObject.FindGameObjectsWithTag("Feeder"))
                feeders.Add(feederObject.transform.Find("Feeder").GetComponent<Feeder>());
        }

        public GameObject GiveRay()
        {
            return availableRays.Dequeue();
        }

        public void TakeBackRay(GameObject ray)
        {
            rayScript = ray.GetComponent<LaserRay>();
            rayScript.SetPosition(transform.position);
            availableRays.Enqueue(ray.gameObject);
        }

        public void AddMovingRay(GameObject ray)
        {
            movingRays.Add(ray);
        }
        
        public void RemoveMovingRay(GameObject ray)
        {
            movingRays.Remove(ray);
        }

        public bool RaysAreMoving()
        {
            return movingRays.Count != 0;
        }
        
        public IEnumerator RaycastParty() // TODO put in igactions
        {
            //yield return new WaitForSeconds((float) 0.3);
            yield return new WaitForSeconds((float) 0.1);
            foreach (Feeder feeder in feeders)
            {
                feeder.NotifyFeederAfterPieceMoved();
            }
            foreach (Receiver receiver in receivers)
            {
                receiver.NotifyEmitterAfterPieceMoved();
            }
        }
        
        public void ResetAllAntiLoop() // TODO put in igactions
        {
            foreach (Receiver receiver in receivers)
                receiver.ResetAntiLoop();
        }
    }
}