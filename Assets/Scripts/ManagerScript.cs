using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;


public class ManagerScript : MonoBehaviour
{

    [Header("Cards")]
    // hands that can be played in the game
    [field: SerializeField] public List<HandScript> Hands;

    // hand currently being played
    HandScript currentHand;

    // players to be managed
    [Header("Players")]
    [field: SerializeField] public PlayerScript Player1;
    [field: SerializeField] public PlayerScript Player2;
    [field: SerializeField] public PlayerScript Player3;
    [field: SerializeField] public PlayerScript Player4;

    // player currently taking their turn
    PlayerScript CurrentPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // put start button on screen(?)
    }

    // Update is called once per frame
    void Update()
    {
        // wait for start to be clicked
        // IF game started
        // hide start button
        // display player scores, threat status, and a marker for player going first
        // pick a random hand of card prefabs to load. Size and position them based on number of cards in the hand
    }

    // set up the main play screen and draw the first hand of cards for players to pick from
    void InitiatePlayLoop()
    {
        // set current player
        CurrentPlayer = Player1;
        // no hands to pull from, abort
        if(Hands.Count == 0)
        {
            Debug.Log("No hands set up to draw! Aborting!!!");
            Application.Quit();
        }
        else
        {
            DrawHand();
        }
    }

    // draw a hand of cards to show the players
    void DrawHand()
    {
        // select a random hand from the list
        currentHand = Hands[Random.Range(0, Hands.Count - 1)];

        // take each card from the selected hand and put them on the screen
        foreach(CardScript card in currentHand.cards)
        {
            //card.GetComponent<Transform>().
        }
    }

    // work to take players choices and resolve them here, updating threat status/health and point totals and bringing the game to its end state if necesary
    void ResolveTurn()
    {

    }
}
