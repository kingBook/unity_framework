var document=fl.getDocumentDOM();
var selections=document.selection;
//.jsfl所在的目录路径
var scriptURI=fl.scriptURI;

//取unity项目目录
var exportFolderPath=scriptURI.substring(0,scriptURI.lastIndexOf("/jsfl")+1);
//导出到unity项目目录的路径
exportFolderPath+="Assets/Textures";
//如果没有文件夹则创建(不需要判断文件夹是否存在)
FLfile.createFolder(exportFolderPath);

//最大的纹理限制
var maxTextureSize={ width:2048, height:2048 };

//是否允许裁剪（true:取各个帧的最小包围盒大小, false:所有帧都一样大小）
//注意：
//如果有空白帧必须设置为 false，否则Unity中Canvas下的Image动画会出现"Invalid AABB inAABB"错误
var isAllowTrimming=false;

//清空输出面板的消息
fl.outputPanel.clear();
//--------------------------------------------------------------------------------------------
var funcs={};
funcs.exportHandler=function(){
	if(selections&&selections.length>0){
		var isExported=false;
		for(var i=0;i<selections.length;i++){
			var element=selections[i];
			if(element.elementType==="instance"){
				if(element.instanceType==="symbol"){//MovieClip、Button、Graphic
					isExported=funcs.exportSymbolElement(element);
				}else if(element.instanceType==="bitmap"){
					//const linkageClassName=element.libraryItem.linkageClassName;
	
					//var libraryItemName=element.libraryItem.name;
					//libraryItemName=libraryItemName.substr(libraryItemName.lastIndexOf("\/")+1);
					
					//导出png的名称
					//var exportName=linkageClassName?linkageClassName:libraryItemName;
					//const filePath=exportFolderPath+"/"+exportName;
					var bitmapSymbol=funcs.convertToSymbol(element,i);
					isExported=funcs.exportSymbolElement(bitmapSymbol);
				}
			}else if(element.elementType==="shape"){
				if(element.isGroup){
					fl.trace("Warning：a group was found and exported as an image.（找到一个组并已将它导出为一张图像）");
				}
				var shapeSymbol=funcs.convertToSymbol(element,i);
				isExported=funcs.exportSymbolElement(shapeSymbol);
			}else{
				alert("Error: unknown element type '"+element.elementType+"'");
			}
		}
		if(isExported){
			alert("Export complete");
		}
	}else{
		alert("Error: no object is selected");
	}
}

/**
* 转换 element 到元件
* @param {Element} element 舞台上的元素
* @param {int} selectionIndex 在selections已选择列表中的索引，不在列表中则填：-1
* @returns {Element} 返回转换后的element
*/
funcs.convertToSymbol=function(element,selectionIndex){
	var depthRecord=element.depth;
	document.selectNone();
	element.selected=true;
	document.convertToSymbol("movie clip","","center");
	var currentElement=document.selection[0];
	for(var i=0;i<1000;i++){//有时候arrange方法会失效，最多执行1000次
		document.arrange("backward");// "back", "backward", "forward",  "front"
		if(currentElement.depth==depthRecord)break;
	}
	if(selectionIndex>-1){
		selections[selectionIndex]=currentElement;
	}
	//还原选择项
	document.selection=selections;
	return currentElement;
}

/**
* 导出舞台上的元件
* @param {Symbol} element 元件
* @returns {boolean} 返回是否导出完成
*/
funcs.exportSymbolElement=function(element){
	const linkageClassName=element.libraryItem.linkageClassName;
	const elementName=element.name;
	
	var libraryItemName=element.libraryItem.name;
	libraryItemName=libraryItemName.substr(libraryItemName.lastIndexOf("\/")+1);
	
	//导出png的名称
	var exportName=elementName?elementName:(linkageClassName?linkageClassName:libraryItemName);
	exportName=exportName.replace(" ","_");//把名字中的空格替换为:"_"
	const filePath=exportFolderPath+"/"+exportName;
	
	//生成位图表
	if(funcs.isOverflowed(element)){
		return funcs.exportEveryFrame(element,exportFolderPath,exportName);
	}else{
		funcs.deleteOldFile(filePath);
		return funcs.exportAllFrameToImage(element,filePath,exportName);
	}
	return false;
}

/**
* 删除旧的文件
* @param {string} filePath 删除的文件路径（不包含文件扩展名）
*/
funcs.deleteOldFile=function(filePath){
	//删除.png
	const pngPath=filePath+".png";
	if(FLfile.exists(pngPath))FLfile.remove(pngPath);
	//删除png.meta文件
	const metaPath=filePath+".png.meta";
	if(FLfile.exists(metaPath))FLfile.remove(metaPath);
}

/**
* 所有帧导出为一张图
* @param {Symbol} element 元件
* @param {string} filePath 文件路径（不包含文件扩展名）
* @param {string} exportName 导出的文件名称
* @returns {boolean} 返回是否导出完成
*/
funcs.exportAllFrameToImage=function(element,filePath,exportName){
	var exporter=new SpriteSheetExporter();
	exporter.addSymbol(element,0);
	funcs.setSpriteSheetExporter(exporter);
	var imageFormat={format:"png",bitDepth:32,backgroundColor:"#00000000"};
	exporter.exportSpriteSheet(filePath,imageFormat,true);
	return true;
}

/**
* 每帧一张图导出所有帧
* @param {Symbol} element 元件
* @param {string} exportFolderPath 导出的文件夹路径
* @param {string} exportName 导出的文件名称
* @returns 返回是否导出完成
*/
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

/**
* 检测导出所有帧到一张图时，是否超出指定大小
* @returns {boolean}
*/
funcs.isOverflowed=function(element){
	var exporter=new SpriteSheetExporter();
	exporter.addSymbol(element,0);
	funcs.setSpriteSheetExporter(exporter);
	return exporter.overflowed;
}

/**
* 设置SpriteSheetExporter
* @param {SpriteSheetExporter} exporter 
*/
funcs.setSpriteSheetExporter=function(exporter){
	exporter.allowTrimming=isAllowTrimming;
	exporter.algorithm="basic";//basic | maxRects
	exporter.layoutFormat="Starling";//Starling | JSON | cocos2D v2 | cocos2D v3
	exporter.autoSize=true;
	exporter.stackDuplicateFrames=true;
	//exporter.allowRotate=true;
	exporter.borderPadding=isAllowTrimming?5:0;
	exporter.shapePadding= isAllowTrimming?5:0;
	exporter.maxSheetWidth=maxTextureSize.width;
	exporter.maxSheetHeight=maxTextureSize.height;
}
//--------------------------------------------------------------------------------------------
funcs.exportHandler();