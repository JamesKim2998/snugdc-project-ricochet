using System;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class StatisticManager : MonoBehaviour {
    // 킬, 데스, 박스획득, 자살, 연속킬, 최고점수, 누적점수, 최고데스, 연속데스, 총 플레이시간, 연속 플레이시간

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
            get { return int.Parse(Decrypt(PlayerPrefs.GetString(m_Key))); }
            set { PlayerPrefs.SetString(m_Key, Encrypt(value.ToString())); }
        }

        public static implicit operator int(EncryptedPref _self) { return _self.val; }
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
    public EncryptedPref totalPlaytime = new EncryptedPref("total_playtime");
    public EncryptedPref continuousPlaytime = new EncryptedPref("continuous_playtime");
    #endregion

    #region misc
    public EncryptedPref totalCratePickUp = new EncryptedPref("total_crate_pick_up");
    #endregion
    
}
