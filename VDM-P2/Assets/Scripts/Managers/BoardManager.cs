using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{

    [Header("Configuration")]
    public int _sideMargin;
    public int _topMargin;
    public Tile tilePrefab;
    public Character characterPrefab;
    public Transform position;

    // Calculate space remaining for the board
    private float _topPanel;
    private float _bottomPanel;
    // TODO: Pause menu etc.

    Tile[,] _tiles;
    Character _character;
    Vector2[] _hintArray;
    Point _start;

    LevelManager _levelManager;

    // Space that the board will take
    Vector2 _resolution;

    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------ PRIVATE -------------------

    void SetTile(TileInfo info, Tile tile)
    {
        WallType infoWalls; infoWalls.left = info.wallLeft; infoWalls.top = info.wallTop;
        if (info.goal) tile.EnableGoal();
        if (info.iceFloor) tile.EnableIce();
        if (info.wallLeft || info.wallTop) tile.EnableWalls(infoWalls);
    } // SetTile


    // ------------------- PUBLIC -------------------

    public void Init(LevelManager levelManager)
    {
        _levelManager = levelManager;
    } // Init

    /// <summary>
    /// Function that calculates the space available for the board and the game.  Uses the height 
    /// of the different panels to calculate this space in relation with the height of the screen.
    /// 
    /// </summary>
    void CalculateSpace()
    {
        // Calculates the space of the top and bottom panels in pixels
        _topPanel = GameManager.GetInstance().GetTopPanelHeight() * GameManager.GetInstance().GetCanvas().scaleFactor;
        _bottomPanel = GameManager.GetInstance().GetBottomPanelHeight() * GameManager.GetInstance().GetCanvas().scaleFactor;

        Vector2 actRes = GameManager.GetInstance().GetScaling().CurrentResolution();

        // Calculates the available space in the current resolution 
        float dispY = (actRes.y - (_bottomPanel + _topPanel)) - (2 * GameManager.GetInstance().GetScaling().ResizeY(_topMargin));
        float dipsX = actRes.x - (2 * GameManager.GetInstance().GetScaling().ResizeX(_sideMargin));

        // Creates the available screen space for the game in pixels
        _resolution = new Vector2(dipsX, dispY);

        // Change resolution to Unity units
        _resolution /= GameManager.GetInstance().GetScaling().TransformationFactor();
    } // CalculateSpace

    public void SetMap(Map map)
    {
        _tiles = new Tile[map.X, map.Y];
        _hintArray = map.hintArray;
        _start = map.start;

        CalculateSpace();

        SpriteRenderer iceFloor = tilePrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();

        Vector2 resolutionTemp = _resolution * GameManager.GetInstance().GetScaling().TransformationFactor();

        float tileSizeX = resolutionTemp.x / _tiles.GetLength(0); // Lo que ocupa un tileen esa resolución 
        float tileSizeY = iceFloor.bounds.size.y * tileSizeX / iceFloor.bounds.size.x;

        for (int y = 0; y < map.Y; ++y)
        {
            for (int x = 0; x < map.X; ++x)
            {
                _tiles[x, y] = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                SetTile(map.tileInfoMatrix[x, y], _tiles[x, y]);
            }
        }

        _character = Instantiate(characterPrefab, new Vector3(_start.x, _start.y, 0), Quaternion.identity, transform);
        _character._tileX = _start.x; _character._tileY = _start.y;

        Vector2 tam = new Vector2(tileSizeX, tileSizeY);
        Vector2 oScale = tilePrefab.transform.localScale;
        Vector2 nScale = GameManager.GetInstance().GetScaling().ResizeObjectScale(iceFloor.bounds.size * GameManager.GetInstance().GetScaling().TransformationFactor(), tam, oScale);
        gameObject.transform.localScale = nScale;

        float factor = nScale.x / oScale.x;

        _character._scalingFactor = factor;

        gameObject.transform.Translate(new Vector3((-(map.X - 1) / 2.0f) * factor, (-(map.Y - 1) / 2.0f) * factor));
    } // SetMap

    public void ReceiveInput(InputManager.InputType it)
    {
        if (it == InputManager.InputType.TAP)
        {
            Debug.Log("Tap");
        } // if
        else
        {
            _character.TryToMove(_tiles, it);
        } // else
    } // ReceiveInput
} // BoardManager