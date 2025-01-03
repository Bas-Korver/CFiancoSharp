using UnityEngine;

namespace Logic
{
    public class Move
    {
        public int XBoardStart {get; set;}
        public int YBoardStart {get; set;}
        public int XBoardEnd {get; set;}
        public int YBoardEnd {get; set;}
        public bool Capture {get; set;}
        public int XCapture {get; set;}
        public int YCapture {get; set;}
        public PlayerColour playerColour {get; set;}
        
        // Constructor for initializing the move.
        public Move(int xBoardStart, int yBoardStart, int xBoardEnd, int yBoardEnd, PlayerColour playerColour, bool capture = false, int xCapture = -1, int yCapture = -1)
        {
            XBoardStart = xBoardStart;
            YBoardStart = yBoardStart;
            XBoardEnd = xBoardEnd;
            YBoardEnd = yBoardEnd;
            Capture = capture;
            XCapture = xCapture;
            YCapture = yCapture;
            this.playerColour = playerColour;
        }
    }
}

