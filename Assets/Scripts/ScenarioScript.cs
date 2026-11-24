using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

// Sets the order of the cardinal directions
public enum optionDirections
{
    None = -1,
    West = 0,
    East = 1,
    North = 2,
    South = 3,
}

[CreateAssetMenu(fileName = "ScenarioScript", menuName = "Scriptable Objects/ScenarioScript")]
public abstract class ScenarioScript : ScriptableObject
{
    // flavorful name of the scenario delvers have to overcome
    [field: SerializeField] public string scenarioTitle;
    // flavorful titles of the actions that can be taken in this scenario that will affect scores
    [System.NonSerialized] public string[] actionTitles;
    // mechanical descriptions of the outcomes of the actions being selected
    // TODO: use something other than the string type so we can write with more than plaintext
    [System.NonSerialized] public string[] actionEffects;

    // events
    public static event Func<PlayerScript, int, Task> OnTreasuresAdded;
    public static event Func<PlayerScript, int, Task> OnTreasuresRemoved;
    public static event Func<PlayerScript, List<PlayerScript>, int, Task> OnTreasuresStolen;
    public static event Func<Task> OnResetDelvers;
    public static event Func<Task> OnSpiritBribingBegan;
    public static event Func<PlayerScript, Task> OnActionResolutionBegan;
    public static event Func<List<PlayerScript>, bool, Task> OnChangePlayerInvolvement;
    public static event Func<List<PlayerScript>, bool, Task> OnChangeIconInvolvement;
    public static event Func<bool, Task> OnConditionResult;

    public static event Func<List<PlayerScript>, bool, Task> OnTreasuresRotated;
    public static event Func<PlayerScript, PlayerScript, Task> OnTreasuresSwapped;
    public static event Func<List<PlayerScript>, PlayerScript, Task> OnRandomDelver;
    public static event Func<bool, Task> OnCoinFlip;

    // delver having their scenario action resolved
    protected PlayerScript currentDelver;

    // count of each action being taken
    protected int westCount = 0;
    protected int eastCount = 0;
    protected int northCount = 0;
    protected int southCount = 0;
    // time for clocktower
    protected DateTime currentTime;
    // flag set when a delver is favored by the spirit
    protected bool aFavoredDelverExists;
    protected PlayerScript delverGoingFirst;

    protected List<PlayerScript> delversInTurnOrder;

    protected List<GameStateScript> gameStateHistory;

    // resolve scenario
    public async Task ScenarioResolution(List<PlayerScript> delvers, PlayerScript currentFirstDelver, List<GameStateScript> currentHistory)
    {
        await ResetDelvers();
        Debug.Log("Resolving spirit calls");
        await SpiritBribing();
        await SpiritBribingResolutions(delvers);
        Debug.Log("Resolving actions for " + name);
        // reset the main tracker vars
        currentDelver = currentFirstDelver;
        delverGoingFirst = currentFirstDelver;
        delversInTurnOrder = delvers;
        westCount = 0;
        eastCount = 0;
        northCount = 0;
        southCount = 0;
        aFavoredDelverExists = false;
        gameStateHistory = currentHistory;
        // perform initial pass of delver choices to set trackers up
        foreach(PlayerScript delver in delvers)
        {
            // set favored flag
            if(delver.favored)
            {
                aFavoredDelverExists = true;
            }
            // increment choice tracker
            if(delver.actionIdx == optionDirections.West)
            {
                westCount++;
            }
            else if(delver.actionIdx == optionDirections.East)
            {
                eastCount++;
            }
            else if(delver.actionIdx == optionDirections.North)
            {
                northCount++;
            }
            else if(delver.actionIdx == optionDirections.South)
            {
                southCount++;
            }
        }
        // perform unique actions based on the child class' implementation until all delvers have had their actions resolved
        do
        {
            // recalculate delver order by score between each action resolution
            List<PlayerScript> delversSortedScores = delvers.OrderByDescending(obj => obj.treasures).ToList();
            await ActionResolutionBegan(currentDelver);
            await ActionResolutions(delversSortedScores);

            // move to next delver in turn order
            currentDelver = currentDelver.rightDelver;
        } while(currentDelver != delverGoingFirst);
    }

    // resolve delvers' spirit calling choices
    async Task SpiritBribingResolutions(List<PlayerScript> delvers)
    {
        // handle edge case of empty list of delvers being passed
        if(delvers.Count == 0)
        {
            return;
        }

        // trackers for whoever is currently favored, if anyone is
        bool spiritFavors = false;
        PlayerScript currentlyFavored = delvers[0];

        // highest briber
        PlayerScript highestBriber = delvers[0];
        // highest bribe
        int highestBribe = 0;
        // flag to ensure no dupe highest bribe are accepted
        bool uniqueHighest = true;
        // bribe totals between favored and non-favored
        int nonFavoredBribePool = 0;
        int favoredBribeTotal = delvers.Count - 1;

        // flag set by default to resort players based on score changes at the end
        // unset only in the rare case where no score changes occur
        bool scoreChange = true;

        // loop through all players and check for calling status
        foreach (PlayerScript potentialCaller in delvers)
        {
            // delver made a bid
            if(potentialCaller.spiritBribe > 0)
            {
                // lose one treasure for offering anything
                // TODO: should this score change happen later?
                await TreasureAdjustmentSingle(potentialCaller, -1);
                // existing favored bid
                if(potentialCaller.favored)
                {
                    // favored exists this round
                    spiritFavors = true;
                    currentlyFavored = potentialCaller;
                    // update favored bribe total
                    favoredBribeTotal += potentialCaller.spiritBribe;
                }
                // non favored bid
                else
                {
                    // add bribe to the pool against the favored
                    nonFavoredBribePool += potentialCaller.spiritBribe;
                    // check for new highest bidder
                    if(potentialCaller.spiritBribe > highestBribe)
                    {
                        // update highest bidder
                        highestBriber = potentialCaller;
                        highestBribe = potentialCaller.spiritBribe;
                        uniqueHighest = true;
                    }
                    // check for dupe highest bid
                    else if(potentialCaller.spiritBribe == highestBribe)
                    {
                        uniqueHighest = false;
                    }
                }
            }
        }

        // if existing favored, compare their bribe to the unfavored pool
        if(spiritFavors)
        {
            // non-favored beat the favored
            if(nonFavoredBribePool > favoredBribeTotal)
            {
                // take favored away from the current favored
                currentlyFavored.favored = false;
                // favor is immediately transfered to highest bidder amongst formerly non-favored 
                if(uniqueHighest)
                {
                    highestBriber.favored = true;
                    await TreasureAdjustmentSingle(highestBriber, -(highestBriber.spiritBribe - 1));
                }
            }
            // favored wins
            else
            {
                // deduct anything else they offered to keep favor
                if(currentlyFavored.spiritBribe > 0)
                {
                    await TreasureAdjustmentSingle(currentlyFavored, -(currentlyFavored.spiritBribe - 1));
                }
                // nobody bid anything, no score change occurs
                else if(nonFavoredBribePool == 0)
                {
                    scoreChange = false;
                }
            }
        }
        // no one currently favored, just see if anyone has the highest bribe
        else if(uniqueHighest && highestBribe > 0)
        {
            // set the flag
            highestBriber.favored = true;
            // remove the rest of their bribe on top of the one already paid in the main loop
            await TreasureAdjustmentSingle(highestBriber, -(highestBriber.spiritBribe - 1));
        }
        // no one offered anything, no score changes occur
        else if(highestBribe == 0)
        {
            scoreChange = false;
        }

        // re-sort delver score list to reflect the new scores if anything changed
        if(scoreChange)
        {
            delvers.Sort((a,b) => a.treasures.CompareTo(b.treasures));
        }
    }

    // resolve players' action choices (implement in child scripts)
    protected virtual async Task ActionResolutions(List<PlayerScript> delvers)
    {
        await Task.CompletedTask;
    }

    // set up arrays based on the number of available actions in the scenario.
    protected void SetActionCount(int actionCount)
    {
        actionTitles = new string[actionCount];
        actionEffects = new string[actionCount];
    }

    // Trigger the event for resetting delver symbology
    protected async Task ResetDelvers()
    {
        await OnResetDelvers?.Invoke();
    }

    // Trigger the event for highlighting delvers that are bribing the spirit
    protected async Task SpiritBribing()
    {
        await OnSpiritBribingBegan?.Invoke();
    }

    // Version of treasureAdjustments that takes in a single delver for convenience.
    protected async Task TreasureAdjustmentSingle(PlayerScript delver, int treasureDelta)
    {
        List<PlayerScript> delvers = new List<PlayerScript>() { delver };
        await TreasureAdjustments(delvers, treasureDelta);
    }

    // Trigger the event for a player gaining / losing treasures.
    protected async Task TreasureAdjustments(List<PlayerScript> delvers, int treasureDelta)
    {
        await ChangePlayerInvolvement(delvers, true);
        
        List<Task> tasks = new List<Task>();
        foreach (PlayerScript delver in delvers)
        {
            // log change for debugging (WILL slow things down if left on in release)
            Debug.Log("Delver with ID " + delver.delverID + " gets " + treasureDelta + " treasures.");
            if (treasureDelta > 0) { tasks.Add(OnTreasuresAdded?.Invoke(delver, treasureDelta)); }
            else if (treasureDelta < 0) { tasks.Add(OnTreasuresRemoved?.Invoke(delver, treasureDelta)); }

            delver.treasures += treasureDelta;
        }
        await Task.WhenAll(tasks);
    }

    // Version of stealTreasures that takes in a single delver for convenience.
    protected async Task StealTreasuresSingle(PlayerScript thievingDelver, PlayerScript targetDelver, int treasureDelta)
    {
        List<PlayerScript> delvers = new List<PlayerScript>() { targetDelver };
        await StealTreasures(thievingDelver, delvers, treasureDelta);
    }

    // Trigger the event for a player stealing treasures from other players.
    protected async Task StealTreasures(PlayerScript thievingDelver, List<PlayerScript> affectedDelvers, int treasureDelta)
    {
        await ChangePlayerInvolvement(affectedDelvers, true);

        Debug.Log("Treasures stolen: " + treasureDelta);
        Debug.Log("Number of affected delvers: " + affectedDelvers.Count);
        
        await OnTreasuresStolen?.Invoke(thievingDelver, affectedDelvers, treasureDelta);
        
        foreach (PlayerScript delver in affectedDelvers) { delver.treasures -= treasureDelta; }
        thievingDelver.treasures += treasureDelta * affectedDelvers.Count;
    }

    // Trigger the event for the beginning of a delver's action resolution.
    protected async Task ActionResolutionBegan(PlayerScript currentDelver)
    {
        // Thread-safe invocation using the null-conditional operator
        await OnActionResolutionBegan?.Invoke(currentDelver);
    }

    // Trigger the event for delver(s) being involved / uninvolved in a resolution effect.
    protected async Task ChangePlayerInvolvement(List<PlayerScript> involvedDelvers, bool involve = true)
    {
        await OnChangePlayerInvolvement?.Invoke(involvedDelvers, involve);
    }

    // Trigger the event for player choice icon(s) being involved / uninvolved in a resolution effect.
    protected async Task ChangeIconInvolvement(List<PlayerScript> involvedDelvers, bool involve = true)
    {
        await OnChangeIconInvolvement?.Invoke(involvedDelvers, involve);
    }

    // Trigger the event for a conditional effect succeeding or failing.
    protected async Task ConditionResult(bool success = true)
    {
        await OnConditionResult?.Invoke(success);
    }

    // Trigger the event for all treasure totals shifting left or right.
    protected async Task RotateTreasures(List<PlayerScript> delvers, bool rotateRight = true)
    {
        await ChangePlayerInvolvement(delvers, true);

        // Store treasure totals for rotation
        int[] treasureTotals = new int[delvers.Count];
        foreach (PlayerScript delver in delvers)
        {
            treasureTotals[delver.delverID - 1] = delver.treasures;
        }

        // Rotate totals left or right
        if (rotateRight)
        {
            foreach (PlayerScript delver in delvers)
            {
                delver.treasures = treasureTotals[delver.leftDelver.delverID - 1];
            }
        }
        else
        {
            foreach (PlayerScript delver in delvers)
            {
                delver.treasures = treasureTotals[delver.rightDelver.delverID - 1];
            }
        }

        await OnTreasuresRotated?.Invoke(delvers, rotateRight);
    }

    // Trigger the event for switching the treasure totals of two delvers.
    protected async Task SwapTreasures(PlayerScript delver1, PlayerScript delver2)
    {
        List<PlayerScript> affectedDelvers = new List<PlayerScript>() { delver1, delver2 };
        await ChangePlayerInvolvement(affectedDelvers);

        int delver1Total = delver1.treasures;
        delver1.treasures = delver2.treasures;
        delver2.treasures = delver1Total;

        await OnTreasuresSwapped(delver1, delver2);
    }

    // Trigger the event for picking a delver at random for the purpose of another effect.
    protected async Task ChooseRandomDelver(List<PlayerScript> affectedDelvers, PlayerScript chosenDelver)
    {
        await ChangePlayerInvolvement(affectedDelvers);
        await OnRandomDelver?.Invoke(affectedDelvers, chosenDelver);
    }

    // Trigger the event for a coin being flipped.
    protected async Task FlipCoin(bool heads)
    {
        await OnCoinFlip?.Invoke(heads);
    }
}
