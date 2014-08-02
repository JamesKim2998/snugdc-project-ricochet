using UnityEngine;
using System.Collections;

public class UIConnectionView : MonoBehaviour 
{
	public bool isHost = true;
	public UILabel titleLabel;
	public UILabel ipLabel;
	public UILabel portLabel;
	public UILabel playerNameLabel;
	
//	public string title { set { if (titleLabel != null) titleLabel.text = value; }}
//	public string ip { set { if (ipLabel != null) ipLabel.text = value; }}
//	public string port { set { if (portLabel != null) portLabel.text = value; } }
//	public string playerName { 
//		set { 
//			if (playerNameLabel != null) playerNameLabel.text = value; 
//			Global.Player().mine.name = value;
//		}
//	}	

	public void OnIPSubmit()
	{
		string _key = isHost ? "host_ip" : "join_ip";
		Debug.Log("ip submit " + ipLabel.text);
		Global.LocalCache().SetString(_key, ipLabel.text);
	}

	public void OnPortSubmit()
	{
		string _key = isHost ? "host_port" : "join_port";
		Debug.Log("port submit " + portLabel.text);
		Global.LocalCache().SetString(_key, portLabel.text);
	}
}
