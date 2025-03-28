using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ViewPlanetInfo : MonoBehaviour
{
    public TextMeshProUGUI title;
    
    void Start()
    {
        string currentPlanet = CollectionStatic.getCurrentPlanet();
        title.text = currentPlanet[..1].ToUpper() + currentPlanet[1..];
        //nothing for description rn coz we didn't get shit yet

    }

    
}
