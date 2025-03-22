using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextBehaviour : MonoBehaviour
{
    public TMP_Text textUI;          // Texte TMP à afficher
    public string fullText;          // Texte complet à écrire
    public string sceneName;         // Nom de la scène à charger
    public float typingSpeed = 0.1f; // Vitesse de frappe

    private bool isTyping = false;
    private bool textFullyDisplayed = false;

    void Start()
    {
        if (textUI == null)
        {
            Debug.LogError("Référence au TMP_Text manquante dans l'inspecteur !");
            return;
        }

        if (string.IsNullOrEmpty(fullText))
        {
            Debug.LogWarning("Le texte à afficher (fullText) est vide.");
        }

        StartCoroutine(TypeText());
    }

    public void OnButtonClick()
    {
        if (!textFullyDisplayed)
        {
            Debug.Log(" Texte affiché instantanément.");
            StopAllCoroutines();
            textUI.text = fullText;
            textFullyDisplayed = true;
        }
        else
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                Debug.Log(" Chargement de la scène : " + sceneName);
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("Nom de scène vide !");
            }
        }
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        textUI.text = "";

        foreach (char letter in fullText)
        {
            textUI.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        textFullyDisplayed = true;
        Debug.Log(" Texte affiché entièrement.");
    }
}
