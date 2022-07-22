using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCOnTriggerEnter : MonoBehaviour
{
    public string CharacterName;
    public GameObject PressToTalk;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Jeżeli na trigger wejdzie gracz
        if (collision.gameObject.tag == "Player")
        {
            //Przypisanie imienia postaci niezależnej, z którą odbędzie się dialog
            DialogueData.current_dialogue_npc = CharacterName;
            //Włączenie możliwości uruchomienia dialogu
            DialogueData.dialogueEnabled = true;
            //Pojawienie się informacji o możliwości przeprowadzenia dialogu
            PressToTalk.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Jeżeli gracz wyjdzie z triggera
        if (collision.gameObject.tag == "Player")
        {
            //Przypisanie aktualnej postaci niezależnej na pusty łańcuch
            DialogueData.current_dialogue_npc = "";
            //Wyłączenie możliwości uruchomienia dialogu
            DialogueData.dialogueEnabled = false;
            //Wyłączenie informacji o możliwości przeprowadzenia dialogu
            PressToTalk.gameObject.SetActive(false);
        }
    }
}
