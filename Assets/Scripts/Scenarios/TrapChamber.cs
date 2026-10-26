using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer.Center.Common;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapChamber", menuName = "Scriptable Objects/TrapChamber")]
public class TrapChamber : ScenarioScript
{
    public TrapChamber()
    {
        scenarioTitle = "The Trap Chamber";
        
        actionTitles.Add("Lose a finger");
        actionTitles.Add("Burned alive");

        actionEffects.Add("-1 [+2]");
        actionEffects.Add("-3 OR +4 IF all delvers pick this [+1 OR +6 IF all delvers pick this]");

    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // lose a finger
            case 0:
                {
                    await TreasureAdjustmentSingle(currentDelver, -1);
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // burned alive
            case 1:
                {
                    // more treasures if all delvers take on the fire together
                    if(aCount == 0)
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