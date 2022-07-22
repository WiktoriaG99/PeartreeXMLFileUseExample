using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

//Skrypt zawierający obsługę dialogu w grze

public class DialogueManager : MonoBehaviour
{
    XmlDocument dialogueXML;
    public GameObject dialogueWindow;
    public Text NPCName;
    public Text NPCLine;
    public Image NPCImage; 
    public GameObject responceButtonPrefab;

    //Wczytanie zawartości pliku Dialogue.XML
    private void Awake()
    {
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/Dialogue");
        dialogueXML = new XmlDocument();
        dialogueXML.LoadXml(xmlTextAsset.text);
    }

    private void Update()
    {
        /*
        Jeżeli gracz jest na triggerze Postaci Niezależnej (DialogueData.dialogueEnabled==true)
        oraz DialogueData.current_dialogue_npc nie jest puste, to
        naciśnięcie E rozpoczyna dialog
        */
        if (Input.GetKeyDown(KeyCode.E) && DialogueData.dialogueEnabled==true && DialogueData.current_dialogue_npc!="")
        {
            //Jeżeli okno dialogowe nie jest już otwarte
            if(!dialogueWindow.activeSelf)
            {
                dialogueWindow.SetActive(true);
                setDialogue();
            }
        }
    }

    /*
     Dialog NPC
     Zaczęcie rozmowy - NPC losuje kwestię dialogową
     */
    private void setDialogue()
    {
        int NPCDialogueID;

        //Przypisanie ID kwestii dialogowej
        NPCDialogueID = setNPCLine(DialogueData.current_dialogue_npc);

        //Wybranie węzła o podanym ID
        XmlNode currentNode = dialogueXML.SelectSingleNode("//dialogues/dialogue[@id = '" + NPCDialogueID + "']");

        //Gdy nie ma węzła o danym ID, a jest to rozpoczęcie rozmowy z NPC, to dialog nie rozpoczyna się
        if (currentNode == null)
        {
            Debug.Log("Nie znaleziono dialogu NPC'a o danym ID");
            //Zakończenie rozmowy
            EndOfTheConversation();
        }
        //Gdy jest węzeł o danym ID
        else
        {
            //Utworzenie obiektu klasy NPCDialogue
            NPCDialogue currentDialogue = new NPCDialogue(currentNode);
            //Wyświetlenie linii NPC
            NPCLine.text = currentDialogue.dialogueLine;
            //Wyświetlenie imienia NPC
            NPCName.text = currentDialogue.dialogueCharacter;
            //Wyświetlenie sprite'a NPC
            var sprite = Resources.Load<Sprite>("Sprites/" + currentDialogue.dialogueImage);
            NPCImage.GetComponent<Image>().sprite = sprite;

            //Brak dostępnych kwestii dialogowych, jeżeli 1 element tablicy dialogueLink jest pusty
            if (currentDialogue.dialogueLink[0] == "")
            {
                //Zakończenie rozmowy z opcją do wyboru "Zakończ rozmowę"
                EndOfTheConversationWithPlayerLine();
            }
            //Wyświetlenie odpowiedzi gracza
            //Przekazanie długości tablicy potrzebnej do dodania odpowiedniej ilości instancji prefaba z odpowiedziami gracza
            //oraz tablicy z ID odpowiedzi
            else
            {
                AddResponces(currentDialogue.dialogueLink.Length, currentDialogue.dialogueLink);
            }
        }
    }

    //Ustawienie dialogu NPC w zależności od kryteriów;
    //Zostanie wybrany jeden pasujący lub wylosowany ze wszystkich spełniających kryteria
    private int setNPCLine(string NPCName)
    {
        int ID;

        //Pobranie poziomu przyjaźni z danym NPC
        string frienshipLevel = DialogueData.friendshipLevelNPC[DialogueData.current_dialogue_npc];

        XmlNodeList dialoguesNodeList;

        //Wybranie kwestii (węzłów) z pliku z dialogami, które spełniają określone kryteria
        //Tu: Imię postaci niezależnej; czy jest to rozpoczęcie rozmowy; odpowiedni poziom przyjaźni; aktualna pogoda w grze albo kwestia niewymagająca określonej pogody
        dialoguesNodeList = dialogueXML.SelectNodes("//dialogues/dialogue[peartree_character = '" + NPCName +  "'" +
            "and peartree_start_conversation = 'yes'" +
            "and peartree_friendship_level = '" + frienshipLevel + "'" +
            "and peartree_weather = '' " +
            "or peartree_character = '" + NPCName + "'" +
            "and peartree_start_conversation = 'yes'" +
            "and peartree_friendship_level = '" + frienshipLevel + "'" +
            "and peartree_weather = '" + DialogueData.current_weather + "']");

        //Losowanie dialogu spośród tych spełniających kryteria
        UnityEngine.Random rnd = new UnityEngine.Random();
        int randomLine = (UnityEngine.Random.Range(0, dialoguesNodeList.Count));
        XmlNode node = dialoguesNodeList[randomLine];
        //Przypisanie ID wybranej kwestii dialogowej
        ID = XmlConvert.ToInt32(node.Attributes["id"].Value);

        return ID;
    }

    /*
     Dialog NPC
     Odpowiedź na opcję dialogową gracza
     */
    private void setDialogue(string ID)
    {
        XmlNode currentNode = dialogueXML.SelectSingleNode("//dialogues/dialogue[@id = '"+ ID +"']");

        //Gdy nie ma węzła o danym ID
        //Jeżeli do odpowiedzi gracza nie jest podpięta żadna odpowiedź NPC'a
        if (currentNode == null)
        {
            EndOfTheConversation();
        }
        else
        {
            //Utworzenie obiektu klasy NPCDialogue
            NPCDialogue currentDialogue = new NPCDialogue(currentNode);

            //Wyświetlenie linii NPC
            NPCLine.text = currentDialogue.dialogueLine;
            //Wyświetlenie imienia NPC
            NPCName.text = currentDialogue.dialogueCharacter;
            //Wyświetlenie sprite'a NPC
            var sprite = Resources.Load<Sprite>("Sprites/" + currentDialogue.dialogueImage);
            NPCImage.GetComponent<Image>().sprite = sprite;

            //!!!Brak dostępnych kwestii dialogowych gracza, jeżeli 1wszy element tablicy z connection jest pusty
            if (currentDialogue.dialogueLink[0] == "")
            {
                //Zakończenie rozmowy
                EndOfTheConversationWithPlayerLine();
            }
            //Wyświetlenie odpowiedzi gracza
            //Przekazanie długości tablicy potrzebnej do dodania odpowiedniej ilości instrancji button prefaba z odpowiedziami gracza
            //oraz tablicy z ID odpowiedzi
            else
            {
                AddResponces(currentDialogue.dialogueLink.Length, currentDialogue.dialogueLink);
            }
        }
    }

    //Metoda kończąca rozmowę z przyciskiem z treścią "Zakończ rozmowę."
    //Wywoływana wtedy, gdy do dialogu NPC nie ma podpiętej następnej odpowiedzi gracza
    private void EndOfTheConversationWithPlayerLine()
    {
        GameObject playerResponcesContainer = GameObject.Find("PlayerResponcesContainer");
        Transform playerResponcesContainerTransform = playerResponcesContainer.GetComponent<Transform>();
        GameObject responceButton;

        //Usuwanie poprzednich buttonów z odpowiedziami gracza
        foreach (Transform child in playerResponcesContainerTransform)
        {
            GameObject.Destroy(child.gameObject);
        }

        responceButton = Instantiate(responceButtonPrefab, playerResponcesContainerTransform);

        responceButton.GetComponentInChildren<Text>().text = "Zakończ rozmowę.";

        responceButton.GetComponent<Button>().onClick.AddListener(CloseDialogWindow);
    }

    //Metoda kończąca rozmowę od razu, bez tworzenia instancji przycisku z treścią "Zakończ rozmowę."
    //Wywoływana wtedy, gdy do dialogu gracza nie ma podpiętej następnej odpowiedzi NPC

    public void EndOfTheConversation()
    {
        GameObject playerResponcesContainer = GameObject.Find("PlayerResponcesContainer");
        Transform playerResponcesContainerTransform = playerResponcesContainer.GetComponent<Transform>();

        //Usuwanie poprzednich buttonów z odpowiedziami gracza
        foreach (Transform child in playerResponcesContainerTransform)
        {
            GameObject.Destroy(child.gameObject);
        }

       CloseDialogWindow();
    }

    private void CloseDialogWindow()
    {
        GameObject playerResponcesContainer = GameObject.Find("PlayerResponcesContainer");
        Transform playerResponcesContainerTransform = playerResponcesContainer.GetComponent<Transform>();

        //Usuwanie poprzednich buttonów z odpowiedziami gracza
        foreach (Transform child in playerResponcesContainerTransform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Czyszczenie okna dialogowoego
        NPCLine.text = "";
        NPCName.text = "";
        //Wyłączenie DialogueCanvas
        dialogueWindow.SetActive(false);
    }

    //Dodanie możliwych odpowiedzi gracza, na podstawie podłączonych ID (links)
    private void AddResponces(int numberOfResponces, string [] links)
    {
        //Znalezenie objektu gry, w którym pojawią się przyciski z odpowiedziami gracza
        GameObject playerResponcesContainer = GameObject.Find("PlayerResponcesContainer");
        Transform playerResponcesContainerTransform = playerResponcesContainer.GetComponent<Transform>();
        GameObject responceButton;

        //Usunięcie poprzednich przycisków z odpowiedziami gracza
        foreach (Transform child in playerResponcesContainerTransform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Tworzenie instancji przycisków z odpowiedziami gracza
        for (int i = 0; i < numberOfResponces; i++)
        {
            responceButton = Instantiate(responceButtonPrefab, playerResponcesContainerTransform);

            //Przypisanie do elementu Text przycisku kwestii z danego ID
            responceButton.GetComponentInChildren<Text>().text = playerResponce(links[i]);
            //Przypisanie do przycisku onClicka z ID odpowiedzi
            responceButton.GetComponent<Button>().AddEventListener(links[i], choosedResponce);
        }
    }

    private string playerResponce(string ID)
    {
        string responce = "";
        XmlNode currentNode = dialogueXML.SelectSingleNode("//dialogues/dialogue[@id = '"+ ID +"']");
        if (currentNode == null)
        {
            Debug.LogError("Nie znaleziono dialogu z podanym ID.");
        }
        //Utworzenie obiektu klasy PlayerDialogue
        PlayerDialogue playerDialogue = new PlayerDialogue(currentNode);
        responce = playerDialogue.dialogueLine;
        return responce;
    }

    private void choosedResponce(string ID)
    {
        XmlNode currentNode = dialogueXML.SelectSingleNode("//dialogues/dialogue[@id = '" + ID + "']");
        if (currentNode == null)
        {
            Debug.LogError("Nie znaleziono dialogu z podanym ID.");
        }
        PlayerDialogue playerDialogue = new PlayerDialogue(currentNode);
        setDialogue(playerDialogue.dialogueLink);
    }

}
class NPCDialogue
{
    public int dialogueID { get; private set; }
    public string dialogueCharacter { get; private set; }
    public string dialogueLine { get; private set; }
    public string dialogueStart { get; private set; }
    public string[] dialogueLink { get; private set; }
    public string dialogueImage { get; private set; }
    public string dialogueWeather { get; private set; }
    public string dialogueFriendshipLevel { get; private set; }

    public NPCDialogue(XmlNode currentDialogue)
    {
        dialogueID = XmlConvert.ToInt32(currentDialogue.Attributes["id"].Value);
        dialogueCharacter = currentDialogue["peartree_character"].InnerText;
        dialogueLine = currentDialogue["peartree_line"].InnerText;
        dialogueStart = currentDialogue["peartree_start_conversation"].InnerText;
        dialogueLink = currentDialogue["peartree_link"].InnerText.Split(',');
        dialogueImage = currentDialogue["peartree_image"].InnerText;
        dialogueWeather = currentDialogue["peartree_weather"].InnerText;
        dialogueFriendshipLevel = currentDialogue["peartree_friendship_level"].InnerText;
    }
}

class PlayerDialogue
{
    public int dialogueID { get; private set; }
    public string dialogueCharacter { get; private set; }
    public string dialogueLine { get; private set; }
    public string dialogueLink { get; private set; }

    public PlayerDialogue(XmlNode currentDialogue)
    {
        dialogueID = XmlConvert.ToInt32(currentDialogue.Attributes["id"].Value);
        dialogueCharacter = currentDialogue["peartree_character"].InnerText;
        dialogueLine = currentDialogue["peartree_line"].InnerText;
        dialogueLink = currentDialogue["peartree_link"].InnerText;
    }
}

//Rozszerzenie przycisku, aby poprawnie działało przypisywanie ID po kliknięciu w przycisk
public static class ButtonExtension
{
    public static void AddEventListener<T> (this Button button, T parameter, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate ()
        {
            OnClick(parameter);
        });
    }
}