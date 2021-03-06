﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="addnewchapter.aspx.vb" Inherits="Customfeatures.UI.addnewchapter"  MasterPageFile="/App_MasterPages/layout.Master"%>
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
     
<h2>Add New chapter</h2>
 <p>&nbsp;</p>
<asp:Panel ID="Panel1" runat="server">
    <span id="listlabel" class="form_label">Choose chapter To Edit</span>
    <asp:DropDownList DataTextField="poblacion" DataValueField="existingchapter" ID="DropDownList1" runat="server" CssClass="dropdownEffect"></asp:DropDownList>
    <asp:Label ID="Label1" runat="server" CssClass="redden"></asp:Label>
    <p>&nbsp;</p>
<asp:Button ID="choosechapterbutton" CssClass="art-button" runat="server" Text="Continue"  OnClick="choosechapterbutton_Click" 
    OnClientClick="choosechapter_ClientClick()" /><asp:HyperLink ID="backtodashboard2"  CssClass="art-button" runat="server">Back to Admin Dashboard</asp:HyperLink>          
</asp:Panel>
<asp:Panel ID="Panel2" runat="server">
        <div class="leftalign">
            <span class="form_label">List of Existing chapters</span>
            <asp:DropDownList DataTextField="poblacion" DataValueField="existingchapter" ID="existingchapterdropdown" runat="server" CssClass="dropdownEffect"></asp:DropDownList>
            <asp:Label ID="msg_existingchapterdropdown" runat="server" CssClass="redden"></asp:Label>
            <p>&nbsp;</p>
            <asp:label id="newchapterlabel" runat="server" CssClass="form_label">New chapter Name:<span class="redden">*</span></asp:label>
            <br />
            <asp:TextBox ID="addnewchaptername" Width="330px" ToolTip="chapter name:(255 characters)" MaxLength="255"  runat="server" CssClass="ui-widget ui-state-default ui-corner-all ui-textinput"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ControlToValidate="addnewchaptername" Display="Dynamic" ErrorMessage="chapter name needed: 255 characters maximum"  setfocusonerror="True" CssClass="regformlabelerror"></asp:RequiredFieldValError>
            [Items marked <span class="redden">*</span> are required]
            <p>&nbsp;</p>
            <asp:Button ID="submitnewnchaptername" CssClass="art-button" runat="server" Text="Add"  OnClick="submitnewnchaptername_Click" 
    OnClientClick="submitnewnchaptername_ClientClick()" />
          <asp:Button ID="Cancel" runat="server" CssClass="art-button" Text="Cancel/Clear" UseSubmitBehavior="False" CausesValidation="False" OnClientClick="cancelFunction(); return false;" />
                    
        <asp:HyperLink ID="BacktoDashboard"  CssClass="art-button" runat="server">Back to Admin Dashboard</asp:HyperLink>          
        </div>


    </asp:Panel>
</div>REM Now we know which series this chapter is to be added to we can display the form to assign a chapter name

<script type="text/javascript">
    /*<![CDATA[*/


    function cancelFunction() {
        $('#<%= addnewchaptername.ClientID %>').val("");
    }


    $(document).ready(function () {
        $('#<%= addnewchaptername.ClientID%>').counter({
            goal: 255
        });
    });
				    /*]]>*/
</script>
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