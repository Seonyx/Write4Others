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

Public Class addnewserieseditor
    Inherits mojoBasePage

    Dim mypageId As Integer = -1
    Dim mymoduleId As Integer = -1
    Dim home As String = ""
    Dim seriesnumber As Integer = -1
    Dim addnewserieseditor As String
    Private IsupdateItemurl As Boolean = False
    Private siteurl = ConfigurationManager.AppSettings("siteurl")
    Private Sitefolder = ConfigurationManager.AppSettings("Sitefolder")
    Private modulepathadmin = ConfigurationManager.AppSettings("modulepathadmin")
    Private seriesGuid As Guid = Guid.Empty
    Private Const featureGuid As String = "4d5b533a-b4db-47c8-94d0-9f1cabca94ac"

    Public Shared ReadOnly Property FeatureGuidGuid() As Guid
        Get
            Return New Guid(featureGuid)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadParams()
    End Sub

    Sub LoadParams()
        Dim seriespopulatedflag As Boolean = False
        seriesdescription.WebEditor.ToolBar = ToolBar.FullWithTemplates
        REM note if there is an incoming querystring with a series number then populate the form with these details.
        REM if not, then allow the client to choose from a drop down list, unless there are none in which case redirect to the add series page
        mypageId = WebUtils.ParseInt32FromQueryString("pageid", mypageId)
        mymoduleId = WebUtils.ParseInt32FromQueryString("mid", mymoduleId)
        home = SiteRoot & "/default.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        addnewserieseditor = SiteRoot & modulepathadmin & "/addnewserieseditor.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        If Not UserCanViewPage(mymoduleId, FeatureGuidGuid) Then
            If Not Request.IsAuthenticated Then
                SiteUtils.RedirectToLoginPage(Me)
            Else
                SiteUtils.RedirectToAccessDeniedPage(Me)
            End If
            Return
        End If
        seriesnumber = WebUtils.ParseInt32FromQueryString("sn", seriesnumber)
        If seriesnumber <> -1 Then
            REM there is an incoming series number so go populate the form.
            'listlabel.Visible = False
            'existingseriesdropdown.Visible = False
            'msg_existingseriesdropdown.Visible = False
            Panel1.Visible = False
            If Not IsPostBack Then
                gopopulateform(seriesnumber)
            End If
        Else
            REM  No series number so blank the form and present dropdown, checking first if there is data and redirecting if not.
            Panel2.Visible = False
            bind_existingseriesdropdown()
        End If
        BacktoDashboard.NavigateUrl = "/admin"
        backtodashboard2.NavigateUrl = "/admin"
        'Cancel.PostBackUrl = "/property-admin"
    End Sub

    Sub friendlyurlcheck(sender As Object, args As ServerValidateEventArgs)
        REM the FriendlyURL object requires a guid.
        Dim iserror As Boolean = False
        If IsupdateItemurl = False Then
            'seriesGuid = Guid.Parse(Session("strseriesGuid"))
            Dim stritemurl As String = ""
            stritemurl = cleanstring(friendlyurl.Text)
            If Len(stritemurl) < 1 Then
                stritemurl = ""
                iserror = True
            End If

            Dim friendlyUrl__1 As New FriendlyUrl(siteSettings.SiteId, stritemurl.Replace("~/", String.Empty))
            If (Not friendlyUrl__1.FoundFriendlyUrl) Then
                Dim newUrl As New FriendlyUrl()
                newUrl.SiteId = siteSettings.SiteId
                newUrl.SiteGuid = siteSettings.SiteGuid
                'newUrl.PageGuid = seriesGuid
                newUrl.Url = stritemurl.Replace("~/", String.Empty)
                newUrl.RealUrl = "~/customfeatures.UI/propertysales/detail.aspx?pageid=" & mypageId.ToInvariantString() & "&mid=" & mymoduleId.ToInvariantString() & "&sn=" & seriesnumber
                newUrl.Save()
                iserror = False
                IsupdateItemurl = True
            Else
                REM there is a record in the db - check if is for this page
                If (friendlyUrl__1.PageGuid <> seriesGuid) Then
                    REM Guid is from another property
                    iserror = True
                ElseIf friendlyUrl__1.PageGuid = Guid.Parse("00000000-0000-0000-0000-000000000000") Then
                    Dim newUrl As New FriendlyUrl()
                    newUrl.SiteId = siteSettings.SiteId
                    newUrl.SiteGuid = siteSettings.SiteGuid
                    newUrl.PageGuid = seriesGuid
                    newUrl.Url = stritemurl.Replace("~/", String.Empty)
                    newUrl.RealUrl = "~/customfeatures.UI/propertysales/detail.aspx?pageid=" & mypageId.ToInvariantString() & "&mid=" & mymoduleId.ToInvariantString() & "&sn=" & seriesnumber
                    newUrl.Save()
                    iserror = False
                    IsupdateItemurl = True
                End If
            End If
        Else
            REM we have updated it once already so...
            iserror = False
        End If
        If iserror = True Then
            args.IsValid = False
        Else
            args.IsValid = True
        End If
    End Sub

    Public Function cleanstring(strbuf As String) As String
        strbuf = Replace(strbuf, "'", "&rsquo;")
        strbuf = Replace(strbuf, """", "&#34;")
        strbuf = Replace(strbuf, "--", "&#45;&#45;")
        strbuf = Replace(strbuf, "[", "&#91;")
        strbuf = Replace(strbuf, "%", "&#37;")
        cleanstring = strbuf
    End Function

    Public Function dirtystring(strbuf As String) As String
        strbuf = Replace(strbuf, "&rsquo;", "'")
        strbuf = Replace(strbuf, "&#34;", """")
        strbuf = Replace(strbuf, "&#45;&#45;", "--")
        strbuf = Replace(strbuf, "&#91;", "[")
        strbuf = Replace(strbuf, "&#37;", "%")
        dirtystring = strbuf
    End Function

    Private Function gopopulateform(seriesid) As Boolean
        Dim retval As Integer = -1
        Try

            Dim myreader As SqlDataReader
            Dim cn As New SqlConnection
            Dim cmd As New SqlCommand
            Dim strsql As String = ""
            Dim format As String = "ddd d MMM yyyy HH:mm "
            Dim time As DateTime = Now()
            Dim isseriespublishedbuf As Boolean = False
            cmd.Parameters.Add("@SeriesID", SqlDbType.VarChar, 255).Value = seriesid
            strsql = "Select *  FROM Series where series.seriesid = @seriesid"
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandText = strsql
            myreader = cmd.ExecuteReader
            If myreader.HasRows Then
                Do While myreader.Read
                    REM populate each field
                    seriesname.Text = myreader.Item("seriesname").ToString
                    REM - maybe adjust the counter (not working because the script isn't included) to be adapted to take off the used number of chars when loading.
                    If (friendlyurl.Text.Length = 0) AndAlso (seriesname.Text.Length > 0) Then
                        Dim friendlyUrlbuf As [String]
                        friendlyUrlbuf = SiteUtils.SuggestFriendlyUrl(seriesname.Text, siteSettings)
                        friendlyurl.Text = "~/" & friendlyUrlbuf
                    End If
                    time = myreader.Item("DateSeriesCreated")
                    dateseriescreated.Text = time.ToString(format)
                    seriesdescription.Text = myreader.Item("SeriesDescription")
                    isseriespublishedbuf = myreader.Item("seriesIsPublished")
                    If isseriespublishedbuf = True Then
                        isseriespublished.Checked = True
                    Else
                    End If
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

    Protected Sub editseriesbutton_click(sender As Object, e As EventArgs) Handles editseriesbutton.Click
        Page.Validate()
        If Page.IsValid Then
            updatedatabase()
        Else

        End If
    End Sub

    Protected Sub chooseseriesbutton_click(sender As Object, e As EventArgs) Handles chooseseriesbutton.Click
        REM get the index of the dropdown and redirect to the same page with the sn argument
        Dim seriesnumber As Integer = existingseriesdropdown.SelectedItem.Value
        Response.Redirect(addnewserieseditor & "&sn=" & seriesnumber)


    End Sub

    Sub updatedatabase()
        REM Add new record to db
        Try
            Dim st1 As String = ""
            Dim st2 As String = ""
            Dim strSQL As String = ""
            Dim cn As New System.Data.SqlClient.SqlConnection
            Dim cmd As New System.Data.SqlClient.SqlCommand
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()

            st1 = "Update series Set seriesname=@seriesname, seriesdescription=@seriesdescription,seriesurl=@seriesurl,seriesIsPublished=@seriesIsPublished"
            st2 = " where seriesid=@seriesid"

            strSQL = st1 & st2
            '                Response.Write(strSQL)
            '               Response.End()
            Dim seriesidbuf As String = ""
            seriesidbuf = seriesnumber
            If seriesidbuf < 1 Then seriesidbuf = -1

            Dim seriesnamebuf As String = ""
            seriesnamebuf = cleanstring(seriesname.Text)
            If Len(seriesnamebuf) < 1 Then seriesnamebuf = ""

            Dim seriesdescriptionbuf As String = ""
            seriesdescriptionbuf = cleanstring(seriesdescription.Text)
            If Len(seriesdescriptionbuf) < 1 Then seriesdescriptionbuf = ""

            Dim seriesurlbuf As String = ""
            seriesurlbuf = cleanstring(friendlyurl.Text)
            If Len(seriesurlbuf) < 1 Then seriesurlbuf = ""

            Dim seriesIsPublishedbuf As Boolean
            If (isseriespublished.Checked = True) Then
                seriesIsPublishedbuf = True
            Else
                seriesIsPublishedbuf = False
            End If

            cmd.Parameters.AddWithValue("@seriesid", seriesidbuf)
            cmd.Parameters.AddWithValue("@seriesname", seriesnamebuf)
            cmd.Parameters.AddWithValue("@seriesdescription", seriesdescriptionbuf)
            cmd.Parameters.AddWithValue("@seriesurl", seriesurlbuf)
            cmd.Parameters.AddWithValue("@seriesIsPublished", seriesIsPublishedbuf)

            cmd.Connection = cn
            cmd.CommandText = strSQL
            'Response.Write(GetSQL(cmd))
            Dim tmpval As String = GetSQL(cmd)
            'Response.End()
            cmd.ExecuteNonQuery()
            cn.Close()
            Response.Redirect(home)


        Catch ex As Exception
            'msg_post.Text = "There was an error updating the database: " & ex.ToString
        End Try

    End Sub

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


End Class