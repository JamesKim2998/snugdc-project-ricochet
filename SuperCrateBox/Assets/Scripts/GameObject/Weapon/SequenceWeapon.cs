using System.Collections.Generic;
using System;
using UnityEngine;

public class SequenceWeapon : Weapon {

	struct Phase {

		public Weapon weapon;

		public delegate string GetNext();
		public GetNext getNext;

		public string next {
			get { return getNext(); }
			set { getNext = () => value; }
		}
	}

	// phases
	Dictionary<string, Phase> phases = new Dictionary<string, Phase>();

	private string m_Phase;
	public string phase { get { return m_Phase; }}
	public string startingPhase;

	// states
	private bool m_IsShooting = false;
	public override bool isShooting { get { return m_IsShooting; }}

	private bool m_IsCooling = false;
	public override bool isCooling { get { return m_IsCooling; }}

	private float m_Cooldown;
	public override float cooldown { get { return m_Cooldown;} set { m_Cooldown = value; }}

	private float m_Cooltime;
	public float cooltime { get { return m_Cooltime; } }

	// projectile
	private int m_PhaseIdx;
	public int phaseIdx { get { return m_PhaseIdx; } }

	// events
	public delegate void OnTrigger(object sender, EventArgs args);
	public event OnTrigger onTrigger;

	public void Update() {

		if (isShooting) {

			var _phase = phases[phase];
			
			if (! _phase.weapon.IsShootable() ) 
			{
				// _phase.weapon.Update(dt);
				
			} else {
				var next = _phase.next;
				
				if (next == null) {
					Cool();
				}
				
				if ( phases.TryGetValue(next, out _phase) ) {
					m_Phase = next;
					++m_PhaseIdx;
					_phase.weapon.Shoot();
					
				} else {
					Debug.LogError("cannot find next phase: " + next);
					Stop();
				}
			}

		} 

		if (isCooling) {

			m_Cooltime -= Time.deltaTime;

			if (m_Cooltime < 0) {
				m_IsCooling = false;
			}

		}

	}

	public override bool IsShootable() {
		if ( isShooting || isCooling ) return false;
		if ( startingPhase == null) return false;

		Phase theStartingPhase;

		if ( ! phases.TryGetValue(startingPhase, out theStartingPhase)) {
			return false;
		} else if (! theStartingPhase.weapon.IsShootable()) {
			return false;
		}

		return true;
	}
	
	public override void Shoot() {
		if (! IsShootable()) {
			Debug.LogError("trying to shoot not shootable weapon!");
			return;
		}

		m_Phase = startingPhase;
		m_PhaseIdx = 0;
		m_IsShooting = true;
		phases[startingPhase].weapon.Shoot();

		if (onTrigger != null) {
			onTrigger(this, new EventArgs());
		}
	}

	public override void Stop() {

		if (isShooting) {
			Cool();
		} else {
			m_IsShooting = false;
			m_IsCooling = false;
		}

		m_Phase = null;

	}

	private void Cool() {
		if (! isShooting || isCooling) return;
		m_IsShooting = false;
		m_IsCooling = true;
		m_Cooldown = cooldown;
	}
}

