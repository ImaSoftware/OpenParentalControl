Imports System.IO
Imports System.Xml.Linq
Imports System.IO.Pipes
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Partial Public Class ConfigForm
    Inherits Form

    Private originalAdminHash As String = String.Empty

    Public Sub New()
        Me.Text = "Configurador - OpenParentalControl"
        Me.ClientSize = New Drawing.Size(720, 460)
        InitializeComponent()

        ' Enlazar eventos (el diseñador mantiene la definición de controles)
        AddHandler btnBrowse.Click, AddressOf BtnBrowse_Click
        AddHandler btnLoad.Click, AddressOf BtnLoad_Click
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnAddProcess.Click, AddressOf BtnAddProcess_Click
        AddHandler btnRemoveProcess.Click, AddressOf BtnRemoveProcess_Click
        AddHandler btnClose.Click, AddressOf BtnClose_Click
        AddHandler btnTestConnection.Click, AddressOf BtnTestConnection_Click
        AddHandler btnReloadService.Click, AddressOf BtnReloadService_Click

        ' Valor por defecto: intentar localizar settings.xml junto al servicio si existe
        Dim candidate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml")
        If File.Exists(candidate) Then
            txtSettingsPath.Text = candidate
        End If
    End Sub

    ' --- Event handlers y lógica (sin cambios funcionales) ---
    Private Sub BtnBrowse_Click(sender As Object, e As EventArgs)
        Using ofd As New OpenFileDialog()
            ofd.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
            If ofd.ShowDialog() = DialogResult.OK Then
                txtSettingsPath.Text = ofd.FileName
            End If
        End Using
    End Sub

    Private Sub BtnLoad_Click(sender As Object, e As EventArgs)
        Dim path = txtSettingsPath.Text.Trim()
        If Not File.Exists(path) Then
            MessageBox.Show("Fichero no encontrado: " & path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        Try
            Dim doc = XDocument.Load(path)
            Dim root = doc.Root

            ' Intervalo
            Dim ks = root.Element("KillIntervalSeconds")
            If ks IsNot Nothing Then txtInterval.Text = ks.Value

            ' Forbidden
            lbForbidden.Items.Clear()
            Dim forbiddenNode = root.Element("Forbidden")
            If forbiddenNode IsNot Nothing Then
                For Each p In forbiddenNode.Elements("Process")
                    lbForbidden.Items.Add(p.Value)
                Next
            End If

            ' Restrictions
            dgvRestrictions.Rows.Clear()
            Dim restrictionsNode = root.Element("Restrictions")
            If restrictionsNode IsNot Nothing Then
                For Each r As XElement In restrictionsNode.Elements("Restriction")
                    Dim s = If(r.Element("Start") Is Nothing, "", r.Element("Start").Value)
                    Dim en = If(r.Element("End") Is Nothing, "", r.Element("End").Value)
                    Dim a = If(r.Element("Action") Is Nothing, "shutdown", r.Element("Action").Value)
                    dgvRestrictions.Rows.Add(s, en, a)
                Next
            End If

            ' Admin hash (no mostrar contraseña)
            Dim ah = root.Element("AdminPasswordHash")
            originalAdminHash = If(ah IsNot Nothing, ah.Value.Trim(), "")
            UpdatePasswordStatus()
            MessageBox.Show("Cargado correctamente.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error al cargar: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdatePasswordStatus()
        If String.IsNullOrEmpty(originalAdminHash) Then
            lblPasswordStatus.Text = "Estado contraseña: SIN contraseña"
        Else
            lblPasswordStatus.Text = "Estado contraseña: PROTEGIDO"
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        Dim path = txtSettingsPath.Text.Trim()
        If String.IsNullOrEmpty(path) Then
            MessageBox.Show("Especifique la ruta de settings.xml.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' Validación contraseña actual si existe
            Dim currentPwd = txtPassword.Text
            Dim newPwd = txtNewPassword.Text

            If Not String.IsNullOrEmpty(originalAdminHash) Then
                If String.IsNullOrEmpty(currentPwd) Then
                    MessageBox.Show("Es necesario introducir la contraseña actual para guardar cambios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
                If ComputeSha256Hash(currentPwd) <> originalAdminHash Then
                    MessageBox.Show("Contraseña actual incorrecta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End If

            Dim doc As New XDocument(New XElement("Settings",
                                                   New XElement("KillIntervalSeconds", txtInterval.Text.Trim()),
                                                   New XElement("Forbidden"),
                                                   New XElement("Restrictions"),
                                                   New XElement("AdminPasswordHash", "")
                                                   ))

            Dim root = doc.Root
            For Each item In lbForbidden.Items
                root.Element("Forbidden").Add(New XElement("Process", item.ToString()))
            Next

            For Each row As DataGridViewRow In dgvRestrictions.Rows
                If row.IsNewRow Then Continue For
                Dim s = If(row.Cells("Start").Value, "").ToString()
                Dim en = If(row.Cells("End").Value, "").ToString()
                Dim a = If(row.Cells("Action").Value, "shutdown").ToString()
                root.Element("Restrictions").Add(New XElement("Restriction",
                                                             New XElement("Start", s),
                                                             New XElement("End", en),
                                                             New XElement("Action", a)))
            Next

            ' Guardar el hash: si hay nueva contraseña, usarla; si no, conservar la existente
            If Not String.IsNullOrEmpty(newPwd) Then
                root.Element("AdminPasswordHash").Value = ComputeSha256Hash(newPwd)
            Else
                root.Element("AdminPasswordHash").Value = originalAdminHash
            End If

            Dim dir = System.IO.Path.GetDirectoryName(path)
            If Not Directory.Exists(dir) Then Directory.CreateDirectory(dir)
            doc.Save(path)

            ' Actualizar estado local
            originalAdminHash = root.Element("AdminPasswordHash").Value
            UpdatePasswordStatus()

            MessageBox.Show("Guardado correctamente.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnAddProcess_Click(sender As Object, e As EventArgs)
        Dim name = txtNewProcess.Text.Trim()
        If String.IsNullOrEmpty(name) Then Return
        If name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) Then
            name = name
        End If
        If Not lbForbidden.Items.Contains(name) Then
            lbForbidden.Items.Add(name)
            txtNewProcess.Clear()
        End If
    End Sub

    Private Sub BtnRemoveProcess_Click(sender As Object, e As EventArgs)
        If lbForbidden.SelectedIndex >= 0 Then
            lbForbidden.Items.RemoveAt(lbForbidden.SelectedIndex)
        End If
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub BtnTestConnection_Click(sender As Object, e As EventArgs)
        Dim pwd = txtPassword.Text
        Dim resp = SendCommandToServiceAsync("STATUS", pwd).GetAwaiter().GetResult()
        MessageBox.Show("Respuesta: " & resp, "Servicio", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BtnReloadService_Click(sender As Object, e As EventArgs)
        Dim pwd = txtPassword.Text
        Dim resp = SendCommandToServiceAsync("RELOAD", pwd).GetAwaiter().GetResult()
        MessageBox.Show("Respuesta: " & resp, "Servicio", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Async Function SendCommandToServiceAsync(command As String, password As String) As Task(Of String)
        Try
            Using client As New NamedPipeClientStream(".", "OpenParentalControlPipe", PipeDirection.InOut, PipeOptions.Asynchronous)
                Await client.ConnectAsync(2000)
                Using sr As New StreamReader(client, Encoding.UTF8, False, 1024, True)
                    Using sw As New StreamWriter(client, Encoding.UTF8, 1024, True)
                        sw.AutoFlush = True
                        Dim hash = ComputeSha256Hash(If(password, ""))
                        Await sw.WriteLineAsync(command & "|" & hash)
                        ' Leer respuesta (hasta newline)
                        Dim response = Await sr.ReadLineAsync()
                        Return If(response, "SIN_RESPUESTA")
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return "ERROR: " & ex.Message
        End Try
    End Function

    Private Function ComputeSha256Hash(raw As String) As String
        If raw Is Nothing Then raw = ""
        Using sha = SHA256.Create()
            Dim bytes = Encoding.UTF8.GetBytes(raw)
            Dim hash = sha.ComputeHash(bytes)
            Dim sb As New StringBuilder()
            For Each b In hash
                sb.Append(b.ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function
End Class