using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectionManager : MonoBehaviour
{
    public GameObject PlanetCanvas;
    public TextMeshProUGUI title;
    

    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Le Soleil", "Mercure", "Venus", "La Terre", "Mars", "Jupiter", "Saturne", "Uranus", "Neptune", "Collection"} },
        { "English", new string[] { "The Sun", "Mercury", "Venus", "The Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Collection" } },
        
    };

    private string[] PrefabPlanetList = new string[] { "sun", "mercury", "venus", "earth", "mars", "jupiter", "saturn", "uranus", "neptune" };
    void Start()
    {
        string language = (PlayerPrefs.GetInt("LanguePreference", 0) == 0) ? "French" : "English";
        int cpt = 1;
        if (translations.ContainsKey(language))
        {
            title.text = translations[language][9];
        }
        foreach (string planet in PrefabPlanetList)
        {
            if (PlayerPrefs.GetInt(planet, -1) != -1)
            {
                if (translations.ContainsKey(language))
                {
                    PlanetCanvas.transform.Find(planet).GetComponent<TextMeshProUGUI>().text = cpt.ToString() + " - " +translations[language][cpt-1];
                }
                
            }
            ++cpt;
        }
        

    }

    //quand on clique sur un bouton de planète, elle trigger cette fonction avec la planète correspondante en param,
    //qui alors va dire à la classe static CollectionStatic sur quelle planète on est, et on load la scène planetTextView
    public void loadPlanetView(string planet)
    {
        if (PlayerPrefs.GetInt(planet, -1) != -1)
        {
            CollectionStatic.setCurrentPlanet(planet);
            SceneManager.LoadScene("PlanetTextView");
        }
        
    }

    
}
