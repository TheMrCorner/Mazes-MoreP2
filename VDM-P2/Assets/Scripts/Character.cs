using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Sprite component of the character")]
    private SpriteRenderer _characterSprite;

    
    [HideInInspector]
    public int _tileX, _tileY;          // used for moving along the board
    public float _scalingFactor = 1;    // used for transforming board steps into world steps
    Stack<TrailType> _directions;       // directions taken by the character


    void Awake()
    {
        _directions = new Stack<TrailType>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------ PRIVATE -------------------
    /// <summary>
    /// Moves the character using the increments provided
    /// </summary>
    /// <param name="xIncrement">(float) amount to move in the X direction</param>
    /// <param name="yIncrement">(float) amount to move in the Y direction</param>
    void UpdatePosition(float xIncrement, float yIncrement)
    {
        transform.position = new Vector3(transform.position.x + xIncrement, transform.position.y + yIncrement , 0);
    }

    /// <summary>
    /// Checks wether an undo of the trail is necessary, and follows the appropriate 
    /// steps in case it is needed
    /// </summary>
    /// <param name="startTile">(Tile) Tile the character is moving from</param>
    /// <param name="endTile">(Tile) Tile the character is moving to</param>
    /// <param name="dir">(TrailType) Direction of the swipe</param>
    /// <param name="oppositeDir">(TrailType) Direction opposite to the swipe</param>
    /// <returns>False if no undo was done, true if some of the trail was undone</returns>
    bool CheckUndo(Tile startTile, Tile endTile, TrailType dir, TrailType oppositeDir)
    {
        bool undo = false;

        if (_directions.Count > 0 && _directions.Peek() == oppositeDir)
        {
            _directions.Pop();
            startTile.DecreaseTrailCounter(dir);
            endTile.DecreaseTrailCounter(oppositeDir);
            undo = true;
        }

        return undo;
    }

    /// <summary>
    /// Increments the trail counters inside the start and end tiles, and also pushes the direction
    /// of the movement to the _directions stack
    /// </summary>
    /// <param name="startTile">(Tile) Tile the character is moving from</param>
    /// <param name="endTile">(Tile) Tile the character is moving to</param>
    /// <param name="currentDir">(TrailType) Direction of the swipe</param>
    /// <param name="oppositeDir">(TrailType) Direction opposite to the swipe</param>
    void IncrementCounters(Tile startTile, Tile endTile, TrailType currentDir, TrailType oppositeDir)
    {
        _directions.Push(currentDir);
        startTile.IncrementTrailCounter(currentDir);
        endTile.IncrementTrailCounter(oppositeDir);
    }

    /// <summary>
    /// Checks the corresponding tiles depending on the direction taken, then checks if
    /// it needs to undo trails or add them instead
    /// </summary>
    /// <param name="board">(Tile[,]) Tile matrix that stores the board's tiles</param>
    /// <param name="tt">(TrailType) Direction of movement</param>
    void CheckDirections(Tile[,] board, TrailType tt)
    {
        Tile startTile = board[_tileX, _tileY];
        Tile endTile = null;
        switch (tt)
        {
            case TrailType.NORTH:
                endTile = board[_tileX, _tileY + 1];
                if (!CheckUndo(startTile, endTile, TrailType.NORTH, TrailType.SOUTH))
                {
                    IncrementCounters(startTile, endTile, TrailType.NORTH, TrailType.SOUTH);
                }
                break;
            case TrailType.SOUTH:
                endTile = board[_tileX, _tileY - 1];
                if (!CheckUndo(startTile, endTile, TrailType.SOUTH, TrailType.NORTH))
                {
                    IncrementCounters(startTile, endTile, TrailType.SOUTH, TrailType.NORTH);
                }
                break;
            case TrailType.EAST:
                endTile = board[_tileX + 1, _tileY];
                if (!CheckUndo(startTile, endTile, TrailType.EAST, TrailType.WEST))
                {
                    IncrementCounters(startTile, endTile, TrailType.EAST, TrailType.WEST);
                }
                break;
            case TrailType.WEST:
                endTile = board[_tileX - 1, _tileY];
                if (!CheckUndo(startTile, endTile, TrailType.WEST, TrailType.EAST))
                {
                    IncrementCounters(startTile, endTile, TrailType.WEST, TrailType.EAST);
                }
                break;
            default:
                break;
        }
    }

    // ------------------- PUBLIC -------------------

    public void ChangeCharacterColor(Color color)
    {
        _characterSprite.color = color;
    }

    /// <summary>
    /// For as long as the character is allowed to move, it will perform the checks in 
    /// CheckDirections, then update the position of the character in the board and in 
    /// world coordinates
    /// </summary>
    /// <param name="board">(Tile[,]) Tile matrix that stores the board's tiles</param>
    /// <param name="tt">(TrailType) Direction of movement</param>
    public void TryToMove(Tile[,] board, InputManager.InputType it)
    {
        bool hasMoved = false;

        switch (it)
        {// TODO: add boundaries check for all of these
            case InputManager.InputType.S_UP:
                while (!board[_tileX, _tileY].IsTopWall() && (!hasMoved || (board[_tileX, _tileY].IsLeftWall() && board[_tileX + 1, _tileY].IsLeftWall())))
                {
                    hasMoved = true;

                    // update the trail 
                    CheckDirections(board, TrailType.NORTH);

                    // update the character's position
                    UpdatePosition(0, _scalingFactor);
                    _tileY++;
                }
                break;
            case InputManager.InputType.S_DOWN:
                while (!board[_tileX, _tileY - 1].IsTopWall() && (!hasMoved || (board[_tileX, _tileY].IsLeftWall() && board[_tileX + 1, _tileY].IsLeftWall())))
                {
                    hasMoved = true;

                    // update the trail 
                    CheckDirections(board, TrailType.SOUTH);

                    // update the character's position
                    UpdatePosition(0, -_scalingFactor);
                    _tileY--;
                }
                break;
            case InputManager.InputType.S_RIGHT:
                while (!board[_tileX + 1, _tileY].IsLeftWall() && (!hasMoved || (board[_tileX, _tileY].IsTopWall() && board[_tileX, _tileY - 1].IsTopWall())))
                {
                    hasMoved = true;

                    // update the trail 
                    CheckDirections(board, TrailType.EAST);

                    // update the character's position
                    UpdatePosition(_scalingFactor, 0);
                    _tileX++;
                }
                break;
            case InputManager.InputType.S_LEFT:
                while (!board[_tileX, _tileY].IsLeftWall() && (!hasMoved || (board[_tileX, _tileY].IsTopWall() && board[_tileX, _tileY - 1].IsTopWall())))
                {
                    hasMoved = true;
                    
                    // update the trail 
                    CheckDirections(board, TrailType.WEST);

                    // update the character's position
                    UpdatePosition(-_scalingFactor, 0);
                    _tileX--;
                }
                break;
            case InputManager.InputType.TAP:
            default:
                break;
        }
    }
}
