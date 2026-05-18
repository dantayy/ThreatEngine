using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hive", menuName = "Scriptable Objects/Hive")]
public class Hive : ScenarioScript
{
    public Hive()
    {
        scenarioTitle = "The Hive";
        
        actionTitles.Add("Sneak left");
        actionTitles.Add("Sneak down the middle");
        actionTitles.Add("Sneak right");

        actionEffects.Add("+2 IF B AND C are also picked. [+1]");
        actionEffects.Add("+2 IF A AND C are also picked [+1]");
        actionEffects.Add("+2 IF A AND B are also picked [+1]");
    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // vars for tracking how many delvers went each way
        int leftCount = 0;
        int middleCount = 0;
        int rightCount = 0;

        // perform initial pass of delver choices to determine how many went left and how many went right
        foreach(PlayerScript delver in delversSortedScores)
        {
            if(delver.actionIdx == 0)
            {
                leftCount++;
            }
            else if(delver.actionIdx == 1)
            {
                middleCount++;
            }
            else if(delver.actionIdx == 2)
            {
                rightCount++;
            }
        }

        // look at each delver's action choice
        PlayerScript currentDelver = firstDelver;
        do
        {
            // handle each possible action choice
            switch (currentDelver.actionIdx)
            {
                // sneak left
                case 0:
                    {
                        // give treasures if others went down the other paths
                        if(middleCount > 0 && rightCount > 0)
                        {
                            TreasureAdjustment(currentDelver, 2);
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 1);
                        }
                        break;
                    }
                // sneak down the middle
                case 1:
                    {
                        // give treasures if others went down the other paths
                        if(leftCount > 0 && rightCount > 0)
                        {
                            TreasureAdjustment(currentDelver, 2);
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 1);
                        }
                        break;
                    }
                // sneak right
                case 2:
                    {
                        // give treasures if others went down the other paths
                        if(leftCount > 0 && middleCount > 0)
                        {
                            TreasureAdjustment(currentDelver, 2);
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 1);
                        }
                        break;
                    }
                default:
                    break;
            }

            // re-sort delver scores list
            delversSortedScores.Sort((a,b) => a.treasures.CompareTo(b.treasures));

            // move to next delver in turn order
            currentDelver = currentDelver.rightDelver;
        } while (currentDelver != firstDelver);
    }
}