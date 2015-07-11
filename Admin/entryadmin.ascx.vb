Imports System
Imports System.Globalization
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.IO
Imports mojoPortal.Business
Imports mojoPortal.Business.WebHelpers
Imports mojoPortal.Features.UI
Imports mojoPortal.Web.Framework
Imports mojoPortal.Web.Controls
Imports mojoPortal.Web
Imports mojoPortal.Web.UI
Imports mojoPortal.Web.Editor
Imports mojoPortal.Net

Public Class entryadmin
    Inherits mojoPortal.Web.SiteModuleControl

    Dim mymoduleId As Integer
    Dim mypageId As Integer
    Private siteurl = ConfigurationManager.AppSettings("siteurl")
    Private Sitefolder = ConfigurationManager.AppSettings("Sitefolder")
    Private modulepathadmin = ConfigurationManager.AppSettings("modulepathadmin")

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadParams()
        Response.Redirect(siteurl & Sitefolder & modulepathadmin & "/list.aspx?pageid=" & CStr(mypageId) & "&mid=" & CStr(mymoduleId))
    End Sub

    Sub LoadParams()

        Dim currentPage As PageSettings = CacheHelper.GetCurrentPage()
        mypageId = currentPage.PageId
        mymoduleId = ModuleId
    End Sub

End Class