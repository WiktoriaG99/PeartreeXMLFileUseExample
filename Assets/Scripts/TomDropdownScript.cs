using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TomDropdownScript : MonoBehaviour
{
    public Dropdown dropdown;
    public void changeData()
    {
        if (dropdown.value == 0)
        {
            DialogueData.friendshipLevelNPC["Tom"] = "stranger";
        }
        if (dropdown.value == 1)
        {
            DialogueData.friendshipLevelNPC["Tom"] = "friend";
        }

    }
}
