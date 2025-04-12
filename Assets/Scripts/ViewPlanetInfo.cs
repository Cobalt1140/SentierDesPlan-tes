using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Mapbox.Unity.Constants;
using static System.Net.WebRequestMethods;


public class ViewPlanetInfo : MonoBehaviour
{
    public TextMeshProUGUI title;
    public GameObject Viewport;

    private TextMeshProUGUI content;
    private Image image;

    string currentPlanet;
    private Dictionary<string, string[]> DicURLs = new Dictionary<string, string[]>()
    {
        {"sun", new string[]{ "https://cnes.fr/dossiers/soleil", "https://science.nasa.gov/sun/" }  },
        {"mercury",new string[]{ "https://cnes.fr/dossiers/planete-mercure", "https://science.nasa.gov/mercury/facts/" } },
        {"venus", new string[]{ "https://cnes.fr/dossiers/planete-venus", "https://science.nasa.gov/venus/venus-facts/" } },
        {"earth", new string[]{ "https://cnes.fr/dossiers/planete-terre", "https://science.nasa.gov/earth/facts/" } },
        {"mars", new string[]{ "https://cnes.fr/dossiers/planete-mars","https://science.nasa.gov/mars/facts/" } },
        {"jupiter",new string[]{"https://cnes.fr/dossiers/planete-jupiter", "https://science.nasa.gov/jupiter/jupiter-facts/" } },
        {"saturn", new string[]{"https://cnes.fr/dossiers/planete-saturne", "https://science.nasa.gov/saturn/facts/" } },
        {"uranus", new string[]{"https://cnes.fr/dossiers/planete-uranus", "https://science.nasa.gov/uranus/facts/" } },
        {"neptune", new string[]{"https://cnes.fr/dossiers/planete-neptune", "https://science.nasa.gov/neptune/neptune-facts/" } }


    };
    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Le Soleil", "Mercure", "Venus", "La Terre", "Mars", "Jupiter", "Saturne", "Uranus", "Neptune"} },
        { "English", new string[] { "The Sun", "Mercury", "Venus", "The Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" } },
        //{ "OG English", new string[] { "sun", "mercury", "venus", "earth", "mars", "jupiter", "saturn", "uranus", "neptune" } }
    };

    
    private string[] PrefabPlanetList = new string[] { "sun", "mercury", "venus", "earth", "mars", "jupiter", "saturn", "uranus", "neptune" };
    private bool currentlyHeader = true;
    private PlanetInfo planetInfo;


    void Start()
    {
        /*
        //debug
        PlayerPrefs.SetInt("mercury", 1);
        PlayerPrefs.SetInt("venus", 1);
        PlayerPrefs.SetInt("earth", 1);
        PlayerPrefs.SetInt("mars", 1);
        PlayerPrefs.SetInt("jupiter", 1);
        PlayerPrefs.SetInt("saturn", 1);
        PlayerPrefs.SetInt("uranus", 1);
        PlayerPrefs.SetInt("neptune", 1);
        */
        content = Viewport.GetComponentInChildren<TextMeshProUGUI>();
        image = content.GetComponentInChildren<Image>();
        string language = (PlayerPrefs.GetInt("LanguePreference", 0) == 0) ? "French" : "English";
        currentPlanet = CollectionStatic.getCurrentPlanet();
        if (translations.ContainsKey(language))
        {
            int cpt = 0;
            int index = -1;
            foreach (string planet in PrefabPlanetList)
            {
                if (planet.Contains(currentPlanet))
                {
                    index = cpt;
                }
                cpt++;
            }
            if (index != -1)
            {
                title.text = translations[language][index];
            }
            planetInfo = getJsonPlanetText(language.ToLower());
            currentlyHeader = true;
            content.text = planetInfo.header;
            image.sprite = Resources.Load<Sprite>("PlanetImages/" + currentPlanet);
            
        }
        
       

    }


    public void OpenURL()
    {

        if (DicURLs.ContainsKey(currentPlanet))
        {
            string URL = DicURLs[currentPlanet][PlayerPrefs.GetInt("LanguePreference",0)];
            Application.OpenURL(URL);
        }
        else
        {
            Debug.Log("Tag non trouvé dans le dictionnaire : " + currentPlanet);
        }
    }

    private PlanetInfo getJsonPlanetText(string language)
    {
        string path = "PlanetJson/" + currentPlanet + " - " + language;
        TextAsset jsonText = Resources.Load<TextAsset>(path);
        if (jsonText == null)
        {
            Debug.LogError("Couldn't load JSON from: " + path);
            return new PlanetInfo { header = "FUNNY \r\nQu'est-ce que le Lorem Ipsum?\r\n\r\nLe Lorem Ipsum est simplement du faux texte employé dans la composition et la mise en page avant impression. Le Lorem Ipsum est le faux texte standard de l'imprimerie depuis les années 1500, quand un imprimeur anonyme assembla ensemble des morceaux de texte pour réaliser un livre spécimen de polices de texte. Il n'a pas fait que survivre cinq siècles, mais s'est aussi adapté à la bureautique informatique, sans que son contenu n'en soit modifié. Il a été popularisé dans les années 1960 grâce à la vente de feuilles Letraset contenant des passages du Lorem Ipsum, et, plus récemment, par son inclusion dans des applications de mise en page de texte, comme Aldus PageMaker.\r\nPourquoi l'utiliser?\r\n\r\nOn sait depuis longtemps que travailler avec du texte lisible et contenant du sens est source de distractions, et empêche de se concentrer sur la mise en page elle-même. L'avantage du Lorem Ipsum sur un texte générique comme 'Du texte. Du texte. Du texte.' est qu'il possède une distribution de lettres plus ou moins normale, et en tout cas comparable avec celle du français standard. De nombreuses suites logicielles de mise en page ou éditeurs de sites Web ont fait du Lorem Ipsum leur faux texte par défaut, et une recherche pour 'Lorem Ipsum' vous conduira vers de nombreux sites qui n'en sont encore qu'à leur phase de construction. Plusieurs versions sont apparues avec le temps, parfois par accident, souvent intentionnellement (histoire d'y rajouter de petits clins d'oeil, voire des phrases embarassantes).\r\n", body = "\r\nLorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi iaculis, felis et hendrerit consectetur, est felis rutrum leo, sodales aliquet nibh nisi at leo. In hac habitasse platea dictumst. In vehicula, neque non hendrerit volutpat, sem magna tempor felis, a venenatis urna elit in erat. Sed tempus ut nisi in rhoncus. Sed efficitur turpis non consequat rutrum. Mauris mi sem, commodo vitae aliquam ultricies, varius at mi. Praesent pharetra nunc leo, in mattis tortor condimentum id.\r\n\r\nPhasellus sed auctor mi. Ut tempus turpis bibendum ligula gravida ornare. Nullam a sagittis orci, ac semper dolor. Quisque quis nunc in justo vehicula rhoncus fringilla vitae elit. Vivamus sapien risus, dapibus eget nisi ac, fringilla volutpat elit. Vivamus sollicitudin augue sit amet augue vulputate, vitae volutpat ligula luctus. Vivamus metus augue, vestibulum consequat arcu et, tincidunt vehicula magna.\r\n\r\nSuspendisse justo nisl, tincidunt id arcu sed, sollicitudin efficitur turpis. Praesent aliquet et arcu et tristique. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Sed maximus ipsum at tortor feugiat, eget rhoncus orci consectetur. Pellentesque scelerisque luctus nibh, at maximus risus suscipit quis. Integer pretium accumsan felis, nec feugiat massa fringilla convallis. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Etiam sed urna dictum, gravida felis sit amet, euismod lectus.\r\n\r\nUt porttitor dolor sed felis ultrices, ac bibendum arcu accumsan. Phasellus eu aliquet neque, non bibendum diam. Interdum et malesuada fames ac ante ipsum primis in faucibus. Sed eget neque sit amet est lobortis porttitor. Quisque lacinia convallis arcu ut ultrices. Suspendisse imperdiet egestas risus vel rutrum. Nulla facilisi. Quisque feugiat pharetra nisl at mattis. Cras tristique odio id quam rhoncus consequat. Proin ipsum ex, feugiat non dapibus vel, semper sit amet metus. Suspendisse lobortis velit diam, at posuere enim interdum nec. Integer a urna ultricies, iaculis leo et, fringilla lacus.\r\n\r\nAenean convallis, massa ac euismod accumsan, lectus eros placerat leo, eget tempus ante nisl sed sapien. Quisque in purus non lorem viverra eleifend nec ac mi. Sed tempus nisi ac enim laoreet, in vestibulum sapien faucibus. Duis tincidunt pulvinar libero sit amet aliquam. Suspendisse maximus diam venenatis, semper neque vitae, interdum ligula. Sed et turpis enim. Ut fringilla sed lorem ut iaculis. Etiam aliquet nulla non tortor consequat porta. " };
        }
        return JsonConvert.DeserializeObject<PlanetInfo>(jsonText.text);
    }

    public void switchHeaderAndBody() //switch between header text and body text of the planet
    {
        if (currentlyHeader)
        {
            content.text = planetInfo.body;
            currentlyHeader = false;
        } else
        {
            content.text = planetInfo.header;
            currentlyHeader = true;
        }

    }
    
}
