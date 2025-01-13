using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace since06022022
{
    public class InGameActions : MonoBehaviour
    {
        // quand y aura reset partie, increase "near" de camera pour reset visually
        private bool hasPieceSelected;
        private RayManager rayManager;
        private bool pieceIsWarping;
        private GameObject pieceToWarp;
        private Material pieceToWarpMat;
        private Material pieceMat;
        private UiManager uiManager;
        private PieceManager pieceManager;
        private GameObject invoker;

        private void Start()
        {
            rayManager = GameObject.Find(Constants.RayManagerPath).GetComponent<RayManager>();
            pieceManager = GameObject.Find(Constants.PieceManagerPath).GetComponent<PieceManager>();
            pieceIsWarping = false;
            pieceToWarp = null;
            pieceToWarpMat = Resources.Load("Materials/" + Constants.TurretHighlightMatName) as Material;
            pieceMat = Resources.Load("Materials/" + Constants.TurretBodyMatName) as Material;
            uiManager = GameObject.Find("UI").GetComponent<UiManager>(); // TODO CONST
            invoker = GameObject.FindGameObjectsWithTag(Constants.InvokerTag)[0];
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                if (!(rayManager.RaysAreMoving() || pieceIsWarping))
                    HandleClick();
        }
        /*
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                if (!(rayManager.RaysAreMoving() || pieceIsWarping) && !hasUiOverlay)
                    HandleClick();
        }*/

        private void HandleClick()
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 400.0f);
            GameObject target = hit.transform.gameObject;

            if (target.CompareTag(Constants.WallTag))
            {   // clicked wall when attempting to click piece
                target = target.transform.parent.gameObject;
            }
            if ((target.CompareTag(Constants.TileTag) || target.CompareTag(Constants.TileKingTag) || target.CompareTag(Constants.InvokerTag)) && target.transform.childCount > 0)
            {   // clicked parent tile when attempting to click piece
                foreach (Transform child in target.transform)
                { // assign child piece
                    if (child.CompareTag(Constants.PieceTag)) {
                        target = child.gameObject;
                    }
                }
            }
            if (target.CompareTag(Constants.PieceTag))
            { // clicking piece
                if (target == pieceToWarp)
                //if (target == pieceToWarp && uiManager.HasEnoughStars(1))
                { // clicked twice -> rotate
                    StartCoroutine(Warp(pieceToWarp, target.transform.parent.gameObject, true));
                    NullifyPieceToWarp();
                    //uiManager.ConsumeStars(1);
                }
                else
                {
                    Receiver receiver = target.transform.Find("Receiver").GetComponent<Receiver>();
                    int pieceMasterColor = receiver.GetMasterColor();
                    bool pieceIsNeutral = receiver.IsNeutral();
                    bool turnOfPlayer1 = uiManager.TurnOfPlayer1;
                    if (!pieceIsNeutral && (turnOfPlayer1 && pieceMasterColor >= 0 || !turnOfPlayer1 && pieceMasterColor <= 0))
                    { // clicking new piece -> unlight old, highlight new
                        NullifyPieceToWarp();
                        pieceToWarp = target;
                        ChangePieceModelMat(pieceToWarp, pieceToWarpMat);
                    }
                }
            }
            else if (pieceToWarp && pieceToWarp.transform.parent.tag != Constants.TileKingTag)
            { // moving piece which is not king
                bool isInvokedPiece = pieceToWarp.transform.parent.tag == Constants.InvokerTag;
                if (target.CompareTag(Constants.TileTag))
                { // clicking tile
                    if (uiManager.PayMovePiece(isInvokedPiece)) {
                        StartCoroutine(Warp(pieceToWarp, target, false));
                        NullifyPieceToWarp();
                        if (isInvokedPiece) {
                            StartCoroutine(RefillInvokers());
                        }
                    }
                }
                else if (target.CompareTag(Constants.SellerTag) && !isInvokedPiece)
                { // selling piece
                    StartCoroutine(Warp(pieceToWarp, pieceManager.transform.gameObject, false));
                    pieceManager.TakeBackPiece(pieceToWarp);
                    NullifyPieceToWarp();
                    uiManager.GrantStars(1);
                }
            }
        }

        public IEnumerator Warp(GameObject piece, GameObject destinationTile, bool doRotate) // TODO 1st arg not needed + use transform not go
        {
            pieceIsWarping = true;
            BoxCollider colReceiver = piece.transform.Find("Receiver").gameObject.GetComponent<BoxCollider>();
            Vector3 colSizeBeforeShrink = colReceiver.size;
            colReceiver.size = colSizeBeforeShrink / 2;
            yield return StartCoroutine(VisualResize(piece, true));

            rayManager.ResetAllAntiLoop(); // ?

            Vector3 destPosition = destinationTile.transform.position;
            Vector3 pieceEuler = piece.transform.eulerAngles;

            piece.transform.position = destPosition;
            piece.transform.eulerAngles = new Vector3(pieceEuler.x, pieceEuler.y + (doRotate ? 45 : 0), pieceEuler.z);
            piece.transform.SetParent(destinationTile.transform);

            yield return StartCoroutine(VisualResize(piece, false));
            colReceiver.size = colSizeBeforeShrink;

            StartCoroutine(rayManager.RaycastParty());

            pieceIsWarping = false;
        }

        private IEnumerator VisualResize(GameObject piece, Boolean shrink)
        {
            Transform model = piece.transform.Find("Model");
            float curScale = model.transform.localScale.x;
            if (shrink)
            {
                while (curScale > 0.4f)
                {
                    curScale -= 0.1f;
                    model.localScale = new Vector3(curScale, curScale, curScale);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                while (curScale < 1f)
                {
                    curScale += 0.1f;
                    model.localScale = new Vector3(curScale, curScale, curScale);
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        private void ChangePieceModelMat(GameObject piece, Material mat)
        {
            piece.transform.Find("Model/Body").GetComponent<Renderer>().material = mat;
            foreach (Transform shooterModel in piece.transform.Find("Model/Guns").transform) {
                shooterModel.GetComponent<Renderer>().material = mat;
            }
        }

        public Boolean PieceIsWarping()
        {
            return pieceIsWarping;
        }

        public void NullifyPieceToWarp()
        {
            if (!pieceToWarp)
                return;
            ChangePieceModelMat(pieceToWarp, pieceMat);
            pieceToWarp = null;
        }

        public IEnumerator RefillInvokers()
        {
            GameObject piece = pieceManager.GivePiece();
            if (piece != null) {
                yield return StartCoroutine(VisualResize(piece, true));
                piece.transform.position = invoker.transform.position;
                piece.transform.SetParent(invoker.transform);
                yield return StartCoroutine(VisualResize(piece, false));
            }
        }

        /*
        public void SetHasUiOverlay(bool has)
        {
            hasUiOverlay = has;
        }
        
        public void WarpInvokedPiece(GameObject piece, int angle)
        {
            StartCoroutine(Warp(piece, selectedInvoker, angle));
            selectedInvoker = null;
        }*/
    }
}