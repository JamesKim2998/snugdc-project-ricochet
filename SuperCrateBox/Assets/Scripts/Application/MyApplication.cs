using UnityEngine;
using System.Collections;
using SimpleJSON;

public class MyApplication : Singleton<MyApplication>
{
	public string keyValueTableURL = "http://m.cafe.naver.com/MemoCommentView.nhn?search.clubid=27374390&search.menuid=13&search.articleid=1";
	
	void Start () {
		DontDestroyOnLoad(gameObject);
		StartCoroutine("FetchKeyValueOptions");
	}

	IEnumerator FetchKeyValueOptions() 
	{
		WWW _www = new WWW(keyValueTableURL);
		yield return _www;
		ApplyKeyValueOptions(_www.text);
	}

	void ApplyKeyValueOptions(string _rawTable) 
	{
		_rawTable = _rawTable.Substring(_rawTable.IndexOf("##TABLE_BEGIN##") + "##TABLE_BEGIN##".Length);
		_rawTable = _rawTable.Substring(0, _rawTable.IndexOf("##TABLE_END##"));
		_rawTable.Replace("&nbsp;", " ");
		
		Debug.Log(_rawTable);
		
		var _jsonTable = JSON.Parse(_rawTable);

		var _masterServerIP = _jsonTable["master_server_ip"];
		NetworkManager.masterServerIP = _masterServerIP;
		NetworkManager.masterServerPort = _jsonTable["master_server_port"].AsInt;
		NetworkManager.natFacilitatorIP = _masterServerIP;
		NetworkManager.natFacilitatorPort = 50005;

	}

}

