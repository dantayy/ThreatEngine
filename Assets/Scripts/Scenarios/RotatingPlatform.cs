using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RotatingPlatform", menuName = "Scriptable Objects/RotatingPlatform")]
public class RotatingPlatform : ScenarioScript
{
    public RotatingPlatform()
    {
        scenarioTitle = "The Rotating Platform";
        SetActionCount(4);

        actionTitles[(int)optionDirections.West] = "Spin left";
        actionEffects[(int)optionDirections.West] = "[Rotate point totals to the left. Then] +2";

        actionTitles[(int)optionDirections.East] = "Spin right";
        actionEffects[(int)optionDirections.East] = "[Rotate point totals to the right. Then] +2";

        actionTitles[(int)optionDirections.North] = "Jump to the center";
        actionEffects[(int)optionDirections.North] = "Pick a delver. Steal 1 from them. [Then steal 1 from each opponent.]";

        actionTitles[(int)optionDirections.South] = "Test the limits of the mechanisms";
        actionEffects[(int)optionDirections.South] = "[Rotate point totals to the right twice. Then] +3 IF two or fewer delvers pick this.";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // left
            case optionDirections.West:
                {
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await RotateTreasures(delversSortedScores, false);
                    }

                    await TreasureAdjustmentSingle(currentDelver, 2);
                    break;
                }
            // right
            case optionDirections.East:
                {
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await RotateTreasures(delversSortedScores, true);
                    }

                    await TreasureAdjustmentSingle(currentDelver, 2);
                    break;
                }
            // center
            case optionDirections.North:
                {
                    // steal from one opponent
                    await StealTreasuresSingle(currentDelver, currentDelver.target, 1);

                    // favored bonus
                    if (currentDelver.favored)
                    {
                        List<PlayerScript> affectedDelvers = new List<PlayerScript>();
                        foreach (PlayerScript target in delversSortedScores)
                        {
                            if (target.delverID != currentDelver.delverID)
                            {
                                affectedDelvers.Add(target);
                            }
                        }
                        
                        // steal from all opponents
                        await StealTreasures(currentDelver, affectedDelvers, 1);
                    }
                    break;

                }
            // test the limits
            case optionDirections.South:
                {
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await RotateTreasures(delversSortedScores, true);
                        await RotateTreasures(delversSortedScores, true);
                    }

                    if(southCount <= 2)
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
