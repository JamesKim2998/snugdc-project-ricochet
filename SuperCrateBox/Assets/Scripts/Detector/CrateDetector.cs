using UnityEngine;
using System.Collections;

public class CrateDetector : MonoBehaviour {
	public delegate void DoObtain(Crate _crate);

	private DoObtain m_DoObtain;
	public DoObtain doObtain { set { m_DoObtain = value; }}

	public void Obtain(Crate _crate) {
		if (! enabled) 
		{
			Debug.Log("trying to obtain disabled CrateDetector!");
			return;
		}

		if ( m_DoObtain != null) 
		{
			m_DoObtain(_crate);
		}
		else 
		{
			Debug.Log("doObtain is not set!");
		}
	}

}
