var document=fl.getDocumentDOM();
var selections=document.selection;
//.jsfl所在的目录路径
var scriptURI=fl.scriptURI;
//取unity项目目录
var exportFolderPath=scriptURI.substring(0,scriptURI.lastIndexOf("/jsfl")+1);
//导出到unity项目目录的路径
exportFolderPath+="Assets/Textures";
//最大的纹理限制
var maxTextureSize={ width:2048, height:2048 };
fl.outputPanel.clear();
//--------------------------------------------------------------------------------------------
var funcs={};
funcs.exportMcToPng=function(){
	if(selections&&selections.length>0){
		var isExportComplete=false;
		for(var i=0;i<selections.length;i++){
			var element=selections[i];
			if(element.elementType=="instance"){
				if(element.instanceType=="symbol"){//MovieClip、Button、Graphic
					isExportComplete=funcs.exportSymbolElement(element);
				}else if(element.instanceType=="bitmap"){
					//const linkageClassName=element.libraryItem.linkageClassName;
	
					//var libraryItemName=element.libraryItem.name;
					//libraryItemName=libraryItemName.substr(libraryItemName.lastIndexOf("\/")+1);
					
					//导出png的名称
					//var exportName=linkageClassName?linkageClassName:libraryItemName;
					//const filePath=exportFolderPath+"/"+exportName;
					var bitmapSymbol=funcs.convertToSymbol(element,i,true);
					isExportComplete=funcs.exportSymbolElement(bitmapSymbol);
				}
			}else if(element.elementType=="shape"){
				var shapeSymbol=funcs.convertToSymbol(element,i,true);
				isExportComplete=funcs.exportSymbolElement(shapeSymbol);
			}else{
				alert("Error: unknown element type '"+element.elementType+"'");
			}
		}
		if(isExportComplete){
			alert("Export complete");
		}
	}else{
		alert("Error: no object is selected");
	}
}

funcs.convertToSymbol=function(element,selectionIndex,isReplaceSelection){
	document.selectNone();
	element.selected=true;
	document.convertToSymbol("movie clip","","center");
	var currentElement=document.selection[0];
	if(isReplaceSelection){
		selections[selectionIndex]=currentElement;
	}
	//还原选择项
	document.selection=selections;
	return currentElement;
}

funcs.exportSymbolElement=function(element){
	const linkageClassName=element.libraryItem.linkageClassName;
	const elementName=element.name;
	
	var libraryItemName=element.libraryItem.name;
	libraryItemName=libraryItemName.substr(libraryItemName.lastIndexOf("\/")+1);
	
	//导出png的名称
	var exportName=elementName?elementName:(linkageClassName?linkageClassName:libraryItemName);
	exportName=exportName.replace(" ","_");//把名字中的空格替换为:"_"
	const filePath=exportFolderPath+"/"+exportName;
	
	if(FLfile.createFolder(exportFolderPath)){
		//fl.trace("Folder has been created");
	}else{
		//fl.trace("Folder already exists");
	}
	
	//生成位图表
	if(funcs.isOverflowed(element)){
		return funcs.exportEveryFrame(element,exportFolderPath,exportName);
	}else{
		funcs.deleteOldFile(filePath);
		return funcs.exportAllFrameToImage(element,filePath,exportName);
	}
	return false;
}

funcs.deleteOldFile=function(filePath){
	//删除.png
	const pngPath=filePath+".png";
	if(FLfile.exists(pngPath))FLfile.remove(pngPath);
	//删除png.meta文件
	const metaPath=filePath+".png.meta";
	if(FLfile.exists(metaPath))FLfile.remove(metaPath);
}

//所有帧导出为一张图
funcs.exportAllFrameToImage=function(element,filePath,exportName){
	var exporter=new SpriteSheetExporter();
	exporter.addSymbol(element,0);
	funcs.setSpriteSheetExporter(exporter);
	var imageFormat={format:"png",bitDepth:32,backgroundColor:"#00000000"};
	exporter.exportSpriteSheet(filePath,imageFormat,true);
	return true;
}

//每帧一张图导出所有帧
funcs.exportEveryFrame=function(element,exportFolderPath,exportName){
	var s=funcs.mulPngAnimXml_beginExport();
	var frameCount=element.libraryItem.timeline.frameCount;
	//先检查是否存在帧大小纹理大小限制
	for(var i=0;i<frameCount;i++){
		var exporter=new SpriteSheetExporter();
		exporter.addSymbol(element,"",i,i+1);
		funcs.setSpriteSheetExporter(exporter);
		if(exporter.overflowed){
			//单帧超出纹理大小限制
			var errorMsg="Error: frame "+(i+1)+" of \'"+exportName+"\' exceeds the texture size limit of "+exporter.maxSheetWidth+"x"+exporter.maxSheetHeight+" cancelled";
			fl.trace(errorMsg);
			alert(errorMsg);
			return false;
		}
	}
	//导出每一帧
	for(var i=0;i<frameCount;i++){
		//帧编号字符串
		var frameNOString=i+"";
		//小于四位，在前面加"0"
		if(frameNOString.length<4){
			const zeroCount=4-frameNOString.length;
			for(var j=0;j<zeroCount;j++){
				frameNOString="0"+frameNOString;
			}
		}
		//导出的文件路径
		const filePath=exportFolderPath+"/"+exportName+frameNOString;
		funcs.deleteOldFile(filePath);
		var exporter=new SpriteSheetExporter();
		exporter.addSymbol(element,"",i,i+1);
		funcs.setSpriteSheetExporter(exporter);
		
		var imageFormat={format:"png",bitDepth:32,backgroundColor:"#00000000"};
		exporter.exportSpriteSheet(filePath,imageFormat,true);
		s+=funcs.mulPngAnimXml_frameExport(exportName+frameNOString);
	}
	s+=funcs.mulPngAnimXml_EndExport();
	//生成记录由多个png组成动画的xml
	FLfile.write(exportFolderPath+"/"+exportName+".multipleImageAnim",s);
	return true;
}

funcs.mulPngAnimXml_beginExport=function(){
	var s = '<?xml version="1.0" encoding="utf-8"?>\n';
	    s+= '<root>\n';
	return s;
}

funcs.mulPngAnimXml_frameExport=function(frameName){
	var s = '\t<name>'+frameName+'</name>\n';
	return s;
}

funcs.mulPngAnimXml_EndExport=function(){
	var	s = '</root>';
	return s;
}

//导出所有帧时，是否超出指定大小
funcs.isOverflowed=function(element){
	var exporter=new SpriteSheetExporter();
	exporter.addSymbol(element,0);
	funcs.setSpriteSheetExporter(exporter);
	return exporter.overflowed;
}

funcs.setSpriteSheetExporter=function(exporter){
	exporter.allowTrimming=true;
	exporter.algorithm="basic";//basic | maxRects
	exporter.layoutFormat="Starling";//Starling | JSON | cocos2D v2 | cocos2D v3
	exporter.autoSize=true;
	exporter.stackDuplicateFrames=true;
	//exporter.allowRotate=true;
	exporter.borderPadding=5;
	exporter.shapePadding=5;
	exporter.maxSheetWidth=maxTextureSize.width;
	exporter.maxSheetHeight=maxTextureSize.height;
}
//--------------------------------------------------------------------------------------------
funcs.exportMcToPng();