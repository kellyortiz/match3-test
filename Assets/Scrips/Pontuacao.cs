using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pontuacao : MonoBehaviour
{
    public static Pontuacao sharedInstance;

    private int movimContador;
    private int pontos;

    public Text moveText, pontosTexto;
    //variable auto computada
    public int Pontos
    {
        get
        {
            return pontos;
        }
        set
        {
            pontos = value;
            pontosTexto.text = "Pontuação: " + pontos;
        }
    }

    public int MovimentacaoContador
    {
        get
        {
            return movimContador;
        }
        set
        {
            movimContador = value;
        }
    }

    void Start()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        movimContador = 2;
        pontosTexto.text = "Pontuação: " + pontosTexto;
    }
}
