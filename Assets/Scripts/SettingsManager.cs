using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public TextMeshProUGUI LOGOTEXT;

    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Parametre" } },
        { "English", new string[] { "Settings"} }
    };

    private void Start()
    {
        int ageIndex = PlayerPrefs.GetInt("AgePreference", 0);
        int langIndex = PlayerPrefs.GetInt("LanguePreference", 0);

        // Définir les langues (modifie les index en fonction des choix du dropdown)
        string language = (langIndex == 0) ? "French" : "English";
        bool isAdult = (ageIndex == 0); // 0 = Adulte, 1 = Enfant (modifie selon ton dropdown)
        if (translations.ContainsKey(language))
        {
            LOGOTEXT.text = translations[language][0];
           
        }
    }
}
