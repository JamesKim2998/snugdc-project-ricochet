using UnityEngine;
using System.Collections;

public class CrateDetector : MonoBehaviour {
	public delegate void DoObtain(Crate _crate);
	public DoObtain doObtain;
}
