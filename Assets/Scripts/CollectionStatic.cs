using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionStatic
{
    public static string currentPlanet = "none";
    public static string getCurrentPlanet()
    {
        return currentPlanet;
    }

    public static void setCurrentPlanet(string planet)
    {

        currentPlanet = planet;
        
    }
}
