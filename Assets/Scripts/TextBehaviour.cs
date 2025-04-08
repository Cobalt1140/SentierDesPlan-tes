using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextBehaviour : MonoBehaviour
{
    public TMP_Text textUI;          // Texte TMP � afficher
    private string fullText;          // Texte complet � �crire
    public string sceneName;         // Nom de la sc�ne � charger
    public float typingSpeed = 0.1f; // Vitesse de frappe
    private Dictionary<string, string[]> traductions = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Imaginez ceci... Les planetes commencent à se déplacer vers la Terre. Elles voyagent dans l'espace, tombent lentement et atterrissent dans des endroits que vous pouvez voir. Vous êtes témoin de cet evenement spectaculaire, et vous avez la possibilite de les explorer. Préparez-vous pour une aventure au coeur de l'univers." } },
        { "English", new string[] { "Imagine this... The planets are starting to move toward Earth. They travel through space, fall slowly, and land in places you can actually see. You are witness to this spectacular event, and you have the opportunity to explore them. Get ready for an adventure into the heart of the universe." } }
    };
    private bool isTyping = false;
    private bool textFullyDisplayed = false;

    void Start()
    {
        if (textUI == null)
        {
            Debug.LogError("R�f�rence au TMP_Text manquante dans l'inspecteur !");
            return;
        }
        
        if (string.IsNullOrEmpty(fullText))
        {
            Debug.LogWarning("Le texte � afficher (fullText) est vide.");
        }

        StartCoroutine(TypeText());
    }

    public void OnButtonClick()
    {
        if (!textFullyDisplayed)
        {
            Debug.Log(" Texte affich� instantan�ment.");
            StopAllCoroutines();
            textUI.text = fullText;
            textFullyDisplayed = true;
        }
        else
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                Debug.Log(" Chargement de la sc�ne : " + sceneName);
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("Nom de sc�ne vide !");
            }
        }
    }

    IEnumerator TypeText()
    {
        int langIndex = PlayerPrefs.GetInt("LanguePreference", 0);
        string langue = (langIndex == 0) ? "French" : "English";

        if (traductions.ContainsKey(langue))
        {
            fullText = traductions[langue][0];

        }
        isTyping = true;
        textUI.text = "";

        foreach (char letter in fullText)
        {
            textUI.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        textFullyDisplayed = true;
        Debug.Log(" Texte affich� enti�rement.");
    }
}