var document=fl.getDocumentDOM();
var selections=document.selection;
//.fla所在的目录路径
/*const pathURI=document.pathURI;
const path=pathURI.substring(0,pathURI.lastIndexOf("\/")+1);*/

//.jsfl所在的目录路径
var scriptURI=fl.scriptURI;
//取unity项目目录
var exportFolderPath=scriptURI.substring(0,scriptURI.lastIndexOf("/jsfl")+1);
//导出到unity项目目录的路径
exportFolderPath+="Assets/Sprites";

var timeline=document.getTimeline();
var timelineCurrentFrame=timeline.currentFrame;//0开始
//timeline.setSelectedFrames(5-1,5-1,true);//
//--------------------------------------------------------------------------------------------
var funcs={};
funcs.exportMcToPng=function(){
	if(selections&&selections.length>0){
		//将同名的多个选择项存入数组
		for(var i=0;i<selections.length;i++){
			var element=selections[i];
			//fl.trace(element.elementType);
			if(element.elementType=="instance"){
				if(element.instanceType=="symbol"){
					const linkageClassName=element.libraryItem.linkageClassName;
					const itemName=element.libraryItem.name;
					itemName=itemName.substr(itemName.lastIndexOf("\/")+1);
					//const instanceName=element.name;
					const exportName=linkageClassName?linkageClassName:itemName;
					const filePath=exportFolderPath+"/"+exportName;
					
					if(FLfile.createFolder(exportFolderPath)){
						//fl.trace("Folder has been created");
					}else{
						//fl.trace("Folder already exists");
					}
					
					funcs.deleteOldFile(filePath);
					
					const totalFrames=element.libraryItem.timeline.frameCount;
					if(totalFrames<=1){
						//只有一帧时，直接导出位图
						document.exportInstanceToPNGSequence(filePath+".png");
					}else{
						//多帧时生成位图表
						funcs.exportSpriteSheet(element.libraryItem,filePath);
					}
					alert("export complete");
				}
			}else{
				fl.trace("error: the selected object is not symbol");
			}
		}
	}else{
		fl.trace("error: no object is selected");
	}
};

funcs.deleteOldFile=function(filePath){
	//如果存在png，则删除
	const pngPath=filePath+".png";
	if(FLfile.exists(pngPath))FLfile.remove(pngPath);
	//如果存在.meta文件，则删除
	const metaPath=filePath+".png.meta";
	if(FLfile.exists(metaPath))FLfile.remove(metaPath);
}

funcs.exportSpriteSheet=function(libraryItem,filePath){
	var exporter=new SpriteSheetExporter();
	exporter.addSymbol(libraryItem,0);
	exporter.canTrim=false;
	exporter.algorithm="basic";//basic | maxRects
	exporter.layoutFormat="Starling";//Starling | JSON | cocos2D v2 | cocos2D v3
	//exporter.borderPadding=5;
	//exporter.shapePadding=5;
	/*exporter.autoSize=false;
	exporter.sheetWidth=2048;
	exporter.sheetHeight=2048;*/
	var imageFormat={format:"png",bitDepth:32,backgroundColor:"#00000000"};
	exporter.exportSpriteSheet(filePath,imageFormat,true);
	
	//document.exportInstanceToPNGSequence(path+"unityB2Editor/Assets/levelsMaterials/"+exportName+".png",startFrame,endFrame);
	//导出图片的大小将取能包含指定导出所有帧的最大宽高
	//selections[i].libraryItem.exportToPNGSequence(path+"unityB2Editor/Assets/levelsMaterials/"+exportName+".png",startFrame,endFrame);
	//fl.trace(selections[i].left+","+selections[i].top+","+selections[i].width+","+selections[i].height);
	
	//document.save();
}
//--------------------------------------------------------------------------------------------
funcs.exportMcToPng();