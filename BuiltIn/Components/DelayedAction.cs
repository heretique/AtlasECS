using System;

namespace Atlas {
	public class DelayedAction : AtlasComponent {

		public float Delay;
		public bool Running = true;
		public Action OnUpdate;
		public Action OnComplete;
	}
}