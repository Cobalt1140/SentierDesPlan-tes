using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstTimePref : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Peut-�tre � refaire, je sais pas si ceci est la meilleure fa�on de skip les pr�f�rences d'intro
        if (PlayerPrefs.GetInt("AgePreference", -1) != -1 && PlayerPrefs.GetInt("LanguePreference", -1) != -1)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
