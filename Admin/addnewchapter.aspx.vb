Imports System
Imports System.Globalization
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient

Imports mojoPortal.Business
Imports mojoPortal.Business.WebHelpers
Imports mojoPortal.Web.Framework
Imports mojoPortal.Features.UI
Imports mojoPortal.Web
Imports mojoPortal.Web.Controls
Imports mojoPortal.Web.UI
Imports mojoPortal.Web.Editor
Imports mojoPortal.Net

Public Class addnewchapter

    Inherits mojoBasePage

    Dim mypageId As Integer = -1
    Dim mymoduleId As Integer = -1
    Dim home As String = ""
    Dim addnewchaptererror As String = ""
    Dim addnewchaptereditor As String
    Dim newchapternametext As String = ""
    Dim seriesnumber As Integer = -1
    Dim errorstring As String = ""
    Private siteurl = ConfigurationManager.AppSettings("siteurl")
    Private Sitefolder = ConfigurationManager.AppSettings("Sitefolder")
    Private modulepathadmin = ConfigurationManager.AppSettings("modulepathadmin")
    Private Const featureGuid As String = "4d5b533a-b4db-47c8-94d0-9f1cabca94ac"

    Public Shared ReadOnly Property FeatureGuidGuid() As Guid
        Get
            Return New Guid(featureGuid)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadParams()
        If Not Page.IsPostBack Then
            bind_existingseriesdropdown()
        End If

    End Sub

    Sub LoadParams()
        mypageId = WebUtils.ParseInt32FromQueryString("pageid", mypageId)
        mymoduleId = WebUtils.ParseInt32FromQueryString("mid", mymoduleId)
        home = SiteRoot & "/default.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        addnewchaptererror = SiteRoot & modulepathadmin & "/addnewchaptererror.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        addnewchaptereditor = SiteRoot & modulepathadmin & "/addnewchaptereditor.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        If Not UserCanViewPage(mymoduleId, FeatureGuidGuid) Then
            If Not Request.IsAuthenticated Then
                SiteUtils.RedirectToLoginPage(Me)
            Else
                SiteUtils.RedirectToAccessDeniedPage(Me)
            End If
            Return
        End If
        REM Now we know which series this chapter is to be added to we can display the form to assign a chapter name
        If IsPostBack Then
            seriesnumber = WebUtils.ParseInt32FromQueryString("sn", seriesnumber)
        End If
        'If seriesnumber <> -1 Then
        REM there is an incoming SERIES number so go populate the form.
        'listlabel.Visible = False
        'existingseriesdropdown.Visible = FalseREM Now we know which series this chapter is to be added to we can display the form to assign a chapter name
        'msg_existingseriesdropdown.Visible = False

        'gopopulateform(seriesnumber)
        'Else
        REM  No series number so blank the form and present dropdown, checking first if there is data and redirecting if not.
        ' bind_existingseriesdropdown()
        'End If
        BacktoDashboard.NavigateUrl = "/admin"
        'Cancel.PostBackUrl = "/property-admin"
    End Sub

    Protected Sub bind_existingchapterdropdown()
        msg_existingseriesdropdown.Text = ""
        Dim myreader As SqlDataReader
        Dim cn As New SqlConnection
        Dim cmd As New SqlCommand
        Dim strsql As String = ""
        strsql = "SELECT chapter.chaptername, chapter.DatechapterCreated  FROM chapter   ORDER BY DatechapterCreated asc"
        cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
        cn.Open()
        cmd.Connection = cn
        cmd.CommandText = strsql

        Try
            myreader = cmd.ExecuteReader

            If myreader.HasRows Then
                existingseriesdropdown.DataSource = myreader
                existingseriesdropdown.DataValueField = "chaptername"
                existingseriesdropdown.DataTextField = "chaptername"
                existingseriesdropdown.DataBind()
            Else
                msg_existingseriesdropdown.Text = msg_existingseriesdropdown.Text & " Error getting chapter list"
            End If
            myreader.Close()
            cn.Close()
        Catch ex As Exception
            'msg_post.Text = "There was an error updating the database: " & ex.ToString

            existingseriesdropdown.Visible = False
            msg_existingseriesdropdown.Text = "No chapter Found"
        End Try
    End Sub

    Private Function checknewchaptername(newchaptername As String) As Boolean
        Dim retval As Boolean = True
        Try

            Dim myreader As SqlDataReader
            Dim cn As New SqlConnection
            Dim cmd As New SqlCommand
            Dim strsql As String = ""
            cmd.Parameters.Add("@chaptername", SqlDbType.VarChar, 255).Value = newchaptername
            strsql = "SELECT chapter.chapterName  FROM chapter where chapter.chaptername = @chaptername"
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandText = strsql
            myreader = cmd.ExecuteReader
            If myreader.HasRows Then
                retval = True
            Else
                retval = False
            End If
            myreader.Close()
            cn.Close()
        Catch ex As Exception
            retval = False
        End Try
        Return retval
    End Function

    Public Function GetSQL(ByRef cmd As System.Data.SqlClient.SqlCommand) As String
        REM This function takes an SQL command object and extracts the sql and the parameters, 
        REM substitues them and returns a string that can be used as SQL for testing
        'Dim P As New System.Data.OleDb.OleDbParameter
        If cmd.CommandText Is Nothing Or cmd.CommandText.Length = 0 Then
            GetSQL = "No SQL Found in command statement"
            Return GetSQL
        End If

        Dim msg As String = ""

        msg = cmd.CommandText

        For Each p As System.Data.SqlClient.SqlParameter In cmd.Parameters
            msg = msg.Replace(p.ParameterName, p.Value.ToString())
        Next

        GetSQL = msg
    End Function

    Function getchapterIDnumber(newchaptername) As Integer
        Dim retval As Integer = -1
        Try

            Dim myreader As SqlDataReader
            Dim cn As New SqlConnection
            Dim cmd As New SqlCommand
            Dim strsql As String = ""
            cmd.Parameters.Add("@chaptername", SqlDbType.VarChar, 255).Value = newchaptername
            strsql = "SELECT serieschapters.chapterID  FROM serieschapters where serieschapters.chaptername = @chaptername"
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandText = strsql
            errorstring = GetSQL(cmd)
            myreader = cmd.ExecuteReader
            If myreader.HasRows Then
                Do While myreader.Read
                    retval = myreader.Item("chapterID").ToString
                Loop
            Else
                retval = -1
            End If
            myreader.Close()
            cn.Close()
        Catch ex As Exception
            retval = -1
        End Try
        Return retval
    End Function

    Protected Sub bind_existingseriesdropdown()
        If Not IsPostBack Then
            msg_existingseriesdropdown.Text = ""
            Dim myreader As SqlDataReader
            Dim cn As New SqlConnection
            Dim cmd As New SqlCommand
            Dim strsql As String = ""
            strsql = "SELECT series.seriesname, series.SeriesID  FROM series   ORDER BY SeriesName asc"
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandText = strsql

            Try
                myreader = cmd.ExecuteReader

                If myreader.HasRows Then
                    existingseriesdropdown.DataSource = myreader
                    existingseriesdropdown.DataValueField = "seriesid"
                    existingseriesdropdown.DataTextField = "seriesname"
                    existingseriesdropdown.DataBind()
                Else
                    msg_existingseriesdropdown.Text = msg_existingseriesdropdown.Text & " Error getting series list"
                End If
                myreader.Close()
                cn.Close()
            Catch ex As Exception
                'msg_post.Text = "There was an error updating the database: " & ex.ToString

                existingseriesdropdown.Visible = False
                msg_existingseriesdropdown.Text = "No Series Found"
            End Try
        End If
    End Sub

    Protected Sub submitnewchaptername_Click(sender As Object, e As EventArgs) Handles submitnewchaptername.Click
        REM get the index of the dropdown and redirect to the addnewchapter page with the sn argument
        REM check that incoming chapter name is not already in the database.
        REM it is, get back to the user with a warning
        REM if it is not in the database then add it and redirect to a page that does the friendly url
        Dim submitchapterflag As Boolean = False
        Dim errortext As String = ""
        newchapternametext = addnewchaptername.Text
        If checknewchaptername(newchapternametext) = False Then
            REM insert the name of the chapter into the db.
            Try
                Dim st1 As String = ""
                Dim st2 As String = ""
                Dim strSQL As String = ""
                Dim cn As New SqlConnection
                Dim cmd As New SqlCommand
                If seriesnumber = -1 Then 'if not -1 then we have querystring
                    seriesnumber = existingseriesdropdown.SelectedItem.Value
                End If
                cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
                cn.Open()

                st1 = "insert into serieschapters(ParentSeriesId,chaptername,DatechapterCreated,chapterispublished,chapterGuid)"
                st2 = " values(@ParentSeriesId, @chaptername, SYSUTCDATETIME(), 0,NewID())"

                strSQL = st1 & st2

                cmd.Parameters.AddWithValue("@chaptername", addnewchaptername.Text)
                cmd.Parameters.AddWithValue("@ParentSeriesId", seriesnumber)

                cmd.Connection = cn
                cmd.CommandText = strSQL
                errorstring = GetSQL(cmd)
                'Response.Write(GetSQL(cmd))
                'Response.End()
                cmd.ExecuteNonQuery()
                cn.Close()
                submitchapterflag = True



            Catch ex As Exception
                errorstring = ex.Message
                REM go back to user with message  
                'Response.Redirect(addnewchaptererror & "&err=" & errorstring.Replace(vbCr, "").Replace(vbLf, ""))
                submitchapterflag = False
                errortext = ("&err=" & errorstring.Replace(vbCr, "").Replace(vbLf, ""))
            End Try
        Else
            REM go back to user with message
            Response.Redirect(addnewchaptererror & "&err=" & errorstring.Replace(vbCr, "").Replace(vbLf, ""))
        End If
        If submitchapterflag = True Then
            'Response.Redirect(home)
            REM now we need to pick a friendly URL
            REM redirect to the editor, pick up new name and us it to edit.
            REM need to get the db id of the new chapter name
            Response.Redirect(addnewchaptereditor & "&cn=" & getchapterIDnumber(newchapternametext))
        Else
            Response.Redirect(addnewchaptererror & "&err=" & errortext)
        End If
        Response.Redirect(addnewchaptereditor & "&sn=" & seriesnumber)
    End Sub

End Class