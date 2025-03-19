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

    // timer-related fields
    [SerializeField] public float playerTurnTime; // time to choose an option for a player
    [SerializeField] public float discussionTime; // time given between hand cards being revealed and the first player being made to choose
    float timeRemaining = 0.0f; // var to set and decrement as the timer
    bool discussionFlag = false;
    bool playerTurnFlag = false;

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

    // player going first in a round
    int FirstPlayerIdx = -1;

    // player currently taking their turn
    int CurrentPlayerIdx = -1;

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

        // pick a random player to go first in the starting round
        FirstPlayerIdx = UnityEngine.Random.Range(0, Players.Count());
    }

    // Update is called once per frame
    void Update()
    {
        // wait for start to be clicked
        // IF game started
        // hide start button
        // display player scores, threat status, and a marker for player going first
        // pick a random hand of card prefabs to load. Size and position them based on number of cards in the hand
        if(discussionFlag && timeRemaining <= 0)
        {
            // flip discussion/player turn flags
            discussionFlag = false;
            playerTurnFlag = true;
            // initiate player choice for the player going first this round
            InitiatePlayerChoice();
        }
        else if(playerTurnFlag && timeRemaining <= 0)
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
        GuideText.text = "Discuss your choices now! Player " + (FirstPlayerIdx + 1) + " begins the decision-making process in...";
        // display timer text
        TimerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        TimerText.gameObject.SetActive(true);
    }
 
    // set up a player to make a choice
    void InitiatePlayerChoice()
    {
        // set current player to the player going first if this is the start of player selection
        if(CurrentPlayerIdx < 0)
        {
            CurrentPlayerIdx = FirstPlayerIdx;
        }
        // else increment the current player choosing
        else
        {
            CurrentPlayerIdx++;
            // handle wrap-around
            if(CurrentPlayerIdx >= Players.Count())
            {
                CurrentPlayerIdx = 0;
            }
            // last player has already gone, player selection is over
            if(CurrentPlayerIdx == FirstPlayerIdx)
            {
                // increment the starting player for the next round
                FirstPlayerIdx = CurrentPlayerIdx + 1;
                if(FirstPlayerIdx >= Players.Count())
                {
                    FirstPlayerIdx = 0;
                }
                // reset current player
                CurrentPlayerIdx = -1;
                // unset the flag
                playerTurnFlag = false;
            }
        }
        // a player is going to make their selection now
        if(playerTurnFlag)
        {
            // update UI
            GuideText.text = "Player " + (CurrentPlayerIdx + 1) + " make your decision now!";
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
        Debug.Log("Pressed one of the buttons I care about!");

        if(CurrentPlayerIdx < 0)
        {
            Debug.Log("No player currently in control to make a choice!");
            return;
        }
        // lock in choices for whatever player is currently in play
        switch (context.action.name)
        {
            case "OptionA":
                Players[CurrentPlayerIdx].playerChoice = 0;
                break;
            case "OptionB":
                Players[CurrentPlayerIdx].playerChoice = 1;
                break;
            case "OptionC":
                Players[CurrentPlayerIdx].playerChoice = 2;
                break;
            case "OptionD":
                Players[CurrentPlayerIdx].playerChoice = 3;
                break;
            case "OptionE":
                Players[CurrentPlayerIdx].playerChoice = 4;
                break;
            case "ThreatAction":
                Players[CurrentPlayerIdx].threatAction = true;
                break;
            case "ChoosePlayer1":
                Players[CurrentPlayerIdx].playerTargets.Add(1);
                break;
            case "ChoosePlayer2":
                Players[CurrentPlayerIdx].playerTargets.Add(2);
                break;
            case "ChoosePlayer3":
                Players[CurrentPlayerIdx].playerTargets.Add(3);
                break;
            case "ChoosePlayer4":
                Players[CurrentPlayerIdx].playerTargets.Add(4);
                break;
            default:
                break;
        }
    }

    // work to take players choices and resolve them here, updating threat status/health and point totals and bringing the game to its end state if necesary
    void ResolveTurn()
    {
        // check who took the threat action
        PlayerScript threatPlayer = null;
        List<PlayerScript> threatActionPlayers = new List<PlayerScript>();
        foreach(PlayerScript player in Players)
        {
            if(player.threat)
            {
                threatPlayer = player;
            }

            if(player.threatAction)
            {
                threatActionPlayers.Add(player);
            }
        }

        // resolve changes in threat status
        if(threatActionPlayers.Count > 0)
        {
            // someone is gaining threat status or failing their defense
            if(threatActionPlayers.Count() == 1)
            {
                // failed defense, remove threat status
                if(threatPlayer)
                {
                    threatPlayer.threat = false;
                }
                // successful threat activation, provide bonus and set flag
                else
                {
                    threatPlayer = threatActionPlayers[0];
                    threatPlayer.threat = true;
                    threatPlayer.score += 5;
                }
            }
            // players are fighting the threat
            else if(threatPlayer)
            {
                // threat defended themselves
                if(threatPlayer.threatAction)
                {
                    // threat succesfully defends themselves against multiple attackers
                    if(threatActionPlayers.Count() > 2)
                    {
                        // deduct points from the dunces
                        foreach(PlayerScript player in threatActionPlayers)
                        {
                            if(!player.threat)
                            {
                                player.score -= 2;
                            }
                        }
                    }
                    // defense was subverted by a single sneaky attacker, passing threat status to them
                    else
                    {
                        foreach(PlayerScript player in threatActionPlayers)
                        {
                            if(player.threat)
                            {
                                player.threat = false;
                                player.score -= 5;
                            }
                            else
                            {
                                player.threat = true;
                                player.score += 5;
                            }
                        }
                    }
                }
                // threat did not defend themselves, they lose threat status and points
                else
                {
                    threatPlayer.threat = false;
                    threatPlayer.score -= 5;
                }
            }
            // multiple people unsuccessfully tried to become the threat at once
            else
            {
                // deduct points from the dunces
                foreach(PlayerScript player in threatActionPlayers)
                {
                    player.score -= 2;
                }
            }

            // reset the threat action flag for everyone that took it
            foreach(PlayerScript player in threatActionPlayers)
            {
                player.threatAction = false;
            }
        }

        // resolve card choice effects
        int playerResolutionIdx = FirstPlayerIdx;
        PlayerScript playerToAffect = null;        
        do
        {
            playerToAffect = Players[playerResolutionIdx];
            // verify they chose a valid card option
            // TODO: prevent this bad choice from being possible in the first place!
            if(playerToAffect.playerChoice >= currentHand.cards.Count())
            {
                Debug.Log("Player " + (playerResolutionIdx + 1) + " chose a non-existant card! Are they stupid?");
            }
            else
            {
                // refer to the TABLE OF CARD EFFECTS to resolve players card choices
                // EffectManager.ResolveEffect(playerToAffect, currentHand.cards[playerToAffect.playerChoice]);
            }

            // update the player resolution index
            playerResolutionIdx++;
            if(playerResolutionIdx == Players.Count())
            {
                playerResolutionIdx = 0;
            }
        } while (playerResolutionIdx != FirstPlayerIdx);

        // determine if a winner exists
        int winningScore = 20;
        PlayerScript winningPlayer = null;
        foreach(PlayerScript player in Players)
        {
            if(player.score >= winningScore)
            {
                winningScore = player.score;
                if(!winningPlayer || winningPlayer.score < winningScore || (winningPlayer.score == player.score && player.threat))
                {
                    winningPlayer = player;
                }
            }
        }

        // TODO: destroy existing hand's instantiated cards to make way for the next hand

        if(winningPlayer)
        {
            Debug.Log("Player has won the game!");
        }
        else
        {
            DrawHand();
        }
    }
}
