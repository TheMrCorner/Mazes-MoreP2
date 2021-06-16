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
    TrailType _comingFrom;
    TrailType _nextDir;
    bool _moving = false;

    // helper variables for their use inside coroutines
    Tile _endTile;                      
    bool _addEndTileTrail = false;
    TrailType _endTileTrailDir;

    void Awake()
    {
        _directions = new Stack<TrailType>();
    }


    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------ PRIVATE -------------------

    /// <summary>
    /// Coroutine that moves the character one step along nextDir and then 
    /// until it finds a crossroad
    /// </summary>
    /// <param name="board">(Tile[,]) Matrix that contains the game tiles</param>
    /// <returns></returns>
    IEnumerator MoveCharacter(Tile[,] board)
    {
        UpdateTrails(board, _nextDir);
        yield return StartCoroutine("UpdatePosition", _nextDir);
        if (_addEndTileTrail) 
        {
            _endTile.IncrementTrailCounter(_endTileTrailDir);
            _addEndTileTrail = false;
        }
        _comingFrom = GetOppositeDir(_nextDir);

        bool crossroad = false;
        // keep moving until we find a crossroad in the board
        while (!crossroad)
        {
            if (OnlyOneWay(board)) // for the next ones any is fine as long as it doesnt go backwards
            {
                UpdateTrails(board, _nextDir);
                yield return StartCoroutine("UpdatePosition", _nextDir);
                if (board[_tileX, _tileY].IsGoal())
                    crossroad = true;

                if (_addEndTileTrail)
                {
                    _endTile.IncrementTrailCounter(_endTileTrailDir);
                    _addEndTileTrail = false;
                }
                _comingFrom = GetOppositeDir(_nextDir);
            }
            else
                crossroad = true; // we found a crossroad
        }

        if (board[_tileX, _tileY].IsGoal())
        {
            GameManager.GetInstance().LevelCompleted();
        }
        else
        {
            _moving = false;
        }
            
    }

    /// <summary>
    /// Updates the character's position in board and world coordinates using a lerp 
    /// to animate the character
    /// </summary>
    /// <param name="dir">(TrailType) Direction the player is moving towards</param>
    IEnumerator UpdatePosition(TrailType dir)
    {
        float xIncrement = 0, yIncrement = 0;
        switch (dir)
        {
            case TrailType.NORTH:
                yIncrement = _scalingFactor;
                _tileY++;
                break;
            case TrailType.SOUTH:
                yIncrement = -_scalingFactor;
                _tileY--;
                break;
            case TrailType.EAST:
                xIncrement = _scalingFactor;
                _tileX++;
                break;
            case TrailType.WEST:
                xIncrement = -_scalingFactor;
                _tileX--;
                break;
            default:
                break;
        }

        float timeElapsed = 0, animationDuration = 0.06f;
        Vector3 startPos = transform.position;
        Vector3 currentPos = transform.position;
        Vector3 finalPos = new Vector3(transform.position.x + xIncrement, transform.position.y + yIncrement, 0);

        while (timeElapsed < animationDuration)
        {
            currentPos.x = Mathf.Lerp(startPos.x, finalPos.x, timeElapsed / animationDuration);
            currentPos.y = Mathf.Lerp(startPos.y, finalPos.y, timeElapsed / animationDuration);

            transform.position = currentPos;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPos;
    }


    /// <summary>
    /// Checks the corresponding tiles depending on the direction taken, then checks if
    /// it needs to undo trails or add them instead
    /// </summary>
    /// <param name="board">(Tile[,]) Tile matrix that stores the board's tiles</param>
    /// <param name="dir">(TrailType) Direction of movement</param>
    void UpdateTrails(Tile[,] board, TrailType dir)
    {
        Tile startTile = board[_tileX, _tileY];
        Tile endTile = null;
        switch (dir)
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
        //endTile.IncrementTrailCounter(oppositeDir);
        _addEndTileTrail = true;
        _endTile = endTile;
        _endTileTrailDir = oppositeDir;
    }

    

    bool OnlyOneWay(Tile[,] board)
    {
        bool onlyOneWay = true;
        switch (_comingFrom)
        {
            case TrailType.NORTH:
                if (!IsSouthAWall(board) && IsWestAWall(board) && IsEastAtWall(board))
                    _nextDir = TrailType.SOUTH;
                else if (IsSouthAWall(board) && !IsWestAWall(board) && IsEastAtWall(board))
                    _nextDir = TrailType.WEST;
                else if (IsSouthAWall(board) && IsWestAWall(board) && !IsEastAtWall(board))
                    _nextDir = TrailType.EAST;
                else
                    onlyOneWay = false;
                break;
            case TrailType.SOUTH:
                if (!IsNorthAWall(board) && IsWestAWall(board) && IsEastAtWall(board))
                    _nextDir = TrailType.NORTH;
                else if (IsNorthAWall(board) && !IsWestAWall(board) && IsEastAtWall(board))
                    _nextDir = TrailType.WEST;
                else if (IsNorthAWall(board) && IsWestAWall(board) && !IsEastAtWall(board))
                    _nextDir = TrailType.EAST;
                else
                    onlyOneWay = false;
                break;
            case TrailType.EAST:
                if (!IsNorthAWall(board) && IsWestAWall(board) && IsSouthAWall(board))
                    _nextDir = TrailType.NORTH;
                else if (IsNorthAWall(board) && !IsWestAWall(board) && IsSouthAWall(board))
                    _nextDir = TrailType.WEST;
                else if (IsNorthAWall(board) && IsWestAWall(board) && !IsSouthAWall(board))
                    _nextDir = TrailType.SOUTH;
                else
                    onlyOneWay = false;
                break;
            case TrailType.WEST:
                if (!IsNorthAWall(board) && IsSouthAWall(board) && IsEastAtWall(board))
                    _nextDir = TrailType.NORTH;
                else if (IsNorthAWall(board) && !IsSouthAWall(board) && IsEastAtWall(board))
                    _nextDir = TrailType.SOUTH;
                else if (IsNorthAWall(board) && IsSouthAWall(board) && !IsEastAtWall(board))
                    _nextDir = TrailType.EAST;
                else
                    onlyOneWay = false;
                break;
            default:
                onlyOneWay = false;
                break;
        }

        return onlyOneWay;
    }

    bool IsWestAWall(Tile[,] board)
    {
        return board[_tileX, _tileY].IsLeftWall();
    }

    bool IsEastAtWall(Tile[,] board)
    {
        return board[_tileX + 1, _tileY].IsLeftWall();
    }

    bool IsNorthAWall(Tile[,] board)
    {
        return board[_tileX, _tileY].IsTopWall();
    }

    bool IsSouthAWall(Tile[,] board)
    {
        return board[_tileX, _tileY - 1].IsTopWall();
    }

    /// <summary>
    /// Given a direction, returns its opposite
    /// </summary>
    /// <param name="dir">(TrailType) Direction we want the opposite of</param>
    /// <returns>Opposite direction</returns>
    TrailType GetOppositeDir(TrailType dir)
    {
        TrailType opposite = dir;

        switch (dir)
        {
            case TrailType.NORTH:
                opposite = TrailType.SOUTH;
                break;
            case TrailType.SOUTH:
                opposite = TrailType.NORTH;
                break;
            case TrailType.EAST:
                opposite = TrailType.WEST;
                break;
            case TrailType.WEST:
                opposite = TrailType.EAST;
                break;
            default:
                break;
        }

        return opposite;
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
        if (!_moving)
        {
            _moving = true;
            bool hasMoved = false;
            switch (it)
            {
                case InputManager.InputType.S_UP:
                    _comingFrom = TrailType.SOUTH;
                    _nextDir = TrailType.NORTH;
                    if (!IsNorthAWall(board)) // for the first movement, it has to follow the swipe direction
                        hasMoved = true;
                    break;
                case InputManager.InputType.S_DOWN:
                    _comingFrom = TrailType.NORTH;
                    _nextDir = TrailType.SOUTH;
                    if (!IsSouthAWall(board)) // for the first movement, it has to follow the swipe direction
                        hasMoved = true;
                    break;
                case InputManager.InputType.S_RIGHT:
                    _comingFrom = TrailType.WEST;
                    _nextDir = TrailType.EAST;
                    if (!IsEastAtWall(board)) // for the first movement, it has to follow the swipe direction
                        hasMoved = true;
                    break;
                case InputManager.InputType.S_LEFT:
                    _comingFrom = TrailType.EAST;
                    _nextDir = TrailType.WEST;
                    if (!IsWestAWall(board)) // for the first movement, it has to follow the swipe direction
                        hasMoved = true;
                    break;
                case InputManager.InputType.TAP:
                case InputManager.InputType.NONE:
                default:
                    return;
            }

            if (hasMoved)
            {
                StartCoroutine("MoveCharacter", board);
            }
            else
            {
                _moving = false;
            }
        }

    }
}
