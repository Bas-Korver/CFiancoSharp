using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logic
{
    public class Game : MonoBehaviour
    {
        // References to the game objects.
        public GameObject gameTile;
        public GameObject gamePiece;
        public GameObject coordinateBox;
        public GameObject mainCamera;
        public TMP_Text WinnerText;
        public GameObject RestartButton;

        // Define size of the square board, the size needs to be an odd number.
        // Minimum size is 3, maximum size is 25.
        private int _boardsize = 9;
        private double _startPosition;
        private int _diagonalLength;

        // Board state
        private GameObject[,] _boardState;
        private GameObject[] _capturePositions = new GameObject[2];
        private bool _captureChecked = false;
        private GameObject[] _whitePlayer;
        private GameObject[] _blackPlayer;
        
        private PlayerType _whitePlayerType = PlayerType.Manual;
        private PlayerType _blackPlayerType = PlayerType.Manual;

        private bool gameOver = false;

        private PlayerColour currentPlayer = PlayerColour.White;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Start()
        {
            _boardsize = int.Parse(GameObject.FindGameObjectWithTag("BoardSizeInput").GetComponent<TMP_InputField>().text);
        
            _diagonalLength = (_boardsize - 3) / 2;
            _boardState = new GameObject[_boardsize, _boardsize];
            _whitePlayer = new GameObject[_boardsize + _diagonalLength * 2];
            _blackPlayer = new GameObject[_boardsize + _diagonalLength * 2];

            SetCamera();
            CreateBoard();

            SpawnGamePieces();

            // Store the black and white pieces in the board state.
            for (int i = 0; i < _whitePlayer.Length; i++)
            {
                SetPosition(_whitePlayer[i]);
                SetPosition(_blackPlayer[i]);
            }
        }
    
        // Update is called once per frame
        // TODO: Do we need this method?
        private void Update()
        {
            if (gameOver && Input.GetMouseButtonDown(0))
            {
                gameOver = false;
                SceneManager.LoadScene("Game");
            }
        }
    
        public bool PositionOnBoard(int x, int y)
        {
            return x >= 0 && x < _boardsize && y >= 0 && y < _boardsize;
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
            if (currentPlayer == PlayerColour.White)
            {
                currentPlayer = PlayerColour.Black;
            }
            else
            {
                currentPlayer = PlayerColour.White;
            }
        }

        public void Winner(PlayerColour winner)
        {
            gameOver = true;
            WinnerText.text = winner + " wins!";
            RestartButton.SetActive(true);
            Debug.Log("WINNER: " + winner);
            
        }
    
        private void CreateBoard()
        {
            // Letters for coordinate system
            const string letters = "abcdefghijklmnopqrstuvwxy";

            // the start position for the tiles, a tile has a dimension of 1 x 1.
            _startPosition = -(_boardsize / 2.0 - 0.5);

            for (var x = 0; x < _boardsize; x++)
            {
                // Create the coordinate boxes for the X and Y axis.
                var coordinateBoxX = Instantiate(coordinateBox,
                    new Vector3((float)(_startPosition + x), (float)(_startPosition - 1), 0.0f), Quaternion.identity);
                var coordinateBoxY = Instantiate(coordinateBox,
                    new Vector3((float)(_startPosition - 1), (float)(_startPosition + x), 0.0f), Quaternion.identity);
                coordinateBoxX.GetComponent<TextMeshPro>().text = letters[x].ToString();
                coordinateBoxY.GetComponent<TextMeshPro>().text = (x + 1).ToString();

                for (var y = 0; y < _boardsize; y++)
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
            mainCamera.GetComponent<Camera>().orthographicSize = (float)(_boardsize + 2) / 2;
            mainCamera.transform.position = new Vector3(-0.225f * _boardsize - 0.375f, 0f, -10f);
        }
    
        private void SpawnGamePieces()
        {
            // Create the white pieces.
            // The row pieces.
            for (int i = 0; i < _boardsize; i++)
            {
                _whitePlayer[i] = Create(PlayerColour.White, i, 0);
            }
            // The left diagonal pieces.
            for (int i = 0; i < _diagonalLength; i++)
            {
                _whitePlayer[_boardsize + i] = Create(PlayerColour.White, 1 + i, 1 + i);
            }
            // The right diagonal pieces.
            for (int i = 0; i < _diagonalLength; i++)
            {
                _whitePlayer[_boardsize + _diagonalLength + i] = Create(PlayerColour.White, _boardsize / 2 + 1 + i, _diagonalLength - i);
            }
        
            // Create the black pieces.
            // The row pieces.
            for (int i = 0; i < _boardsize; i++)
            {
                _blackPlayer[i] = Create(PlayerColour.Black, i, _boardsize - 1);
            }
            // The left diagonal pieces.
            for (int i = 0; i < _diagonalLength; i++)
            {
                _blackPlayer[_boardsize + i] = Create(PlayerColour.Black, 1 + i, _boardsize - 2 - i);
            }
            // The right diagonal pieces.
            for (int i = 0; i < _diagonalLength; i++)
            {
                _blackPlayer[_boardsize + _diagonalLength + i] = Create(PlayerColour.Black, _boardsize / 2 + 1 + i, _boardsize - _diagonalLength - 1 + i);
            }
        }
    
        public GameObject Create(Enum pColour, int x, int y)
        {
            GameObject obj = Instantiate(gamePiece, new Vector3(0, 0, -1), Quaternion.identity);
            GamePieceLogic gpLogic = obj.GetComponent<GamePieceLogic>();
            gpLogic.name = pColour.ToString();
            gpLogic.SetXboard(x);
            gpLogic.SetYboard(y);
            gpLogic.SetBoardSize(_boardsize);
            gpLogic.Activate();

            return obj;
        }
    
        public void SetPosition(GameObject obj)
        {
            GamePieceLogic gpLogic = obj.GetComponent<GamePieceLogic>();
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
            return currentPlayer;
        }
    
        public bool IsGameOver()
        {
            return gameOver;
        }
    
        public void SetBoardSize(int size)
        {
            _boardsize = size;
        }
    
        public void SetPlayer(Enum playerColour)
        {
            currentPlayer = (PlayerColour) playerColour;
        }
        
        public void SetPlayerType(Enum playerColour, Enum playerType)
        {
            if (playerColour.ToString() == PlayerColour.White.ToString())
            {
                _whitePlayerType = (PlayerType) playerType;
            }
            else
            {
                _blackPlayerType = (PlayerType) playerType;
            }
        }
        
        public PlayerType GetPlayerType(Enum playerColour)
        {
            return playerColour.ToString() == PlayerColour.White.ToString() ? _whitePlayerType : _blackPlayerType;
        }
    }
}