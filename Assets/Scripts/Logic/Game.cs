using System;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject gameTile;
    public GameObject gamePiece;
    public GameObject coordinateBox;
    
    // Define size of the square board, the size needs to be an odd number.
    // Minimum size is 3, maximum size is 9.
    private int _boardsize = 9;
    private double _startPositionX;
    private double _startPositionY;

    private GameObject[,] _boardState = new GameObject[9, 9];
    private GameObject[] blackPlayer = new GameObject[9 + 9 - 3];
    private GameObject[] whitePlayer = new GameObject[9 + 9 - 3];
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateBoard()
    {
        // Letters for coordinate system
        const string letters = "abcdefghijklmnopqrstuvwxyz";
        
        // the start position for the tiles, a tile has a dimension of 1 x 1.
        _startPositionX = -(_boardsize / 2.0 - 0.5);
        _startPositionY = -(_boardsize / 2.0 - 0.5);

        for (int x = 0; x < _boardsize; x++)
        {
            
            GameObject coordinateBoxX = Instantiate(coordinateBox, new Vector3((float)(_startPositionX + x), (float)(_startPositionY - 1), 0), Quaternion.identity);
            GameObject coordinateBoxY = Instantiate(coordinateBox, new Vector3((float)(_startPositionX - 1), (float)(_startPositionY + x), 0), Quaternion.identity);
            coordinateBoxX.GetComponent<TextMeshPro>().text = letters[x].ToString();
            coordinateBoxY.GetComponent<TextMeshPro>().text = (x + 1).ToString();
            
            for (int y = 0; y < _boardsize; y++)
            {
                Color colour;
                if ((x + y) % 2 == 0)
                {
                    colour = new Color(124/255f, 148/255f, 91/255f, 1.0f);
                }
                else
                {
                    colour = new Color(238/255f, 238/255f, 212/255f, 1.0f);
                }
                
                GameObject tile = Instantiate(gameTile, new Vector3((float)(_startPositionX + x), (float)(_startPositionY + y), 0), Quaternion.identity);
                tile.GetComponent<SpriteRenderer>().color = colour;
            }
        }
    }
}
