<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.ProductTagPage" CodeBehind="ProductTag.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductsByTag" Src="~/Modules/ProductsByTag.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ProductsByTag ID="ctrlProductsByTag" runat="server" />
</asp:Content>
