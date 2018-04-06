'############## Jan 11,2011-Class Description Start ############################
'-- This class consider as container of Information
'-- It is used to hold information during the application life cycle
'-- to store basic information it exposes some constructor
'-- to store more than that it contains one hashtable
'-- Value can be added in the hashtable using item property
'-- It also exposes some properties to get values and also some times set values
'############## Jan 11,2011-Class Description End   ############################
Public Class UserContext
    Private UsrCrd As UserCredential
    Private ADUser As StructADCred
    Private selectedPlan As String
    Private MACAddress As String
    Private requestPage As String
    Private nasId As String
    Private AccCode As String
    Private Others As New Hashtable

    Public Sub New(ByVal usrcred As UserCredential, ByVal planid As String, ByVal httpReq As HttpRequest)
        'For EnCryption Changes Start ------------------------------
        Dim commonFun As PMSCommonFun
        commonFun = PMSCommonFun.getInstance
        UsrCrd.usrId = Replace(usrcred.usrId, "'", "")
        UsrCrd.passwd = Replace(usrcred.passwd, "'", "")
        UsrCrd.ACCID = usrcred.ACCID
        UsrCrd.GuestID = usrcred.GuestID
        selectedPlan = planid
        Dim qs As String = httpReq.QueryString("encry")
        'For EnCryption Changes End ------------------------------
        MACAddress = commonFun.DecrptQueryString("MA", qs)
        requestPage = commonFun.DecrptQueryString("OS", qs)
        nasId = commonFun.DecrptQueryString("UI", qs)
        'For EnCryption Changes End ------------------------------
    End Sub
    Public Sub New(ByVal usrcred As UserCredential, ByVal httpReq As HttpRequest)
        'For EnCryption Changes Start ------------------------------
        Dim commonFun As PMSCommonFun
        commonFun = PMSCommonFun.getInstance
        Dim qs As String = httpReq.QueryString("encry")
        'For EnCryption Changes End ------------------------------
        UsrCrd.usrId = Replace(usrcred.usrId, "'", "")
        UsrCrd.passwd = Replace(usrcred.passwd, "'", "")
        UsrCrd.ACCID = usrcred.ACCID
        UsrCrd.GuestID = usrcred.GuestID
        selectedPlan = -1
        MACAddress = commonFun.DecrptQueryString("MA", qs)
        requestPage = commonFun.DecrptQueryString("OS", qs)
        nasId = commonFun.DecrptQueryString("UI", qs)
        'For EnCryption Changes End ------------------------------
    End Sub
    Public Sub New(ByVal uId As String, ByVal pwd As String, ByVal ACCID As Long, ByVal GuestID As Long, ByVal httpReq As HttpRequest)

        'For EnCryption Changes Start ------------------------------
        Dim commonFun As PMSCommonFun
        commonFun = PMSCommonFun.getInstance
        Dim qs As String = httpReq.QueryString("encry")
        'For EnCryption Changes End ------------------------------

        UsrCrd.usrId = Replace(uId, "'", "")
        UsrCrd.passwd = Replace(pwd, "'", "")
        UsrCrd.ACCID = ACCID
        UsrCrd.GuestID = GuestID
        selectedPlan = -1
        MACAddress = commonFun.DecrptQueryString("MA", qs)
        requestPage = commonFun.DecrptQueryString("OS", qs)
        nasId = commonFun.DecrptQueryString("UI", qs)
        'For EnCryption Changes End ------------------------------
    End Sub
    Public Sub New(ByVal uId As String, ByVal pwd As String, ByVal ACCID As Long, ByVal GuestID As Long, ByVal planId As String, ByVal pmsType As PMSNAMES, ByVal pmsVer As String, ByVal httpReq As HttpRequest)

        'For EnCryption Changes Start ------------------------------
        Dim commonFun As PMSCommonFun
        commonFun = PMSCommonFun.getInstance
        Dim qs As String = httpReq.QueryString("encry")
        'For EnCryption Changes End ------------------------------
        UsrCrd.usrId = Replace(uId, "'", "")
        UsrCrd.passwd = Replace(pwd, "'", "")
        UsrCrd.ACCID = ACCID
        UsrCrd.GuestID = GuestID
        selectedPlan = planId
        MACAddress = commonFun.DecrptQueryString("MA", qs)
        requestPage = commonFun.DecrptQueryString("OS", qs)
        nasId = commonFun.DecrptQueryString("UI", qs)
        'For EnCryption Changes End ------------------------------
    End Sub

    'Set and Read usrId Variable Start
    ReadOnly Property userId() As String
        Get
            Return UsrCrd.usrId
        End Get
    End Property

    ReadOnly Property AccessID() As Long
        Get
            Return UsrCrd.ACCID
        End Get
    End Property

    Property GuestID() As Long
        Get
            Return UsrCrd.GuestID
        End Get
        Set(ByVal value As Long)
            UsrCrd.GuestID = value
        End Set
    End Property

    ReadOnly Property roomNo() As String
        Get
            Return UsrCrd.usrId
        End Get
    End Property

    'Set and Read Passwd Variable Start
    ReadOnly Property password() As String
        Get
            Return UsrCrd.passwd
        End Get
    End Property

    ReadOnly Property guestName() As String
        Get
            Return UsrCrd.passwd
        End Get
    End Property
    'Set and Read Passwd Variable End
    Property selectedPlanId() As String
        Get
            Return selectedPlan
        End Get
        Set(ByVal Value As String)
            selectedPlan = Value
        End Set
    End Property

    ReadOnly Property machineId() As String
        Get
            Return MACAddress
        End Get
    End Property

    ReadOnly Property requestedPage() As String
        Get
            Return requestPage
        End Get
    End Property

    ReadOnly Property nomadixId() As String
        Get
            Return nasId
        End Get
    End Property

    ReadOnly Property ADUserID() As String
        Get
            Return ADUser.ADuserid
        End Get
    End Property

    ReadOnly Property ADPassword() As String
        Get
            Return ADUser.ADuserid
        End Get
    End Property

    Property item(ByVal strKey As String)
        Get
            Return Others.Item(strKey)
        End Get
        Set(ByVal Value)
            If Others.ContainsKey(strKey) Then
                Others.Item(strKey) = Value
            Else
                Others.Add(strKey, Value)
            End If
        End Set
    End Property
End Class
