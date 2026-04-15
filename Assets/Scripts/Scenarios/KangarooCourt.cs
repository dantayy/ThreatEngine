using System.Collections.Generic;
using System.Linq;
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
    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // trackers for highest and lowest scores
        int highestTreasures = delversSortedScores.First().treasures;
        int lowestTreasures = delversSortedScores.Last().treasures;
        // look at each delver's action choice
        PlayerScript currentDelver = firstDelver;
        do
        {
            // handle each possible action choice
            switch (currentDelver.actionIdx)
            {
                // trapped in the holding cell
                case 0:
                    {
                        // remove from delver's treasures
                        TreasureAdjustment(currentDelver, -3);
                        break;
                    }
                // balance the scale
                case 1:
                    {
                        // add treasures to last place delver(s)
                        for(int i = delversSortedScores.Count - 1; i >= 0; i--)
                        {
                            if(delversSortedScores[i].treasures == lowestTreasures)
                            {
                                TreasureAdjustment(delversSortedScores[i], 5);
                            }
                            else
                            {
                                break;
                            }
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 3);
                        }
                        break;
                    }
                // unbalance the scale
                case 2:
                    {
                        // remove treasures from first place delver(s)
                        for(int i = 0; i < delversSortedScores.Count; i++)
                        {
                            if(delversSortedScores[i].treasures == highestTreasures)
                            {
                                TreasureAdjustment(delversSortedScores[i], -5);
                            }
                            else
                            {
                                break;
                            }
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 5);
                        }
                        break;
                    }
                case 3:
                    {
                        // manipulate treasures of all delvers not in first or last place
                        for(int i = 1; i < delversSortedScores.Count - 1; i++)
                        {
                            if(delversSortedScores[i].treasures != highestTreasures && delversSortedScores[i].treasures != lowestTreasures)
                            {
                                // steal from everyone in the middle when favored
                                if(currentDelver.favored)
                                {
                                    TreasureAdjustment(delversSortedScores[i], -3);
                                    TreasureAdjustment(currentDelver, 3);
                                }
                                // boost everyone in the middle
                                else
                                {
                                    TreasureAdjustment(delversSortedScores[i], 3);
                                }
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            // re-sort delver scores list
            delversSortedScores.Sort((a,b) => a.treasures.CompareTo(b.treasures));
            // reset highest and lowest score trackers
            highestTreasures = delversSortedScores.First().treasures;
            lowestTreasures = delversSortedScores.Last().treasures;
            // move to next delver in turn order
            currentDelver = currentDelver.rightDelver;
        } while (currentDelver != firstDelver);
    }
}