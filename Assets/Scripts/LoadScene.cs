using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class LoadScene : MonoBehaviour
    
{
    public string sceneName;  // Nom de la scène à charger

    // Cette méthode sera appelée lorsque le bouton est cliqué
    public void OnButtonClick()
    {
        // Charger la scène spécifiée
        SceneManager.LoadScene(sceneName);
    }
}
