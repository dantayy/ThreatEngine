using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Marsh", menuName = "Scriptable Objects/Marsh")]
public class Marsh : ScenarioScript
{
    public Marsh()
    {
        scenarioTitle = "The Marsh";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Trudge through";
        actionEffects[(int)optionDirections.West] = "+2 [Then +2]";
        
        actionTitles[(int)optionDirections.East] = "Climb the dying trees";
        actionEffects[(int)optionDirections.East] = "+5 IF only you pick this. [Then +2]";
        
        actionTitles[(int)optionDirections.North] = "Build a bridge";
        actionEffects[(int)optionDirections.North] = "+1 OR +5 IF ALL delvers pick this. [Then +2]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // trudge
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, 2);

                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // climb
            case optionDirections.East:
                {
                    if(eastCount == 1)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 5);
                    }

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // build
            case optionDirections.North:
                {
                    if (northCount == delversSortedScores.Count)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 5);
                    }
                    else
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
