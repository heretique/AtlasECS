using UnityEngine;
using System.Collections.Generic;

namespace Atlas {
	public class CollisionTracker : AtlasComponent {

		public List<Collision> Collisions = new List<Collision>();

		public void OnCollisionEnter(Collision other) {
			Collisions.Add(other);
		}

		public void OnCollisionExit(Collision other) {
			Collisions.Remove(other);
		}

		public new void OnDisable() {
			base.OnDisable();
			Collisions.Clear();
		}
	}
}