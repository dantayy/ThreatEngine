using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cathedral", menuName = "Scriptable Objects/Cathedral")]
public class Cathedral : ScenarioScript
{
    public Cathedral()
    {
        scenarioTitle = "The Cathedral";
        
        actionTitles.Add("Pray alone");
        actionTitles.Add("Take from the offering bin");
        actionTitles.Add("Break the marble effigy together");

        actionEffects.Add("+5 [+3] IF only you choose this");
        actionEffects.Add("+1 [+2]");
        actionEffects.Add("+4 [+2] IF everyone chooses this");
    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // flag to unset when more than one delver takes the prayer action, nullifying treasures gained for all who picked it
        bool prayingAlone = true;
        // flag to set when everyone is breaking the effigy together
        bool breakingTogether = false;
        // flag to set when we've already checked to see if everyone is breaking the effigy together
        bool breakCheck = false;
        // look at each delver's action choice
        PlayerScript currentDelver = firstDelver;
        do
        {
            // handle each possible action choice
            switch (currentDelver.actionIdx)
            {
                // pray alone
                case 0:
                    {
                        // check if we've already found other prayer actions
                        if(!prayingAlone)
                        {
                            break;
                        }
                        // determine who else may be praying
                        foreach (PlayerScript potentialPatron in delversSortedScores)
                        {
                            // more than one person praying, none who choose this action will be rewarded
                            if (potentialPatron.actionIdx == currentDelver.actionIdx && potentialPatron != currentDelver)
                            {
                                prayingAlone = false;
                                break;
                            }
                        }
                        // this delver IS the only one praying, dole out the treasures
                        if(prayingAlone)
                        {
                            // add to delver's treasures
                            currentDelver.treasures += 5;
                            // favored bonus
                            if (currentDelver.favored)
                            {
                                // add to delver's treasures
                                currentDelver.treasures += 3;
                            }
                        }
                        // can't be breaking the effigy together if someone is praying alone
                        breakCheck = true;
                        break;
                    }
                // take from the offering bin
                case 1:
                    {
                        // add to delver's treasures
                        currentDelver.treasures += 1;
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            currentDelver.treasures += 2;
                        }
                        // can't be breaking the effigy together if someone is taking from the offering bin
                        breakCheck = true;
                        break;
                    }
                // Break the marble effigy together
                case 2:
                    {
                        // check once if unity of choice has not yet been established
                        if(!breakingTogether)
                        {
                            // we've already run this check for unity
                            if(breakCheck)
                            {
                                break;
                            }
                            // determine if anyone isn't breaking the effigy
                            foreach (PlayerScript potentialBreaker in delversSortedScores)
                            {
                                // someone isn't working to break the effigy, noone who chooses this action will be rewarded
                                if (potentialBreaker.actionIdx != currentDelver.actionIdx)
                                {
                                    breakCheck = true;
                                    break;
                                }
                            }
                            // noone deviated from the united path, set both flags
                            if(!breakCheck)
                            {
                                breakingTogether = true;
                                breakCheck = true;
                            }
                        }
                        // EVERYONE is working to break the effigy together, dole out treasures
                        if(breakingTogether)
                        {
                            // add to delver's treasures
                            currentDelver.treasures += 4;
                            // favored bonus
                            if (currentDelver.favored)
                            {
                                // add to delver's treasures
                                currentDelver.treasures += 2;
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            // re-sort delver scores list
            delversSortedScores.Sort((a,b) => a.treasures.CompareTo(b.treasures));

            // move to next delver in turn order
            currentDelver = currentDelver.rightDelver;
        } while (currentDelver != firstDelver);
    }
}