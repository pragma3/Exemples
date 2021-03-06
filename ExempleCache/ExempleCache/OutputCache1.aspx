﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutputCache1.aspx.cs" Inherits="ExempleCache.OutputCache1" %>
<%@ OutputCache Duration="25" VaryByParam="id" Location="Server" %>
<!-- Cet exemple montre que la cache retourne des informations différente en fonction de la valeur du paramètre id -->
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Label ID="lblTexte" runat="server" />
    </div>
    <div>
    <a href="OutputCache1?id=1">Id = 1</a><br />
    <a href="OutputCache1?id=2">Id = 2</a><br />
    <a href="OutputCache1?id=3">Id = 3</a><br />
    </div>
    <div>
    Heure génération page : <asp:Label ID="lblHeure" runat="server" /><br />
Heure réelle : <input type="text" id="heure" />
        <script>

            var currentdate = new Date();
            var datetime =  "" + currentdate.getHours() + ":"
                            + currentdate.getMinutes() + ":"
                            + currentdate.getSeconds();
            document.getElementById("heure").value = datetime;
        </script>
    </div>
    </form>
</body>
</html>
