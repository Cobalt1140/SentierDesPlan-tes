using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        ApplyPreferences();
    }

    void ApplyPreferences()
    {
        // Charger les préférences
        int ageIndex = PlayerPrefs.GetInt("AgePreference", 0);
        int langIndex = PlayerPrefs.GetInt("LanguePreference", 0);

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
}
