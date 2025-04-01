using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionStatic
{
    //Cette classe static va se souvenir de la dernière planète avec laquelle on a interragis, pour la fournir par après dans PlanetTextView
    
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
