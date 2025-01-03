using System.Collections.Generic;
using Logic;
using UnityEngine;

namespace Bot
{
    public class AIMovement : MonoBehaviour
    {
        public GameObject controller;
        private int _yDir;
        private PlayerColour _playerColour;
    
        public string[,] ConvertBoardState(GameObject[,] boardState)
        {
            controller = GameObject.FindGameObjectWithTag("GameController");
            int boardsize = controller.GetComponent<Game>().GetBoardSize();
            string[,] boardStateConverted = new string[boardsize, boardsize];
        
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    if (boardState[i, j] == null)
                    {
                        boardStateConverted[i, j] = null;
                    }
                    else
                    {
                        boardStateConverted[i, j] = boardState[i, j].name;
                    }
                }
            }

            return boardStateConverted;
        }
    
        //Generate all legal moves for a specific player.
        public List<Move> GenerateMoves(string[,] boardStateConverted)
        {
            List<Move> moves = new List<Move>();
            bool capture = false;
            switch (controller.GetComponent<Game>().GetCurrentPlayer())
            {
                case PlayerColour.White:
                    _yDir = 1;
                    _playerColour = PlayerColour.White;
                    break;
                case PlayerColour.Black:
                    _yDir = -1;
                    _playerColour = PlayerColour.Black;
                    break;
            }
        
            // First check if there are any capture moves available
            for (int x = 0; x < boardStateConverted.GetLength(0); x++)
            {
                for (int y = 0; y < boardStateConverted.GetLength(1); y++)
                {
                    if (boardStateConverted[x, y] == _playerColour.ToString())
                    {
                        if (CapturePosition(x, y, -1, _yDir, boardStateConverted, _playerColour))
                        {
                            // Add new capture move to moves list
                            moves.Add(new Move(x, y, x - 2, y + _yDir * 2, _playerColour, true, x - 1, y + _yDir));
                            capture = true;
                        }
                        if (CapturePosition(x, y, +1, _yDir, boardStateConverted, _playerColour))
                        {
                            moves.Add(new Move(x, y, x + 2, y + _yDir * 2, _playerColour, true, x + 1, y + _yDir));
                            capture = true;
                        }
                    }
                }
            }
        
            // If there are no capture moves available, add all possible moves to the moves list
            if (!capture)
            {
                for (int x = 0; x < boardStateConverted.GetLength(0); x++)
                {
                    for (int y = 0; y < boardStateConverted.GetLength(1); y++)
                    {
                        if (boardStateConverted[x, y] == _playerColour.ToString())
                        {
                            if (controller.GetComponent<Game>().PositionOnBoard(x, y + _yDir) && boardStateConverted[x, y + _yDir] == null)
                            {
                                moves.Add(new Move(x, y, x, y + _yDir, _playerColour));
                            }
                            if (controller.GetComponent<Game>().PositionOnBoard(x - 1, y + _yDir) && boardStateConverted[x - 1, y] == null)
                            {
                                moves.Add(new Move(x, y, x - 1, y, _playerColour));
                            }
                            if (controller.GetComponent<Game>().PositionOnBoard(x + 1, y + _yDir) && boardStateConverted[x + 1, y] == null)
                            {
                                moves.Add(new Move(x, y, x + 1, y, _playerColour));
                            }
                        }
                    }
                }
            }
        
            return moves;
        }
    
        private bool CapturePosition(int x, int y, int xDir, int yDir, string[,] boardStateConverted, PlayerColour playerColour)
        {
            return controller.GetComponent<Game>().PositionOnBoard(x + xDir, y + yDir) &&
                   controller.GetComponent<Game>().PositionOnBoard(x + xDir * 2, y + yDir * 2) &&
                   GetPosition(x + xDir, y + yDir, boardStateConverted) != null &&
                   GetPosition(x + xDir * 2, y + yDir * 2, boardStateConverted) == null &&
                   GetPosition(x + xDir, y + yDir, boardStateConverted) != playerColour.ToString();
        }
    
        private string GetPosition(int x, int y, string[,] boardStateConverted)
        {
            return boardStateConverted[x, y];
        }
        
        // Apply the given move to the board state.
        public string[,] ExecuteMove(Move move, string[,] boardStateConverted)
        {
            // Make a copy of the board state.
            string[,] newBoardStateConverted = new string[boardStateConverted.GetLength(0), boardStateConverted.GetLength(1)];
            
            for (int i = 0; i < boardStateConverted.GetLength(0); i++)
            {
                for (int j = 0; j < boardStateConverted.GetLength(1); j++)
                {
                    newBoardStateConverted[i, j] = boardStateConverted[i, j];
                }
            }
            
            newBoardStateConverted[move.XBoardEnd, move.YBoardEnd] = newBoardStateConverted[move.XBoardStart, move.YBoardStart];
            newBoardStateConverted[move.XBoardStart, move.YBoardStart] = null;
            if (move.Capture)
            {
                newBoardStateConverted[move.XCapture, move.YCapture] = null;
            }
            
            return newBoardStateConverted;
        }
    }
}
