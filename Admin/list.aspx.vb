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



Public Class list
    Inherits mojoBasePage

    Dim mypageId As Integer = -1
    Dim mymoduleId As Integer = -1
    Dim home As String = ""

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
    End Sub

    Sub LoadParams()
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
        newseriesbtn.NavigateUrl = SiteRoot & modulepathadmin & "/addnewseries.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        editseriesbtn.NavigateUrl = SiteRoot & modulepathadmin & "/addnewserieseditor.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        newchapter.NavigateUrl = SiteRoot & modulepathadmin & "/addnewchapter.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        editchapter.NavigateUrl = SiteRoot & modulepathadmin & "/addnewchaptereditor.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId)
        'HyperLink1.NavigateUrl = "/property-admin"
        'Cancel.PostBackUrl = "/property-admin"
    End Sub
End Class