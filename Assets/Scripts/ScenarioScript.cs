using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScenarioScript", menuName = "Scriptable Objects/ScenarioScript")]
public abstract class ScenarioScript : ScriptableObject
{
    // flavorful name of the scenario delvers have to overcome
    [field: SerializeField] public string scenarioTitle;
    // flavorful titles of the actions that can be taken in this scenario that will affect scores
    [System.NonSerialized] public List<string> actionTitles = new List<string>();
    // mechanical descriptions of the outcomes of the actions being selected
    // TODO: use something other than the string type so we can write with more than plaintext
    [System.NonSerialized] public List<string> actionEffects = new List<string>();
    // flags set to consider a scenario as either early-game or late-game
    [field: SerializeField] public bool earlyGame = false;
    [field: SerializeField] public bool lateGame = false;

    // events
    public static event Func<PlayerScript, int, Task> OnTreasuresAdded;
    public static event Func<PlayerScript, int, Task> OnTreasuresRemoved;
    public static event Func<List<PlayerScript>, PlayerScript, Task> OnActionResolutionBegan;

    // delver having their scenario action resolved
    protected PlayerScript currentDelver;
    // count of each action being taken
    protected int aCount = 0;
    protected int bCount = 0;
    protected int cCount = 0;
    protected int dCount = 0;
    // time for clocktower
    protected DateTime currentTime;
    // flag set when a delver is favored by the spirit
    protected bool delverFavored;

    // resolve scenario
    public async Task ScenarioResolution(List<PlayerScript> delversSortedScores, PlayerScript delverGoingFirst)
    {
        Debug.Log("Resolving spirit calls");
        await SpiritCallingResolutions(delversSortedScores);
        Debug.Log("Resolving actions for " + name);
        // reset the main tracker vars
        currentDelver = delverGoingFirst;
        aCount = 0;
        bCount = 0;
        cCount = 0;
        dCount = 0;
        delverFavored = false;
        // perform initial pass of delver choices to set trackers up
        foreach(PlayerScript delver in delversSortedScores)
        {
            // set favored flag
            if(delver.favored)
            {
                delverFavored = true;
            }
            // increment choice tracker
            if(delver.actionIdx == 0)
            {
                aCount++;
            }
            else if(delver.actionIdx == 1)
            {
                bCount++;
            }
            else if(delver.actionIdx == 2)
            {
                cCount++;
            }
            else if(delver.actionIdx == 3)
            {
                dCount++;
            }
        }
        // perform unique actions based on the child class' implementation until all delvers have had their actions resolved
        do
        {
            await ActionResolutionBegan(delversSortedScores, currentDelver);
            await ActionResolutions(delversSortedScores);
        }while(currentDelver != delverGoingFirst);
    }

    // resolve delvers' spirit calling choices
    async Task SpiritCallingResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle edge case of empty list of delvers being passed
        if(delversSortedScores.Count == 0)
        {
            return;
        }

        // trackers for whoever is currently favored, if anyone is
        bool spiritFavors = false;
        PlayerScript currentlyFavored = delversSortedScores[0];

        // highest briber
        PlayerScript highestBriber = delversSortedScores[0];
        // highest bribe
        int highestBribe = 0;
        // flag to ensure no dupe highest bribe are accepted
        bool uniqueHighest = true;
        // bribe totals between favored and non-favored
        int nonFavoredBribePool = 0;
        int favoredBribeTotal = delversSortedScores.count - 1;

        // flag set by default to resort players based on score changes at the end
        // unset only in the rare case where no score changes occur
        bool scoreChange = true;

        // loop through all players and check for calling status
        foreach (PlayerScript potentialCaller in delversSortedScores)
        {
            // delver made a bid
            if(potentialCaller.spiritBribe > 0)
            {
                // lose one treasure for offering anything
                // TODO: should this score change happen later?
                await TreasureAdjustment(potentialCaller, -1);
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
                    await TreasureAdjustment(highestBriber, -(highestBriber.spiritBribe - 1));
                }
            }
            // favored wins
            else
            {
                // deduct anything else they offered to keep favor
                if(currentlyFavored.spiritBribe > 0)
                {
                    await TreasureAdjustment(currentlyFavored, -(currentlyFavored.spiritBribe - 1));
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
            await TreasureAdjustment(highestBriber, -(highestBriber.spiritBribe - 1));
        }
        // no one offered anything, no score changes occur
        else if(highestBribe == 0)
        {
            scoreChange = false;
        }

        // re-sort delver score list to reflect the new scores if anything changed
        if(scoreChange)
        {
            delversSortedScores.Sort((a,b) => a.treasures.CompareTo(b.treasures));
        }
    }

    // resolve players' action choices (implement in child scripts)
    protected virtual async Task ActionResolutions(List<PlayerScript> delverSortedScores)
    {
        await Task.CompletedTask;
    }

    protected async Task TreasureAdjustment(PlayerScript delver, int treasureDelta)
    {
        // log change for debugging (WILL slow things down if left on in release)
        Debug.Log("Delver with ID " + delver.delverID + " gets " + treasureDelta + " treasures.");

        if(treasureDelta > 0) { await OnTreasuresAdded?.Invoke(delver, treasureDelta); }
        else if(treasureDelta < 0) { await OnTreasuresRemoved?.Invoke(delver, treasureDelta); }

        delver.treasures += treasureDelta;
    }

    // Trigger the animation event for the beginning of a player's action resolution.
    protected async Task ActionResolutionBegan(List<PlayerScript> delvers, PlayerScript currentDelver)
    {
        // Thread-safe invocation using the null-conditional operator
        await OnActionResolutionBegan?.Invoke(delvers, currentDelver);
    }
}
