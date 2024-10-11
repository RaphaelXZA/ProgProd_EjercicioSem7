using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI DineroGobiernoText;
    public TextMeshProUGUI DineroPoblacionText;
    public TextMeshProUGUI DesarrolloEducacionText;
    public TextMeshProUGUI DesarrolloSaludText;
    public TextMeshProUGUI DesarrolloSeguridadText;
    public TextMeshProUGUI DesarrolloAgriculturaText;
    public TextMeshProUGUI ProbabilidadRevolucionText;

    private void Awake()
    {
        SuscribirEventos();
    }

    private void Start()
    {
        ActualizarTodasLasUI();
        ActualizarProbabilidadRevolucion(0);
    }


    private void SuscribirEventos()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.On_CambioDeDineroGobierno += ActualizarDineroGobiernoUI;
            GameManager.Instance.On_CambioDeDineroPoblacion += ActualizarDineroPoblacionUI;
            GameManager.Instance.On_CambioDeAreasDesarrollo += ActualizarAreasDesarrolloUI;
            EventosAleatorios.Instance.On_CambioProbabilidadRevolucion += ActualizarProbabilidadRevolucion;
        }
        else
        {
            Debug.LogError("GameManager.Instance es null en UIManager.SuscribirEventos");
        }
    }

    public void ActualizarTodasLasUI()
    {
        if (GameManager.Instance != null)
        {
            ActualizarDineroGobiernoUI(GameManager.Instance.DineroGobierno);
            ActualizarDineroPoblacionUI(GameManager.Instance.DineroPoblacion);
            ActualizarAreasDesarrolloUI();
            ActualizarProbabilidadRevolucion(EventosAleatorios.Instance.ObtenerProbabilidadRevolucion());
        }
        else
        {
            Debug.LogError("GameManager.Instance es null en UIManager.ActualizarTodasLasUI");
        }
    }

    private void ActualizarDineroGobiernoUI(int dinero)
    {
        DineroGobiernoText.text = $"Dinero Gobierno: {dinero}";
    }

    private void ActualizarDineroPoblacionUI(int dinero)
    {
        DineroPoblacionText.text = $"Dinero Población: {dinero}";
    }

    private void ActualizarAreasDesarrolloUI()
    {
        DesarrolloEducacionText.text = $"Desarrollo Educación: {GameManager.Instance.DesarrolloEducacion}/100";
        DesarrolloSaludText.text = $"Desarrollo Salud: {GameManager.Instance.DesarrolloSalud}/100";
        DesarrolloSeguridadText.text = $"Desarrollo Seguridad: {GameManager.Instance.DesarrolloSeguridad}/100";
        DesarrolloAgriculturaText.text = $"Desarrollo Agricultura: {GameManager.Instance.DesarrolloAgricultura}/100";
    }

    private void ActualizarProbabilidadRevolucion(float probabilidad)
    {
        ProbabilidadRevolucionText.text = $"Probabilidad de Revolución: {probabilidad:P0}";
    }
}
