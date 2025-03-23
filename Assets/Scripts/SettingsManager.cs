using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject menuFR;
    public GameObject menuEN;
    public GameObject settingsPanel;

    private GameObject previousPanel; // Variable pour stocker le panneau pr�c�dent

    // Fonction pour ouvrir les param�tres et enregistrer le menu pr�c�dent
    public void OpenSettings()
    {
        if (menuFR.activeSelf)
        {
            previousPanel = menuFR;
        }
        else if (menuEN.activeSelf)
        {
            previousPanel = menuEN;
        }

        // Activer les param�tres et d�sactiver le menu actuel
        settingsPanel.SetActive(true);
        if (previousPanel != null) previousPanel.SetActive(false);
    }

    // Fonction pour fermer les param�tres et revenir au menu pr�c�dent
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (previousPanel != null) previousPanel.SetActive(true);
    }
}
