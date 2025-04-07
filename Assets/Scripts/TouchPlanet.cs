using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TouchPlanet : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    private List<string> planetList = new List<string> { "earth", "jupiter", "mars",
        "mercury", "moon", "neptune", "pluton","saturn", "sun", "uranus", "venus" };
    private bool isAnimating = false;
    

    // Update is called once per frame
    void Update()
    {
        if (!isAnimating)
        {
            if (Input.GetMouseButtonDown(0)) DetectTouchOrClick(Input.mousePosition);

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                DetectTouchOrClick(Input.GetTouch(0).position);
        }
        
    }
    //on check le tag du objet touché, on prends son nom et on l'ajoute en tant que int dans les PlayerPrefs
    void DetectTouchOrClick(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            //debugText.text = "PLANET HIT!!:";
            //debugText.text = hit.transform.tag.ToString();



            if (planetList.Contains(hit.transform.tag))
            {
                //debugText.text = hit.transform.tag.ToString()+" HAS BEEN HIT!";
                PlayerPrefs.SetInt(hit.transform.tag, 1);
                CollectionStatic.setCurrentPlanet(hit.transform.tag);


                StartCoroutine(AnimateAndLoadScene(hit.transform));

                //SceneManager.LoadScene("PlanetTextView");

            }
            
        }
       
    }
    //fonction pour faire une animation sur la planète pour qu'elle grandisse puis qu'elle se rétrécit pendant 2 secs
    IEnumerator AnimateAndLoadScene(Transform planetTransform)
    {
        isAnimating = true;

        Vector3 originalScale = planetTransform.localScale;

        float time = 0f;
        float totalDuration = 0.60f;
        float scaleUpDuration = totalDuration * 0.25f;
        float scaleDownDuration = totalDuration - scaleUpDuration;


        // Phase 1: Scale up over 0.5 seconds
        while (time < 0.25f)
        {
            float scaleFactor = Mathf.Lerp(1f, 1.2f, time / scaleUpDuration); // Increase by 20%
            planetTransform.localScale = originalScale * scaleFactor;

            if (debugText != null)
                debugText.text = $"Scaling Up: {time:F2}s";

            time += Time.deltaTime;
            yield return null;
        }

        // Phase 2: Scale down to zero over the next 1.5 seconds
        float shrinkTime = 0f;
        
        while (shrinkTime < scaleDownDuration)
        {
            float scaleFactor = Mathf.Lerp(1.2f, 0f, shrinkTime / scaleDownDuration);
            planetTransform.localScale = originalScale * scaleFactor;

            if (debugText != null)
                debugText.text = $"Shrinking: {shrinkTime:F2}s";

            shrinkTime += Time.deltaTime;
            yield return null;
        }

        planetTransform.localScale = Vector3.zero;

        SceneManager.LoadScene("PlanetTextView");
    }
}
