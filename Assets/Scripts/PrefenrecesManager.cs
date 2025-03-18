using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Pour le bouton

public class PreferencesManager : MonoBehaviour
{
    public TMP_Dropdown Age_Dropdown;   // Dropdown pour l'�ge
    public TMP_Dropdown Langue_Dropdown; // Dropdown pour la langue
    public Button validerButton;        // Bouton "Valider"

    [System.Serializable]
    public class DropdownSelection
    {
        public int AGE_Index;
        public int Lang_Index;
        public GameObject panel; // Panel � afficher
    }

    public List<DropdownSelection> panelMappings; // Liste des combinaisons

    void Start()
    {
        validerButton.onClick.AddListener(OnValiderClicked); // Attacher l'�v�nement du bouton
    }

    void OnValiderClicked()
    {
        // D�sactiver tous les panels avant d'en activer un
        foreach (DropdownSelection mapping in panelMappings)
        {
            if (mapping.panel != null)
            {
                mapping.panel.SetActive(false);
            }
        }

        // R�cup�rer les valeurs des Dropdowns
        int ageIndex = Age_Dropdown.value;
        int langIndex = Langue_Dropdown.value;

        // Trouver le bon panel � activer
        foreach (DropdownSelection mapping in panelMappings)
        {
            if (mapping.AGE_Index == ageIndex && mapping.Lang_Index == langIndex)
            {
                if (mapping.panel != null)
                {
                    mapping.panel.SetActive(true);
                }
                break;
            }
        }
    }
}
