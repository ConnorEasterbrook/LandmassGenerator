using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public MapDisplay mapDisplay;

    public void setFalloffMap()
    {
        if (mapDisplay.useFalloff)
        {
            mapDisplay.useFalloff = false;
        }
        else
        {
            mapDisplay.useFalloff = true;
        }
    }
}
