using System.Collections.Generic;
using System.Threading.Tasks;
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

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // scoop loot from the fountain
            case 0:
                {
                    // add to delver's treasures
                    await TreasureAdjustmentSingle(currentDelver, 3);
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        // add to delver's treasures
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            // hit and run
            case 1:
                {
                    // steal treasures from target
                    await StealTreasuresSingle(currentDelver, currentDelver.target, 2);

                    // favored bonus
                    if (currentDelver.favored)
                    {
                        List<PlayerScript> affectedDelvers = new List<PlayerScript>();

                        // take from every other player
                        foreach (PlayerScript target in delversSortedScores)
                        {
                            if (
                                target.delverID != currentDelver.delverID &&
                                target.delverID != currentDelver.target.delverID
                            )
                            {
                                affectedDelvers.Add(target);
                            }
                        }
                        await StealTreasures(currentDelver, affectedDelvers, 1);
                    }
                    break;
                }
            // make room for bigger treasures
            case 2:
                // determine who has stolen from this delver
                foreach (PlayerScript potentialThief in delversSortedScores)
                {
                    if (potentialThief.target == currentDelver)
                    {
                        // add treasures when stolen from
                        await TreasureAdjustmentSingle(currentDelver, 4);
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add to delver's treasures
                            await TreasureAdjustmentSingle(currentDelver, 2);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
}
