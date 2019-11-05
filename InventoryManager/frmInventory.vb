﻿Imports System.Data.SqlClient

Public Class frmInventory
    Private outstock, instocks, critstock As String
    Private selectedRow As Integer

    Private Sub frmInventory_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'JandADataSet3.tblInventory' table. You can move, or remove it, as needed.
        Call viewItemList_reload()
    End Sub

    Private Sub btnAddItem_Click(sender As Object, e As EventArgs) Handles btnAddItem.Click
        saveType1 = 1
        Dim ab As New frmItemManager
        ab.Show()
        frmMenu.Enabled = False
    End Sub

    Private Sub btnUpdateItem_Click(sender As Object, e As EventArgs) Handles btnUpdateItem.Click
        If (selectedRow > 0) Then
            saveType1 = 2
            frmItemManager.txtItemName.Text = dgvItemList.Rows(selectedRow).Cells(1).Value()
            frmItemManager.txtItemQuantity.Text = dgvItemList.Rows(selectedRow).Cells(2).Value()
            frmItemManager.txtDescription.Text = dgvItemList.Rows(selectedRow).Cells(3).Value()
            If (dgvItemList.Rows(selectedRow).Cells(4).Value() = "HAIR") Then
                frmItemManager.rdoHair.Checked = True
            ElseIf (dgvItemList.Rows(selectedRow).Cells(4).Value() = "BODY") Then
                frmItemManager.rdoBody.Checked = True
            ElseIf (dgvItemList.Rows(selectedRow).Cells(4).Value() = "NAILS") Then
                frmItemManager.rdoNails.Checked = True
            End If
            itemID = dgvItemList.Rows(selectedRow).Cells(0).Value()
            Dim ab As New frmItemManager
            ab.Show()
            frmMenu.Enabled = False
        End If
    End Sub
    Private Sub dgvUserList_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvItemList.CellClick
        If (dgvItemList.Rows.Count > 0) Then
            Try
                selectedRow = e.RowIndex
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub frmInventory_EnabledChanged(sender As Object, e As EventArgs) Handles MyBase.EnabledChanged
        Call viewItemList_reload()
    End Sub

    Private Sub viewItemList_reload()
        Call ConnectTOSQLServer()
        strSQL = "select * from vw_InventoryView"
        Console.WriteLine(strSQL)
        dataadapter = New SqlDataAdapter(strSQL, Connection)
        Dim ItemList As New DataSet()

        dataadapter.Fill(ItemList, "vw_InventoryView")
        dgvItemList.DataSource = ItemList
        dgvItemList.DataMember = "vw_InventoryView"

        strSQL = "select COUNT(itemID) from vw_InventoryView where Quantity = '0'"
        Console.WriteLine()
        cmd = New SqlCommand(strSQL, Connection)
        reader = cmd.ExecuteReader()
        Do While reader.HasRows
            Do While reader.Read()
                outstock = reader.GetInt32(0)
            Loop
            reader.NextResult()
        Loop
        reader.Close()

        strSQL = "select COUNT(itemID) from vw_InventoryView where Quantity <> '0'"
        Console.WriteLine()
        cmd = New SqlCommand(strSQL, Connection)
        reader = cmd.ExecuteReader()
        Do While reader.HasRows
            Do While reader.Read()
                instocks = reader.GetInt32(0)
            Loop
            reader.NextResult()
        Loop
        reader.Close()

        lblActiveCount.Text = instocks
        lblCritCount.Text = outstock

        strSQL = "select count(ItemID) as CritCount from tblInventory where Quantity <= [Critical Point]"
        Console.WriteLine()
        cmd = New SqlCommand(strSQL, Connection)
        reader = cmd.ExecuteReader()
        Do While reader.HasRows
            Do While reader.Read()
                critstock = reader.GetInt32(0)
            Loop
            reader.NextResult()
        Loop
        reader.Close()

        lblActiveCount.Text = instocks
        lblInactiveCount.Text = outstock
        lblCritCount.Text = critstock
        Call DisConnectSQLServer()
    End Sub
End Class