using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "KangarooCourt", menuName = "Scriptable Objects/KangarooCourt")]
public class KangarooCourt : ScenarioScript
{
    public KangarooCourt()
    {
        scenarioTitle = "The Kangaroo Court";
        actionTitles[0] = "Lift the gavel";
        actionTitles[1] = "Trapped in the holding cell";
        actionTitles[2] = "Balance the scale";
        actionTitles[3] = "Unbalance the scale";
        actionTitles[4] = "Destroy the scale";
        actionEffects[0] = "+1 [+4]";
        actionEffects[1] = "-3 [0]";
        actionEffects[2] = "+5 for last place delver(s) [+3]";
        actionEffects[3] = "-5 for first place delver(s) [+5]";
        actionEffects[4] = "+3 to all delvers not in first or last place [Steal 3 from them instead]";
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
                // lift the gavel
                case 0:
                    {
                        // add to delver's treasures
                        currentDelver.treasures += 1;
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add to delver's treasures
                            currentDelver.treasures += 4;
                        }
                        break;
                    }
                // trapped in the holding cell
                case 1:
                    {
                        // add to delver's treasures
                        currentDelver.treasures -= 3;
                        break;
                    }
                // balance the scale
                case 2:
                    {
                        // add treasures to last place delver(s)
                        for(int i = delversSortedScores.Count - 1; i >= 0; i--)
                        {
                            if(delversSortedScores[i].treasures == lowestTreasures)
                            {
                                delversSortedScores[i].treasures += 5;
                            }
                            else
                            {
                                break;
                            }
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            currentDelver.treasures += 3;
                        }
                        break;
                    }
                // unbalance the scale
                case 3:
                    {
                        // remove treasures from first place delver(s)
                        for(int i = 0; i < delversSortedScores.Count; i++)
                        {
                            if(delversSortedScores[i].treasures == highestTreasures)
                            {
                                delversSortedScores[i].treasures -= 5;
                            }
                            else
                            {
                                break;
                            }
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            currentDelver.treasures += 5;
                        }
                        break;
                    }
                case 4:
                    {
                        // manipulate treasures of all delvers not in first or last place
                        for(int i = 0; i < delversSortedScores.Count; i++)
                        {
                            if(delversSortedScores[i].treasures != highestTreasures && delversSortedScores[i].treasures != lowestTreasures)
                            {
                                // steal from everyone in the middle when favored
                                if(currentDelver.favored)
                                {
                                    delversSortedScores[i].treasures -= 3;
                                    currentDelver.treasures += 3;
                                }
                                // boost everyone in the middle
                                else
                                {
                                    delversSortedScores[i].treasures += 3;
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
