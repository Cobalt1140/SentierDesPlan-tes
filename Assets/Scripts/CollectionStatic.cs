using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionStatic
{
    //Cette classe static va se souvenir de la derni�re plan�te avec laquelle on a interragis, pour la fournir par apr�s dans PlanetTextView
    
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
