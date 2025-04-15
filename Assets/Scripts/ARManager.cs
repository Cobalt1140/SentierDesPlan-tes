using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARManager : MonoBehaviour
{

    public TextMeshProUGUI tutorialTitle;
    public TextMeshProUGUI tutorialContent;
    public TextMeshProUGUI mapsBtn;

    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Tutoriel","Carte", "Vous vous trouvez dans la vue de la caméra! Sélectionnez un corps céleste en haut de l'écran et la boussole en bas de l'écran vous guidera. Si vous ne trouvez rien près de vous, consultez de nouveau la carte. Une fois trouvé, cliquez sur le corps céleste pour le collecter." } },
        { "English", new string[] { "Tutorial","Map", "You have found yourself on the camera view! Select a celestial body on the top of the screen and let the compass on the bottom of the screen guide you to it. If you can't seem to find anything near you, check the map again. When found, click on the celestial body to collect it." } }
    };

    // Start is called before the first frame update
    void Start()
    {
        int langIndex = PlayerPrefs.GetInt("LanguePreference", 0);
        string language = (langIndex == 0) ? "French" : "English";
        if (translations.ContainsKey(language))
        {
            tutorialTitle.text = translations[language][0];
            tutorialContent.text = translations[language][2];
            mapsBtn.text = translations[language][1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
