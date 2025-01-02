using Logic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovePlateLogic : MonoBehaviour
{
    public GameObject controller;

    private GameObject _ownerReference = null;

    private int _matrixX;
    private int _matrixY;
    private int _xDir;
    private int _yDir;
    private int _boardSize;

    
    public bool capturePostition = false;

    public void Start()
    {
        if (capturePostition)
        {
            // Change the color of the move plate to red.
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUpAsButton()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (capturePostition)
        {
            GameObject gamePiece = controller.GetComponent<Game>().GetPosition(_matrixX, _matrixY);
            
            Destroy(gamePiece);
        }
        
        // Check if piece moves to a win position.
        if (_ownerReference.GetComponent<GamePieceLogic>().name == PlayerColour.White.ToString() && _matrixY == _boardSize - 1)
        {
            controller.GetComponent<Game>().Winner(PlayerColour.White);
        }
        else if (_ownerReference.GetComponent<GamePieceLogic>().name == "Black" && _matrixY == 0)
        {
            controller.GetComponent<Game>().Winner(PlayerColour.Black);
        }
        
        controller.GetComponent<Game>().SetPositionEmpty(_ownerReference.GetComponent<GamePieceLogic>().GetXboard(), 
            _ownerReference.GetComponent<GamePieceLogic>().GetYboard());
        
        _ownerReference.GetComponent<GamePieceLogic>().SetXboard(_matrixX + _xDir);
        _ownerReference.GetComponent<GamePieceLogic>().SetYboard(_matrixY + _yDir);
        _ownerReference.GetComponent<GamePieceLogic>().SetCoords();
        
        controller.GetComponent<Game>().SetPosition(_ownerReference);

        
        controller.GetComponent<Game>().NextTurn();
        controller.GetComponent<Game>().SetCaptureChecked(false);
        GameObject[] positions = new GameObject[2];
        controller.GetComponent<Game>().SetCapturePositions(positions);
        
        _ownerReference.GetComponent<GamePieceLogic>().DestroyMovePlates();
    }

    public void SetCoords(int x, int y)
    {
        _matrixX = x;
        _matrixY = y;
    }
    
    public void SetDirection(int x, int y)
    {
        _xDir = x;
        _yDir = y;
    }

    public void SetReference(GameObject obj)
    {
        _ownerReference = obj;
    }
    
    public GameObject GetReference()
    {
        return _ownerReference;
    }
    
    public void SetBoardSize(int size)
    {
        _boardSize = size;
    }
}
