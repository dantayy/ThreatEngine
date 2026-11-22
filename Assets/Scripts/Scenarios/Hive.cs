using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Hive", menuName = "Scriptable Objects/Hive")]
public class Hive : ScenarioScript
{
    public Hive()
    {
        scenarioTitle = "The Hive";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Sneak left";
        actionEffects[(int)optionDirections.West] = "+2 IF North AND East are also picked. [Then +1]";
        
        actionTitles[(int)optionDirections.East] = "Sneak right";
        actionEffects[(int)optionDirections.East] = "+2 IF North AND West are also picked [Then +1]";
        
        actionTitles[(int)optionDirections.North] = "Sneak down the middle";
        actionEffects[(int)optionDirections.North] = "+2 IF West AND East are also picked [Then +1]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // sneak left
            case optionDirections.West:
                {
                    // give treasures if others went down the other paths
                    if(eastCount > 0 && northCount > 0)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            // sneak right
            case optionDirections.East:
                {
                    // give treasures if others went down the other paths
                    if(westCount > 0 && eastCount > 0)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            // sneak down the middle
            case optionDirections.North:
                {
                    // give treasures if others went down the other paths
                    if (westCount > 0 && northCount > 0)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
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
