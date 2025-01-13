using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace since06022022
{
    public class PieceManager : MonoBehaviour
    {
        private Stack<GameObject> availableTurrets;

        private void Start()
        {
            availableTurrets = new Stack<GameObject>();
            foreach (Transform turret in transform)
                availableTurrets.Push(turret.gameObject);
        }

        public GameObject GivePiece()
        {
            GameObject toReturn = null;
            try {
                toReturn = availableTurrets.Pop();
            } catch(InvalidOperationException e) {
                print(e);
            }
            return toReturn;
        }
        /*
        public GameObject GivePiece(Constants.PieceType pieceType)
        {
            GameObject toReturn = null;
            switch (pieceType)
            {
                case Constants.PieceType.Turret:
                    try {
                        toReturn = availableTurrets.Pop();
                    } catch(InvalidOperationException e) {
                        print(e);
                    }
                    break;
            }
            return toReturn;
        }*/

        public void TakeBackPiece(GameObject piece)
        {
            availableTurrets.Push(piece);
            // reset rotation
            Vector3 pieceEuler = piece.transform.eulerAngles;
            piece.transform.eulerAngles = new Vector3(pieceEuler.x, 0, pieceEuler.z);
        }
        /*
        public void TakeBackPiece(Piece piece)
        {
            Transform storage = transform;
            GameObject pieceGameObject = piece.GameObject;
            switch (piece.Type)
            {
                case Constants.PieceType.Turret:
                // todo use warp and use this method instead of antiloop code
                    storage = turretStorage;
                    availableTurrets.Push(pieceGameObject);
                    break;
            }
            pieceGameObject.transform.position = transform.position;
            pieceGameObject.transform.SetParent(storage);
        }*/
    }
}