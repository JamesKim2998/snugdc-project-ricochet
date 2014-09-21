using SimpleJSON;
using UnityEngine;
using System.Collections;

public class ConfigurationManager {
    public void Load()
    {
        
        var _gameModeAsset = (TextAsset) Resources.Load("game_mode", typeof (TextAsset));
        var _gameModeConfig = LitJson.JsonMapper.ToObject(_gameModeAsset.text);
        var _deathMatchConfig = _gameModeConfig[GameModeType.DEATH_MATCH.ToString()];
        GameModeDeathMatchDef.defaultRespawnDelay = (int)_deathMatchConfig["respawn_delay"];
        GameModeDeathMatchDef.defaultRespawnCount = (int)_deathMatchConfig["respawn_count"];
        GameModeDeathMatchDef.defaultTimeLimit = (int) _deathMatchConfig["time_limit"];
    }

    public void Save()
    {}
}
