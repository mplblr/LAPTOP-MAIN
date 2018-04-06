Public Class WirelessDetails
    Private _WireLessMAC As String = ""
    Private _HitCount As Integer = 0
    Private _AccessCode As String = ""
    Public Sub New(ByVal WirelessMAC As String, ByVal HitCount As Integer)
        _WireLessMAC = WirelessMAC
        _HitCount = HitCount
    End Sub
    Public Sub New(ByVal AccCode As String)
        _WireLessMAC = ""
        _HitCount = 0
        _AccessCode = AccCode
    End Sub
    ReadOnly Property WireLessMAC() As String
        Get
            Return _WireLessMAC
        End Get
    End Property
    ReadOnly Property AccessCode() As String
        Get
            Return _AccessCode
        End Get
    End Property
    Property NoOFHIT() As Integer
        Get
            Return _HitCount
        End Get
        Set(ByVal Value As Integer)
            _HitCount = Value
        End Set
    End Property

End Class
