<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="addnewserieserror.aspx.vb" Inherits="Customfeatures.UI.addnewserieserror" MasterPageFile="/App_MasterPages/layout.Master"%>

<asp:Content ContentPlaceHolderID="leftContent" ID="MPLeftPane" runat="server" >
</asp:Content>
<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
<portal:OuterWrapperPanel ID="pnlOuterWrap" runat="server">
<mp:CornerRounderTop id="ctop1" runat="server" EnableViewState="false"  />
<portal:InnerWrapperPanel ID="pnlInnerWrap" runat="server" CssClass="panelwrapper ">
<portal:HeadingControl ID="heading" runat="server" />
<portal:OuterBodyPanel ID="pnlOuterBody" runat="server">
<portal:InnerBodyPanel ID="pnlInnerBody" runat="server" CssClass="modulecontent">

   <h3 class="redden">Add New Series Error</h3>
    <p>&nbsp;</p>
    An error occured while trying to add a new series. Please click the link to try again. Make sure the series name is unique (i.e. not already in the dropdown list of series names).
    If this error occurs again please contact the System administrator at <a href="mailto:info@seonyx.com">info@seonyx.com</a>
    <p>&nbsp;</p>  
      <asp:HyperLink ID="BacktoDashboard"  CssClass="art-button" runat="server">Back to Admin Dashboard</asp:HyperLink>          

</portal:InnerBodyPanel>
</portal:OuterBodyPanel>
<portal:EmptyPanel id="divCleared" runat="server" CssClass="cleared" SkinID="cleared"></portal:EmptyPanel>
</portal:InnerWrapperPanel> 
<mp:CornerRounderBottom id="cbottom1" runat="server" EnableViewState="false" />	
</portal:OuterWrapperPanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="rightContent" ID="MPRightPane" runat="server" >
</asp:Content>
<asp:Content ContentPlaceHolderID="pageEditContent" ID="MPPageEdit" runat="server" />