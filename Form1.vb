Imports System
Imports System.IO
Imports System.IO.Path

Public Class Form1

    Enum uLedPositionArrangeType
        La_456
        La_852
        La_856
        La_854
    End Enum

    Enum uLedColoeArrangeType
        Ra_RGB
        Rg_RBG

        Ra_GRB
        Ra_GBR

        Ra_BRG
        Ra_BGR
    End Enum

    Public uLedInfoList As List(Of uLedInfo)

    Private Sub From1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.uLedInfoList = New List(Of uLedInfo)
    End Sub
    Private Sub Btn_Calculate_Click(sender As Object, e As EventArgs) Handles Btn_Calculate.Click
        Me.CalculateuLedInfo(Me.nud_AxisAngle.Value,
                             Me.nud_PanelAngle.Value,
                             Me.nud_CrossMarkMachX.Value,
                             Me.nud_CrossMarkMachY.Value,
                             Me.nud_uLedFirstDistX.Value,
                             Me.nud_uLedFirstDistY.Value,
                             Me.nud_uLedPixelPitchX.Value,
                             Me.nud_uLedPixelPitchY.Value,
                             Me.nud_uLedSubPixelPitchX.Value,
                             Me.nud_uLedSubPixelPitchY.Value,
                             Me.nud_DataNumber.Value,
                             Me.nud_GateNumber.Value,
                             uLedPositionArrangeType.La_852,
                             uLedColoeArrangeType.Ra_RGB,
                             160000,
                             80000,
                             10,
                             0.5,
                             0.5,
                             0.0,
                             0.0,
                             2500,
                             2500,
                             100,
                             100)
    End Sub

    Private Sub CalculateuLedInfo(ByVal AxisAngle As Double,
                                  ByVal PanelAngle As Double,
                                  ByVal CrossMarkMachineX As Double,
                                  ByVal CrossMarkMachineY As Double,
                                  ByVal uLedFirstDistX As Double,
                                  ByVal uLedFirstDistY As Double,
                                  ByVal uLedPixelPitchX As Double,
                                  ByVal uLedPixelPitchY As Double,
                                  ByVal uLedSubPixelPitchX As Double,
                                  ByVal uLedSubPixelPitchY As Double,
                                  ByVal DataNumber As Integer,
                                  ByVal GateNumber As Integer,
                                  ByVal uLedPositionArrange As uLedPositionArrangeType,
                                  ByVal uLEdColorArrange As uLedColoeArrangeType,
                                  ByVal CcdPitchX As Double,
                                  ByVal CcdOverlapX As Double,
                                  ByVal ScanTotalCount As Double,
                                  ByVal LesOptResX As Double,
                                  ByVal LesOptResY As Double,
                                  ByVal FirstFrameMachineX As Double,
                                  ByVal FirstFrameMachIneY As Double,
                                  ByVal FramePitchX As Double,
                                  ByVal FramePitchY As Double,
                                  ByVal FrameOverlapX As Double,
                                  ByVal FrameOverlapY As Double)

        Dim tmp_PreAngle As Double = 270 * Math.PI / 180                        ' 因為垂直角度，要預加270度
        Dim tmp_PanelAngle As Double = PanelAngle * Math.PI / 180
        Dim tmp_ToRadian As Double = Math.PI / 180
        Dim tmp_LedAngle As Double = 0.0
        Dim tmp_DistX As Double = 0.0
        Dim tmp_DistY As Double = 0.0
        Dim tmp_LedRadius As Double = 0.0
        Dim tmp_Quo As UInteger = 0.0
        Dim tmp_Rem As UInteger = 0.0
        Dim tmp_PositionType(2) As Point
        Dim tmp_ColorType(2) As Integer

        Select Case uLedPositionArrange

            Case uLedPositionArrangeType.La_456
                tmp_PositionType = {New Point(-1, 0), New Point(0, 0), New Point(1, 0)}

            Case uLedPositionArrangeType.La_852
                tmp_PositionType = {New Point(0, -1), New Point(0, 0), New Point(0, 1)}

            Case uLedPositionArrangeType.La_854
                tmp_PositionType = {New Point(0, -1), New Point(0, 0), New Point(-1, 0)}

            Case uLedPositionArrangeType.La_856
                tmp_PositionType = {New Point(0, -1), New Point(0, 0), New Point(1, 0)}

        End Select

        Select Case uLEdColorArrange

            Case uLedColoeArrangeType.Ra_BGR
                tmp_ColorType = {(2), (1), (0)}

            Case uLedColoeArrangeType.Ra_BRG
                tmp_ColorType = {(2), (0), (1)}

            Case uLedColoeArrangeType.Ra_GBR
                tmp_ColorType = {(1), (2), (0)}

            Case uLedColoeArrangeType.Ra_GRB
                tmp_ColorType = {(1), (0), (2)}

            Case uLedColoeArrangeType.Ra_RGB
                tmp_ColorType = {(0), (1), (2)}

            Case uLedColoeArrangeType.Rg_RBG
                tmp_ColorType = {(0), (2), (1)}

        End Select

        For cnt_h As UInteger = 0 To DataNumber - 1
            For cnt_v As UInteger = 0 To GateNumber - 1
                For cnt_p As Integer = 0 To 2
                    Dim tmp_uLedInfo As New uLedInfo

                    ' 算出Mark與每個Led的X/Y距離
                    tmp_DistX = uLedFirstDistX + uLedPixelPitchX * cnt_h + tmp_PositionType(cnt_p).X * uLedSubPixelPitchX - CrossMarkMachineX
                    tmp_DistY = uLedFirstDistY + uLedPixelPitchY * cnt_v + tmp_PositionType(cnt_p).Y * uLedSubPixelPitchY - CrossMarkMachineY

                    ' uLed Color
                    tmp_uLedInfo.uLedChannel = tmp_ColorType(cnt_p)

                    tmp_LedAngle = Math.Atan2(tmp_DistY, tmp_DistX)
                    tmp_LedRadius = Math.Sqrt(tmp_DistX * tmp_DistX + tmp_DistY * tmp_DistY)

                    tmp_uLedInfo.uLedData = cnt_h + 1
                    tmp_uLedInfo.uLedGate = cnt_v + 1

                    ' 逆時針為正，順時針為負
                    tmp_uLedInfo.uLedMachinX = CrossMarkMachineX + tmp_LedRadius * Math.Sin((tmp_LedAngle + tmp_PanelAngle + tmp_PreAngle) * tmp_ToRadian)
                    tmp_uLedInfo.uLedMachinY = CrossMarkMachineY + tmp_LedRadius * Math.Cos((tmp_LedAngle + PanelAngle + tmp_PreAngle) * tmp_ToRadian)

                    ' 計算CcdIndex
                    tmp_Quo = (tmp_uLedInfo.uLedMachinX - FirstFrameMachineX) / CcdPitchX
                    tmp_Rem = (tmp_uLedInfo.uLedMachinX - FirstFrameMachineX) - (tmp_Quo * CcdPitchX)

                    If tmp_Quo > 0 And tmp_Rem > CcdOverlapX Then
                        tmp_uLedInfo.uLedCcdIndex = tmp_Quo
                    ElseIf tmp_Quo > 0 And tmp_Rem > CcdOverlapX Then
                        tmp_uLedInfo.uLedCcdIndex = tmp_Quo - 1
                    Else
                        tmp_uLedInfo.uLedCcdIndex = 0
                    End If

                    ' 計算ScanIndex
                    tmp_Quo = Math.Floor((tmp_uLedInfo.uLedMachinX - FirstFrameMachineX - tmp_uLedInfo.uLedCcdIndex * CcdPitchX) / FramePitchX)
                    tmp_Rem = (tmp_uLedInfo.uLedMachinX - FirstFrameMachineX - tmp_uLedInfo.uLedCcdIndex * CcdPitchX) - (tmp_Quo * FramePitchX)

                    If tmp_Quo > 0 And tmp_Rem > FrameOverlapX Then
                        tmp_uLedInfo.uLedScanIndex = tmp_Quo
                    ElseIf tmp_Quo > 0 And tmp_Rem <= FrameOverlapX Then
                        tmp_uLedInfo.uLedScanIndex = tmp_Quo - 1
                    Else
                        tmp_uLedInfo.uLedScanIndex = 0
                    End If

                    ' 計算FrameIndex
                    tmp_Quo = Math.Floor((tmp_uLedInfo.uLedMachinY - FirstFrameMachIneY) / FramePitchY)
                    tmp_Rem = (tmp_uLedInfo.uLedMachinY - FirstFrameMachIneY) - (tmp_Quo * FramePitchY)

                    If tmp_Quo > 0 And tmp_Rem > FrameOverlapY Then
                        tmp_uLedInfo.uLedFrameIndex = tmp_Quo
                    ElseIf tmp_Quo > 0 And tmp_Rem <= FrameOverlapY Then
                        tmp_uLedInfo.uLedFrameIndex = tmp_Quo - 1
                    Else
                        tmp_uLedInfo.uLedFrameIndex = 0
                    End If

                    ' 每個uLED在當下畫面離左上角實際距離
                    tmp_DistX = tmp_uLedInfo.uLedMachinX - FirstFrameMachineX - tmp_uLedInfo.uLedCcdIndex * CcdPitchX - tmp_uLedInfo.uLedScanIndex * FramePitchX
                    tmp_DistY = tmp_uLedInfo.uLedMachinY - FirstFrameMachIneY - tmp_uLedInfo.uLedFrameIndex * FramePitchY

                    ' 每個uLED在影像的位置，角度順時針為正，逆時針角度為負。
                    tmp_uLedInfo.uLedImageX = (tmp_DistX * Math.Cos(AxisAngle * tmp_ToRadian) - tmp_DistY * Math.Sin(AxisAngle * tmp_ToRadian)) / LesOptResX
                    tmp_uLedInfo.uLedImageY = (tmp_DistX * Math.Sin(AxisAngle * tmp_ToRadian) + tmp_DistY * Math.Cos(AxisAngle * tmp_ToRadian)) / LesOptResY

                    If tmp_uLedInfo.uLedImageX > 0 AndAlso tmp_uLedInfo.uLedImageY > 0 Then
                        Me.uLedInfoList.Add(tmp_uLedInfo)
                    End If

                Next
            Next
        Next

    End Sub

End Class

Public Class uLedInfo

    Private Data As UInteger
    Private Gate As UInteger
    Private Channel As Integer

    Private CcdIndex As Integer
    Private ScanIndex As Integer
    Private FrameIndex As Integer

    Private ImageX As Integer
    Private ImageY As Integer

    Private MachinX As Double
    Private MachinY As Double

    Public Property uLedData As UInteger
        Get
            Return Me.Data
        End Get
        Set(value As UInteger)
            Me.Data = value
        End Set
    End Property

    Public Property uLedGate As UInteger
        Get
            Return Me.Gate
        End Get
        Set(value As UInteger)
            Me.Data = value
        End Set
    End Property

    Public Property uLedChannel As UInteger
        Get
            Return Me.Channel
        End Get
        Set(value As UInteger)
            Me.Channel = value
        End Set
    End Property

    Public Property uLedCcdIndex As UInteger
        Get
            Return Me.CcdIndex
        End Get
        Set(value As UInteger)
            Me.CcdIndex = value
        End Set
    End Property

    Public Property uLedScanIndex As UInteger
        Get
            Return Me.ScanIndex
        End Get
        Set(value As UInteger)
            Me.ScanIndex = value
        End Set
    End Property

    Public Property uLedFrameIndex As UInteger
        Get
            Return Me.FrameIndex
        End Get
        Set(value As UInteger)
            Me.FrameIndex = value
        End Set
    End Property

    Public Property uLedImageX As UInteger
        Get
            Return Me.ImageX
        End Get
        Set(value As UInteger)
            Me.ImageX = value
        End Set
    End Property
    Public Property uLedImageY As UInteger
        Get
            Return Me.ImageY
        End Get
        Set(value As UInteger)
            Me.ImageY = value
        End Set
    End Property

    Public Property uLedMachinX As UInteger
        Get
            Return Me.MachinX
        End Get
        Set(value As UInteger)
            Me.MachinX = value
        End Set
    End Property

    Public Property uLedMachinY As UInteger
        Get
            Return Me.MachinY
        End Get
        Set(value As UInteger)
            Me.MachinY = value
        End Set
    End Property

    Public Sub New()
        Me.Data = 0
        Me.Gate = 0
        Me.Channel = -1

        Me.CcdIndex = -1
        Me.ScanIndex = -1
        Me.FrameIndex = -1

        Me.ImageX = -1
        Me.ImageY = -1

        Me.MachinX = 0.0
        Me.MachinY = 0.0
    End Sub

End Class

