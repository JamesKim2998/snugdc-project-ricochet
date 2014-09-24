using UnityEngine;

public class PropertyHP : MonoBehaviour {

	public int hpMax = 1;
	private int m_HP;

	public int hp {
		get { return m_HP; }
		
		set { 
			var _old = m_HP;

			m_HP = Mathf.Clamp(value, 0, hpMax);
			
			if (_old > 0 && m_HP == 0) {
				if (postDead != null) postDead();
			}
		}
	}

	public delegate void PostDead();
	public PostDead postDead;

	void Start() {
		hp = hpMax;
	}

	public void heal(int _amount) { 
		hp += _amount;
	}
	
	public void damage(AttackData _attackData) {
		hp -= _attackData.damage;
	}

	public static implicit operator int(PropertyHP _hp) {
		return _hp.hp;
	}

}
