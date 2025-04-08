using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject playButton;
    public TextMeshProUGUI settingsButtonText;
    public TextMeshProUGUI rankingButtonText;
    public TextMeshProUGUI titleText;

    private bool isTutorialFinished;
    
 
    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Commencer", "Paramètres", "Carte","Classement", "Le sentier des Planètes" } },
        { "English", new string[] { "Start", "Settings", "Map", "Rankings", "The Planet Trail" } }
    };

    void Start()
    {
        
        int ageIndex = getAgePreference(); //0 = child, 1 = adult
        int langIndex = getLangPreference(); // 0 = FR; 1 = EN
        if (ageIndex == -1 || langIndex == -1) //si première fois qu'on joue, redirige vers paramètres
        {
            SceneManager.LoadScene("Settings");
        }
        isTutorialFinished = PlayerPrefs.GetInt("tutorial", -1) != -1; //0 = pas finis tuto; 1 = finis tuto
        ApplyPreferences(ageIndex, langIndex);
    }

    int getAgePreference()
    {
        Debug.Log(PlayerPrefs.GetInt("AgePreference", -1));
        return PlayerPrefs.GetInt("AgePreference", -1);
    }

    int getLangPreference()
    {
        Debug.Log(PlayerPrefs.GetInt("LanguePreference", -1));
        return PlayerPrefs.GetInt("LanguePreference", -1);

    }
    void ApplyPreferences(int ageIndex, int langIndex)
    {

        
        // Définir les langues (modifie les index en fonction des choix du dropdown)
        string language = (langIndex == 0) ? "French" : "English";
        bool isAdult = (ageIndex == 0); // 0 = child, 1 = adult (modifie selon ton dropdown)
        
        // Appliquer la langue aux boutons
        if (translations.ContainsKey(language))
        {
            if (!isTutorialFinished)
            {
                playButton.GetComponentInChildren<TextMeshProUGUI>().text = translations[language][0];
                playButton.GetComponent<Button>().onClick.AddListener(() => switchScenes("FirstPage"));
            } else
            {
                playButton.GetComponentInChildren<TextMeshProUGUI>().text = translations[language][2];
                playButton.GetComponent<Button>().onClick.AddListener(() => switchScenes("Map"));
            }
            
            settingsButtonText.text = translations[language][1];
            rankingButtonText.text = translations[language][3];
            titleText.text = translations[language][4];
        }
        

        // Afficher le bon contenu
       //adultContent.SetActive(isAdult);
        //childContent.SetActive(!isAdult);
    }

    private void switchScenes(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    
}
