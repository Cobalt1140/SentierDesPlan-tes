using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Import pour changer de sc�ne

public class PreferencesManager : MonoBehaviour
{
    public TMP_Dropdown Age_Dropdown;
    public TMP_Dropdown Langue_Dropdown;
    public Button validerButton;

    void Start()
    {
        
        Debug.Log("PreferencesManager - Start");
        
        
        if (Age_Dropdown == null) Debug.LogError("Age_Dropdown n'est pas assign�.");
        if (Langue_Dropdown == null) Debug.LogError("Langue_Dropdown n'est pas assign�.");
        if (validerButton == null) Debug.LogError("validerButton n'est pas assign�.");

        validerButton.onClick.AddListener(OnValiderClicked);
    }

    void OnValiderClicked()
    {
        Debug.Log("Bouton 'Valider' cliqu�.");

        int ageIndex = Age_Dropdown.value;
        int langIndex = Langue_Dropdown.value;

        Debug.Log($"S�lections sauvegard�es : Age Index = {ageIndex}, Langue Index = {langIndex}");

        // Sauvegarder les pr�f�rences dans PlayerPrefs
        PlayerPrefs.SetInt("AgePreference", ageIndex);
        PlayerPrefs.SetInt("LanguePreference", langIndex);
        PlayerPrefs.Save();

        // Charger la sc�ne du menu (remplace "NomDeLaSceneMenu" par le nom de ta sc�ne)
        SceneManager.LoadScene("Menu");
    }
}
