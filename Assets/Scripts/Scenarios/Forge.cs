using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Forge", menuName = "Scriptable Objects/Forge")]
public class Forge : ScenarioScript
{
    public Forge()
    {
        scenarioTitle = "The Forge";
        SetActionCount(2);

        actionTitles[(int)optionDirections.West] = "Melt down the materials";
        actionEffects[(int)optionDirections.West] = "+6 IF the delver on your RIGHT picked East. [Then +2]";
        
        actionTitles[(int)optionDirections.East] = "Retrieve metal trinkets";
        actionEffects[(int)optionDirections.East] = "+1 OR +3 IF the delver on your LEFT also picked this. [Then +3 IF any delvers picked West.]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // melt down
            case optionDirections.West:
                {
                    if(currentDelver.rightDelver.actionIdx == optionDirections.East)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 6);
                    }

                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // retrieve
            case optionDirections.East:
                {
                    if (currentDelver.leftDelver.actionIdx == optionDirections.East)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    else
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }

                    if(currentDelver.favored && westCount > 0)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
