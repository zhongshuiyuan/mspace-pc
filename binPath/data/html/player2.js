var addPresetPosWin;
var PGIS_sysname = "";
var PGIS_citizenid = "";
var PGIS_password = "";
var PGIS_policeID = "";
//��ʷ��Ƶ��������
var VideoForPGIS_HistoryWin;
Ext.onReady(function () {
    hSpeed = new Ext.form.ComboBox({
        store: new Ext.data.SimpleStore({
            fields: ['value'],
            data: [['1'], ['2'], ['3'], ['4'], ['5'], ['6'], ['7'], ['8']]
        }),
        displayField: 'value',
        valueField: 'value',
        labelSeparator: '',
        name: 'hSpeed',
        fieldLabel: 'ˮƽ�ٶ�',
        width: 35,
        triggerAction: 'all',
        forceSelection: true,
        editable: true,
        mode: 'local',
        value: '1'
    });

    vSpeed = new Ext.form.ComboBox({
        store: new Ext.data.SimpleStore({
            fields: ['value'],
            data: [['1'], ['2'], ['3'], ['4'], ['5'], ['6'], ['7'], ['8']]
        }),
        displayField: 'value',
        valueField: 'value',
        name: 'vSpeed',
        fieldLabel: '��ֱ�ٶ�',
        triggerAction: 'all',
        labelSeparator: '',
        forceSelection: true,
        width: 35,
        editable: true,
        mode: 'local',
        value: '1'
    });

    lensSpeed = new Ext.form.ComboBox({
        store: new Ext.data.SimpleStore({
            fields: ['value'],
            data: [['1'], ['2'], ['3'], ['4'], ['5'], ['6'], ['7'], ['8']]
        }),
        displayField: 'value',
        valueField: 'value',
        name: 'lensSpeed',
        fieldLabel: '��ͷ�ٶ�',
        labelSeparator: '',
        width: 35,
        triggerAction: 'all',
        forceSelection: true,
        editable: true,
        mode: 'local',
        value: '1'
    });

    /*	//2008.12.11 ����ȥ�����Ʋ��ԣ�ΪOSD�ó�λ��
        controlPolicy = new Ext.form.ComboBox({
             store: new Ext.data.SimpleStore({
                    fields:['id','value'],
                    data:[['0','������ռ'],['1','����ռ']]
               }),
             displayField:'value',
             valueField:'value',
             name:'controlPolicy',
             fieldLabel:'���Ʋ���',
             triggerAction: 'all',
             forceSelection: true,
             editable:true,
             width:100,
             mode:'local'
        });
        controlPolicy.value = '������ռ';
        controlPolicy.disable();
    */
    switchCombo = new Ext.form.ComboBox({
        store: new Ext.data.Store(
				     {
				         url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=loadSwitchType',
				         reader: new Ext.data.JsonReader(
                             { root: 'data' }, [{ name: 'id' }, { name: 'value' }]
                  )
				     }),
        displayField: 'value',
        valueField: 'value',
        name: 'switch',
        fieldLabel: '��������',
        triggerAction: 'all',
        width: 100,
        forceSelection: true,
        editable: true,
        mode: 'local'
    });

    //��̨���Ƶ�ͼ�갴ť���	
    var iconPanel = new Ext.Panel({
        width: 70,
        items: [{
            layout: 'column',
            items: [{ columnWidth: .3, items: [{ html: '<div onMouseUp="speedConstrolUp(\'leftUp\',this)" onMouseDown="speedConstrolDown(\'leftUp\',this)"><img src="../monitoring/image/leftup.gif" style="display:block" alt="����"/><img style="display:none" src="/PMPlatForm/scene/common/images/leftUp_2.gif" alt="����"/></div>' }] },
                    { columnWidth: .3, items: [{ html: '<div onMouseUp="speedConstrolUp(\'up\',this)"  onMouseDown="speedConstrolDown(\'up\',this)"><img src="../monitoring/image/up.gif" style="display:block" alt="��"/><img style="display:none" src="/PMPlatForm/scene/common/images/up_2.gif" alt="��"/></div>' }] },
                    { columnWidth: .3, items: [{ html: '<div onMouseUp="speedConstrolUp(\'leftUp\',this)"  onMouseDown="speedConstrolDown(\'rightUp\',this)"><img src="../monitoring/image/rightup.gif" style="display:block" alt="����"/><img style="display:none" src="/PMPlatForm/scene/common/images/rightUp_2.gif" alt="����"/></div>' }] }]
        }, {
            layout: 'column',
            items: [{ columnWidth: .3, items: [{ html: '<div onMouseUp="speedConstrolUp(\'left\',this)"  onMouseDown="speedConstrolDown(\'left\',this)"><img src="../monitoring/image/left.gif" style="display:block" alt="��"/><img style="display:none" src="/PMPlatForm/scene/common/images/left_2.gif" alt="��"/></div>' }] },
                    { columnWidth: .3, items: [{ html: '<div ><img src="../monitoring/image/center.gif"/></div>' }] },
                    { columnWidth: .3, items: [{ html: '<div onMouseUp="speedConstrolUp(\'right\',this)"  onMouseDown="speedConstrolDown(\'right\',this)"><img src="../monitoring/image/right.gif" style="display:block" alt="��"/><img style="display:none" src="/PMPlatForm/scene/common/images/right_2.gif" alt="��"/></div>' }] }]
        }, {
            layout: 'column',
            items: [{ columnWidth: .3, items: [{ html: '<div onMouseUp="speedConstrolUp(\'leftDown\',this)"  onMouseDown="speedConstrolDown(\'leftDown\',this)"><img src="../monitoring/image/leftdown.gif" style="display:block" alt="����"/><img style="display:none" src="/PMPlatForm/scene/common/images/leftDown_2.gif" alt="����"/></div>' }] },
                    { columnWidth: .3, items: [{ html: '<div onMouseUp="speedConstrolUp(\'Down\',this)"  onMouseDown="speedConstrolDown(\'down\',this)"><img src="../monitoring/image/down.gif" style="display:block" alt="��"/><img style="display:none" src="/PMPlatForm/scene/common/images/down_2.gif" alt="��"/></div>' }] },
                    { columnWidth: .3, items: [{ html: '<div onMouseUp="speedConstrolUp(\'rightDown\',this)"  onMouseDown="speedConstrolDown(\'rightDown\',this)"><img src="../monitoring/image/rightdown.gif" style="display:block" alt="����"/><img style="display:none" src="/PMPlatForm/scene/common/images/rightDown_2.gif" alt="����"/></div>' }] }]
        }, {},
			  {
			      layout: 'column',
			      items: [{ columnWidth: .3, items: [{ html: '<div onMouseUp="lensConstrolUp(\'ZoomIn\',this)"  onMouseDown="lensConstrolDown(\'ZoomIn\',this)"><img src="../monitoring/image/b01.gif" style="display:block" alt="��ͷ�Ŵ�"/><img style="display:none" src="/PMPlatForm/scene/common/images/zoomIn_2.gif" alt="��ͷ�Ŵ�"/></div>' }] },
						 { columnWidth: .3, items: [{ html: '<div onMouseUp="lensConstrolUp(\'IrisOpen\',this)"  onMouseDown="lensConstrolDown(\'IrisOpen\',this)"><img src="../monitoring/image/b03.gif" style="display:block" alt="��Ȧ�Ŵ�"/><img style="display:none" src="/PMPlatForm/scene/common/images/irisOpen_2.gif" alt="��Ȧ�Ŵ�"/></div>' }] },
						 { columnWidth: .3, items: [{ html: '<div onMouseUp="lensConstrolUp(\'FocusNear\',this)"  onMouseDown="lensConstrolDown(\'FocusNear\',this)"><img src="../monitoring/image/b05.gif" style="display:block" alt="�۽���"/><img style="display:none" src="/PMPlatForm/scene/common/images/focusNear_2.gif" alt="�����"/></div>' }] }]
			  }, {
			      layout: 'column',
			      items: [{ columnWidth: .3, items: [{ html: '<div onMouseUp="lensConstrolUp(\'ZoomOut\',this)"  onMouseDown="lensConstrolDown(\'ZoomOut\',this)"><img src="../monitoring/image/b02.gif" style="display:block" alt="��ͷ��С"/><img style="display:none" src="/PMPlatForm/scene/common/images/zoomOut_2.gif" alt="��ͷ��С"/></div>' }] },
						 { columnWidth: .3, items: [{ html: '<div onMouseUp="lensConstrolUp(\'IrisClose\',this)"  onMouseDown="lensConstrolDown(\'IrisClose\',this)"><img src="../monitoring/image/b04.gif" style="display:block" alt="��Ȧ��С"/><img style="display:none" src="/PMPlatForm/scene/common/images/irisClose_2.gif" alt="��Ȧ��С"/></div>' }] },
						 { columnWidth: .3, items: [{ html: '<div onMouseUp="lensConstrolUp(\'FocusFar\',this)"  onMouseDown="lensConstrolDown(\'FocusFar\',this)"><img src="../monitoring/image/b06.gif" style="display:block" alt="�۽�Զ"/><img style="display:none" src="/PMPlatForm/scene/common/images/focusFar_2.gif" alt="����Զ"/></div>' }] }]
			  }]
    });

    //��̨���Ƶ����������	

    var comboPanel = new Ext.Panel({
        width: 100,
        layout: 'form',
        labelAlign: 'right',
        items: [hSpeed, vSpeed, lensSpeed]
    });

    //Ԥ��λ�ñ��		
    presetPositionGridStore = new Ext.data.Store({
        url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=loadPresetPosition',
        reader: new Ext.data.JsonReader({ root: 'data' }, [
			{ name: 'id' }, { name: 'position' }
        ])
    });


    var sm = new Ext.grid.CheckboxSelectionModel();

    presetPositionGrid = new Ext.grid.GridPanel({
        store: presetPositionGridStore,
        cm: new Ext.grid.ColumnModel([
            sm,
            { id: 'id', header: "���", width: 35, dataIndex: 'id' },
            { id: 'position', header: "Ԥ��λ����", width: 125, dataIndex: 'position' }
        ]),
        sm: sm,
        viewConfig: {
            enableRowBody: true
        },
        //title:'Ԥ��λ�б�',
        autoScroll: true,
        height: 150,
        autoExpandColumn: 'position',
        stripeRows: true,
        //width:215,
        width: 160,
        bbar: [{ text: '��', handler: callPresetPosition },
			  //{text:'����',handler:function(){Ext.MessageBox.prompt('','������Ԥ��λ:',addPresetPosition);
			  {
			      text: '��', handler: function () {
			          if (getCurrentWinDeviceID() == "") {
			              Ext.Msg.alert('��ʾ', '��ѡ��һ����������в���'); //��� 2009-8-12 8:55:10
			              return;
			          }
			          showAddPresetPosWin();
			      }
			  },
			  { text: 'ɾ', handler: delePresetPosition },
			  {
			      text: '��', handler: function () {
			          if (getCurrentWinDeviceID() == "") {
			              Ext.Msg.alert('��ʾ', '��ѡ��һ����������в���'); //��� 2009-8-12 8:55:10
			              return;
			          }
			          if (Ext.getCmp("video_CameraGuard")) {
			              Ext.getCmp("video_CameraGuard").show();
			          } else {
			              new AXY.Video.CameraGuard({
			                  id: "video_CameraGuard"
			              }).show();
			          }
			          log.info("��ȡ���������IDΪ:" + getCurrentWinDeviceID() + ",׼����ȡ��������Ŀ���λ���ݡ�");
			          //Ext.getCmp("video_CameraGuard").loadData("13030000001340000001");
			          Ext.getCmp("video_CameraGuard").loadData(getCurrentWinDeviceID());
			      }
			  }
        ]
    });


    //ǰ���豸������      
    deviceControlPanel = new Ext.FormPanel({
        title: 'ǰ���豸����',
        region: 'east',
        frame: true,
        //width:250,
        //width:195,
        width: 300,
        //collapseMode:'mini',
        height: 700,
        collapsible: true,
        defaults: { xtype: 'fieldset' },
        items: [{
            title: '��̨����',
            width: 183,
            labelAlign: 'left',
            labelWidth: 55,
            items: [{
                layout: 'column',
                //labelWidth:60,
                items: [{ columnWidth: .4, items: iconPanel },
                    { columnWidth: .6, items: comboPanel }]
            }
                   // ,{},controlPolicy
            ]
        }, {
            title: '���ز���',
            width: 183,
            labelWidth: 55,
            labelAlign: 'left',
            items: switchCombo,
            buttonAlign: 'left',
            buttons: [{
                text: '����',
                handler: switchOperate
            }, {
                text: '�ر�',
                handler: switchOperate
            }]
        }, {
            title: 'Ԥ��λ����',
            width: 183,
            items: presetPositionGrid
        }, {
            title: '��¼/���ſ���',
            width: 183,
            layout: 'column',
            items: [
                  { columnWidth: .15, items: [{ html: '<div onMouseDown="contorlPanelDown(this)" onMouseUp="contorlPanelUp(this)" onclick="onPlayControl(\'login\')"><img src="images/login.bmp" style="display:block" alt="��¼����ƽ̨"/><img src="images/logoff.bmp" style="display:none" alt="ע��"/></div>' }] },
                  { columnWidth: .02, items: [{ html: '' }] },
                  { columnWidth: .15, items: [{ html: '<div onMouseDown="contorlPanelDown(this)" onMouseUp="contorlPanelUp(this)" onclick="onPlayControl(\'logout\')"><img src="images/logoff.bmp" style="display:block" alt="�ǳ�����ƽ̨"/><img src="images/logoff.bmp" style="display:none" alt="ע��"/></div>' }] },
                  { columnWidth: .02, items: [{ html: '' }] },
                  { columnWidth: .15, items: [{ html: '<div onMouseDown="contorlPanelDown(this)" onMouseUp="contorlPanelUp(this)" onclick="vlcplayer.ScreenPartition1()"><img src="images/1.jpg" style="display:block" alt="����"/><img src="images/1_2.gif" style="display:none" alt="����"/></div>' }] },
                  { columnWidth: .02, items: [{ html: '' }] },
                  { columnWidth: .15, items: [{ html: '<div onMouseDown="contorlPanelDown(this)" onMouseUp="contorlPanelUp(this)" onclick="vlcplayer.ScreenPartition2()"><img src="images/4.jpg" style="display:block" alt="�ķ���"/><img src="images/4-2.gif" style="display:none" alt="�ķ���"/></div>' }] },
                  { columnWidth: .02, items: [{ html: '' }] },
                  { columnWidth: .15, items: [{ html: '<div onMouseDown="contorlPanelDown(this)" onMouseUp="contorlPanelUp(this)" onclick="onPlayControl(\'stop\')"><img src="images/stop.gif" style="display:block" alt="ֹͣ����"/><img src="images/stop_2.gif" style="display:none" alt="ֹͣ����"/></div>' }] },
                  { columnWidth: .02, items: [{ html: '' }] },
                  { columnWidth: .15, items: [{ html: '<div onMouseDown="contorlPanelDown(this)" onMouseUp="contorlPanelUp(this)" onclick="onPlayControl(\'history\')"><img src="images/history.gif" style="display:block" alt="��ʷ�ط�"/><img src="images/history_2.gif" style="display:none" alt="��ʷ�ط�"/></div>' }] },
                  { columnWidth: .02, items: [{ html: '' }] }
            ]
        }
        ]
    });

    var basePanel = new Ext.Panel({
        width: 948,
        height: 600,
        title: 'PGIS��Ƶ',
        layout: 'column',
        items: [{ columnWidth: .75, items: [{ html: "<OBJECT ID='vlcplayer' CLASSID='CLSID:E6963FEA-95F6-4AA7-8C2C-44D5FB979B9A' width='704' height='576' align=><param name='url' value='D:\1.264'></OBJECT>" }] },
				{ columnWidth: .25, items: [deviceControlPanel] }
        ],
        renderTo: 'basePanel'
    });


    //2010��5��6��10:48:03 by liufeng ΪPGIS����ҳ�棬����VLC���Ų�������������ع���
    if (vlcplayer.url == undefined) {
        Ext.Msg.alert("������ʾ", "���������û�а�װϵͳ�������ActiveX�ؼ�,<a href='../mainframe/PlayerX_Setup.exe' target='_blank'>�������</a>,��װ��Ϻ�����������IE�����!");
    } else if (vlcplayer.VerSion == undefined) {
        //vlcplayer.ScreenPartition1();
        Ext.Msg.alert("������ʾ", "����װ�Ĳ���������������°汾,�����ذ�װ���°汾,<a href='../mainframe/PlayerX_Setup.exe' target='_blank'>�������</a>,��װ��Ϻ�����������IE�����!");
    } else if (vlcplayer.VerSion != VLC_PUGIN_VERSION) {
        //vlcplayer.ScreenPartition1();
        var constVersion = VLC_PUGIN_VERSION;  //VLC_PUGIN_VERSION ������ common/js/SysConstants.js
        var vlcplsyerVersion = vlcplayer.VerSion;
        var C_Version = constVersion.substring(0, 1) + constVersion.substring(2, 3) + constVersion.substring(4, 5) + constVersion.substring(6)
        var V_Version = vlcplsyerVersion.substring(0, 1) + vlcplsyerVersion.substring(2, 3) + vlcplsyerVersion.substring(4, 5) + vlcplsyerVersion.substring(6);
        log.info("PGIS����ҳ��VLC�����C_Version=" + C_Version + ",V_Version=" + V_Version);
        if (C_Version > V_Version) {
            Ext.Msg.alert("������ʾ", "����װ�Ĳ���������������°汾,�����ذ�װ���°汾,<a href='../mainframe/PlayerX_Setup.exe' target='_blank'>�������</a>,��װ��Ϻ�����������IE�����!");
        }
    } else {
        //vlcplayer.ScreenPartition1();
    }

});  //onReady end








//�����ĸ���������̨��������ͼ���Ӧ����갴��,�ɿ��Ĵ���
function speedConstrolDown(opt, dom) {
    log.info("������̨���ư�ť");
    dom.childNodes[0].style.display = "none";
    dom.childNodes[1].style.display = "block";
    //	    var deviceID = getCurrentWinDeviceID();
    var deviceID = getCurrentWinDeviceID();  //����plugin.js�Ĵ˷���
    if (deviceID != null && deviceID != "") {
        var requestTime = (new Date()).getTime();
        Ext.Ajax.request({
            url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=speedConstrol',
            params: {
                commond: opt,
                detailOperation: 'start',
                sysname: sysname,
                password: password,
                citizenid: citizenid,
                policeID: policeID,
                hSpeed: hSpeed.getValue(),
                vSpeed: vSpeed.getValue(),
                deviceID: deviceID,
                //2009��10��28��16:12:50 by liufeng �������ܣ�������ʱ�䷢�͵���������
                requestTime: requestTime
            },
            success: function (response) {
                if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                    Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                }
            },
            failure: function () {
                Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
            }
        });
    }
}

function speedConstrolUp(opt, dom) {
    log.info("�ɿ���̨���ư�ť");
    dom.childNodes[0].style.display = "block";
    dom.childNodes[1].style.display = "none";
    //	    var deviceID = getCurrentWinDeviceID();
    var deviceID = getCurrentWinDeviceID();
    if (deviceID != null && deviceID != "") {
        Ext.Ajax.request({
            url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=speedConstrol',
            params: {
                commond: opt, detailOperation: 'stop', deviceID: deviceID,
                sysname: sysname, password: password, citizenid: citizenid, policeID: policeID
            },
            success: function (response) {
                if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                    Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                }
            },
            failure: function () {
                Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
            }
        });
    }
}

function lensConstrolDown(opt, dom) {
    log.info("���¾�ͷ���ư�ť,��ʼ��������");
    dom.childNodes[0].style.display = "none";
    dom.childNodes[1].style.display = "block";
    //	    var deviceID = getCurrentWinDeviceID();
    var deviceID = getCurrentWinDeviceID();
    if (deviceID != null && deviceID != "") {
        Ext.Ajax.request({
            url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=lensConstrol',
            params: {
                commond: opt, detailOperation: 'start', lensSpeed: lensSpeed.getValue(), deviceID: deviceID,
                sysname: sysname, password: password, citizenid: citizenid, policeID: policeID
            },
            success: function (response) {
                if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                    Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                }
                log.info("���¾�ͷ���ư�ť������ϣ�Ajax����ɹ�!");
            },
            failure: function () {
                Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
            }
        });
    }
}

function lensConstrolUp(opt, dom) {
    log.info("�ɿ���ͷ���ư�ť,��ʼ��������");
    dom.childNodes[0].style.display = "block";
    dom.childNodes[1].style.display = "none";
    //	    var deviceID = getCurrentWinDeviceID();
    var deviceID = getCurrentWinDeviceID();
    if (deviceID != null && deviceID != "") {
        Ext.Ajax.request({
            url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=lensConstrol',
            params: { commond: opt, detailOperation: 'stop', deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID },
            success: function (response) {
                if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                    Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                }
                log.info("�ɿ���ͷ���ư�ť������ϣ�Ajax����ɹ�!");
            },
            failure: function () {
                Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
            }
        });
    }
}
function contorlPanelDown(dom) {
    dom.childNodes[0].style.display = "none";
    dom.childNodes[1].style.display = "block";
}
function contorlPanelUp(dom) {
    dom.childNodes[0].style.display = "block";
    dom.childNodes[1].style.display = "none";
}
//Ԥ��λ����'����'��ť������
function callPresetPosition() {
    var record = presetPositionGrid.getSelectionModel().getSelections();
    var sum = presetPositionGrid.getSelectionModel().getCount();
    var deviceID = getCurrentWinDeviceID();
    if (sum == 0) {
        Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: "��ѡ��һ��Ԥ��λ", buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
    } else if (sum > 1) {
        Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: "ÿ��ֻ�ܵ���һ��Ԥ��λ", buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
    } else {
        var id = record[0].get('id');
        Ext.Ajax.request({
            url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=presetPostionOperate',
            params: { detailOperation: 'call', id: id, deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID },
            success: function (response) {
                if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                    Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                }
            },
            failure: function () {
                Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
            }
        });
    }

}

//Ԥ��λ����'���'��ť������
function addPresetPosition(btn, text) {
    if (btn == 'ok') {
        if (text == null || text == "") {
            log.info("δ����Ԥ��λ����,����Ԥ��λʧ��");
            Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.VALIDATEFAILEDINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
        } else {
            var deviceID = getCurrentWinDeviceID();
            Ext.Ajax.request({
                url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=presetPostionOperate',
                paramse: { detailOperation: 'add', position: text, deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID },
                success: function (response) {
                    if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                        Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                    } else {
                        presetPositionGridStore.load({ params: { deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID } });
                    }
                },
                failure: function () {
                    Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                }
            });
        }
    }
}

//��������Ԥ��λ�ĶԻ�������
addPresetPosPanel = new Ext.form.FormPanel({
    bodyStyle: 'padding:20px;',
    frame: true,
    items: [{
        layout: 'form',
        items: [
		new Ext.form.NumberField({
		    fieldLabel: 'Ԥ��λ���',
		    width: 190,
		    frame: true,
		    id: 'presetPosID',
		    name: 'presetPosID',
		    allowBlank: false,
		    maxValue: 255,
		    minValue: 1,
		    //validator:validatePosID,
		    labelSeparator: ''
		})]
    }, {
        layout: 'form',
        items: [
		new Ext.form.TextField({
		    fieldLabel: 'Ԥ��λ����',
		    width: 190,
		    frame: true,
		    id: 'presetPosName',
		    name: 'presetPosName',
		    allowBlank: false,
		    blankText: '����Ϊ��',
		    labelSeparator: ''
		})]
    }
    ]
});
function validatePosID(posID) {
    if (posID < 129) {
        return true;
    }
    else {
        return "Ԥ��λ���ֻ������λ����";
    }
}

//��ʾ����Ԥ��λ�ĶԻ���
function showAddPresetPosWin() {
    if (!addPresetPosWin) {//����windows������
        addPresetPosWin = new Ext.Window({
            el: 'hello-win',
            layout: 'fit',
            width: 400,
            height: 180,
            title: '���Ԥ��λ',
            items: [addPresetPosPanel],
            closeAction: 'hide',
            plain: true,
            buttonAlign: 'center',
            //�������buttons����
            buttons: [{
                text: '���',
                handler: function () {
                    if (!addPresetPosPanel.form.isValid()) {
                        Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.VALIDATEFAILEDINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                    } else {
                        var presetPosID = Ext.getCmp('presetPosID').getValue();
                        var presetPosName = Ext.getCmp('presetPosName').getValue();
                        var deviceID = getCurrentWinDeviceID();
                        log.info("presetPosID:" + presetPosID + " presetPosName:" + presetPosName + " deviceID:" + deviceID);
                        addPresetPosWin.hide();
                        var deviceID = getCurrentWinDeviceID();
                        //����������ж��Ƿ�����ͬ��Ԥ��λ 2009-8-6 15:24:38
                        //var number=presetPositionGridStore.find("id",presetPosID); //��� 2009-8-12 8:55:27
                        var isHave = false;
                        presetPositionGridStore.findBy(function (record, id) {
                            if (record.get("id") == presetPosID) {
                                isHave = true;
                            }
                        }, this);
                        if (isHave) {
                            Ext.Msg.alert("��ʾ", "�Ѿ�����ͬ���Ƶ�Ԥ��λ");
                            return;
                        }

                        Ext.Ajax.request({
                            url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=presetPostionOperate',
                            params: { detailOperation: 'add', presetPosID: presetPosID, presetPosName: presetPosName, deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID },
                            success: function (response) {
                                if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                                    Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                                } else {
                                    presetPositionGridStore.load({ params: { deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID } });
                                }
                            },
                            failure: function () {
                                Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                            }
                        });
                    }
                }
            }, {
                text: 'ȡ��',
                handler: function () {
                    addPresetPosPanel.form.reset();
                    addPresetPosWin.hide();
                }
            }]
        });
    }
    addPresetPosPanel.form.reset();
    addPresetPosWin.show();
};


//Ԥ��λ����'ɾ��'��ť������
function delePresetPosition() {
    var record = presetPositionGrid.getSelectionModel().getSelections();
    //2009-09-26 �����޸� ���ɾ����ʾ
    if (record.length < 1) {///////
        Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.SELECTRECORDTODELETEINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });///////
    } else {///////

        var sum = presetPositionGrid.getSelectionModel().getCount();
        var deviceID = getCurrentWinDeviceID();
        var deleteDevice = "";
        var msg = "";
        for (i = 0; i < sum; i = i + 1) {
            var id = record[i].get('id');
            //1,2,15,16,17��Ԥ��λΪ����Ԥ��λ��������ɾ��,2010��3��26��9:13:30 zhf����
            if (id == "1" || id == "2" || id == "15" || id == "16" || id == "17") {
                Ext.Msg.alert("��ʾ", "1,2,15,16,17��Ԥ��λΪ����Ԥ��λ��������ɾ��");
                return;



            }
            else {
                deleteDevice = deleteDevice + id + ",";
            }
        }

        Ext.Msg.confirm(SysConst.PROMPTINFOTEXT, SysConst.DELETECONFIRMINFOTEXT, function (_btn) {///////
            if (_btn == 'no') {///////
                return;///////
            } else {///////

                deleteDevice = deleteDevice.substring(0, deleteDevice.length - 1);
                Ext.Ajax.request({
                    url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=delPresetPositon',
                    params: { detailOperation: 'delete', id: deleteDevice, deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID },
                    success: function (response) {
                        if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                            Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                        } else {
                            presetPositionGridStore.load({ params: { deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID } });
                        }
                    },
                    failure: function () {
                        Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                    }
                });
            }//end of second else
        })//end of ext.msg.confirm
        //presetPositionGridStore.load({params:{deviceID:deviceID}});

    }//end of first else
}//end of delePresetPosition()

//���ؿ��������������رհ�ť�Ĵ�����
function switchOperate(btn) {
    var opt;
    var id;
    if (btn.text == '����') {
        opt = 'open';
    } else {
        opt = 'close';
    }
    var index = switchCombo.store.find('value', switchCombo.getValue());
    id = switchCombo.store.collect('id')[index];
    var deviceID = getCurrentWinDeviceID();
    if (deviceID != null && deviceID != "") {
        Ext.Ajax.request({
            url: PROJECT_NAME + '/monitoring/PGISVideoManage.do?operation=switchOperate',
            params: { detailOperation: opt, switchid: id, deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID },
            success: function (response) {
                if (response.responseText != "") {//2009-11-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                    Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
                }
            },
            failure: function () {
                Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
            }
        });
    }
}













//�����ڵ�����ʶ����ʵʱ����ʷ��������ʶ
var subjectArray = ["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""];
//������������ʶ
var playerSubject = "";
//�������Ĳ���״̬
var playerStatus = "no";
var sipPluginPort = "5022";
var PROJECT_NAME = '/PMPlatForm';
var curWinDeviceID = "";
//��ǰ�û�ѡ�еĴ���
var currentChooseWindow = 1;



/*****************************************************����ʵʱ��Ƶ******************************************************/
//id �豸ID
function playVideo(sysname, citizenid, password, id, policeID) {
    if (vlcplayer.url == undefined) {
        return;
    }
    vlcplayer.JsSleep();
    setTimeout(function () { vlcplayer.JsWakeUp(); }, 2000);
    var status = playerStatus;//��ò���״̬
    var deviceID = id;
    playerStatus = "rtm_play";  //���ò���״̬
    var url = PROJECT_NAME + "/monitoring/PGISVideoManage.do?operation=playRealtimeVideo"
														+ "&deviceID=" + deviceID
														+ "&sysname=" + sysname
														+ "&citizenid=" + citizenid
														+ "&password=" + password
														+ "&policeID=" + policeID;
    var successFunc = function (response) {
        var text = response.responseText;
        if (text != "") {
            alert(text);
        }
    };
    var failureFunc = function () {
        log.info('�㲥ʧ�ܣ������ƽ̨�����������쳣');
    };
    sendAjax(url, successFunc, failureFunc);
}// end of playVideo

/*****************************************************ָֹͣ�����ڵ�ʵʱ��Ƶ*********************************************/
function stopRealtimeVideo(sysname, citizenid, password, winID, policeID, mode) {
    log.info("��ǰҪֹͣ����Ƶ�Ĵ����ǣ�" + winID);
    //stopPlayerStream(winID);
    /*if(subjectArray[winID]==""){
 		return;
 	}*/
    subjectArray[winID] = "";

    var url = PROJECT_NAME + "/monitoring/PGISVideoManage.do?operation=stopRealtimeVideo"
														+ "&sysname=" + sysname
														+ "&citizenid=" + citizenid
														+ "&winID=" + winID
														+ "&password=" + password
														+ "&policeID=" + policeID
														+ "&onlySetWinStatus=" + mode;
    var successFunc = function (response) {
    };
    var failureFunc = function () {
        //alert('�Ͽ�ʵʱ�㲥ʧ�ܣ������ƽ̨�����������쳣');
    };
    sendAjax(url, successFunc, failureFunc);
    playerStatus = "no";
}//end of stopRealtimeVideo

/****************************************************ֹͣ����������*****************************************************/
function stopPlayerStream(winID) {
    if (vlcplayer.url == undefined) {
        return;
    }
    var curWinID = vlcplayer.windowID;
    vlcplayer.windowID = winID;
    vlcplayer.StopStream(winID);
    vlcplayer.windowID = curWinID;
}
/************************************************������յ�SIP���smp��Ϣ��Ĵ�����*******************************************/
function smp(commandOfSmp) {

    if (vlcplayer.url == undefined) {
        return;
    }
    log.info("�յ��Ĳ����smp�ǣ�" + commandOfSmp);
    var array = commandOfSmp.split(",");
    array = array[4].split(":");
    var port = array[1];

    if (port == '9910')
        return; //�˿�Ϊ9910,Ϊ���أ�������



    var win_id = 1;
    for (var i = 1; i < 17; i++) {
        if (vlcplayer.getRecvPort(i) == port) {
            win_id = i;
            break;
        }
    }
    //var ip = LocalIP ;   //mainTab.jsp�ж����ȫ�ֱ������ǿͻ��˵ı���IP
    //2010.11.28 by liufeng @ wuhu
    array = commandOfSmp.split(",");
    array = array[3].split(":");
    var ip = array[1];

    var url = "udp://@" + ip + ":" + port;
    vlcplayer.windowID = win_id;  //�Ѳ�������������Ϊsmp�а����Ĵ��ں�
    vlcplayer.url = url;
    log.info("������������url�ǣ�" + url + "����ǰ��������Ϊ��" + win_id);
}// end of smp
/************************************************������յ�SIP���subject��Ϣ��Ĵ�����***************************************/

function subject(subjectString) {
    if (subjectString != "") {
        var array = subjectString.split(",");
        var array2 = subjectString.split(":");
        curWinDeviceID = array2[0];//��ǰ�㲥�豸��ID
        log.info("��ǰ�㲥�豸��ID�ǣ�" + curWinDeviceID);
        array = array[1].split(":");
        var id = array[1];
        id = parseInt(id); //����ID    
        subjectArray[id] = subjectString;  //���øô��ڵ�����ʶ


        loadPresetAndSwitch();
    }

}// end of subject


/************************************************�յ�SIP���bye��Ϣ��Ĵ�����*******************************************/
function onByeReceive(commandOfBye) {

    log.info("in onByeReceive");
    var array = commandOfBye.split(",");
    array = array[1].split(":");
    var win_ID = parseInt(array[1]);
    stopPlayerStream(win_ID);
    log.info("ͣ�����Ĵ���ID�ǣ�" + win_ID);
    subjectArray[win_ID] = "";   //���ڷ��ֶ�ֹͣ�������������Ѳ��

    //���Ԥ��λ��Ϣ
    clearPresetAndSwitch();
    stopRealtimeVideo(sysname, citizenid, password, win_ID, policeID, "onlySetWinStatus");
}// end of onByeReceive
/***********************************************��ͨ��SIP��Ϣ������**************************************************/
function MsgDispose(msg) {
    var str = msg;
    if ("XMLMSG" == str.substring(0, 6)) {
        var jsonStr = str.substring(6);
        var jsonData = eval("(" + jsonStr + ")");

        var msgType = jsonData.msgType;
        var msgContent;
        switch (parseInt(msgType)) {
            case 35:
                //������OSD��Ϣ
                if (vlcplayer.url == undefined) {
                    return;
                }

                vlcplayer.windowID = jsonData.playerWinID;
                vlcplayer.OSDCameraID = jsonData.OSDCameraID;
                vlcplayer.OSD = jsonData.OSD;
                break;
        }
    }
}
/*******************************************��֪������sip����Ķ˿�******************************************************/
function setSIPPluginPort(port) {
    var url = PROJECT_NAME + "/specialLogon.do?operation=setSIPPort" + "&port=" + port;
    var successFunc = function (response) {
    };
    var failureFunc = function () {
    };
    sendAjax(url, successFunc, failureFunc);
}// end of  setSIPPluginPort

function saveSIPPluginPort(port) {
    sipPluginPort = port;
}

function getSIPPluginPort(port) {
    return sipPluginPort;
}
/** 
	 * function setPGISParameter ����PGIS����ƽ̨��ȫ�ֱ���
	 * @param mapping PGIS��¼��ʶ
	 * @param form PGIS����
	 * @param request �û������֤����
	 */
function setPGISParameter(sysname, citizenid, password, policeID) {
    PGIS_sysname = sysname;
    PGIS_citizenid = citizenid;
    PGIS_password = password;
    //2014��1��23��16:45:57 ��������
    PGIS_policeID = policeID;
}
/*******************************************��¼����ƽ̨*****************************************************************/
function playVideoOfPMPlatform(sysname, citizenid, password, deviceID, policeID, onlyLogin) {

    var url = PROJECT_NAME + "/specialLogon.do?sysname=" + sysname + "&citizenid=" + citizenid
			 + "&password=" + password + "&port=" + sipPluginPort + "&policeID=" + policeID;

    var successFunc = function (response) {
        var text = response.responseText;
        if (text != null && text != "") {
            var tmp = text.split(",");
            if (tmp[0] == "success") {
                var userID = tmp[1];
                log.info("�յ�ƽ̨���ص��û�ID:" + userID);
                UActr.uname = userID;
                sendHeartBeats();
                if (onlyLogin != true) {
                    playVideo(sysname, citizenid, password, deviceID, policeID);
                }
                //����PGIS����ƽ̨��ȫ�ֱ���
                //setPGISParameter(sysname,citizenid,password,policeID);
                log.info("��¼����ƽ̨�ɹ������㲥��Ƶ");
            }
            else if (tmp[0] == "online") {
                var userID = tmp[1];
                //alert("�յ�ƽ̨���ص��û�ID:"+userID);
                UActr.uname = userID;
                sendHeartBeats();
                if (onlyLogin != true) {
                    playVideo(sysname, citizenid, password, deviceID, policeID);
                }
                //����PGIS����ƽ̨��ȫ�ֱ���
                //setPGISParameter(sysname,citizenid,password,policeID);
                //alert("�Ѿ����ߣ�ֱ�ӵ㲥��Ƶ");
            }
            else {
                log.info("in successFunc,response is:" + text);
                alert(text);
            }
        }
    };
    var failureFunc = function () {
        log.info('��¼ʧ�ܣ������ƽ̨�����������쳣');
        //alert('��¼ʧ�ܣ������ƽ̨�����������쳣');
    };
    sendAjax(url, successFunc, failureFunc);
}

/*******************************************�ǳ�����ƽ̨*****************************************************************/
function logoutPMPlatform(sysname, citizenid, password, policeID) {
    var url = PROJECT_NAME + "/monitoring/PGISVideoManage.do?operation=logout"
														+ "&sysname=" + sysname
														+ "&citizenid=" + citizenid
														+ "&password=" + password
														+ "&policeID=" + policeID;
    var successFunc = function (response) {
        var text = response.responseText;
        if (text == "success") {
            //alert("�ǳ�����ƽ̨�ɹ���");
        }
        else {
            //alert('�ǳ�����ƽ̨ʧ��!');
        }
    };
    var failureFunc = function () {
        log.info('�ǳ�����ƽ̨ʧ�ܣ������ƽ̨�����������쳣');
    };
    sendAjax(url, successFunc, failureFunc);
}

/*******************************************����ajax����*****************************************************************/
function sendAjax(url, successFunc, failureFunc) {
    var xmlHttp;
    if (window.ActiveXObject) {
        xmlHttp = new ActiveXObject("Microsoft.XMLHTTP")
    }
    else if (window.XMLHttpRequest) {
        xmlHttp = new XMLHttpRequest()
    }


    xmlHttp.open("POST", url, true);

    xmlHttp.onreadystatechange = function () {
        try {
            if ((xmlHttp.readystate == 4) && (xmlHttp.status == 200)) {
                if (successFunc) {
                    successFunc(xmlHttp);
                }
            }
            else {
                if (failureFunc) {
                    failureFunc();
                }
            }
        }
        catch (err) {
            log.info(err);

        }
    }
    xmlHttp.send();
}

function contorlPanelDown(dom) {
    dom.childNodes[0].style.display = "none";
    dom.childNodes[1].style.display = "block";
}
function contorlPanelUp(dom) {
    dom.childNodes[0].style.display = "block";
    dom.childNodes[1].style.display = "none";
}
function onClose() {
    //stopPlayerStream();
    //�ر�ҳ��ʱ��ע����ǰʹ�õ��û�		
    logoutPMPlatform(sysname, citizenid, password, policeID);
    UActr.Close();
    sleep(500);


    //alert("�ر���Ƶ����ҳ��");

}
function onPlayControl(opt) {
    switch (opt) {
        case "play":
            if (curWinDeviceID != "") {
                playVideo(sysname, citizenid, password, curWinDeviceID, policeID);
            }
            break;
        case "stop":
            //alert("ֹͣ"); 			
            stopRealtimeVideo(sysname, citizenid, password, getcurrentChooseWindow(), policeID);
            break;

        case "logout":
            //alert("�ǳ�����ƽ̨");
            logoutPMPlatform(sysname, citizenid, password, policeID);
            break;

        case "login":
            playVideoOfPMPlatform(sysname, citizenid, password, "", policeID, true);
            break;


        case "history":
            if (!VideoForPGIS_HistoryWin) {
                var vs = new AXY.Monitoring.VideoSearch({
                    rowDoubleClickHandler: function () {
                        //�˴����˫�������߼�
                        var selectedRec = this.getSelectedRecord()[0];
                        var prerecordTime = selectedRec.get("prerecordTime"); //Ԥ¼ʱ��
                        var startTimeValue = selectedRec.get("startTime");
                        var stopTimeValue = selectedRec.get("stopTime");
                        var sourceDeviceValue = selectedRec.get("sourceDeviceID");
                        var storageServerValue = selectedRec.get("storageServerID");
                        var sourceDeviceName = selectedRec.get("sourceDevice");
                        var sort = selectedRec.get("sort");
                        log.info("¼������Ϊ:" + sort);
                        log.info("�洢������ID �����������У�storageServerValue��" + storageServerValue);
                        log.info("startTimeValue:" + startTimeValue);
                        log.info("stopTimeValue:" + stopTimeValue);
                        playHistoryVideo4GIS(startTimeValue, stopTimeValue, prerecordTime, sourceDeviceValue, sourceDeviceName, storageServerValue, sort);


                    },
                    getPageBar: function () {
                        return new Ext.PagingToolbar({
                            store: this.resultGridStore,
                            pageSize: this.pageSize,
                            displayInfo: false,
                            beforePageText: "��",
                            emptyMsg: "û�в�ѯ����¼"
                        });
                    }
                });

                VideoForPGIS_HistoryWin = new Ext.Window({
                    title: '',
                    layout: 'fit',
                    width: 610,
                    height: 506,
                    closeAction: 'hide',
                    items: vs
                });
            }
            VideoForPGIS_HistoryWin.show();
            break;

    }
}

function sleep(n) {
    var start = new Date().getTime();
    while (true) if (new Date().getTime() - start > n) break;
}

function getCurrentWinDeviceID() {
    return curWinDeviceID;
}

//�ӷ������˼����豸��Ԥ��λ
function loadPresetAndSwitch() {
    var deviceID = curWinDeviceID;
    presetPositionGridStore.load({ params: { deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID } });
    switchCombo.store.load({ params: { deviceID: deviceID, sysname: sysname, password: password, citizenid: citizenid, policeID: policeID } });
}

function clearPresetAndSwitch() {
    presetPositionGridStore.removeAll();
    switchCombo.store.removeAll();
}
/***********************************************����/����û���ǰѡ�еĴ���***********************************************/
function setcurrentChooseWindow(win_id) {
    currentChooseWindow = win_id;
}
function getcurrentChooseWindow() {
    return currentChooseWindow;
}

/***********************************************�����û����VLC������ڵ��¼�***********************************************/
function onClickVLCWindow() {
    log.info("in VideoForPGIS player2.js onClickVLCWindow");
    var newChooseWindow = vlcplayer.JsManualWinID();
    log.info("�û�ѡ��ĵ�ǰ�����ǣ�" + newChooseWindow);
    if (newChooseWindow != getcurrentChooseWindow()) {
        setcurrentChooseWindow(newChooseWindow);//�����û���ǰѡ�еĴ���
        var subjectStr = subjectArray[newChooseWindow];
        log.info("in VideoForPGIS player2.js onClickVLCWindow,subjectStr:" + subjectStr);
        if (subjectStr != "") {
            var array = subjectStr.split(",");
            array = array[0].split(":");
            var deviceID = array[0];//���û���ѡ��Ĵ��ڵ�ý���ʶ�н����������ID
            log.info("�û�ѡ��ĵ�ǰ�����ǣ�" + newChooseWindow + "��Ӧ��������ǣ�" + deviceID);
            curWinDeviceID = deviceID;//���õ�ǰ�豸Ϊ���������ID
            loadPresetAndSwitch();//���ص�ǰ�豸��Ԥ��λ
        }
        else {
            clearPresetAndSwitch();
        }
    }
}

/*******************************************************��ÿͻ���IP***********************************************/
function getLocalIPAddr() {
    var oSetting = null;
    var ip = "";
    try {
        oSetting = new ActiveXObject("rcbdyctl.Setting");
        ip = oSetting.GetIPAddress;

        if (ip.length == 0) {
            return "û�����ӵ�����";
        }
        oSetting = null;
    } catch (e) {

    }
    log.info("�ͻ���IP�ǣ�" + ip);
    return ip;
}
//��������˷���Ajax������Ϣ��by zhf 2012��8��16��15:41:32
function sendHeartBeats() {
    log.info("��֪�������˿ͻ�������");
    Ext.Ajax.request({
        url: PROJECT_NAME + '/specialLogon.do',
        params: { operation: 'sendHearBeats' }
    });
    setTimeout(function () { sendHeartBeats(); }, 60000);
}

//startTime ��Ƶ���Ŀ�ʼʱ��
//stopTime ��Ƶ���Ľ���ʱ��
//prerecordTime Ԥ¼ʱ��
//sourceDevice ��������豸ID
//storageServerID �洢���������豸ID��һ�����ڿ��ܰ�������洢��������
//sourceDeviceName ����������ƣ�2008��11��20�գ�������һ��name�Ĳ�����Ϊ����ʷ�㲥��ʱ�򴫵��豸���ƣ�����ʾ�ڲ����������ϣ�
//sortý������
function playHistoryVideo4GIS(startTime, stopTime, prerecordTime, sourceDevice, sourceDeviceName, storageServerID, sort) {
    log.info("in player2.js playHistoryVideo4GIS");
    var url = PROJECT_NAME + "/monitoring/PGISVideoManage.do?operation=playback"
                                                + "&sysname=" + sysname
                                                + "&citizenid=" + citizenid
                                                + "&password=" + password
                                                + "&policeID=" + policeID;
    if (window.vlcplayer) {
        //log.info("ý�岥�����Ѽ���");
    } else {
        //log.info("ý�岥������δ���أ�����ý�岥����");	
        showPlayWin();
    }
    //�����������������������δ���أ�˵���û�û��Ȩ�޻�������������⣬��ֱ�ӷ���
    if (!window.vlcplayer) {
        return;
    }

    /********************************************************************************************************/
    /*˵����  ��1����PGIS��ʵʱ��Ƶ���Ƿ������Զ�ѡ����д��ڵġ�����ͳһ�ķ��PGIS��ʷ��Ƶ�ط�ҲӦ���Ƿ������Զ�ѡ����д��ڵġ�
             ��2��ĿǰPGIS��ʷ��Ƶ�ط�ʹ�÷������Զ�ѡ����д��ڿ����������С������������ȿ��ƿ��ܻ������⣬
             ��3���˴�ע�͵��Ĵ���Ϊ��ǰ���õ�ǰ���ڲ�����ʷ��Ƶ�Ĵ��롣ֱ������win_IDΪ1  2011��3��17��14:41:13  zhf  */

    /*//��õ�ǰ����
    var  win_ID = vlcplayer.windowID;
    playBackType[win_ID]="common";
    //�����������ĵ�ǰ����
    vlcplayer.JsSleep();
    //��ʱ5���Ӻ��ͷŲ�������ǰ���ڣ�Ŀ���Ǳ��������û�б��㲥��������ɴ�������
    setTimeout(function(){vlcplayer.JsWakeUp();},3000);*/
    var win_ID = 1;
    /********************************************************************************************************/


    /***************������Ƶ���Ŀ�ʼʱ��ͽ���ʱ�䣬��������Ƶ���ĳ���***************/
    var period = 0;
    var sTime = new Date();
    var eTime = sTime;
    if (startTime != null && startTime != "") {
        sTime = new Date(startTime.split("-").join("/"));
    }
    if (stopTime != null && stopTime != "") {
        eTime = new Date(stopTime.split("-").join("/"));
    }
    log.info("startTime:" + startTime + "stopTime" + stopTime);
    log.info("��ʼʱ�䣺" + sTime + "����ʱ��:" + eTime);
    var num_prerecordTime = new Number(prerecordTime); //�ַ���ת������ֵ���ܽ�������������
    /********************************************************************************************************/
    /*˵����  ��1����PGIS��ʵʱ��Ƶ���Ƿ������Զ�ѡ����д��ڵġ�����ͳһ�ķ��PGIS��ʷ��Ƶ�ط�ҲӦ���Ƿ������Զ�ѡ����д��ڵġ�
             ��2��ĿǰPGIS��ʷ��Ƶ�ط�ʹ�÷������Զ�ѡ����д��ڿ����������С������������ȿ��ƿ��ܻ������⣬
             ��3���˴�ע�͵��Ĵ���Ϊ��ǰ���õ�ǰ���ڲ�����ʷ��Ƶ�Ĵ��롣2011��3��17��14:41:13  zhf  */
    //periodArray[win_ID] = (eTime.getTime()-sTime.getTime())/1000 + num_prerecordTime; 

    //log.info("����"+win_ID+"���ŵ���ʷ��Ƶ��ʱ���ǣ���Ԥ¼ʱ�䣩��"+periodArray[win_ID] + "��Ԥ¼ʱ����" +prerecordTime+",¼������:"+sort);
    /********************************************************************************************************/
    /*********************��������˷��ͽ�����ʷ��Ƶ���ӵ�����*******************/
    Ext.Ajax.request({
        url: url,
        params: {
            startTime: startTime,
            stopTime: stopTime,
            prerecordTime: prerecordTime,
            sourceDevice: sourceDevice,
            storageServerID: storageServerID,
            sort: sort,
            win_ID: win_ID
        },
        success: function (response) {
            if (response.responseText != "") {//2009-10-05 ����ͳһ�޸� �ύ�ɹ��󣬷�������Ӧ��Ϣ
                Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: response.responseText, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.WARNING, minWidth: SysConst.MESSAGEBOXMINWIDTH });
            }
            //2009��9��26��18:48:15 by liufeng ע�͵�����
            //subject(text);//Ŀǰͳһ��SIP����յ�invite��Ϣ�����subject�������˴����ÿɲ�Ҫ
        },
        failure: function (form, action) {
            Ext.Msg.show({ title: SysConst.PROMPTINFOTEXT, msg: SysConst.OPERATEFAILUREINFOTEXT, buttons: Ext.MessageBox.OK, icon: Ext.MessageBox.INFO, minWidth: SysConst.MESSAGEBOXMINWIDTH });
        }
    });
    /********************************************************************************************************/
    /*˵����  ��1����PGIS��ʵʱ��Ƶ���Ƿ������Զ�ѡ����д��ڵġ�����ͳһ�ķ��PGIS��ʷ��Ƶ�ط�ҲӦ���Ƿ������Զ�ѡ����д��ڵġ�
             ��2��ĿǰPGIS��ʷ��Ƶ�ط�ʹ�÷������Զ�ѡ����д��ڿ����������С������������ȿ��ƿ��ܻ������⣬
             ��3���˴�ע�͵��Ĵ���Ϊ��ǰ���õ�ǰ���ڲ�����ʷ��Ƶ�Ĵ��롣2011��3��17��14:41:13  zhf  */
    /*//�˴���״̬������Ϊ�˿��ƻ��ͣ����û���������ʼ�����������ڲ���յ�smp��Ϣ�󣬲Ž�״̬��Ϊ"htm_play"����ʼ����
    statusArray[win_ID] = "htm_stop";
    ////showPlayWin();
    //�Ѵ˴��ڶ�Ӧ�Ļ���λ�÷�����ʼ0λ��
    posArray[win_ID]=0;	
    deviceNameArray[win_ID] = sourceDeviceName;
    speedArray[win_ID]=1.0;
    storageServerIDArray[vlcplayer.windowID] = storageServerID; 
    log.info("�洢��������ʶ�洢���");*/
    /********************************************************************************************************/
}