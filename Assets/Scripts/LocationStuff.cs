using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI; // Pour accéder à Dropdown

public class LocationStuff : MonoBehaviour
{
    [Header("UI Debug")]
    public TMP_Text debugTxt;
    public TMP_Text secondDebugText;

    [Header("UI Dropdown")]
    public TMP_Dropdown planetDropdown; // Référence au dropdown à assigner dans l'inspector

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

    // Planète actuellement sélectionnée dans le dropdown
    private string selectedPlanetName = "";

    // Facteur pour convertir des degrés de latitude en mètres (~111.32 km/°)
    private const double metersPerDegreeLat = 111320.0;

    // Distances astronomiques réelles (en millions de km)
    private Dictionary<string, double> astronomicalDistances = new Dictionary<string, double>()
    {
        { "sun", 0 },           // Le Soleil est le centre de référence
        { "mercury", 57.9 },    // Distance moyenne Soleil-Mercure en millions de km
        { "venus", 108.2 },     // Distance moyenne Soleil-Vénus
        { "earth", 149.6 },     // Distance moyenne Soleil-Terre
        { "moon", 0.384 },      // Distance moyenne Terre-Lune en millions de km (pas du Soleil)
        { "mars", 227.9 },      // Distance moyenne Soleil-Mars
        { "jupiter", 778.5 },   // Distance moyenne Soleil-Jupiter
        { "saturn", 1432.0 },   // Distance moyenne Soleil-Saturne
        { "neptune", 4495.0 },  // Distance moyenne Soleil-Neptune
        { "pluton", 5906.0 }    // Distance moyenne Soleil-Pluton
    };

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

        // Initialiser le dropdown avec les planètes
        InitializeDropdown();

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

            Debug.Log("GPS active. Reference set to: " + referenceLoc.lat + ", " + referenceLoc.lon);
        }
    }

    // Initialiser le dropdown avec les noms des planètes
    private void InitializeDropdown()
    {
        if (planetDropdown == null) return;

        // Vider le dropdown
        planetDropdown.ClearOptions();

        // Créer la liste des options
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        // Ajouter une option "Sélectionner une planète" en premier
        options.Add(new TMP_Dropdown.OptionData("Sélectionner une planète"));

        // Ajouter chaque planète
        foreach (var planet in planetList)
        {
            // Capitaliser la première lettre pour un meilleur affichage
            string displayName = char.ToUpper(planet.planetName[0]) + planet.planetName.Substring(1);
            options.Add(new TMP_Dropdown.OptionData(displayName));
        }

        // Mettre à jour le dropdown
        planetDropdown.AddOptions(options);

        // Ajouter un listener pour les changements de sélection
        planetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Sélectionner la première option par défaut
        planetDropdown.value = 0;
        selectedPlanetName = "";

        // Message initial dans le debugTxt
        if (debugTxt) debugTxt.text = "Sélectionnez une planète pour voir les informations de distance.";
    }

    // Méthode appelée quand la sélection du dropdown change
    public void OnDropdownValueChanged(int index)
    {
        if (index == 0)
        {
            // "Sélectionner une planète" sélectionné
            selectedPlanetName = "";
            if (debugTxt) debugTxt.text = "Sélectionnez une planète pour voir les informations de distance.";
        }
        else if (index > 0 && index <= planetList.Count)
        {
            // Une planète spécifique sélectionnée
            selectedPlanetName = planetList[index - 1].planetName;

            // Mettre à jour immédiatement l'affichage des distances
            UpdateDistanceDisplay();
        }
    }

    void Update()
    {
        if (!gps_ok || referenceLoc == null) return;

        if (Input.location.status != LocationServiceStatus.Running)
        {
            if (debugTxt) debugTxt.text = "GPS is disabled or not running.";
            return;
        }

        // Mise à jour de la position courante
        currLoc.lat = Input.location.lastData.latitude;
        currLoc.lon = Input.location.lastData.longitude;

        // Si une planète est sélectionnée, mettre à jour l'affichage de la distance
        if (!string.IsNullOrEmpty(selectedPlanetName))
        {
            UpdateDistanceDisplay();
        }

        // Parcours de toutes les planètes pour faire apparaître les objets 3D
        foreach (var planet in planetList)
        {
            // Instancier si pas déjà fait
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
                    continue;
                }

                // Calculer la position AR
                Vector3 pos = ConvertGPSToARPosition(referenceLoc, planet.gpsLatitude, planet.gpsLongitude);
                GameObject newObj = Instantiate(foundPrefab, pos, Quaternion.identity);
                newObj.name = planet.planetName;

                instantiatedObjects[planet.planetName] = newObj;
            }
            else
            {
                // Mise à jour de la position
                Vector3 newPos = ConvertGPSToARPosition(referenceLoc, planet.gpsLatitude, planet.gpsLongitude);
                instantiatedObjects[planet.planetName].transform.position = newPos;
            }
        }

        // --- Update Arrow Rotation ---
        if (!string.IsNullOrEmpty(selectedPlanetName) &&
            instantiatedObjects.ContainsKey(selectedPlanetName) &&
            arrowUI != null)
        {
            arrowUI.gameObject.SetActive(true);

            Vector3 toPlanet = instantiatedObjects[selectedPlanetName].transform.position - Camera.main.transform.position;
            Vector3 camForward = Camera.main.transform.forward;

            // Project direction onto horizontal plane
            Vector3 flatToPlanet = Vector3.ProjectOnPlane(toPlanet, Vector3.up);
            Vector3 flatForward = Vector3.ProjectOnPlane(camForward, Vector3.up);

            float angle = Vector3.SignedAngle(flatForward, flatToPlanet, Vector3.up);

            arrowUI.localEulerAngles = new Vector3(0, 0, -angle);  // Negative because UI rotates opposite
        }
        else
        {
            arrowUI.gameObject.SetActive(false);
        }
    }

    // Mettre à jour l'affichage des distances pour la planète sélectionnée
    private void UpdateDistanceDisplay()
    {
        if (string.IsNullOrEmpty(selectedPlanetName) || !debugTxt) return;

        // Chercher la planète sélectionnée
        Planet selectedPlanet = null;
        foreach (var planet in planetList)
        {
            if (planet.planetName == selectedPlanetName)
            {
                selectedPlanet = planet;
                break;
            }
        }

        if (selectedPlanet == null) return;

        // Calculer la distance réelle sur terre
        double distMeters = DistanceInMeters(currLoc.lat, currLoc.lon,
                                           selectedPlanet.gpsLatitude, selectedPlanet.gpsLongitude);

        // Obtenir la distance astronomique
        string astronomicalDistanceText = "N/A";
        if (astronomicalDistances.ContainsKey(selectedPlanetName))
        {
            double distMillion = astronomicalDistances[selectedPlanetName];
            if (distMillion < 1)
            {
                // Pour la Lune, afficher en milliers de km
                astronomicalDistanceText = $"{distMillion * 1000:F0} mille km";
            }
            else
            {
                // Pour les autres planètes, afficher en millions de km
                astronomicalDistanceText = $"{distMillion:F1} millions km";
            }
        }

        // Construire le texte d'information
        string info = $"🪐 {char.ToUpper(selectedPlanetName[0]) + selectedPlanetName.Substring(1)} 🪐\n\n";
        info += $"Distance actuelle: {FormatDistance(distMeters)}\n\n";
        info += $"Distance dans l'espace: {astronomicalDistanceText}";

        // Mettre à jour le texte d'affichage
        debugTxt.text = info;
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

// Les classes restent les mêmes
[System.Serializable]
public class PlanetDataWrapper
{
    public List<Planet> planets;
}

[System.Serializable]
public class Planet
{
    public string planetName;
    public double gpsLatitude;
    public double gpsLongitude;
    public string prefabId;
}

[System.Serializable]
public class GPSLoc
{
    public double lat;
    public double lon;
    public GPSLoc() { lat = 0; lon = 0; }
    public GPSLoc(double lat, double lon) { this.lat = lat; this.lon = lon; }
}

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