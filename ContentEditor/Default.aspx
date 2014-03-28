<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ContentEditor._Default" ValidateRequest="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Content Editor</title>
    <style type="text/css">
        .style1
        {
            font-size: large;
        }
    </style>
</head>
<body bgcolor="#666666">
    <form id="form1" runat="server">
    <div>
    
        <br />
        <br />
    
        <span class="style1">Select Page to modify</span><br />
        <asp:DropDownList ID="DropDownListPages" runat="server" Height="21px" 
            onselectedindexchanged="DropDownListPages_SelectedIndexChanged" 
            Width="284px" AutoPostBack="True">
        </asp:DropDownList>
    
        <br />
    
    </div>
    <div>
    
        <span class="style1">Select Section to modify</span><br />
    
    </div>
        <asp:DropDownList ID="DropDownListSections" runat="server" Height="21px" 
            Width="284px" AutoPostBack="True" 
        onselectedindexchanged="DropDownListSections_SelectedIndexChanged">
        </asp:DropDownList>
    
    
         <br /><span class="style1">
    <br />
    Select Revision to use</span><p>
        <asp:DropDownList ID="DropDownListRevisions" runat="server" AutoPostBack="True" 
            onselectedindexchanged="DropDownListRevisions_SelectedIndexChanged" 
            Width="171px">
        </asp:DropDownList>
    
    </p>
    
    <p class="style1">
        Update Content Below. Preview or/and save changes when done
        </p>
    <p class="style1">
        <asp:Button ID="ButtonPreview" runat="server" onclick="ButtonPreview_Click" Text="Preview Changes" />
    </p>
    <asp:TextBox ID="TextBoxEdit" runat="server" Height="406px" Width="723px" 
        TextMode="MultiLine"></asp:TextBox>
    
    <br />
    <asp:Button ID="ButtonSave" runat="server" Enabled="False" 
        onclick="ButtonSave_Click" Text="Save Changes" BackColor="#FF3300" />
    
    <p>
        <asp:CheckBox ID="CheckBoxDontTweet" runat="server" 
            Text="DO NOT Tweet changes" />
    </p>
    
    </form>
</body>
</html>
