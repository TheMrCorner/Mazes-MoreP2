﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Class that controls the character and it's movement. 
/// 
/// </summary>
public class Character : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Sprite component of the character")]
    private SpriteRenderer _characterSprite;   // Character sprite

    [Header("Arrow")]
    public SpriteRenderer _north;              // North arrow 
    public SpriteRenderer _south;              // South arrow
    public SpriteRenderer _west;               // west arrow
    public SpriteRenderer _east;               // east arrow
    
    [HideInInspector]
    public int _tileX, _tileY;                 // used for moving along the board
    public float _scalingFactor = 1;           // used for transforming board steps into world steps
    
    Stack<TrailType> _directions;              // directions taken by the character
    TrailType _comingFrom;                     // Last Type
    TrailType _nextDir;                        // Next direction
    bool _moving = false;                      // Flag to check if moving

    // helper variables for their use inside coroutines
    Tile _endTile;                             
    bool _addEndTileTrail = false;
    TrailType _endTileTrailDir;

    // for easy re-starting of the character
    int _tileStartX, _tileStartY;
    Vector3 _startPos;

    void Awake()
    {
        _directions = new Stack<TrailType>();
    } // Awake 


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
        // the first step has to follow the direction of the swipe
        UpdateTrails(board, _nextDir);
        yield return StartCoroutine("UpdatePosition", _nextDir);
        if (_addEndTileTrail) 
        {
            _endTile.IncrementTrailCounter(_endTileTrailDir);
            _addEndTileTrail = false;
        } // if
        _comingFrom = GetOppositeDir(_nextDir);


        // for the next ones any direction is fine as long as it doesnt go backwards
        bool canMove = true;
        while (canMove) // keep moving until we find a crossroad in the board
        {
            if (board[_tileX, _tileY].IsIce()) 
            {
                if (!CanMoveStraight(board))
                {
                    canMove = false;
                } // if
            } // if
            else if (!OnlyOneWay(board)) 
            {
                canMove = false;
                
            } // else if

            if (canMove)
            {
                UpdateTrails(board, _nextDir);
                yield return StartCoroutine("UpdatePosition", _nextDir);
                if (board[_tileX, _tileY].IsGoal())
                    canMove = false;

                if (_addEndTileTrail)
                {
                    _endTile.IncrementTrailCounter(_endTileTrailDir);
                    _addEndTileTrail = false;
                } // if
                _comingFrom = GetOppositeDir(_nextDir);
            } // if
        } // while

        if (board[_tileX, _tileY].IsGoal())
        {
            GameManager.GetInstance().LevelCompleted();
        } // if
        else
        {
            ShowDirections(board);

            _moving = false;
        } // else
    } // MoveCharacter

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
        } // switch

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
        } // while

        transform.position = finalPos;
    } // UpdatePosition


    /// <summary>
    /// 
    /// Checks the corresponding tiles depending on the direction taken, then checks if
    /// it needs to undo trails or add them instead
    /// 
    /// </summary>
    /// <param name="board"> (Tile[,]) Tile matrix that stores the board's tiles. </param>
    /// <param name="dir"> (TrailType) Direction of movement. </param>
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
                } // if 
                break;
            case TrailType.SOUTH:
                endTile = board[_tileX, _tileY - 1];
                if (!CheckUndo(startTile, endTile, TrailType.SOUTH, TrailType.NORTH))
                {
                    IncrementCounters(startTile, endTile, TrailType.SOUTH, TrailType.NORTH);
                } // if
                break;
            case TrailType.EAST:
                endTile = board[_tileX + 1, _tileY];
                if (!CheckUndo(startTile, endTile, TrailType.EAST, TrailType.WEST))
                {
                    IncrementCounters(startTile, endTile, TrailType.EAST, TrailType.WEST);
                } // if
                break;
            case TrailType.WEST:
                endTile = board[_tileX - 1, _tileY];
                if (!CheckUndo(startTile, endTile, TrailType.WEST, TrailType.EAST))
                {
                    IncrementCounters(startTile, endTile, TrailType.WEST, TrailType.EAST);
                } // if
                break;
            default:
                break;
        } // switcj
    } // UpdateTrails
    
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
        } // if

        return undo;
    } // CheckUndo

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
    } // IncrementCounters 

    /// <summary>
    /// 
    /// Check if the player can move straight forward.
    /// 
    /// </summary>
    /// <param name="board"> (Tile[]) Board to check. </param>
    /// <returns> (bool) If it can move or not. </returns>
    bool CanMoveStraight(Tile[,] board)
    {
        bool canMoveStraight = false;

        switch (_nextDir)
        {
            case TrailType.NORTH:
                if (!IsNorthAWall(board))
                    canMoveStraight = true;
                break;
            case TrailType.SOUTH:
                if (!IsSouthAWall(board))
                    canMoveStraight = true;
                break;
            case TrailType.EAST:
                if (!IsEastAtWall(board))
                    canMoveStraight = true;
                break;
            case TrailType.WEST:
                if (!IsWestAWall(board))
                    canMoveStraight = true;
                break;
            case TrailType.START:
                break;
            default:
                return canMoveStraight;
        } // switch

        _comingFrom = GetOppositeDir(_nextDir);
        return canMoveStraight;
    } // CanMoveStraight

    /// <summary>
    /// 
    /// Check if there is only one way available.
    /// 
    /// </summary>
    /// <param name="board"> (Tile[]) Board to check. </param>
    /// <returns> (bool) If there is only way. </returns>
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
        } // switch

        return onlyOneWay;
    } // OnlyOneWay

    /// <summary>
    /// 
    /// Check if the west has wall.
    /// 
    /// </summary>
    /// <param name="board"> (Tile[]) Board to check. </param>
    /// <returns> (bool) Is wall or not. </returns>
    bool IsWestAWall(Tile[,] board)
    {
        return board[_tileX, _tileY].IsLeftWall();
    } // IsWestAWall

    /// <summary>
    /// 
    /// Check if east has a wall
    /// 
    /// </summary>
    /// <param name="board"> (Tile[]) Board to check. </param>
    /// <returns> (bool) Is wall or not. </returns>
    bool IsEastAtWall(Tile[,] board)
    {
        return board[_tileX + 1, _tileY].IsLeftWall();
    } // IsEastAtWall

    /// <summary>
    /// 
    /// Check if north has a wall. 
    /// 
    /// </summary>
    /// <param name="board"> (Tile[]) Board to check. </param>
    /// <returns> (bool) Is wall or not. </returns>
    bool IsNorthAWall(Tile[,] board)
    {
        return board[_tileX, _tileY].IsTopWall();
    } // IsNorthAWall

    /// <summary>
    /// 
    /// Check if south is a wall or not. 
    /// 
    /// </summary>
    /// <param name="board"> (Tile[]) Board to check. </param>
    /// <returns> (bool) Is wall or not. </returns>
    bool IsSouthAWall(Tile[,] board)
    {
        return board[_tileX, _tileY - 1].IsTopWall();
    } // IsSouthAWall

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
    } // GetOppositeDir

    /// <summary>
    /// 
    /// Deactivate all arrows.
    /// 
    /// </summary>
    private void UnShowDirections()
    {
        _east.gameObject.SetActive(false);
        _west.gameObject.SetActive(false);
        _north.gameObject.SetActive(false);
        _south.gameObject.SetActive(false);
    } // UnShowDirections

    // ------------------- PUBLIC -------------------
    /// <summary>
    /// 
    /// Save the starting point. 
    /// 
    /// </summary>
    public void SaveStartingPoint()
    {
        _tileStartX = _tileX;
        _tileStartY = _tileY;
        _startPos = transform.position;
    } // SaveStartingPoint

    /// <summary>
    /// 
    /// Reset the character position to it's starting point.
    /// 
    /// </summary>
    public void ResetCharacterPos()
    {
        _tileX = _tileStartX;
        _tileY = _tileStartY;
        transform.position = _startPos;
    } // ResetCharacterPost

    /// <summary>
    /// 
    /// Method to activate or deactivate the arrows 
    /// checking the possible paths.
    /// 
    /// </summary>
    /// <param name="board"> (Tile[]) Board to check everything. </param>
    public void ShowDirections(Tile[,] board)
    {
        if (!IsEastAtWall(board) && _comingFrom != TrailType.EAST)
            _east.gameObject.SetActive(true);
        else
            _east.gameObject.SetActive(false);

        if (!IsWestAWall(board) && _comingFrom != TrailType.WEST)
            _west.gameObject.SetActive(true);
        else
            _west.gameObject.SetActive(false);

        if (!IsNorthAWall(board) && _comingFrom != TrailType.NORTH)
            _north.gameObject.SetActive(true);
        else
            _north.gameObject.SetActive(false);

        if (!IsSouthAWall(board) && _comingFrom != TrailType.SOUTH)
            _south.gameObject.SetActive(true);
        else
            _south.gameObject.SetActive(false);
    } // ShowDirections

    /// <summary>
    /// 
    /// Method to set the position of the character in the board.
    /// 
    /// </summary>
    /// <param name="X"> (int) Pos X. </param>
    /// <param name="Y"> (int) Pos Y. </param>
    public void SetPositions(int X, int Y)
    {
        _tileX = X;
        _tileY = Y;
        _comingFrom = TrailType.START;
    } // SetPositions

    /// <summary>
    /// 
    /// Method to set the color of the player and the arrows.
    /// 
    /// </summary>
    /// <param name="color"> (Color) Color to set everything. </param>
    public void ChangeCharacterColor(Color color)
    {
        _characterSprite.color = color;
        _north.color = color;
        _east.color = color;
        _west.color = color;
        _south.color = color;
    } // ChangeCharacterColor

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
                    _nextDir = TrailType.NORTH;
                    hasMoved = CanMoveStraight(board);// for the first movement, it has to follow the swipe direction
                    break;
                case InputManager.InputType.S_DOWN:
                    _nextDir = TrailType.SOUTH;
                    hasMoved = CanMoveStraight(board);// for the first movement, it has to follow the swipe direction
                    break;
                case InputManager.InputType.S_RIGHT:
                    _nextDir = TrailType.EAST;
                    hasMoved = CanMoveStraight(board);// for the first movement, it has to follow the swipe direction
                    break;
                case InputManager.InputType.S_LEFT:
                    _nextDir = TrailType.WEST;
                    hasMoved = CanMoveStraight(board);// for the first movement, it has to follow the swipe direction
                    break;
                case InputManager.InputType.TAP:
                case InputManager.InputType.NONE:
                default:
                    return;
            } // switch

            if (hasMoved)
            {
                UnShowDirections();
                StartCoroutine("MoveCharacter", board);
            } // if
            else
            {
                _moving = false;
            } // else
        } // if
    } // TryToMove
} // Character
