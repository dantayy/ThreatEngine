using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "PickPocketPavillion", menuName = "Scriptable Objects/PickPocketPavillion")]
public class PickPocketPavillion : ScenarioScript
{
    public PickPocketPavillion()
    {
        scenarioTitle = "The Pickpocket Pavillion";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Scoop loot from the fountain";
        actionEffects[(int)optionDirections.West] = "+3 [+3]";
        
        actionTitles[(int)optionDirections.East] = "Make room for bigger treasures";
        actionEffects[(int)optionDirections.East] = "+4 [+2] per player who steals from you.";

        actionTitles[(int)optionDirections.North] = "Hit and run";
        actionEffects[(int)optionDirections.North] = "Pick a player. Steal 2 from them. [Steal 1 from all other opponents.]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // scoop loot from the fountain
            case optionDirections.West:
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
            // make room for bigger treasures
            case optionDirections.East:
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
            // hit and run
            case optionDirections.North:
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
            default:
                break;
        }
    }
}
