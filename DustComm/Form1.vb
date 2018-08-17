Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Data.OleDb
Imports EASendMail
Public Class Form1
    Dim portdata As String
    Dim vArray As Array
    Dim comOpen As String
    Dim port As Array
    Dim time As String
    Dim time1 As String
    Dim p_length As Integer
    Dim tempChart As New Series
    Dim readbuffer As String
    Dim read As String
    Dim provider As String
    Dim datafile As String
    Dim connstring As String
    Dim cnn As New OleDbConnection
    Dim cmd As New OleDbCommand
    Dim userMsg As String
    'Dim potChart As New Series
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        userMsg = Microsoft.VisualBasic.InputBox("Please Enter Password", "DustComm Password Manager", "Enter your password here", 500, 300)
        If userMsg = "dustcomm7" Then

        Else
            MessageBox.Show("Wrong Password")
            End
        End If
        Label5.Text = Format(Now, "General Date")
        port = IO.Ports.SerialPort.GetPortNames()
        ComboBox1.Items.AddRange(port)
        p_length = port.Length
        Timer1.Enabled = True
        Chart1.Series.Clear()
        Chart1.Titles.Add("Dust Monitoring")
        tempChart.Name = "Dust Concentration (ug/m3)"
        tempChart.ChartType = SeriesChartType.Line
        Chart1.Series.Add(tempChart)
        provider = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
        datafile = "D:\mydata.mdb"
        connstring = provider & datafile
        cnn.ConnectionString = connstring
    End Sub
    Private Sub SerialPort1_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        If comOpen Then
            'System.Threading.Thread.Sleep(500)
            readbuffer = SerialPort1.ReadExisting
            'vArray = Split(readbuffer, ",")
            Me.Invoke(New EventHandler(AddressOf updateTemp))
        End If
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        SerialPort1.PortName = ComboBox1.Text
        SerialPort1.BaudRate = CInt(ComboBox2.Text)

        Try
            SerialPort1.Open()
            comOpen = SerialPort1.IsOpen
        Catch ex As Exception
            comOpen = False
            MsgBox(ex.Message)

        End Try
        Button1.Enabled = False
        Button2.Enabled = True
        ComboBox1.Enabled = False
        ComboBox2.Enabled = False

    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        If comOpen Then
            SerialPort1.DiscardInBuffer()
            SerialPort1.Close()
            comOpen = False
            Button1.Enabled = True
            Button2.Enabled = False
            ComboBox1.Enabled = True
            ComboBox2.Enabled = True

        End If
        ComboBox1.Text = ""
        ComboBox2.Text = ""
        TextBox1.Clear()
        TextBox2.Clear()
        ' ListBox1.ClearSelected()
    End Sub
    Public Sub updateTemp(ByVal sender As Object, ByVal e As System.EventArgs)
        read = readbuffer.Replace(vbCr, "").Replace(vbLf, "")
        TextBox1.Text = read
        TextBox2.Text = "0001"
        'TextBox2.Text = vArray(1)
        Dim cnn As New OleDbConnection
        Dim cmd As New OleDbCommand
        provider = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
        datafile = "D:\mydata.mdb"
        connstring = provider & datafile
        cnn.ConnectionString = connstring
        Try
            cnn.Open()
            cmd.Connection = cnn
            cmd.CommandText = "Insert into Table1([time], [dust]) values(@time, @dust)"
            cmd.Parameters.AddWithValue("@time", time1)
            cmd.Parameters.AddWithValue("@Password", TextBox1.Text)
            cmd.ExecuteNonQuery()
            cnn.Close()
            'MsgBox("Data added")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        tempChart.Points.AddXY(time, read)
        'SerialPort1.DiscardInBuffer()
        ' potChart.Points.AddXY(time, vArray(1))
        ' ListBox1.Items.Insert(0, vArray(0))
        'ListBox1.Items.Insert(1, time)
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        ComboBox1.Items.Clear()
        port = IO.Ports.SerialPort.GetPortNames()
        ComboBox1.Items.AddRange(port)
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        port = IO.Ports.SerialPort.GetPortNames()
        time = TimeOfDay
        Label5.Text = Format(Now, "General Date")
        time1 = Format(Now, "General Date")
        If p_length <> port.Length Then
            ComboBox1.Items.Clear()
            ComboBox1.Items.AddRange(port)
            p_length = port.Length
        End If
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Timer1.Enabled = False
        'SerialPort1.DiscardInBuffer()
        SerialPort1.Close()
        comOpen = False
        'ListBox1.ClearSelected()
        End
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim oMail As New SmtpMail("TryIt")
        Dim oSmtp As New SmtpClient()

        oMail.From = "swainamit18sl@gmail.com"
        oMail.To = "hbsahu@gmail.com"
        oMail.Cc = "swainamit18is@gmail.com"
        oMail.Subject = "First Mail"
        oMail.AddAttachment("D:\mydata.mdb")
        oMail.AddAttachment("D:\screenGrab.bmp")
        oMail.TextBody = "This is a test email sent from VB.NET project with Gmail"

        'Dim oServer As New SmtpServer("smtp.mail.yahoo.com")
        Dim oServer As New SmtpServer("smtp.gmail.com")

        oServer.User = "swainamit18sl@gmail.com"
        oServer.Password = "ilsunnyleone"
        oServer.Port = 465
        oServer.ConnectType = SmtpConnectType.ConnectSSLAuto
        Try
            Console.WriteLine("start to send email over SSL ...")
            oSmtp.SendMail(oServer, oMail)
            Console.WriteLine("email was sent successfully!")
            MsgBox("Mail has been Sent Successfully")
        Catch ep As Exception
            Console.WriteLine("failed to send email with the following error:")
            Console.WriteLine(ep.Message)
            MsgBox("Sending Failed!")
        End Try
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim screenSize As Size = New Size(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
        Dim screenGrab As New Bitmap(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
        Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(screenGrab)
        g.CopyFromScreen(New Point(0, 0), New Point(0, 0), screenSize)
        screenGrab.Save("D:\screenGrab.bmp")
    End Sub
End Class
