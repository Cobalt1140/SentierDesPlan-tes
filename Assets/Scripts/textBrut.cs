using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class textBrut : MonoBehaviour
{
    public TMP_Text text; // Référence au texte à afficher
    public string fullText;
    public string sceneName; // Change le nom de la scène
    private int clickCount = 0;

    public void OnButtonClick()
    {
        clickCount++;

        if (clickCount == 1)
        {
            text.text = fullText; // Afficher le texte complet
        }
        else if (clickCount == 2)
        {
            SceneManager.LoadScene(sceneName); // Changer de scène
        }
    }
}
