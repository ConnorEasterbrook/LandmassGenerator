using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public MapDisplay mapDisplay;
    public Dropdown islandTypeDropdown;

    private void Start() 
    {
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        IslandTypeDropdown();
    }

    public void IslandTypeDropdown()
    {
        string[] enumNames = Enum.GetNames(typeof(MapDisplay.IslandTypeEnum));
        List<string> names = new List<string>(enumNames);

        islandTypeDropdown.AddOptions(names);

        islandTypeDropdown.onValueChanged.AddListener(delegate
        {
            IslandTypeDropdown_Changed(islandTypeDropdown);
        });
    }

    public void IslandTypeDropdown_Changed(Dropdown change)
    {
        if (change.value == 0)
        {
            mapDisplay.islandType = MapDisplay.IslandTypeEnum.Regular;
        }
        else if (change.value == 1)
        {
            mapDisplay.islandType = MapDisplay.IslandTypeEnum.Pangaea;
        }
        else if (change.value == 2)
        {
            mapDisplay.islandType = MapDisplay.IslandTypeEnum.Round;
        }
        else if (change.value == 3)
        {
            mapDisplay.islandType = MapDisplay.IslandTypeEnum.Ring;
        }
        else if (change.value == 4)
        {
            mapDisplay.islandType = MapDisplay.IslandTypeEnum.Archipelago;
        }
        else if (change.value == 5)
        {
            mapDisplay.islandType = MapDisplay.IslandTypeEnum.Lake;
        }
    }
}
