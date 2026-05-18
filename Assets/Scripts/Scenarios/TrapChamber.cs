using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapChamber", menuName = "Scriptable Objects/TrapChamber")]
public class TrapChamber : ScenarioScript
{
    public TrapChamber()
    {
        scenarioTitle = "The Trap Chamber";
        
        actionTitles.Add("Lose a finger");
        actionTitles.Add("Burned alive");

        actionEffects.Add("-1 [+2]");
        actionEffects.Add("-3 OR +4 IF all delvers pick this [+1 OR +6 IF all delvers pick this]");

    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // look at each delver's action choice
        PlayerScript currentDelver = firstDelver;

        // flags for burning status
        bool burningTogether = true;
        bool burnCheck = false;

        do
        {
            // handle each possible action choice
            switch (currentDelver.actionIdx)
            {
                // lose a finger
                case 0:
                    {
                        TreasureAdjustment(currentDelver, -1);
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 2);
                        }
                        if(!burnCheck)
                        {
                            burnCheck = true;
                            burningTogether = false;
                        }
                        break;
                    }
                // burned alive
                case 1:
                    {
                        // check all delvers once to see if everyone is burning together
                        if(!burnCheck)
                        {
                            foreach(PlayerScript delver in delversSortedScores)
                            {
                                if(delver.actionIdx != 1)
                                {
                                    burningTogether = false;
                                    break;
                                }
                            }
                            burnCheck = true;
                        }
                        // more treasures if all delvers take on the fire together
                        if(burningTogether)
                        {
                            TreasureAdjustment(currentDelver, 4);
                            if(currentDelver.favored)
                            {
                                TreasureAdjustment(currentDelver, 6);
                            }
                        }
                        // negative treasures without unity
                        else
                        {
                            TreasureAdjustment(currentDelver, -3);
                            if(currentDelver.favored)
                            {
                                TreasureAdjustment(currentDelver, 1);
                            }
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