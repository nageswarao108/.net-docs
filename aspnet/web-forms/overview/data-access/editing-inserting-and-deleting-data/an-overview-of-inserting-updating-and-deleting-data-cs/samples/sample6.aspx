<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
    DataKeyNames="ProductID" DataSourceID="ObjectDataSource1">
    <Columns>
        <asp:CommandField ShowDeleteButton="True"
            ShowEditButton="True" />
        ... BoundFields removed for brevity ...
    </Columns>
</asp:GridView>