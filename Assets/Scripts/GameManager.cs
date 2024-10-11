using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int DineroGobierno { get; private set; }
    public int DineroPoblacion { get; private set; }
    public int DesarrolloEducacion { get; private set; }
    public int DesarrolloSalud { get; private set; }
    public int DesarrolloSeguridad { get; private set; }
    public int DesarrolloAgricultura { get; private set; }

    public event Action<int> On_CambioDeDineroGobierno;
    public event Action<int> On_CambioDeDineroPoblacion;
    public event Action On_CambioDeAreasDesarrollo;

    public string[] NombresAreasDesarrollo = { "Educacion", "Salud", "Seguridad", "Agricultura" };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarJuego();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActualizarUI();
    }

    private void InicializarJuego()
    {
        DineroGobierno = 2000;
        DineroPoblacion = 1000;
        DesarrolloEducacion = 5;
        DesarrolloSalud = 5;
        DesarrolloSeguridad = 5;
        DesarrolloAgricultura = 5;
    }

    public void ActualizarDineroGobierno(int cantidad)
    {
        DineroGobierno = Math.Max(0, DineroGobierno + cantidad);
        On_CambioDeDineroGobierno?.Invoke(DineroGobierno);
    }

    public void ActualizarDineroPoblacion(int cantidad)
    {
        DineroPoblacion += cantidad;
        On_CambioDeDineroPoblacion?.Invoke(DineroPoblacion);
    }

    public void ActualizarAreaDeDesarrollo(string area, int incremento)
    {
        switch (area)
        {
            case "Educacion":
                DesarrolloEducacion = Mathf.Min(DesarrolloEducacion + incremento, 100);
                break;
            case "Salud":
                DesarrolloSalud = Mathf.Min(DesarrolloSalud + incremento, 100);
                break;
            case "Seguridad":
                DesarrolloSeguridad = Mathf.Min(DesarrolloSeguridad + incremento, 100);
                break;
            case "Agricultura":
                DesarrolloAgricultura = Mathf.Min(DesarrolloAgricultura + incremento, 100);
                break;
        }
        On_CambioDeAreasDesarrollo?.Invoke();
    }

    private void ActualizarUI()
    {
        On_CambioDeDineroGobierno?.Invoke(DineroGobierno);
        On_CambioDeDineroPoblacion?.Invoke(DineroPoblacion);
        On_CambioDeAreasDesarrollo?.Invoke();
    }

    public int ObtenerDesarrolloPorArea(string area)
    {
        switch (area)
        {
            case "Educacion": return DesarrolloEducacion;
            case "Salud": return DesarrolloSalud;
            case "Seguridad": return DesarrolloSeguridad;
            case "Agricultura": return DesarrolloAgricultura;
            default: return 0;
        }
    }

    public void ReiniciarJuego()
    {
        DineroGobierno = 2000;
        DineroPoblacion = 1000;
        DesarrolloEducacion = 5;
        DesarrolloSalud = 5;
        DesarrolloSeguridad = 5;
        DesarrolloAgricultura = 5;

        ForzarActualizacionUI();
    }

    public void ForzarActualizacionUI()
    {
        On_CambioDeDineroGobierno?.Invoke(DineroGobierno);
        On_CambioDeDineroPoblacion?.Invoke(DineroPoblacion);
        On_CambioDeAreasDesarrollo?.Invoke();
    }
}
