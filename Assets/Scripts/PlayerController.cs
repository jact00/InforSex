using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Google.XR.Cardboard;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public List<Transform> destinos;
    public Transform destinoFinal;
    public GameObject infoPanel1;
    public TextAsset txtPreguntas;
    public GameObject quizUI;
    public GameObject gameOverUI;
    public AudioSource selectSound;

    private NavMeshAgent nma;
    private int score;
    private Estado estado;
    private List<Pregunta> preguntas;
    private Button[] quizButtons;
    private TextMeshProUGUI cuadroPregunta;
    private TextMeshProUGUI cuadroNumero;
    private Pregunta preguntaActual;
    private int btnIndex;
    private TextMeshProUGUI mensajeGameOver;
    private Button[] gameOverButtons;
    private bool pressed;
 
    private enum Estado
    {
        caminando,
        leyendo,
        enQuiz,
        gameOver
    }

    void Start()
    {
        pressed = false;
        GameObject panelPregunta = quizUI.GetComponentsInChildren<Image>()[1].gameObject;
        cuadroPregunta = panelPregunta.GetComponentInChildren<TextMeshProUGUI>();
        GameObject panelNumero = panelPregunta.GetComponentsInChildren<Image>()[1].gameObject;
        cuadroNumero = panelNumero.GetComponentInChildren<TextMeshProUGUI>();
        GameObject panelGameOver = gameOverUI.GetComponentsInChildren<Image>()[1].gameObject;
        mensajeGameOver = panelGameOver.GetComponentInChildren<TextMeshProUGUI>();
        quizButtons = quizUI.GetComponentsInChildren<Button>();
        gameOverButtons = gameOverUI.GetComponentsInChildren<Button>();
        estado = Estado.caminando;
        nma = GetComponent<NavMeshAgent>();
        nma.SetDestination(SiguienteDestino());
    }
    
    private bool CardboardTriggerClick()
    {
        if(!pressed)
        {
            if(Api.IsTriggerPressed && !Api.IsTriggerHeldPressed)
            {
                pressed = true;
            }
            return false;
        }
        else
        {
            if(Api.IsTriggerHeldPressed)
            {
                pressed = false;
                return false;
            }
            if(!Api.IsTriggerPressed)
            {
                pressed = false;
                return true;
            }
            return false;
        }
    }

    private void Update()
    {
        if(estado == Estado.leyendo)
        {
            InfoHandler();
        }
        if(estado == Estado.enQuiz)
        {
            QuizHandler();
        }
        if(estado == Estado.gameOver)
        {
            GameOverHandler();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Final") || other.gameObject.CompareTag("Coleccionable"))
        {
            nma.isStopped = true;
            if (other.gameObject.CompareTag("Coleccionable"))
            {
                estado = Estado.leyendo;
                other.gameObject.SetActive(false);
                nma.SetDestination(SiguienteDestino());
            }
            else
            {
                InicializarQuiz();
            }
        }    
    }

    public void InicializarQuiz()
    {
        estado = Estado.enQuiz;
        score = 0;
        btnIndex = 0;
        preguntas = new List<Pregunta>();
        CargarPreguntas();
        preguntaActual = SiguientePregunta();
        MostrarPregunta();
        quizUI.SetActive(true);
    }

    private void MostrarSiguiente()
    {
        btnIndex = 0;
        preguntaActual = SiguientePregunta();
        MostrarPregunta();
    }

    private void IncrementarPuntuacion()
    {
        score++;
    }

    private void GameOver()
    {
        estado = Estado.gameOver;
        btnIndex = 0;
        gameOverButtons[0].Select();
        quizUI.SetActive(false);
        gameOverUI.SetActive(true);
        if(score > 3)
        {
            mensajeGameOver.text = "Ganaste!";
            mensajeGameOver.gameObject.GetComponentInParent<Image>().color = Color.green;
        }
        else
        {
            mensajeGameOver.text = "Perdiste!";
            mensajeGameOver.gameObject.GetComponentInParent<Image>().color = Color.red;
        }
    }

    private void InfoHandler()
    {
        if(CardboardTriggerClick() || Input.GetMouseButtonUp(0))
        {
            if(infoPanel1.activeSelf)
            {
                infoPanel1.SetActive(false);
            }
            else
            {
                GameObject infoUi = infoPanel1.GetComponentInParent<RectTransform>().gameObject;
                infoUi.SetActive(false);
                infoPanel1.SetActive(true);
                estado = Estado.caminando;
                nma.isStopped = false;
            }
        }
    }
    private void QuizHandler()
    {
        if(CardboardTriggerClick())
        {
            btnIndex = (btnIndex + 1) % 4;
            quizButtons[btnIndex].Select();
            selectSound.Play();
        }
        if(Api.IsTriggerHeldPressed)
        {
            quizButtons[btnIndex].onClick.Invoke();
        }
    }

    private void GameOverHandler()
    {
        if (CardboardTriggerClick())
        {
            btnIndex = (btnIndex + 1) % 3;
            gameOverButtons[btnIndex].Select();
            selectSound.Play();
        }
        if (Api.IsTriggerHeldPressed)
        {
            gameOverButtons[btnIndex].onClick.Invoke();
        }
    }

    private Pregunta SiguientePregunta()
    {
        int idx = new System.Random().Next(preguntas.Count);
        Pregunta sigPregunta = preguntas[idx];
        preguntas.RemoveAt(idx);
        return sigPregunta;
    }

    private Vector3 SiguienteDestino()
    {
        Transform sigDestino = destinoFinal;

        if(destinos.Count > 0)
        {
            int idx = new System.Random().Next(destinos.Count);
            sigDestino = destinos[idx];
            destinos.RemoveAt(idx);
        }
        return sigDestino.position;
    }

    private void MostrarPregunta()
    {
        cuadroPregunta.text = preguntaActual.pregunta;
        cuadroNumero.text = score + "/5";

        List<string> respuestas = preguntaActual.respuestas;
        System.Random rnd = new System.Random();
        quizButtons[0].Select();
        for(int i = 0; i < 4; i++)
        {
            int idx = rnd.Next(respuestas.Count);
            string respuesta = respuestas[idx];
            respuestas.RemoveAt(idx);
            quizButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = respuesta;
            quizButtons[i].onClick.RemoveAllListeners();
            ColorBlock c = quizButtons[i].colors;
            if (respuesta == preguntaActual.respuestaCorrecta)
            {
                quizButtons[i].onClick.AddListener(IncrementarPuntuacion);
            }
            quizButtons[i].colors = c;
            if(preguntas.Count > 5)
            {
                quizButtons[i].onClick.AddListener(MostrarSiguiente);
            }
            else
            {
                quizButtons[i].onClick.AddListener(GameOver);
            }
        }
    }

    private void CargarPreguntas()
    {
        string[] qa = txtPreguntas.text.Split('\n');
        for(int i = 0; i < qa.Length; i += 5)
        {
            Pregunta pregunta = new Pregunta();
            pregunta.pregunta = qa[i];
            pregunta.respuestaCorrecta = qa[i + 1];
            for(int j = 1; j <= 4; j++)
            {
                pregunta.AgregarRespuesta(qa[i + j]);
            }
            preguntas.Add(pregunta);
        }
    }
}