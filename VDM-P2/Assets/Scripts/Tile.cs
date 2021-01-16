using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: do we want 4 wall types and have it easier, or 2 types and have it cleaner?
public struct WallType
{
    public bool top, /*right, bottom,*/ left;
}

public class Tile : MonoBehaviour
{
    #region variables
    [SerializeField] [Tooltip("Child component that stores the ice floor sprite")]
    private GameObject iceFloor;
    [SerializeField] [Tooltip("Child component that stores the top wall sprite")]
    private GameObject wallTop;
    //[SerializeField] [Tooltip("Child component that stores the right wall sprite")]
    //private GameObject wallRight;
    //[SerializeField] [Tooltip("Child component that stores the right wall sprite")]
    //private GameObject wallBottom;
    [SerializeField] [Tooltip("Child component that stores the left wall sprite")]
    private GameObject wallLeft;
    [SerializeField] [Tooltip("Child component that stores the goal sprite")]
    private GameObject goal;
    [SerializeField] [Tooltip("Trail looking up")]
    private GameObject trailUp;
    [SerializeField] [Tooltip("Trail looking Right")]
    private GameObject trailRight;
    [SerializeField] [Tooltip("Trail looking down")]
    private GameObject trailDown;
    [SerializeField] [Tooltip("Trail looking left")]
    private GameObject trailLeft;
    #endregion //variables

    #region methods
    private void Start()
    {
#if UNITY_EDITOR
        if (iceFloor == null)
        {
            Debug.LogError("iceFloor is null. Can't start without the ice floor sprite");
            gameObject.SetActive(false);
            return;
        }
        if (wallTop == null)
        {
            Debug.LogError("wallTop is null. Can't start without the wall top sprite");
            gameObject.SetActive(false);
            return;
        }
        if (wallLeft == null)
        {
            Debug.LogError("wallLeft is null. Can't start without the wall left sprite");
            gameObject.SetActive(false);
            return;
        }
        if (goal == null)
        {
            Debug.LogError("goal is null. Can't start without the goal sprite");
            gameObject.SetActive(false);
            return;
        }
#endif
    }

    /// <summary> Enables the ice sprite </summary>
    public void EnableIce()
    {
        iceFloor.SetActive(true);
    }
    /// <summary> Disables the ice sprite </summary>
    public void DisableIce()
    {
        iceFloor.SetActive(false);
    }

    /// <summary> Enables the given wall sprites </summary>
    public void EnableWalls(WallType walls)
    {
        if (walls.top) wallTop.SetActive(true);
        if (walls.left) wallLeft.SetActive(true);

    }
    /// <summary> Disables the given wall sprites </summary>
    public void DisableWalls(WallType walls)
    {
        if (walls.top) wallTop.SetActive(false);
        if (walls.left) wallLeft.SetActive(false);
    }

    /// <summary> Enables the goal sprite </summary>
    public void EnableGoal()
    {
        goal.SetActive(true);
    }
    /// <summary> Disables the goal sprite </summary>
    public void DisableGoal()
    {
        goal.SetActive(false);
    }
    #endregion //methods
}
