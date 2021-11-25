using System.Collections;
using System.Collections.Generic;

public class Pregunta
{
    private string _pregunta;
    private string _respuestaCorrecta;
    private List<string> _respuestas = new List<string>();
    
    public List<string> respuestas { get => _respuestas;}
    public string respuestaCorrecta { get => _respuestaCorrecta; set => _respuestaCorrecta = value; }
    public string pregunta { get => _pregunta; set => _pregunta = value; }

    public void AgregarRespuesta(string respuesta)
    {
        _respuestas.Add(respuesta);
    }
}
