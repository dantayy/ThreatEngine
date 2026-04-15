using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickPocketPavillion", menuName = "Scriptable Objects/PickPocketPavillion")]
public class PickPocketPavillion : ScenarioScript
{
    public PickPocketPavillion()
    {
        scenarioTitle = "The Pickpocket Pavillion";

        actionTitles.Add("Scoop loot from the fountain");
        actionTitles.Add("Hit and run");
        actionTitles.Add("Make room for bigger treasures");

        actionEffects.Add("+3 [+3]");
        actionEffects.Add("Pick a player. Steal 2 from them. [Steal 1 from all other opponents.]");
        actionEffects.Add("+4 [+2] per player who steals from you.");
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
                        TreasureAdjustment(currentDelver, 3);
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add to delver's treasures
                            TreasureAdjustment(currentDelver, 3);
                        }
                        break;
                    }
                // hit and run
                case 1:
                    {
                        // add to delver's treasures
                        TreasureAdjustment(currentDelver, 2);
                        // take from target's treasures
                        TreasureAdjustment(currentDelver.targets[currentDelver.targets.Count - 1], -2);
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add treasures for every other player
                            TreasureAdjustment(currentDelver, delversSortedScores.Count - 2);
                            // take from every other player
                            foreach (PlayerScript target in delversSortedScores)
                            {
                                if (
                                    target.delverID != currentDelver.delverID &&
                                    target.delverID != currentDelver.targets[currentDelver.targets.Count - 1].delverID
                                )
                                {
                                    TreasureAdjustment(target, -1);
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
                            TreasureAdjustment(currentDelver, 4);
                            // favored bonus
                            if (currentDelver.favored)
                            {
                                // add to delver's treasures
                                TreasureAdjustment(currentDelver, 2);
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
