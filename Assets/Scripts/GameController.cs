using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("UI References")]
    public Button[] botonesCrearMinisterio;
    public Button[] botonesSubirNivelMinisterio;
    public Button botonPasarTurno;
    public Button botonCobrarImpuestosAdicionales;
    public TextMeshProUGUI textoTurnoActual;

    [Header("Prefabs")]
    public GameObject[] prefabsMinisterios;

    [Header("Managers")]
    public UIManager uiManager;

    [Header("UI Ministerios")]
    public MinisterioUI[] MinisteriosUI;

    [Header("UI Inicio de Juego")]
    public GameObject panelGameplay;
    public GameObject panelInicial;
    public Button botonIniciarJuego;

    [Header("UI Fin de Juego")]
    public GameObject panelFinJuego;
    public TextMeshProUGUI textoMensajeFinJuego;
    public Button botonReiniciarJuego;


    private Ministerio[] ministerios;

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
            return;
        }

        InicializarJuego();
    }

    private void Start()
    {
        botonReiniciarJuego.onClick.AddListener(ReiniciarJuego);
        botonIniciarJuego.onClick.AddListener(IniciarGameplay);
        panelFinJuego.SetActive(false);
        panelGameplay.SetActive(false);
    }

    private void IniciarGameplay()
    {
        panelInicial.SetActive(false);
        panelGameplay.SetActive(true);
    }

    private void InicializarJuego()
    {
        ministerios = new Ministerio[4];
        for (int i = 0; i < botonesCrearMinisterio.Length; i++)
        {
            int index = i;
            botonesCrearMinisterio[i].onClick.AddListener(() => CrearMinisterio(index));
            botonesSubirNivelMinisterio[i].onClick.AddListener(() => SubirNivelMinisterio(index));
            botonesSubirNivelMinisterio[i].interactable = false;
        }

        botonPasarTurno.onClick.AddListener(PasarTurno);
        botonCobrarImpuestosAdicionales.onClick.AddListener(CobrarImpuestosAdicionales);

        ActualizarUITurno();
    }

    private void CrearMinisterio(int index)
    {
        if (ministerios[index] == null)
        {
            GameObject nuevoMinisterio = Instantiate(prefabsMinisterios[index]);
            Ministerio ministerioComponent = nuevoMinisterio.GetComponent<Ministerio>();

            if (ministerioComponent.Crear())
            {
                ministerios[index] = ministerioComponent;
                ministerios[index].OnDestruccion += (m) => HandleMinisterioDestruccion(index);
                ActualizarBotonesMinisterio(index, true);
                TurnController.Instance.Ministerios.Add(ministerios[index]);
                MinisteriosUI[index].InicializarUI(ministerios[index]);
            }
            else
            {
                Destroy(nuevoMinisterio);
                UIMessageManager.Instance.ShowMessage("No hay suficiente dinero para crear el ministerio.");
            }
        }
    }

    private void HandleMinisterioDestruccion(int index)
    {
        if (ministerios[index] != null)
        {
            MinisteriosUI[index].InicializarUI(null);
            Destroy(ministerios[index].gameObject);
            ministerios[index] = null;
            ActualizarBotonesMinisterio(index, false);
            TurnController.Instance.Ministerios.RemoveAll(m => m == null);
        }
    }

    private void ActualizarBotonesMinisterio(int index, bool ministerioCreado)
    {
        botonesCrearMinisterio[index].interactable = !ministerioCreado;
        botonesSubirNivelMinisterio[index].interactable = ministerioCreado;
    }

    private void DestruirMinisterio(int index)
    {
        if (ministerios[index] != null)
        {
            ministerios[index].Destruir();
            MinisteriosUI[index].InicializarUI(null);
            Destroy(ministerios[index].gameObject);
            ministerios[index] = null;
            botonesCrearMinisterio[index].interactable = true;
            botonesSubirNivelMinisterio[index].interactable = false;
            TurnController.Instance.Ministerios.Remove(ministerios[index]);
        }
    }

    private void SubirNivelMinisterio(int index)
    {
        if (ministerios[index] != null)
        {
            ministerios[index].SubirNivel();
            ActualizarUIMinisterio(index);
        }
    }

    private void PasarTurno()
    {
        TurnController.Instance.PasarTurno();
        ActualizarUITurno();
        VerificarFinDelJuego();
    }

    private void CobrarImpuestosAdicionales()
    {
        float tasaImpuestosAdicionales = 0.10f;
        int impuestosAdicionales = (int)(GameManager.Instance.DineroPoblacion * tasaImpuestosAdicionales);
        GameManager.Instance.ActualizarDineroPoblacion(-impuestosAdicionales);
        GameManager.Instance.ActualizarDineroGobierno(impuestosAdicionales);
        UIMessageManager.Instance.ShowMessage("No abuses de los impuestos adicionales porque la poblacion podria revelarse contra ti.");

        if (EventosAleatorios.Instance != null)
        {
            EventosAleatorios.Instance.AumentarProbabilidadRevolucion();
            EventosAleatorios.Instance.VerificarRevolucion();
        }

        if (EventosAleatorios.Instance.VerificarRevolucion())
        {
            GameController.Instance.FinalizarJuego(false, "¡Te han hecho un golpe de estado!");
            return;
        }
    }

    private void ActualizarUITurno()
    {
        textoTurnoActual.text = $"TURNO: {TurnController.Instance.TurnoActual}";
    }

    private void VerificarFinDelJuego()
    {
        if (GameManager.Instance.DineroGobierno <= 0)
        {
            FinalizarJuego(false, "Tu gobierno se ha quedado sin dinero");
        }
        else if (TodosLosDesarrollosMaximos())
        {
            FinalizarJuego(true, "Todos los desarrollos están al máximo. ¡Has ganado!");
        }
    }

    private bool TodosLosDesarrollosMaximos()
    {
        int desarrolloMaximo = 100;
        return GameManager.Instance.DesarrolloEducacion >= desarrolloMaximo &&
               GameManager.Instance.DesarrolloSalud >= desarrolloMaximo &&
               GameManager.Instance.DesarrolloSeguridad >= desarrolloMaximo &&
               GameManager.Instance.DesarrolloAgricultura >= desarrolloMaximo;
    }

    public void FinalizarJuego(bool victoria, string razon)
    {
        string mensaje = victoria ? $"Victoria: {razon}" : $"Derrota: {razon}";
        
        DesactivarBotonesJuego();

        textoMensajeFinJuego.text = mensaje;
        panelFinJuego.SetActive(true);

    }

    private void DesactivarBotonesJuego()
    {
        botonPasarTurno.interactable = false;
        botonCobrarImpuestosAdicionales.interactable = false;
        foreach (var boton in botonesCrearMinisterio)
        {
            boton.interactable = false;
        }
        foreach (var boton in botonesSubirNivelMinisterio)
        {
            boton.interactable = false;
        }
    }

    public void ReiniciarJuego()
    {
        panelFinJuego.SetActive(false);

        GameManager.Instance.ReiniciarJuego();
        TurnController.Instance.ReiniciarTurnos();
        EventosAleatorios.Instance.ReiniciarProbabilidadRevolucion();

        foreach (var ministerio in ministerios)
        {
            if (ministerio != null)
            {
                Destroy(ministerio.gameObject);
            }
        }

        ministerios = new Ministerio[4];

        botonPasarTurno.interactable = true;
        botonCobrarImpuestosAdicionales.interactable = true;
        for (int i = 0; i < botonesCrearMinisterio.Length; i++)
        {
            botonesCrearMinisterio[i].interactable = true;
            botonesSubirNivelMinisterio[i].interactable = false;
        }

        foreach (var ministerioUI in MinisteriosUI)
        {
            ministerioUI.InicializarUI(null);
        }

        ActualizarUICompleta();
    }

    private void ActualizarUICompleta()
    {
        ActualizarUITurno();
        GameManager.Instance.ForzarActualizacionUI();
        EventosAleatorios.Instance.ActualizarUIProbabilidadRevolucion();
        uiManager.ActualizarTodasLasUI();
        UIMessageManager.Instance.RestartMessageBox();
    }


    public void ActualizarUIMinisterio(int index)
    {
        MinisteriosUI[index].InicializarUI(ministerios[index]);
    }
}