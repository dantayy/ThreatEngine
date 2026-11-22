using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapChamber", menuName = "Scriptable Objects/TrapChamber")]
public class TrapChamber : ScenarioScript
{
    public TrapChamber()
    {
        scenarioTitle = "The Trap Chamber";
        SetActionCount(2);

        actionTitles[(int)optionDirections.West] = "Lose a finger";
        actionEffects[(int)optionDirections.West] = "-1 [Then +2]";

        actionTitles[(int)optionDirections.East] = "Burned alive";
        actionEffects[(int)optionDirections.East] = "-3 OR +4 IF all delvers pick this. [Then +1 OR +6 IF all delvers pick this.]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // lose a finger
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, -1);
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // burned alive
            case optionDirections.East:
                {
                    // more treasures if all delvers take on the fire together
                    if(westCount == 0)
                    {
                        await ConditionResult(true);
                        await TreasureAdjustmentSingle(currentDelver, 4);
                        if(currentDelver.favored)
                        {
                            await TreasureAdjustmentSingle(currentDelver, 6);
                        }
                    }
                    // negative treasures without unity
                    else
                    {
                        await ConditionResult(false);
                        await TreasureAdjustmentSingle(currentDelver, -3);
                        if(currentDelver.favored)
                        {
                            await TreasureAdjustmentSingle(currentDelver, 1);
                        }
                    }
                    break;
                }
            default:
                break;
        }
    }
}
