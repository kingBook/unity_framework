var document=fl.getDocumentDOM();
var selections=document.selection;
//.jsfl所在的目录路径
var scriptURI=fl.scriptURI;
//取unity项目目录
var exportFolderPath=scriptURI.substring(0,scriptURI.lastIndexOf("/jsfl")+1);
//导出到unity项目目录的路径
exportFolderPath+="Assets/Textures";
//--------------------------------------------------------------------------------------------
var funcs={};
funcs.exportMcToPng=function(){
	if(selections&&selections.length>0){
		var isHasExport=false;
		for(var i=0;i<selections.length;i++){
			var element=selections[i];
			if(element.elementType=="instance"){
				if(element.instanceType=="symbol"){
					funcs.exportSymbolItem(element);
					isHasExport=true;
				}
			}else{
				alert("error: the selected object is not symbol");
			}
		}
		if(isHasExport){
			alert("export complete");
		}
	}else{
		alert("error: no object is selected");
	}
}

funcs.exportSymbolItem=function(element){
	const linkageClassName=element.libraryItem.linkageClassName;
	const itemName=element.libraryItem.name;
	itemName=itemName.substr(itemName.lastIndexOf("\/")+1);
	const exportName=linkageClassName?linkageClassName:itemName;
	const filePath=exportFolderPath+"/"+exportName;
	
	if(FLfile.createFolder(exportFolderPath)){
		//fl.trace("Folder has been created");
	}else{
		//fl.trace("Folder already exists");
	}
	
	const totalFrames=element.libraryItem.timeline.frameCount;
	if(totalFrames<=1){
		funcs.deleteOldFile(filePath);
		//exportInstanceToPNGSequence方法，只允许选中一个
		document.selectNone();
		element.selected=true;
		//只有一帧时，直接导出位图
		document.exportInstanceToPNGSequence(filePath+".png");
		//还原选择项
		document.selection=selections;
	}else{
		//多帧时生成位图表
		var maxSheetWidth=2048;
		var maxSheetHeight=2048;
		if(funcs.isOverflowed(element.libraryItem,maxSheetWidth,maxSheetHeight)){
			funcs.exportEveryFrame(element.libraryItem,exportFolderPath,exportName);
		}else{
			funcs.deleteOldFile(filePath);
			funcs.exportAllFrameToImage(element.libraryItem,filePath);
		}
	}
}

funcs.deleteOldFile=function(filePath){
	//如果存在png，则删除
	const pngPath=filePath+".png";
	if(FLfile.exists(pngPath))FLfile.remove(pngPath);
	//如果存在.meta文件，则删除
	const metaPath=filePath+".png.meta";
	if(FLfile.exists(metaPath))FLfile.remove(metaPath);
}

//所有帧导出为一张图
funcs.exportAllFrameToImage=function(libraryItem,filePath,maxSheetWidth,maxSheetHeight){
	var exporter=new SpriteSheetExporter();
	exporter.addSymbol(libraryItem,0);
	exporter.canTrim=false;
	exporter.algorithm="basic";//basic | maxRects
	exporter.layoutFormat="Starling";//Starling | JSON | cocos2D v2 | cocos2D v3
	exporter.autoSize=true;
	exporter.maxSheetWidth=maxSheetWidth;
	exporter.maxSheetHeight=maxSheetHeight;
	var imageFormat={format:"png",bitDepth:32,backgroundColor:"#00000000"};
	exporter.exportSpriteSheet(filePath,imageFormat,true);
}

//每帧一张图导出所有帧
funcs.exportEveryFrame=function(libraryItem,exportFolderPath,exportName){
	var frameCount=libraryItem.timeline.frameCount;
	for(var i=1;i<=frameCount;i++){
		const filePath=exportFolderPath+"/"+exportName+i;
		funcs.deleteOldFile(filePath);
		var exporter=new SpriteSheetExporter();
		exporter.addSymbol(libraryItem,i-1,i);
		exporter.canTrim=false;
		exporter.algorithm="basic";//basic | maxRects
		exporter.layoutFormat="Starling";//Starling | JSON | cocos2D v2 | cocos2D v3
		exporter.autoSize=true;
		exporter.maxSheetWidth=2048;
		exporter.maxSheetHeight=2048;
		var imageFormat={format:"png",bitDepth:32,backgroundColor:"#00000000"};
		exporter.exportSpriteSheet(filePath,imageFormat,true);
	}
}

//导出所有帧时，是否超出指定大小
funcs.isOverflowed=function(libraryItem,maxSheetWidth,maxSheetHeight){
	var exporter=new SpriteSheetExporter();
	exporter.addSymbol(libraryItem,0);
	exporter.canTrim=false;
	exporter.algorithm="basic";//basic | maxRects
	exporter.layoutFormat="Starling";//Starling | JSON | cocos2D v2 | cocos2D v3
	exporter.autoSize=true;
	exporter.maxSheetWidth=maxSheetWidth;
	exporter.maxSheetHeight=maxSheetHeight;
	return exporter.overflowed;
}
//--------------------------------------------------------------------------------------------
funcs.exportMcToPng();