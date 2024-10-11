using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public static TurnController Instance { get; private set; }

    public int TurnoActual { get; private set; } = 1;
    public List<Ministerio> Ministerios = new List<Ministerio>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PasarTurno()
    {
        TurnoActual++;

        Ministerios.RemoveAll(m => m == null);

        foreach (var ministerio in Ministerios)
        {
            ministerio.Mantenimiento();
            ministerio.AplicarDesarrollo();
        }

        int ingresosPoblacion = CalcularIngresosPoblacion();
        GameManager.Instance.ActualizarDineroPoblacion(ingresosPoblacion);

        CobrarImpuestos();

        EventosAleatorios.Instance.IntentarGenerarEvento();
        EventosAleatorios.Instance.ReiniciarProbabilidadRevolucion();
    }

    private int CalcularIngresosPoblacion()
    {
        int desarrolloTotal =
            GameManager.Instance.DesarrolloEducacion +
            GameManager.Instance.DesarrolloSalud +
            GameManager.Instance.DesarrolloSeguridad +
            GameManager.Instance.DesarrolloAgricultura;

        return desarrolloTotal * 2; 
    }

    private void CobrarImpuestos()
    {
        float tasaImpuestos = 0.3f;
        int impuestos = (int)(GameManager.Instance.DineroPoblacion * tasaImpuestos);
        GameManager.Instance.ActualizarDineroPoblacion(-impuestos);
        GameManager.Instance.ActualizarDineroGobierno(impuestos);
    }

    public void ReiniciarTurnos()
    {
        TurnoActual = 1;
        Ministerios.Clear();
    }
}
