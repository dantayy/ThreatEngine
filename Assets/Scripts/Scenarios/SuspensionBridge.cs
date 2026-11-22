using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SuspensionBridge", menuName = "Scriptable Objects/SuspensionBridge")]
public class SuspensionBridge : ScenarioScript
{
    public SuspensionBridge()
    {
        scenarioTitle = "The Suspension Bridge";
        SetActionCount(2);

        actionTitles[(int)optionDirections.West] = "Walk on the left";
        actionEffects[(int)optionDirections.West] = "+1 per delver who picked B [+1 OR +4 IF every other delver picks B]";

        actionTitles[(int)optionDirections.East] = "Walk on the right";
        actionEffects[(int)optionDirections.East] = "+1 per delver who picked A [+1 OR +4 IF every other delver picks A]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // walk on the left
            case optionDirections.West:
                {
                    // treasures for every delver going on the other side
                    await TreasureAdjustmentSingle(currentDelver, eastCount);
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        // extra bonus if everyone else went right
                        if(westCount == 1)
                        {
                            await ConditionResult(true);
                            await TreasureAdjustmentSingle(currentDelver, 4);
                        }
                        // default bonus
                        else
                        {
                            await ConditionResult(false);
                            await TreasureAdjustmentSingle(currentDelver, 1);
                        }
                    }
                    break;
                }
            // walk on the right
            case optionDirections.East:
                {
                    // treasures for every delver going on the other side
                    await TreasureAdjustmentSingle(currentDelver, westCount);
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        // extra bonus if everyone else went left
                        if(eastCount == 1)
                        {
                            await ConditionResult(true);
                            await TreasureAdjustmentSingle(currentDelver, 4);
                        }
                        // default bonus
                        else
                        {
                            await ConditionResult(false);
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
