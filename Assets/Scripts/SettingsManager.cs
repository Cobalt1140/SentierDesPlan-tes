using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Import pour le bouton

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown langDropdown;
    public TMP_Dropdown ageDropdown;
    public TextMeshProUGUI LOGOTEXT;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI langText;
    public Button validerButton; // Bouton pour appliquer les changements

    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Param�tre", "Format: ", "Langue: " } },
        { "English", new string[] { "Settings", "Format: ", "Language: "} }
    };

    private void Start()
    {
        Debug.Log("SettingsManager - Chargement des pr�f�rences");

        // Charger les pr�f�rences enregistr�es
        int langIndex = PlayerPrefs.GetInt("LanguePreference", 0);
        int ageIndex = PlayerPrefs.GetInt("AgePreference", 0);

        Debug.Log($"Pr�f�rences charg�es : Langue = {langIndex}, �ge = {ageIndex}");

        // Afficher les valeurs dans les dropdowns
        langDropdown.value = langIndex;
        ageDropdown.value = ageIndex;

        // Mettre � jour l'interface
        UpdateUI(langIndex);

        // Ajouter un listener au bouton pour sauvegarder uniquement lorsqu'on appuie dessus
        if (validerButton != null)
        {
            validerButton.onClick.AddListener(SavePreferences);
        }
        else
        {
            Debug.LogError("Le bouton 'Valider' n'est pas assign� dans l'inspecteur !");
        }
    }

    private void SavePreferences()
    {
        int selectedLang = langDropdown.value;
        int selectedAge = ageDropdown.value;

        // Sauvegarder les pr�f�rences dans PlayerPrefs
        PlayerPrefs.SetInt("LanguePreference", selectedLang);
        PlayerPrefs.SetInt("AgePreference", selectedAge);
        PlayerPrefs.Save();

        Debug.Log($"Pr�f�rences sauvegard�es : Langue = {selectedLang}, �ge = {selectedAge}");

        // Mettre � jour l'affichage en fonction de la langue s�lectionn�e
        UpdateUI(selectedLang);
    }

    private void UpdateUI(int langIndex)
    {
        string language = (langIndex == 0) ? "French" : "English";

        if (translations.ContainsKey(language))
        {
            LOGOTEXT.text = translations[language][0];
            ageText.text = translations[language][1];
            langText.text = translations[language][2];
        }
    }
}
