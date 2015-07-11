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
Public Class addnewseries
    Inherits mojoBasePage


    Dim mypageId As Integer = -1
    Dim mymoduleId As Integer = -1
    Dim home As String = ""
    Dim addnewserieserror As String = ""
    Dim addnewserieseditor As String
    Dim newseriesnametext As String = ""
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
        addnewserieserror = SiteRoot & modulepathadmin & "/addnewserieserror.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        addnewserieseditor = SiteRoot & modulepathadmin & "/addnewserieseditor.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        If Not UserCanViewPage(mymoduleId, FeatureGuidGuid) Then
            If Not Request.IsAuthenticated Then
                SiteUtils.RedirectToLoginPage(Me)
            Else
                SiteUtils.RedirectToAccessDeniedPage(Me)
            End If
            Return
        End If
        BacktoDashboard.NavigateUrl = "/admin"
        'Cancel.PostBackUrl = "/property-admin"
    End Sub

    Protected Sub bind_existingseriesdropdown()
        msg_existingseriesdropdown.Text = ""
        Dim myreader As SqlDataReader
        Dim cn As New SqlConnection
        Dim cmd As New SqlCommand
        Dim strsql As String = ""
        strsql = "SELECT series.seriesname, series.DateSeriesCreated  FROM series   ORDER BY DateSeriesCreated asc"
        cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
        cn.Open()
        cmd.Connection = cn
        cmd.CommandText = strsql

        Try
            myreader = cmd.ExecuteReader

            If myreader.HasRows Then
                existingseriesdropdown.DataSource = myreader
                existingseriesdropdown.DataValueField = "seriesname"
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
    End Sub

    Protected Sub submitnewnseriesname_click(sender As Object, e As EventArgs) Handles submitnewnseriesname.click
        REM check that incoming series name is not already in the database.
        REM it is, get back to the user with a warning
        REM if it is not in the database then add it and redirect to a page that does the friendly url
        Dim submitseriesflag As Boolean = False
        newseriesnametext = addnewseriesname.Text
        If checknewseriesname(newseriesnametext) = False Then
            REM insert the name of the series into the db.
            Try
                Dim st1 As String = ""
                Dim st2 As String = ""
                Dim strSQL As String = ""
                Dim cn As New SqlConnection
                Dim cmd As New SqlCommand

                cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
                cn.Open()

                st1 = "insert into Series(seriesname,DateSeriesCreated,seriesispublished,SeriesGuid)"
                st2 = " values(@seriesname, SYSUTCDATETIME(), 0,NewID())"

                strSQL = st1 & st2

                cmd.Parameters.AddWithValue("@seriesname", addnewseriesname.Text)

                cmd.Connection = cn
                cmd.CommandText = strSQL
                errorstring = GetSQL(cmd)
                'Response.Write(GetSQL(cmd))
                'Response.End()
                cmd.ExecuteNonQuery()
                cn.Close()
                submitseriesflag = True



            Catch ex As Exception
                errorstring = ex.Message
                REM go back to user with message  
                Response.Redirect(addnewserieserror & "&err=" & errorstring.Replace(vbCr, "").Replace(vbLf, ""))
            End Try
        Else
            REM go back to user with message
            Response.Redirect(addnewserieserror & "&err=" & errorstring.Replace(vbCr, "").Replace(vbLf, ""))
        End If
        If submitseriesflag = True Then
            'Response.Redirect(home)
            REM now we need to pick a friendly URL
            REM redirect to the editor, pick up new name and us it to edit.
            REM need to get the db id of the new series name
            Response.Redirect(addnewserieseditor & "&sn=" & getseriesIDnumber(newseriesnametext))
        Else
            Response.Redirect(addnewserieserror & "&err=unknown")
        End If
    End Sub

    Private Function checknewseriesname(newseriesname As String) As Boolean
        Dim retval As Boolean = True
        Try

            Dim myreader As SQLDataReader
            Dim cn As New SQLConnection
            Dim cmd As New SQLCommand
            Dim strsql As String = ""
            cmd.Parameters.Add("@Seriesname", SqlDbType.VarChar, 255).Value = newseriesname
            strsql = "SELECT series.SeriesName  FROM Series where series.seriesname = @seriesname"
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandText = strsql
            myreader = cmd.ExecuteReader
            If myreader.hasrows Then
                retval = True
            Else
                retval = False
            End If
            myreader.close()
            cn.close()
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

    Function getseriesIDnumber(newseriesname) As Integer
        Dim retval As Integer = -1
        Try

            Dim myreader As SqlDataReader
            Dim cn As New SqlConnection
            Dim cmd As New SqlCommand
            Dim strsql As String = ""
            cmd.Parameters.Add("@Seriesname", SqlDbType.VarChar, 255).Value = newseriesname
            strsql = "SELECT series.SeriesID  FROM Series where series.seriesname = @seriesname"
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandText = strsql
            myreader = cmd.ExecuteReader
            If myreader.HasRows Then
                Do While myreader.Read
                    retval = myreader.Item("seriesID").ToString
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

End Class