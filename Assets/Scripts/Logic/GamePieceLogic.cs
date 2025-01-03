using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class GamePieceLogic : MonoBehaviour
    {
        // References to the game objects.
        public GameObject controller;
        public GameObject movePlate;

        // Positions of the game piece in board coordinates and instantiate the piece outside the board.
        private int _xBoard = -1;
        private int _yBoard = -1;
        private int _boardSize;
        private double _startPosition;

        private int _yDir;

        // Sprites that the game piece can show.
        public Sprite blackPiece;
        public Sprite whitePiece;

        public void Activate()
        {
            controller = GameObject.FindGameObjectWithTag("GameController");

            // Use the set board coordinates and transform them to world coordinates.
            SetCoords();

            switch (this.name)
            {
                case nameof(PlayerColour.White):
                    this.GetComponent<SpriteRenderer>().sprite = whitePiece;
                    _yDir = 1;
                    break;
                case nameof(PlayerColour.Black):
                    this.GetComponent<SpriteRenderer>().sprite = blackPiece;
                    _yDir = -1;
                    break;
            }
        }

        public void SetCoords()
        {
            _startPosition = -(_boardSize / 2.0 - 0.5);

            this.transform.position =
                new Vector3((float)(_startPosition + _xBoard), (float)(_startPosition + _yBoard), -1.0f);
        }

        public int GetXboard()
        {
            return _xBoard;
        }

        public int GetYboard()
        {
            return _yBoard;
        }

        public void SetXboard(int x)
        {
            _xBoard = x;
        }

        public void SetYboard(int y)
        {
            _yBoard = y;
        }

        public void SetBoardSize(int size)
        {
            _boardSize = size;
        }

        private void OnMouseUpAsButton()
        {
            Enum.TryParse<PlayerColour>(this.name, out PlayerColour playerColour);
            if (!controller.GetComponent<Game>().IsGameOver() &&
                controller.GetComponent<Game>().GetCurrentPlayer().ToString() == this.name &&
                controller.GetComponent<Game>().GetPlayerType(playerColour) == PlayerType.Manual)
            {
                DestroyMovePlates();

                bool capture = CheckCaptureMoves();
                Game game = controller.GetComponent<Game>();
            
                if (capture)
                {
                    if (game.CapturePosition(_xBoard, _yBoard, -1, _yDir, gameObject))
                    {
                        MovePlateSpawn(_xBoard - 1, _yBoard + _yDir, -1, _yDir, true);
                    }
                    if (game.CapturePosition(_xBoard, _yBoard, +1, _yDir, gameObject))
                    {
                        MovePlateSpawn(_xBoard + 1, _yBoard + _yDir, 1, _yDir, true);
                    }
                }
                else
                {
                    PieceMovePlate();
                }
            }
            
            // string[,] convertedState = controller.GetComponent<AI>().ConvertBoardState(controller.GetComponent<Game>().GetBoardState());
            // List<Move> moves = controller.GetComponent<AI>().GenerateMoves(convertedState);
            // foreach (Move move in moves)
            // {
            //     Debug.Log(controller.GetComponent<Game>().GetBoardState()[move.XBoardStart, move.YBoardStart]);
            //     Debug.Log("Player: " + move.playerColour + ", X start: " + move.XBoardStart + ", Y start: " + move.YBoardStart  + ", X end: " + move.XBoardEnd  + ", Y end: " + move.YBoardEnd);
            // }
            // Debug.Log("End");
        }

        public void DestroyMovePlates()
        {
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
            for (int i = 0; i < movePlates.Length; i++)
            {
                Destroy(movePlates[i]);
            }
        }

        private bool CheckCaptureMoves()
        {
            Game game = controller.GetComponent<Game>();
            GameObject[] capturePositions = game.GetCapturePositions();

            if (!game.GetCaptureChecked())
            {
                GameObject[] playerPieces = this.name switch
                {
                    nameof(PlayerColour.White) => game.GetWhitePlayerPieces(),
                    nameof(PlayerColour.Black) => game.GetBlackPlayerPieces(),
                    _ => new GameObject[] { }
                };

                for (int i = 0; i < playerPieces.Length; i++)
                {
                    if (playerPieces[i] != null)
                    {
                        int xBoard = playerPieces[i].GetComponent<GamePieceLogic>().GetXboard();
                        int yBoard = playerPieces[i].GetComponent<GamePieceLogic>().GetYboard();

                        if (game.CapturePosition(xBoard, yBoard, -1, _yDir, gameObject))
                        {
                            if (capturePositions[0] == null)
                            {
                                capturePositions[0] = gameObject;
                            }
                            else if (capturePositions[1] == null)
                            {
                                capturePositions[1] = gameObject;
                            }
                            // Should not be triggerd
                            else
                            {
                                Debug.LogError("Error: More than two capture positions found.");
                            }
                        }
                        if (game.CapturePosition(xBoard, yBoard, 1, _yDir, gameObject))
                        {
                            if (capturePositions[0] == null)
                            {
                                capturePositions[0] = gameObject;
                            }
                            else if (capturePositions[1] == null)
                            {
                                capturePositions[1] = gameObject;
                            }
                            // Should not be triggerd
                            else
                            {
                                Debug.LogError("Error: More than two capture positions found.");
                            }
                        }
                    }
                }
            }

            game.SetCapturePositions(capturePositions);
            game.SetCaptureChecked(true);

            if (capturePositions[0] != null || capturePositions[1] != null)
            {
                return true;
            }
            return false;
        }

        private void PieceMovePlate()
        {
            PointMovePlate(_xBoard, _yBoard + _yDir);
            PointMovePlate(_xBoard - 1, _yBoard);
            PointMovePlate(_xBoard + 1, _yBoard);
        }

        public void PointMovePlate(int x, int y)
        {
            Game game = controller.GetComponent<Game>();

            if (game.PositionOnBoard(x, y))
            {
                GameObject piece = game.GetPosition(x, y);

                if (piece == null)
                {
                    MovePlateSpawn(x, y);
                }
            }
        }

        public void MovePlateSpawn(int boardX, int boardY, int xDir = 0, int yDir = 0, bool capture = false)
        {
            float x = (float)_startPosition + boardX;
            float y = (float)_startPosition + boardY;

            GameObject mp = Instantiate(movePlate, new Vector3(x, y, -2.0f), Quaternion.identity);

            MovePlateLogic mpLogic = mp.GetComponent<MovePlateLogic>();
            mpLogic.capturePostition = capture;
            mpLogic.SetReference(gameObject);
            mpLogic.SetCoords(boardX, boardY);
            mpLogic.SetDirection(xDir, yDir);
            mpLogic.SetBoardSize(_boardSize);
        }
    }
}