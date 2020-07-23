using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager sharedInstance;

    private int movesCounter;
    private int score;

    public Text moveText, scoreText;
    //variable auto computada
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = "Score: " + score;
        }
    }

    public int MovesCounter
    {
        get
        {
            return movesCounter;
        }
        set
        {
            movesCounter = value;
            moveText.text = "Moves: " + movesCounter;
        }
    }

    // Start is called before the first frame update
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
        movesCounter = 30;
        moveText.text = "Moves: " + movesCounter;
        scoreText.text = "Score: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
