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
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) DetectTouchOrClick(Input.mousePosition);

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            DetectTouchOrClick(Input.GetTouch(0).position);
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
                SceneManager.LoadScene("PlanetTextView");
            }
            
        }
       
    }
}
