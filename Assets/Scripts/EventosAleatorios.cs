using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosAleatorios : MonoBehaviour
{
    public static EventosAleatorios Instance { get; private set; }

    private string[] areasDesarrollo = { "Educacion", "Salud", "Seguridad", "Agricultura" };
    private float probabilidadRevolucion = 0f;
    public event Action<float> On_CambioProbabilidadRevolucion;

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

    public void IntentarGenerarEvento()
    {
        if (UnityEngine.Random.value <= 0.3f)
        {
            GenerarEvento();
        }
        else
        {
            UIMessageManager.Instance.ShowMessage("No ha ocurrido ningún evento este turno.");
        }
    }

    private void GenerarEvento()
    {
        string areaAfectada = areasDesarrollo[UnityEngine.Random.Range(0, areasDesarrollo.Length)];
        int nivelDesarrollo = GameManager.Instance.ObtenerDesarrolloPorArea(areaAfectada);

        float probabilidadPositiva = nivelDesarrollo / 100f;
        bool esPositivo = UnityEngine.Random.value < probabilidadPositiva;

        int intensidad = UnityEngine.Random.Range(100, 300);

        if (esPositivo)
        {
            GameManager.Instance.ActualizarDineroGobierno(intensidad);
            UIMessageManager.Instance.ShowMessage($"El gobierno ganó {intensidad} de dinero debido a un evento positivo causado por el nivel de desarrollo en {areaAfectada}.");
        }
        else
        {
            GameManager.Instance.ActualizarDineroGobierno(-intensidad);
            UIMessageManager.Instance.ShowMessage($"El gobierno perdió {intensidad} de dinero debido a un evento negativo causado por el nivel de desarrollo en {areaAfectada}.");
        }
    }

    public void AumentarProbabilidadRevolucion()
    {
        probabilidadRevolucion += 0.1f;
        probabilidadRevolucion = Mathf.Min(probabilidadRevolucion, 1f);
        On_CambioProbabilidadRevolucion?.Invoke(probabilidadRevolucion);
    }

    public bool VerificarRevolucion()
    {
        if (UnityEngine.Random.value < probabilidadRevolucion)
        {
            return true;
        }
        return false;
    }

    public void ReiniciarProbabilidadRevolucion()
    {
        probabilidadRevolucion = 0f;
        ActualizarUIProbabilidadRevolucion();

    }

    public float ObtenerProbabilidadRevolucion()
    {
        return probabilidadRevolucion;
    }

    public void ActualizarUIProbabilidadRevolucion()
    {
        On_CambioProbabilidadRevolucion?.Invoke(probabilidadRevolucion);
    }
}
