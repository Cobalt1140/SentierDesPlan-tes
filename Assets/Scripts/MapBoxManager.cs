using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity;

public class MapboxMapManager : MonoBehaviour
{
    [Header("Mapbox Settings")]
    public string mapboxToken = "pk.eyJ1IjoiYXl0ZWMxOTQ1IiwiYSI6ImNtN3g5emVmaTAzamIyaXNmYm9uamcyc3kifQ.k8G9kIvUoHuj73CfsBjmdQ";
    public Vector2d startLocation = new Vector2d(50.8153, 4.3816);

    [Header("Zoom Settings")]
    [Range(10f, 22f)]
    public float zoom = 16f;
    public float zoomSpeed = 0.3f;
    public float minZoom = 15f;
    public float maxZoom = 18f;

    [Header("Pan Settings")]
    public float panSpeed = 0.0005f;

    [Header("Map Content")]
    public GameObject markerPrefab;
    public GameObject locationMarkerPrefab;

    private AbstractMap map;
    private GameObject locationMarkerInstance;

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
    };

    private List<GameObject> spawnedMarkers = new List<GameObject>();

    IEnumerator Start()
    {
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
        map.Initialize(startLocation, (int)zoom);

        yield return new WaitForSeconds(2f);
        PlaceMarkers();

        // GPS
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

    void UpdateMarkers()
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

    void UpdateUserLocation()
    {
        if (locationMarkerInstance != null && Input.location.status == LocationServiceStatus.Running)
        {
            double lat = Input.location.lastData.latitude;
            double lon = Input.location.lastData.longitude;
            Vector2d userLatLon = new Vector2d(lat, lon);
            Vector3 worldPos = map.GeoToWorldPosition(userLatLon, true);

            locationMarkerInstance.transform.position = worldPos;
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

            zoom += diff * 0.01f;
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
