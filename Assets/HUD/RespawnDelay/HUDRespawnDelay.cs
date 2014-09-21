using UnityEngine;
using System.Collections;

public class HUDRespawnDelay : MonoBehaviour
{
    private int m_Delay;

    public int delay
    {
        get { return m_Delay; }
        set
        {
            m_Delay = value;
            m_TimeLeft = -1f;
            countLabel.text = ((int) delay).ToString();
            if (delay != 0) enabled = true;
        }
    }

    public UILabel countLabel;
    public Animator animator;

    private float m_TimeLeft = -1f;

	void Awake()
	{
	    if (!animator) animator = GetComponent<Animator>();
	}

    void Start()
    {
        var _mode = Game.Mode as GameModePropertyRespawn;

        if (_mode == null)
        {
            Debug.LogWarning("Game mode should support respawn. Ignore.");
            enabled = false;
            countLabel.text = "";
            return;
        }

        delay = _mode.respawnDelay;
        Game.Character.postCharacterDead += ListenCharacterDead;
    }

    void OnDestroy()
    {
        Game.Character.postCharacterDead -= ListenCharacterDead;
    }

	void Update ()
	{
	    if (m_TimeLeft < 0)
	    {
	        enabled = false;
	        countLabel.text = "";
	        return;
	    }

	    var _old = (int) m_TimeLeft;
	    m_TimeLeft -= Time.deltaTime;

	    var _cur = (int) m_TimeLeft;

	    if (_cur != _old)
	    {
            countLabel.text = (_cur + 1).ToString();
	        animator.SetTrigger("count");
	    }
	}

    void ListenCharacterDead(Character _character)
    {
        if (!_character.IsMine())
            return;

        if (!Game.Progress.IsState(GameProgress.State.OVER))
        {
            m_TimeLeft = delay;
            enabled = true;
            countLabel.text = ((int) m_TimeLeft).ToString();
        }
    }
}
