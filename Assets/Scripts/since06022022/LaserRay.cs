using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace since06022022
{
    public class LaserRay : MonoBehaviour
    {
        private LineRenderer lr;
        private Material mat;
        private RayManager rayManager;
        [FormerlySerializedAs("cancel")] public bool isKillable; // avoid micro rays
        public int id;

        private void Start()
        {
            lr = GetComponent<LineRenderer>();
            rayManager = GameObject.Find(Constants.RayManagerPath).GetComponent<RayManager>();
            isKillable = false;
            id = GetInstanceID();
        }

        public IEnumerator Propagate()
        {
            RaycastHit hit;
            Vector3 destination;
            
            rayManager.AddMovingRay(gameObject);
            Physics.Raycast(transform.position, transform.forward, out hit, 100.0f);
            destination = hit.point;
            
            while (!isKillable && transform.position != destination) // TODO other than strict equality for destination (x - y < 0.1)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination,
                    Constants.PropagationSpeed * Time.deltaTime);
                lr.SetPosition(1,
                    Vector3.MoveTowards(lr.GetPosition(1), destination, Constants.PropagationSpeed * Time.deltaTime));
                yield return new WaitForSeconds(0.001f);
            }
            rayManager.RemoveMovingRay(gameObject);
        }
        
        public IEnumerator Dissipate()
        {
            rayManager.AddMovingRay(gameObject);
            Vector3 rayVector2;
            while (!isKillable)
            {
                rayVector2 = transform.position - lr.GetPosition(0);
                if (rayVector2.magnitude < 0.3)
                {
                    SetKillable(true);
                }
                lr.SetPosition(0, Vector3.MoveTowards(lr.GetPosition(0), transform.position, Constants.PropagationSpeed * Time.deltaTime));
                yield return new WaitForSeconds(0.001f);
            }
            rayManager.TakeBackRay(gameObject);
            rayManager.RemoveMovingRay(gameObject);
            SetKillable(false);
        }

        public void SetKillable(bool killable)
        {
            isKillable = killable;
        }

        public void SetMaterial(Material mat)
        {
            lr.material = mat;
        }
        
        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
            lr.SetPosition(0, pos);
            lr.SetPosition(1, pos);
        }

        public void SetLrStartPos(Vector3 pos)
        {
            lr.SetPosition(0, pos);
        }
        
        public void SetEndPos(Vector3 pos)
        {
            transform.position = pos;
            lr.SetPosition(1, pos);
        }
    }
}