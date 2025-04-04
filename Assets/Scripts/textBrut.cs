using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class textBrut : MonoBehaviour
{
    public TMP_Text text;             // R�f�rence au texte � afficher
    public string fullText;           // Texte � afficher
    public string sceneName;          // Nom de la sc�ne suivante
    private int clickCount = 0;

    public void OnButtonClick()
    {
        clickCount++;
        Debug.Log(" Bouton cliqu� - Count: " + clickCount);

        if (clickCount == 1)
        {
            if (text != null)
            {
                text.text = fullText;
                Debug.Log(" Texte affich� : " + fullText);
            }
            else
            {
                Debug.LogError(" R�f�rence TMP_Text non assign�e !");
            }
        }
        else if (clickCount == 2)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                Debug.Log(" Chargement de la sc�ne : " + sceneName);
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError(" Aucun nom de sc�ne fourni !");
            }
        }
    }
}
