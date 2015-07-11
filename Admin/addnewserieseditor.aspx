<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="addnewserieseditor.aspx.vb" Inherits="Customfeatures.UI.addnewserieseditor"  MasterPageFile="/App_MasterPages/layout.Master"%>
<asp:Content ContentPlaceHolderID="leftContent" ID="MPLeftPane" runat="server" >
</asp:Content>
<asp:Content ContentPlaceHolderID="mainContent" ID="MPContent" runat="server">
<portal:outerwrapperpanel ID="pnlOuterWrap" runat="server">
<mp:CornerRounderTop id="ctop1" runat="server" EnableViewState="false"  />
<portal:InnerWrapperPanel ID="pnlInnerWrap" runat="server" CssClass="panelwrapper ">
<portal:HeadingControl ID="heading" runat="server" />
<portal:OuterBodyPanel ID="pnlOuterBody" runat="server">
<portal:InnerBodyPanel ID="pnlInnerBody" runat="server" CssClass="modulecontent">
<div class="art-post">
   <h3>Series Editor</h3>
     <p>&nbsp;</p>
    <asp:Panel ID="Panel1" runat="server">
        
     <span id="listlabel" class="form_label">Choose Series To Edit</span>
     <asp:DropDownList DataTextField="poblacion" DataValueField="existingseries" ID="existingseriesdropdown" runat="server" CssClass="dropdownEffect"></asp:DropDownList>
    <asp:Label ID="msg_existingseriesdropdown" runat="server" CssClass="redden"></asp:Label>
        <p>&nbsp;</p>
<asp:Button ID="chooseseriesbutton" CssClass="art-button" runat="server" Text="Continue"  OnClick="chooseseriesbutton_Click" 
    OnClientClick="chooseseries_ClientClick()" /><asp:HyperLink ID="backtodashboard2"  CssClass="art-button" runat="server">Back to Admin Dashboard</asp:HyperLink>          
     </asp:Panel>
    <!-- xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx -->
    <asp:Panel ID="Panel2" runat="server">
    <div class="leftalign" style="width: 50%">
    <asp:Label ID="Label2" runat="server" CssClass="form_label">Series name:</asp:Label>
    <span class="ui-widget redden">*</span><br />
    <!-- Need to populate the editor seriesname if there is an incoming field or present dropdown if there is none. Must not fail if list is empty -->
    <asp:TextBox ID="seriesname" Width="330px" ToolTip="Summary:(255 characters)" MaxLength="255"  runat="server" CssClass="ui-widget"></asp:TextBox>
        <asp:RequiredFieldValidator ID="SeriesnameRequiredFieldValidator" runat="server" ControlToValidate="seriesname" Display="Dynamic" ErrorMessage="Name required: 255 character max"  setfocusonerror="True" CssClass="ui-widget redden"></asp:RequiredFieldValidator>
    </div>
    <div class="leftalign form_label" style="width: 20%">Date created:<br /> <asp:Label ID="dateseriescreated" runat="server" CssClass="unchangabletext"></asp:Label></div>
    <div class="leftalign form_label" style="width: 20%">Is published?:<br /> <asp:Label ID="ispublishedlabel" runat="server" CssClass=""></asp:Label><asp:CheckBox ID="isseriespublished" CssClass="zeromargin" runat="server" /></div>
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
    <p>&nbsp;</p>
    <!-- xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx -->
    <div class="clearfix"></div>
    <div class="leftalign">
    <p><span class="form_label">Description or summary of the series</span></p>
    <mpe:EditorControl ID="seriesdescription" runat="server"></mpe:EditorControl>
                <p>&nbsp;</p>
            <asp:Button ID="editseriesbutton" CssClass="art-button" runat="server" Text="Update"  causesvalidation="true"  OnClick="editseriesbutton_Click" 
    OnClientClick="editseries_ClientClick()" /><asp:HyperLink ID="BacktoDashboard"  CssClass="art-button" runat="server">Back to Admin Dashboard</asp:HyperLink>          
        </div>
    </asp:Panel>
    <!-- xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx -->
      
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
</portal:outerwrapperpanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="rightContent" ID="MPRightPane" runat="server" >
    
    
    
</asp:Content>
<asp:Content ContentPlaceHolderID="pageEditContent" ID="MPPageEdit" runat="server" />