﻿/**************************************
    Webutler V2.1 - www.webutler.de
    Copyright (c) 2008 - 2011
    Autor: Sven Zinke
    Free for any use
    Lizenz: GPL
**************************************/

CKEDITOR.skins.add('silver',(function(){return{editor:{css:['editor.css']},dialog:{css:['dialog.css']},separator:{canGroup:false},templates:{css:['templates.css']},margins:[0,14,18,14]};})());(function(){CKEDITOR.dialog?a():CKEDITOR.on('dialogPluginReady',a);function a(){CKEDITOR.dialog.on('resize',function(b){var c=b.data,d=c.width,e=c.height,f=c.dialog,g=f.parts.contents;if(c.skin!='silver')return;g.setStyles({width:d+'px',height:e+'px'});if(!CKEDITOR.env.ie||CKEDITOR.env.ie9Compat)return;setTimeout(function(){var h=f.parts.dialog.getChild([0,0,0]),i=h.getChild(0),j=i.getSize('width');e+=i.getChild(0).getSize('height')+1;var k=h.getChild(2);k.setSize('width',j);k=h.getChild(7);k.setSize('width',j-28);k=h.getChild(4);k.setSize('height',e);k=h.getChild(5);k.setSize('height',e);},100);});};})();
