using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextBehaviour : MonoBehaviour
{
    public TMP_Text textUI;         // Référence au texte TMP
    public string fullText;         // Texte complet à afficher
    public string sceneName;        // Nom de la scène à charger
    public float typingSpeed = 0.1f; // Vitesse d'affichage des lettres

    private bool isTyping = false;
    private bool textFullyDisplayed = false;

    void Start()
    {
        StartCoroutine(TypeText()); // Lancement automatique de l'affichage lettre par lettre
    }

    public void OnButtonClick()
    {
        if (!textFullyDisplayed)
        {
            StopAllCoroutines(); // Arrêter l'animation si en cours
            textUI.text = fullText; // Afficher directement le texte complet
            textFullyDisplayed = true;
        }
        else
        {
            SceneManager.LoadScene(sceneName); // Changer de scène
        }
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        textUI.text = ""; // Vider le texte initialement

        foreach (char letter in fullText)
        {
            textUI.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        textFullyDisplayed = true; // Le texte est entièrement affiché
    }
}
