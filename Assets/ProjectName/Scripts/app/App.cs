namespace framework{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class App:BaseApp<App>{
		
		protected override void Start() {
			base.Start();
			var game=create<Game>("Game1",null);
			var level=create<Level>("Level1",null,game.transform);

		}
		

		


	}
}
