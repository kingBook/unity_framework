using System.Collections.Generic;

public class App:BaseApp<App>{
		
	protected override void init(){
		base.init();
		var game=create<Game>("Game1",null);
		var level=create<Level>("Level1",null,game.transform);
	}

}

