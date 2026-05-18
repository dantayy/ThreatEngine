using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HallOfMirrors", menuName = "Scriptable Objects/HallOfMirrors")]
public class HallOfMirrors : ScenarioScript
{
    public HallOfMirrors()
    {
        scenarioTitle = "The Hall Of Mirrors";
        
        actionTitles.Add("Listen for the spirit's guidance");
        actionTitles.Add("Take your time");
        actionTitles.Add("Look for a trail to follow");
        actionTitles.Add("Cannonball run");

        actionEffects.Add("+1 IF only you pick this [+3]");
        actionEffects.Add("+2 IF only you pick this [+2]");
        actionEffects.Add("+3 IF only you pick this [+1]");
        actionEffects.Add("+4 IF only you pick this [+0]");

        earlyGame = true;
    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // vars for tracking how many delvers went each way
        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;

        // perform initial pass of delver choices to determine how many went left and how many went right
        foreach(PlayerScript delver in delversSortedScores)
        {
            if(delver.actionIdx == 0)
            {
                a++;
            }
            else if(delver.actionIdx == 1)
            {
                b++;
            }
            else if(delver.actionIdx == 2)
            {
                c++;
            }
            else if(delver.actionIdx == 3)
            {
                d++;
            }
        }

        // look at each delver's action choice
        PlayerScript currentDelver = firstDelver;
        do
        {
            // handle each possible action choice
            switch (currentDelver.actionIdx)
            {
                // listen for the spirit's guidance
                case 0:
                    {
                        // give treasures if exclusive
                        if(a == 1)
                        {
                            TreasureAdjustment(currentDelver, 1);
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 3);
                        }
                        break;
                    }
                // take your time
                case 1:
                    {
                        // give treasures if exclusive
                        if(b == 1)
                        {
                            TreasureAdjustment(currentDelver, 2);
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 2);
                        }
                        break;
                    }
                // look for a trail to follow
                case 2:
                    {
                        // give treasures if exclusive
                        if(c == 1)
                        {
                            TreasureAdjustment(currentDelver, 3);
                        }
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 1);
                        }
                        break;
                    }
                // cannonball run
                case 3:
                    {
                        // give treasures if exclusive
                        if(d == 1)
                        {
                            TreasureAdjustment(currentDelver, 4);
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