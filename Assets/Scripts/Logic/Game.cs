using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Logic
{
    public class Game : MonoBehaviour
    {
        // References to the game objects.
        public GameObject gameTile;
        public GameObject gamePiece;
        public GameObject coordinateBox;
        public GameObject mainCamera;
        public TMP_Text winnerText;
        public GameObject restartButton;

        // Define size of the square board, the size needs to be an odd number.
        // Minimum size is 5, maximum size is 25.
        private int _boardSize = 9;

        // Board state
        private GameObject[,] _boardState;
        private GameObject[] _whitePlayer;
        private GameObject[] _blackPlayer;
        private bool _gameOver;
        private int _diagonalLength;
        private double _startPosition;
        private bool _captureChecked;
        private GameObject[] _capturePositions = new GameObject[2];

        private PlayerColour _currentPlayer = PlayerColour.White;
        private PlayerType _blackPlayerType = PlayerType.Manual;
        private PlayerType _whitePlayerType = PlayerType.Manual;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Start()
        {
            _boardSize = int.Parse(GameObject.FindGameObjectWithTag("BoardSizeInput").GetComponent<TMP_InputField>()
                .text);

            _diagonalLength = (_boardSize - 3) / 2;
            _boardState = new GameObject[_boardSize, _boardSize];
            _whitePlayer = new GameObject[_boardSize + _diagonalLength * 2];
            _blackPlayer = new GameObject[_boardSize + _diagonalLength * 2];

            SetCamera();
            CreateBoard();

            SpawnGamePieces();

            // Store the black and white pieces in the board state.
            for (var i = 0; i < _whitePlayer.Length; i++)
            {
                SetPosition(_whitePlayer[i]);
                SetPosition(_blackPlayer[i]);
            }
        }

        public void Update()
        {
            PlayerType currentPlayerType = _currentPlayer == PlayerColour.White ? _whitePlayerType : _blackPlayerType;
            if (!_gameOver && currentPlayerType == PlayerType.AI)
            {
                Move bestMove;
                bestMove = gameObject.GetComponent<AI>().FindBestMove();
                PlayAIMove(bestMove);
            }
        }

        private void PlayAIMove(Move move)
        {
            int winRow;
            // Check if piece moves to a win position.
            switch (move.playerColour)
            {
                case PlayerColour.White:
                    winRow = _boardSize - 1;
                    break;
                case PlayerColour.Black:
                    winRow = 0;
                    break;
                default:
                    Debug.Log("Invalid player colour");
                    Application.Quit();
                    return;
                
            }
            
            if (move.YBoardEnd == winRow)
            {
                Winner(move.playerColour);
            }

            if (move.Capture)
            {
                GameObject oponnentObj = GetPosition(move.XCapture, move.YCapture);
                Destroy(oponnentObj);
            }
            
            GameObject obj = GetPosition(move.XBoardStart, move.YBoardStart);
            SetPositionEmpty(move.XBoardStart, move.YBoardStart);
            obj.GetComponent<GamePieceLogic>().SetXboard(move.XBoardEnd);
            obj.GetComponent<GamePieceLogic>().SetYboard(move.YBoardEnd);
            obj.GetComponent<GamePieceLogic>().SetCoords();
            SetPosition(obj);
            NextTurn();
            
        }

        // Checks if the given position is a legal one.
        public bool PositionOnBoard(int x, int y)
        {
            return x >= 0 && x < _boardSize && y >= 0 && y < _boardSize;
        }

        public bool CapturePosition(int x, int y, int xDir, int yDir, GameObject obj)
        {
            return PositionOnBoard(x + xDir, y + yDir) &&
                   PositionOnBoard(x + xDir * 2, y + yDir * 2) &&
                   GetPosition(x + xDir, y + yDir) != null &&
                   GetPosition(x + xDir * 2, y + yDir * 2) == null &&
                   GetPosition(x + xDir, y + yDir).name != obj.name;
        }

        public void NextTurn()
        {
            if (_currentPlayer == PlayerColour.White)
                _currentPlayer = PlayerColour.Black;
            else
                _currentPlayer = PlayerColour.White;
        }

        public void Winner(PlayerColour winner)
        {
            _gameOver = true;
            winnerText.text = winner + " wins!";
            restartButton.SetActive(true);
            Debug.Log("WINNER: " + winner);
        }

        private void CreateBoard()
        {
            // Letters for coordinate system
            const string letters = "abcdefghijklmnopqrstuvwxy";

            // the start position for the tiles, a tile has a dimension of 1 x 1.
            _startPosition = -(_boardSize / 2.0 - 0.5);

            for (var x = 0; x < _boardSize; x++)
            {
                // Create the coordinate boxes for the X and Y axis.
                var coordinateBoxX = Instantiate(coordinateBox,
                    new Vector3((float)(_startPosition + x), (float)(_startPosition - 1), 0.0f), Quaternion.identity);
                var coordinateBoxY = Instantiate(coordinateBox,
                    new Vector3((float)(_startPosition - 1), (float)(_startPosition + x), 0.0f), Quaternion.identity);
                coordinateBoxX.GetComponent<TextMeshPro>().text = letters[x].ToString();
                coordinateBoxY.GetComponent<TextMeshPro>().text = (x + 1).ToString();

                for (var y = 0; y < _boardSize; y++)
                {
                    // Set the colour of the tile.
                    Color colour;
                    if ((x + y) % 2 == 0)
                        // On even positions, the tile gets a greenish colour.
                        colour = new Color(124 / 255f, 148 / 255f, 91 / 255f, 1.0f);
                    else
                        // On odd positions, the tile gets a yellow white colour.
                        colour = new Color(238 / 255f, 238 / 255f, 212 / 255f, 1.0f);

                    // Create the tile.
                    var tile = Instantiate(gameTile,
                        new Vector3((float)(_startPosition + x), (float)(_startPosition + y), 0), Quaternion.identity);
                    tile.GetComponent<SpriteRenderer>().color = colour;
                }
            }
        }

        private void SetCamera()
        {
            mainCamera.GetComponent<Camera>().orthographicSize = (float)(_boardSize + 2) / 2;
            mainCamera.transform.position = new Vector3(-0.225f * _boardSize - 0.375f, 0f, -10f);
        }

        private void SpawnGamePieces()
        {
            // Create the white pieces.
            // The row pieces.
            for (var i = 0; i < _boardSize; i++) _whitePlayer[i] = Create(PlayerColour.White, i, 0);

            // The left diagonal pieces.
            for (var i = 0; i < _diagonalLength; i++)
                _whitePlayer[_boardSize + i] = Create(PlayerColour.White, 1 + i, 1 + i);

            // The right diagonal pieces.
            for (var i = 0; i < _diagonalLength; i++)
                _whitePlayer[_boardSize + _diagonalLength + i] =
                    Create(PlayerColour.White, _boardSize / 2 + 1 + i, _diagonalLength - i);

            // Create the black pieces.
            // The row pieces.
            for (var i = 0; i < _boardSize; i++) _blackPlayer[i] = Create(PlayerColour.Black, i, _boardSize - 1);

            // The left diagonal pieces.
            for (var i = 0; i < _diagonalLength; i++)
                _blackPlayer[_boardSize + i] = Create(PlayerColour.Black, 1 + i, _boardSize - 2 - i);

            // The right diagonal pieces.
            for (var i = 0; i < _diagonalLength; i++)
                _blackPlayer[_boardSize + _diagonalLength + i] = Create(PlayerColour.Black, _boardSize / 2 + 1 + i,
                    _boardSize - _diagonalLength - 1 + i);
        }

        public GameObject Create(Enum pColour, int x, int y)
        {
            var obj = Instantiate(gamePiece, new Vector3(0, 0, -1), Quaternion.identity);
            var gpLogic = obj.GetComponent<GamePieceLogic>();
            gpLogic.name = pColour.ToString();
            gpLogic.SetXboard(x);
            gpLogic.SetYboard(y);
            gpLogic.SetBoardSize(_boardSize);
            gpLogic.Activate();

            return obj;
        }

        public void SetPosition(GameObject obj)
        {
            var gpLogic = obj.GetComponent<GamePieceLogic>();
            _boardState[gpLogic.GetXboard(), gpLogic.GetYboard()] = obj;
        }

        public void SetPositionEmpty(int x, int y)
        {
            _boardState[x, y] = null;
        }

        public GameObject GetPosition(int x, int y)
        {
            return _boardState[x, y];
        }

        public GameObject[,] GetBoardState()
        {
            return _boardState;
        }

        public GameObject[] GetCapturePositions()
        {
            return _capturePositions;
        }

        public void SetCapturePositions(GameObject[] positions)
        {
            _capturePositions = positions;
        }

        public bool GetCaptureChecked()
        {
            return _captureChecked;
        }

        public void SetCaptureChecked(bool value)
        {
            _captureChecked = value;
        }

        public GameObject[] GetWhitePlayerPieces()
        {
            return _whitePlayer;
        }

        public GameObject[] GetBlackPlayerPieces()
        {
            return _blackPlayer;
        }


        public PlayerColour GetCurrentPlayer()
        {
            return _currentPlayer;
        }

        public bool IsGameOver()
        {
            return _gameOver;
        }

        public void SetBoardSize(int size)
        {
            _boardSize = size;
        }
        
        public int GetBoardSize()
        {
            return _boardSize;
        }

        public void SetPlayer(Enum playerColour)
        {
            _currentPlayer = (PlayerColour)playerColour;
        }

        public void SetPlayerType(Enum playerColour, Enum playerType)
        {
            if (playerColour.ToString() == PlayerColour.White.ToString())
                _whitePlayerType = (PlayerType)playerType;
            else
                _blackPlayerType = (PlayerType)playerType;
        }

        public PlayerType GetPlayerType(Enum playerColour)
        {
            return playerColour.ToString() == PlayerColour.White.ToString() ? _whitePlayerType : _blackPlayerType;
        }
    }
}