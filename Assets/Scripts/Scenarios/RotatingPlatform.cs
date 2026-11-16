using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "RotatingPlatform", menuName = "Scriptable Objects/RotatingPlatform")]
public class RotatingPlatform : ScenarioScript
{
    public RotatingPlatform()
    {
        scenarioTitle = "The Rotating Platform";

        actionTitles.Add("Spin left");
        actionTitles.Add("Spin right");
        actionTitles.Add("Jump to the center");
        actionTitles.Add("Test the limits of the mechanisms");

        actionEffects.Add("[Rotate point totals to the left. Then] +2");
        actionEffects.Add("[Rotate point totals to the right. Then] +2");
        actionEffects.Add("Pick a delver. Steal 1 from them. [Then steal 1 from each opponent.]");
        actionEffects.Add("[Rotate point totals to the right twice. Then] +3 IF two or fewer delvers pick this.");
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // left
            case 0:
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
            case 1:
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
            case 2:
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
            case 3:
                {
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await RotateTreasures(delversSortedScores, true);
                        await RotateTreasures(delversSortedScores, true);
                    }

                    if(dCount <= 2)
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
