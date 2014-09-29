using UnityEngine;
using System.Collections;

public class UIStatistic : MonoBehaviour
{
    public StatisticType type;
    public UILabel value;

	void Start () {
        Invoke("Refresh", 0.1f);
	}
	
	public void Refresh () {
        var _statistics = StatisticManager.Instance;
        value.text = _statistics[type].val.ToString();
	}
}
