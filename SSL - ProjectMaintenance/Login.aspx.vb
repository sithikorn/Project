Option Explicit On

Imports System
Imports System.Xml
Imports System.Data
Imports System.IO

Public Class Login
    Inherits System.Web.UI.Page

    Private SecurityUser_ As New ws_securityuser.SecurityUser
    Private SecurityRule_ As New ws_securityrule.SecurityRule

    Private XMLEmpActive As New XmlDocument
    Private XMLSTRING As String = ""
    Private secuser_db As String = "SecurityUser"
    Private Const xmlNothing As String = "<?xml version='1.0' encoding='utf-8' ?>"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        ' Dim booEmployee As Boolean
        Dim booPassword As Boolean
        Dim booUserActive As Boolean
        Dim booSecurityFunction As Boolean

        ' Remove space from head and last of textbox value.
        txtUserName.Text = Trim(txtUserName.Text.ToUpper)
        txtPassword.Text = Trim(txtPassword.Text.ToUpper)

        ' Check txtUserName and txtPassword is not blank
        If txtUserName.Text <> "" Then
            If txtPassword.Text <> "" Then
                'booEmployee = CheckEmployeeID()
                'If booEmployee = True Then

                booUserActive = CheckUserActive()
                If booUserActive = False Then
                    lblPopUpLoginResult.Text = "User No Active!"
                Else
                    booPassword = CheckPassword()
                    If booPassword = False Then
                        lblPopUpLoginResult.Text = "Password is incorrect !"
                        txtPassword.Focus()
                    Else
                        booSecurityFunction = CheckLoginSecurity()
                        If booSecurityFunction = True Then
                            'Session.Timeout = 5
                            Session("strUsername") = txtUserName.Text
                            Session("strPassword") = txtPassword.Text
                            lblPopUpLoginResult.Text = "Successfull, Please wait...."
                            Session("LoginProject") = txtUserName.Text
                            Response.Redirect("frmProject.aspx")

                            ' Let page do scroll position on Top page
                            'Page.MaintainScrollPositionOnPostBack = True
                        Else
                            lblPopUpLoginResult.Text = "User No permission!"
                        End If
                    End If
                End If
                'Else
                '    lblMsg.Text = "Not found Employee ID : " + txtUserName.Text
                'End If
            Else
                lblPopUpLoginResult.Text = "Password could not be blank !!!"
                txtPassword.Focus()
            End If
        Else
            lblPopUpLoginResult.Text = "User name could not be blank !!!"
        End If
    End Sub

    Private Function CheckUserActive(Optional ByVal sLoginType As String = "") As Boolean
        Dim XMLSTRING As String = ""
        Dim XMLDocEmp As New XmlDocument
        Dim Nodelist As XmlNodeList
        Dim sUserName As String = ""

        If sLoginType = "" Then
            sUserName = txtUserName.Text
        Else
            If sLoginType.ToUpper = "LOGIN" Then
                sUserName = txtUserName.Text
            End If
        End If


        SecurityUser_.GetActive(XMLSTRING)
        XMLDocEmp.LoadXml(XMLSTRING)
        Nodelist = XMLDocEmp.SelectNodes("/SecurityUsers/SecurityUser [@SecurityUser='" & sUserName & "']")
        If Nodelist.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Function CheckPassword(Optional ByVal sLoginType As String = "") As Boolean
        Dim XMLSTRING As String = ""
        Dim XMLDocEmp As New XmlDocument
        Dim Nodelist As XmlNodeList

        Dim sUserName As String = ""
        Dim sPassword As String = ""

        If sLoginType = "" Then
            sUserName = txtUserName.Text
            sPassword = txtPassword.Text
        Else
            If sLoginType.ToUpper = "LOGIN" Then
                sUserName = txtUserName.Text
                sPassword = txtPassword.Text
            End If
        End If

        'SecurityUser_.CheckPassword(txtUserName.Text, txtPassword.Text, XMLSTRING)
        SecurityUser_.CheckPassword(sUserName, sPassword, XMLSTRING)
        XMLDocEmp.LoadXml(XMLSTRING)
        Nodelist = XMLDocEmp.SelectNodes("results/result [value='true']")
        If Nodelist.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Function CheckLoginSecurity() As Boolean
        Dim XMLSTRING As String = ""
        Dim XMLDocEmp As New XmlDocument
        Dim Nodelist As XmlNodeList

        SecurityRule_.GetAvailableFunction(txtUserName.Text, XMLSTRING)
        If XMLSTRING = xmlNothing Then
            Return False
        Else
            XMLDocEmp.LoadXml(XMLSTRING)
            Nodelist = XMLDocEmp.SelectNodes("SecurityRules/SecurityRule")
            If Nodelist.Count = 0 Then
                Return False
            Else
                For Each XNode As XmlNode In Nodelist
                    If XNode.Attributes("SecurityFunction").Value = "ProjectMaintenance_View" Then
                        Session("canViewpageProject") = True
                    End If
                    If XNode.Attributes("SecurityFunction").Value = "ProjectMaintenance_Edit" Then
                        Session("canEditProject") = True
                    End If

                Next

                If Session("canViewpageProject") = True Then
                    Return True
                Else
                    Return False
                End If


            End If
        End If
    End Function


End Class