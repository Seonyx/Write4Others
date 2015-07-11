<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="list.aspx.vb" Inherits="Customfeatures.UI.list"   MasterPageFile="/App_MasterPages/layout.Master" %>


<asp:Content ContentPlaceHolderID="leftContent" ID="MPLeftPane" runat="server" >
</asp:Content>
<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
<portal:OuterWrapperPanel ID="pnlOuterWrap" runat="server">
<mp:CornerRounderTop id="ctop1" runat="server" EnableViewState="false"  />
<portal:InnerWrapperPanel ID="pnlInnerWrap" runat="server" CssClass="panelwrapper ">
<portal:HeadingControl ID="heading" runat="server" />
<portal:OuterBodyPanel ID="pnlOuterBody" runat="server">
<portal:InnerBodyPanel ID="pnlInnerBody" runat="server" CssClass="modulecontent">
    <div class="art-post">
     <h2>Dashboard</h2>
        <p>&nbsp;</p>
                  <asp:HyperLink ID="newseriesbtn" CssClass="art-button" runat="server">Add new series</asp:HyperLink>
             &nbsp;
                  <asp:HyperLink ID="editseriesbtn" CssClass="art-button" runat="server">Edit series details</asp:HyperLink>
             &nbsp;
                  <asp:HyperLink ID="newchapter"  CssClass="art-button" runat="server">New chapter</asp:HyperLink>
            &nbsp;
                  <asp:HyperLink ID="editchapter"  CssClass="art-button" runat="server">Edit existing chapter</asp:HyperLink>
                 &nbsp;

                  
        </div>

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



