using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ManagerScript : MonoBehaviour
{

    // UI elements
    [SerializeField] public Canvas GameCanvas;
    [SerializeField] public Button StartButton;
    [SerializeField] public TextMeshProUGUI GuideText;
    [SerializeField] public TextMeshProUGUI TimerText;
    // NOTE: put these gameobjects in this list in the same order as the playerscripts they are representing!


    // timer-related fields
    [SerializeField] public float playerTurnTime; // time to choose an option for a player
    [SerializeField] public float discussionTime; // time given between hand cards being revealed and the first player being made to choose
    float timeRemaining = 0.0f; // var to set and decrement as the timer
    bool discussionFlag = false;
    bool delverTurnFlag = false;

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
    [SerializeField] public PlayerScript delver1;
    [SerializeField] public PlayerScript delver2;
    [SerializeField] public PlayerScript delver3;
    [SerializeField] public PlayerScript delver4;
    [SerializeField] public PlayerScript delver5;
    [SerializeField] public PlayerScript delver6;
    [SerializeField] public PlayerScript delver7;
    [SerializeField] public PlayerScript delver8;

    // list of players, to be arranged by score programatically as game progresses
    public List<PlayerScript> delvers;

    // delver going first in a round
    PlayerScript firstDelver;

    // delver currently taking their turn
    bool currentDelverSet = false;
    PlayerScript currentDelver;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // hide header UI text initially
        GuideText.enabled = false;
        TimerText.gameObject.SetActive(false);

        // make the play button initiate the main game state
        StartButton.onClick.AddListener(InitiatePlayLoop);

        // initialize the card position var
        cardPos.Set(0, GameCanvas.pixelRect.height / 2);

        // push all delvers into list
        delvers.Add(delver1);
        delvers.Add(delver2);
        delvers.Add(delver3);
        delvers.Add(delver4);
        //delvers.Add(delver5);
        //delvers.Add(delver6);
        //delvers.Add(delver7);
        //delvers.Add(delver8);

        // connect all delvers to each other
        for(int i = 0; i < delvers.Count; i++)
        {
            delvers[i].delverID = i + 1;
            delvers[i].playerTitleText.text = "Player " + delvers[i].delverID;

            int rightDelverIdx = i + 1;
            int leftDelverIdx = i - 1;

            // wrap around the list of delvers when necessary
            if(i + 1 == delvers.Count) { rightDelverIdx = 0; }
            if(i - 1 == -1) { leftDelverIdx = delvers.Count - 1; }

            delvers[i].rightDelver = delvers[rightDelverIdx];
            delvers[i].leftDelver = delvers[leftDelverIdx];
        }

        // pick a random delver to go first in the starting round
        // TODO: let players choose this by vote instead?
        firstDelver = delvers[UnityEngine.Random.Range(0, delvers.Count())];
    }

    // Update is called once per frame
    void Update()
    {
        if(discussionFlag && timeRemaining <= 0)
        {
            // flip discussion/player turn flags
            discussionFlag = false;
            delverTurnFlag = true;
            // initiate player choice for the player going first this round
            InitiatePlayerChoice();
        }
        else if(delverTurnFlag && timeRemaining <= 0)
        {
            InitiatePlayerChoice();
        }
        // update timer text when something is being counted down
        else if(timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            TimerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
        // hide the timer text when nothing is being counted down
        else if(TimerText.gameObject.activeSelf)
        {
            TimerText.gameObject.SetActive(false);
        }
    }

    // set up the main play screen and draw the first hand of cards for players to pick from
    void InitiatePlayLoop()
    {
        // hide start button
        StartButton.gameObject.SetActive(false);

        // show the selection timer text
        GuideText.text = "Starting...";
        GuideText.enabled = true;

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
        currentHand = Hands[UnityEngine.Random.Range(0, Hands.Count)];

        // set spacing according to number of cards
        cardSpacing = (int)(GameCanvas.pixelRect.width) / (currentHand.cards.Count + 1);

        // set initial x position for first card
        cardPos.x = cardSpacing;

        // take each card from the selected hand and put them on the screen
        GameObject cardRef;
        foreach(CardScript card in currentHand.cards)
        {
            cardRef = Instantiate(card.gameObject);
            cardRef.transform.SetParent(GameCanvas.gameObject.transform);
            // move card to predetermined location
            cardRef.transform.SetPositionAndRotation(cardPos, Quaternion.identity);
            // update card position for next card
            cardPos.x += cardSpacing;
        }

        // initiate discussion timer
        timeRemaining = discussionTime;
        // set discussion flag for main event loop to catch and work with
        discussionFlag = true;
        // update guide text
        GuideText.text = "Discuss your choices now! Delver " + firstDelver.delverID + " begins the decision-making process in...";
        // display timer text
        TimerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        TimerText.gameObject.SetActive(true);
    }
 
    // set up a player to make a choice
    void InitiatePlayerChoice()
    {
        // set current player to the player going first if this is the start of player selection
        if(!currentDelverSet)
        {
            currentDelver = firstDelver;
            currentDelverSet = true;
        }
        // else increment the current player choosing
        else
        {
            currentDelver = currentDelver.rightDelver;
            // last player has already gone, player selection is over
            if(currentDelver == firstDelver)
            {
                // increment the starting player for the next round
                firstDelver = firstDelver.rightDelver;
                // reset current delver
                currentDelverSet = false;
                // unset the flag
                delverTurnFlag = false;
            }
        }
        // a player is going to make their selection now
        if(delverTurnFlag)
        {
            // update UI
            GuideText.text = "Delver " + currentDelver.delverID + " make your decision now!";
            // reset timer
            timeRemaining = playerTurnTime;
            // reset timer text (should already be activated from the discussion)
            TimerText.text = Mathf.CeilToInt(timeRemaining).ToString();            
        }
        // resolve the turn with all choices made
        else
        {
            GuideText.text = "Selections made! Resolving choices now...";
            ResolveTurn();
        }
    }

    // player input callbacks
    public void PlayerInputCallback(InputAction.CallbackContext context)
    {
        //Debug.Log("Pressed one of the buttons I care about!");

        if (!currentDelverSet)
        {
            Debug.Log("No delver currently in control to make a choice!");
            return;
        }
        // lock in choices for whatever player is currently in play
        switch (context.action.name)
        {
            case "OptionA":
                currentDelver.actionIdx = 0;
                Debug.Log("Chose A");
                currentDelver.playerScoreText.text = "Chose A";
                break;
            case "OptionB":
                currentDelver.actionIdx = 1;
                Debug.Log("Chose B");
                currentDelver.playerScoreText.text = "Chose B";
                break;
            case "OptionC":
                currentDelver.actionIdx = 2;
                Debug.Log("Chose C");
                currentDelver.playerScoreText.text = "Chose C";
                break;
            case "OptionD":
                currentDelver.actionIdx = 3;
                Debug.Log("Chose D");
                currentDelver.playerScoreText.text = "Chose D";
                break;
            case "OptionE":
                currentDelver.actionIdx = 4;
                Debug.Log("Chose E");
                currentDelver.playerScoreText.text = "Chose E";
                break;
            case "Confirm":
                Debug.Log("Confirming");
                if(currentDelver.actionIdx != -1) { timeRemaining = 0; }
                break;
            case "ThreatAction":
                currentDelver.callToSpirit = true;
                break;
            case "ChoosePlayer1":
                currentDelver.targets.Add(delver1);
                break;
            case "ChoosePlayer2":
                currentDelver.targets.Add(delver2);
                break;
            case "ChoosePlayer3":
                currentDelver.targets.Add(delver3);
                break;
            case "ChoosePlayer4":
                currentDelver.targets.Add(delver4);
                break;
            case "ChoosePlayer5":
                currentDelver.targets.Add(delver5);
                break;
            case "ChoosePlayer6":
                currentDelver.targets.Add(delver6);
                break;
            case "ChoosePlayer7":
                currentDelver.targets.Add(delver7);
                break;
            case "ChoosePlayer8":
                currentDelver.targets.Add(delver8);
                break;
            default:
                break;
        }
    }

    // work to take players choices and resolve them here, updating threat status/health and point totals and bringing the game to its end state if necesary
    void ResolveTurn()
    {
        // TODO: use the new scenario scripts here...somehow!
        Debug.Log("Turns are resolving btw");
        // determine if a winner exists
        int maxTreasures = 20;
        PlayerScript triumphantDelver = null;
        foreach(PlayerScript delver in delvers)
        {
            if(delver.treasures >= maxTreasures)
            {
                maxTreasures = delver.treasures;
                if(!triumphantDelver || triumphantDelver.treasures < maxTreasures || (triumphantDelver.treasures == delver.treasures && delver.favored))
                {
                    triumphantDelver = delver;
                }
            }
        }

        // destroy existing hand's instantiated cards to make way for the next hand
        foreach(CardScript card in GameCanvas.GetComponentsInChildren<CardScript>())
        {
            Destroy(card.gameObject);
        }

        if(triumphantDelver)
        {
            Debug.Log("Player has won the game!");
        }
        else
        {
            DrawHand();
        }
    }
}