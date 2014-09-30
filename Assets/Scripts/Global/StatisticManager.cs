using System;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class StatisticManager 
{

    private readonly Dictionary<StatisticType, EncryptedPref> m_Statistics;
    public EncryptedPref this[StatisticType _type] { get { return m_Statistics[_type]; } } 

    private StatisticManager()
    {
        m_Statistics = new Dictionary<StatisticType, EncryptedPref>()
        {
            { StatisticType.TOTAL_KILL, totalKill },
            { StatisticType.TOTAL_DEATH, totalDeath },
            { StatisticType.TOTAL_SUICIDE, totalSuicide },
            { StatisticType.HIGHEST_KILL, highestKill },
            { StatisticType.HIGHEST_DEATH, highestDeath },
            { StatisticType.CONSECUTIVE_KILL_COUNT, consecutiveKillCount },
            { StatisticType.CONSECUTIVE_DEATH_COUNT, consecutiveDeathCount },
            { StatisticType.HIGHSCORE, highscore },
            { StatisticType.CUMULATIVE_SCORE, cumulativeScore },
            { StatisticType.TOTAL_PLAYTIME, totalPlaytime },
            { StatisticType.CONTINUOUS_PLAYTIME, continuousPlaytime },
            { StatisticType.TOTAL_CRATE_PICKUP, totalCratePickUp },
        };
    }

    static public readonly StatisticManager Instance = new StatisticManager();

    public class EncryptedPref
    {
        public EncryptedPref(string _key)
        {
            m_Key = _key; 
        }

        static EncryptedPref()
        {
            var cspParams = new CspParameters { KeyContainerName = "qwehfuh324" + "wiueoij213" };
            m_Provider = new RSACryptoServiceProvider(cspParams);
        }

        #region crypto
        private static readonly RSACryptoServiceProvider m_Provider;

        private static string Encrypt(string _org)
        {
            return Convert.ToBase64String(m_Provider.Encrypt(System.Text.Encoding.UTF8.GetBytes(_org), true));
        }

        private static string Decrypt(string _encrypted)
        {
            return System.Text.Encoding.UTF8.GetString(m_Provider.Decrypt(Convert.FromBase64String(_encrypted), true));
        }
        #endregion

        private readonly string m_Key;

        public int val
        {
            get { return PlayerPrefs.HasKey(m_Key) ? int.Parse(Decrypt(PlayerPrefs.GetString(m_Key))) : 0; }
            set { PlayerPrefs.SetString(m_Key, Encrypt(value.ToString())); }
        }

        public static implicit operator int(EncryptedPref _self) { return _self.val; }
    }

    public void Save()
    {
        totalPlaytime.val += (int)playtime;
        if (continuousPlaytime < playtime)
            continuousPlaytime.val = (int)playtime;
    }

    #region kill/death
    public EncryptedPref totalKill = new EncryptedPref("total_kill");
    public EncryptedPref totalDeath = new EncryptedPref("total_death");
    public EncryptedPref totalSuicide = new EncryptedPref("total_suicide");
    public EncryptedPref highestKill = new EncryptedPref("highest_kill_count");
    public EncryptedPref highestDeath = new EncryptedPref("highest_death_count");
    public EncryptedPref consecutiveKillCount = new EncryptedPref("consecutive_kill_count");
    public EncryptedPref consecutiveDeathCount = new EncryptedPref("consecutive_death_count");
    #endregion

    #region score
    public EncryptedPref highscore = new EncryptedPref("highscore");
    public EncryptedPref cumulativeScore = new EncryptedPref("cumulative_score");
    #endregion

    #region time

    public float playtime = 0;
    public EncryptedPref totalPlaytime = new EncryptedPref("total_playtime");
    public EncryptedPref continuousPlaytime = new EncryptedPref("continuous_playtime");
    #endregion

    #region misc
    public EncryptedPref totalCratePickUp = new EncryptedPref("total_crate_pick_up");
    #endregion
    
    

}
