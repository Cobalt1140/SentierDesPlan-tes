using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string sceneName; // Nom de la scène à charger

    public void OnButtonClick()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Le nom de la scène est vide ou non défini !");
            return;
        }

        Debug.Log("Chargement de la scène : " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
