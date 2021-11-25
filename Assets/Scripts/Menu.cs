using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{ 
    public GameObject panelMenu;
    public GameObject panelNombres;

    private void Start()
    {
        panelNombres.SetActive(false);
    }

    public void Iniciar()
    {
        SceneManager.LoadScene("Juego", LoadSceneMode.Single);
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void MostrarNombres()
    {
        panelNombres.SetActive(true);
        panelMenu.SetActive(false);
    }

    public void Regresar()
    {
        panelNombres.SetActive(false);
        panelMenu.SetActive(true);
    }
}
