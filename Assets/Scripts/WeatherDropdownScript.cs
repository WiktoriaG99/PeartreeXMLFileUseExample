using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherDropdownScript : MonoBehaviour
{
    public Dropdown dropdown;
    public void changeData()
     {
        if (dropdown.value == 0)
        {
            DialogueData.current_weather = "sunny";
        }
        if (dropdown.value == 1)
        {
            DialogueData.current_weather = "rainy";
        }
     }
}
