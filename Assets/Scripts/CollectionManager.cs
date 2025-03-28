using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectionManager : MonoBehaviour
{
    public TextMeshProUGUI earth;
    public TextMeshProUGUI jupiter;
    public TextMeshProUGUI mars;
    public TextMeshProUGUI mercury;
    public TextMeshProUGUI moon;
    public TextMeshProUGUI neptune;
    public TextMeshProUGUI pluton;
    public TextMeshProUGUI saturn;
    public TextMeshProUGUI sun;
    public TextMeshProUGUI uranus;
    public TextMeshProUGUI venus;

    void Start()
    {
        //ce code est du dogshit, à refaire par après
        
        if (PlayerPrefs.GetInt("earth", -1) != -1)
        {
            earth.text = "1 - Earth";
        }
        if (PlayerPrefs.GetInt("jupiter", -1) != -1)
        {
            jupiter.text = "2 - Jupiter";
        }
        if (PlayerPrefs.GetInt("mars", -1) != -1)
        {
            mars.text = "3 - Mars";
        }
        if (PlayerPrefs.GetInt("mercury", -1) != -1)
        {
            mercury.text = "4 - Mercury";
        }
        if (PlayerPrefs.GetInt("moon", -1) != -1)
        {
            moon.text = "5 - Moon";
        }
        if (PlayerPrefs.GetInt("neptune", -1) != -1)
        {
            neptune.text = "6 - Neptune";
        }
        if (PlayerPrefs.GetInt("pluton", -1) != -1)
        {
            pluton.text = "7 - Pluton";
        }
        if (PlayerPrefs.GetInt("saturn", -1) != -1)
        {
            saturn.text = "8 - Saturn";
        }
        if (PlayerPrefs.GetInt("sun", -1) != -1)
        {
            sun.text = "9 - Sun";
        }
        if (PlayerPrefs.GetInt("uranus", -1) != -1)
        {
            uranus.text = "10 - Uranus";
        }
        if (PlayerPrefs.GetInt("venus", -1) != -1)
        {
            venus.text = "11 - Venus";
        }
    }

    public void loadPlanetView(string planet)
    {
        if (PlayerPrefs.GetInt(planet, -1) != -1)
        {
            CollectionStatic.setCurrentPlanet(planet);
            SceneManager.LoadScene("PlanetTextView");
        }
        
    }

    
}
