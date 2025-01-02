using System;
using Logic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    // References to the game objects.
    public GameObject gameTile;
    public GameObject gamePiece;
    public GameObject coordinateBox;
    public GameObject mainCamera;

    // Define size of the square board, the size needs to be an odd number.
    // Minimum size is 3, maximum size is 25.
    private readonly int _boardsize = 9;
    private double _startPosition;
    private int _diagonalLength;

    private GameObject[,] _boardState;
    private GameObject[] _capturePositions = new GameObject[2];
    private bool _captureChecked = false;
    private GameObject[] _whitePlayer;
    private GameObject[] _blackPlayer;

    private bool gameOver = false;

    private PlayerColour currentPlayer = PlayerColour.White;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        CheckBoardSize();
        
        _diagonalLength = (_boardsize - 3) / 2;
        _boardState = new GameObject[_boardsize, _boardsize];
        _whitePlayer = new GameObject[_boardsize + _diagonalLength * 2];
        _blackPlayer = new GameObject[_boardsize + _diagonalLength * 2];

        SetCameraSize();
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
    
    public PlayerColour GetCurrentPlayer()
    {
        return currentPlayer;
    }
    
    public bool IsGameOver()
    {
        return gameOver;
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
        Debug.Log("WINNER: " + winner);
    }

    private void CheckBoardSize()
    {
        if (_boardsize % 2 == 0)
        {
            Debug.Log("The board size must be an odd number.");
            Application.Quit();
        }
        if (_boardsize < 3)
        {
            Debug.Log("The board size must be at least 3.");
            Application.Quit();
        }
        if (_boardsize > 25)
        {
            Debug.Log("The board size must be at most 25.");
            Application.Quit();
        }
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
    
    private void SetCameraSize()
    {
        mainCamera.GetComponent<Camera>().orthographicSize = (float)(_boardsize + 2) / 2;
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
        
        // _whitePlayer = new GameObject[]
        // {
        //     // The row pieces.
        //     Create("whitePiece", 0, 0), Create("whitePiece", 1, 0), Create("whitePiece", 2, 0),
        //     Create("whitePiece", 3, 0), Create("whitePiece", 4, 0), Create("whitePiece", 5, 0),
        //     Create("whitePiece", 6, 0), Create("whitePiece", 7, 0), Create("whitePiece", 8, 0),
        //     // The diagonal pieces.
        //     Create("whitePiece", 1, 1), Create("whitePiece", 2, 2), Create("whitePiece", 3, 3),
        //     Create("whitePiece", 5, 3), Create("whitePiece", 6, 2), Create("whitePiece", 7, 1)
        // };
        //
        // _blackPlayer = new GameObject[]
        // {
        //     // Create the black pieces.
        //     // The row pieces.
        //     Create("blackPiece", 0, 8), Create("blackPiece", 1, 8), Create("blackPiece", 2, 8),
        //     Create("blackPiece", 3, 8), Create("blackPiece", 4, 8), Create("blackPiece", 5, 8),
        //     Create("blackPiece", 6, 8), Create("blackPiece", 7, 8), Create("blackPiece", 8, 8),
        //     // The diagonal pieces.
        //     Create("blackPiece", 1, 7), Create("blackPiece", 2, 6), Create("blackPiece", 3, 5),
        //     Create("blackPiece", 5, 5), Create("blackPiece", 6, 6), Create("blackPiece", 7, 7)
        // };
    }



}