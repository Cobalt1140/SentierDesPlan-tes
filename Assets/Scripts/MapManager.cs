using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialTitle;

    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Tutoriel", "Vous vous trouvez sur la carte! Votre objectif est de vous approcher des corps célestes marqués et d'activer la caméra pour les trouver en vue réel. Vous pouvez appuyer sur le bouton en bas à droite pour centrer la caméra sur votre localisation." } },
        { "English", new string[] { "Tutorial", "You are now on the map! Your objective is to get close to the marked celestial bodies and activate the camera to then see them in real life! You can press on the button in the bottom right to center the map on your position." } }
    };
    // Start is called before the first frame update
    void Start()
    {
        int langIndex = PlayerPrefs.GetInt("LanguePreference", 0);
        string language = (langIndex == 0) ? "French" : "English";
        if (translations.ContainsKey(language))
        {
            tutorialTitle.text = translations[language][0];
            tutorialText.text = translations[language][1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
