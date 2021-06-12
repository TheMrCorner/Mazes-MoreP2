using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Sprite component of the character")]
    private SpriteRenderer characterSprite;

    // used for moving along the board
    [HideInInspector]
    public int tileX, tileY;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------ PRIVATE -------------------
    void UpdatePosition(int xIncrement, int yIncrement)
    {
        transform.position = new Vector3(transform.position.x + xIncrement, transform.position.y + yIncrement , 0);
    }

    // ------------------- PUBLIC -------------------

    public void ChangeCharacterColor(Color color)
    {
        characterSprite.color = color;
    }

    public void TryToMove(Tile[,] board, InputManager.InputType it)
    {
        switch (it)
        {// TODO: add boundaries check for all of these
            case InputManager.InputType.S_UP:
                while (!board[tileX, tileY].IsTopWall()) 
                {
                    tileY++;
                    UpdatePosition(0, 1);
                }
                break;
            case InputManager.InputType.S_DOWN:
                while (!board[tileX, tileY - 1].IsTopWall()) 
                {
                    tileY--;
                    UpdatePosition(0, -1);
                }
                break;
            case InputManager.InputType.S_RIGHT:
                while (!board[tileX + 1, tileY].IsLeftWall())
                {
                    tileX++;
                    UpdatePosition(1, 0);
                }
                break;
            case InputManager.InputType.S_LEFT:
                while (!board[tileX, tileY].IsLeftWall())
                {
                    tileX--;
                    UpdatePosition(-1, 0);
                }
                break;
            case InputManager.InputType.TAP:
            default:
                break;
        }
    }
}
