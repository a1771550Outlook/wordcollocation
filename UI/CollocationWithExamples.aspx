<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CollocationWithExamples.aspx.cs" Inherits="UI.CollocationWithExamples" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Collocation List</title>
	<link href="css/site.css" rel="stylesheet" />
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<h1>Collocations and Their Examples</h1>
			<p>
				<asp:GridView ID="CollocationList" runat="server" AutoGenerateColumns="False" CssClass="DataWebControlStyle">
					<HeaderStyle CssClass="HeaderStyle" />
					<AlternatingRowStyle CssClass="AlternatingRowStyle" />
					<Columns>
						<asp:BoundField DataField="CollocationId" HeaderText="CollocationId"/>
						<asp:BoundField DataField="Pos" HeaderText="POS" />
						<asp:BoundField DataField="ColPos" HeaderText="ColPOS" />
						<asp:BoundField DataField="Word" HeaderText="Word" />
						<asp:BoundField DataField="ColWord" HeaderText="ColWord" />
						<asp:TemplateField HeaderText="Pattern">
							<ItemTemplate>
								<asp:Label runat="server" ID="lblPattern" Text='<%#Eval("PatternTextString") %>'></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Examples">
							<ItemTemplate>
								<asp:BulletedList ID="BulletedList1" runat="server" DataSource='<%# Eval("WcExampleList") %>'
									DataTextField="Entry">
								</asp:BulletedList>
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
				&nbsp;
			</p>

			<asp:Label runat="server" ID="lblShow"></asp:Label>
		</div>
	</form>
</body>

</html>
