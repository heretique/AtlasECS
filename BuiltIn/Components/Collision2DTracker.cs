using UnityEngine;
using System.Collections.Generic;

namespace Atlas {
	public class Collision2DTracker : AtlasComponent {

		public List<Collision2D> Collisions = new List<Collision2D>();

		public void OnCollisionEnter2D(Collision2D other) {
			Collisions.Add(other);
		}

		public void OnCollisionExit2D(Collision2D other) {
			Collisions.Remove(other);
		}

		public new void OnDisable() {
			base.OnDisable();
			Collisions.Clear();
		}
	}
}