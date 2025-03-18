using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class LoadScene : MonoBehaviour
    
{
    public string sceneName;  // Nom de la sc�ne � charger

    // Cette m�thode sera appel�e lorsque le bouton est cliqu�
    public void OnButtonClick()
    {
        // Charger la sc�ne sp�cifi�e
        SceneManager.LoadScene(sceneName);
    }
}
