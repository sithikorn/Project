'********Project Maintenance**********
'Create Sithikorn
'Create date 31/10/2022

Imports System.Xml
Imports AjaxControlToolkit
Imports System.IO
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Drawing


Public Class frmProject
    Inherits System.Web.UI.Page

#Region "Declaration"

    Private DatabaseDescription_ As New ws_DatabaseDescription.DatabaseDescription
    Private Project_ As New ws_Project.Project
    Private SecurityUser_ As New ws_securityuser.SecurityUser
    Private SecurityRule_ As New ws_SecurityRule.SecurityRule

    Private xmlPictureCurrent As New XmlDocument()      ' Use for delete document

    ' --- constrant ---
    Const OUTPUT_BLANK As String = "<?xml version='1.0' encoding='utf-8'  ?>"

    Private SCHEMA_XPATH_VALUE As String = "DatabaseDescriptions/DatabaseDescription"
    Private SCHEMA_XPATH_Key As String = "DatabaseDescriptions/DatabaseDescription [@PrimaryKey='1']"
    Private SCHEMA_XPATH_AllowEdit As String = "DatabaseDescriptions/DatabaseDescription [@AllowEdit='0']"
    Private SCHEMA_XPATH_ShowOnSelectList As String = "DatabaseDescriptions/DatabaseDescription [@ShowOnSelectList='1']"
    Private SCHEMA_XPATH_Validate_Blank As String = "DatabaseDescriptions/DatabaseDescription  [@Validate_Blank='1']"
    Private SCHEMA_XPATH_Validate_Duplicate As String = "DatabaseDescriptions/DatabaseDescription  [@Validate_Duplicate_Function !='']"

    'สร้าง control แยกตามกรุ๊ป
    'Private Employee_XPATH_GROUP_Employee As String = "ps_DatabaseDescriptions/DatabaseDescription [@Group_='General Info']"
    Private Data_XPATH_GROUP As String = "DatabaseDescriptions/DatabaseDescription [@SequenceGroup_='"

    Private AllXML As New List(Of String)
    Private GroupXML As New List(Of String)
    Private sConnectionStringDB As String
    Private sConectionTypeDB As String
    Private sPathPic As String

    Private sTableName As String = "Project"
    Private Table_XPATH_SELECTNODE As String = "/Projects/Project"

    Private TEXT_CREATEELEMENTS As String = "Projects"
    Private TEXT_CREATEELEMENT As String = "Project"


    Private TEXT_MESSAGE_COUNTBY As String = "projects"


#End Region

#Region "Control Event"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Page.Form.Attributes.Add("enctype", "multipart/form-data")

        If Session("LoginProject") Is Nothing Then
            Response.Redirect("~/Login.aspx")
        End If

        If Not Me.Page.IsPostBack Then
            iframePDF.Src = ""
            'ข้อมูลในกริด
            ViewState("CurrentAlphabet") = ""
            ViewState("TypeSearch") = "Text"
            Me.GenerateAlphabets()
            GetAlldata()
        End If

    End Sub

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        Try
            'create control
            Dim xmlString As String = ""
            Dim xmlReadSchema As New XmlDocument

            'xml schema
            DatabaseDescription_.GetByTableName(sTableName, xmlString)
            xmlReadSchema.LoadXml(xmlString)
            'xmlReadSchema.Save("C:\temp\schema.xml")
            Session("CreateSchema") = xmlString

            Dim nodelist As XmlNodeList
            nodelist = xmlReadSchema.SelectNodes(SCHEMA_XPATH_VALUE)
            For Each node As XmlNode In nodelist
                AllXML.Add(node.Attributes("SequenceGroup_").InnerText + "," + node.Attributes("Group_").InnerText)
            Next
            'หาจำนวนกรุ๊ป
            AllXML.Sort()
            GroupXML = AllXML.Distinct.ToList

            LoadControlxml()

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [Page_PreInit] (" + ex.Message + ")');", True)
        End Try
    End Sub

    Function GetIndex(ByVal grd As GridView, ByVal fieldName As String) As Integer
        For i As Integer = 0 To grd.Columns.Count - 1
            Dim field As DataControlField = grd.Columns(i)


            Dim bfield As BoundField = TryCast(field, BoundField)


            'Assuming accessing happens at data level, e.g with data field's name
            If bfield IsNot Nothing AndAlso bfield.DataField = fieldName Then
                Return i
            End If
        Next
        Return -1
    End Function


    Protected Sub gvData_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles gvData.SelectedIndexChanged
        Dim row As GridViewRow = gvData.SelectedRow

        'get from key
        Dim grd As GridView = TryCast(row.NamingContainer, GridView)

        Dim XMLSTRING As String = ""
        Dim xmlread As New XmlDocument
        Dim AttributeNodeList As XmlNodeList
        Dim Selected As String = ""

        XMLSTRING = Session("CreateSchema").ToString
        xmlread.LoadXml(XMLSTRING)
        AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_Key)
        For Each NodeSchema As XmlNode In AttributeNodeList
            If grd IsNot Nothing Then
                Dim index As Integer = GetIndex(gvData, NodeSchema.Attributes("FieldName").InnerText)
                If index <> -1 Then
                    Selected = row.Cells(index).Text
                End If
            End If
        Next


        If Selected <> "" Then

            lbStatus.Text = "Edit"
            lbStatusHistory.Text = "Edit"
            lbDataKey.Text = Selected

            SetDefaultData()

            'ดึงข้อมูลเก่าเพื่อมาแก้ไข
            ShowDetailEdit(Selected)
            'ตรวจสอบ AllowEdit
            CheckAllowEdit()
            iframePDF.Visible = False
        End If

    End Sub

    Sub Btn_ClickSave(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        If Session("canEditProject") = True Then
            Dim boCheck As Boolean
            boCheck = ValidateControl()
            If boCheck = True Then
                Dim boCheckDuplicate As Boolean
                boCheckDuplicate = ValidateDuplicate()
                If boCheckDuplicate = True Then
                    Save()
                Else
                    ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Duplicate , " & HDduplicate.Value & "');", True)
                End If

            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Please fill in all required fields.');", True)
            End If
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "text", "hidePleaseWait()", True)
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "text", "hidePleaseWait()", True)
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('You have no permission.');", True)
        End If
    End Sub

    'Sub Btn_ClickSaveHistory(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSaveHistory.Click

    '    Dim boCheck As Boolean
    '    boCheck = ValidateControlHistory()
    '    If boCheck = True Then
    '        Dim boCheckDuplicate As Boolean
    '        boCheckDuplicate = ValidateDuplicateHistory()
    '        If boCheckDuplicate = True Then
    '            SaveHistory()
    '        Else
    '            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Duplicate , " & HDduplicate.Value & "');", True)
    '        End If

    '    Else
    '        ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Please fill in all required fields.');", True)
    '    End If
    '    ScriptManager.RegisterStartupScript(Me.Page, Page.GetType(), "text", "hidePleaseWait()", True)

    'End Sub


    Sub Btn_ClickCancel(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancelSave.Click
        'Cancel click แล้วใส่ค่า default เอาไว้
        PanelR.Visible = False
        pnSave.Visible = False
        SetDefaultData()

    End Sub

    Sub Btn_ClickDelete(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
        'confrim delete
        If Session("canEditProject") = True Then

            Dim confirmValue As String = Request.Form("confirm_value")
            confirmValue = Right(confirmValue, 3)
            confirmValue = confirmValue.Trim(CChar(","))
            If confirmValue = "Yes" Then
                lbStatus.Text = "Delete"
                Save()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "text", "hidePleaseWait()", True)
            End If
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "text", "hidePleaseWait()", True)
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('You have no permission.');", True)
        End If
    End Sub

    Sub Btn_ClickAdd(ByVal sender As Object, ByVal e As EventArgs) Handles btnAdd.Click
        'เมื่อกดปุ่ม new employee ให้เอาค่า defualt มาใส่
        PanelR.Visible = True
        pnSave.Visible = True
        'cmdDelete.Visible = False
        lbStatus.Text = "Add"
        SetDefaultData()
        CheckAllowAdd()
        cmdSave.Enabled = True

    End Sub

    Sub Btn_ClickSearch(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        ViewState("CurrentAlphabet") = txtSearch.Text.Trim
        ViewState("TypeSearch") = "Text"
        GenerateAlphabets()
        GetAlldataSearch(txtSearch.Text.Trim)
    End Sub

    Protected Sub Alphabet_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim lnkAlphabet As LinkButton = DirectCast(sender, LinkButton)
        If lnkAlphabet.Text = "ALL" Then
            ViewState("CurrentAlphabet") = ""
        Else
            ViewState("CurrentAlphabet") = lnkAlphabet.Text
        End If
        ViewState("TypeSearch") = "Alphabet"
        Me.GenerateAlphabets()
        gvData.PageIndex = 0
        GetAlldataSearchAlphabets(ViewState("CurrentAlphabet"))
    End Sub

    Sub Check_Clicked(ByVal sender As Object, ByVal e As EventArgs)

        If ViewState("TypeSearch") = "Text" Then
            ViewState("CurrentAlphabet") = txtSearch.Text.Trim
            GenerateAlphabets()
            GetAlldataSearch(ViewState("CurrentAlphabet"))
        Else
            GenerateAlphabets()
            GetAlldataSearchAlphabets(ViewState("CurrentAlphabet"))
        End If

    End Sub

#End Region

#Region "Methods"
    Private Sub GenerateAlphabets()
        Dim alphabets As New List(Of ListItem)()
        Dim alphabet As New ListItem()
        alphabet.Value = "ALL"
        alphabet.Selected = alphabet.Value.Equals(ViewState("CurrentAlphabet"))
        alphabets.Add(alphabet)
        For i As Integer = 65 To 90
            alphabet = New ListItem()
            alphabet.Value = [Char].ConvertFromUtf32(i)
            alphabet.Selected = alphabet.Value.Equals(ViewState("CurrentAlphabet"))
            alphabets.Add(alphabet)
        Next
        rptAlphabets.DataSource = alphabets
        rptAlphabets.DataBind()
    End Sub

    Private Sub LoadControlxml()
        Try
            Dim xmlString As String = ""
            Dim xmlread As New XmlDocument
            Dim xmlreadGroup As New XmlDocument

            Dim nodSelectList As XmlNodeList
            Dim nodeControllist As XmlNodeList
            Dim strSplitGroup As Array
            Dim iColumnWidth As Integer

            'สร้างคอลัมน์ select
            Dim test As New CommandField
            test.ShowSelectButton = True
            test.ItemStyle.Font.Underline = True
            test.ItemStyle.Width = "70"
            test.ItemStyle.CssClass = "text-center"
            gvData.Columns.Add(test)

            'Schema
            xmlString = Session("CreateSchema").ToString
            xmlread.LoadXml(xmlString)

            'สร้าง column DataBound ในกริดเอามาเฉพาะคอลัมน์ที่ให้แสดงบนGrid OnSelectList = 1
            nodSelectList = xmlread.SelectNodes(SCHEMA_XPATH_ShowOnSelectList)
            For Each nodeColumnGrid As XmlNode In nodSelectList
                'สร้าง column ในกริด
                If nodeColumnGrid.Attributes("FieldName").InnerText = "ProjectCode" Then
                    iColumnWidth = 80
                Else
                    iColumnWidth = 250
                End If
                AddBoundField(gvData, nodeColumnGrid.Attributes("FieldName").InnerText, iColumnWidth, nodeColumnGrid.Attributes("Description").InnerText)
            Next

            'สร้าง Panel และ Control
            'วนลูป Gruop จาก List of
            'สร้าง html control แต่ล่ะ tab
            For i As Integer = 0 To GroupXML.Count - 1
                'คือค่า Group_,SequenceGroup_
                strSplitGroup = Split(GroupXML(i), ",")
                'สร้าง Html Control li
                Dim liName As New HtmlGenericControl("li")
                If Request.Cookies("GroupClick") Is Nothing Then
                    If i = 0 Then
                        liName.Attributes.Add("class", "active")
                    End If
                Else
                    Dim CookieGroup As String = HttpUtility.UrlDecode(Request.Cookies("GroupClick").Value)
                    If strSplitGroup(1) = CookieGroup Then
                        liName.Attributes.Add("class", "active")
                    End If
                End If

                liName.Attributes.Add("id", "li_" + strSplitGroup(0))
                liName.Attributes.Add("role", "presentation")
                'ใส่ li ใน ul ที่ได้สร้างไว้
                ulGroup.Controls.Add(liName)
                'สร้าง Html Control a เอาไปใส่ไว้ใน li
                Dim aName As New HtmlGenericControl("a")
                aName.InnerText = strSplitGroup(1)
                aName.Attributes.Add("href", "#Div" + strSplitGroup(0))
                aName.Attributes.Add("data-toggle", "tab")
                'aName.Attributes.Add("runat", "server")
                liName.Controls.Add(aName)

                'สร้าง Div ไว้ใส่กลุ่ม Control และตั้ง id ตามกลุ่ม
                Dim divName As New HtmlGenericControl
                divName.TagName = "div"
                '1=SequenceGroup_
                divName.ID = "Div" + strSplitGroup(0)
                'If i = 0 Then
                '    divName.Attributes.Add("class", "tab-pane fade in active")
                'Else
                '    divName.Attributes.Add("class", "tab-pane fade")
                'End If
                If Request.Cookies("GroupClick") Is Nothing Then
                    If i = 0 Then
                        divName.Attributes.Add("class", "tab-pane fade in active")
                    Else
                        divName.Attributes.Add("class", "tab-pane fade")
                    End If
                Else
                    Dim CookieGroup As String = HttpUtility.UrlDecode(Request.Cookies("GroupClick").Value)
                    If strSplitGroup(1) = CookieGroup Then
                        divName.Attributes.Add("class", "tab-pane fade in active")
                    Else
                        divName.Attributes.Add("class", "tab-pane fade")
                    End If
                End If

                'สร้าง Div ไว้ใส่กลุ่ม Control และตั้ง id ตามกลุ่ม
                divContent.Controls.Add(divName)

            Next
            'สร้าง control แต่ล่ะ tab
            For i As Integer = 0 To GroupXML.Count - 1
                'คือค่า Group_,SequenceGroup_
                strSplitGroup = Split(GroupXML(i), ",")
                'ตย. ps_DatabaseDescriptions/DatabaseDescription [@Group_='General Info']
                nodeControllist = xmlread.SelectNodes(Data_XPATH_GROUP + strSplitGroup(0) + "']")
                For Each nodeControl As XmlNode In nodeControllist
                    AddConDetail(nodeControl.Attributes("FieldName").InnerText, strSplitGroup(0), nodeControl.Attributes("Control_Type").InnerText, nodeControl.Attributes("Validate_Blank").InnerText, nodeControl.Attributes("Validate_DataType").InnerText, nodeControl.Attributes("Lookup_Data").InnerText, nodeControl.Attributes("Description").InnerText, nodeControl.Attributes("SecurityRule").InnerText, nodeControl.Attributes("SaveHistoryChange").InnerText)
                Next
            Next


        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [LoadControlxml] (" + ex.Message + ")');", True)
        End Try
    End Sub

    Private Sub AddConDetail(ByVal ColumnName As String, ByVal PanelGroup As String, ByVal ColumnType As String, ByVal Validate_Blank As String, ByVal Validate_DataType As String, ByVal Lookup_Data As String, ByVal Description As String, ByVal SecurityRule As String, ByVal SaveHistoryChange As String)

        Try
            'หา div ตามกรุ๊ป
            Dim DivFind As HtmlGenericControl = DirectCast(Page.FindControl("Div" + PanelGroup), HtmlGenericControl)
            'ใส่ div form-group
            Dim divgroup As New HtmlGenericControl
            divgroup.TagName = "div"
            divgroup.Attributes.Add("class", "form-group")
            DivFind.Controls.Add(divgroup)
            'ใส่ div inline
            Dim divgrouplb As New HtmlGenericControl
            divgrouplb.TagName = "div"
            divgrouplb.Attributes.Add("class", "form-inline")
            divgroup.Controls.Add(divgrouplb)
            'Blank ให้ใส่ *สีแดง
            If Validate_Blank = 1 Then
                Dim cLabel As New Label
                cLabel.Text = "*"
                cLabel.ForeColor = Drawing.Color.Red
                cLabel.ID = "Vd" + ColumnName
                cLabel.CssClass = "control-label"
                divgrouplb.Controls.Add(cLabel)
            End If
            'label แสดงชื่อฟิวด์
            Dim Label As New Label
            Label.ID = "lb" + ColumnName
            Label.Text = Description + " : "
            Label.CssClass = "control-label"
            Label.Font.Bold = True
            divgrouplb.Controls.Add(Label)
            'สร้าง .net control ตาม xml
            Select Case ColumnType

                Case "label"
                    Dim cLabel As New Label
                    cLabel.ID = ColumnName
                    cLabel.CssClass = "control-label"
                    divgroup.Controls.Add(cLabel)
                Case "textbox"
                    Dim cTextbox As New TextBox
                    cTextbox.ID = ColumnName
                    cTextbox.Width = 250
                    cTextbox.CssClass = "form-control input-sm"

                    If Validate_DataType = "text" Then
                        cTextbox.Height = 250
                        cTextbox.TextMode = TextBoxMode.MultiLine
                    End If

                    Dim cHiddenField As New HiddenField
                    cHiddenField.ID = "HF" + ColumnName

                    If SecurityRule = "AdminProject" Then
                        'cTextbox.AutoPostBack = True
                        cTextbox.Attributes.Add("OnFocus", "OnFocus(this);")
                        cTextbox.Attributes.Add("OnChange", "OnChangeSecurityRule(this);")

                        divgroup.Controls.Add(cHiddenField)
                    ElseIf SaveHistoryChange = "1" Then
                        'cTextbox.AutoPostBack = True
                        'AddHandler cTextbox.TextChanged, AddressOf OnTextChanged
                        cTextbox.Attributes.Add("OnFocus", "OnFocus(this);")
                        cTextbox.Attributes.Add("OnChange", "OnChange(this);")

                        divgroup.Controls.Add(cHiddenField)

                        'Dim scriptText As String = ""
                        'scriptText &= "function OnFocusHistory(elementRef){"
                        'scriptText &= "   var txt = document.getElementById(elementRef.id).value; "
                        'scriptText &= "  document.getElementById('HFbeforchange').value = txt; "
                        'scriptText &= "}"
                        'ClientScript.RegisterClientScriptBlock(Me.GetType(), _
                        '    "OnFocus", scriptText, True)

                        'cTextbox.Attributes.Add("OnFocus", "OnFocus(this)")

                        'Dim scriptTextOnChange As String = ""
                        'scriptTextOnChange &= "function OnChangeHistory(elementRef){"
                        'scriptTextOnChange &= " var prev = $(elementRef).data('prev'); "
                        'scriptTextOnChange &= " $(elementRef).data('prev', $(elementRef).val()); "
                        'scriptTextOnChange &= " var Controltype = document.getElementById(elementRef.id); "
                        'scriptTextOnChange &= " var HFbeforchange = $(HFbeforchange).val();"
                        'scriptTextOnChange &= "     if (prev != undefined) {"
                        'scriptTextOnChange &= "         $('#HFbeforchange').val(prev); "
                        'scriptTextOnChange &= "     }"
                        'scriptTextOnChange &= "  $('#HFcontrolchange').val(elementRef.id);"
                        'scriptTextOnChange &= "  $('#HFcontroltype').val(Controltype.type);"
                        'scriptTextOnChange &= "}"
                        'ClientScript.RegisterClientScriptBlock(Me.GetType(), _
                        '    "OnChange", scriptTextOnChange, True)
                        'cTextbox.Attributes.Add("OnChange", "OnChangeHistory(this)")

                    End If

                    divgroup.Controls.Add(cTextbox)


                    'ถ้าเป็น textbox แล้วก็ integer ให้ใส่ได้เฉพาะ integer
                    If Validate_DataType = "int" Then
                        Dim RangeValidator As New RegularExpressionValidator
                        RangeValidator.ID = "rv" + ColumnName
                        RangeValidator.ErrorMessage = "Input Number"
                        RangeValidator.ValidationExpression = "^\d*(\.|,|(\.\d{1,2})|(,\d{1,2}))?$" ' "^\d+$"
                        RangeValidator.ControlToValidate = ColumnName
                        RangeValidator.ValidationGroup = "1"
                        RangeValidator.ForeColor = Color.Red
                        divgroup.Controls.Add(RangeValidator)

                    End If

                Case "calendar"
                    Dim cTextbox As New TextBox
                    cTextbox.ID = ColumnName
                    cTextbox.MaxLength = 10
                    cTextbox.Width = 250
                    cTextbox.CssClass = "form-control input-sm"
                    cTextbox.AutoCompleteType = AutoCompleteType.None
                    Dim cHiddenField As New HiddenField
                    cHiddenField.ID = "HF" + ColumnName

                    If SecurityRule = "AdminProject" Then
                        cTextbox.AutoPostBack = True
                        cTextbox.Attributes.Add("OnFocus", "OnFocus(this);")
                        cTextbox.Attributes.Add("OnChange", "OnChangeSecurityRuleCalendar(this);")

                        divgroup.Controls.Add(cHiddenField)
                    ElseIf SaveHistoryChange = "1" Then
                        cTextbox.AutoPostBack = True
                        'AddHandler cTextbox.TextChanged, AddressOf OnTextChangedCalendar
                        cTextbox.Attributes.Add("OnFocus", "OnFocus(this);")
                        cTextbox.Attributes.Add("OnChange", "OnChangeCalendar(this);")

                        divgroup.Controls.Add(cHiddenField)
                    End If

                    divgroup.Controls.Add(cTextbox)
                    'format date
                    Dim Calendar As New CalendarExtender
                    Calendar.TargetControlID = ColumnName
                    Calendar.ID = "Cal" + ColumnName
                    Calendar.Format = "dd/MM/yyyy"
                    Calendar.CssClass = "cal_Theme1"
                    Calendar.Enabled = "True"
                    If SecurityRule <> "" Or SaveHistoryChange = "1" Then
                        Calendar.OnClientShown = "CalendarExtenderShow"
                    End If
                    divgroup.Controls.Add(Calendar)

                    'ตรวจสอบ ใส่วันที่ถูกต้องไหม
                    Dim CompareCalendar As New CompareValidator
                    CompareCalendar.ControlToValidate = ColumnName
                    CompareCalendar.Type = ValidationDataType.Date
                    CompareCalendar.Text = "date type mismatch"
                    CompareCalendar.Operator = ValidationCompareOperator.DataTypeCheck
                    CompareCalendar.SetFocusOnError = True
                    CompareCalendar.ValueToCompare = DateTime.Now.ToShortDateString() 'DateTime.Today.ToString("dd/MM/yyyy")
                    CompareCalendar.Display = ValidatorDisplay.Dynamic
                    divgroup.Controls.Add(CompareCalendar)

                Case "dropdownlist"

                    Dim ds As New DataSet()
                    Dim xmlTbName As String
                    Dim xmlDoc As New XmlDocument

                    Dim cDropDownList As New DropDownList
                    cDropDownList.CssClass = "form-control input-sm"
                    cDropDownList.ID = ColumnName
                    cDropDownList.Width = 250

                    Dim cHiddenField As New HiddenField
                    cHiddenField.ID = "HF" + ColumnName

                    If SecurityRule = "AdminProject" Then
                        cDropDownList.AutoPostBack = True
                        cDropDownList.Attributes.Add("OnClick", "OnClick(this)")
                        cDropDownList.Attributes.Add("OnChange", "OnChangeSecurityRule(this)")

                        divgroup.Controls.Add(cHiddenField)

                    ElseIf SaveHistoryChange = "1" Then
                        cDropDownList.AutoPostBack = True
                        'AddHandler cDropDownList.SelectedIndexChanged, AddressOf OnSelectedIndexChangedMethod
                        cDropDownList.Attributes.Add("OnClick", "OnClick(this)")
                        cDropDownList.Attributes.Add("OnChange", "OnChange(this)")

                        divgroup.Controls.Add(cHiddenField)

                        'Dim scriptText As String = ""
                        'scriptText &= "function OnClickHistory(elementRef){"
                        'scriptText &= "     var ddl = document.getElementById(elementRef.id);"
                        'scriptText &= "     var SelVal = ddl.options[ddl.selectedIndex].value;"
                        'scriptText &= "     var Chkcontrol = document.getElementById('HFcontrolchange').value;"
                        'scriptText &= "     var HFbeforchange = document.getElementById('HFbeforchange').value;"
                        'scriptText &= "         if (HFbeforchange == '' || Chkcontrol != elementRef.id) {"
                        'scriptText &= "             document.getElementById('HFbeforchange').value = SelVal;"
                        'scriptText &= "         }"
                        'scriptText &= "}"

                        'ClientScript.RegisterClientScriptBlock(Me.GetType(), _
                        '    "OnClick", scriptText, True)

                        'cDropDownList.Attributes.Add("OnClick", "OnClick(this)")

                        'Dim scriptTextOnChange As String = ""
                        'scriptTextOnChange &= "function OnChangeHistory(elementRef){"
                        'scriptTextOnChange &= " var prev = $(elementRef).data('prev');"
                        'scriptTextOnChange &= " $(elementRef).data('prev', $(elementRef).val());"
                        'scriptTextOnChange &= " var Controltype = document.getElementById(elementRef.id);"
                        'scriptTextOnChange &= " var HFbeforchange = $(HFbeforchange).val();"
                        'scriptTextOnChange &= "     if (prev != undefined) {"
                        'scriptTextOnChange &= "         $('#HFbeforchange').val(prev);"
                        'scriptTextOnChange &= "     }"
                        'scriptTextOnChange &= " $('#HFcontrolchange').val(elementRef.id);"
                        'scriptTextOnChange &= " $('#HFcontroltype').val(Controltype.type);"
                        'scriptTextOnChange &= "}"

                        'ClientScript.RegisterClientScriptBlock(Me.GetType(), _
                        '    "OnChange", scriptTextOnChange, True)

                        'cDropDownList.Attributes.Add("OnChange", "OnChangeHistory(this)")

                    End If

                    If Validate_DataType = "boolean" Then
                        cDropDownList.Items.Add("Yes")
                        cDropDownList.Items.Add("No")
                        divgroup.Controls.Add(cDropDownList)
                    Else
                        'ข้อมูลจาก xml
                        Dim xmlString As String = ""
                        DatabaseDescription_.ExecuteSQL(Lookup_Data, xmlString)
                        If xmlString <> OUTPUT_BLANK Then

                            ds.ReadXml(New XmlTextReader(New StringReader(xmlString)))
                            xmlDoc.LoadXml(xmlString)
                            xmlTbName = xmlDoc.ChildNodes(1).FirstChild.Name

                            Dim i As Integer = -1
                            Dim ii As Integer = 0
                            Dim cDataTextField As String = ""
                            Dim columnNameList As New List(Of String)()
                            Dim dv As DataView = ds.Tables(xmlTbName).DefaultView
                            For Each dataColumn As DataColumn In dv.Table.Columns
                                columnNameList.Add(dataColumn.ColumnName)
                                If dataColumn.ColumnName = "MixDescription" Then
                                    cDataTextField = "MixDescription"
                                    ii = i + 1
                                End If
                                i = i + 1
                            Next
                            Dim columnNames As String() = columnNameList.ToArray()
                            'dv.Sort = columnNames(0)
                            If cDataTextField <> "" Then
                                cDropDownList.DataTextField = columnNames(ii)
                                cDropDownList.Font.Size = 7
                            Else
                                cDropDownList.DataTextField = columnNames(0)
                            End If
                            cDropDownList.DataValueField = columnNames(0)
                            cDropDownList.DataSource = dv
                            cDropDownList.DataBind()

                            divgroup.Controls.Add(cDropDownList)
                        Else
                            divgroup.Controls.Add(cDropDownList)
                            'ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Data not found [AddConDetail] " + ColumnName + "');", True)
                        End If
                    End If
                Case "fileupload"
                    Dim fileUpload As New FileUpload
                    fileUpload.ID = ColumnName
                    fileUpload.Width = 500
                    divgroup.Controls.Add(fileUpload)


                    'ใส่ div inline
                    Dim divgrouplbFrame As New HtmlGenericControl
                    divgrouplbFrame.TagName = "div"
                    divgrouplbFrame.Attributes.Add("class", "form-inline")
                    divgroup.Controls.Add(divgrouplbFrame)

                    Dim Button As New Button
                    Button.ID = "btn" + ColumnName
                    Button.Text = "Open"
                    fileUpload.Width = 500
                    AddHandler Button.Click, AddressOf Me.Button_Click
                    'Button.Attributes.Add("onclick", "OnClickPDF(this)")
                    divgrouplbFrame.Controls.Add(Button)

            End Select
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [AddConDetail] (" + ex.Message + ")');", True)
        End Try
    End Sub

    Protected Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try



            Dim textbox As TextBox = DirectCast(Page.FindControl("ProjectCode"), TextBox)
            'Dim folderPath As String = Server.MapPath("~/FilePDf/" + textbox.Text + "/")

            Dim NameAtt As String = (TryCast(sender, Button)).ID.Substring(3, (TryCast(sender, Button)).ID.Length - 4)
            Dim IDidx As String = (TryCast(sender, Button)).ID.Substring((TryCast(sender, Button)).ID.Length - 1)
            Dim id As String = NameAtt + "Name" + IDidx

            Dim textboxOpen As TextBox = DirectCast(Page.FindControl(id), TextBox)

            If textboxOpen.Text <> "" Then
                Dim folderPath As String = "file://sslvmsql1/APPLICATION/ProjectMaintenance/FilePDf/" + textbox.Text + "/" + textboxOpen.Text
                ''Dim sFilePath As String = folderPath + textboxOpen.Text
                'Dim buffer As Byte()
                'Using conn As New SqlConnection("Persist Security Info=False;User ID=ws_serv;Password=ws_serv;Initial Catalog=MFGSAG;Data Source=sslvmsql1;Max Pool Size=100;Connection TimeOut=1500")
                '    conn.Open()
                '    Using cmd As New SqlCommand("SELECT Attachment1 FROM Project WHERE ProjectCode = @ProjectCode", conn)
                '        cmd.Parameters.AddWithValue("@ProjectCode", textbox.Text)
                '        buffer = cmd.ExecuteScalar()
                '    End Using
                '    conn.Close()
                'End Using

                ''sFilePath = System.IO.Path.GetTempFileName()
                ''ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('" + folderPath + textboxOpen.Text + "');", True)
                ''System.IO.File.Move(sFilePath, System.IO.Path.ChangeExtension(sFilePath, ".pdf"))
                ''sFilePath = System.IO.Path.ChangeExtension(sFilePath, ".pdf")
                ''ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('" + folderPath + "');", True)
                ''System.IO.File.WriteAllBytes(sFilePath, buffer)



                'Dim sStream As New FileStream("\\SSLNAS1\Temp\SS\test.pdf", FileMode.Create, FileAccess.ReadWrite)
                'sStream.Write(buffer, 0, buffer.Length)

                'Dim act As Action(Of String) = New Action(Of String)(AddressOf OpenPDFFile)
                'act.BeginInvoke("\\SSLNAS1\Temp\SS\test.pdf", Nothing, Nothing)
                Dim ifr As HtmlControl = DirectCast(Page.FindControl("iframePDF"), HtmlControl)
                ifr.Attributes("src") = folderPath
                ifr.Visible = True
                'iframePDF.Src = folderPath

            End If



        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [GetAlldata] (" + ex.Message + ")');", True)
        End Try
    End Sub

    Private Shared Sub OpenPDFFile(ByVal sFilePath)
        Using p As New System.Diagnostics.Process
            p.StartInfo = New System.Diagnostics.ProcessStartInfo(sFilePath)
            p.Start()
            p.WaitForExit()
            Try
                System.IO.File.Delete(sFilePath)
            Catch
            End Try
        End Using
    End Sub

    Private Sub GetAlldata()

        Dim xmlString As String = ""
        Dim xmlDataString As String = ""
        Dim xmlSchema As New XmlDocument
        Dim xmlData As New XmlDocument
        Dim nodeSelectlist As XmlNodeList
        Dim nodeListData As XmlNodeList
        Dim dt As New DataTable
        'Dim xmlRead As New XmlDocument

        Try
            xmlString = Session("CreateSchema").ToString
            xmlSchema.LoadXml(xmlString)

            'ข้อมูลคอลัมน์ที่ต้องการแสดงในกริด
            nodeSelectlist = xmlSchema.SelectNodes(SCHEMA_XPATH_ShowOnSelectList)

            'สร้าง คอลัมน์ ในกริดตามที่ต้องการให้แสดง
            For Each node As XmlNode In nodeSelectlist
                dt.Columns.Add(node.Attributes("FieldName").InnerText, GetType(String))
            Next

            Project_.GetAll(xmlDataString)
            'xmlRead.LoadXml(xmlDataString)
            Session("GetAllData") = xmlDataString
            'xmlRead.Save("C:\temp\Data.xml")
            'xmlDataString = xmlDataString
            'hr_Employee_.GetAll_Short(xmlDataString)
            If xmlDataString <> OUTPUT_BLANK Then

                xmlData.LoadXml(xmlDataString)
                nodeListData = xmlData.DocumentElement.ChildNodes

                'วนข้อมูลทั้งหมดเพื่อเอามาใส่ในแถว
                For Each nodeData As XmlNode In nodeListData
                    Dim dtrow As DataRow = dt.NewRow()
                    For Each nodeSelect As XmlNode In nodeSelectlist
                        dtrow(nodeSelect.Attributes("FieldName").InnerText) = nodeData.Attributes(nodeSelect.Attributes("FieldName").InnerText).InnerText
                    Next
                    dt.Rows.Add(dtrow)
                Next

                gvData.DataSource = dt
                gvData.DataBind()
                lblResult.Text = "Total " + Convert.ToString(dt.Rows.Count) + " " + TEXT_MESSAGE_COUNTBY
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Data not found');", True)
            End If

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [GetAlldata] (" + ex.Message + ")');", True)
        End Try

    End Sub

    Private Sub ReadDataTimeout()

        Dim xmlString As String = ""
        Dim xmlReadSchema As New XmlDocument
        Dim xmlDataString As String = ""
        'Dim xmlRead As New XmlDocument

        'xml schema
        DatabaseDescription_.GetByTableName(sTableName, xmlString)
        xmlReadSchema.LoadXml(xmlString)
        Session("CreateSchema") = xmlString

        Project_.GetAll(xmlDataString)
        'xmlRead.LoadXml(xmlDataString)
        Session("GetAllData") = xmlDataString

    End Sub

    Private Sub GetAlldataSearch(ByVal Search As String)
        'ScriptManager.RegisterStartupScript(Me.Page, Page.GetType(), "text", "showPleaseWait()", True)

        'If Session("GetAllData") Is Nothing Then
        ReadDataTimeout() 'refresh data
        'End If

        Dim xmlString As String = ""
        Dim xmlDataString As String = ""
        Dim xmlSchema As New XmlDocument
        Dim xmlData As New XmlDocument
        Dim nodeSelectlist As XmlNodeList
        Dim nodeListData As XmlNodeList
        Dim dt As New DataTable
        Dim StatusActive As String = ""
        Try
            xmlString = Session("CreateSchema").ToString
            xmlSchema.LoadXml(xmlString)

            'ข้อมูลคอลัมน์ที่ต้องการแสดงในกริด
            nodeSelectlist = xmlSchema.SelectNodes(SCHEMA_XPATH_ShowOnSelectList)

            'สร้าง คอลัมน์ ในกริดตามที่ต้องการให้แสดง
            For Each node As XmlNode In nodeSelectlist
                dt.Columns.Add(node.Attributes("FieldName").InnerText, GetType(String))
            Next
            'ต่างกับ get all เพื่อเอามาค้นหาจาก get all
            xmlDataString = Session("GetAllData").ToString
            'hr_Employee_.GetAll_Short(xmlDataString)
            If xmlDataString <> OUTPUT_BLANK Then

                xmlData.LoadXml(xmlDataString)
                'nodeListData = xmlData.DocumentElement.ChildNodes
                If Search <> "" Then
                    'If chkStatus.Checked = True Then
                    '    nodeListData = xmlData.SelectNodes("//*[(contains(translate(@EmployeeID,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'" & Search.ToUpper & "') or contains(translate(@LastName,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'" & Search.ToUpper & "') or contains(translate(@FirstName,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'" & Search.ToUpper & "')) and ( @Status_='AC' or @Status_='PN') ]")
                    '    StatusActive = Chr(44) + Chr(34) + "Active" + Chr(34)
                    'Else
                    'nodeListData = xmlData.SelectNodes("//*[contains(@EmployeeID,'" & Search & "') or contains(@LastName,'" & Search & "') or contains(@FirstName,'" & Search & "')]")
                    nodeListData = xmlData.SelectNodes("//*[contains(translate(@ProjectCode,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'" & Search.ToUpper & "') or contains(translate(@Description,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'" & Search.ToUpper & "') ]")
                    'End If
                Else
                    'If chkStatus.Checked = True Then
                    '    nodeListData = xmlData.SelectNodes("//*[@Status_='AC' or @Status_='PN' ]")
                    '    StatusActive = Chr(44) + Chr(34) + "Active" + Chr(34)
                    'Else
                    nodeListData = xmlData.DocumentElement.ChildNodes
                    'End If
                End If
                'วนข้อมูลทั้งหมดเพื่อเอามาใส่ในแถว
                For Each nodeData As XmlNode In nodeListData
                    Dim dtrow As DataRow = dt.NewRow()
                    For Each nodeSelect As XmlNode In nodeSelectlist
                        dtrow(nodeSelect.Attributes("FieldName").InnerText) = nodeData.Attributes(nodeSelect.Attributes("FieldName").InnerText).InnerText
                    Next
                    dt.Rows.Add(dtrow)
                Next

                gvData.DataSource = dt
                gvData.DataBind()
                lblResult.Text = "Search result " + Chr(34) + Search + Chr(34) + StatusActive + " , " + Convert.ToString(dt.Rows.Count) + " " + TEXT_MESSAGE_COUNTBY
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Data not found');", True)
            End If

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [GetAlldataSearch] (" + ex.Message + ")');", True)
        End Try

        'ScriptManager.RegisterStartupScript(Me.Page, Page.GetType(), "text", "hidePleaseWait()", True)
        'ScriptManager.RegisterStartupScript(Me.Page, Page.GetType(), "text", "  <script type='text/javascript'>hidePleaseWait();</script>", False)

    End Sub

    Private Sub GetAlldataSearchAlphabets(ByVal Search As String)
        'ScriptManager.RegisterStartupScript(Me.Page, Page.GetType(), "text", "showPleaseWait()", True)
        If Session("GetAllData") Is Nothing Then
            ReadDataTimeout()
        End If

        Dim xmlString As String = ""
        Dim xmlDataString As String = ""
        Dim xmlSchema As New XmlDocument
        Dim xmlData As New XmlDocument
        Dim nodeSelectlist As XmlNodeList
        Dim nodeListData As XmlNodeList
        Dim dt As New DataTable
        Dim StatusActive As String = ""
        Try
            xmlString = Session("CreateSchema").ToString
            xmlSchema.LoadXml(xmlString)

            'ข้อมูลคอลัมน์ที่ต้องการแสดงในกริด
            nodeSelectlist = xmlSchema.SelectNodes(SCHEMA_XPATH_ShowOnSelectList)

            'สร้าง คอลัมน์ ในกริดตามที่ต้องการให้แสดง
            For Each node As XmlNode In nodeSelectlist
                dt.Columns.Add(node.Attributes("FieldName").InnerText, GetType(String))
            Next
            'ต่างกับ get all เพื่อเอามาค้นหาจาก get all
            xmlDataString = Session("GetAllData").ToString
            'hr_Employee_.GetAll_Short(xmlDataString)
            If xmlDataString <> OUTPUT_BLANK Then

                xmlData.LoadXml(xmlDataString)
                'nodeListData = xmlData.DocumentElement.ChildNodes
                If Search <> "" Then
                    'If chkStatus.Checked = True Then
                    '    nodeListData = xmlData.SelectNodes("//*[starts-with(translate(@FirstName,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'" & Search.ToUpper & "') and (@Status_='AC' or @Status_='PN') ]")
                    '    StatusActive = Chr(44) + Chr(34) + "Active" + Chr(34)
                    'Else
                    'nodeListData = xmlData.SelectNodes("//*[contains(@EmployeeID,'" & Search & "') or contains(@LastName,'" & Search & "') or contains(@FirstName,'" & Search & "')]")
                    nodeListData = xmlData.SelectNodes("//*[starts-with(translate(@ProjectCode,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'" & Search.ToUpper & "')]")
                    'End If
                Else
                    'If chkStatus.Checked = True Then
                    '    nodeListData = xmlData.SelectNodes("//*[@Status_='AC' or @Status_='PN' ]")
                    '    StatusActive = Chr(44) + Chr(34) + "Active" + Chr(34)
                    'Else
                    nodeListData = xmlData.DocumentElement.ChildNodes
                    'End If
                End If
                'วนข้อมูลทั้งหมดเพื่อเอามาใส่ในแถว
                For Each nodeData As XmlNode In nodeListData
                    Dim dtrow As DataRow = dt.NewRow()
                    For Each nodeSelect As XmlNode In nodeSelectlist
                        dtrow(nodeSelect.Attributes("FieldName").InnerText) = nodeData.Attributes(nodeSelect.Attributes("FieldName").InnerText).InnerText
                    Next
                    dt.Rows.Add(dtrow)
                Next

                gvData.DataSource = dt
                gvData.DataBind()
                lblResult.Text = "Search result " + Chr(34) + Search + Chr(34) + StatusActive + " , " + Convert.ToString(dt.Rows.Count) + " " + TEXT_MESSAGE_COUNTBY
            Else
                ScriptManager.RegisterStartupScript(Me, Page.GetType, "alert", "alert('Data not found');", True)
            End If

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Page.GetType, "alert", "alert('Error during [GetAlldataSearchAlphabets] (" + ex.Message + ")');", True)
        End Try
        'ScriptManager.RegisterStartupScript(Me.Page, Page.GetType(), "text", "hidePleaseWait()", True)
        'ScriptManager.RegisterStartupScript(Me.Page, Page.GetType(), "text", "  <script type='text/javascript'>hidePleaseWait();</script>", False)

    End Sub

    Private Sub ShowDetailEdit(ByVal KeyRow As String)
        'If TabName.Value <> "" Then
        '    Session("sessGruop") = TabName.Value
        'End If

        Dim node As XmlNode
        Dim XMLSTRING As String = ""
        Dim xmlread As New XmlDocument
        Dim AttributeNodeList As XmlNodeList
        Dim strfieldname As String

        'XMLSTRING = Session("CreateData").ToString
        Project_.GetByProjectID(KeyRow, XMLSTRING)

        Try
            If XMLSTRING <> OUTPUT_BLANK Then
                xmlread.LoadXml(XMLSTRING)
                'xmlread.Save("C:\temp\Employee.xml")
                node = xmlread.SelectSingleNode(Table_XPATH_SELECTNODE)

                If Not node Is Nothing Then

                    XMLSTRING = Session("CreateSchema").ToString
                    xmlread.LoadXml(XMLSTRING)
                    AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_VALUE)

                    'วน schema เพื่อหา control และใส่ข้อมูลเพื่อมา edit
                    For Each NodeSchema As XmlNode In AttributeNodeList

                        strfieldname = NodeSchema.Attributes("FieldName").InnerText
                        If Not IsNothing(node.Attributes(strfieldname)) Then
                            Select Case NodeSchema.Attributes("Control_Type").InnerText
                                Case "textbox"
                                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                                    cTextbox.Text = node.Attributes(strfieldname).InnerText
                                Case "label"
                                    Dim cLabel As Label = DirectCast(Page.FindControl(strfieldname), Label)
                                    cLabel.Text = node.Attributes(strfieldname).InnerText
                                Case "calendar"
                                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                                    cTextbox.Text = node.Attributes(strfieldname).InnerText
                                Case "dropdownlist"
                                    Dim cList As DropDownList = DirectCast(Page.FindControl(strfieldname), DropDownList)
                                    Dim iindex As Integer = 0
                                    If node.Attributes(strfieldname).InnerText <> "" Then
                                        For Each item As ListItem In cList.Items
                                            If NodeSchema.Attributes("Validate_DataType").InnerText = "boolean" Then
                                                If node.Attributes(strfieldname).InnerText = "1" Then
                                                    cList.SelectedValue = "Yes"
                                                Else
                                                    cList.SelectedValue = "No"
                                                End If
                                                Exit For
                                            Else
                                                If item.Value.ToUpper.Trim = "" Then
                                                    Dim a = strfieldname
                                                End If
                                                If item.Value.ToUpper.Trim = node.Attributes(strfieldname).InnerText.ToUpper.Trim Then
                                                    'cList.SelectedValue = node.Attributes(strfieldname).InnerText
                                                    cList.SelectedIndex = iindex
                                                    Exit For
                                                End If
                                                iindex = iindex + 1
                                            End If
                                        Next
                                    Else
                                        For Each item As ListItem In cList.Items
                                            If item.Value.ToUpper.Trim = "" Then
                                                cList.SelectedValue = item.Value
                                                Exit For
                                            End If
                                        Next

                                        'cList.SelectedIndex = 0
                                    End If
                            End Select
                        End If
                    Next

                    PanelR.Visible = True
                    pnSave.Visible = True

                Else
                    ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Data not found');", True)
                End If
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Data not found');", True)
            End If

        Catch ex As Exception
            Dim vError As String = "alert('Error during [ShowDetailEdit] (" + ex.Message + ")');"
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", vError, True)
        End Try

    End Sub

    Private Function ValidateControl() As Boolean
        Dim chk As Boolean
        chk = True
        Dim strfieldname As String
        Dim xmlString As String = ""
        Dim xmlread As XmlDocument
        Dim AttributeNodeList As XmlNodeList

        xmlread = New XmlDocument
        xmlString = Session("CreateSchema").ToString
        xmlread.LoadXml(xmlString)
        '
        AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_Validate_Blank)

        'วน schema เพื่อหา control ที่ต้องการเชค blank
        For Each NodeSchema As XmlNode In AttributeNodeList

            strfieldname = NodeSchema.Attributes("FieldName").InnerText
            Select Case NodeSchema.Attributes("Control_Type").InnerText
                Case "textbox"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If cTextbox.Text = "" Then
                        chk = chk And False
                    Else
                        chk = chk And True
                    End If
                Case "label"
                    Dim cLabel As Label = DirectCast(Page.FindControl(strfieldname), Label)
                    If cLabel.Text = "" Then
                        chk = chk And False
                    Else
                        chk = chk And True
                    End If
                Case "calendar"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If cTextbox.Text = "" Then
                        chk = chk And False
                    Else
                        chk = chk And True
                    End If
                Case "dropdownlist"
                    Dim cList As DropDownList = DirectCast(Page.FindControl(strfieldname), DropDownList)
                    If cList.Text = "" Then
                        chk = chk And False
                    Else
                        chk = chk And True
                    End If
            End Select

        Next

        Return chk

    End Function

    Private Function ValidateDuplicate() As Boolean
        If lbStatus.Text <> "Add" Then Return True

        Dim chk As Boolean
        chk = True
        Dim strDuplicate As String
        Dim xmlString As String = ""
        Dim xmlread As XmlDocument
        Dim xmlResult As New XmlDocument
        Dim AttributeNodeList As XmlNodeList
        Dim resultNode As XmlNode
        Dim stringXMLResult As String = ""
        Dim DuplicateArray As Array
        Dim Lookup_Data As String = ""

        HDduplicate.Value = ""
        xmlread = New XmlDocument
        xmlString = Session("CreateSchema").ToString
        xmlread.LoadXml(xmlString)
        '
        AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_Validate_Duplicate)

        'วน schema เพื่อหา control ที่ต้องการเชค Duplicate
        For Each NodeSchema As XmlNode In AttributeNodeList

            strDuplicate = NodeSchema.Attributes("Validate_Duplicate_Function").InnerText

            DuplicateArray = (Split(strDuplicate, ","))
            Dim vString As String
            Dim i As Integer = 0
            For Each vString In DuplicateArray
                Dim vDuplicate As String
                vDuplicate = vString.Trim

                If i = 0 Then
                    Lookup_Data = vString.Trim
                ElseIf i = 1 Then
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(vDuplicate), TextBox)
                    Lookup_Data = Lookup_Data + " '" + cTextbox.Text.Trim + "'"
                    HDduplicate.Value = HDduplicate.Value + cTextbox.Text.Trim
                Else
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(vDuplicate), TextBox)
                    Lookup_Data = Lookup_Data + ", '" + cTextbox.Text.Trim + "'"
                    HDduplicate.Value = HDduplicate.Value + ", " + cTextbox.Text.Trim
                End If
                i = i + 1
            Next

            DatabaseDescription_.ExecuteSQL(Lookup_Data, stringXMLResult)
            If stringXMLResult <> "<?xml version='1.0' encoding='utf-8'  ?>" Then
                chk = False
                Exit For
            End If

        Next

        Return chk

    End Function

    Private Sub CheckAllowAdd()

        Dim xmlString As String = ""
        Dim xmlread As XmlDocument
        Dim AttributeNodeList As XmlNodeList

        xmlread = New XmlDocument
        xmlString = Session("CreateSchema").ToString
        xmlread.LoadXml(xmlString)
        '
        AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_VALUE)
        Dim strfieldname As String
        'วน schema เพื่อหา control ที่ต้องการ Enabled Add
        For Each NodeSchema As XmlNode In AttributeNodeList

            strfieldname = NodeSchema.Attributes("FieldName").InnerText
            Select Case NodeSchema.Attributes("Control_Type").InnerText
                Case "textbox"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If NodeSchema.Attributes("AllowAdd").InnerText = "1" Then
                        cTextbox.Enabled = True
                    Else
                        cTextbox.Enabled = False
                    End If
                Case "calendar"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If NodeSchema.Attributes("AllowAdd").InnerText = "1" Then
                        cTextbox.Enabled = True
                    Else
                        cTextbox.Enabled = False
                    End If
                Case "dropdownlist"
                    Dim cList As DropDownList = DirectCast(Page.FindControl(strfieldname), DropDownList)
                    If NodeSchema.Attributes("AllowAdd").InnerText = "1" Then
                        cList.Enabled = True
                    Else
                        cList.Enabled = False
                    End If
            End Select

        Next
    End Sub

    Private Sub Save()

        ' init field to be saved by compare fields in excel with the configuration...

        Dim NewNode As XmlNode
        Dim NewAttr As XmlAttribute
        Dim XMLSchema As XmlDocument
        Dim AttributeNodeList As XmlNodeList
        Dim XMLSTRING As String = ""
        Dim NewDeclare As XmlDeclaration
        Dim XMLSave As New XmlDocument

        Dim stringXMLResult As String = ""
        Dim xmlResult As New XmlDocument
        Dim resultNode As XmlNode

        Try

            NewNode = XMLSave.CreateElement(TEXT_CREATEELEMENTS)
            XMLSave.AppendChild(NewNode)

            NewDeclare = XMLSave.CreateXmlDeclaration("1.0", "", Nothing)
            'Add the new node to the document. 
            Dim rootX As XmlElement = XMLSave.DocumentElement
            XMLSave.InsertBefore(NewDeclare, rootX)

            NewNode = XMLSave.CreateElement(TEXT_CREATEELEMENT)

            ' Create all attribure from schema...
            XMLSchema = New XmlDocument
            XMLSTRING = Session("CreateSchema").ToString
            XMLSchema.LoadXml(XMLSTRING)

            AttributeNodeList = XMLSchema.DocumentElement.ChildNodes
            ' Loop of each field...
            For Each node As XmlNode In AttributeNodeList
                NewAttr = XMLSave.CreateAttribute(node.Attributes("FieldName").Value)
                NewNode.Attributes.Append(NewAttr)
            Next
            ' assign value to attribute....
            Dim FieldName As String
            For Each node As XmlNode In AttributeNodeList
                FieldName = node.Attributes("FieldName").Value

                Select Case node.Attributes("Control_Type").Value

                    Case "textbox"
                        Dim textbox As TextBox = DirectCast(Page.FindControl(FieldName), TextBox)
                        NewNode.Attributes(FieldName).Value = textbox.Text.Trim
                    Case "dropdownlist"
                        Dim dropdownlist As DropDownList = DirectCast(Page.FindControl(FieldName), DropDownList)
                        If node.Attributes("Validate_DataType").Value = "boolean" Then
                            If dropdownlist.Text = "Yes" Then
                                NewNode.Attributes(FieldName).Value = "True"
                            Else
                                NewNode.Attributes(FieldName).Value = "False"
                            End If
                        Else
                            NewNode.Attributes(FieldName).Value = dropdownlist.Text
                        End If
                    Case "calendar"
                        Dim textbox As TextBox = DirectCast(Page.FindControl(FieldName), TextBox)
                        NewNode.Attributes(FieldName).Value = textbox.Text.Trim
                    Case "fileupload"
                        Dim textbox As TextBox = DirectCast(Page.FindControl("ProjectCode"), TextBox)
                        Dim cFileUpload As FileUpload = DirectCast(Page.FindControl(FieldName), FileUpload)

                        Dim folderPath As String = Server.MapPath("~/FilePDf/" + textbox.Text + "/")

                        'Dim folderPath As String = "\\sslvmsql1\APPLICATION\ProjectMaintenance\FilePDf\" + textbox.Text + "\"
                        'Check whether Directory (Folder) exists.
                        If Not Directory.Exists(folderPath) Then
                            'If Directory (Folder) does not exists. Create it.
                            Directory.CreateDirectory(folderPath)
                        End If

                        If cFileUpload.HasFile Then
                            'Save the File to the Directory (Folder).

                            cFileUpload.SaveAs(folderPath & Path.GetFileName(cFileUpload.FileName))
                            ' ส่งซื่อไฟล์ไปที่ attibute นี้แทน attibute name ชื่อไฟล์ เอาไว้ดูไฟล์เก่าที่เคยเซฟ อัพเดทตอนเซฟอีกที
                            ' path ตอนนี้ตั้งไว้ที่ sslvmsql1 
                            NewNode.Attributes(FieldName).Value = Path.GetFileName(cFileUpload.PostedFile.FileName)


                            'Using conn As New SqlConnection("Persist Security Info=False;User ID=ws_serv;Password=ws_serv;Initial Catalog=MFGSAG;Data Source=sslvmsql1;Max Pool Size=100;Connection TimeOut=1500")
                            '    conn.Open()
                            '    Using cmd As New SqlCommand("Insert Into ProjectCode (ProjectCode,) Values (@ProjectCode)", conn)
                            '        cmd.Parameters.Add(New SqlParameter("@PDF", SqlDbType.VarBinary)).Value = System.IO.File.ReadAllBytes(cFileUpload.FileName)

                            '        cmd.ExecuteNonQuery()
                            '    End Using
                            '    conn.Close()
                            'End Using

                        End If


                        'Dim bytes() As Byte = IO.File.ReadAllBytes(cFileUpload.PostedFile.FileName) 'Read the file
                        'Dim sb As New System.Text.StringBuilder
                        'For Each b As Byte In bytes
                        '    sb.Append(Convert.ToString(b, 2).PadLeft(8, "0"c))
                        'Next
                        'Dim s As String = sb.ToString

                        'NewNode.Attributes(FieldName).Value = s




                End Select
            Next

            'Dim textboxStatus As TextBox = DirectCast(Page.FindControl("Status_"), TextBox)
            'If textboxStatus.Text = "TR" Then
            '    ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Cannot save status TR');", True)
            '    Exit Sub
            'End If

            XMLSave.DocumentElement.AppendChild(NewNode)
            'XML to String
            'XMLSave.Save("C:\temp\Save.xml")
            Dim strXML As String = XMLSave.OuterXml
            'Sent data string and return result
            If lbStatus.Text = "Add" Then
                Project_.Add(strXML, stringXMLResult)
            ElseIf lbStatus.Text = "Edit" Then
                Project_.Save(strXML, stringXMLResult)
            ElseIf lbStatus.Text = "Delete" Then
                Project_.Delete(strXML, stringXMLResult)
            End If


            xmlResult.LoadXml(stringXMLResult)
            'find result
            resultNode = xmlResult.SelectSingleNode("results/result")

            If resultNode Is Nothing Then
                ' sent value to show alert in sub procedure
                ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [Save], Please contact IS Department !!!');", True)
            Else
                If resultNode.ChildNodes(0).InnerText.ToUpper = "TRUE" Then
                    SetDefaultData()
                    PanelR.Visible = False
                    pnSave.Visible = False
                    'for refresh data create session
                    GetAlldata()
                    'for Search in session xml
                    GetAlldataSearch(txtSearch.Text.Trim)
                Else
                    ' sent value to show alert in sub procedure
                    Dim vString = "alert('Error during [Save]," + resultNode.ChildNodes(1).InnerText.Replace("'", """") + "');"
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Alert", vString, True)
                    'ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [Save], Please contact IS Department !!! (check result)');", True)
                End If
            End If

        Catch ex As Exception
            ' sent value to show alert in sub procedure
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [Save] (" + ex.Message + ")');", True)
        Finally
            XMLSchema = Nothing
        End Try
    End Sub
    Sub ShowPageCommand(ByVal s As Object, ByVal e As GridViewPageEventArgs) Handles gvData.PageIndexChanging
        gvData.PageIndex = e.NewPageIndex
        If ViewState("TypeSearch") = "Text" Then
            GetAlldataSearch(ViewState("CurrentAlphabet"))
        Else
            GetAlldataSearchAlphabets(ViewState("CurrentAlphabet"))
        End If

    End Sub

    Private Sub SetDefaultData()
        Dim chk As Boolean
        chk = True

        Dim xmlString As String = ""
        Dim xmlread As XmlDocument
        Dim AttributeNodeList As XmlNodeList

        xmlread = New XmlDocument
        xmlString = Session("CreateSchema").ToString
        xmlread.LoadXml(xmlString)
        '
        AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_VALUE)
        Dim strfieldname As String
        'วน schema เพื่อหา control ที่ต้องการใส่ค่า defualt
        For Each NodeSchema As XmlNode In AttributeNodeList

            strfieldname = NodeSchema.Attributes("FieldName").InnerText
            Select Case NodeSchema.Attributes("Control_Type").InnerText
                Case "textbox"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    cTextbox.Text = NodeSchema.Attributes("DefaultData").InnerText
                Case "label"
                    Dim cLabel As Label = DirectCast(Page.FindControl(strfieldname), Label)
                    cLabel.Text = NodeSchema.Attributes("DefaultData").InnerText
                Case "calendar"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If NodeSchema.Attributes("DefaultData").InnerText = "" Then
                        cTextbox.Text = DateTime.Today.ToString("dd/MM/yyyy")
                    Else
                        cTextbox.Text = NodeSchema.Attributes("DefaultData").InnerText
                    End If
                Case "dropdownlist"
                    Dim cList As DropDownList = DirectCast(Page.FindControl(strfieldname), DropDownList)
                    If NodeSchema.Attributes("DefaultData").InnerText <> "" Then
                        If NodeSchema.Attributes("Validate_DataType").InnerText = "boolean" Then
                            If NodeSchema.Attributes("DefaultData").InnerText = "True" Then
                                cList.Text = "Yes"
                            Else
                                cList.Text = "No"
                            End If

                        Else

                            For Each item As ListItem In cList.Items
                                If item.Value = NodeSchema.Attributes("DefaultData").InnerText Then
                                    cList.SelectedValue = NodeSchema.Attributes("DefaultData").InnerText
                                End If
                            Next

                        End If
                    Else
                        cList.SelectedValue = ""
                    End If

            End Select

        Next


    End Sub

    Private Sub SetDefaultDataHistory()
        Dim chk As Boolean
        chk = True

        Dim xmlString As String = ""
        Dim xmlread As XmlDocument
        Dim AttributeNodeList As XmlNodeList

        xmlread = New XmlDocument
        xmlString = Session("CreateSchemaHistory").ToString
        xmlread.LoadXml(xmlString)
        '
        AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_VALUE)
        Dim strfieldname As String
        'วน schema เพื่อหา control ที่ต้องการใส่ค่า defualt
        For Each NodeSchema As XmlNode In AttributeNodeList

            strfieldname = "E" + NodeSchema.Attributes("FieldName").InnerText
            Select Case NodeSchema.Attributes("Control_Type").InnerText
                Case "textbox"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    cTextbox.Text = NodeSchema.Attributes("DefaultData").InnerText
                Case "label"
                    Dim cLabel As Label = DirectCast(Page.FindControl(strfieldname), Label)
                    cLabel.Text = NodeSchema.Attributes("DefaultData").InnerText
                Case "calendar"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If NodeSchema.Attributes("DefaultData").InnerText = "" Then
                        cTextbox.Text = "" 'DateTime.Today.ToString("dd/MM/yyyy")
                    Else
                        cTextbox.Text = NodeSchema.Attributes("DefaultData").InnerText
                    End If
                Case "dropdownlist"
                    Dim cList As DropDownList = DirectCast(Page.FindControl(strfieldname), DropDownList)
                    If NodeSchema.Attributes("DefaultData").InnerText <> "" Then
                        If NodeSchema.Attributes("Validate_DataType").InnerText = "boolean" Then
                            If NodeSchema.Attributes("DefaultData").InnerText = "True" Then
                                cList.Text = "Yes"
                            Else
                                cList.Text = "No"
                            End If

                        Else
                            For Each item As ListItem In cList.Items
                                If item.Value = NodeSchema.Attributes("DefaultData").InnerText Then
                                    cList.Text = NodeSchema.Attributes("DefaultData").InnerText
                                End If
                            Next
                        End If
                    End If
            End Select
        Next

    End Sub


    Private Sub CheckAllowEdit()
        Dim xmlString As String = ""
        Dim xmlread As XmlDocument
        Dim AttributeNodeList As XmlNodeList

        xmlread = New XmlDocument
        xmlString = Session("CreateSchema").ToString
        xmlread.LoadXml(xmlString)
        '
        AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_VALUE)
        Dim strfieldname As String
        'วน schema เพื่อหา control ที่ต้องการ Enabled Edit
        For Each NodeSchema As XmlNode In AttributeNodeList

            strfieldname = NodeSchema.Attributes("FieldName").InnerText
            Select Case NodeSchema.Attributes("Control_Type").InnerText
                Case "textbox"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If NodeSchema.Attributes("AllowEdit").InnerText = "1" Then
                        cTextbox.Enabled = True
                    Else
                        cTextbox.Enabled = False
                    End If
                Case "calendar"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If NodeSchema.Attributes("AllowEdit").InnerText = "1" Then
                        cTextbox.Enabled = True
                    Else
                        cTextbox.Enabled = False
                    End If
                Case "dropdownlist"
                    Dim cList As DropDownList = DirectCast(Page.FindControl(strfieldname), DropDownList)
                    If NodeSchema.Attributes("AllowEdit").InnerText = "1" Then
                        cList.Enabled = True
                    Else
                        cList.Enabled = False
                    End If
            End Select

        Next
    End Sub

    Private Sub CheckAllowEditHistory()
        Dim xmlString As String = ""
        Dim xmlread As XmlDocument
        Dim AttributeNodeList As XmlNodeList

        xmlread = New XmlDocument
        xmlString = Session("CreateSchemaHistory").ToString
        xmlread.LoadXml(xmlString)
        '
        AttributeNodeList = xmlread.SelectNodes(SCHEMA_XPATH_VALUE)
        Dim strfieldname As String
        'วน schema เพื่อหา control ที่ต้องการ Enabled Edit
        For Each NodeSchema As XmlNode In AttributeNodeList

            strfieldname = "E" + NodeSchema.Attributes("FieldName").InnerText
            Select Case NodeSchema.Attributes("Control_Type").InnerText
                Case "textbox"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If NodeSchema.Attributes("AllowEdit").InnerText = "1" Then
                        cTextbox.Enabled = True
                    Else
                        cTextbox.Enabled = False
                    End If
                Case "calendar"
                    Dim cTextbox As TextBox = DirectCast(Page.FindControl(strfieldname), TextBox)
                    If NodeSchema.Attributes("AllowEdit").InnerText = "1" Then
                        cTextbox.Enabled = True
                    Else
                        cTextbox.Enabled = False
                    End If
                Case "dropdownlist"
                    Dim cList As DropDownList = DirectCast(Page.FindControl(strfieldname), DropDownList)
                    If NodeSchema.Attributes("AllowEdit").InnerText = "1" Then
                        cList.Enabled = True
                    Else
                        cList.Enabled = False
                    End If
            End Select

        Next
    End Sub


    Private Sub AddBoundField(ByVal grid As GridView,
                              ByVal ControlID As String,
                              ByVal columnWidth As Integer,
                              ByVal Description As String,
                              Optional ByVal fontWeight As Integer = 11)
        'สร้าง BoundField

        Dim b As New BoundField
        b.HeaderText = Description
        b.DataField = ControlID
        b.HeaderStyle.Width = Unit.Pixel(columnWidth)
        b.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        b.HeaderStyle.Font.Name = "Tahoma"
        b.HeaderStyle.CssClass = "text-center"
        b.ItemStyle.HorizontalAlign = HorizontalAlign.Left
        b.ItemStyle.Font.Size = Unit.Pixel(fontWeight).Value
        b.ItemStyle.Font.Name = "Tahoma"
        grid.Columns.Add(b)

    End Sub

    Private Sub AddBoundField_History(ByVal grid As GridView,
                            ByVal ControlID As String,
                            ByVal columnWidth As Integer,
                            ByVal Description As String,
                            Optional ByVal fontWeight As Integer = 11)
        'สร้าง BoundField

        Dim b As New BoundField
        b.HeaderText = Description
        b.DataField = ControlID
        b.HeaderStyle.Width = Unit.Pixel(columnWidth)
        b.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        b.HeaderStyle.Font.Name = "Tahoma"
        b.HeaderStyle.CssClass = "text-center"
        b.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        b.ItemStyle.Font.Size = Unit.Pixel(fontWeight).Value
        b.ItemStyle.Font.Name = "Tahoma"
        grid.Columns.Add(b)

    End Sub


    Private Function ValidateBeforeDelete() As Boolean
        Dim cTextbox As TextBox = DirectCast(Page.FindControl("EmployeeID"), TextBox)
        If cTextbox.Text = "" Then
            Return False
        End If
        Return True

    End Function

    Private Function ConvertToYMD(ByVal DATESTRING As String) As String
        Dim arDatetime As Array
        Dim arTime As Array
        Dim dateYMD As String
        If DATESTRING.Length > 10 Then
            arTime = DATESTRING.Split(" ")
            arDatetime = arTime(0).Split("/")
            dateYMD = arDatetime(2) + "/" + arDatetime(1) + "/" + arDatetime(0) + " " + arTime(1)
        Else
            arDatetime = DATESTRING.Split("/")
            dateYMD = arDatetime(2) + "/" + arDatetime(1) + "/" + arDatetime(0)
        End If

        Return dateYMD

    End Function

    'Private Sub EndScan()
    '    lblEmpID.Text = ""
    '    lblName.Text = ""
    '    Utility_.Abort()
    '    ModalPopupFinger.Hide()
    'End Sub

    Private Function CheckUserActive_ByName(ByVal sLoginName As String) As Boolean
        Dim XMLSTRING As String = ""
        Dim XMLDocEmp As New XmlDocument
        Dim Nodelist As XmlNodeList
        Dim sUserName As String = ""

        sUserName = sLoginName

        SecurityUser_.GetActive(XMLSTRING)
        XMLDocEmp.LoadXml(XMLSTRING)
        Nodelist = XMLDocEmp.SelectNodes("/SecurityUsers/SecurityUser [@SecurityUser='" & sUserName & "']")
        If Nodelist.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Protected Sub Btn_ClickTranfer(sender As Object, e As EventArgs) Handles cmdTransfer.Click
        Dim stringXMLResult As String = ""
        Dim xmlResult As New XmlDocument
        Dim resultNode As XmlNode


        Project_.TransferToSage(stringXMLResult)

        xmlResult.LoadXml(stringXMLResult)
        'find result
        resultNode = xmlResult.SelectSingleNode("results/result")

        If resultNode Is Nothing Then
            ' sent value to show alert in sub procedure
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [Transfer], Please contact IS Department !!!');", True)
        Else
            If resultNode.ChildNodes(0).InnerText.ToUpper = "TRUE" Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Completed');", True)
            Else
                ' sent value to show alert in sub procedure
                Dim vString = "alert('Error during [Transfer]," + resultNode.ChildNodes(1).InnerText.Replace("'", """") + "');"
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Alert", vString, True)
                'ScriptManager.RegisterStartupScript(Me, Me.GetType, "alert", "alert('Error during [Save], Please contact IS Department !!! (check result)');", True)
            End If
        End If
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "text", "hidePleaseWait()", True)

    End Sub

    'Protected Sub btnUpload1_Click(sender As Object, e As EventArgs) Handles btnUpload1.Click
    '    If ValidateBeforeUpload() Then
    '    End If
    'End Sub

    'Private Function ValidateBeforeUpload() As Boolean
    '    Dim cTextbox As TextBox = DirectCast(Page.FindControl("ProjectCode"), TextBox)
    '    Dim cFileUpload As FileUpload = DirectCast(Page.FindControl("Attachment1"), FileUpload)

    '    If cTextbox.Text = "" Then

    '        Return False
    '    End If
    '    If Not cFileUpload.HasFile Then

    '        Return False
    '    End If

    '    Return True

    'End Function

    'Private Function CheckLoginSecurity(ByVal sUserID As String) As Boolean
    '    Dim XMLSTRING As String = ""
    '    Dim XMLDocEmp As New XmlDocument
    '    Dim Nodelist As XmlNodeList
    '    Dim canViewpage As Boolean = False

    '    Dim CanChange As Boolean = False

    '    SecurityRule_.GetAvailableFunction(sUserID, XMLSTRING)
    '    If XMLSTRING = OUTPUT_BLANK Then
    '        Return False
    '    Else

    '        If XMLSTRING = "<?xml version='1.0' encoding='utf-8'  ?>" Then
    '            ScriptManager.RegisterStartupScript(me.Page, Page.GetType, "alert", "alert('Error during [GetAvailableFunction : Security] ');", True)
    '        Else
    '            XMLDocEmp.LoadXml(XMLSTRING)
    '            Nodelist = XMLDocEmp.SelectNodes("SecurityRules/SecurityRule")
    '            If Nodelist.Count = 0 Then
    '                Return False
    '            Else
    '                For Each XNode As XmlNode In Nodelist
    '                    If XNode.Attributes("SecurityFunction").Value = "Employee Maintenance" Then
    '                        CanChange = True
    '                    End If

    '                Next

    '                If CanChange = True Then
    '                    Return True
    '                Else
    '                    Return False
    '                End If

    '            End If
    '        End If

    '    End If
    'End Function



#End Region

End Class