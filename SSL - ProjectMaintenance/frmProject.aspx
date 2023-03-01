<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="frmProject.aspx.vb" Inherits="SSL___ProjectMaintenance.frmProject" Culture="en-GB" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Project Maintenance</title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/CalendarTheme.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="App_Themes/popupTheme.css" />
    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->


    <style type="text/css">
        .selector-for-some-widget {
            box-sizing: content-box;
        }

        html, body {
            height: 100%;
        }

        #Contain {
            border: 1px solid Gray;
        }

        #tophead {
            border: 1px solid Gray;
            margin: 20px;
            background: white;
            border-radius: 9px;
            box-shadow: 3px 3px 3px rgba(0, 0, 0, 0.5);
        }

        #Lpanel, #PanelHistory {
            border: 1px solid Gray;
            border-radius: 9px;
        }

        #PanelR, #pnSave {
            border: 1px solid Gray;
            background: white;
            border-radius: 9px;
        }
        /*gridview*/ .table table tbody tr td a, .table table tbody tr td span {
            position: relative;
            float: left;
            padding: 2px 2px;
            margin-left: -1px;
            line-height: 1.42857143;
            color: #337ab7;
            text-decoration: none;
            background-color: #fff;
            border: 1px solid #ddd;
        }

        .table table > tbody > tr > td > span {
            z-index: 3;
            color: #fff;
            cursor: default;
            background-color: #337ab7;
            border-color: #337ab7;
        }

        .table table > tbody > tr > td:first-child > a, .table table > tbody > tr > td:first-child > span {
            margin-left: 0;
            border-top-left-radius: 2px;
            border-bottom-left-radius: 2px;
        }

        .table table > tbody > tr > td:last-child > a, .table table > tbody > tr > td:last-child > span {
            border-top-right-radius: 2px;
            border-bottom-right-radius: 2px;
        }

        .table table > tbody > tr > td > a:hover, .table table > tbody > tr > td > span:hover, .table table > tbody > tr > td > a:focus, .table table > tbody > tr > td > span:focus {
            z-index: 2;
            color: #23527c;
            background-color: #eee;
            border-color: #ddd;
        }
         #pdf_container { background: #ccc; text-align: center; display: none; padding: 5px; height: 820px; overflow: auto; }

    </style>

    <%--<script src="Scripts/jquery-3.0.0.js" type="text/javascript"></script>--%>

<%--    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.6.347/pdf.min.js"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.6.347/pdf_viewer.min.css" rel="stylesheet" type="text/css" />--%>



    <script type="text/javascript">

        //$(function () {
        //    $("[id*=gvFiles] .view").click(function () {
        //        var fileId = $(this).attr("rel");
        //        $.ajax({
        //            type: "POST",
        //            url: "http://sslvmsql1/ws_mfgsag/Project.asmx/GetPDF",
        //            data: "{ProjectCode: " + fileId + "}",
        //            contentType: "application/json; charset=utf-8",
        //            dataType: "json",
        //            success: function (r) {
        //                LoadPdfFromBlob(r.d.Data);
        //            }
        //        });
        //    });
        //});

        //var pdfjsLib = window['pdfjs-dist/build/pdf'];
        //pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.6.347/pdf.worker.min.js';
        //var pdfDoc = null;
        //var scale = 1; //Set Scale for zooming PDF.
        //var resolution = 1; //Set Resolution to Adjust PDF clarity.

        //function LoadPdfFromBlob(blob) {
        //    //Read PDF from BLOB.
        //    pdfjsLib.getDocument({ data: blob }).promise.then(function (pdfDoc_) {
        //        pdfDoc = pdfDoc_;

        //        //Reference the Container DIV.
        //        var pdf_container = document.getElementById("pdf_container");
        //        pdf_container.innerHTML = "";
        //        pdf_container.style.display = "block";

        //        //Loop and render all pages.
        //        for (var i = 1; i <= pdfDoc.numPages; i++) {
        //            RenderPage(pdf_container, i);
        //        }
        //    });
        //};
        //function RenderPage(pdf_container, num) {
        //    pdfDoc.getPage(num).then(function (page) {
        //        //Create Canvas element and append to the Container DIV.
        //        var canvas = document.createElement('canvas');
        //        canvas.id = 'pdf-' + num;
        //        ctx = canvas.getContext('2d');
        //        pdf_container.appendChild(canvas);

        //        //Create and add empty DIV to add SPACE between pages.
        //        var spacer = document.createElement("div");
        //        spacer.style.height = "20px";
        //        pdf_container.appendChild(spacer);

        //        //Set the Canvas dimensions using ViewPort and Scale.
        //        var viewport = page.getViewport({ scale: scale });
        //        canvas.height = resolution * viewport.height;
        //        canvas.width = resolution * viewport.width;

        //        //Render the PDF page.
        //        var renderContext = {
        //            canvasContext: ctx,
        //            viewport: viewport,
        //            transform: [resolution, 0, 0, resolution, 0, 0]
        //        };

        //        page.render(renderContext);
        //    });
        //};

        //$(document).ready(function() {
        //    // ทำให้ event ทำงาน
        //    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        //    function EndRequestHandler(sender, args) {
        //        li_click();
        //    }
        //});
        //เวลาคลิกกรุ๊ปจะเก็บใส่ cookie ไว้
        function li_click() {
            $("ul[id*=ulGroup] li").click(function () {
                setCookie("GroupClick", $(this).text());

            });
        }
        //สร้าง cookie
        function setCookie(name, value, expires, path, domain, secure) {
            document.cookie = name + "=" + escape(value) +
	        ((expires) ? ";	expires=" + expires.toGMTString() : "") +
    	    ((path) ? "; path=" + path : "") +
	        ((domain) ? "; domain=" + domain : "") +
	        ((secure) ? "; secure" : "");
        }
        // Confirm delete
        function Confirm() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            if (confirm("Do you want to delete data?")) {
                confirm_value.value = "Yes";
                showPleaseWait();
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }

        // History
        function ConfirmHistory() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            if (confirm("Do you want to delete data?")) {
                confirm_value.value = "Yes";
                showPleaseWait();
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }

        // Confirm delete
        //        function ConfirmEffective() {
        //            var confirm_value = document.createElement("INPUT");
        //            confirm_value.type = "hidden";
        //            confirm_value.name = "confirm_value";
        //            if (confirm("Do you want to Confirm Effective Date ?")) {
        //                confirm_value.value = "Yes";
        //                showPleaseWait();
        //            } else {
        //                confirm_value.value = "No";
        //            }
        //            document.forms[0].appendChild(confirm_value);
        //        }

        function Validate() {
            if (Page_ClientValidate('1')) {
                hidePopResign();
                if (confirm('Do you want to save data?')) {
                    showPleaseWait();
                    return true;
                } else {
                    return false;
                }
            }
        }

        function ValidateHistory() {
            if (Page_ClientValidate('1')) {
                if (confirm('Do you want to save data?')) {
                    showPleaseWait();
                    return true;
                } else {
                    return false;
                }
            }
        }

        /**
        * Displays overlay with "Please wait" text. Based on bootstrap modal. Contains animated progress bar.
        */
        function showPleaseWait() {
            //hidePleaseWait()
            document.body.style.cursor = 'wait';
            var modalLoading = '<div class="modal" id="pleaseWaitDialog" data-backdrop="static" data-keyboard="false role="dialog">\
                                <div class="modal-dialog">\
                                    <div class="modal-content">\
                                        <div class="modal-header">\
                                            <h4 class="modal-title">Please wait...</h4>\
                                        </div>\
                                        <div class="modal-body">\
                                            <div class="progress">\
                                              <div class="progress-bar progress-bar-success progress-bar-striped active" role="progressbar"\
                                              aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width:100%; height: 40px">\
                                              </div>\
                                            </div>\
                                        </div>\
                                    </div>\
                                </div>\
                            </div>';
            $(document.body).append(modalLoading);
            //alert("test")
            $("#pleaseWaitDialog").modal("show");
        }

        /**
        * Hides "Please wait" overlay. See function showPleaseWait().
        */
        function hidePleaseWait() {
            $('#pleaseWaitDialog').modal('hide');
            //hide the modal

            $('body').removeClass('modal-open');
            //modal-open class is added on body so it has to be removed

            $('.modal-backdrop').remove();
            //need to remove div with modal-backdrop class
            document.body.style.cursor = 'default';
        }
        function hidePopResign() {
            $('#popModal').modal('hide');
        }

        function hidePopEffective() {
            $('#ModalEffective').modal('hide');
        }

        function ClickScan() {
            document.getElementById("btnScan").click();
        }

        //        function ShowPopFinger(elementRef) {
        //            document.getElementById("btnConfirm").disabled = true;
        //            document.getElementById("txtEffectivePop").disabled = true;
        //            document.getElementById("lblEmpID").innerHTML = '';
        //            document.getElementById("lblName").innerHTML = '';
        //            console.log("showPop");
        //            $find('ModalPopupFinger').show();
        //            
        //        }

        function OnClick(elementRef) {
            var ddl = document.getElementById(elementRef.id);
            var SelVal = ddl.options[ddl.selectedIndex].value;
            var Chkcontrol = document.getElementById('HFcontrolchange').value
            var HFbeforchange = document.getElementById('HF' + elementRef.id).value;

            if (HFbeforchange == '' || Chkcontrol != elementRef.id) {
                document.getElementById('HF' + elementRef.id).value = SelVal;
            }
        }

        function OnClickPDF(elementRef) {
            var fileId = document.getElementById(elementRef.id);
            $.ajax({
                type: "POST",
                url: "http://sslvmsql1/ws_mfgsag/Project.asmx?op=GetPDF",
                data: "{ProjectCode: " + fileId.substring(3) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    LoadPdfFromBlob(r.d.Data);
                }
            });

        }

        function OnFocus(elementRef) {
            var txt = document.getElementById(elementRef.id).value;
            document.getElementById('HF' + elementRef.id).value = txt;
            console.log("OnFocus");
            console.log(txt);
        }

        function OnChangeSecurityRule(elementRef) {
            var vStatus = document.getElementById('lbStatus').innerText;
            console.log("OnChangeSecurityRule");
            //            document.getElementById('lblEmpID').innerHTML = '';
            //            document.getElementById('lblName').innerHTML = '';
            //            document.getElementById('lblFingerPrint').innerHTML = '';
            //            
            if (vStatus == "Edit") {
                //            var prev = $(elementRef).data("prev");
                //            $(elementRef).data("prev", $(elementRef).val());
                var Controltype = document.getElementById(elementRef.id)
                //Update hidden field with current value from previous value

                //            if (prev != undefined) {
                //                $('#HF' + elementRef.id).val(prev);
                //            }
                $('#HFcontrolchange').val(elementRef.id);
                $('#HFcontroltype').val(Controltype.type);
                $('#HFcontroltypeData').val('String');

                var label = document.getElementById('lb' + elementRef.id).innerText.replace(":", "")
                document.getElementById('lblScanFor').innerText = label;

                document.getElementById('txtEffectivePop').style.visibility = "hidden";
                document.getElementById('lbEffective').style.visibility = "hidden";

                var d = new Date();
                var day = ('0' + d.getDate()).slice(-2);
                var month = ('0' + (d.getMonth() + 1)).slice(-2);
                var year = d.getFullYear();
                var n = day + '/' + month + '/' + year;

                document.getElementById('txtEffectivePop').value = n;

                $find('ModalPopupFinger').show();
            }
        }

        function OnChangeSecurityRuleCalendar(elementRef) {
            var vStatus = document.getElementById('lbStatus').innerText;
            //console.log(vStatus);
            if (vStatus == "Edit") {
                //            var prev = $(elementRef).data("prev");
                //            $(elementRef).data("prev", $(elementRef).val());
                var Controltype = document.getElementById(elementRef.id)
                //Update hidden field with current value from previous value

                //            if (prev != undefined) {
                //                $('#HF' + elementRef.id).val(prev);
                //            }
                $('#HFcontrolchange').val(elementRef.id);
                $('#HFcontroltype').val(Controltype.type);
                $('#HFcontroltypeData').val('Calendar');

                var label = document.getElementById('lb' + elementRef.id).innerText.replace(":", "")
                document.getElementById('lblScanFor').innerText = label;

                document.getElementById('txtEffectivePop').style.visibility = "hidden";
                document.getElementById('lbEffective').style.visibility = "hidden";

                var d = new Date();
                var day = ('0' + d.getDate()).slice(-2);
                var month = ('0' + (d.getMonth() + 1)).slice(-2);
                var year = d.getFullYear();
                var n = day + '/' + month + '/' + year;

                document.getElementById('txtEffectivePop').value = n;

                $find('ModalPopupFinger').show();
            }
        }

        function OnChange(elementRef) {
            var vStatus = document.getElementById('lbStatus').innerText;
            //console.log(vStatus);
            if (vStatus == "Edit") {
                //var prev = $(elementRef).data('prev');
                //$(elementRef).data('prev', $(elementRef).val());
                var Controltype = document.getElementById(elementRef.id);
                //alert(prev)
                //if (prev != undefined) {
                //$('#HF' + elementRef.id).val(prev);
                //}
                $('#HFcontrolchange').val(elementRef.id);
                $('#HFcontroltype').val(Controltype.type);
                $('#HFcontroltypeData').val('String');

                var d = new Date();
                var day = ('0' + d.getDate()).slice(-2);
                var month = ('0' + (d.getMonth() + 1)).slice(-2);
                var year = d.getFullYear();
                var n = day + '/' + month + '/' + year;
                document.getElementById('txtEffective').defaultValue = n;

                //document.getElementById('txtEffective').value = new Date(); 
                //$('#ModalEffective').modal('show');
                $('#ModalEffective').modal({ backdrop: 'static', keyboard: false });
            }
        }

        function OnChangeCalendar(elementRef) {
            var vStatus = document.getElementById('lbStatus').innerText;
            //console.log(vStatus);
            if (vStatus == "Edit") {
                //            var prev = $(elementRef).data('prev');
                //            $(elementRef).data('prev', $(elementRef).val());
                var Controltype = document.getElementById(elementRef.id);

                //            if (prev != undefined) {
                //                $('#HF' + elementRef.id).val(prev);
                //            }
                $('#HFcontrolchange').val(elementRef.id);
                $('#HFcontroltype').val(Controltype.type);
                $('#HFcontroltypeData').val('Calendar');

                var d = new Date();
                var day = ('0' + d.getDate()).slice(-2);
                var month = ('0' + (d.getMonth() + 1)).slice(-2);
                var year = d.getFullYear();
                var n = day + '/' + month + '/' + year;
                document.getElementById('txtEffective').defaultValue = n;

                $('#ModalEffective').modal({ backdrop: 'static', keyboard: false });
            }
        }

        function CalendarExtenderShow(sender, args) {
            //console.log($('#HF' + sender.get_id().substring(3)).val());
            var ValueBefor = document.getElementById('HF' + sender.get_id().substring(3)).value;
            //console.log(ValueBefor);
            $('#HFbeforchangeCalendar').val(ValueBefor);
        }

        function ClosePop() {
            var ValueBefor = document.getElementById('HF' + document.getElementById('HFcontrolchange').value).value;
            var ValueBeforCalendar = document.getElementById('HFbeforchangeCalendar').value;
            var ChkTypeData = document.getElementById('HFcontroltypeData').value

            if (ChkTypeData == 'String') {
                document.getElementById(document.getElementById('HFcontrolchange').value).value = ValueBefor;
            } else {
                document.getElementById(document.getElementById('HFcontrolchange').value).value = ValueBeforCalendar;
                document.getElementById('PanelR').focus();
            }
            document.getElementById("btnConfirm").disabled = true;
            document.getElementById("txtEffectivePop").disabled = true;
            document.getElementById("lblEmpID").innerHTML = '';
            document.getElementById("lblName").innerHTML = '';


            document.getElementById('HFcontrolchange').value = '';

            //            console.log('ClosePop');
            //            console.log(ValueBefor);
        }

    </script>

</head>
<body>
    <div class="container-fluid" id="Contain" style="background-color: #e6ffff">
        <form id="form1" runat="server">
            <AjaxToolkit:ToolkitScriptManager runat="server"></AjaxToolkit:ToolkitScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <%-- BEGIN head          --%>
                    <div class="row col-lg-offset-3  ">
                        <div class="row col-lg-4" id="tophead">
                            <div class="row" style="text-align: center">
                                <asp:Label ID="Label6" runat="server" Text="Project Maintenance" Font-Bold="True"
                                    Font-Size="Medium"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Label ID="lbStatus" runat="server" Text="" Style="display: none;"></asp:Label>
                        <asp:Label ID="lbStatusHistory" runat="server" Text="" Style="display: none;"></asp:Label>
                        <asp:Label ID="lbDataKey" runat="server" Text="" Style="display: none;"></asp:Label>
                    </div>
                    <div class="row">
                        <div class="col-lg-4" id="Lpanel" style="background-color: #f2ffcc; height: 850px">
                            <br />
                            <center>
                                <div class="form-group form-inline">
                                    <%--  <asp:CheckBox ID="chkStatus" Text="Active Employee" runat="server" OnCheckedChanged="Check_Clicked"
                                    AutoPostBack="True" />
                                &nbsp;&nbsp;&nbsp;--%>
                                    <asp:TextBox ID="txtSearch" CssClass="form-control " runat="server"></asp:TextBox>
                                    <asp:Button ID="btnSearch" CssClass="btn btn-default btn-sm" runat="server" Text="Search" />
                                    &nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnAdd" CssClass="btn btn-default btn-sm" runat="server" Text="New Project"
                                    OnClick="Btn_ClickAdd" />
                                    <%--OnClientClick="showPleaseWait(); " --%>
                                        &nbsp; &nbsp; &nbsp;
                                    <asp:Button ID="cmdTransfer" CssClass="btn btn-primary btn-sm" runat="server" Text="Transfer to Sage"
                                        OnClick="Btn_ClickTranfer" OnClientClick="showPleaseWait();" />

                                </div>
                                <%--ตัวอักษร A-Z--%>
                                <div class="AlphabetPager">
                                    <asp:Repeater ID="rptAlphabets" runat="server">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LinkButton1" runat="server" Text='<%#Eval("Value")%>' Visible='<%#Not Convert.ToBoolean(Eval("Selected"))%>'
                                                OnClick="Alphabet_Click" />
                                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("Value")%>' Visible='<%# Convert.ToBoolean(Eval("Selected"))%>' />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <br />
                                <center>
                                    <div id="result">
                                        <asp:Label ID="lblResult" runat="server" Text="This is Result" Font-Bold="True"></asp:Label>
                                    </div>
                                </center>
                                <br />
                                <asp:GridView ID="gvData" runat="server" CssClass="table table-striped table-bordered table-hover"
                                    CellPadding="2" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False"
                                    HorizontalAlign="Center" OnPageIndexChanging="ShowPageCommand" AllowPaging="True"
                                    PageSize="15">
                                    <EmptyDataTemplate>
                                        <table cellspacing="0" rules="all" border="0" style="border-collapse: collapse;">
                                            <tr>
                                                <td colspan="99" align="center">No records found for the search criteria.
                                                </td>
                                            </tr>
                                        </table>
                                    </EmptyDataTemplate>
                                    <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                                    <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                    <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                                    <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" Height="30px" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </center>
                        </div>
                        <div class="col-lg-4" id="Rpanel">
                            <%--panel ด้านขวา --%>
                            <asp:Panel CssClass="col-lg-12" ID="PanelR" Height="810" runat="server" BackColor="White"
                                Visible="false" ScrollBars="Vertical">
                              <%--  <asp:Panel ID="pnlFormUpload" Visible="true" runat="server" Style="margin-bottom: 0px; padding-top: 10px">
                                    <center>
                                        <div>
                                            <asp:FileUpload ID="Attachment1" runat="server" Width="500px" />
                                            <asp:Button ID="btnUpload1" CssClass="btn btn-default btn-xs " runat="server" Text="Upload Attachment1"
                                                AutoPostBack="True" />
                                        </div>

                                    </center>
                                </asp:Panel>--%>
                                <%--  กรุ๊ป--%>
                                <div class="row form-inline col-lg-12" id="GCon">
                                    <ul id="ulGroup" class="nav nav-tabs" runat="server" style="font-size: x-small">
                                    </ul>
                                    <br />
                                    <%--   control แต่ล่ะกรุ๊ป--%>
                                    <div class="tab-content" id="divContent" runat="server">
                                    </div>
                                    <br />
                                </div>
                            </asp:Panel>
                            <asp:Panel CssClass="col-lg-12" ID="pnSave" Height="40" runat="server" Visible="False"
                                BackColor="White">
                                <div class="row form-group" style="padding-top: 4px">
                                    <center>
                                        <asp:Button ID="cmdSave" CssClass="btn btn-success btn-sm" runat="server" Text="Save"
                                            OnClientClick="return  Validate()" OnClick="Btn_ClickSave" ValidationGroup="1" />
                                        &nbsp; &nbsp; &nbsp;
                                    <asp:Button ID="cmdDelete" CssClass="btn btn-danger btn-sm" runat="server" Text="Delete"
                                        OnClick="Btn_ClickDelete" OnClientClick=" Confirm(); " />
                                        &nbsp; &nbsp; &nbsp;
                                    <asp:Button ID="cmdCancelSave" CssClass="btn btn-default btn-sm" runat="server" Text="Cancel"
                                        OnClick="Btn_ClickCancel" />

                                    </center>
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="col-lg-4" >
                            <iframe id="iframePDF" visible ="false"   runat="server" scrolling="no" frameborder="0" style="border:none; overflow:hidden;" height="800" width="600"></iframe>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="cmdSave" />
                </Triggers>
            </asp:UpdatePanel>
            <asp:HiddenField ID="HDduplicate" runat="server" />
            <asp:HiddenField ID="HFcontrolchange" runat="server" />
            <asp:HiddenField ID="HFbeforchangeCalendar" runat="server" />
            <asp:HiddenField ID="HFcontroltype" runat="server" />
            <asp:HiddenField ID="HFcontroltypeData" runat="server" />
            <br />
            <br />
      <%--      <div>
                <asp:UpdatePanel ID="udpUpdatePanel" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnConfirm" />
                    </Triggers>
                    <ContentTemplate>
                        <AjaxToolkit:ModalPopupExtender ID="ModalPopupFinger" runat="server" BackgroundCssClass="ModalPopupBackground"
                            TargetControlID="hypOpenFake" OkControlID="hypConfirmFake" Drag="true" PopupDragHandleControlID="divPopupHeaderFinger"
                            PopupControlID="divPanelPopUpFinger">
                 
                            <Animations>
                <OnShown>
                 <ScriptAction Script="ClickScan();" /> 
                </OnShown>
                            </Animations>
                        </AjaxToolkit:ModalPopupExtender>
                        <asp:HyperLink ID="hypOpenFake" runat="server"></asp:HyperLink>
                        <asp:HyperLink ID="hypConfirmFake" runat="server"></asp:HyperLink>
                        <div id="divPanelPopUpFinger" class="popupLogin">
                            <div class="popup_Container">
                                <div class="popup_Titlebar" id="divPopupHeaderFinger">
                                    <div id="divTitlePopupTime" class="TitlebarLeft">
                                        <asp:Label ID="lblTitlePopupLeave" runat="server" Text="FingerPrint Scan"></asp:Label>&nbsp;&nbsp;&nbsp;
                                    -
                                    <asp:Label ID="lblScanFor" runat="server" Text="" ForeColor="Yellow" EnableViewState="true"></asp:Label>
                                    </div>
                                  
                                </div>
                                <div class="popup_Body">
                                    <table class="popUp" width="250px">
                                        <tr>
                                            <td>
                                                <asp:UpdatePanel ID="UpdatePanelButton" runat="server">
                                                    <ContentTemplate>
                                                        <asp:Label ID="Label4" runat="server">Please scan Finger to confirm</asp:Label>
                                                        &nbsp;&nbsp;&nbsp;
                                                    <asp:Button ID="btnScan" runat="server" CssClass="btn btn-default btn-sm" Text="Scan"
                                                        Style="height: 26px" AutoPostBack="true" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:UpdateProgress ID="UpdateProgressScan" runat="server" AssociatedUpdatePanelID="UpdatePanelButton">
                                                    <ProgressTemplate>
                                                        <asp:Image ID="ImageScan" runat="server" ImageUrl="~/images/Fingerprint.gif" Width="120"
                                                            Height="160px" />
                                                    </ProgressTemplate>
                                                </asp:UpdateProgress>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:UpdatePanel ID="UpdatePanelPressScanButton" runat="server">
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="btnScan" EventName="Click" />
                                                    </Triggers>
                                                    <ContentTemplate>
                                                        <asp:Label ID="lblEmpID" runat="server" ForeColor="Blue"></asp:Label>&nbsp;:&nbsp;
                                                    <asp:Label ID="lblName" runat="server" ForeColor="Blue"></asp:Label>
                                                        <br />
                                                        <asp:Label ID="lblFingerPrint" runat="server" ForeColor="Red"></asp:Label>
                                                        <br />
                                                        <div class="row form-inline form-group">
                                                            <asp:Label ID="lbEffective" runat="server" Text="Effective Date :" CssClass="control-label"
                                                                Width="100px"></asp:Label>
                                                            <asp:TextBox ID="txtEffectivePop" runat="server" CssClass="form-control input-sm"
                                                                Width="100px"></asp:TextBox>
                                                            <AjaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtEffectivePop"
                                                                Format="dd/MM/yyyy" PopupButtonID="txtEffectivePop">
                                                            </AjaxToolkit:CalendarExtender>

                                                        </div>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <asp:UpdatePanel ID="UpdatePanelSubmit" runat="server">
                                                    <ContentTemplate>
                                                        <asp:Button ID="btnConfirm" CssClass="btn btn-default btn-sm btn-success" runat="server"
                                                            Text="Confirm" Style="height: 26px" Enabled="False" AutoPostBack="True" />&nbsp;&nbsp;
                                                 
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                                <asp:UpdateProgress ID="UpdateProgressLoad" runat="server" AssociatedUpdatePanelID="UpdatePanelSubmit">
                                                    <ProgressTemplate>
                                                        <asp:Image ID="ImageLoading" runat="server" ImageUrl="~/images/waiting.gif" />
                                                        <br />
                                                        <asp:Label ID="lblLoading" runat="server" ForeColor="Green">Processing</asp:Label>
                                                    </ProgressTemplate>
                                                </asp:UpdateProgress>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>--%>
        </form>
    </div>

    <script src="js/jquery-1.12.4.js" type="text/javascript"></script>
    <script src="js/bootstrap.min.js" type="text/javascript"></script>

</body>
</html>
