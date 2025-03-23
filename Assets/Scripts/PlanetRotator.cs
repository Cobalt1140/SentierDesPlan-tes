using UnityEngine;

public class PlanetRotator : MonoBehaviour
{
    public float rotationSpeed = 10f; // Ajuste la vitesse de rotation dans l'Inspector

    void Update()
    {
        // Fait tourner la plan�te sur son axe Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
