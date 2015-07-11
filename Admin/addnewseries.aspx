<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="addnewseries.aspx.vb" Inherits="Customfeatures.UI.addnewseries" MasterPageFile="/App_MasterPages/layout.Master"%>

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
     
<h2>Add New Series</h2>
 <p>&nbsp;</p>
        <div class="leftalign">
            <span class="form_label">List of Existing Series</span>
            <asp:DropDownList DataTextField="poblacion" DataValueField="existingseries" ID="existingseriesdropdown" runat="server" CssClass="dropdownEffect"></asp:DropDownList>
            <asp:Label ID="msg_existingseriesdropdown" runat="server" CssClass="redden"></asp:Label>
            <p>&nbsp;</p>
            <asp:label id="newserieslabel" runat="server" CssClass="form_label">New Series Name:<span class="redden">*</span></asp:label>
            <br />
            <asp:TextBox ID="addnewseriesname" Width="330px" ToolTip="Series name:(255 characters)" MaxLength="255"  runat="server" CssClass="ui-widget ui-state-default ui-corner-all ui-textinput"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator" runat="server" ControlToValidate="addnewseriesname" Display="Dynamic" ErrorMessage="Series name needed: 255 characters maximum"  setfocusonerror="True" CssClass="regformlabelerror"></asp:RequiredFieldValidator>
            [Items marked <span class="redden">*</span> are required]
            <p>&nbsp;</p>
            <asp:Button ID="submitnewnseriesname" CssClass="art-button" runat="server" Text="Add"  OnClick="submitnewnseriesname_Click" 
    OnClientClick="submitnewnseriesname_ClientClick()" />
          <asp:Button ID="Cancel" runat="server" CssClass="art-button" Text="Cancel/Clear" UseSubmitBehavior="False" CausesValidation="False" OnClientClick="cancelFunction(); return false;" />
                    
        <asp:HyperLink ID="BacktoDashboard"  CssClass="art-button" runat="server">Back to Admin Dashboard</asp:HyperLink>          
        </div>



</div>

<script type="text/javascript">
    /*<![CDATA[*/


    function cancelFunction() {
        $('#<%= addnewseriesname.ClientID %>').val("");
    }


    $(document).ready(function () {
        $('#<%= addnewseriesname.ClientID%>').counter({
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