using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{

    [Header("Configuration")]
    public int _sideMargin;
    public int _topMargin;
    public GameObject _board;
    public Tile _tilePrefab;
    public Character _characterPrefab;
    public Transform position;
    public int _tilesHint; // Number of tiles for hint

    // Calculate space remaining for the board
    private float _topPanel;
    private float _bottomPanel;
    // TODO: Pause menu etc.

   
    private Tile[,] _tiles;
    private Character _character;
    private Vector2[] _hintArray;
    private int _currentHint = 0;
    private Point _start;

    private LevelManager _levelManager;

    // Space that the board will take
    private Vector2 _resolution;

    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------ PRIVATE -------------------

    private void SetTile(TileInfo info, Tile tile)
    {
        WallType infoWalls; infoWalls.left = info.wallLeft; infoWalls.top = info.wallTop;
        if (info.goal) tile.EnableGoal();
        if (info.iceFloor) tile.EnableIce();
        if (info.wallLeft || info.wallTop) tile.EnableWalls(infoWalls);
    } // SetTile


    private TrailType GetDir(Vector2 a, Vector2 b)
    {
        Vector2 res = b - a;

        if (res.x == 1)
        {
            return TrailType.EAST;
        } // if
        else if (res.x == -1)
        {
            return TrailType.WEST;
        } // else if
        else if (res.y == 1)
        {
            return TrailType.NORTH;
        } // else if
        else if (res.y == -1)
        {
            return TrailType.SOUTH;
        } // else if

        return TrailType.START;
    } // GetDir

    /// <summary>
    /// Given a direction, returns its opposite
    /// </summary>
    /// <param name="dir">(TrailType) Direction we want the opposite of</param>
    /// <returns>Opposite direction</returns>
    private TrailType GetOppositeDir(TrailType dir)
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

    private IEnumerator ShowHint()
    {
        int i;
        for (i = 0; i < _tilesHint && (i + _currentHint) < (_hintArray.Length - 1); i++)
        {
            int access = i + _currentHint;
            TrailType h = GetDir(_hintArray[access], _hintArray[access + 1]);
            _tiles[(int)_hintArray[access].x, (int)_hintArray[access].y].EnableHint(h);
            _tiles[(int)_hintArray[access + 1].x, (int)_hintArray[access + 1].y].EnableHint(GetOppositeDir(h));
            yield return null;
        } // for

        _currentHint += i;
    } // ShowHint

    /// <summary>
    /// 
    /// Calculates the space available for the board, using the top and bottom panel 
    /// of the scene.
    /// 
    /// </summary>
    private void CalculateSpace()
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

        // Change resolution to Unity units to use it for positions
        _resolution /= GameManager.GetInstance().GetScaling().TransformationFactor();
    } // CalculateSpace



    /// <summary>
    /// 
    /// Calculates the size that one tile will have with the space available
    /// calculated previously. 
    /// 
    /// </summary>
    /// <param name="sprite"> (SpriteRenderer) Sprite to use as reference. </param>
    /// <returns> (Vector2) Size in pixels that will take te Tile. </returns>
    private Vector2 CalculateSize(SpriteRenderer sprite)
    {
        // Transform resolution to pixels 
        Vector2 resolutionTemp = _resolution * GameManager.GetInstance().GetScaling().TransformationFactor();

        // Calculate how many space requires a Tile breadthways and upwards related to that resolution
        float tileSizeX = resolutionTemp.x / _tiles.GetLength(0);
        float tileSizeY = resolutionTemp.y / _tiles.GetLength(1);

        // Choose the lower one to scale it fitting the shortest one
        if (tileSizeX < tileSizeY)
        {
            tileSizeY = sprite.bounds.size.y * tileSizeX / sprite.bounds.size.x;
        } // if
        else
        {
            tileSizeX = sprite.bounds.size.x * tileSizeY / sprite.bounds.size.y;
        } // else

        // Return result as a Vector
        return new Vector2(tileSizeX, tileSizeY);
    } // CalculateSize

    /// <summary>
    /// Resets the trail counters for every tile in the board, also removing the visuals
    /// </summary>
    void RemoveTrails()
    {
        foreach (Tile tile in _tiles)
        {
            while (tile.IsNorthTrail())
                tile.DecreaseTrailCounter(TrailType.NORTH);
            while (tile.IsSouthTrail())
                tile.DecreaseTrailCounter(TrailType.SOUTH);
            while (tile.IsEastTrail())
                tile.DecreaseTrailCounter(TrailType.EAST);
            while (tile.IsWestTrail())
                tile.DecreaseTrailCounter(TrailType.WEST);
        }
    }

    // ------------------- PUBLIC -------------------

    public void Init(LevelManager levelManager)
    {
        _levelManager = levelManager;
    } // Init

    public void SetMap(Map map)
    {
        // Init board sizes and variables
        _tiles = new Tile[map.X, map.Y];
        _hintArray = map.hintArray; _tilesHint = Mathf.CeilToInt(_hintArray.Length / 3.0f);
        _start = map.start;

        // Calculate space available for board
        CalculateSpace();

        // Set trail color and goal color
        _tilePrefab.GetComponent<Tile>().SetColor(GameManager.GetInstance().GetLevelPackage().playerColor);

        // Get size in pixels of tile (using ice sprite as reference because it fills one tile completely) and resize
        SpriteRenderer iceFloor = _tilePrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Vector2 tam = CalculateSize(iceFloor);

        // Instantiate tiles
        for (int y = 0; y < map.Y; ++y)
        {
            for (int x = 0; x < map.X; ++x)
            {
                _tiles[x, y] = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity, _board.transform);
                SetTile(map.tileInfoMatrix[x, y], _tiles[x, y]);
            }
        }

        // Instantiate character and set it's color
        _character = Instantiate(_characterPrefab, new Vector3(_start.x, _start.y, 0), Quaternion.identity, _board.transform);
        _character.GetComponent<Character>().SetPositions(_start.x, _start.y);
        _character.GetComponent<Character>().ChangeCharacterColor(GameManager.GetInstance().GetLevelPackage().playerColor);
        _character.GetComponent<Character>().ShowDirections(_tiles);

        // Scale tiles and Board
        Vector2 oScale = _tilePrefab.transform.localScale;
        Vector2 nScale = GameManager.GetInstance().GetScaling().ResizeObjectScale(iceFloor.bounds.size * GameManager.GetInstance().GetScaling().TransformationFactor(), tam, oScale);
        gameObject.transform.localScale = nScale;
        _board.transform.localScale = nScale;

        // Scale Player movement
        float factor = nScale.x / oScale.x;
        _character._scalingFactor = factor;

        // Relocate board
        gameObject.transform.Translate(new Vector3((-(map.X - 1) / 2.0f) * factor, ((-(map.Y - 1) / 2.0f) * factor) - 2));
        _board.transform.Translate(new Vector3((-(map.X - 1) / 2.0f) * factor, ((-(map.Y - 1) / 2.0f) * factor) - 1));

        // Save character position for restart
        _character.SaveStartingPoint();
    } // SetMap

    public void ReceiveInput(InputManager.InputType it)
    {
        if (it == InputManager.InputType.TAP)
        {
            //Debug.Log("Tap");
        } // if
        else
        {
            _character.TryToMove(_tiles, it);
        } // else
    } // ReceiveInput

    public int TryShowHint()
    {
        if(_currentHint >= _hintArray.Length)
        {
            // All hints shown
            return -2;
        }
        else if(GameManager.GetInstance().GetPlayerData()._hints > 0)
        {
            GameManager.GetInstance().HintShown();
            StartCoroutine("ShowHint");
            return 0;
        } // if

        return -1;
    } // TryShowHint

    public void ResetBoard()
    {
        _character.ResetCharacterPos();
        RemoveTrails();
    }

    public void EmptyBoard()
    {
        DestroyImmediate(_board);
        _board = new GameObject();

        gameObject.transform.position = new Vector3(0, 0, 0);
        _board.transform.position = new Vector3(0, 0, 0);
    } // EmptyBoard
} // BoardManager