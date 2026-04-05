using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickPocketPavillion", menuName = "Scriptable Objects/PickPocketPavillion")]
public class PickPocketPavillion : ScenarioScript
{
    public PickPocketPavillion()
    {
        scenarioTitle = "The Pickpocket Pavillion";
        actionTitles[0] = "Scoop loot from the fountain";
        actionTitles[1] = "Hit and run";
        actionTitles[2] = "Make room for bigger treasures";
        actionEffects[0] = "+3 [+3]";
        actionEffects[1] = "Pick a player. Steal 2 from them. [Steal 1 from all other opponents.]";
        actionEffects[2] = "+4 [+2] per player who steals from you.";
    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // look at each delver's action choice
        PlayerScript currentDelver = firstDelver;
        do
        {
            // handle each possible action choice
            switch (currentDelver.actionIdx)
            {
                // scoop loot from the fountain
                case 0:
                    {
                        // add to delver's treasures
                        currentDelver.treasures += 3;
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add to delver's treasures
                            currentDelver.treasures += 3;
                        }
                        break;
                    }
                // hit and run
                case 1:
                    {
                        // add to delver's treasures
                        currentDelver.treasures += 2;
                        // take from target's treasures
                        currentDelver.targets[currentDelver.targets.Count - 1].treasures -= 2;
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add treasures for every other player
                            currentDelver.treasures += delversSortedScores.Count - 2;
                            // take from every other player
                            foreach (PlayerScript target in delversSortedScores)
                            {
                                if (
                                    target.delverID != currentDelver.delverID &&
                                    target.delverID != currentDelver.targets[currentDelver.targets.Count - 1].delverID
                                )
                                {
                                    target.treasures--;
                                }
                            }
                        }
                        break;
                    }
                // make room for bigger treasures
                case 2:
                    // determine who has stolen from this delver
                    foreach (PlayerScript potentialThief in delversSortedScores)
                    {
                        if (potentialThief.targets[potentialThief.targets.Count - 1] == currentDelver)
                        {
                            // add treasures when stolen from
                            currentDelver.treasures += 4;
                            // favored bonus
                            if (currentDelver.favored)
                            {
                                // add to delver's treasures
                                currentDelver.treasures += 2;
                            }
                        }
                    }
                    break;
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
