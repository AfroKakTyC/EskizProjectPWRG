using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataTypes;

public class PrefabContainer : MonoBehaviour
{
    public GameObject door;
    public GameObject double_leaf_window;
    public GameObject tricuspid_window;
    public GameObject balcony_left_door;
    public GameObject balcony_right_door;

    public GameObject GetWindow(WindowType type)
	{
        if (type == WindowType.double_leaf_window)
		{
            return double_leaf_window;
		}
        else if (type == WindowType.tricuspid_window)
		{
            return tricuspid_window;
		}
        else if (type == WindowType.balcony_left_door)
		{
            return balcony_left_door;
		}
        else if (type == WindowType.balcony_right_door)
		{
            return balcony_right_door;
		}            
        else
		{
            Debug.LogError("Prefab container does not have that type of window '" + type + "'");
            return (null);
		}
	}
}
