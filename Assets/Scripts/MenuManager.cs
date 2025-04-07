using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI startButtonText;
    public TextMeshProUGUI settingsButtonText;
    public TextMeshProUGUI mapsButtonText;
    public TextMeshProUGUI rankingButtonText;
    
    // public GameObject adultContent;
    // public GameObject childContent;

    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Commencer", "Parametre", "Maps","Classement" } },
        { "English", new string[] { "Start", "Settings", "Maps", "Ranking" } }
    };

    void Start()
    {
        int ageMode = getAgePreference();
        int langMode = getLangPreference();
        switchToIntroPreferences(ageMode, langMode);
        ApplyPreferences(ageMode, langMode);
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
        bool isAdult = (ageIndex == 0); // 0 = Adulte, 1 = Enfant (modifie selon ton dropdown)
        
        // Appliquer la langue aux boutons
        if (translations.ContainsKey(language))
        {
            startButtonText.text = translations[language][0];
            settingsButtonText.text = translations[language][1];
            mapsButtonText.text = translations[language][2];
            rankingButtonText.text = translations[language][3];
        }
        

        // Afficher le bon contenu
       //adultContent.SetActive(isAdult);
        //childContent.SetActive(!isAdult);
    }

    void switchToIntroPreferences(int ageIndex, int langIndex)
    {
        //switch to Intro Preferences if first time or aren't set up
        if (ageIndex == -1 && langIndex == -1)
        {
            SceneManager.LoadScene("IntroPreferences");
        } 
    }
}
