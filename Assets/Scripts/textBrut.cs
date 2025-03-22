using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class textBrut : MonoBehaviour
{
    public TMP_Text text;             // Référence au texte à afficher
    public string fullText;           // Texte à afficher
    public string sceneName;          // Nom de la scène suivante
    private int clickCount = 0;

    public void OnButtonClick()
    {
        clickCount++;
        Debug.Log(" Bouton cliqué - Count: " + clickCount);

        if (clickCount == 1)
        {
            if (text != null)
            {
                text.text = fullText;
                Debug.Log(" Texte affiché : " + fullText);
            }
            else
            {
                Debug.LogError(" Référence TMP_Text non assignée !");
            }
        }
        else if (clickCount == 2)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                Debug.Log(" Chargement de la scène : " + sceneName);
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError(" Aucun nom de scène fourni !");
            }
        }
    }
}
