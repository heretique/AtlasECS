using UnityEngine;
using System.Collections.Generic;

namespace Atlas {
	public class TriggerTracker : AtlasComponent {

		public List<Collider> Colliders = new List<Collider>();

		public void OnTriggerEnter(Collider other) {
			Colliders.Add(other);
		}

		public void OnTriggerExit(Collider other) {
			Colliders.Remove(other);
		}

		public new void OnDisable() {
			base.OnDisable();
			Colliders.Clear();
		}
	}
}