using UnityEngine;
using System.Collections.Generic; // Pour utiliser List<>
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using Mapbox.Unity;

public class MapboxMapManager : MonoBehaviour
{
    [Header("Mapbox Settings")]
    public string mapboxToken; // 📌 Token modifiable depuis l'Inspector
    public Vector2d startLocation = new Vector2d(50.8153, 4.3816); // Latitude/Longitude du campus
    public float zoom = 16f; // Niveau de zoom

    public GameObject markerPrefab; // Prefab du marqueur
    private AbstractMap map;

    [System.Serializable]
    public class MapPoint
    {
        public string name;
        public double latitude;
        public double longitude;
    }

    public List<MapPoint> points = new List<MapPoint>(); // Liste des points

    private List<GameObject> spawnedMarkers = new List<GameObject>();

    void Start()
    {
        if (string.IsNullOrEmpty(mapboxToken))
        {
            Debug.LogError(" Mapbox Access Token est vide ! Ajoute-le dans l'Inspector.");
            return;
        }

        InitializeMap();
        PlaceMarkers();
    }

    void InitializeMap()
    {
        GameObject mapObject = new GameObject("MapboxMap");
        map = mapObject.AddComponent<AbstractMap>();

        // Définir dynamiquement l'access token
        MapboxAccess.Instance.Configuration.AccessToken = mapboxToken;

        // Initialiser la carte
        map.Initialize(startLocation, (int)zoom);
    }

    void PlaceMarkers()
    {
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
