using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string sceneName; // Nom de la sc�ne � charger

    public void OnButtonClick()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Le nom de la sc�ne est vide ou non d�fini !");
            return;
        }

        Debug.Log("Chargement de la sc�ne : " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
