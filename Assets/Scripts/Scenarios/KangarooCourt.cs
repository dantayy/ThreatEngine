using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "KangarooCourt", menuName = "Scriptable Objects/KangarooCourt")]
public class KangarooCourt : ScenarioScript
{
    public KangarooCourt()
    {
        scenarioTitle = "The Kangaroo Court";
        SetActionCount(4);

        actionTitles[(int)optionDirections.West] = "Trapped in the holding cell";
        actionEffects[(int)optionDirections.West] = "-3 [+0 instead]";
        
        actionTitles[(int)optionDirections.East] = "Destroy the scale";
        actionEffects[(int)optionDirections.East] = "+3 to all delvers not in first or last place. [Steal 3 from them instead.]";
        
        actionTitles[(int)optionDirections.North] = "Balance the scale";
        actionEffects[(int)optionDirections.North] = "+5 for last place delver(s) [Then +3]";
        
        actionTitles[(int)optionDirections.South] = "Unbalance the scale";
        actionEffects[(int)optionDirections.South] = "-5 for first place delver(s) [Then +5]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // trapped in the holding cell
            case optionDirections.West:
                {
                    // remove from delver's treasures
                    if(!currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, -3);
                    }
                    break;
                }
            // destroy the scale
            case optionDirections.East:
                {
                    List<PlayerScript> affectedDelvers = new List<PlayerScript>();
                    for (int i = 1; i < delversSortedScores.Count - 1; i++)
                    {
                        int highestTreasureCount = delversSortedScores.First().treasures;
                        int lowestTreasureCount = delversSortedScores.Last().treasures;

                        // manipulate treasures of all delvers not in first or last place
                        if (delversSortedScores[i].treasures != highestTreasureCount && delversSortedScores[i].treasures != lowestTreasureCount)
                        {
                            affectedDelvers.Add(delversSortedScores[i]);
                        }
                    }

                    if (currentDelver.favored)
                    {
                        await StealTreasures(currentDelver, affectedDelvers, 3);
                        break;
                    }
                    await TreasureAdjustments(affectedDelvers, 3);
                    break;
                }
            // balance the scale
            case optionDirections.North:
                {
                    // add treasures to last place delver(s)
                    int lowestTreasureCount = delversSortedScores.Last().treasures;
                    List<PlayerScript> affectedDelvers = new List<PlayerScript>();
                    for(int i = delversSortedScores.Count - 1; i >= 0; i--)
                    {
                        if(delversSortedScores[i].treasures == lowestTreasureCount)
                        {
                            affectedDelvers.Add(delversSortedScores[i]);
                        }
                    }
                    await TreasureAdjustments(affectedDelvers, 5);

                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            // unbalance the scale
            case optionDirections.South:
                {
                    // remove treasures from first place delver(s)
                    int highestTreasureCount = delversSortedScores.First().treasures;
                    List<PlayerScript> affectedDelvers = new List<PlayerScript>();
                    for(int i = 0; i < delversSortedScores.Count; i++)
                    {
                        if(delversSortedScores[i].treasures == highestTreasureCount)
                        {
                            affectedDelvers.Add(delversSortedScores[i]);
                        }
                    }
                    await TreasureAdjustments(affectedDelvers, -5);

                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 5);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
