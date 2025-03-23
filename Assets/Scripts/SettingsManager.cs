using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject menuFR;
    public GameObject menuEN;
    public GameObject settingsPanel;

    private GameObject previousPanel; // Variable pour stocker le panneau précédent

    // Fonction pour ouvrir les paramètres et enregistrer le menu précédent
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

        // Activer les paramètres et désactiver le menu actuel
        settingsPanel.SetActive(true);
        if (previousPanel != null) previousPanel.SetActive(false);
    }

    // Fonction pour fermer les paramètres et revenir au menu précédent
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (previousPanel != null) previousPanel.SetActive(true);
    }
}
