using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "WindTunnel", menuName = "Scriptable Objects/WindTunnel")]
public class WindTunnel : ScenarioScript
{
    public WindTunnel()
    {
        scenarioTitle = "The Wind Tunnel";
        SetActionCount(4);

        actionTitles[(int)optionDirections.West] = "Proceed with caution";
        actionEffects[(int)optionDirections.West] = "+1 [Then +2]";
        
        actionTitles[(int)optionDirections.East] = "Let the wind carry you";
        actionEffects[(int)optionDirections.East] = "-2 [Then +0]";
        
        actionTitles[(int)optionDirections.North] = "Link arms";
        actionEffects[(int)optionDirections.North] = "Set your treasures equal to second place delver. [Set your treasures equal to first place delver instead.]";

        actionTitles[(int)optionDirections.South] = "Follow the leader";
        actionEffects[(int)optionDirections.South] = "1st place delver gets -3. [Then you get +3]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // proceed
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, 1);

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // wind
            case optionDirections.East:
                {
                    await TreasureAdjustmentSingle(currentDelver, -2);
                    break;
                }
            // link
            case optionDirections.North:
                {
                    PlayerScript delverBeingCopied = delversSortedScores[1];
                    if (currentDelver.favored)
                    {
                        delverBeingCopied = delversSortedScores[0];
                    }
                    await TreasureAdjustmentSingle(currentDelver, delverBeingCopied.treasures - currentDelver.treasures);
                    break;
                }
            // follow
            case optionDirections.South:
                {
                    await TreasureAdjustmentSingle(delversSortedScores[0], -3);

                    if (currentDelver.favored)
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
