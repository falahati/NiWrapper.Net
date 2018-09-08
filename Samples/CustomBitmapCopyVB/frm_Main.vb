Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports OpenNIWrapper

' ReSharper disable once InconsistentNaming
' ReSharper disable once UnusedMember.Global
Public Class frm_Main
    Dim _canceled As Boolean = False

    Function HandleError(status As OpenNI.Status) As Boolean
        If (status = OpenNI.Status.Ok) Then
            Return True
        End If
        Console.WriteLine("Error: " + status.ToString() + " - " + OpenNI.LastError)
        Console.ReadLine()
        Return False
    End Function

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click
        _canceled = True
    End Sub

    Dim _depthStream As VideoStream
    Dim _colorStream As VideoStream

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Dim status As OpenNI.Status
        Console.WriteLine(OpenNI.Version.ToString())
        status = OpenNI.Initialize()
        If (Not HandleError(status)) Then Environment.Exit(0)
        Dim devices As DeviceInfo() = OpenNI.EnumerateDevices()
        If (devices.Length = 0) Then Environment.Exit(0)
        Dim device As Device = devices(0).OpenDevice()
        Using (device)
            If (device.HasSensor(Device.SensorType.Depth) AndAlso device.HasSensor(Device.SensorType.Color)) Then
                _depthStream = device.CreateVideoStream(Device.SensorType.Depth)
                _colorStream = device.CreateVideoStream(Device.SensorType.Color)
                Try
                    device.ImageRegistration = Device.ImageRegistrationMode.DepthToColor
                    device.DepthColorSyncEnabled = True
                Catch ex As Exception
                    MessageBox.Show(
                        "No Support for Depth to Color registration and/or Depth and Color sync is not available when OpenNI used along with Kinect. Yet.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Try
                If (_depthStream.IsValid AndAlso _colorStream.IsValid) Then
                    If (Not HandleError(_depthStream.Start()) OrElse Not HandleError(_colorStream.Start())) Then
                        OpenNI.Shutdown()
                        Environment.Exit(0)
                    End If
                    While (Not _canceled)
                        If (_depthStream.IsFrameAvailable() AndAlso _colorStream.IsFrameAvailable()) Then
                            If (Not (pictureBox1.Image Is Nothing)) Then pictureBox1.Image.Dispose()
                            Using depthFrame As VideoFrameRef = _depthStream.ReadFrame()
                                Using colorFrame As VideoFrameRef = _colorStream.ReadFrame()
                                    pictureBox1.Image = ToBitmap(depthFrame, colorFrame)
                                End Using
                            End Using
                        End If
                        Application.DoEvents()
                    End While
                End If
            End If
        End Using
        OpenNI.Shutdown()
        Environment.Exit(0)
    End Sub

    Dim _bit As Bitmap

    Private Function ToBitmap(depthFrame As VideoFrameRef, colorFrame As VideoFrameRef) As Bitmap
        Dim imageBytes(depthFrame.FrameSize.Width*depthFrame.FrameSize.Height*3 - 1) As Byte
        If _
            (_bit Is Nothing OrElse _bit.Width <> depthFrame.FrameSize.Width OrElse
             _bit.Height <> depthFrame.FrameSize.Height OrElse _bit.PixelFormat <> PixelFormat.Format24bppRgb) Then
            _bit = New Bitmap(depthFrame.FrameSize.Width, depthFrame.FrameSize.Height, PixelFormat.Format24bppRgb)
        End If
        Dim maxDepth As UInt16 = 0
        Dim minDepth As UInt16 = UInt16.MaxValue
        Dim dataPosition As IntPtr = depthFrame.Data
        For p = 0 To (depthFrame.DataSize/2) - 1
            Dim depth As UInt16 = Marshal.ReadInt16(dataPosition)
            If (depth > maxDepth) Then maxDepth = depth
            If (depth < minDepth) Then minDepth = depth
            dataPosition += 2
        Next
        Dim i = 0
        For y = 0 To depthFrame.FrameSize.Height - 1
            Dim depthPixelPosition = New IntPtr((y*depthFrame.DataStrideBytes) + depthFrame.Data.ToInt64())
            Dim colorPixelPosition = New IntPtr((y*colorFrame.DataStrideBytes) + colorFrame.Data.ToInt64())
            For x = 0 To depthFrame.FrameSize.Width - 1
                Dim depth As UInt16 = Marshal.ReadInt16(depthPixelPosition)
                Dim color As Rgb = Marshal.PtrToStructure(colorPixelPosition, GetType(Rgb))
                If (depth > 0) Then
                    If (IsThisSpecialDepth(depth, x, y)) Then
                        ' Special GREEN
                        imageBytes(i) = 0 ' Red
                        imageBytes(i + 1) = (CType(color.G, Integer) + color.R + color.B)\3 ' Geen
                        imageBytes(i + 2) = 0 ' Blue
                    Else
                        ' Normal Color
                        imageBytes(i) = color.R ' Red
                        imageBytes(i + 1) = color.G ' Geen
                        imageBytes(i + 2) = color.B ' Blue
                    End If
                Else
                    ' Black Shadow
                    imageBytes(i) = 0 ' Red
                    imageBytes(i + 1) = 0 ' Geen
                    imageBytes(i + 2) = 0 ' Blue
                End If
                depthPixelPosition += Marshal.SizeOf(GetType(Int16))
                colorPixelPosition += Marshal.SizeOf(GetType(Rgb))
                i += Marshal.SizeOf(GetType(Rgb))
            Next
        Next

        Dim bitData As BitmapData = _bit.LockBits(New Rectangle(0, 0, _bit.Width, _bit.Height), ImageLockMode.WriteOnly,
                                                  PixelFormat.Format24bppRgb)
        Marshal.Copy(imageBytes, 0, bitData.Scan0, imageBytes.Length - 1)
        _bit.UnlockBits(bitData)

        Return New Bitmap(_bit)
    End Function

    Private Function IsThisSpecialDepth(depthValue As UInt16, x As Integer, y As Integer) As Boolean
        Dim rwX, rwY, rwZ As Single
        If (CoordinateConverter.ConvertDepthToWorld(_depthStream, x, y, depthValue, rwX, rwY, rwZ) = OpenNI.Status.Ok) _
            Then
            Return rwZ < 1000
        End If
        Return False
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure Rgb
        Public B As Byte
        Public G As Byte
        Public R As Byte
    End Structure
End Class

