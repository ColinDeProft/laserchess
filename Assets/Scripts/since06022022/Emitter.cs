using System;
using System.Collections.Generic;
using UnityEngine;

namespace since06022022
{
    public class Emitter : MonoBehaviour
    {
        private List<Laser> shootersScripts;

        private void Start()
        {
            shootersScripts = new List<Laser>();
            foreach (Transform shooter in transform)
            {
                shootersScripts.Add(shooter.GetComponent<Laser>());
            }
        }

        public void NotifyLasersAfterPieceMoved()
        {
            foreach (Laser shooter in shootersScripts)
            {
                shooter.UpdateAfterPieceMoved();
            }    
        }
        
        public void Activate(Material mat)
        {
            foreach (Laser shooter in shootersScripts)
            {
                shooter.SetCurrentMaterial(mat);
                shooter.ShootRay();
            }  
        }

        public void Deactivate()
        {
            foreach (Laser shooter in shootersScripts)
            {
                shooter.BreakRay();
            }
        }
    }
}