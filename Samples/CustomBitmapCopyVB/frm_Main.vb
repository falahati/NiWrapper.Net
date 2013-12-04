Imports OpenNIWrapper
Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging

Public Class frm_Main
    Dim canceled As Boolean = False
    Function HandleError(status As OpenNI.Status) As Boolean
        If (status = OpenNI.Status.OK) Then
            Return True
        End If
        Console.WriteLine("Error: " + status.ToString() + " - " + OpenNI.LastError)
        Console.ReadLine()
        Return False
    End Function

    Private Sub button1_Click(sender As System.Object, e As System.EventArgs) Handles button1.Click
        canceled = True
    End Sub
    Dim depthStream As VideoStream
    Dim colorStream As VideoStream
    Private Sub Form1_Shown(sender As System.Object, e As System.EventArgs) Handles MyBase.Shown
        Dim status As OpenNI.Status
        Console.WriteLine(OpenNI.Version.ToString())
        status = OpenNI.Initialize()
        If (Not HandleError(status)) Then Environment.Exit(0)
        Dim devices As DeviceInfo() = OpenNI.EnumerateDevices()
        If (devices.Length = 0) Then Environment.Exit(0)
        Dim device As Device = devices(0).OpenDevice()
        Using (device)
            If (device.hasSensor(device.SensorType.DEPTH) AndAlso device.hasSensor(device.SensorType.COLOR)) Then
                depthStream = device.CreateVideoStream(device.SensorType.DEPTH)
                colorStream = device.CreateVideoStream(device.SensorType.COLOR)
                Try
                    device.ImageRegistration = device.ImageRegistrationMode.DEPTH_TO_COLOR
                    device.DepthColorSyncEnabled = True
                Catch ex As Exception
                    MessageBox.Show("No Support for Depth to Color registration and/or Depth and Color sync is not available when OpenNI used along with Kinect. Yet.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Try
                If (depthStream.isValid AndAlso colorStream.isValid) Then
                    If (Not HandleError(depthStream.Start()) OrElse Not HandleError(colorStream.Start())) Then
                        OpenNI.Shutdown()
                        Environment.Exit(0)
                    End If
                    While (Not canceled)
                        If (depthStream.isFrameAvailable() AndAlso colorStream.isFrameAvailable()) Then
                            Dim depthFrame As VideoFrameRef = depthStream.readFrame()
                            Dim colorFrame As VideoFrameRef = colorStream.readFrame()
                            If (Not (pictureBox1.Image Is Nothing)) Then pictureBox1.Image.Dispose()
                            pictureBox1.Image = ToBitmap(depthFrame, colorFrame)
                            depthFrame.Release()
                            colorFrame.Release()
                        End If
                        Application.DoEvents()
                    End While
                End If
            End If
        End Using
        OpenNI.Shutdown()
        Environment.Exit(0)
    End Sub
    Dim bit As Bitmap
    Private Function ToBitmap(depthFrame As VideoFrameRef, colorFrame As VideoFrameRef) As Bitmap
        Dim imageBytes(depthFrame.FrameSize.Width * depthFrame.FrameSize.Height * 3 - 1) As Byte
        If (bit Is Nothing OrElse bit.Width <> depthFrame.FrameSize.Width OrElse bit.Height <> depthFrame.FrameSize.Height OrElse bit.PixelFormat <> Imaging.PixelFormat.Format24bppRgb) Then
            bit = New Bitmap(depthFrame.FrameSize.Width, depthFrame.FrameSize.Height, Imaging.PixelFormat.Format24bppRgb)
        End If
        Dim maxDepth As UInt16 = 0
        Dim minDepth As UInt16 = UInt16.MaxValue
        Dim dataPosition As IntPtr = depthFrame.Data
        For p As Integer = 0 To (depthFrame.DataSize / 2) - 1
            Dim depth As UInt16 = Marshal.ReadInt16(dataPosition)
            If (depth > maxDepth) Then maxDepth = depth
            If (depth < minDepth) Then minDepth = depth
            dataPosition += 2
        Next
        Dim i As Integer = 0
        For y As Integer = 0 To depthFrame.FrameSize.Height - 1
            Dim depthPixelPosition As IntPtr = New IntPtr(CType((y * depthFrame.DataStrideBytes) + depthFrame.Data, Integer))
            Dim colorPixelPosition As IntPtr = New IntPtr(CType((y * colorFrame.DataStrideBytes) + colorFrame.Data, Integer))
            For x As Integer = 0 To depthFrame.FrameSize.Width - 1
                Dim depth As UInt16 = Marshal.ReadInt16(depthPixelPosition)
                Dim color As RGB = Marshal.PtrToStructure(colorPixelPosition, GetType(RGB))
                If (depth > 0) Then
                    If (IsThisSpecialDepth(depth, x, y)) Then
                        ' Special GREEN
                        imageBytes(i) = 0 ' Red
                        imageBytes(i + 1) = (CType(color.G, Integer) + color.R + color.B) \ 3 ' Geen
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
                colorPixelPosition += Marshal.SizeOf(GetType(RGB))
                i += Marshal.SizeOf(GetType(RGB))
            Next
        Next

        Dim bitData As BitmapData = bit.LockBits(New Rectangle(0, 0, bit.Width, bit.Height), Imaging.ImageLockMode.WriteOnly, Imaging.PixelFormat.Format24bppRgb)
        Marshal.Copy(imageBytes, 0, bitData.Scan0, imageBytes.Length - 1)
        bit.UnlockBits(bitData)

        Return New Bitmap(bit)
    End Function

    Private Function IsThisSpecialDepth(depthValue As UInt16, x As Integer, y As Integer) As Boolean
        Dim rwX, rwY, rwZ As Single
        If (CoordinateConverter.convertDepthToWorld(depthStream, x, y, depthValue, rwX, rwY, rwZ) = OpenNI.Status.OK) Then
            Return rwZ < 1000
        End If
        Return False
    End Function
    <StructLayout(LayoutKind.Sequential)>
    Public Structure RGB
        Public B As Byte
        Public G As Byte
        Public R As Byte
    End Structure
End Class

