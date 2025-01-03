using Bot;
using Logic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameObject controller;

    public Move FindBestMove()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        Debug.Log("AI is thinking...");

        var depth = 3;
        var alpha = -int.MaxValue;
        var beta = int.MaxValue;

        var boardStateConverted = controller.GetComponent<AIMovement>()
            .ConvertBoardState(controller.GetComponent<Game>().GetBoardState());

        var score = -int.MaxValue;
        var moves = controller.GetComponent<AIMovement>().GenerateMoves(boardStateConverted);
        Move bestMove = null;

        foreach (var move in moves)
        {
            var value = -NegaMax(
                controller.GetComponent<AIMovement>().ExecuteMove(move, boardStateConverted),
                depth,
                alpha,
                beta, controller.GetComponent<Game>().GetCurrentPlayer());
            {
                score = value;
                bestMove = move;
            }
            if (score > alpha) alpha = score;
        }

        Debug.Log("Selected move with a value of: " + score);
        return bestMove;
    }

    private int NegaMax(string[,] boardStateConverted, int depth, int alpha, int beta, PlayerColour playerColour)
    {
        if (Terminal(boardStateConverted) || depth == 0) return Evaluate(boardStateConverted, playerColour);

        var score = -int.MaxValue;
        var moves = controller.GetComponent<AIMovement>().GenerateMoves(boardStateConverted);
        playerColour = playerColour == PlayerColour.White ? PlayerColour.Black : PlayerColour.White;

        foreach (var move in moves)
        {
            var value = -NegaMax(controller.GetComponent<AIMovement>().ExecuteMove(move, boardStateConverted),
                depth - 1, -beta, -alpha, playerColour);
            if (value > score) score = value;
            if (score > alpha) alpha = score;
            if (score >= beta) break; // Cut-off
        }

        return score;
    }

    private bool Terminal(string[,] boardStateConverted)
    {
        for (var x = 0; x < boardStateConverted.GetLength(0); x++)
            if (boardStateConverted[x, boardStateConverted.GetLength(1) - 1] == PlayerColour.White.ToString())
                return true;

        for (var x = 0; x < boardStateConverted.GetLength(0); x++)
            if (boardStateConverted[x, 0] == PlayerColour.Black.ToString())
                return true;

        return false;
    }

    // Evaluate the board state with a heuristic function.
    private int Evaluate(string[,] boardStateConverted, PlayerColour playerColour)
    {
        var score = 0;
        // Define the counters needed for the heuristic function.
        var piecesWhite = 0;
        var piecesBlack = 0;
        var movesToWinWhite = 0;
        var movesToWinBlack = 0;
        var openingsToWinWhite = 0;
        var openingsToWinBlack = 0;
        var winPositionWhite = 0;
        var winPositionBlack = 0;

        // Define weights
        var weightPieces = 5;
        var weightMovesToWin = 20;
        var weightOpeningsToWin = 10 ;
        var weightWinPosition = int.MaxValue;
        var weightsFirstRow = 2;
        

        // Count the number of pieces for each player.
        for (var x = 0; x < boardStateConverted.GetLength(0); x++)
        {
            for (var y = 0; y < boardStateConverted.GetLength(1); y++)
            {
                if (boardStateConverted[x, y] == PlayerColour.White.ToString())
                    piecesWhite++;
                else if (boardStateConverted[x, y] == PlayerColour.Black.ToString()) piecesBlack++;
            }
        }

        // Calculate the number of moves needed to win for each player.
        for (var x = 0; x < boardStateConverted.GetLength(0); x++)
        {
            for (var y = 0; y < boardStateConverted.GetLength(1); y++)
            {
                if (boardStateConverted[x, y] == PlayerColour.White.ToString())
                {
                    movesToWinWhite += (y + 1) * weightsFirstRow;
                }
                else if (boardStateConverted[x, y] == PlayerColour.Black.ToString())
                {
                    movesToWinBlack += (boardStateConverted.GetLength(1) - y) * weightsFirstRow;
                }
            }
        }

        // Calculate the number of openings available to win position for each player.
        for (var x = 0; x < boardStateConverted.GetLength(0); x++)
        {
            if (boardStateConverted[x, boardStateConverted.GetLength(1) - 1] == null) openingsToWinWhite++;

            if (boardStateConverted[x, boardStateConverted.GetLength(1) - 1] == PlayerColour.White.ToString())
                winPositionWhite++;
            
            if (boardStateConverted[x, 0] == null) openingsToWinBlack++;

            if (boardStateConverted[x, 0] == PlayerColour.Black.ToString()) winPositionBlack++;
        }

        // Calculate the score using the heuristic function.
        score += weightPieces * (piecesWhite - piecesBlack);
        score += weightMovesToWin * (movesToWinWhite - movesToWinBlack);
        score += weightOpeningsToWin * (openingsToWinWhite - openingsToWinBlack);
        score += weightWinPosition * winPositionWhite;
        score += -weightWinPosition * winPositionBlack;
        
        if (playerColour == PlayerColour.White) return score;
        return -score;
    }
}