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
        { "French", new string[] { "� Propos", "Cr�dits:\n","\n \n Cette application est une collaboration entre la HELB et l'ULB, dans les domaines de l'informatique et de l'astronomie." } },
        { "English", new string[] { "About", "Credits:\n", "\n \n This application is a collaboration between the HELB and the ULB, in the domains of computer science and astronomy." } }
    };
    // Start is called before the first frame update
    void Start()
    {
        string credits = "TWAROWSKI Hubert\nEL MOUDDEN Mohammed\nABDILLAHI DARAR Rayan\nOUBOUALI Ayoub\nBIGORO Jos�\nKARNAY Rom�o";
        int langIndex = PlayerPrefs.GetInt("LanguePreference", 0);
        string language = (langIndex == 0) ? "French" : "English";
        if (translations.ContainsKey(language))
        {
            title.text = translations[language][0];
            content.text = translations[language][1] + credits + translations[language][2];
            
        }

    }

    
}
