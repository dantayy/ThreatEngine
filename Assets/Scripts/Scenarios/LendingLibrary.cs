using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "LendingLibrary", menuName = "Scriptable Objects/LendingLibrary")]
public class LendingLibrary : ScenarioScript
{
    public LendingLibrary()
    {
        scenarioTitle = "The Lending Library";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Ignore the illusions";
        actionEffects[(int)optionDirections.West] = "+2 [Then +2]";
        
        actionTitles[(int)optionDirections.East] = "Topple the shelves behind you";
        actionEffects[(int)optionDirections.East] = "Pick a delver. They get -4. [All your opponents get -4 instead.]";
        
        actionTitles[(int)optionDirections.North] = "Read aloud";
        actionEffects[(int)optionDirections.North] = "+4 [then +2] AND +1 to each opponent.";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // ignore
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, 2);

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // topple
            case optionDirections.East:
                {
                    if (currentDelver.favored)
                    {
                        List<PlayerScript> affectedDelvers = delversSortedScores;
                        affectedDelvers.Remove(currentDelver);

                        await TreasureAdjustments(affectedDelvers, -4);
                    }
                    else
                    {
                        await TreasureAdjustmentSingle(currentDelver.target, -4);
                    }

                    break;
                }
            // read
            case optionDirections.North:
                {
                    await TreasureAdjustmentSingle(currentDelver, 4);

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }

                    List<PlayerScript> affectedDelvers = delversSortedScores;
                    affectedDelvers.Remove(currentDelver);

                    await TreasureAdjustments(affectedDelvers, 1);

                    break;
                }
            default:
                break;
        }
    }
}
