using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Skrypt zawierający informacje potrzebne do dialogu

public static class DialogueData
{
    //Postać niezależna, z którą będzie przeprowadzony dialog
    public static string current_dialogue_npc = "";
    //Pogoda w grze
    public static string current_weather = "sunny";
    //Możliwość rozpoczęcia dialogu
    public static bool dialogueEnabled = false;
    //Aktualne poziomy przyjaźni z postaciami niezależnymi
    public static Dictionary<string, string> friendshipLevelNPC = new Dictionary<string, string>()
    {
       {"Anna", "stranger"},
       {"Tom", "stranger"}
    };

}