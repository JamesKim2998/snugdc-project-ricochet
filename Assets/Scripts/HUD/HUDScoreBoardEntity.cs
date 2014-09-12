using UnityEngine;
using System.Collections;

 public class HUDScoreBoardEntity : MonoBehaviour 
{
	private string m_Player;
	public string player { 
		get {return m_Player; } 
		set { 
			if (m_Player == value) return;

			m_Player = value; 

			if (m_Player != null)
			{
				statistic = Game.Statistic.Get(m_Player);
			}
			else
			{
				statistic = null;
			}

			Refresh();
		}
	}

	private PlayerStatistics m_Statistic;
	public PlayerStatistics statistic { 
		get { return m_Statistic; } 
		set { 
			if (m_Statistic == value) return;

			if (m_Statistic != null)
			{
				m_Statistic.death.postChanged -= ListenDeathChanged;
				m_Statistic.score.postChanged -= ListenScoreChanged;
			}

			m_Statistic = value;

			if (m_Statistic != null)
			{
				m_Player = m_Statistic.player;
				m_Statistic.death.postChanged += ListenDeathChanged;
				m_Statistic.score.postChanged += ListenScoreChanged;
			}
			else 
			{
				m_Player = null;
			}

			Refresh();
		} 
	}

	public UI2DSprite icon;
	public UILabel nameLabel;
	public UILabel killLabel;
	public UILabel deathLabel;
	public UILabel scoreLabel;

	public bool connected { 
		set {
			if (value)
			{
				icon.color = Color.white;
			}
			else
			{
				icon.color = Color.red;
			}
		} 
	}

	public int kill { set { killLabel.text = value.ToString(); } }
	public int death { set { deathLabel.text = value.ToString(); } }
	public int score { set { scoreLabel.text = value.ToString(); } }
	
	void Start()
	{
//		statistic = Game.Statistic.mine;
	}

	void Destroy()
	{
		player = null;
		statistic = null;
	}

	void Refresh()
	{
		var _player = Global.Player().Get(player);

		if (_player != null)
		{
			connected = true;
			nameLabel.text = _player.name;
			// todo: icon 변경.
		}
		else
		{
			connected = false;
		}

		if (m_Statistic != null)
		{
			death = m_Statistic.death;
			score = m_Statistic.score;
		}
	}

	void OnServerInitialized()
	{
//		// note: Start 시점에서 mine이 존재하지 않는 경우에 대비해
//		// 한번더 set 해줍니다.
//		player = Network.player.guid;
	}

	void ListenPlayerDisconnected(string _player)
	{
		if (_player == m_Player)
			connected = false;
	}

	void ListenKillChanged()
	{

	}

	void ListenDeathChanged(Statistic<int> _statistic)
	{
		death = _statistic;
	}

	void ListenScoreChanged(Statistic<int> _statistic)
	{
		score = _statistic;
	}
}
