using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnaDropdownScript : MonoBehaviour
{
    public Dropdown dropdown;
    public void changeData()
    {
        if (dropdown.value == 0)
        {
            DialogueData.friendshipLevelNPC["Anna"] = "stranger";
        }
        if (dropdown.value == 1)
        {
            DialogueData.friendshipLevelNPC["Anna"] = "friend";
        }
        
    }
}
