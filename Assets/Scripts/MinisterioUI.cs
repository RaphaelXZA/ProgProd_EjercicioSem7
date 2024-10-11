using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinisterioUI : MonoBehaviour
{
    public Image MinisterioImage;
    public TextMeshProUGUI VidaNivelText;
    private Ministerio _ministerioAsociado;

    public Ministerio MinisterioAsociado
    {
        get => _ministerioAsociado;
        set
        {
            if (_ministerioAsociado != null)
            {
                _ministerioAsociado.OnCambio -= ActualizarUI;
            }
            _ministerioAsociado = value;
            if (_ministerioAsociado != null)
            {
                _ministerioAsociado.OnCambio += ActualizarUI;
            }
            ActualizarUI(_ministerioAsociado);
        }
    }

    private void Start()
    {
        OcultarUI();
    }

    public void InicializarUI(Ministerio ministerio)
    {
        MinisterioAsociado = ministerio;
    }

    private void ActualizarUI(Ministerio ministerio)
    {
        if (ministerio != null && ministerio.EstaActivo)
        {
            MostrarUI();
            VidaNivelText.text = $"Vida: {ministerio.Vida} " +
                $"Nivel: {ministerio.Nivel} " +
                $"Mantenimiento: {ministerio.CostoDeMantenimiento} " +
                $"Min. inversion: {ministerio.CostoDeMantenimiento + (int)(ministerio.CostoDeMantenimiento * (50 / 100f))}";
        }
        else
        {
            OcultarUI();
        }
    }

    private void MostrarUI()
    {
        MinisterioImage.gameObject.SetActive(true);
        VidaNivelText.gameObject.SetActive(true);
    }

    private void OcultarUI()
    {
        MinisterioImage.gameObject.SetActive(false);
        VidaNivelText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (MinisterioAsociado != null)
        {
            MinisterioAsociado.OnCambio -= ActualizarUI;
        }
    }
}
