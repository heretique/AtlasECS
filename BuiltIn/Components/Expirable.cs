using UnityEngine;

namespace Atlas {
	public class Expirable : AtlasComponent {

		public float TimeRemaining;
		public GameObject Target;

		protected override void Initialize() {
			if (Target == null) {
				Target = gameObject;
			}
		}
	}
}