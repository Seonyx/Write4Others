<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="addnewserieseditor.aspx.vb" Inherits="Customfeatures.UI.addnewserieseditor"  MasterPageFile="/App_MasterPages/layout.Master"%>
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
   <h3>Series Editor</h3>
     <p>&nbsp;</p>
    <!-- xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx -->
    <div class="leftalign" style="width: 50%">
    <asp:Label ID="Label2" runat="server" CssClass="form_label">Series name:</asp:Label>
    <span class="ui-widget redden">*</span><br />
    <!-- Need to populate the editor seriesname if there is an incoming field or present dropdown if there is none. Must not fail if list is empty -->
    <asp:TextBox ID="seriesname" Width="330px" ToolTip="Summary:(255 characters)" MaxLength="255"  runat="server" CssClass="ui-widget"></asp:TextBox>
        <asp:RequiredFieldValidator ID="SeriesnameRequiredFieldValidator" runat="server" ControlToValidate="seriesname" Display="Dynamic" ErrorMessage="Name required: 255 character max"  setfocusonerror="True" CssClass="ui-widget redden"></asp:RequiredFieldValidator>
    </div>
    <div class="leftalign form_label" style="width: 20%">Date created:<br /> <asp:Label ID="dateseriescreated" runat="server" CssClass="unchangabletext"></asp:Label></div>
     <p>&nbsp;</p>
    <!-- xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx -->
    <div class="clearfix"></div>
    <div class="leftalign">
    <p>This is the &rsquo;friendly&rsquo; url the system has suggested for your page. Please feel free to change it if you wish.</p>
    <asp:label id="friendlyurllabel" runat="server" CssClass="form_label">Friendly URL:</asp:label><br />
    <asp:label id="friendlyurllabellblError" runat="server" CssClass="ui-widget"></asp:label>
    <asp:TextBox ID="friendlyurl"  CausesValidation="true" runat="server" MaxLength="255" CssClass="forminput verywidetextbox"></asp:TextBox>
    <span id="spnUrlWarning" runat="server" style="font-weight: normal; display:none;" class="txterror"></span>
    <asp:CustomValidator runat="server" id="checkurl" ControlToValidate="friendlyurl" OnServerValidate="friendlyurlcheck"  ErrorMessage="This alias is already in use." />
    </div>
    <!-- xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx -->
    <div class="clearfix"></div>
    <div class="leftalign">
    <p>Description or summary of the series</p>
    <mpe:EditorControl ID="seriesdescription" runat="server"></mpe:EditorControl>
    <p>&nbsp;</p>
        </div>
    <!-- xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx -->
      <asp:HyperLink ID="BacktoDashboard"  CssClass="art-button" runat="server">Back to Admin Dashboard</asp:HyperLink>          
</div>

    <script type="text/javascript">
        /*<![CDATA[*/
        $(document).ready(function () {
         $('#<%= seriesname.ClientID%>').counter({
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