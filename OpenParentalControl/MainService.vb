Imports System.Timers
Imports System.IO
Imports System.Xml.Linq
Imports System.Diagnostics
Imports System.Threading
Imports System.Threading.Tasks
Imports System.IO.Pipes

Public Class MainService

    Private watchdog As Timers.Timer
    Private settingsPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml")
    Private logPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "service.log")
    Private usageStatePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usage.state")
    Private forbidden As List(Of String)
    Private restrictions As List(Of Restriction)
    Private intervalSeconds As Integer = 5

    ' Daily limit
    Private dailyLimit As TimeSpan = TimeSpan.Zero
    Private dailyLimitAction As String = "shutdown"
    Private usageToday As TimeSpan = TimeSpan.Zero
    Private lastUsageDate As Date = Date.MinValue
    Private usageSaveCounter As Integer = 0

    ' Named pipe server
    Private pipeCts As CancellationTokenSource
    Private pipeTask As Task
    Private adminHash As String = String.Empty
    Private syncObj As New Object()

    Protected Overrides Sub OnStart(ByVal args() As String)
        WriteLog("OnStart")
        LoadSettings()
        LoadUsageState()
        watchdog = New Timers.Timer(Math.Max(1000, intervalSeconds * 1000))
        AddHandler watchdog.Elapsed, AddressOf OnTick
        watchdog.AutoReset = True
        watchdog.Start()
        WriteLog("Servicio iniciado. Intervalo (s): " & intervalSeconds.ToString())
        StartPipeServer()
    End Sub

    Protected Overrides Sub OnStop()
        WriteLog("OnStop")
        If watchdog IsNot Nothing Then
            watchdog.Stop()
            watchdog.Dispose()
            watchdog = Nothing
        End If
        SaveUsageState()
        StopPipeServer()
        WriteLog("Servicio detenido.")
    End Sub

    Private Sub LoadSettings()
        Try
            If Not File.Exists(settingsPath) Then
                CreateDefaultSettings()
                WriteLog("settings.xml no existía; creado archivo por defecto.")
            End If

            Dim doc = XDocument.Load(settingsPath)
            Dim root = doc.Root

            Dim ks = root.Element("KillIntervalSeconds")
            If ks IsNot Nothing AndAlso Integer.TryParse(ks.Value, intervalSeconds) Then
                intervalSeconds = Math.Max(1, intervalSeconds)
            Else
                intervalSeconds = 5
            End If

            forbidden = New List(Of String)
            Dim forbiddenNode = root.Element("Forbidden")
            If forbiddenNode IsNot Nothing Then
                For Each p As XElement In forbiddenNode.Elements("Process")
                    Dim name = p.Value.Trim().ToLower()
                    If name <> "" Then
                        If name.EndsWith(".exe") Then name = name.Substring(0, name.Length - 4)
                        forbidden.Add(name)
                    End If
                Next
            End If

            restrictions = New List(Of Restriction)
            Dim restrictionsNode = root.Element("Restrictions")
            If restrictionsNode IsNot Nothing Then
                For Each r As XElement In restrictionsNode.Elements("Restriction")
                    Dim s = If(r.Element("Start") Is Nothing, "", r.Element("Start").Value)
                    Dim en = If(r.Element("End") Is Nothing, "", r.Element("End").Value)
                    Dim a = If(r.Element("Action") Is Nothing, "", r.Element("Action").Value).Trim().ToLower()
                    If s <> "" AndAlso en <> "" Then
                        Dim ts As TimeSpan
                        Dim te As TimeSpan
                        If TimeSpan.TryParse(s, ts) AndAlso TimeSpan.TryParse(en, te) Then
                            restrictions.Add(New Restriction(ts, te, a))
                        End If
                    End If
                Next
            End If

            ' DailyLimit
            dailyLimit = TimeSpan.Zero
            dailyLimitAction = "shutdown"
            Dim dl = root.Element("DailyLimit")
            If dl IsNot Nothing Then
                Dim hNode = dl.Element("Hours")
                Dim mNode = dl.Element("Minutes")
                Dim actNode = dl.Element("Action")
                Dim h As Integer = 0
                Dim m As Integer = 0
                If hNode IsNot Nothing Then Integer.TryParse(hNode.Value, h)
                If mNode IsNot Nothing Then Integer.TryParse(mNode.Value, m)
                dailyLimit = New TimeSpan(Math.Max(0, h), Math.Max(0, m), 0)
                If actNode IsNot Nothing Then dailyLimitAction = actNode.Value.Trim().ToLower()
            End If

            ' Leer hash de administrador
            Dim ah = root.Element("AdminPasswordHash")
            adminHash = If(ah IsNot Nothing, ah.Value.Trim(), "")

            WriteLog("Settings cargados. Procesos prohibidos: " & forbidden.Count.ToString() & ". Restricciones: " & restrictions.Count.ToString() & ". DailyLimit: " & dailyLimit.ToString())
        Catch ex As Exception
            WriteLog("Error cargando settings.xml: " & ex.ToString())
            forbidden = New List(Of String)
            restrictions = New List(Of Restriction)
            intervalSeconds = 5
            dailyLimit = TimeSpan.Zero
            adminHash = ""
        End Try
    End Sub

    Private Sub LoadUsageState()
        Try
            If File.Exists(usageStatePath) Then
                Dim lines = File.ReadAllLines(usageStatePath)
                If lines.Length >= 2 Then
                    Dim d As Date
                    If Date.TryParse(lines(0), d) Then
                        lastUsageDate = d.Date
                    Else
                        lastUsageDate = Date.Today
                    End If
                    Dim seconds As Long = 0
                    If Long.TryParse(lines(1), seconds) Then
                        usageToday = TimeSpan.FromSeconds(seconds)
                    Else
                        usageToday = TimeSpan.Zero
                    End If
                Else
                    lastUsageDate = Date.Today
                    usageToday = TimeSpan.Zero
                End If
            Else
                lastUsageDate = Date.Today
                usageToday = TimeSpan.Zero
            End If
            WriteLog("Estado de uso cargado: fecha=" & lastUsageDate.ToShortDateString() & " usados(s)=" & usageToday.TotalSeconds.ToString())
        Catch ex As Exception
            WriteLog("Error cargando usage.state: " & ex.ToString())
            lastUsageDate = Date.Today
            usageToday = TimeSpan.Zero
        End Try
    End Sub

    Private Sub SaveUsageState()
        Try
            Dim lines = New List(Of String)
            lines.Add(lastUsageDate.ToString("yyyy-MM-dd"))
            lines.Add(CLng(usageToday.TotalSeconds).ToString())
            File.WriteAllLines(usageStatePath, lines.ToArray())
            WriteLog("Estado de uso guardado: fecha=" & lastUsageDate.ToShortDateString() & " usados(s)=" & usageToday.TotalSeconds.ToString())
        Catch ex As Exception
            WriteLog("Error guardando usage.state: " & ex.ToString())
        End Try
    End Sub

    Private Sub OnTick(sender As Object, e As ElapsedEventArgs)
        Try
            ' Primero actualizar uso diario
            TryUpdateDailyUsage()

            KillForbiddenProcesses()
            EnforceRestrictions()
        Catch ex As Exception
            WriteLog("Error en OnTick: " & ex.ToString())
        End Try
    End Sub

    Private Sub TryUpdateDailyUsage()
        If dailyLimit = TimeSpan.Zero Then
            Return
        End If

        Dim today = Date.Today
        SyncLock syncObj
            If lastUsageDate <> today Then
                ' Nuevo día: resetear contador
                lastUsageDate = today
                usageToday = TimeSpan.Zero
                WriteLog("Nuevo día, contador diario reseteado.")
                SaveUsageState()
            End If
        End SyncLock

        ' Determinar si hay sesión interactiva: comprobar explorer.exe
        Dim interactive As Boolean = False
        Try
            Dim explorers = Process.GetProcessesByName("explorer")
            If explorers IsNot Nothing AndAlso explorers.Length > 0 Then
                interactive = True
            End If
        Catch ex As Exception
            interactive = False
        End Try

        If interactive Then
            ' Incrementar uso en intervalSeconds
            Dim incSeconds As Integer = Math.Max(1, intervalSeconds)
            usageToday = usageToday.Add(TimeSpan.FromSeconds(incSeconds))
            usageSaveCounter += 1
            ' Guardar estado cada 12 incrementos (ajustable)
            If usageSaveCounter >= 12 Then
                SaveUsageState()
                usageSaveCounter = 0
            End If

            WriteLog("Uso diario incrementado: +" & incSeconds & "s total=" & usageToday.ToString())
            ' Comprobar límite
            If usageToday >= dailyLimit Then
                WriteLog("Límite diario alcanzado. Acción: " & dailyLimitAction)
                Select Case dailyLimitAction
                    Case "shutdown"
                        ShutdownNow(force:=True)
                    Case "force_shutdown"
                        ShutdownNow(force:=True)
                    Case "logout"
                        LogoffNow()
                    Case Else
                        ' por defecto shutdown
                        ShutdownNow(force:=True)
                End Select
                ' Después de ejecutar la acción guardamos y evitamos múltiples ejecuciones inmediatas
                SaveUsageState()
            End If
        End If
    End Sub

    Private Sub KillForbiddenProcesses()
        For Each procName In forbidden
            Try
                Dim procs = Process.GetProcessesByName(procName)
                For Each p In procs
                    Try
                        WriteLog("Matando proceso: " & p.ProcessName & " (PID " & p.Id & ")")
                        p.Kill()
                    Catch ex As Exception
                        WriteLog("Error al matar proceso " & p.ProcessName & ": " & ex.Message)
                    End Try
                Next
            Catch ex As Exception
                WriteLog("Error al obtener procesos para " & procName & ": " & ex.Message)
            End Try
        Next
    End Sub

    Private Sub EnforceRestrictions()
        Dim nowTime = DateTime.Now.TimeOfDay
        For Each r In restrictions
            If r.IsNowInPeriod(nowTime) Then
                Select Case r.Action
                    Case "shutdown"
                        WriteLog("Periodo restringido activo - acción: shutdown")
                        ShutdownNow()
                    Case "logout"
                        WriteLog("Periodo restringido activo - acción: logout")
                        LogoffNow()
                    Case "force_shutdown"
                        WriteLog("Periodo restringido activo - acción: force_shutdown")
                        ShutdownNow(force:=True)
                    Case Else
                        WriteLog("Periodo restringido activo - acción desconocida: " & r.Action)
                End Select
            End If
        Next
    End Sub

    Private Sub ShutdownNow(Optional force As Boolean = False)
        Try
            Dim args = If(force, "/s /t 0 /f", "/s /t 0")
            Dim psi As New ProcessStartInfo("shutdown", args)
            psi.CreateNoWindow = True
            psi.UseShellExecute = False
            Process.Start(psi)
            WriteLog("Comando shutdown lanzado: " & args)
        Catch ex As Exception
            WriteLog("Error al intentar shutdown: " & ex.ToString())
        End Try
    End Sub

    Private Sub LogoffNow()
        Try
            Dim psi As New ProcessStartInfo("shutdown", "/l /f")
            psi.CreateNoWindow = True
            psi.UseShellExecute = False
            Process.Start(psi)
            WriteLog("Comando logoff lanzado (/l /f).")
        Catch ex As Exception
            WriteLog("Error al intentar logoff: " & ex.ToString())
        End Try
    End Sub

    Private Sub WriteLog(text As String)
        Try
            Dim line = DateTime.Now.ToString("s") & " - " & text & Environment.NewLine
            File.AppendAllText(logPath, line)
        Catch
            ' No hacer nada si el log falla
        End Try
    End Sub

    Private Sub CreateDefaultSettings()
        Dim doc As New XDocument(
            New XElement("Settings",
                New XElement("KillIntervalSeconds", "5"),
                New XElement("Forbidden",
                    New XElement("Process", "notepad.exe"),
                    New XElement("Process", "calc.exe")
                ),
                New XElement("Restrictions",
                    New XElement("Restriction",
                        New XElement("Start", "22:00"),
                        New XElement("End", "07:00"),
                        New XElement("Action", "shutdown")
                    )
                ),
                New XElement("DailyLimit",
                    New XElement("Hours", "0"),
                    New XElement("Minutes", "30"),
                    New XElement("Action", "shutdown")
                ),
                New XElement("AdminPasswordHash", "")
            )
        )
        doc.Save(settingsPath)
    End Sub

    Private Class Restriction
        Public Property StartTime As TimeSpan
        Public Property EndTime As TimeSpan
        Public Property Action As String

        Public Sub New(s As TimeSpan, e As TimeSpan, a As String)
            StartTime = s
            EndTime = e
            Action = a
        End Sub

        Public Function IsNowInPeriod(now As TimeSpan) As Boolean
            If StartTime <= EndTime Then
                Return (now >= StartTime AndAlso now < EndTime)
            Else
                ' Cruza medianoche
                Return (now >= StartTime) OrElse (now < EndTime)
            End If
        End Function
    End Class

    ' ----- Named pipe server (sin cambios) -----
    Private Sub StartPipeServer()
        StopPipeServer()
        pipeCts = New CancellationTokenSource()
        pipeTask = Task.Run(Sub() PipeServerLoop(pipeCts.Token))
        WriteLog("Pipe server iniciado.")
    End Sub

    Private Sub StopPipeServer()
        Try
            If pipeCts IsNot Nothing Then
                pipeCts.Cancel()
                pipeCts.Dispose()
                pipeCts = Nothing
            End If
            If pipeTask IsNot Nothing Then
                pipeTask.Wait(1000)
                pipeTask = Nothing
            End If
            WriteLog("Pipe server detenido.")
        Catch ex As Exception
            WriteLog("Error deteniendo pipe server: " & ex.Message)
        End Try
    End Sub

    Private Sub PipeServerLoop(ct As CancellationToken)
        While Not ct.IsCancellationRequested
            Try
                Using server As New NamedPipeServerStream("OpenParentalControlPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous)
                    Dim connected = server.WaitForConnectionAsync(ct)
                    connected.Wait(ct)
                    If ct.IsCancellationRequested Then Exit While

                    Using sr As New StreamReader(server, System.Text.Encoding.UTF8)
                        Using sw As New StreamWriter(server, System.Text.Encoding.UTF8)
                            sw.AutoFlush = True
                            Dim line = sr.ReadLine()
                            If String.IsNullOrEmpty(line) Then
                                sw.WriteLine("EMPTY")
                                Continue While
                            End If
                            ' Esperado: COMMAND|hash
                            Dim parts = line.Split("|"c)
                            Dim cmd = parts(0).Trim().ToUpperInvariant()
                            Dim suppliedHash = If(parts.Length > 1, parts(1).Trim(), "")

                            Dim authorized As Boolean = False
                            SyncLock syncObj
                                If String.IsNullOrEmpty(adminHash) Then
                                    ' Sin contraseña configurada -> permitir
                                    authorized = True
                                Else
                                    authorized = (String.Equals(adminHash, suppliedHash, StringComparison.OrdinalIgnoreCase))
                                End If
                            End SyncLock

                            If Not authorized Then
                                WriteLog("Intento de comando no autorizado: " & cmd)
                                sw.WriteLine("AUTHFAIL")
                                Continue While
                            End If

                            Select Case cmd
                                Case "RELOAD"
                                    WriteLog("Comando RELOAD recibido por pipe. Volviendo a cargar settings.")
                                    LoadSettings()
                                    sw.WriteLine("OK")
                                Case "STATUS"
                                    Dim msg = "OK;Forbidden=" & forbidden.Count.ToString() & ";Restrictions=" & restrictions.Count.ToString() & ";DailyLimit=" & dailyLimit.ToString() & ";UsedSeconds=" & CLng(usageToday.TotalSeconds).ToString()
                                    sw.WriteLine(msg)
                                Case Else
                                    sw.WriteLine("UNKNOWN")
                            End Select
                        End Using
                    End Using
                End Using
            Catch ex As OperationCanceledException
                Exit While
            Catch ex As Exception
                WriteLog("Error en PipeServerLoop: " & ex.ToString())
                Thread.Sleep(500)
            End Try
        End While
    End Sub
    ' ----- fin pipe server -----

End Class