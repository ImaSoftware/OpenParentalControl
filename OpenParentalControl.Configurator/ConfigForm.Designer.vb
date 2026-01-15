<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ConfigForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer = Nothing

    Friend WithEvents txtSettingsPath As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents btnLoad As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents lblInterval As System.Windows.Forms.Label
    Friend WithEvents txtInterval As System.Windows.Forms.TextBox
    Friend WithEvents lbForbidden As System.Windows.Forms.ListBox
    Friend WithEvents txtNewProcess As System.Windows.Forms.TextBox
    Friend WithEvents btnAddProcess As System.Windows.Forms.Button
    Friend WithEvents btnRemoveProcess As System.Windows.Forms.Button
    Friend WithEvents dgvRestrictions As System.Windows.Forms.DataGridView
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblNewPassword As System.Windows.Forms.Label
    Friend WithEvents txtNewPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblPasswordStatus As System.Windows.Forms.Label
    Friend WithEvents btnTestConnection As System.Windows.Forms.Button
    Friend WithEvents btnReloadService As System.Windows.Forms.Button

    ' Nuevos controles para límite diario
    Friend WithEvents lblDailyLimit As System.Windows.Forms.Label
    Friend WithEvents nudHours As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudMinutes As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblHours As System.Windows.Forms.Label
    Friend WithEvents lblMinutes As System.Windows.Forms.Label
    Friend WithEvents cbDailyAction As System.Windows.Forms.ComboBox

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.txtSettingsPath = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.lblInterval = New System.Windows.Forms.Label()
        Me.txtInterval = New System.Windows.Forms.TextBox()
        Me.lbForbidden = New System.Windows.Forms.ListBox()
        Me.txtNewProcess = New System.Windows.Forms.TextBox()
        Me.btnAddProcess = New System.Windows.Forms.Button()
        Me.btnRemoveProcess = New System.Windows.Forms.Button()
        Me.dgvRestrictions = New System.Windows.Forms.DataGridView()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.lblNewPassword = New System.Windows.Forms.Label()
        Me.txtNewPassword = New System.Windows.Forms.TextBox()
        Me.lblPasswordStatus = New System.Windows.Forms.Label()
        Me.btnTestConnection = New System.Windows.Forms.Button()
        Me.btnReloadService = New System.Windows.Forms.Button()
        Me.lblDailyLimit = New System.Windows.Forms.Label()
        Me.nudHours = New System.Windows.Forms.NumericUpDown()
        Me.nudMinutes = New System.Windows.Forms.NumericUpDown()
        Me.lblHours = New System.Windows.Forms.Label()
        Me.lblMinutes = New System.Windows.Forms.Label()
        Me.cbDailyAction = New System.Windows.Forms.ComboBox()
        CType(Me.dgvRestrictions, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudHours, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudMinutes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtSettingsPath
        '
        Me.txtSettingsPath.Location = New System.Drawing.Point(12, 12)
        Me.txtSettingsPath.Name = "txtSettingsPath"
        Me.txtSettingsPath.Size = New System.Drawing.Size(520, 20)
        Me.txtSettingsPath.TabIndex = 0
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(540, 10)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(80, 23)
        Me.btnBrowse.TabIndex = 1
        Me.btnBrowse.Text = "Examinar..."
        '
        'btnLoad
        '
        Me.btnLoad.Location = New System.Drawing.Point(630, 10)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(70, 23)
        Me.btnLoad.TabIndex = 2
        Me.btnLoad.Text = "Cargar"
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(630, 46)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(70, 23)
        Me.btnSave.TabIndex = 3
        Me.btnSave.Text = "Guardar"
        '
        'lblInterval
        '
        Me.lblInterval.AutoSize = True
        Me.lblInterval.Location = New System.Drawing.Point(12, 46)
        Me.lblInterval.Name = "lblInterval"
        Me.lblInterval.Size = New System.Drawing.Size(65, 13)
        Me.lblInterval.TabIndex = 4
        Me.lblInterval.Text = "Intervalo (s):"
        '
        'txtInterval
        '
        Me.txtInterval.Location = New System.Drawing.Point(90, 42)
        Me.txtInterval.Name = "txtInterval"
        Me.txtInterval.Size = New System.Drawing.Size(60, 20)
        Me.txtInterval.TabIndex = 5
        Me.txtInterval.Text = "5"
        '
        'lbForbidden
        '
        Me.lbForbidden.Location = New System.Drawing.Point(12, 80)
        Me.lbForbidden.Name = "lbForbidden"
        Me.lbForbidden.Size = New System.Drawing.Size(240, 251)
        Me.lbForbidden.TabIndex = 6
        '
        'txtNewProcess
        '
        Me.txtNewProcess.Location = New System.Drawing.Point(12, 350)
        Me.txtNewProcess.Name = "txtNewProcess"
        Me.txtNewProcess.Size = New System.Drawing.Size(160, 20)
        Me.txtNewProcess.TabIndex = 7
        '
        'btnAddProcess
        '
        Me.btnAddProcess.Location = New System.Drawing.Point(180, 348)
        Me.btnAddProcess.Name = "btnAddProcess"
        Me.btnAddProcess.Size = New System.Drawing.Size(72, 23)
        Me.btnAddProcess.TabIndex = 8
        Me.btnAddProcess.Text = "Añadir"
        '
        'btnRemoveProcess
        '
        Me.btnRemoveProcess.Location = New System.Drawing.Point(12, 380)
        Me.btnRemoveProcess.Name = "btnRemoveProcess"
        Me.btnRemoveProcess.Size = New System.Drawing.Size(240, 23)
        Me.btnRemoveProcess.TabIndex = 9
        Me.btnRemoveProcess.Text = "Eliminar seleccionado"
        '
        'dgvRestrictions
        '
        Me.dgvRestrictions.AllowUserToAddRows = False
        Me.dgvRestrictions.Location = New System.Drawing.Point(270, 80)
        Me.dgvRestrictions.Name = "dgvRestrictions"
        Me.dgvRestrictions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvRestrictions.Size = New System.Drawing.Size(430, 320)
        Me.dgvRestrictions.TabIndex = 10
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(630, 380)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(70, 23)
        Me.btnClose.TabIndex = 11
        Me.btnClose.Text = "Cerrar"
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Location = New System.Drawing.Point(12, 410)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(96, 13)
        Me.lblPassword.TabIndex = 12
        Me.lblPassword.Text = "Contraseña actual:"
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(12, 428)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(160, 20)
        Me.txtPassword.TabIndex = 13
        '
        'lblNewPassword
        '
        Me.lblNewPassword.AutoSize = True
        Me.lblNewPassword.Location = New System.Drawing.Point(200, 410)
        Me.lblNewPassword.Name = "lblNewPassword"
        Me.lblNewPassword.Size = New System.Drawing.Size(98, 13)
        Me.lblNewPassword.TabIndex = 14
        Me.lblNewPassword.Text = "Nueva contraseña:"
        '
        'txtNewPassword
        '
        Me.txtNewPassword.Location = New System.Drawing.Point(200, 428)
        Me.txtNewPassword.Name = "txtNewPassword"
        Me.txtNewPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtNewPassword.Size = New System.Drawing.Size(160, 20)
        Me.txtNewPassword.TabIndex = 15
        '
        'lblPasswordStatus
        '
        Me.lblPasswordStatus.AutoSize = True
        Me.lblPasswordStatus.Location = New System.Drawing.Point(317, 408)
        Me.lblPasswordStatus.Name = "lblPasswordStatus"
        Me.lblPasswordStatus.Size = New System.Drawing.Size(163, 13)
        Me.lblPasswordStatus.TabIndex = 16
        Me.lblPasswordStatus.Text = "Estado contraseña: desconocido"
        '
        'btnTestConnection
        '
        Me.btnTestConnection.Location = New System.Drawing.Point(480, 424)
        Me.btnTestConnection.Name = "btnTestConnection"
        Me.btnTestConnection.Size = New System.Drawing.Size(100, 23)
        Me.btnTestConnection.TabIndex = 17
        Me.btnTestConnection.Text = "Probar servicio"
        '
        'btnReloadService
        '
        Me.btnReloadService.Location = New System.Drawing.Point(590, 424)
        Me.btnReloadService.Name = "btnReloadService"
        Me.btnReloadService.Size = New System.Drawing.Size(100, 23)
        Me.btnReloadService.TabIndex = 18
        Me.btnReloadService.Text = "Recargar servicio"
        '
        'lblDailyLimit
        '
        Me.lblDailyLimit.AutoSize = True
        Me.lblDailyLimit.Location = New System.Drawing.Point(170, 46)
        Me.lblDailyLimit.Name = "lblDailyLimit"
        Me.lblDailyLimit.Size = New System.Drawing.Size(123, 13)
        Me.lblDailyLimit.TabIndex = 19
        Me.lblDailyLimit.Text = "Límite diario (horas/min):"
        '
        'nudHours
        '
        Me.nudHours.Location = New System.Drawing.Point(300, 44)
        Me.nudHours.Maximum = New Decimal(New Integer() {23, 0, 0, 0})
        Me.nudHours.Name = "nudHours"
        Me.nudHours.Size = New System.Drawing.Size(50, 20)
        Me.nudHours.TabIndex = 20
        '
        'nudMinutes
        '
        Me.nudMinutes.Location = New System.Drawing.Point(405, 44)
        Me.nudMinutes.Maximum = New Decimal(New Integer() {59, 0, 0, 0})
        Me.nudMinutes.Name = "nudMinutes"
        Me.nudMinutes.Size = New System.Drawing.Size(50, 20)
        Me.nudMinutes.TabIndex = 22
        '
        'lblHours
        '
        Me.lblHours.AutoSize = True
        Me.lblHours.Location = New System.Drawing.Point(356, 46)
        Me.lblHours.Name = "lblHours"
        Me.lblHours.Size = New System.Drawing.Size(33, 13)
        Me.lblHours.TabIndex = 21
        Me.lblHours.Text = "horas"
        '
        'lblMinutes
        '
        Me.lblMinutes.AutoSize = True
        Me.lblMinutes.Location = New System.Drawing.Point(461, 46)
        Me.lblMinutes.Name = "lblMinutes"
        Me.lblMinutes.Size = New System.Drawing.Size(43, 13)
        Me.lblMinutes.TabIndex = 23
        Me.lblMinutes.Text = "minutos"
        '
        'cbDailyAction
        '
        Me.cbDailyAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDailyAction.Items.AddRange(New Object() {"shutdown", "logout", "force_shutdown"})
        Me.cbDailyAction.Location = New System.Drawing.Point(520, 43)
        Me.cbDailyAction.Name = "cbDailyAction"
        Me.cbDailyAction.Size = New System.Drawing.Size(100, 21)
        Me.cbDailyAction.TabIndex = 24
        '
        'ConfigForm
        '
        Me.ClientSize = New System.Drawing.Size(720, 460)
        Me.Controls.Add(Me.txtSettingsPath)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.btnLoad)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblInterval)
        Me.Controls.Add(Me.txtInterval)
        Me.Controls.Add(Me.lblDailyLimit)
        Me.Controls.Add(Me.nudHours)
        Me.Controls.Add(Me.lblHours)
        Me.Controls.Add(Me.nudMinutes)
        Me.Controls.Add(Me.lblMinutes)
        Me.Controls.Add(Me.cbDailyAction)
        Me.Controls.Add(Me.lbForbidden)
        Me.Controls.Add(Me.txtNewProcess)
        Me.Controls.Add(Me.btnAddProcess)
        Me.Controls.Add(Me.btnRemoveProcess)
        Me.Controls.Add(Me.dgvRestrictions)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.lblPassword)
        Me.Controls.Add(Me.txtPassword)
        Me.Controls.Add(Me.lblNewPassword)
        Me.Controls.Add(Me.txtNewPassword)
        Me.Controls.Add(Me.lblPasswordStatus)
        Me.Controls.Add(Me.btnTestConnection)
        Me.Controls.Add(Me.btnReloadService)
        Me.Name = "ConfigForm"
        Me.Text = "Configurador - OpenParentalControl"
        CType(Me.dgvRestrictions, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudHours, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudMinutes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class