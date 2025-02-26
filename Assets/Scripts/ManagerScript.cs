using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;


public class ManagerScript : MonoBehaviour
{

    // UI elements
    [SerializeField] public Canvas GameCanvas;
    [SerializeField] public Button StartButton;
    [SerializeField] public TextMeshProUGUI TimerText;
    [SerializeField] public float playerTurnTime;
    [SerializeField] public float timeBetweenRounds;
    private bool timerRunning = false;

    // hands that can be played in the game
    [SerializeField] public List<HandScript> Hands;
    [SerializeField] GameObject cardPrefab;

    // hand currently being played
    HandScript currentHand;

    // dynamic position to place cards with
    Vector2 cardPos = new Vector2();
    // dynamic spacing between cards
    int cardSpacing = 0;

    // players to be managed
    [SerializeField] public List<PlayerScript> Players;

    // player currently taking their turn
    PlayerScript CurrentPlayer = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // hide the timer initially
        TimerText.enabled = false;

        // make the play button initiate the main game state
        StartButton.onClick.AddListener(InitiatePlayLoop);

        // initialize the card position var
        cardPos.Set(0, GameCanvas.pixelRect.height / 2);
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
        // hide start button
        StartButton.enabled = false;

        // show the selection timer text
        TimerText.text = "Starting...";
        TimerText.enabled = true;

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
        currentHand = Hands[Random.Range(0, Hands.Count)];

        cardSpacing = (int)(GameCanvas.pixelRect.width) / (currentHand.cards.Count + 1);

        cardPos.x = cardSpacing;

        GameObject cardRef;
        // take each card from the selected hand and put them on the screen
        foreach(CardScript card in currentHand.cards)
        {
            cardRef = Instantiate(card.gameObject);
            cardRef.transform.SetParent(GameCanvas.gameObject.transform);
            cardRef.transform.SetPositionAndRotation(cardPos, Quaternion.identity);
            cardPos.x += cardSpacing;
        }
    }

    // work to take players choices and resolve them here, updating threat status/health and point totals and bringing the game to its end state if necesary
    void ResolveTurn()
    {

    }
}
