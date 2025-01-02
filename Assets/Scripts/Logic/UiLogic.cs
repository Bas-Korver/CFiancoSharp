using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Logic
{
    public class UiLogic : MonoBehaviour
    {
        public TMP_InputField boardSizeInput;
        public TMP_Text warningText;
        public GameObject controller;
        public TMP_Dropdown whitePlayerType;
        public TMP_Dropdown blackPlayerType;
        
        public void ChangeWhitePlayerType(int value)
        {
            Enum.TryParse<PlayerType>(whitePlayerType.options[value].text, out PlayerType playerType);
            controller = GameObject.FindGameObjectWithTag("GameController");
            controller.GetComponent<Game>().SetPlayerType(PlayerColour.White, playerType);
        } 
        
        public void ChangeBlackPlayerType(int value)
        {
            Enum.TryParse<PlayerType>(blackPlayerType.options[value].text, out PlayerType playerType);
            controller = GameObject.FindGameObjectWithTag("GameController");
            controller.GetComponent<Game>().SetPlayerType(PlayerColour.Black, playerType);
        }
    
        public void SaveBoardSize()
        {
            controller = GameObject.FindGameObjectWithTag("GameController");
            
            string value = boardSizeInput.text;
            if (IsInteger(value) && LegalBoardSize(int.Parse(value)))
            {
                controller.GetComponent<Game>().SetBoardSize(int.Parse(boardSizeInput.text));
                Reload();
                controller.GetComponent<Game>().Start();
                warningText.enabled = false;
            }
        }
        
        public void ReloadScene()
        {
            SceneManager.LoadScene("Game");
        }


        private bool IsInteger(string boardSize)
        {
            if (int.TryParse(boardSize, out _)) return true;
            
            Debug.Log("The board size must be an integer.");
            warningText.enabled = true;
            return false;
        }

        
        private bool LegalBoardSize(int boardSize)
        {
            if (boardSize % 2 == 0)
            {
                warningText.enabled = true;
                Debug.Log("The board size must be an odd number.");
                return false;
            }
            if (boardSize < 5)
            {
                warningText.enabled = true;
                Debug.Log("The board size must be at least 5.");
                return false;
            }
            if (boardSize > 25)
            {
                warningText.enabled = true;
                Debug.Log("The board size must be at most 25.");
                return false;
            }
            
            return true;
        }

        private void Reload()
        {
            // Find objects with tag GameTile, CoordinateBox, GamePiece, and MovePlate and destroy them.
            GameObject[] gameTiles = GameObject.FindGameObjectsWithTag("GameTile");
            GameObject[] coordinateBoxes = GameObject.FindGameObjectsWithTag("CoordinateBox");
            GameObject[] gamePieces = GameObject.FindGameObjectsWithTag("GamePiece");
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
            
            controller.GetComponent<Game>().SetPlayer(PlayerColour.White);
            for (int i = 0; i < gameTiles.Length; i++)
            {
                Destroy(gameTiles[i]);
            }
            for (int i = 0; i < coordinateBoxes.Length; i++)
            {
                Destroy(coordinateBoxes[i]);
            }
            for (int i = 0; i < gamePieces.Length; i++)
            {
                Destroy(gamePieces[i]);
            }
            for (int i = 0; i < movePlates.Length; i++)
            {
                Destroy(movePlates[i]);
            }
        }
    }
}

