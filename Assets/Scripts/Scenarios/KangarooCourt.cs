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

        actionTitles.Add("Trapped in the holding cell");
        actionTitles.Add("Balance the scale");
        actionTitles.Add("Unbalance the scale");
        actionTitles.Add("Destroy the scale");

        actionEffects.Add("-3 [0]");
        actionEffects.Add("+5 for last place delver(s) [+3]");
        actionEffects.Add("-5 for first place delver(s) [+5]");
        actionEffects.Add("+3 to all delvers not in first or last place [Steal 3 from them instead]");

        lateGame = true;
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // trapped in the holding cell
            case 0:
                {
                    // remove from delver's treasures
                    if(!currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, -3);
                    }
                    break;
                }
            // balance the scale
            case 1:
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
            case 2:
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
            // destroy the scale
            case 3:
                {
                    List<PlayerScript> affectedDelvers = new List<PlayerScript>();
                    for(int i = 1; i < delversSortedScores.Count - 1; i++)
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
            default:
                break;
        }
    }
}