using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Networking;
using System.IO;

public class LocationStuff : MonoBehaviour
{
    [Header("UI Debug")]
    public TMP_Text debugTxt;
    public TMP_Text secondDebugText;

    [Header("JSON")]
    public string jsonFileName = "planets.json";  // Nom du fichier JSON dans StreamingAssets

    [Header("List of Prefabs (names must match prefabId)")]
    public List<GameObject> planetPrefabs; // Dans l'Inspector, ajoute tes prefabs (earth, mars, etc.)

    [Header("UI Arrow")]
    public RectTransform arrowUI;  // Assign in Inspector (a UI arrow image)
    public Camera mainCamera;      // Reference to main AR camera

    // Dictionnaire: prefabId -> Prefab
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    // Dictionnaire: planetName -> instance de la planète (si déjà instanciée)
    private Dictionary<string, GameObject> instantiatedObjects = new Dictionary<string, GameObject>();

    // Liste des planètes chargées depuis le JSON
    private List<Planet> planetList = new List<Planet>();

    // Indique si le GPS fonctionne
    private bool gps_ok = false;

    // Position GPS courante et référence
    private GPSLoc currLoc = new GPSLoc();
    private GPSLoc referenceLoc = null; // défini quand le GPS est prêt

    // Facteur pour convertir des degrés de latitude en mètres (~111.32 km/°)
    private const double metersPerDegreeLat = 111320.0;

    IEnumerator Start()
    {
        // 1) Construire le dictionnaire [prefabId → Prefab] via p.name
        //    Attention à ce que p.name (dans Unity) corresponde au champ "prefabId" du JSON
        foreach (var p in planetPrefabs)
        {
            // On stocke en minuscules pour éviter les soucis de casse
            prefabDictionary[p.name.ToLower()] = p;
        }

        // 2) Charger les planètes depuis le JSON
        yield return StartCoroutine(LoadPlanetsFromJSON());
        if (planetList.Count == 0)
        {
            if (debugTxt) debugTxt.text = "No planets loaded from JSON.";
            yield break;
        }

        // 3) Initialiser le GPS
        if (!Input.location.isEnabledByUser)
        {
            if (debugTxt) debugTxt.text = "GPS is disabled on this device.";
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            if (debugTxt) debugTxt.text = "Timeout: GPS took too long to respond.";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            if (debugTxt) debugTxt.text = "Error: Unable to determine device location.";
            yield break;
        }
        else
        {
            gps_ok = true;
            // On enregistre la position GPS initiale comme référence
            referenceLoc = new GPSLoc(
                (float)Input.location.lastData.latitude,
                (float)Input.location.lastData.longitude
            );

            if (debugTxt)
                debugTxt.text = $"GPS active. Reference set to: {referenceLoc.lat:F6}, {referenceLoc.lon:F6}";
            Debug.Log("GPS active. Reference set to: " + referenceLoc.lat + ", " + referenceLoc.lon);
        }
    }

    void Update()
    {
        if (!gps_ok || referenceLoc == null) return;

        if (Input.location.status != LocationServiceStatus.Running)
        {
            if (debugTxt) debugTxt.text = "GPS is disabled or not running."; //TODO ajouter un élément qui indique à l'utilisateur que la permission pour la caméra n'est pas permise
            return;
        }

        // Mise à jour de la position courante
        currLoc.lat = Input.location.lastData.latitude;
        currLoc.lon = Input.location.lastData.longitude;

        // Affichage
        string info = $"Current Position:\nLat: {currLoc.lat:F6}\nLon: {currLoc.lon:F6}\nRef: {referenceLoc.lat:F6},{referenceLoc.lon:F6}\n";

        // Parcours de toutes les planètes
        foreach (var planet in planetList)
        {
            // Calcul de la distance (pour debug)
            double distMeters = DistanceInMeters(currLoc.lat, currLoc.lon, planet.gpsLatitude, planet.gpsLongitude);
            info += $"\n- {planet.planetName}: {FormatDistance(distMeters)} away";

            // Instancier si pas déjà
            if (!instantiatedObjects.ContainsKey(planet.planetName))
            {
                // Chercher le prefab correspondant
                string idLower = planet.prefabId.ToLower();
                GameObject foundPrefab = null;
                if (prefabDictionary.ContainsKey(idLower))
                {
                    foundPrefab = prefabDictionary[idLower];
                }
                else
                {
                    info += $"\nNo prefab found for '{planet.prefabId}'";
                    continue;
                }

                // Calculer la position AR
                Vector3 pos = ConvertGPSToARPosition(referenceLoc, planet.gpsLatitude, planet.gpsLongitude);
                GameObject newObj = Instantiate(foundPrefab, pos, Quaternion.identity);
                newObj.name = planet.planetName;

                instantiatedObjects[planet.planetName] = newObj;
                info += $"\nSpawned {planet.planetName} at {pos}";
            }
            else
            {
                // Mise à jour de la position
                Vector3 newPos = ConvertGPSToARPosition(referenceLoc, planet.gpsLatitude, planet.gpsLongitude);
                instantiatedObjects[planet.planetName].transform.position = newPos;
            }
        }
        
        // --- Find the closest planet ---
        GameObject closestPlanet = null;
        double closestDist = double.MaxValue;
        secondDebugText.text = "";
        foreach (var kvp in instantiatedObjects)
        {
            GameObject planetObj = kvp.Value;
            double dist = Vector3.Distance(planetObj.transform.position, Camera.main.transform.position);
            secondDebugText.text += "\n Planet: " + planetObj.name + " " + dist;
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPlanet = planetObj;
                
            }
        }
        secondDebugText.text += "\n Closest Planet: " + closestPlanet.name + " " + closestDist.ToString();

        // --- Update Arrow Rotation ---
        if (closestPlanet != null && arrowUI != null)
        {
            arrowUI.gameObject.SetActive(true);
            
            Vector3 toPlanet = closestPlanet.transform.position - Camera.main.transform.position;
            Vector3 camForward = Camera.main.transform.forward;

            // Project direction onto horizontal plane
            Vector3 flatToPlanet = Vector3.ProjectOnPlane(toPlanet, Vector3.up);
            Vector3 flatForward = Vector3.ProjectOnPlane(camForward, Vector3.up);

            float angle = Vector3.SignedAngle(flatForward, flatToPlanet, Vector3.up);

            arrowUI.localEulerAngles = new Vector3(0, 0, -angle);  // Negative because UI rotates opposite
            
            /*
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Vector3 planetScreenPos = mainCamera.WorldToScreenPoint(closestPlanet.transform.position);

            Vector2 direction = (planetScreenPos - screenCenter).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            arrowUI.rotation = Quaternion.Euler(0, 0, angle - 90);  // Subtract 90 if your arrow points up
            secondDebugText.text += "\n Angle: "+angle;
            */

        } else
        {
            arrowUI.gameObject.SetActive(false);
            secondDebugText.text += "\n Closest Planet is null";
        }


        if (debugTxt) debugTxt.text = info;
    }

    // Lecture du JSON (Planets)
    private IEnumerator LoadPlanetsFromJSON()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading JSON: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            // Désérialisation
            PlanetDataWrapper data = JsonUtility.FromJson<PlanetDataWrapper>(json);
            if (data != null && data.planets != null)
            {
                planetList = data.planets;
                Debug.Log($"Loaded {planetList.Count} planets from JSON");
            }
        }
    }

    // Conversion GPS -> AR
    private Vector3 ConvertGPSToARPosition(GPSLoc reference, double targetLat, double targetLon)
    {
        double latDiff = targetLat - reference.lat;
        double lonDiff = targetLon - reference.lon;

        float offsetZ = (float)(latDiff * metersPerDegreeLat);
        float offsetX = (float)(lonDiff * metersPerDegreeLat * Math.Cos(reference.lat * Math.PI / 180.0));

        return new Vector3(offsetX, 0, offsetZ);
    }

    // Calcul distance en mètres (pour debug)
    private double DistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        double distKm = Distance(lat1, lon1, lat2, lon2, 'K');
        return distKm * 1000.0;
    }

    private string FormatDistance(double distMeters)
    {
        if (distMeters < 1000) return $"{distMeters:F1} m";
        else return $"{distMeters / 1000.0:F2} km";
    }

    // Méthode Distance standard
    private double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
    {
        if ((lat1 == lat2) && (lon1 == lon2)) return 0;

        double theta = lon1 - lon2;
        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) +
                      Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
        dist = Math.Acos(dist);
        dist = Rad2Deg(dist);
        dist = dist * 60 * 1.1515; // miles
        if (unit == 'K') dist *= 1.609344;
        else if (unit == 'N') dist *= 0.8684;
        return dist;
    }

    private double Deg2Rad(double deg) => deg * Math.PI / 180.0;
    private double Rad2Deg(double rad) => rad / Math.PI * 180.0;
}

// Classe pour la structure du JSON
[System.Serializable]
public class PlanetDataWrapper
{
    public List<Planet> planets;
}

// Classe correspondant à chaque planète dans le JSON
[System.Serializable]
public class Planet
{
    public string planetName;
    public double gpsLatitude;
    public double gpsLongitude;
    public string prefabId;
}

// Stockage simple d'une position lat/lon
[System.Serializable]
public class GPSLoc
{
    public double lat;
    public double lon;
    public GPSLoc() { lat = 0; lon = 0; }
    public GPSLoc(double lat, double lon) { this.lat = lat; this.lon = lon; }
}

// Classe pour relier un prefab à un lat/lon
[System.Serializable]
public class GPSObject
{
    public string objectName;
    public GPSLoc position;
    public GameObject prefab;

    public GPSObject(string name, double lat, double lon, GameObject prefab)
    {
        this.objectName = name;
        this.position = new GPSLoc(lat, lon);
        this.prefab = prefab;
    }
}
