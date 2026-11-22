using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ReflectingPool", menuName = "Scriptable Objects/ReflectingPool")]
public class ReflectingPool : ScenarioScript
{
    public ReflectingPool()
    {
        scenarioTitle = "The Reflecting Pool";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Quick dip";
        actionEffects[(int)optionDirections.West] = "+2 [Then +2]";
        
        actionTitles[(int)optionDirections.East] = "Swim to the other side";
        actionEffects[(int)optionDirections.East] = "+6 IF no delvers picked North. [Then +3 IF at least one delver picked West.]";
        
        actionTitles[(int)optionDirections.North] = "Drain the pool and scrounge for coins";
        actionEffects[(int)optionDirections.North] = "+4 IF only you picked this. [Then +2]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // dip
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, 2);

                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // swim
            case optionDirections.East:
                {
                    if(northCount == 0)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 6);
                    }

                    if (currentDelver.favored && westCount >= 1)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            // drain
            case optionDirections.North:
                {
                    if (northCount == 1)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 4);
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
