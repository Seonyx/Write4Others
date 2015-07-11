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
            gopopulateform(seriesnumber)
        Else
            REM  No series number so blank the form and present dropdown, checking first if there is data and redirecting if not.

        End If
        BacktoDashboard.NavigateUrl = "/admin"
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
            cmd.Parameters.Add("@SeriesID", SqlDbType.VarChar, 255).Value = seriesid
            strsql = "SELECT *  FROM Series where series.seriesid = @seriesid"
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
                        time = myreader.Item("DateSeriesCreated")
                        dateseriescreated.Text = time.ToString(format)
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