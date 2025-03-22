using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextBehaviour : MonoBehaviour
{
    public TMP_Text textUI;          // Texte TMP � afficher
    public string fullText;          // Texte complet � �crire
    public string sceneName;         // Nom de la sc�ne � charger
    public float typingSpeed = 0.1f; // Vitesse de frappe

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
