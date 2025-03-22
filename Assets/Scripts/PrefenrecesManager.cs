using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreferencesManager : MonoBehaviour
{
    public TMP_Dropdown Age_Dropdown;
    public TMP_Dropdown Langue_Dropdown;
    public Button validerButton;

    [System.Serializable]
    public class DropdownSelection
    {
        public int AGE_Index;
        public int Lang_Index;
        public GameObject panel;
    }

    public List<DropdownSelection> panelMappings;

    void Start()
    {
        Debug.Log("PreferencesManager - Start");

        if (Age_Dropdown == null) Debug.LogError("Age_Dropdown n'est pas assigné.");
        if (Langue_Dropdown == null) Debug.LogError("Langue_Dropdown n'est pas assigné.");
        if (validerButton == null) Debug.LogError("validerButton n'est pas assigné.");
        if (panelMappings == null || panelMappings.Count == 0) Debug.LogWarning("panelMappings est vide ou non assigné.");

        validerButton.onClick.AddListener(OnValiderClicked);
    }

    void OnValiderClicked()
    {
        Debug.Log("Bouton 'Valider' cliqué.");

        // Désactiver tous les panels
        foreach (DropdownSelection mapping in panelMappings)
        {
            if (mapping.panel != null)
            {
                mapping.panel.SetActive(false);
            }
        }

        int ageIndex = Age_Dropdown.value;
        int langIndex = Langue_Dropdown.value;

        Debug.Log($"Sélections : Age Index = {ageIndex}, Langue Index = {langIndex}");

        bool panelTrouve = false;

        foreach (DropdownSelection mapping in panelMappings)
        {
            if (mapping.AGE_Index == ageIndex && mapping.Lang_Index == langIndex)
            {
                if (mapping.panel != null)
                {
                    mapping.panel.SetActive(true);
                    Debug.Log("Panel activé : " + mapping.panel.name);
                }
                else
                {
                    Debug.LogWarning("Le panel correspondant est null.");
                }

                panelTrouve = true;
                break;
            }
        }

        if (!panelTrouve)
        {
            Debug.LogWarning("Aucun panel correspondant trouvé pour les sélections actuelles.");
        }
    }
}
