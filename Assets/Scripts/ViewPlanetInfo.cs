using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;


public class ViewPlanetInfo : MonoBehaviour
{
    public TextMeshProUGUI title;

    string currentPlanet;
    private Dictionary<string, string> DicURLs = new Dictionary<string, string>()
    {
        {"sun", "https://science.nasa.gov/sun/" },
        {"mercury","https://science.nasa.gov/mercury/facts/" },
        {"venus", "https://science.nasa.gov/venus/venus-facts/" },
        {"earth", "https://science.nasa.gov/earth/facts/" },
        {"mars", "https://science.nasa.gov/mars/facts/"},
        {"jupiter","https://science.nasa.gov/jupiter/jupiter-facts/" },
        {"saturn", "https://science.nasa.gov/saturn/facts/" },
        {"uranus", "https://science.nasa.gov/uranus/facts/" },
        {"neptune", "https://science.nasa.gov/neptune/neptune-facts/" },
        

    };
    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Le Soleil", "Mercure", "Venus", "La Terre", "Mars", "Jupiter", "Saturne", "Uranus", "Neptune"} },
        { "English", new string[] { "The Sun", "Mercury", "Venus", "The Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" } },
        //{ "OG English", new string[] { "sun", "mercury", "venus", "earth", "mars", "jupiter", "saturn", "uranus", "neptune" } }
    };


    private string[] PrefabPlanetList = new string[] { "sun", "mercury", "venus", "earth", "mars", "jupiter", "saturn", "uranus", "neptune" };
    private bool currentlyHeader = true;


    void Start()
    {
        string language = (PlayerPrefs.GetInt("LanguePreference", 0) == 0) ? "French" : "English";
        currentPlanet = CollectionStatic.getCurrentPlanet();
        if (translations.ContainsKey(language))
        {
            int cpt = 0;
            int index = 0;
            foreach (string planet in PrefabPlanetList)
            {
                if (planet.Contains(currentPlanet))
                {
                    index = cpt;
                }
                cpt++;
            }
            title.text = translations[language][index];

        }
        
       

    }


    public void OpenURL()
    {
        if (DicURLs.ContainsKey(currentPlanet))
        {
            string URL = DicURLs[currentPlanet];
            Application.OpenURL(URL);
        }
        else
        {
            Debug.Log("Tag non trouvé dans le dictionnaire : " + currentPlanet);
        }
    }

    public void switchHeaderAndBody() //switch between header text and body text of the planet
    {

    }
    
}
