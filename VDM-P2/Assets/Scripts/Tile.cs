﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WallType
{
    public bool top, left;
}

public enum TrailType { NORTH, SOUTH, EAST, WEST};

public class Tile : MonoBehaviour
{
    #region variables
    [SerializeField] [Tooltip("Child component that stores the ice floor sprite")]
    private GameObject _iceFloor;
    [SerializeField] [Tooltip("Child component that stores the top wall sprite")]
    private GameObject _wallTop;
    [SerializeField] [Tooltip("Child component that stores the left wall sprite")]
    private GameObject _wallLeft;
    [SerializeField] [Tooltip("Child component that stores the goal sprite")]
    private GameObject _goal;
    [SerializeField] [Tooltip("Child component that stores the north trail")]
    private GameObject _trailNorth;
    [SerializeField] [Tooltip("Child component that stores the east trail")]
    private GameObject _trailEast;
    [SerializeField] [Tooltip("Child component that stores the south trail")]
    private GameObject _trailSouth;
    [SerializeField] [Tooltip("Child component that stores the west trail")]
    private GameObject _trailWest;

    /// <summary>
    /// Stores the number of times the player has gone over the tile.
    /// Access with (int)TrailType
    /// </summary>
    int[] _trailCounter = { 0, 0, 0, 0 };
    #endregion //variables

    #region methods
    private void Start()
    {
#if UNITY_EDITOR
        if (_iceFloor == null)
        {
            Debug.LogError("iceFloor is null. Can't start without the ice floor sprite");
            gameObject.SetActive(false);
            return;
        }
        if (_wallTop == null)
        {
            Debug.LogError("wallTop is null. Can't start without the wall top sprite");
            gameObject.SetActive(false);
            return;
        }
        if (_wallLeft == null)
        {
            Debug.LogError("wallLeft is null. Can't start without the wall left sprite");
            gameObject.SetActive(false);
            return;
        }
        if (_goal == null)
        {
            Debug.LogError("goal is null. Can't start without the goal sprite");
            gameObject.SetActive(false);
            return;
        }
        if (_trailEast == null || _trailNorth == null || _trailSouth == null || _trailWest == null)
        {
            Debug.LogError("some of the trails are null. Can't start without the trail sprites");
            gameObject.SetActive(false);
            return;
        }
#endif
    }

    // -----------------------------------------------
    // ----- methods that turn components on/off -----
    // -----------------------------------------------

    /// <summary> Enables the ice sprite </summary>
    public void EnableIce()
    {
        _iceFloor.SetActive(true);
    }
    /// <summary> Disables the ice sprite </summary>
    public void DisableIce()
    {
        _iceFloor.SetActive(false);
    }

    /// <summary> Enables the given wall sprites </summary>
    public void EnableWalls(WallType walls)
    {
        if (walls.top) _wallTop.SetActive(true);
        if (walls.left) _wallLeft.SetActive(true);

    }
    /// <summary> Disables the given wall sprites </summary>
    public void DisableWalls(WallType walls)
    {
        if (walls.top) _wallTop.SetActive(false);
        if (walls.left) _wallLeft.SetActive(false);
    }

    /// <summary> Enables the goal sprite </summary>
    public void EnableGoal()
    {
        _goal.SetActive(true);
    }
    /// <summary> Disables the goal sprite </summary>
    public void DisableGoal()
    {
        _goal.SetActive(false);
    }

    /// <summary> Enables the specified trail sprite </summary>
    public void EnableTrail(TrailType tt)
    {
        switch (tt)
        {
            case TrailType.NORTH:
                _trailNorth.SetActive(true);
                break;
            case TrailType.SOUTH:
                _trailSouth.SetActive(true);
                break;
            case TrailType.EAST:
                _trailEast.SetActive(true);
                break;
            case TrailType.WEST:
                _trailWest.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary> Disables the specified trail sprite </summary>
    public void DisableTrail(TrailType tt)
    {
        switch (tt)
        {
            case TrailType.NORTH:
                _trailNorth.SetActive(false);
                break;
            case TrailType.SOUTH:
                _trailSouth.SetActive(false);
                break;
            case TrailType.EAST:
                _trailEast.SetActive(false);
                break;
            case TrailType.WEST:
                _trailWest.SetActive(false);
                break;
            default:
                break;
        }
    }

    // -----------------------------------------------
    // -----           setters/getters           -----
    // -----------------------------------------------

    public bool IsTopWall()
    {
        return _wallTop.active;
    }

    public bool IsLeftWall()
    {
        return _wallLeft.active;
    }

    public int getTrailCount(TrailType tt)
    {
        return _trailCounter[(int)tt];
    }

    public void IncrementTrailCounter(TrailType tt)
    {
        if (_trailCounter[(int)tt] == 0)
        {
            EnableTrail(tt);
        }
        _trailCounter[(int)tt]++;
    }

    public void DecreaseTrailCounter(TrailType tt)
    {
        ref int ttCounter = ref _trailCounter[(int)tt];
        if (ttCounter > 0)
        {
            ttCounter--;

            if (ttCounter == 0)
            {
                DisableTrail(tt);
            }
        }
           
    }
    #endregion //methods
}
