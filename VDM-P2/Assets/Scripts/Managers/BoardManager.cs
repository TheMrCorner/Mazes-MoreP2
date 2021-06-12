using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    public Tile tilePrefab;
    public Character characterPrefab;

    Tile[,] _tiles;
    Character _character;
    Vector2[] _hintArray;
    Point _start;

    LevelManager _levelManager;

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
    }


    // ------------------- PUBLIC -------------------

    public void Init(LevelManager levelManager)
    {
        _levelManager = levelManager;
    }

    public void SetMap(Map map)
    {
        _tiles = new Tile[map.X, map.Y];
        _hintArray = map.hintArray;
        _start = map.start;

        for (int y = 0; y < map.Y; ++y)
        {
            for (int x = 0; x < map.X; ++x)
            {
                _tiles[x, y] = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                SetTile(map.tileInfoMatrix[x, y], _tiles[x, y]);
            }
        }

        _character = Instantiate(characterPrefab, new Vector3(_start.x, _start.y, 0), Quaternion.identity, transform);

        gameObject.transform.Translate(new Vector3(-(map.X - 1) / 2.0f, -(map.Y - 1)/2.0f));
        //TODO: scale, but using which method? ResizeObjectScale?
    }

    public void ReceiveInput(InputManager.InputType it)
    {
        if (it == InputManager.InputType.TAP)
        {
            Debug.Log("Tap");
        }
        else
        {
            // TODO: move character
        }
    }

}


///// <summary>
///// This class is the one that will control the board of the game and 
///// manage all the changes in the tiles. Calculates the space between 
///// the panels to locate the game. Calculates how many tiles are needed
///// for the current level. Sets the space for the current configuration
///// and scales everything keeping aspect ratio. 
///// </summary>
//public class BoardManager : MonoBehaviour
//{
//    /// <summary> Space that the board will take </summary>
//    Vector2 _resolution;

//    // Margins
//    /// <summary> Margin between the top and the bottom panel and the game space </summary>
//    public int _topMargin;
//    /// <summary> Margin between the side points and the game space </summary>
//    public int _sideMargin;

//    /// <summary> Empty game object that will contains the board and that will set its position </summary>
//    public Transform _boardPanel;
//    /// <summary> Matrix with the tiles of the board </summary>
//    GameObject[,] _board;

//    /// <summary> Stack with the path of the player (the tiles that the player pressed in order) </summary>
//    Stack<Vector2> _path;

//    // Tile's prefab
//    public Tile tilePrefab;
//    ///// <summary> Gray tile's prefab (not pressed) and father of the tile colors and paths </summary>
//    //GameObject _tile;
//    ///// <summary> Dotted road showing the actual path of the player matching one tile with the previus one </summary>
//    //GameObject _playerPath;
//    ///// <summary> Colored tile to mark a tile like pressed </summary>
//    //GameObject _colorTile;
//    ///// <summary> Dotted colored road showing the right path given by the hints </summary>
//    //GameObject _pathColor;

//    /// <summary> Starting level's tile </summary>
//    GameObject _origin;

//    // Panels to calculate the space for the board
//    /// <summary> Top canvas panel </summary>
//    private float _topPanel;
//    /// <summary> Bottom canvas panel </summary>
//    private float _bottomPanel;

//    /// <summary> Class with all the level info </summary>
//    //Levels _level;


//    /// <summary> How many tiles will be in the current level (WidthxHeight) </summary>
//    Vector2 _dimensions;

//    // Hint management
//    /// <summary> Last position of the hints array </summary>
//    int _lastHint = 1;


//    /*
//    /// <summary>
//    /// Converts a pixel meassure to Unity Units.
//    /// </summary>
//    /// <param name="pixel">Pixels</param>
//    /// <returns>Unity Units</returns>
//    float PixelToUnityPosition(float pixel)
//    {
//        return pixel /= GameManager.GetInstance().GetScaling().UnityUds();
//    }
//    */

//    private void Awake()
//    {
//        // Initiates the tiles stack before is needed
//        _path = new Stack<Vector2>();
//    }


//    /*
//    /// <summary>
//    /// Initiates all the values and call the functions to make an specific board
//    /// </summary>
//    /// <param name="level">Determined level that will load in the board</param>
//    /// <param name="c">Number of the random color to load a tile</param>
//    public void Init(Levels level, int c)
//    {
//        _level = level;

//        _dimensions = new Vector2(_level.layout[0].Length, _level.layout.Length);

//        // Creates the board with an specific size for n tiles in width and m tiles in height
//        // Then it will be adjust with the available space
//        _board = new GameObject[(int)_dimensions.y, (int)_dimensions.x];

//        _topMargin = (int)(7.5 * _dimensions.y);
//        _sideMargin = (int)(11.7 * _dimensions.x);

//        InitGameObjects(c);

//        CalculateSpace();

//        CalculatePosition();
//    }
//    */

//    #region Calculate and create Board
//    /*
//    /// <summary>
//    /// Set each prefab of the tile and its children loading them from AssetBundle
//    /// </summary>
//    /// <param name="color">Number of the random tile color to load it</param>
//    void InitGameObjects(int color)
//    {
//        _tile = GameManager.GetInstance().getPrefabFromTileAssetBundle("block_00");
//        _playerPath = GameManager.GetInstance().getPrefabFromPathAssetBundle("block_00_hint");
//        _colorTile = GameManager.GetInstance().getPrefabFromTileAssetBundle("block_0" + color);
//        _pathColor = GameManager.GetInstance().getPrefabFromPathAssetBundle("block_0" + color + "_hint");
//    }
//    */


//    /*
//    /// <summary>
//    /// Function that calculates the space available for the board and the game.  Uses the height 
//    /// of the different panels to calculate this space in relation with the height of the screen.
//    /// 
//    /// </summary>
//    void CalculateSpace()
//    {
//        // Calculates the space of the top and bottom panels in pixels
//        _topPanel = GameManager.GetInstance().panelSuperiorHeight() * GameManager.GetInstance().GetCanvas().scaleFactor;
//        _bottomPanel = GameManager.GetInstance().panelInferiorHeight() * GameManager.GetInstance().GetCanvas().scaleFactor;

//        Vector2 actRes = GameManager.GetInstance().getResolution();

//        // Calculates the available space in the current resolution 
//        float dispY = (actRes.y - (_bottomPanel + _topPanel)) - (2 * GameManager.GetInstance().GetScaling().ResizeY(_topMargin));
//        float dipsX = actRes.x - (2 * GameManager.GetInstance().GetScaling().ResizeX(_sideMargin));

//        // Creates the available screen space for the game in pixels
//        _resolution = new Vector2(dipsX, dispY);

//        // Change reslution to Unity units
//        _resolution /= GameManager.GetInstance().GetScaling().UnityUds();

//        DefineTileSize();
//    }
//    */


//    /*
//    /// <summary>
//    /// Determinates the tiles size considering the width and height dimensions of the level
//    /// and the scaled resolution of the game space
//    /// </summary>
//    void DefineTileSize()
//    {
//        Vector2 resolutionTemp = _resolution * GameManager.GetInstance().GetScaling().UnityUds();

//        float finalSize;
//        float tileSizeX;
//        float tileSizeY;

//        // If the level has more than 5 tiles height
//        if (_dimensions.y > 5)
//        {
//            // Fits 6x8 tiles in the  calculated resolution, 
//            // so each tile will have 1/6 x 1/8 size of the resolution
//            tileSizeX = resolutionTemp.x / 6;
//            tileSizeY = resolutionTemp.y / 8;
//        }
//        else
//        {
//            // Fits 6x5 tiles in the  calculated resolution, 
//            // so each tile will have 1/6 x 1/5 size of the resolution
//            tileSizeX = resolutionTemp.x / 6;
//            tileSizeY = resolutionTemp.y / 5;

//        }

//        // Chooses the higher size between the new tile height and width
//        if (tileSizeY < tileSizeX)
//        {
//            finalSize = tileSizeY;
//        }
//        else
//        {
//            finalSize = tileSizeX;
//        }

//        InstantiateTiles();

//        // Board scaling before all the tiles are instantiate
//        Vector2 nTam = new Vector2(finalSize, finalSize);
//        Vector2 nScale = GameManager.GetInstance().GetScaling().resizeObjectScaleKeepingAspectRatio(_tile.GetComponent<SpriteRenderer>().bounds.size * GameManager.GetInstance().GetScaling().UnityUds(),
//            nTam, _tile.transform.localScale);
//        _boardPanel.transform.localScale = new Vector3(nScale.x, nScale.y, 1);
//    }
//    */


//    /*
//    /// <summary>
//    /// Generates all the tiles with their children and sets them in their respective position.
//    /// </summary>
//    void InstantiateTiles()
//    {
//        // Runs all the tiles in the board
//        for (int i = 0; i < _dimensions.y; i++)
//        {
//            for (int j = 0; j < _dimensions.x; j++)
//            {
//                // If the level contains the character 0 means that is not pressed tile
//                if (_level.layout[i][j] != '0')
//                {
//                    Vector3 position = new Vector3();

//                    position.z = -1;

//                    // Reposition of each tile considering the iterator to equitative separation between them
//                    // Compares if the dimensions are pair to add 0.5 for a better centered of the board
//                    if (_dimensions.x % 2 == 0)
//                    {
//                        position.x = (_boardPanel.position.x - (_dimensions.x / 2) + 0.5f) + j;
//                    }
//                    else
//                    {
//                        position.x = ((_boardPanel.position.x - (int)(_dimensions.x / 2))) + j;
//                    }

//                    if (_dimensions.y % 2 == 0)
//                    {
//                        position.y = (_boardPanel.position.y + (_dimensions.y / 2) - 0.5f) - i;
//                    }
//                    else
//                    {
//                        position.y = (_boardPanel.position.y + (int)(_dimensions.y / 2)) - i;
//                    }

//                    ConfigTile(position, j, i);

//                    // If the level contains the character 2 means that is the first tile of the path
//                    // so the color will be activated
//                    if (_level.layout[i][j] == '2')
//                    {
//                        _path.Push(new Vector2(j, i));

//                        _board[i, j].GetComponent<Tile>().ActivateColor();

//                        _origin = _board[i, j];
//                    }
//                }
//            }
//        }
//    }
//    */


//    /*
//    /// <summary>
//    /// Instantiates a tile in a specific position and sets its children
//    /// </summary>
//    /// <param name="pos">Real position of the tile in the game space</param>
//    /// <param name="posX">Horizontas position of the tile in the board</param>
//    /// <param name="posY">Vertical position of the tile in the board</param>
//    void ConfigTile(Vector3 pos, int posX, int posY)
//    {
//        // Instantiate GameObjects needed
//        GameObject nTile = Instantiate(_tile, pos, Quaternion.identity);
//        GameObject clTile = Instantiate(_colorTile, pos, Quaternion.identity);
//        GameObject pathPivot = new GameObject("PathPivot");
//        pathPivot.transform.SetPositionAndRotation(pos, Quaternion.identity);
//        GameObject plPath = Instantiate(_playerPath, pos, Quaternion.identity);
//        GameObject hintPivot = new GameObject("HintPivot");
//        hintPivot.transform.SetPositionAndRotation(pos, Quaternion.identity);
//        GameObject hnPath = Instantiate(_pathColor, pos, Quaternion.identity);

//        // Attacht them to parents
//        nTile.transform.SetParent(_boardPanel);
//        clTile.transform.SetParent(nTile.transform);
//        hintPivot.transform.SetParent(nTile.transform);
//        pathPivot.transform.SetParent(nTile.transform);

//        // Configure paths to rotate correctly
//        plPath.transform.SetParent(pathPivot.transform);
//        plPath.transform.SetPositionAndRotation(pathPivot.transform.position + new Vector3(0.5f, 0, 0), pathPivot.transform.rotation);

//        hnPath.transform.SetParent(hintPivot.transform);
//        hnPath.transform.SetPositionAndRotation(hintPivot.transform.position + new Vector3(0.5f, 0, 0), hintPivot.transform.rotation);

//        // Configure Tile info for later use
//        nTile.transform.GetComponent<Tile>().SetTile(clTile, pathPivot, hintPivot, new Vector2(posX, posY));

//        // Save the tile in the array
//        _board[posY, posX] = nTile;
//    }
//    */


//    /*
//    /// <summary>
//    /// Calculates the position of the panel board (which contains the board with all the tiles) in Unity units
//    /// </summary>
//    void CalculatePosition()
//    {
//        Vector3 position = new Vector3();

//        float dispDistance = GameManager.GetInstance().getResolution().y - (_bottomPanel + _topPanel);

//        dispDistance /= 2; // Calculates the distance to the middle point between the panels

//        position.y = (GameManager.GetInstance().getResolution().y - _topPanel) - dispDistance;

//        // Calculates the position in Unity units
//        // If the position is higher than the middle then it is in positive Unity units
//        if (position.y > (GameManager.GetInstance().getResolution().y / 2))
//        {
//            position.y -= (GameManager.GetInstance().getResolution().y / 2);

//            position.y = PixelToUnityPosition(position.y);
//        }
//        // If not, it is in negative units
//        else if (position.y < (GameManager.GetInstance().getResolution().y / 2))
//        {
//            position.y = (GameManager.GetInstance().getResolution().y / 2) - position.y;

//            position.y = (PixelToUnityPosition(position.y) * (-1));
//        }
//        // Finally, the 0, 0, 0 position
//        else
//        {
//            position.y = 0;
//        }

//        _boardPanel.SetPositionAndRotation(position, _boardPanel.rotation);
//    }
//    */
//    #endregion

//    #region GamePlay
//    /*
//    /// <summary>
//    /// Checks out if the input position (mouse click or touched screen) is on a tile
//    /// and change the tile state and information 
//    /// </summary>
//    /// <param name="position">World position of the input</param>
//    public void Touched(Vector2 position)
//    {
//        float degrees;
//        RaycastHit2D ray = Physics2D.Raycast(position, Vector2.zero);
//        if (ray)
//        {
//            // If the ray collides with a tile
//            if (ray.collider.gameObject.GetComponent<Tile>())
//            {
//                Tile tile = ray.collider.gameObject.GetComponent<Tile>();

//                // Cheks out if the pressed tile is on the path stack already
//                if (_path.Contains(tile.getPositionInBoard()))
//                {
//                    // Reset all the following tiles in the path to not pressed and pop it from the stack
//                    while (_path.Peek() != tile.getPositionInBoard())
//                    {
//                        _board[(int)_path.Peek().y, (int)_path.Peek().x].GetComponent<Tile>().ResetTile();
//                        _path.Pop();
//                    }
//                }
//                else
//                {
//                    // If the tile is not in the path stack, checks out if there are some pressed tile around
//                    // and if one of these is the last one in the path
//                    // If the last one is the left one
//                    if (CheckTile(new Vector2(tile.getPositionInBoard().x - 1, tile.getPositionInBoard().y)))
//                    {
//                        // Activates tile and rotates its path to the left
//                        degrees = 180.0f;
//                        TileAdded(tile, degrees);
//                    }
//                    // If the last one is the right one
//                    else if (CheckTile(new Vector2(tile.getPositionInBoard().x + 1, tile.getPositionInBoard().y)))
//                    {
//                        // Activates tile and rotates its path to the right (default)
//                        degrees = 0.0f;
//                        TileAdded(tile, degrees);
//                    }
//                    // If the last one is the under one
//                    else if (CheckTile(new Vector2(tile.getPositionInBoard().x, tile.getPositionInBoard().y - 1)))
//                    {
//                        // Activates tile and rotates its path to down
//                        degrees = 90.0f;
//                        TileAdded(tile, degrees);
//                    }
//                    // If the last one is the uppper one
//                    else if (CheckTile(new Vector2(tile.getPositionInBoard().x, tile.getPositionInBoard().y + 1)))
//                    {
//                        // Activates tile and rotates its path to up
//                        degrees = -90.0f;
//                        TileAdded(tile, degrees);
//                    }
//                }
//            }
//        }
//    }
//    */


//    /*
//    /// <summary>
//    /// Push the new tile position in the stack and activates the tile's children
//    /// with the random color and its path
//    /// </summary>
//    /// <param name="t">Tile class with all the information of each tile</param>
//    /// <param name="degrees">Degrees to rotate the player path when is active</param>
//    public void TileAdded(Tile t, float degrees)
//    {
//        _path.Push(t.getPositionInBoard());

//        t.ActivateColor();
//        t.RotatePlayerPath(degrees);
//    }
//    */

//    /// <summary>
//    /// Checks if a tile's position is on the path stack
//    /// </summary>
//    /// <param name="position">Tile's position in the board</param>
//    /// <returns>If the tile's position is in the path stack or not</returns>
//    public bool CheckTile(Vector2 position)
//    {
//        // Checks if the tile's position is in the board and is not out of limits
//        if (position.x >= _dimensions.x || position.x < 0 || position.y >= _dimensions.y || position.y < 0)
//        {
//            return false;
//        }
//        // If there is a tile in the board's position
//        else if (_board[(int)position.y, (int)position.x] != null)
//        {
//            // Checks if this position is alredy in the path stack
//            if (_path.Peek() == position)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//        else
//        {
//            return false;
//        }
//    }


//    /*
//    /// <summary>
//    /// The player ask for a hint to know the right path of the level.
//    /// Checks the last hint given and activates the following right paths
//    /// </summary>
//    public void HintGiven()
//    {
//        float degrees = 0.0f;
//        int i;

//        // Activates 3 new hints path if them fit in the board
//        for (i = _lastHint; i < (_lastHint + 4) && i < _level.path.Length; i++)
//        {
//            // If the previus hint is on the left, rotate the next hint to the left
//            if ((int)_level.path[i].x == (int)_level.path[i - 1].x - 1)
//            {
//                degrees = -90.0f;
//            }
//            // If the previus hint is on the right, rotate the next hint to the right
//            else if ((int)_level.path[i].x == (int)_level.path[i - 1].x + 1)
//            {
//                degrees = 90.0f;
//            }
//            // If the previus hint is under, rotate the next hint to down
//            else if ((int)_level.path[i].y == (int)_level.path[i - 1].y - 1)
//            {
//                degrees = 0.0f;
//            }
//            // If the previus hint is upper, rotate the next hint to up
//            else if ((int)_level.path[i].y == (int)_level.path[i - 1].y + 1)
//            {
//                degrees = 180.0f;
//            }

//            _board[(int)_level.path[i].x, (int)_level.path[i].y].GetComponent<Tile>().RotateHintPath(degrees);
//        }

//        // Update the last hint
//        _lastHint = i;
//    }
//    */


//    /*
//    /// <summary>
//    /// Reset all the tiles in the level to the original state and pop them from the path stack
//    /// </summary>
//    public void ResetLevel()
//    {
//        while (_path.Peek() != _origin.GetComponent<Tile>().getPositionInBoard())
//        {
//            _board[(int)_path.Peek().y, (int)_path.Peek().x].GetComponent<Tile>().ResetTile();
//            _path.Pop();
//        }
//    }
//    */


//    /*
//    /// <summary>
//    /// Checks if the player path contains all the tiles and has the same lenth of the hints path.
//    /// That is because it is only a way to press all the tiles in a path
//    /// </summary>
//    /// <returns>If the level is ended or not (all the tiles are pressed)</returns>
//    public bool Ended()
//    {
//        if (_path.ToArray().Length == _level.path.Length)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }
//    */


//    /*
//    /// <summary>
//    /// Checks if fits new path hints
//    /// </summary>
//    /// <returns>If new hints fits in the board</returns>
//    public bool CanBuyHint()
//    {
//        return _lastHint != (int)_level.path.Length;
//    }
//    */
//    #endregion
//}