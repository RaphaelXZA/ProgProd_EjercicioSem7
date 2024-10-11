using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ministerio : MonoBehaviour, IMinisterio
{
    public string Nombre;
    private int _nivel = 1;
    public int Nivel
    {
        get => _nivel;
        private set
        {
            if (_nivel != value)
            {
                _nivel = value;
                OnCambio?.Invoke(this);
            }
        }
    }
    public int CostoDeMantenimiento;
    public int IncrementoDeAreaDesarrollo;
    private int _vida = 5;
    public int Vida
    {
        get => _vida;
        private set
        {
            if (_vida != value)
            {
                _vida = value;
                OnCambio?.Invoke(this);
            }
        }
    }
    public string AreaDesarrollo;
    private bool _estaActivo;
    public bool EstaActivo
    {
        get => _estaActivo;
        private set
        {
            if (_estaActivo != value)
            {
                _estaActivo = value;
                OnCambio?.Invoke(this);
            }
        }
    }

    
    public event Action<Ministerio> OnCambio;
    public event Action<Ministerio> OnDestruccion;

    public bool Crear()
    {
        if (GameManager.Instance.DineroGobierno >= CostoDeMantenimiento)
        {
            GameManager.Instance.ActualizarDineroGobierno(-CostoDeMantenimiento);
            EstaActivo = true;
            OnCambio?.Invoke(this);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SubirNivel()
    {
        int minimaInversion = CostoDeMantenimiento + (int)(CostoDeMantenimiento * (50 / 100f));
        if (GameManager.Instance.DineroGobierno >= minimaInversion)
        {
            GameManager.Instance.ActualizarDineroGobierno(-minimaInversion);
            Nivel++;
            IncrementoDeAreaDesarrollo += 5;
            CostoDeMantenimiento += 100;
        }
        else
        {
            UIMessageManager.Instance.ShowMessage("No hay suficiente dinero para invertir en el ministerio.");
        }
    }

    public void Mantenimiento()
    {
        if (!EstaActivo) return;

        if (GameManager.Instance.DineroGobierno >= CostoDeMantenimiento)
        {
            GameManager.Instance.ActualizarDineroGobierno(-CostoDeMantenimiento);
        }
        else
        {
            Vida -= 1;
            UIMessageManager.Instance.ShowMessage($"El Ministerio de {Nombre} perdió 1 punto de vida por falta de mantenimiento.");
            if (Vida <= 0)
            {
                Destruir();
            }
        }
    }

    public void AplicarDesarrollo()
    {
        if (EstaActivo)
        {
            GameManager.Instance.ActualizarAreaDeDesarrollo(AreaDesarrollo, IncrementoDeAreaDesarrollo);
            UIMessageManager.Instance.ShowMessage($"El Ministerio de {Nombre} aumentó el desarrollo de {AreaDesarrollo} en {IncrementoDeAreaDesarrollo} puntos.");
        }
    }

    public void Destruir()
    {
        EstaActivo = false;
        Nivel = 1;
        Vida = 5;
        IncrementoDeAreaDesarrollo = 5;
        UIMessageManager.Instance.ShowMessage($"El Ministerio de {Nombre} ha sido destruido.");
        OnDestruccion?.Invoke(this);
        OnCambio?.Invoke(this);
    }
}


