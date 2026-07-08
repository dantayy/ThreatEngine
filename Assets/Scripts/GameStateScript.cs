using System.Collections.Generic;
using System.Data;
using UnityEngine;

public struct GameStateScript
{
    public ScenarioScript scenario;
    public int spiritFavoredID;
    public Dictionary<PlayerScript,int> delverScores;
    public Dictionary<PlayerScript,int> delverChoices;
    public Dictionary<PlayerScript,PlayerScript> delverTargets;
    public Dictionary<PlayerScript,bool> spiritCalled;

    public GameStateScript(ScenarioScript scen)
    {
        scenario = scen;
        spiritFavoredID = -1;
        delverScores = new Dictionary<PlayerScript, int>();
        delverChoices = new Dictionary<PlayerScript, int>();
        delverTargets = new Dictionary<PlayerScript, PlayerScript>();
        spiritCalled = new Dictionary<PlayerScript, bool>();
    }
}
