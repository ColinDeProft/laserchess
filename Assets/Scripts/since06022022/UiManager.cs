using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace since06022022
{
    public class UiManager : MonoBehaviour
    {
        public bool TurnOfPlayer1 { get; private set; } = true;
        private int starsCount1 = Constants.StartingScore1;
        private int starsCount2 = Constants.StartingScore2;
        private List<Receiver> receivers;
        private TextMeshProUGUI uiScore1;
        private TextMeshProUGUI uiScore2;
        private InGameActions igActions;
        private RayManager rayManager;
        private PieceManager pieceManager;
        private GameObject uiEndTurnButton;
        private const int invokedPiecePrice = 1;
        private int addedInvokedPiecePrice1 = invokedPiecePrice;
        private int addedInvokedPiecePrice2 = invokedPiecePrice;
        private TextMeshProUGUI invokedPiecePriceText;
        private const int movePiecePrice = 1;

        private void Start()
        {
            foreach (GameObject uiColorObject in GameObject.FindGameObjectsWithTag("UiColor")) // TODO constant "UirColorPlayer1", "UirColorPlayer2"
                switch (uiColorObject.name)
                {
                    case "UiColor1":
                        uiColorObject.GetComponent<Image>().color = Constants.Colors[0];
                        break;
                    case "UiColor2":
                        uiColorObject.GetComponent<Image>().color = Constants.Colors[1];
                        break;
                    case "UiEndTurnButton":
                        uiEndTurnButton = uiColorObject;
                        uiEndTurnButton.GetComponent<Image>().color = Constants.Colors[0];
                        uiEndTurnButton.GetComponent<Button>().onClick.AddListener(EndTurn);
                        break;
                    case "UiRestartButton":
                        uiColorObject.GetComponent<Button>().onClick.AddListener(ReloadScene);
                        break;
                }
            foreach (TextMeshProUGUI uiScore in GameObject.FindGameObjectsWithTag("UiScore").Select(go => go.GetComponent<TextMeshProUGUI>()).ToList())
                switch (uiScore.name)
                {
                    case "Score1":
                        uiScore1 = uiScore;
                        uiScore1.text = starsCount1 + "/" + Constants.WinningScore;
                        break;
                    case "Score2":
                        uiScore2 = uiScore;
                        uiScore2.text = starsCount2 + "/" + Constants.WinningScore;
                        break;
                    case "InvokedPrice":
                        invokedPiecePriceText = uiScore;
                        invokedPiecePriceText.text = invokedPiecePrice + "";
                        break;
                }
                
            receivers = new List<Receiver>();
            foreach (GameObject piece in GameObject.FindGameObjectsWithTag(Constants.PieceTag))
                receivers.Add(piece.transform.Find(Constants.ReceiverName).GetComponent<Receiver>());
            
            igActions = Camera.main.GetComponent<InGameActions>();
            rayManager = GameObject.Find(Constants.RayManagerPath).GetComponent<RayManager>();
            pieceManager = GameObject.Find(Constants.PieceManagerPath).GetComponent<PieceManager>();
        }

        private void UpdateEndTurnButtonColor()
        {
            uiEndTurnButton.GetComponent<Image>().color = TurnOfPlayer1 ? Constants.Colors[0] : Constants.Colors[1]; 
        }

        private void EndTurn()
        {
            if (igActions.PieceIsWarping() || rayManager.RaysAreMoving())
                return;
            int ownedPiecesCount = 0;
            if(TurnOfPlayer1) {
                foreach (Receiver receiver in receivers)
                    if(receiver.IsOwnedBy1())
                        ownedPiecesCount++;
                invokedPiecePriceText.text = addedInvokedPiecePrice2 + "";
            } else {
                foreach (Receiver receiver in receivers)
                    if(receiver.IsOwnedBy2())
                        ownedPiecesCount++;
                invokedPiecePriceText.text = addedInvokedPiecePrice1 + "";
            }
            GrantStars(ownedPiecesCount);
            TurnOfPlayer1 = !TurnOfPlayer1;
            igActions.NullifyPieceToWarp();
            UpdateEndTurnButtonColor();
            //StartCoroutine(igActions.RefillInvokers());
        }

        public void GrantStars(int count)
        {
            if (TurnOfPlayer1)
            {
                starsCount1 += count;
                uiScore1.text = starsCount1 + "/" + Constants.WinningScore;
            }
            else
            {
                starsCount2 += count;
                uiScore2.text = starsCount2 + "/" + Constants.WinningScore;
            }
        }
        
        public Boolean PayMovePiece(bool isInvokedPiece)
        {
            int price = isInvokedPiece ? (TurnOfPlayer1 ? addedInvokedPiecePrice1 : addedInvokedPiecePrice2) : movePiecePrice;
            if(HasEnoughStars(price)) {
                ConsumeStars(price);
                if(isInvokedPiece) {
                    if(TurnOfPlayer1) {
                        addedInvokedPiecePrice1 += 1;
                        invokedPiecePriceText.text = addedInvokedPiecePrice1 + "";
                    } else {
                        addedInvokedPiecePrice2 += 1;
                        invokedPiecePriceText.text = addedInvokedPiecePrice2 + "";
                    }
                }
                return true;
            } else {
                //redtext
                return false;
            }
        }

        private void ConsumeStars(int count)
        {
            if (TurnOfPlayer1)
            {
                starsCount1 -= count;
                uiScore1.text = starsCount1 + "/" + Constants.WinningScore;
            }
            else
            {
                starsCount2 -= count;
                uiScore2.text = starsCount2 + "/" + Constants.WinningScore;
            }
        }
        
        private Boolean HasEnoughStars(int count)
        {
            return TurnOfPlayer1 ? starsCount1 - count >= 0 : starsCount2 - count >= 0;
        }

        private void ReloadScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
        
        /*
        public void BuyPieceOverlay()
        {
            igActions.SetHasUiOverlay(true);
            buyOverlay.SetActive(true);
        }

        private void HandleBuyPiece(Constants.PieceType type, int price)
        {
            if (HasEnoughStars(price))
            {
                pieceToDispense = new Piece(type, price);
                ProposeRotation(true);
            }
        }
        
        private void ProposeRotation(bool propose)
        {
            buyPieceButs.ForEach(button => button.SetActive(!propose));
            rotateBut.ForEach(button => button.SetActive(propose));
        }

        private void HandleRotationChoice(int rotation)
        {
            pieceToDispense.Rotation = rotation;
            DispensePiece();
        }
        
        private void DispensePiece()
        {
            ConsumeStars(pieceToDispense.Price);
            GameObject piece = pieceManager.GivePiece(pieceToDispense.Type);
            Vector3 pieceEuler = piece.transform.eulerAngles;
            piece.transform.eulerAngles = new Vector3(pieceEuler.x, 0, pieceEuler.z);
            igActions.WarpInvokedPiece(pieceManager.GivePiece(pieceToDispense.Type), pieceToDispense.Rotation);
            HideOverlay();
            UpdatePieceAvailability();
        }

        private void HideOverlay()
        {
            pieceToDispense = null;
            buyOverlay.SetActive(false);
            ProposeRotation(false);
            igActions.SetHasUiOverlay(false);
        }
        
        private void UpdatePieceAvailability()
        {
            piecePrices.ForEach(price =>
                    price.transform.parent.parent.GetComponent<Button>().interactable =
                        (TurnOfPlayer1 ? starsCount1 : starsCount2) >= int.Parse(price.text) ? true : false);
            // in one line because yes
        }
        
        private void EndTurn()
        {
            if (igActions.PieceIsWarping() || rayManager.RaysAreMoving())
                return;
            if (TurnOfPlayer1)
            {
                foreach (Receiver receiver in receivers)
                    if(receiver.IsOwnedBy1()) 
                        starsCount1++;
                uiScore1.text = starsCount1 + "";
            }
            else
            {
                foreach (Receiver receiver in receivers)
                    if(receiver.IsOwnedBy2()) 
                        starsCount2++;
                uiScore2.text = starsCount2 + "";
            }
            TurnOfPlayer1 = !TurnOfPlayer1;
            igActions.NullifyPieceToWarp();
            HideOverlay();
            UpdatePieceAvailability();
            UpdateEndTurnButtonColor();
        }*/
        
    }
}