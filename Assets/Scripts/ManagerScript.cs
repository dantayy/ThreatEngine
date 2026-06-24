using NUnit.Framework;
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
    bool delverTurnFlag = false;

    // turn counter
    int turnCount = 1;

    // scenario pool variables
    [SerializeField] public List<ScenarioScript> scenarios;

    List<ScenarioScript> earlyScenarios = new List<ScenarioScript>();
    List<ScenarioScript> midScenarios = new List<ScenarioScript>();
    List<ScenarioScript> lateScenarios = new List<ScenarioScript>();
    ScenarioScript currentScenario;
    int scenarioIdx = 0;
    int currentScenarioOptionCount = 0;

    // UI elements for displaying scenario option details
    public List<OptionScript> optionDisplays;
    private string[] optionIDs = { "A", "B", "C", "D" };

    // players to be managed
    [SerializeField] public PlayerScript delver1;
    [SerializeField] public PlayerScript delver2;
    [SerializeField] public PlayerScript delver3;
    [SerializeField] public PlayerScript delver4;
    [SerializeField] public PlayerScript delver5;
    [SerializeField] public PlayerScript delver6;
    [SerializeField] public PlayerScript delver7;
    [SerializeField] public PlayerScript delver8;

    // list of players
    public List<PlayerScript> delvers;

    // delver going first in a round
    PlayerScript firstDelver;

    // delver currently taking their turn
    bool currentDelverSet = false;
    PlayerScript currentDelver;

    // game state history
    List<GameStateScript> gameStateHistory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // hide header UI text initially
        GuideText.enabled = false;
        TimerText.gameObject.SetActive(false);

        // make the play button initiate the main game state
        StartButton.onClick.AddListener(InitiatePlayLoop);

        // push all delvers into list
        delvers.Add(delver1);
        delvers.Add(delver2);
        delvers.Add(delver3);
        delvers.Add(delver4);
        //delvers.Add(delver5);
        //delvers.Add(delver6);
        //delvers.Add(delver7);
        //delvers.Add(delver8);

        // connect all delvers to each other relatively
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

        // fill the early, mid, and late game scenario lists
        foreach(ScenarioScript scenario in scenarios)
        {
            if(scenario.earlyGame)
            {
                earlyScenarios.Add(scenario);
            }
            else if(scenario.lateGame)
            {
                lateScenarios.Add(scenario);
            }
            else
            {
                midScenarios.Add(scenario);
            }
        }
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

    // set up the main play screen and select a scenario
    void InitiatePlayLoop()
    {
        // hide start button
        StartButton.gameObject.SetActive(false);

        // show the selection timer text
        GuideText.text = "Starting...";
        GuideText.enabled = true;

        // reset turn counter
        turnCount = 1;

        // reset game state history
        gameStateHistory.Clear();

        // no scenarios to pull from, abort
        if(scenarios.Count == 0)
        {
            Debug.Log("No scenarios available! Aborting!!!");
            Application.Quit();
        }
        else
        {
            SetUpScenario();
        }
    }

    // select a scenario and display its options for players to choose
    void SetUpScenario()
    {
        // handle case of tesseract scenario bringing back the previous scenario
        if(gameStateHistory[gameStateHistory.Count - 1].scenario is Tesseract)
        {
            currentScenario = gameStateHistory[gameStateHistory.Count - 2].scenario;
        }
        // pick new scenario randomly
        else
        {
            // pick from a different pool of scenarios depending on how far into the game we are
            // early game
            if(turnCount == 1)
            {
                // edge case of all early game scenarios being played (impossible if at least one early game scenario is in the general pool)
                if(earlyScenarios.Count == 0)
                {
                    currentScenario = scenarios[UnityEngine.Random.Range(0, scenarios.Count)];
                }
                else
                {
                    scenarioIdx = UnityEngine.Random.Range(0, earlyScenarios.Count);
                    currentScenario = earlyScenarios[scenarioIdx];
                    earlyScenarios.RemoveAt(scenarioIdx);
                }
            }
            // mid game
            else if(turnCount < 5)
            {
                // edge case of all mid game scenarios being played
                if(midScenarios.Count == 0)
                {
                    currentScenario = scenarios[UnityEngine.Random.Range(0, scenarios.Count)];
                }
                else
                {
                    scenarioIdx = UnityEngine.Random.Range(0, midScenarios.Count);
                    currentScenario = midScenarios[scenarioIdx];
                    midScenarios.RemoveAt(scenarioIdx);
                }
            }
            // late game
            else
            {
                // edge case of all late game scenarios being played
                if(lateScenarios.Count == 0)
                {
                    currentScenario = scenarios[UnityEngine.Random.Range(0, scenarios.Count)];
                }
                else
                {
                    scenarioIdx = UnityEngine.Random.Range(0, lateScenarios.Count);
                    currentScenario = lateScenarios[scenarioIdx];
                    lateScenarios.RemoveAt(scenarioIdx);
                }
            }
        }

        currentScenarioOptionCount = currentScenario.actionTitles.Count;

        // fill the option prefabs with this scenario's information
        for (int i = 0; i < currentScenarioOptionCount; i++)
        {
            OptionScript currentOption = optionDisplays[i];
            currentOption.gameObject.SetActive(true);
            currentOption.title = currentScenario.actionTitles[i];
            currentOption.effect = currentScenario.actionEffects[i];
            currentOption.DisplayOption();
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
        // display player targeting inputs
        foreach(PlayerScript delver in delvers)
        {
            delver.playerInputIcons.SetActive(true);
        }
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
            // if any players didn't make choices in time, make a random choice for them and mark them to receive a penalty
            foreach(PlayerScript delver in delvers)
            {
                int randomDelver = UnityEngine.Random.Range(0, currentScenarioOptionCount);
                if (delver.actionIdx == -1) { delver.actionIdx = randomDelver; }
                if(delver.target == null) { delver.target = delvers[UnityEngine.Random.Range(0, delvers.Count)]; }
                delver.choseRandomly = true;
                currentDelver.playerDebugText.text = "Chose Randomly (" + optionIDs[randomDelver] + ")";
            }
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
                currentDelver.playerDebugText.text = "Chose A";
                break;
            case "OptionB":
                if (currentScenarioOptionCount <= 1) { break; }
                currentDelver.actionIdx = 1;
                currentDelver.playerDebugText.text = "Chose B";
                break;
            case "OptionC":
                if (currentScenarioOptionCount <= 2) { break; }
                currentDelver.actionIdx = 2;
                currentDelver.playerDebugText.text = "Chose C";
                break;
            case "OptionD":
                if(currentScenarioOptionCount <= 3) { break; }
                currentDelver.actionIdx = 3;
                currentDelver.playerDebugText.text = "Chose D";
                break;
            case "Confirm":
                Debug.Log("Confirming choice");
                if(currentDelver.actionIdx != -1) { timeRemaining = 0; }
                break;
            case "ThreatAction":
                currentDelver.callToSpirit = true;
                break;
            case "ChoosePlayer1":
                currentDelver.target = delver1;
                break;
            case "ChoosePlayer2":
                currentDelver.target = delver2;
                break;
            case "ChoosePlayer3":
                currentDelver.target = delver3;
                break;
            case "ChoosePlayer4":
                currentDelver.target = delver4;
                break;
            case "ChoosePlayer5":
                currentDelver.target = delver5;
                break;
            case "ChoosePlayer6":
                currentDelver.target = delver6;
                break;
            case "ChoosePlayer7":
                currentDelver.target = delver7;
                break;
            case "ChoosePlayer8":
                currentDelver.target = delver8;
                break;
            default:
                break;
        }
    }

    // work to take players choices and resolve them here, updating threat status/health and point totals and bringing the game to its end state if necesary
    void ResolveTurn()
    {
        // resolve the scenario
        List<PlayerScript> delversSortedByTreasures = delvers.OrderByDescending(obj => obj.treasures).ToList();
        currentScenario.ScenarioResolution(delversSortedByTreasures, firstDelver);

        // set up game state record
        GameStateScript state = new GameStateScript();
        state.scenario = currentScenario;

        // determine if a winner exists and update the game state history
        int maxTreasures = 20;
        PlayerScript triumphantDelver = null;
        foreach(PlayerScript delver in delvers)
        {
            // set up game state for this delver to be recorded
            state.delverChoices[delver] = delver.actionIdx;
            state.delverScores[delver] = delver.treasures;
            state.delverTargets[delver] = delver.target;
            state.spiritCalled[delver] = delver.callToSpirit;
            if(delver.favored)
            {
                state.spiritFavoredID = delver.delverID;
            }
            if(delver.treasures >= maxTreasures)
            {
                maxTreasures = delver.treasures;
                if(!triumphantDelver || triumphantDelver.treasures < maxTreasures || (triumphantDelver.treasures == delver.treasures && delver.favored))
                {
                    triumphantDelver = delver;
                }
            }
        }

        // push game state to history
        gameStateHistory.Add(state);

        // clear all player choices
        foreach(PlayerScript delver in delvers)
        {
            delver.ClearChoices();
        }

        // destroy existing hand's instantiated cards to make way for the next hand
        foreach(CardScript card in GameCanvas.GetComponentsInChildren<CardScript>())
        {
            Destroy(card.gameObject);
        }

        // end game
        if(triumphantDelver)
        {
            // TODO: save game state history?
            Debug.Log(triumphantDelver.name +  " has won the game!");
        }
        // start next round
        else
        {
            turnCount++;
            SetUpScenario();
        }
    }
}