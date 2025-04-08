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
    public GameObject closeButton;
    public GameObject resetProgressBtn;
    public TextMeshProUGUI confirmationText;

    private TextMeshProUGUI resetProgressBtnText;
    private Dictionary<string, string[]> translations = new Dictionary<string, string[]>()
    {
        { "French", new string[] { "Paramètres", "Format: ", "Langue: ", "Enfant", "Adulte","Réinitialiser toute la progression" ,"Voulez-vous remmettre tous vos progrès à 0? (Vous pourrez de nouveau avoir une place dans le classement)" } },
        { "English", new string[] { "Settings", "Format: ", "Language: ", "Child", "Adult","Reset All Progress" ,"Are you sure you want to reset all progress? (You will be able to get a spot in the ranking again)" } }
    };

    private void Start()
    {
        resetProgressBtnText = resetProgressBtn.GetComponentInChildren<TextMeshProUGUI>();

        // Charger les préférences enregistrées
        int langIndex = PlayerPrefs.GetInt("LanguePreference", -1);
        int ageIndex = PlayerPrefs.GetInt("AgePreference", -1);
        string language;
        if (langIndex == -1 || ageIndex == -1) //paramètres pour la première fois qu'on joue
        {
            closeButton.SetActive(false);
            resetProgressBtn.SetActive(false);
            langDropdown.value = 0; //âge par défaut
            ageDropdown.value = 0;
            language = "French"; //language par défaut
            
            
        } else
        {
            // Afficher les valeurs dans les dropdowns
            langDropdown.value = langIndex;
            ageDropdown.value = ageIndex;
            language = (langIndex == 0) ? "French" : "English";
            
            
        }
        
        // Mettre à jour l'interface
        UpdateUI(langIndex);

        // Ajouter un listener au bouton pour sauvegarder uniquement lorsqu'on appuie dessus
        if (validerButton != null)
        {
            validerButton.onClick.AddListener(SavePreferences);
        }
        else
        {
            Debug.LogError("Le bouton 'Valider' n'est pas assigné dans l'inspecteur !");
        }
    }

    private void SavePreferences()
    {
        int selectedLang = langDropdown.value;
        int selectedAge = ageDropdown.value;

        // Sauvegarder les préférences dans PlayerPrefs
        PlayerPrefs.SetInt("LanguePreference", selectedLang);
        PlayerPrefs.SetInt("AgePreference", selectedAge);
        
        PlayerPrefs.Save();
    }

    public void UpdateUI(int langIndex)
    {
        string language = (langIndex == 0) ? "French" : "English";

        if (translations.ContainsKey(language))
        {
            LOGOTEXT.text = translations[language][0];
            ageText.text = translations[language][1];
            langText.text = translations[language][2];
            ageDropdown.options[0].text = translations[language][3];
            ageDropdown.options[1].text = translations[language][4];
            confirmationText.text = translations[language][6];
            resetProgressBtnText.text = translations[language][5];
            if (ageDropdown.value == 0)
            {
                ageDropdown.GetComponentInChildren<TextMeshProUGUI>().text = translations[language][3];
            }
            else
            {
                ageDropdown.GetComponentInChildren<TextMeshProUGUI>().text = translations[language][4];
            }
        }
    }



    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt("tutorial", -1);
        PlayerPrefs.SetInt("earth", -1);
        PlayerPrefs.SetInt("jupiter", -1);
        PlayerPrefs.SetInt("mars", -1);
        PlayerPrefs.SetInt("mercury", -1);
        PlayerPrefs.SetInt("neptune", -1);
        PlayerPrefs.SetInt("moon", -1);
        PlayerPrefs.SetInt("pluton", -1);
        PlayerPrefs.SetInt("saturn", -1);
        PlayerPrefs.SetInt("sun", -1);
        PlayerPrefs.SetInt("uranus", -1);
        PlayerPrefs.SetInt("venus", -1);
    }
}
