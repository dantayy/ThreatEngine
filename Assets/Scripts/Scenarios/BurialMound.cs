using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BurialMound", menuName = "Scriptable Objects/BurialMound")]
public class BurialMound : ScenarioScript
{
    public BurialMound()
    {
        scenarioTitle = "The Burial Mound";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Take what lies above";
        actionEffects[(int)optionDirections.West] = "+5 [Then +1]";

        actionTitles[(int)optionDirections.East] = "Excavate the corpse";
        actionEffects[(int)optionDirections.East] = "+1 OR +10 IF picked by EXACTLY THREE delvers. [Then +1]";

        actionTitles[(int)optionDirections.North] = "Pledge yourself to the spirit";
        actionEffects[(int)optionDirections.North] = "+20 IF no one else picks this. [Then +1]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // take
            case optionDirections.West:
                {
                    // add to delver's treasures
                    await TreasureAdjustmentSingle(currentDelver, 5);
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            // excavate
            case optionDirections.East:
                {
                    if (eastCount == 3)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 10);
                    }
                    else
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            // pledge
            case optionDirections.North:
                {
                    if (northCount == 1)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 20);
                    }
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
