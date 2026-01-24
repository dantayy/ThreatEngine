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
    }

    protected override void ActionResolutions(List<PlayerScript> delvers, PlayerScript firstDelver)
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
                            currentDelver.treasures += delvers.Count - 2;
                            // take from every other player
                            foreach (PlayerScript target in delvers)
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
                case 2:
                    // determine who has stolen from this delver
                    foreach (PlayerScript potentialThief in delvers)
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
            currentDelver = currentDelver.rightDelver;
        } while (currentDelver != firstDelver);
    }
}
