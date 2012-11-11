<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="fence.aspx.cs" Inherits="ZxtMobile.fence" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>添加电子围栏</title>
    <style type="text/css">
        body {font-size: 12px;color: #666;font-family: Arial,宋体;}
        .inner_input{ width:220px; height:17px; border:1px solid #aaa; border-left:1px inset #000; border-top:1px inset #000; padding-top:3px; padding-left:3px; line-height:17px;}        
        .btnbg{font-size:12px;height:26px;background:#165798;border-width:1; color:#fff; cursor:pointer; line-height:22px; text-align:center; margin:3px;}
        .div_float_logo{ width:49px; height:55px;  position:absolute; left:-20px; top:-20px; background:url(../images/first_ico.png) no-repeat; z-index:9;}
        *html .div_float_logo{background-image:none;filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src="images/first_ico.png", sizingMethod="");}
        #div_float{ width:350px; border:1px solid #25467d; position:absolute; left:250px; top:220px; z-index:1000;}
        #div_list{ width:140px; border:1px solid #25467d; position:absolute; left:70px; top:120px; z-index:999;}
        #div_list_head{width:140px; height:20px; color:#FFFFFF; line-height:20px; background:#16589a; border-bottom:3px solid #699f19; float:left; position:relative;}
        .div_float_head{ width:350px; height:20px; line-height:20px; background:#16589a; border-bottom:3px solid #699f19; float:left; position:relative;}
        .div_float_head img{ float:right; margin:3px 3px 0 0; cursor:pointer;}
        .div_float_content{ background-color:#FFF;filter:alpha(opacity=90);opacity:0.90; padding:10px; line-height:18px; overflow:auto}
        .div_float_content table{ margin:0 auto;}
        .div_float_content td{ line-height:30px;}
        .div_float_content input.text{ width:110px; overflow:hidden; height:16px; line-height:12px; font-size:12px;}
        .slider{ width:170px; float:left; margin-top:7px;}
        .slider-span{ width:16px; float:left; line-height:30px; font-size:14px; margin-left:5px;}
        .div_float_content ul{width:330px;padding:0;margin:0;}
        .div_float_content ul li{width:330px; height:24px; line-height:24px; float:left;padding:0;margin:0; list-style:none; text-indent:10px;}
        .div_float_content ul.cods{ width:312px; border:1px solid #afc2e6; background:#e2eaf9; color:#000; float:left;}
        .div_float_content ul.cods1{ width:312px; border:1px solid #afc2e6; color:#000; margin-top:10px; float:left;}
        .div_float_content ul.cods1 li.title{ background:#e2eaf9; width:312px;}
        .spoi{}
        .spoi a{color:#000; text-decoration:none;}
        .spoi a:hover{color:#000; text-decoration:none;}
        .spoi1{ color:#fff; background:#005cb5; cursor:pointer;}
        .spoi1 a{color:#fff; text-decoration:none;}
        .spoi1 a:hover{color:#fff; text-decoration:none;}
        table.inner_route_result{}
        .inner_route_result td{border-bottom:1px solid #c0c0c0; padding:2px; line-height:25px; color:#000;}
        .td_style{cursor:pointer;}
        .td_style1{ background:#afc2e6; cursor:pointer;}
        .gray{color:#727272;}
        .busline_title{ cursor:pointer; line-height:26px; background:#16589a; color:#fff; margin-bottom:1px;width:330px;float:left;}
        .busline{width:330px;}
        dl{ margin:0; padding:0; color:#000;}
        dl dt{ width:330px; height:20px; line-height:20px; background:#afc2e6;float:left; text-indent:10px;}
        dl dd{ float:left; display:block; margin:0; padding:0;}
        dl dd.busname a{ color:#494949; text-decoration:none; display:block; width:165px; height:22px; line-height:22px;text-indent:10px;}
        dl dd.busname a:hover{ color:#494949; text-decoration:underline; display:block; width:165px; height:22px; line-height:22px; text-indent:10px;}
    </style>
    <script type="text/javascript" src="js/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="http://apis.mapabc.com/webapi/auth.json?t=javascriptmap&v=3.1.1&key=f6c97a7f64063cfee7c2dc2157847204d4dbf093331b2fd5c4351c24252a3ef1a0000638c20d3b24"></script>
    <script type="text/javascript">
    <%=VarInit %>
    var mapObj,mouseTool;
    var circleEditor, polyEditor;
    var editStatus = new Object();
    editStatus.op = 0; //op=0,添加;op=1,修改圆形;op=2,修改矩形和多边形;
    function mapInit() {
        var opt = {
            level: 11, //初始地图视野级别
            center: new MMap.LngLat(114.296428, 30.58243), //设置地图中心点
            doubleClickZoom: true, //双击放大地图
            scrollwheel: true//鼠标滚轮缩放地图
        }
        mapObj = new MMap.Map('map',opt);
        mapObj.plugin(["MMap.ToolBar", "MMap.OverView", "MMap.Scale"], function () {
            toolbar = new MMap.ToolBar();
            toolbar.autoPosition = false; //加载工具条
            mapObj.addControl(toolbar);
            overview = new MMap.OverView(); //加载鹰眼
            mapObj.addControl(overview);
            scale = new MMap.Scale(); //加载比例尺
            mapObj.addControl(scale);
        });
        mapObj.plugin(['MMap.MouseTool'], function () {
            mouseTool = new MMap.MouseTool(mapObj);
            mapObj.bind(mouseTool, "draw", function (e) {
                $("#fencename").val("");
                $("#btnOK").val("添加围栏");
                $("#div_float").css("left", "300px");
                $("#div_float").css("right", "auto");
                $("#div_float").css("top", "220px");
                $("#div_float").show();
                editStatus.op = 0;
                if (typeof (e.path) == 'undefined') {
                    //圆形
                    $("#fencetypeid").val("1");
                    $("#fencetype").val("圆形");
                    $("#fencedata").val(e.getCenter() + ";" + e.getRadius());
                } else {
                    //矩形或多边形
                    $("#fencetypeid").val("3");
                    $("#fencetype").val("多边形");
                    var str = e.path.toString();
                    if (str.split(',').length == 8) {
                        var arr = str.split(',');
                        if (arr[1] == arr[3] && arr[5] == arr[7]) {
                            $("#fencetypeid").val("2");
                            $("#fencetype").val("矩形");
                        }
                    }
                    $("#fencedata").val(e.path.toString());
                }
            });
        });
        fenceinit();
    }

    function fenceinit() {
        for (var i = 0; i < fences.length; i++) {
            if (fences[i].type == "1") {
                mapObj.addOverlays(new MMap.Circle({
                    id: fences[i].id,
                    center: fences[i].center, //圆心，基点 
                    radius: fences[i].radius, //半径 
                    strokeColor: "#0000ff", //线颜色 
                    strokeOpacity: 0.5, //线透明度 
                    strokeWeight: 2, //线宽 
                    fillColor: "#f5deb3", //填充颜色 
                    fillOpacity: 0.35//填充透明度 
                }));
            } else {
                mapObj.addOverlays(new MMap.Polygon({
                    id: fences[i].id,
                    path: fences[i].arr,
                    strokeColor: "#0000ff",
                    strokeOpacity: 0.5,
                    strokeWeight: 2,
                    fillColor: "#f5deb3",
                    fillOpacity: 0.35
                }));
            }
        }
        if (fences.length > 0) {
            mapObj.setFitView();
        }
    }

    function closewin(b) {
        $("#div_float").hide();
        if (editStatus.op == 0) {
            if (mouseTool) mouseTool.close(b);
        } else if (editStatus.op == 1) {
            if (circleEditor) {
                circleEditor.close();
                if (b) {
                    var str = editStatus.data.split(";");
                    editStatus.obj.setCenter(new MMap.LngLat(str[0].split(",")[0],str[0].split(",")[1]));
                    editStatus.obj.setRadius(str[1]);
                }
            }
        } else {
            if (polyEditor) {
                polyEditor.close();
                if (b) {
                    var arr = new Array();
                    var str = editStatus.data.split(",");
                    for (var i = 0; i < str.length; i++) {
                        arr.push(new MMap.LngLat(str[i], str[++i])); 
                    }
                    editStatus.obj.setPath(arr);
                }
            }
        }
    }

    function submit() {
        if ($("#fencename").val() == "") {
            alert("请输入围栏名称");
            return false;
        }
        if (editStatus.op == 0) {
            $.post("addfence.ashx", {
                name: $("#fencename").val(),
                type: $("#fencetypeid").val(),
                data: $("#fencedata").val()
            }, function (result) {
                if (result == "s") {
                    alert("电子围栏添加成功");
                    closewin(false);
                } else {
                    alert("添加失败，请重试");
                }
            });
        } else {
            $.post("fenceedit.ashx", {
                id: editStatus.id,
                name: $("#fencename").val(),
                type: $("#fencetypeid").val(),
                data: $("#fencedata").val()
            }, function (result) {
                if (result == "s") {
                    alert("电子围栏修改成功");
                    closewin(false);
                } else {
                    alert("修改失败，请重试");
                }
            });
        }
    }

    function showfence(id) {
        var arr = new Array();
        arr.push(mapObj.getOverlays(id));
        mapObj.setFitView(arr);
    }
    function editfence(id, name, type) {
        var obj = mapObj.getOverlays(id);
        editStatus.id = id;
        editStatus.obj = obj;
        $("#fencename").val(name);
        $("#fencetypeid").val(type);
        if (type == 1) {
            $("#fencetype").val("圆形");
            editStatus.op = 1;
         }
        else if (type == 2) {
            $("#fencetype").val("矩形");
            editStatus.op = 2;
         }
        else {
            $("#fencetype").val("多边形");
            editStatus.op = 3;
        }
        if (obj.center) {
            $("#fencedata").val(obj.getCenter() + ";" + obj.getRadius());
            editStatus.data = obj.getCenter() + ";" + obj.getRadius();
            mapObj.plugin(['MMap.CircleEditor'], function () {
                circleEditor = new MMap.CircleEditor(mapObj, obj);
                mapObj.bind(circleEditor, "move", function (e) {//移动
                    $("#fencedata").val(obj.getCenter() + ";" + obj.getRadius());
                });
                mapObj.bind(circleEditor, "adjust", function (e) {//改变半径
                    $("#fencedata").val(obj.getCenter() + ";" + obj.getRadius());
                });
                circleEditor.open();
            });
        } else {
            $("#fencedata").val(obj.getPath().toString());
            editStatus.data = obj.getPath().toString();
            mapObj.plugin(['MMap.PolyEditor'], function () {
                polyEditor = new MMap.PolyEditor(mapObj, obj);
                mapObj.bind(polyEditor, "addnode", function (e) {//添加顶点
                    var str = obj.getPath().toString();
                    $("#fencedata").val(str);
                    if (str.split(',').length == 8) {
                        var arr = str.split(',');
                        if (arr[1] == arr[3] && arr[5] == arr[7]) {
                            $("#fencetypeid").val("2");
                            $("#fencetype").val("矩形");
                        } else {
                            $("#fencetypeid").val("3");
                            $("#fencetype").val("多边形");
                        }
                    } else {
                        $("#fencetypeid").val("3");
                        $("#fencetype").val("多边形");
                    }
                });
                mapObj.bind(polyEditor, "adjust", function (e) {//调整顶点
                    var str = obj.getPath().toString();
                    $("#fencedata").val(str);
                    if (str.split(',').length == 8) {
                        var arr = str.split(',');
                        if (arr[1] == arr[3] && arr[5] == arr[7]) {
                            $("#fencetypeid").val("2");
                            $("#fencetype").val("矩形");
                        } else {
                            $("#fencetypeid").val("3");
                            $("#fencetype").val("多边形");
                        }
                    } else {
                        $("#fencetypeid").val("3");
                        $("#fencetype").val("多边形");
                    }
                });
                mapObj.bind(polyEditor, "move", function (e) {//移动
                    var str = obj.getPath().toString();
                    $("#fencedata").val(str);
                    if (str.split(',').length == 8) {
                        var arr = str.split(',');
                        if (arr[1] == arr[3] && arr[5] == arr[7]) {
                            $("#fencetypeid").val("2");
                            $("#fencetype").val("矩形");
                        } else {
                            $("#fencetypeid").val("3");
                            $("#fencetype").val("多边形");
                        }
                    } else {
                        $("#fencetypeid").val("3");
                        $("#fencetype").val("多边形");
                    }
                });
                mapObj.bind(polyEditor, "removenode", function (e) {//删除顶点
                    var str = obj.getPath().toString();
                    $("#fencedata").val(str);
                    if (str.split(',').length == 8) {
                        var arr = str.split(',');
                        if (arr[1] == arr[3] && arr[5] == arr[7]) {
                            $("#fencetypeid").val("2");
                            $("#fencetype").val("矩形");
                        } else {
                            $("#fencetypeid").val("3");
                            $("#fencetype").val("多边形");
                        }
                    } else {
                        $("#fencetypeid").val("3");
                        $("#fencetype").val("多边形");
                    }
                });
                polyEditor.open();
            });
        }
        $("#btnOK").val("修改围栏");
        $("#div_float").css("left", "auto");
        $("#div_float").css("right", "10px");
        $("#div_float").css("top", "20px");
        $("#div_float").show();
    }
    function delfence(id) {
    }
    </script>
</head>
<body onload='mapInit();'>
    <div>
      <input type='button' value='画多边形围栏' onclick='mouseTool.polygon();'/>   
      <input type='button' value='画矩形围栏' onclick='mouseTool.rectangle();'/>   
      <input type='button' value='画圆形围栏' onclick='mouseTool.circle();'/>
    </div>
    <div id='map' style="width:100%;height:600px;"></div>   
    <div id="div_list" style="display:block;">
        <div id="div_list_head">&nbsp;&nbsp;&nbsp;电子围栏</div>
        <div style="height:400px;" class="div_float_content">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tbody>
                  <tr>
                      <td><a href="javascript:showfence('1')">矩形测试1</a>
                      </td>
                      <td>
                      <a href="javascript:editfence('1','矩形测试1',2)">编辑</a>
                      <a href="javascript:delfence('1')">删除</a>
                      </td>
                  </tr>
                  <tr>
                      <td><a href="javascript:showfence('2')">圆形测试2</a>
                      </td>
                      <td>
                      <a href="javascript:editfence('2','圆形测试2',1)">编辑</a>
                      <a href="javascript:delfence('2')">删除</a>
                      </td>
                  </tr>
                  <tr>
                      <td><a href="javascript:showfence('3')">多边形测试</a>
                      </td>
                      <td>
                      <a href="javascript:editfence('3','多边形测试',3)">编辑</a>
                      <a href="javascript:delfence('3')">删除</a>
                      </td>
                  </tr>
                </tbody>
                </table>
             </div>
    </div>
    <div id="div_float" style="display:none;">
        <div class="div_float_head"><img src="images/ico_close.jpg" onclick="closewin(true)" alt="" /><div class="div_float_logo"></div></div>
        <div class="div_float_content">
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tbody>
                  <tr>
                      <td>围栏名称：<input type="text" style="width:120px;" name="fencename" id="fencename" value="" class="inner_input" />
                      </td>
                  </tr>
                  <tr>
                      <td>围栏类型：<input type="text" style="width:60px;" disabled="disabled" name="fencetype" id="fencetype" value="" class="inner_input" />
                        <input type="hidden" id="fencetypeid" value="" />
                      </td>
                  </tr>
                  <tr>
                      <td>围栏数据：<input type="text" style="width:240px;" disabled="disabled" name="fencedata" id="fencedata" value="" class="inner_input" />
                      </td>
                  </tr>
                  <tr>
                    <td align="right"><input id="btnOK" name="btnOK" type="button" class="btnbg" value="添加围栏" style="width:70px;" onclick="submit()" />
                    <input id="btnCancel" name="btnCancel" type="button" class="btnbg" value="取消" style="width:70px;" onclick="closewin(true)" /></td>
                  </tr>
                </tbody>
                </table>
             </div>
    </div>
</body>
</html>
