using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    // resolve scenario
    public void ScenarioResolution(List<PlayerScript> delversSortedScores, PlayerScript delverGoingFirst)
    {
        SpiritCallingResolutions(delversSortedScores);
        ActionResolutions(delversSortedScores, delverGoingFirst);
    }

    // resolve delvers' spirit calling choices
    void SpiritCallingResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle edge case of empty list of delvers being passed
        if(delversSortedScores.Count == 0)
        {
            return;
        }

        // trackers for whoever is currently favored, if anyone is
        bool spiritFavors = false;
        PlayerScript currentlyFavored = delversSortedScores[0];

        // all delvers who called to spirit are held here
        List<PlayerScript> calledToSpirit = new List<PlayerScript>();

        // flag set by default to resort players based on score changes at the end
        // unset only in the rare case where no score changes occur
        bool scoreChange = true;

        // loop through all players and check for calling status
        foreach (PlayerScript potentialCaller in delversSortedScores)
        {
            // delver called to spirit
            if (potentialCaller.callToSpirit)
            {
                calledToSpirit.Add(potentialCaller);
                // reset flag for next round
                potentialCaller.callToSpirit = false;
            }
            // delver is currently favored
            if (potentialCaller.favored)
            {
                currentlyFavored = potentialCaller;
                spiritFavors = true;
            }
        }

        // resolve calling effects
        // delver currently favored
        if (spiritFavors)
        {
            // favored delver calling to the spirit themselves
            if (calledToSpirit.Contains(currentlyFavored))
            {
                // communion from favored alone keeps the spirit attached
                if (calledToSpirit.Count == 1)
                {
                    scoreChange = false;
                }
                // one other delver called to the spirit
                else if (calledToSpirit.Count == 2)
                {
                    foreach (PlayerScript caller in calledToSpirit)
                    {
                        // non-favored delver misdirected by the spirit, loses a few treasures in the process
                        if (!caller.favored)
                        {
                            caller.treasures -= 2;
                            break;
                        }
                    }
                }
                // spirit overwhelmed by cacophany from within and without, abandoning favored and taking many treasures back with them
                else
                {
                    currentlyFavored.treasures -= 5;
                    currentlyFavored.favored = false;
                }
            }
            // favored delver remaining quiet
            else
            {
                // spirit loses connection with favored, takes a few treasures with them as they leave to spectate the rest of the competition
                if (calledToSpirit.Count == 0)
                {
                    currentlyFavored.treasures -= 2;
                    currentlyFavored.favored = false;
                }
                // spirit is swayed by a new singular voice, taking many treasures with them to their new favored
                else if (calledToSpirit.Count == 1)
                {
                    currentlyFavored.treasures -= 5;
                    currentlyFavored.favored = false;

                    calledToSpirit[0].treasures += 5;
                    calledToSpirit[0].favored = true;
                }
                // spirit finds calm in mind of favored when confronted by cacophany of compeititors, causes offenders to lose a few treasures
                else
                {
                    foreach (PlayerScript delver in calledToSpirit)
                    {
                        delver.treasures -= 2;
                    }
                }
            }
        }
        // no delver currently favored
        else
        {
            // spirit remains in shadow, waiting for a greedy delver to commune with them
            if (calledToSpirit.Count == 0)
            {
                scoreChange = false;
            }
            // one voice gains the spirit's favor and a gift of a few treasures
            else if (calledToSpirit.Count == 1)
            {
                calledToSpirit[0].favored = true;
                calledToSpirit[0].treasures += 2;   
            }
            // too many voices call to spirit, causing them to misdirect all who participated and lose a few treasures in the process
            else
            {
                foreach (PlayerScript caller in calledToSpirit)
                {
                    caller.treasures -= 2;
                }
            }
        }

        // re-sort delver score list to reflect the new scores if anything changed
        if(scoreChange)
        {
            delversSortedScores.Sort((a,b) => a.treasures.CompareTo(b.treasures));
        }
    }

    // resolve players' action choices (implement in child scripts)
    protected virtual void ActionResolutions(List<PlayerScript> delverSortedScores, PlayerScript delverGoingFirst) { }
}
