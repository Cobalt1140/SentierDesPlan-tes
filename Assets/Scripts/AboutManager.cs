using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AboutManager : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;

    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "À Propos", "Crédits:\n" } },
        { "English", new string[] { "About", "Credits:\n" } }
    };
    // Start is called before the first frame update
    void Start()
    {
        string credits = "TWAROWSKI Hubert\nEL MOUDDEN Mohammed\nABDILLAHI DARAR Rayan\nOUBOUALI Ayoub\nBIGORO José\nKARNAY Roméo";
        int langIndex = PlayerPrefs.GetInt("LanguePreference", 0);
        string language = (langIndex == 0) ? "French" : "English";
        if (translations.ContainsKey(language))
        {
            title.text = translations[language][0];
            content.text = translations[language][1] + credits;
        }

    }

    
}
