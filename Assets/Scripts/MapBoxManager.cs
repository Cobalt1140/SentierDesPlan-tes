using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity;

public class MapboxMapManager : MonoBehaviour
{
    [Header("Mapbox Settings")]
    public string mapboxToken = "pk.eyJ1IjoiYXl0ZWMxOTQ1IiwiYSI6ImNtN3g5emVmaTAzamIyaXNmYm9uamcyc3kifQ.k8G9kIvUoHuj73CfsBjmdQ"; // Token par défaut
    public Vector2d startLocation = new Vector2d(50.8153, 4.3816); // Latitude/Longitude par défaut
    public float zoom = 16f; // Niveau de zoom par défaut

    public GameObject markerPrefab; // Prefab du marqueur
    private AbstractMap map;

    [System.Serializable]
    public class MapPoint
    {
        public string name;
        public double latitude;
        public double longitude;
    }

    public List<MapPoint> points = new List<MapPoint>
    {
        new MapPoint { name = "Point1", latitude = 50.8153, longitude = 4.3816 },
        new MapPoint { name = "Point2", latitude = 50.8160, longitude = 4.3820 }
    }; // Points de test par défaut

    private List<GameObject> spawnedMarkers = new List<GameObject>();

    void Start()
    {
        Debug.Log("Application démarrée !");

        if (string.IsNullOrEmpty(mapboxToken))
        {
            Debug.LogError("Mapbox Access Token est vide ! Ajoute-le dans l'Inspector.");
            return;
        }

        StartCoroutine(InitializeMap());
    }

    IEnumerator InitializeMap()
    {
        Debug.Log("Initialisation de la carte...");

        GameObject mapObject = new GameObject("MapboxMap");
        map = mapObject.AddComponent<AbstractMap>();

        if (map == null)
        {
            Debug.LogError("ERREUR : L'objet Map n'a pas pu être créé !");
            yield break;
        }

        yield return null;

        if (MapboxAccess.Instance == null)
        {
            Debug.LogError("ERREUR : MapboxAccess.Instance est NULL !");
            yield break;
        }

        MapboxAccess.Instance.Configuration.AccessToken = mapboxToken;

        map.Initialize(startLocation, (int)zoom);

        yield return new WaitForSeconds(2f);

        PlaceMarkers();
    }

    void PlaceMarkers()
    {
        if (map == null)
        {
            Debug.LogError("ERREUR : `map` est NULL, impossible de placer les marqueurs.");
            return;
        }

        foreach (var point in points)
        {
            Vector2d location = new Vector2d(point.latitude, point.longitude);
            Vector3 worldPosition = map.GeoToWorldPosition(location, true);

            GameObject marker = Instantiate(markerPrefab, worldPosition, Quaternion.identity);
            marker.name = point.name;
            marker.transform.SetParent(map.transform, true);
            spawnedMarkers.Add(marker);
        }
    }

    void Update()
    {
        if (map == null)
            return;

        foreach (var marker in spawnedMarkers)
        {
            MapPoint pointData = points.Find(p => p.name == marker.name);
            if (pointData != null)
            {
                Vector2d location = new Vector2d(pointData.latitude, pointData.longitude);
                marker.transform.position = map.GeoToWorldPosition(location, true);
            }
        }
    }
}