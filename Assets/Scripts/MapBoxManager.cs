﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity;
using UnityEngine.Networking;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class MapboxMapManager : MonoBehaviour
{
    [Header("Mapbox Settings")]
    public string mapboxToken = "pk.eyJ1IjoiYXl0ZWMxOTQ1IiwiYSI6ImNtN3g5emVmaTAzamIyaXNmYm9uamcyc3kifQ.k8G9kIvUoHuj73CfsBjmdQ";
    public Vector2d startLocation = new Vector2d(50.838420201988555, 4.322096405101632);

    [Header("Zoom Settings")]
    [Range(10f, 22f)]
    public float zoom = 16f;
    public float zoomSpeed = 0.3f;
    public float minZoom = 15f;
    public float maxZoom = 18f;

    [Header("Pan Settings")]
    public float panSpeed = 0.000005f;

    [Header("Map Content")]
    public GameObject markerPrefab;
    public GameObject locationMarkerPrefab;

    [Header("Notification")]
    public TMPro.TextMeshProUGUI notificationText;

    private AbstractMap map;
    private GameObject locationMarkerInstance;
    private HashSet<string> notifiedPlanets = new HashSet<string>();

    [System.Serializable]
    public class MapPoint
    {
        public string name;
        public double latitude;
        public double longitude;
    }

    [System.Serializable]
    private class PlanetData
    {
        public string planetName;
        public double gpsLatitude;
        public double gpsLongitude;
        public string prefabId;
    }

    [System.Serializable]
    private class PlanetDataList
    {
        public List<PlanetData> planets;
    }

    public List<MapPoint> points = new List<MapPoint>();
    private List<GameObject> spawnedMarkers = new List<GameObject>();

    IEnumerator Start()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(1f);
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.LogError("Permission localisation refusée par l'utilisateur.");
                yield break;
            }
        }
#endif

        if (string.IsNullOrEmpty(mapboxToken))
        {
            Debug.LogError("Mapbox Access Token est vide.");
            yield break;
        }

        if (markerPrefab == null)
            Debug.LogWarning("markerPrefab n'est pas assigné.");
        if (locationMarkerPrefab == null)
            Debug.LogWarning("locationMarkerPrefab n'est pas assigné.");

        GameObject mapObject = new GameObject("MapboxMap");
        map = mapObject.AddComponent<AbstractMap>();
        MapboxAccess.Instance.Configuration.AccessToken = mapboxToken;

        map.OnInitialized += () =>
        {
            Debug.Log(" Carte initialisée, chargement des points JSON.");
            StartCoroutine(LoadJsonCoroutine());
        };

        map.Initialize(startLocation, (int)zoom);

        StartCoroutine(InitLocation());
    }

    IEnumerator InitLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Localisation désactivée !");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("Impossible de récupérer la position.");
            yield break;
        }

        if (locationMarkerPrefab != null)
        {
            locationMarkerInstance = Instantiate(locationMarkerPrefab, Vector3.zero, Quaternion.identity);
            locationMarkerInstance.name = "UserLocationMarker";
        }
    }

    IEnumerator LoadJsonCoroutine()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "planets.json");

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erreur de lecture JSON sur Android : " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
#else
        if (!File.Exists(path))
        {
            Debug.LogError(" Fichier JSON non trouvé : " + path);
            yield break;
        }

        string json = File.ReadAllText(path);
#endif

        PlanetDataList planetDataList = JsonUtility.FromJson<PlanetDataList>(json);

        if (planetDataList == null || planetDataList.planets == null)
        {
            Debug.LogError(" Échec du parsing JSON.");
            yield break;
        }

        points = new List<MapPoint>();

        foreach (var planet in planetDataList.planets)
        {
            points.Add(new MapPoint
            {
                name = planet.planetName,
                latitude = planet.gpsLatitude,
                longitude = planet.gpsLongitude
            });
        }

        Debug.Log($" {points.Count} planètes chargées depuis le fichier JSON.");
        PlaceMarkers();
    }

    void Update()
    {
        if (map == null) return;

        UpdateMarkers();
        HandleTouchZoom();
        HandleTouchPan();
        UpdateUserLocation();
    }

    void PlaceMarkers()
    {
        Debug.Log($" Nombre de points à placer : {points.Count}");

        foreach (var point in points)
        {
            Vector2d location = new Vector2d(point.latitude, point.longitude);
            Vector3 worldPosition = map.GeoToWorldPosition(location, true);

            Debug.Log($" Placer {point.name} → {worldPosition}");

            GameObject marker = Instantiate(markerPrefab, worldPosition, Quaternion.identity);
            marker.name = point.name;
            marker.transform.SetParent(map.transform, true);
            spawnedMarkers.Add(marker);
        }
    }

    void UpdateMarkers()
    {
        float minScale = 0.1f; // encore plus petit au zoom max
        float maxScale = 1.2f;
        float t = (zoom - minZoom) / (maxZoom - minZoom);
        float dynamicScale = Mathf.Lerp(maxScale, minScale, t);

        foreach (var marker in spawnedMarkers)
        {
            MapPoint pointData = points.Find(p => p.name == marker.name);
            if (pointData != null)
            {
                Vector2d location = new Vector2d(pointData.latitude, pointData.longitude);
                Vector3 worldPos = map.GeoToWorldPosition(location, true);

                // Décalage vertical pour ne pas masquer l'utilisateur
                worldPos += Vector3.up * 0.5f;

                marker.transform.position = worldPos;
                marker.transform.localScale = Vector3.one * dynamicScale;

                // Pour UI markers (optionnel)
                RectTransform rt = marker.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.sizeDelta = Vector2.one * (dynamicScale * 100f);
                }
            }
        }
    }



    void UpdateUserLocation()
    {
        if (locationMarkerInstance != null && Input.location.status == LocationServiceStatus.Running)
        {
            double lat = Input.location.lastData.latitude;
            double lon = Input.location.lastData.longitude;
            Vector2d userLatLon = new Vector2d(lat, lon);
            Vector3 worldPos = map.GeoToWorldPosition(userLatLon, true);

            // Déplacement du marqueur utilisateur légèrement plus haut que les planètes
            locationMarkerInstance.transform.position = worldPos + Vector3.up * 1f;

            foreach (var planet in points)
            {
                double dist = GetDistanceInMeters(lat, lon, planet.latitude, planet.longitude);
                if (dist <= 10 && !notifiedPlanets.Contains(planet.name))
                {
                    notifiedPlanets.Add(planet.name);
                    StartCoroutine(ShowNotification($" Tu es proche de {planet.name} !"));
                }
            }
        }
    }


    IEnumerator ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            yield return new WaitForSeconds(4f);
            notificationText.gameObject.SetActive(false);
        }
    }

    double GetDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371000;
        double dLat = (lat2 - lat1) * (Mathf.PI / 180);
        double dLon = (lon2 - lon1) * (Mathf.PI / 180);
        double a = Mathf.Sin((float)dLat / 2) * Mathf.Sin((float)dLat / 2) +
                   Mathf.Cos((float)(lat1 * Mathf.PI / 180)) * Mathf.Cos((float)(lat2 * Mathf.PI / 180)) *
                   Mathf.Sin((float)dLon / 2) * Mathf.Sin((float)dLon / 2);
        double c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt((float)(1 - a)));
        return R * c;
    }

    public void CenterMapOnUser()
    {
        if (locationMarkerInstance != null && Input.location.status == LocationServiceStatus.Running)
        {
            double lat = Input.location.lastData.latitude;
            double lon = Input.location.lastData.longitude;
            Vector2d userLatLon = new Vector2d(lat, lon);
            map.Initialize(userLatLon, (int)zoom);
        }
    }

    void HandleTouchZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevT0 = t0.position - t0.deltaPosition;
            Vector2 prevT1 = t1.position - t1.deltaPosition;

            float prevMag = (prevT0 - prevT1).magnitude;
            float currMag = (t0.position - t1.position).magnitude;
            float diff = currMag - prevMag;

            zoom += diff * 0.002f;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            map.UpdateMap(map.CenterLatitudeLongitude, zoom);
        }
    }

    void HandleTouchPan()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                float zoomFactor = Mathf.Pow(2, zoom - minZoom);

                double latOffset = -delta.y * panSpeed / zoomFactor;
                double lonOffset = -delta.x * panSpeed / zoomFactor;

                Vector2d newCenter = new Vector2d(
                    map.CenterLatitudeLongitude.x + latOffset,
                    map.CenterLatitudeLongitude.y + lonOffset
                );

                map.UpdateMap(newCenter, zoom);
            }
        }
    }
}
