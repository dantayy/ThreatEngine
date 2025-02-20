using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    public enum choices {
        noChoice,
        choice1,
        choice2,
        choice3,
        choice4,
        choice5,
        choice6,
        choice7,
        choice8,
        choice9,
        choiceThreat
    }

    public int score = 0;
    public bool threat = false;
    public choices playerChoice = choices.noChoice;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
