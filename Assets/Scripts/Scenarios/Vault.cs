using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Vault", menuName = "Scriptable Objects/Vault")]
public class Vault : ScenarioScript
{
    public Vault()
    {
        scenarioTitle = "The Vault";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Scavenge the exterior";
        actionEffects[(int)optionDirections.West] = "+2 [Then +2]";
        
        actionTitles[(int)optionDirections.East] = "Pick the lock";
        actionEffects[(int)optionDirections.East] = "+12 divided by the number of delvers who picked this. [Then +3 IF ALL delvers picked this.]";
        
        actionTitles[(int)optionDirections.North] = "Blow the door wide open";
        actionEffects[(int)optionDirections.North] = "+6 IF at least one delver didn't pick this. [Then +1]";
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
                    await TreasureAdjustmentSingle(currentDelver, 12 / eastCount);

                    if (currentDelver.favored && eastCount == delversSortedScores.Count)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            // build
            case optionDirections.North:
                {
                    if (northCount < delversSortedScores.Count)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 6);
                    }

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
