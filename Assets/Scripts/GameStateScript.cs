using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameStateScript : ScriptableObject
{
    public ScenarioScript scenario;
    public int spiritFavoredID;
    public Dictionary<PlayerScript,int> delverScores;
    public Dictionary<PlayerScript,int> delverChoices;
    public Dictionary<PlayerScript,PlayerScript> delverTargets;
    public Dictionary<PlayerScript,bool> spiritCalled;
}
