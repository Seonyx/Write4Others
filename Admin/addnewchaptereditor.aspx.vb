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

Public Class addnewchaptereditor

    Inherits mojoBasePage

    Dim mypageId As Integer = -1
    Dim mymoduleId As Integer = -1
    Dim home As String = ""
    Dim chapternumber As Integer = -1
    Dim seriesnumber As Integer = -1
    Dim addnewserieseditor As String
    Dim errorstring As String = ""
    Private IsupdateItemurl As Boolean = False
    Private siteurl = ConfigurationManager.AppSettings("siteurl")
    Private Sitefolder = ConfigurationManager.AppSettings("Sitefolder")
    Private modulepathadmin = ConfigurationManager.AppSettings("modulepathadmin")
    Private seriesGuid As Guid = Guid.Empty
    Public test As String = ""
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
        chaptertext.WebEditor.ToolBar = ToolBar.FullWithTemplates
        REM fuck this - we have to have a chapter number incoming on the qrystring otherwise we redirect
        REM when we get the chapter number, lookup the series number
        mypageId = WebUtils.ParseInt32FromQueryString("pageid", mypageId)
        mymoduleId = WebUtils.ParseInt32FromQueryString("mid", mymoduleId)
        home = SiteRoot & "/default.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        'addnewserieseditor = SiteRoot & modulepathadmin & "/addnewserieseditor.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        If Not UserCanViewPage(mymoduleId, FeatureGuidGuid) Then
            If Not Request.IsAuthenticated Then
                SiteUtils.RedirectToLoginPage(Me)
            Else
                SiteUtils.RedirectToAccessDeniedPage(Me)
            End If
            Return
        End If
        chapternumber = WebUtils.ParseInt32FromQueryString("cn", chapternumber)
        If chapternumber = -1 Then chapternumber = 56

        If chapternumber <> -1 Then
            REM there is an incoming chapter number so lookup series id of parent go populate the form.
            seriesnumber = getseriesnumber(chapternumber)
            'listlabel.Visible = False
            'existingseriesdropdown.Visible = False
            'msg_existingseriesdropdown.Visible = False
            '    Panel1.Visible = False
            If Not IsPostBack Then
                gopopulateform(chapternumber)
            End If

        Else
            REM  No series number so blank the form and present dropdown, checking first if there is data and redirecting if not.
            '   Panel2.Visible = False
            '  bind_existingseriesdropdown()
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
                newUrl.RealUrl = "~/customfeatures.UI/propertysales/detail.aspx?pageid=" & mypageId.ToInvariantString() & "&mid=" & mymoduleId.ToInvariantString() & "&cn=" & chapternumber
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
                    newUrl.RealUrl = "~/customfeatures.UI/propertysales/detail.aspx?pageid=" & mypageId.ToInvariantString() & "&mid=" & mymoduleId.ToInvariantString() & "&cn=" & chapternumber
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

    Private Function gopopulateform(chapterid) As Boolean
        Dim retval As Integer = -1
        Try

            Dim myreader As SqlDataReader
            Dim cn As New SqlConnection
            Dim cmd As New SqlCommand
            Dim strsql As String = ""
            Dim format As String = "ddd d MMM yyyy HH:mm "
            Dim time As DateTime = Now()
            Dim ischapterpublishedbuf As Boolean = False
            'cmd.Parameters.Add("@chapterID", SqlDbType.Int).Value = chapterid
            cmd.Parameters.Add("@chapterID", SqlDbType.Int)
            cmd.Parameters("@chapterID").Value = chapterid
            strsql = "Select *  FROM SeriesChapters where seriesChapters.chapterid = @chapterid"
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandText = strsql
            errorstring = GetSQL(cmd)
            myreader = cmd.ExecuteReader
            If myreader.HasRows Then
                Do While myreader.Read
                    REM populate each field
                    Dim chapternameindex As Integer = myreader.GetOrdinal("chaptername")
                    If myreader.IsDBNull(chapternameindex) Then
                        chaptername.Text = String.Empty
                    Else
                        chaptername.Text = myreader.Item("chaptername").ToString
                    End If
                    'chaptername.Text = myreader.Item("chaptername").ToString
                    REM - maybe adjust the counter (not working because the script isn't included) to be adapted to take off the used number of chars when loading.
                    If (friendlyurl.Text.Length = 0) AndAlso (chaptername.Text.Length > 0) Then
                        Dim friendlyUrlbuf As [String]
                        friendlyUrlbuf = SiteUtils.SuggestFriendlyUrl(chaptername.Text, siteSettings)
                        friendlyurl.Text = "~/" & friendlyUrlbuf
                    End If
                    time = myreader.Item("DateChapterCreated")
                    datechaptercreated.Text = time.ToString(format)
                    Dim chaptertextindex As Integer = myreader.GetOrdinal("chaptertext")
                    If myreader.IsDBNull(chaptertextindex) Then
                        'chaptertext.Text = String.Empty
                    Else
                        chaptertext.Text = myreader.Item("chaptertext").ToString
                    End If
                    'chaptertext.Text = myreader.Item("chaptertext")
                    ischapterpublishedbuf = myreader.Item("chapterIsPublished")
                    If ischapterpublishedbuf = True Then
                        ischapterpublished.Checked = True
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

    Protected Sub chooseseriesbutton_click(sender As Object, e As EventArgs) Handles chooseseriesbutton.Click
        '    REM get the index of the dropdown and redirect to the same page with the sn argument
        '    Dim seriesnumber As Integer = existingseriesdropdown.SelectedItem.Value
        '    Response.Redirect(addnewserieseditor & "&sn=" & seriesnumber)
    End Sub

    Sub updatedatabase()
        REM Add new record to db
        REM this is complete shyte. Need to update therecord by chapterid not seriesid
        Try
            Dim st1 As String = ""
            Dim st2 As String = ""
            Dim strSQL As String = ""
            Dim cn As New System.Data.SqlClient.SqlConnection
            Dim cmd As New System.Data.SqlClient.SqlCommand
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()

            st1 = "Update serieschapters Set chaptername=@chaptername, chaptertext=@chaptertext,chapterurl=@chapterurl,chapterIsPublished=@chapterIsPublished"
            st2 = " where serieschapters.chapterid=@chapterid"

            strSQL = st1 & st2
            '                Response.Write(strSQL)
            '               Response.End()
            Dim chapteridbuf As String = ""
            chapteridbuf = chapternumber
            If chapteridbuf < 1 Then chapteridbuf = -1

            Dim chapternamebuf As String = ""
            chapternamebuf = cleanstring(chaptername.Text)
            If Len(chapternamebuf) < 1 Then chapternamebuf = ""

            Dim chaptertextbuf As String = ""
            chaptertextbuf = cleanstring(chaptertext.Text)
            If Len(chaptertextbuf) < 1 Then chaptertextbuf = ""

            Dim chapterurlbuf As String = ""
            chapterurlbuf = cleanstring(friendlyurl.Text)
            If Len(chapterurlbuf) < 1 Then chapterurlbuf = ""

            Dim chapterIsPublishedbuf As Boolean
            If (ischapterpublished.Checked = True) Then
                chapterIsPublishedbuf = 1
            Else
                chapterIsPublishedbuf = 0
            End If

            cmd.Parameters.AddWithValue("@chapterid", chapteridbuf)
            cmd.Parameters.AddWithValue("@seriesid", seriesnumber)
            cmd.Parameters.AddWithValue("@chaptername", chapternamebuf)
            cmd.Parameters.AddWithValue("@chaptertext", chaptertextbuf)
            cmd.Parameters.AddWithValue("@chapterurl", chapterurlbuf)
            cmd.Parameters.AddWithValue("@chapterIsPublished", chapterIsPublishedbuf)

            cmd.Connection = cn
            cmd.CommandText = strSQL
            Dim tmpval As String = GetSQL(cmd)
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

    Protected Sub editchapterbutton_Click(sender As Object, e As EventArgs) Handles editchapterbutton.Click
        If IsPostBack Then
            updatedatabase()
        End If
    End Sub

    Function getseriesnumber(chapternumber) As Integer
        Dim retval As Integer = -1
        Try

            Dim myreader As SqlDataReader
            Dim cn As New SqlConnection
            Dim cmd As New SqlCommand
            Dim strsql As String = ""

            cmd.Parameters.Add("@chapterID", SqlDbType.Int)
            cmd.Parameters("@chapterID").Value = chapternumber
            strsql = "Select ParentSeriesId  FROM SeriesChapters where seriesChapters.chapterid = @chapterid"
            cn.ConnectionString = ConfigurationManager.AppSettings("MSSQLConnectionString")
            cn.Open()
            cmd.Connection = cn
            cmd.CommandText = strsql
            errorstring = GetSQL(cmd)
            myreader = cmd.ExecuteReader
            If myreader.HasRows Then
                Do While myreader.Read
                    REM populate each field
                    Dim ParentSeriesIdindex As Integer = myreader.GetOrdinal("ParentSeriesId")
                    If myreader.IsDBNull(ParentSeriesIdindex) Then
                        retval = -1
                    Else
                        retval = myreader.Item("ParentSeriesId").ToString
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

End Class