Imports System.Data.Common
Imports ITCCORE.Microsense.CodeBase

Public Class GeneralDB
    Public Shared Function ResolveNull(ByVal val As Object) As String

        If val.Equals(DBNull.Value) Then
            Return ""
        Else
            Return val.ToString().Trim()
        End If

    End Function
End Class

Public Class DiscountData

    Private _profileType As String
    Private _columnToSearch As String
    Private _dataToSearch As String
    Private _discount As Double = 0
    Private _complimentaryPlan As Integer = 0
    Private _hidePlans As Boolean = False
    Private _autoConnectPlan As Integer = 0
    Private _autoConnectDiscount As Double = 0

    Public Property ProfileType() As String
        Get
            Return _profileType
        End Get
        Private Set(ByVal value As String)
            _profileType = value
        End Set
    End Property

    Public Property ColumnToSearch() As String
        Get
            Return _columnToSearch
        End Get
        Private Set(ByVal value As String)
            _columnToSearch = value
        End Set
    End Property

    Public Property DataToSearch() As String
        Get
            Return _dataToSearch
        End Get
        Private Set(ByVal value As String)
            _dataToSearch = value
        End Set
    End Property

    Public Property Discount() As Double
        Get
            Return _discount
        End Get
        Private Set(ByVal value As Double)
            _discount = value
        End Set
    End Property

    Public Property ComplimentaryPlan() As Integer
        Get
            Return _complimentaryPlan
        End Get
        Private Set(ByVal value As Integer)
            _complimentaryPlan = value
        End Set
    End Property

    Public Property HidePlans() As Boolean
        Get
            Return _hidePlans
        End Get
        Private Set(ByVal value As Boolean)
            _hidePlans = value
        End Set
    End Property


    Public Property AutoConnectPlan() As Integer
        Get
            Return _autoConnectPlan
        End Get
        Private Set(ByVal value As Integer)
            _autoConnectPlan = value
        End Set
    End Property

    Public Property AutoConnectDiscount() As Double
        Get
            Return _autoConnectDiscount
        End Get
        Private Set(ByVal value As Double)
            _autoConnectDiscount = value
        End Set
    End Property

    Public Sub New(ByVal _profileType As String, ByVal _columnToSearch As String, ByVal _dataToSearch As String)
        Me.ProfileType = _profileType
        Me.ColumnToSearch = _columnToSearch
        Me.DataToSearch = _dataToSearch
        SetDiscountData()
    End Sub

    Private Sub SetDiscountData()

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = String.Format("Select Top 1 Discount, ComplimentaryPlan, HidePlans, AutoConnectPlan, AutoConnectDiscount From DiscountMappings1 Where Active = 1 And [Type] = @ProfileType And {0} = @SearchData", Me.ColumnToSearch)

            DatabaseUtil.AddInputParameter(com, "@ProfileType", DbType.String, Me.ProfileType)
            DatabaseUtil.AddInputParameter(com, "@SearchData", DbType.String, Me.DataToSearch)


            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then

                Dim discountStr As String = GeneralDB.ResolveNull(result.Rows(0)("Discount"))
                Double.TryParse(discountStr, Me.Discount)

                Dim compPlanStr As String = GeneralDB.ResolveNull(result.Rows(0)("ComplimentaryPlan"))
                Integer.TryParse(compPlanStr, Me.ComplimentaryPlan)

                'Dim hidePlanStr As String = GeneralDB.ResolveNull(result.Rows(0)("HidePlans"))
                'Dim hidePlanVal As Integer = 0
                'Integer.TryParse(hidePlanStr, hidePlanVal)

                'If hidePlanVal = 1 Then
                '    Me.HidePlans = True
                'End If

                Me.HidePlans = False

                Dim autoConnectDiscountStr As String = GeneralDB.ResolveNull(result.Rows(0)("AutoConnectDiscount"))
                Double.TryParse(autoConnectDiscountStr, Me.AutoConnectDiscount)

                Dim autoConnectPlanStr As String = GeneralDB.ResolveNull(result.Rows(0)("AutoConnectPlan"))
                Integer.TryParse(autoConnectPlanStr, Me.AutoConnectPlan)

            End If


        Catch ex As Exception
            objLog.writeExceptionLogFile("SetDiscountDataExp", ex)
        End Try

    End Sub

End Class

Public Class AutoConnect
    Private _autoConnectPlan As Integer
    Private _autoConnectDiscount As Double = 0

    Public Property AutoConnectPlan() As Integer
        Get
            Return _autoConnectPlan
        End Get
        Private Set(ByVal value As Integer)
            _autoConnectPlan = value
        End Set
    End Property

    Public Property AutoConnectDiscount() As Double
        Get
            Return _autoConnectDiscount
        End Get
        Private Set(ByVal value As Double)
            _autoConnectDiscount = value
        End Set
    End Property

    Public Sub New(ByVal _autoConnectPlan As Integer, ByVal _autoConnectDiscount As Double)
        Me.AutoConnectPlan = _autoConnectPlan
        Me.AutoConnectDiscount = _autoConnectDiscount
    End Sub

End Class

Public Class GuestProfile

    Private _guestID As String = ""
    Private _accessID As String = ""
    Private _guestRoomNo As String = ""
    Private _guestRegCode As String = ""
    Private _guestVIPStatus As String = ""
    Private _guestTitle As String = ""
    Private _guestFirstName As String = ""
    Private _guestLastName As String = ""
    Private _guestChkInTime As String = ""
    Private _guestExpChkOutTime As String = ""
    Private _daysStay As Integer = 0
    Private _lsgDays As Integer = 7 'Default value
    Private _configuredLSGDiscount As Double = 0
    Private _defaultCompPlan As Integer = 2
    Private _defaultDiscountPlan As Integer = 2
    Private _defaultAutoConnectDaysForDiscount As Integer = 1

    Private _blockCode As String = ""
    Private _companyCode As String = ""
    Private _membershipType As String = ""
    Private _membershipLevel As String = ""
    Private _membershipName As String = ""
    Private _rateCode As String = ""
    Private _roomCategory As String = ""
    Private _roomClass As String = ""
    Private _privilegeCode As String = ""

    Private _blockCodeDiscount As Double = 0
    Private _companyCodeDiscount As Double = 0
    Private _membershipTypeDiscount As Double = 0
    Private _membershipLevelDiscount As Double = 0
    Private _rateCodeDiscount As Double = 0
    Private _roomCategoryDiscount As Double = 0
    Private _roomClassDiscount As Double = 0
    Private _lsgDiscount As Double = 0

    Private _complimentaryPlan As Integer
    Private _hidePlans As Boolean

    Private _discountApplicable As Double = 0
    Private _discountTypeApplicable As String = ""
    Private _complimentaryTypeApplicable As String = ""

    Private _discountInfoList As New List(Of String)
    Private _displayList As New List(Of String)

    Private _complimentaryConsiderable As Boolean

    '------- Exceptional case where guest need to be autoconnected for a particular plan
    Private _exceptionalAutoConnect As AutoConnect
    Public Property ExceptionalAutoConnect() As AutoConnect
        Get
            Return _exceptionalAutoConnect
        End Get
        Private Set(ByVal value As AutoConnect)
            _exceptionalAutoConnect = value
        End Set
    End Property
    '------- Exceptional case where guest need to be autoconnected for a particular plan

    Public Property GuestID() As String
        Get
            Return _guestID
        End Get
        Private Set(ByVal value As String)
            _guestID = value
        End Set
    End Property

    Public Property AccessID() As String
        Get
            Return _accessID
        End Get
        Private Set(ByVal value As String)
            _accessID = value
        End Set
    End Property

    Public Property GuestRoomNo() As String
        Get
            Return _guestRoomNo
        End Get
        Private Set(ByVal value As String)
            _guestRoomNo = value
        End Set
    End Property

    Public Property GuestRegCode() As String
        Get
            Return _guestRegCode
        End Get
        Private Set(ByVal value As String)
            _guestRegCode = value
        End Set
    End Property

    Public Property GuestVIPStatus() As String
        Get
            Return _guestVIPStatus
        End Get
        Private Set(ByVal value As String)
            _guestVIPStatus = value
        End Set
    End Property

    Public Property GuestTitle() As String
        Get
            Return _guestTitle
        End Get
        Private Set(ByVal value As String)
            _guestTitle = value
        End Set
    End Property

    Public Property GuestFirstName() As String
        Get
            Return _guestFirstName
        End Get
        Private Set(ByVal value As String)
            _guestFirstName = value
        End Set
    End Property

    Public Property GuestLastName() As String
        Get
            Return _guestLastName
        End Get
        Private Set(ByVal value As String)
            _guestLastName = value
        End Set
    End Property

    Public ReadOnly Property GuestFullName()
        Get
            Dim name As String = Me.GuestTitle.Trim() + " " + Me.GuestFirstName.Trim() + " " + Me.GuestLastName.Trim()
            name = name.Replace("  ", "")
            name = name.Trim()
            Return name
        End Get
    End Property

    Public Property GuestChkInTime() As String
        Get
            Return _guestChkInTime
        End Get
        Private Set(ByVal value As String)
            _guestChkInTime = value
        End Set
    End Property

    Public Property GuestExpChkOutTime() As String
        Get
            Return _guestExpChkOutTime
        End Get
        Private Set(ByVal value As String)
            _guestExpChkOutTime = value
        End Set
    End Property

    Public Property DaysStay() As Integer
        Get
            Return _daysStay
        End Get
        Private Set(ByVal value As Integer)
            _daysStay = value
        End Set
    End Property

    Public Property MinDaysForLSG() As Integer
        Get
            Return _lsgDays
        End Get
        Private Set(ByVal value As Integer)
            _lsgDays = value
        End Set
    End Property

    Public Property ConfiguredLSGDiscount() As Double
        Get
            Return _configuredLSGDiscount
        End Get
        Private Set(ByVal value As Double)
            _configuredLSGDiscount = value
        End Set
    End Property

    Public Property DefaultComplementaryPlan() As Integer
        Get
            Return _defaultCompPlan
        End Get
        Private Set(ByVal value As Integer)
            _defaultCompPlan = value
        End Set
    End Property

    Public Property DefaultDiscountPlan() As Integer
        Get
            Return _defaultDiscountPlan
        End Get
        Private Set(ByVal value As Integer)
            _defaultDiscountPlan = value
        End Set
    End Property

    Public Property DefaultAutoConnectDaysForDiscount() As Integer
        Get
            Return _defaultAutoConnectDaysForDiscount
        End Get
        Private Set(ByVal value As Integer)
            _defaultAutoConnectDaysForDiscount = value
        End Set
    End Property


    Public Property BlockCode() As String
        Get
            Return _blockCode
        End Get
        Private Set(ByVal value As String)
            _blockCode = value
        End Set
    End Property

    Public Property CompanyCode() As String
        Get
            Return _companyCode
        End Get
        Private Set(ByVal value As String)
            _companyCode = value
        End Set
    End Property

    Public Property MembershipType() As String
        Get
            Return _membershipType
        End Get
        Private Set(ByVal value As String)
            _membershipType = value
        End Set
    End Property

    Public Property MembershipLevel() As String
        Get
            Return _membershipLevel
        End Get
        Private Set(ByVal value As String)
            _membershipLevel = value
        End Set
    End Property

    Public Property MembershipName() As String
        Get
            Return _membershipName
        End Get
        Private Set(ByVal value As String)
            _membershipName = value
        End Set
    End Property

    Public Property RateCode() As String
        Get
            Return _rateCode
        End Get
        Private Set(ByVal value As String)
            _rateCode = value
        End Set
    End Property

    Public Property RoomCategory() As String
        Get
            Return _roomCategory
        End Get
        Private Set(ByVal value As String)
            _roomCategory = value
        End Set
    End Property

    Public Property RoomClass() As String
        Get
            Return _roomClass
        End Get
        Private Set(ByVal value As String)
            _roomClass = value
        End Set
    End Property

    Public Property PrivilegeCode() As String
        Get
            Return _privilegeCode
        End Get
        Private Set(ByVal value As String)
            _privilegeCode = value
        End Set
    End Property


    'Discount data section

    Public Property BlockCodeDiscount() As Double
        Get
            Return _blockCodeDiscount
        End Get
        Private Set(ByVal value As Double)
            _blockCodeDiscount = value
        End Set
    End Property

    Public Property CompanyCodeDiscount() As Double
        Get
            Return _companyCodeDiscount
        End Get
        Private Set(ByVal value As Double)
            _companyCodeDiscount = value
        End Set
    End Property

    Public Property MembershipTypeDiscount() As Double
        Get
            Return _membershipTypeDiscount
        End Get
        Private Set(ByVal value As Double)
            _membershipTypeDiscount = value
        End Set
    End Property

    Public Property MembershipLevelDiscount() As Double
        Get
            Return _membershipLevelDiscount
        End Get
        Private Set(ByVal value As Double)
            _membershipLevelDiscount = value
        End Set
    End Property

    Public Property RateCodeDiscount() As Double
        Get
            Return _rateCodeDiscount
        End Get
        Private Set(ByVal value As Double)
            _rateCodeDiscount = value
        End Set
    End Property

    Public Property RoomCategoryDiscount() As Double
        Get
            Return _roomCategoryDiscount
        End Get
        Private Set(ByVal value As Double)
            _roomCategoryDiscount = value
        End Set
    End Property

    Public Property RoomClassDiscount() As Double
        Get
            Return _roomClassDiscount
        End Get
        Private Set(ByVal value As Double)
            _roomClassDiscount = value
        End Set
    End Property

    Public Property LSGDiscount() As Double
        Get
            Return _lsgDiscount
        End Get
        Private Set(ByVal value As Double)
            _lsgDiscount = value
        End Set
    End Property

    Public Property ComplimentaryPlan() As Integer
        Get
            Return _complimentaryPlan
        End Get
        Private Set(ByVal value As Integer)
            _complimentaryPlan = value
        End Set
    End Property

    Public Property HidePlans() As Boolean
        Get
            Return _hidePlans
        End Get
        Private Set(ByVal value As Boolean)
            _hidePlans = value
        End Set
    End Property

    Public Property DiscountApplicable() As Double
        Get
            Return _discountApplicable
        End Get
        Private Set(ByVal value As Double)
            _discountApplicable = value
        End Set
    End Property

    Public Property DiscountTypeApplicable() As String
        Get
            Return _discountTypeApplicable
        End Get
        Private Set(ByVal value As String)
            _discountTypeApplicable = value
        End Set
    End Property

    Public Property ComplimentaryTypeApplicable() As String
        Get
            Return _complimentaryTypeApplicable
        End Get
        Private Set(ByVal value As String)
            _complimentaryTypeApplicable = value
        End Set
    End Property

    Public ReadOnly Property DiscountInfoList() As List(Of String)
        Get
            Return _discountInfoList
        End Get
    End Property

    Public ReadOnly Property DisplayList() As List(Of String)
        Get
            Return _displayList
        End Get
    End Property

    Public ReadOnly Property IsComplimentaryConsiderable() As Boolean
        Get
            Return _complimentaryConsiderable
        End Get
    End Property

    Public Sub New(ByVal _guestID As String, ByVal _accessID As String)

        '------- Exceptional case where guest need to be autoconnected for a particular plan
        Me.ExceptionalAutoConnect = Nothing
        '------- Exceptional case where guest need to be autoconnected for a particular plan

        Me.GuestID = _guestID
        Me.AccessID = _accessID
        SetBasicGuestProfile()
        SetMembershipName()
        SetLSGProfile()
        SetExtendedGuestProfile()

        If Me.ComplimentaryPlan > 0 Then
            _complimentaryConsiderable = True
        End If
        _complimentaryConsiderable = IsComplimentaryToConsider()

        SetDiscountInfo()
        SetDisplayInfo()


        Try
            'If secoundCoupon(_guestID, _accessID) = 1 Then

            '    Try
            '        Dim objlog As LoggerService
            '        objlog = LoggerService.gtInstance
            '        objlog.write2LogFile("Test" & _guestID, "COMP success")
            '    Catch ex As Exception

            '    End Try



            '    _complimentaryConsiderable = False
            '    _discountApplicable = 0

            'End If
        Catch ex As Exception

        End Try

        Try
            'If secoundCouponD(_guestID, _accessID) = 1 Then
            '    Try
            '        Dim objlog As LoggerService
            '        objlog = LoggerService.gtInstance
            '        objlog.write2LogFile("Test1" & _guestID, "DISC success")
            '    Catch ex As Exception

            '    End Try
            '    _complimentaryConsiderable = False
            '    _discountApplicable = 0

            'End If
        Catch ex As Exception

        End Try







    End Sub

    Public Function secoundCoupon(ByVal guestid As Long, ByVal accid As Long) As Integer
        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "select * from guest where guestcfa7='COMP' and guestid=@GuestID"
            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, guestid)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then

                Try
                    ' objlog.write2LogFile("Test" & _guestID, "GuestId" & guestid & "Accid" & accid & "Comp Found")
                Catch ex As Exception

                End Try



                Dim sql_query As String = ""
                sql_query = "SELECT ID,ACC_ROOM, ACC_GuestRegCode,GuestId FROM ACCESSUSAGE INNER JOIN Guest ON ACCESSUSAGE.ACC_GuestRegCode=Guest.GuestRegCode AND Guest.GuestStatus='A'" & _
                             vbCrLf & " WHERE Guest.guestid =" & guestid & " and  ID <>" & accid

                com.CommandText = sql_query
                'Check By ID END ##############

                Dim result1 As DataTable = DatabaseUtil.ExecuteSelect(com)


                If result1.Rows.Count > 0 Then

                    Dim straccid As String = ""

                    straccid = result1.Rows(0)(0).ToString()

                    Try
                        '  objlog.write2LogFile("Test" & _guestID, "GuestId" & guestid & "Accid" & accid & "First Coupon  Found" & "accid=" & straccid)
                    Catch ex As Exception

                    End Try


                    Dim sq As String = ""

                    sq = "Select * from bill where BillPostedAmount =0 and BillGrCId = " & guestid & "   And BillAccID =" & straccid & "  And BillType = 0"
                    com.CommandText = sq

                    Dim result2 As DataTable = DatabaseUtil.ExecuteSelect(com)

                    If result2.Rows.Count > 0 Then


                        Try
                            'objlog.write2LogFile("Test" & _guestID, "GuestId" & guestid & "Accid" & accid & "First Coupon  used" & "accid=" & straccid)
                        Catch ex As Exception

                        End Try


                        Return 1

                    Else
                        Return 0
                    End If


                Else

                    Return 0

                End If

            Else

                Return 0

            End If

        Catch ex As Exception
            Return 0
        End Try







    End Function


    Public Function secoundCouponD(ByVal guestid As Long, ByVal accid As Long) As Integer
        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "select * from guest where guestcfa7 like 'Disc%' and guestid=@GuestID"
            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, guestid)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then

                Try
                    'objlog.write2LogFile("Test1" & _guestID, "GuestId" & guestid & "Accid" & accid & "Comp Found")
                Catch ex As Exception

                End Try



                Dim sql_query As String = ""
                sql_query = "SELECT ID,ACC_ROOM, ACC_GuestRegCode,GuestId FROM ACCESSUSAGE INNER JOIN Guest ON ACCESSUSAGE.ACC_GuestRegCode=Guest.GuestRegCode AND Guest.GuestStatus='A'" & _
                             vbCrLf & " WHERE Guest.guestid =" & guestid & " and  ID <>" & accid

                com.CommandText = sql_query
                'Check By ID END ##############

                Dim result1 As DataTable = DatabaseUtil.ExecuteSelect(com)


                If result1.Rows.Count > 0 Then

                    Dim straccid As String = ""

                    straccid = result1.Rows(0)(0).ToString()

                    Try
                        ' objlog.write2LogFile("Test1" & _guestID, "GuestId" & guestid & "Accid" & accid & "First Coupon  Found" & "accid=" & straccid)
                    Catch ex As Exception

                    End Try

                    Dim str As String = 0
                    Str = "Select Top 1 Case When (BillAmount * BilltmpNoofDays) > BillPostedAmount Then 1 Else 0 End As DiscountApplied " & vbNewLine & _
                             "From Bill " & vbNewLine & _
                             "Where BillGrCId = " & guestid & "   And BillAccID =" & straccid & "  And BillType = 0  " & vbNewLine & _
                             "Order By BillID Desc"

                    Try
                        'objlog.write2LogFile("Test1", str)
                    Catch ex As Exception

                    End Try

                    com.CommandText = str
                    Dim result2 As DataTable

                    Try
                        result2 = DatabaseUtil.ExecuteSelect(com)
                    Catch ex As Exception
                        objlog.write2LogFile("Test1_err", ex.Message)
                    End Try





                    If result2.Rows.Count > 0 Then
                        Try
                            ' objlog.write2LogFile("Test1", "Bill found")
                        Catch ex As Exception

                        End Try


                        Dim discountApplied As String = result2.Rows(0)("DiscountApplied").ToString()
                        Try
                            'objlog.write2LogFile("Test1" & _guestID, "GuestId" & guestid & "Accid" & accid & "disc First Coupon  used" & "accid=" & straccid & "discountApplied" & discountApplied)
                        Catch ex As Exception

                        End Try

                        If discountApplied = "1" Then
                            Return 1
                        Else
                            Return 0
                        End If






                    Else
                        Return 0
                    End If


                Else

                    Return 0

                End If

            Else

                Return 0

            End If

        Catch ex1 As Exception
            Return 0
        End Try







    End Function





    Private Sub SetDiscountInfo()

        Dim considerComplimentary As Boolean = Me.IsComplimentaryConsiderable
        If Me.DiscountApplicable > 0 Then

            If (Me.ComplimentaryPlan) > 0 And (considerComplimentary = True) Then
                If Me.HidePlans = False Then
                    _discountInfoList.Insert(0, Me.DiscountApplicable.ToString() + "% Discount on Chargeable plans")
                End If
            Else
                _discountInfoList.Insert(0, Me.DiscountApplicable.ToString() + "% Discount on selected plan")
            End If
        End If

        If Me.CompanyCode <> "" Then
            _discountInfoList.Insert(0, Me.CompanyCode)
        End If

    End Sub

    Private Sub SetDisplayInfo()

        If Me.MembershipName <> "" Then
            If Me.GuestVIPStatus <> "" Then
                _displayList.Add("(VIP) Member " + Me.MembershipName)
            Else
                _displayList.Add("Member " + Me.MembershipName)
            End If
        Else
            If Me.GuestVIPStatus <> "" Then
                _displayList.Add("(VIP)")
            End If
        End If

        Dim discountInfo As String = String.Join(" | ", Me.DiscountInfoList.ToArray())

        If discountInfo <> "" Then
            _displayList.Add(discountInfo)
        End If

    End Sub

    Private Function IsComplimentaryToConsider() As Boolean

        Dim objLog As LoggerService = LoggerService.gtInstance()
        If Me.ComplimentaryPlan > 0 Then

            If Me.AccessID = "" Then
                Return True
            End If

            Me.AccessID = Me.AccessID.Trim()

            Try
                Dim conn As DbConnection = DatabaseUtil.GetConnection()
                Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

                com.CommandText = "Select Top 1 BillACCId From Bill Where BillId In (Select Max(BillID) From BillGuestProfileMappings Where GuestID = @GuestID And AppliedDiscount = 100)"
                DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, Me.GuestID)

                Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

                If result.Rows.Count > 0 Then
                    Dim billAccID As String = GeneralDB.ResolveNull(result.Rows(0)("BillACCId"))

                    If Me.AccessID = billAccID Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Return True
                End If

            Catch ex As Exception
                objLog.writeExceptionLogFile("IsComplimentaryToConsiderExp", ex)
                Return True
            End Try
        Else
            Return False
        End If

    End Function

    Public Shared Sub LogBillWithGuestProfile(ByVal billID As String, ByVal planID As String, ByVal planAmount As String, ByVal profile As GuestProfile, ByVal appliedDiscount As Double, ByVal actionTaken As String)
        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Insert Into BillGuestProfileMappings (BillID, GuestID, GuestRoomNo, GuestRegCode, GuestInitial, GuestFirstName, GuestLastName, BillPlanID, PlanAmount, BlockCode, BlockCodeDiscount, CompanyCode, CompanyCodeDiscount, MembershipType, MembershipTypeDiscount, MembershipLevel, MembershipLevelDiscount, RateCode, RateCodeDiscount, RoomCategory, RoomCategoryDiscount, RoomClass, RoomClassDiscount, ComplementaryPlanID, GuestChkInTime, GuestExpChkOutTime, DaysStay, MinDaysForLSG, LSGDiscount, AppliedDiscount, ActionTaken) " & vbNewLine & _
                              "Values (@BillID, @GuestID, @GuestRoomNo, @GuestRegCode, @GuestInitial, @GuestFirstName, @GuestLastName, @BillPlanID, @PlanAmount, @BlockCode, @BlockCodeDiscount, @CompanyCode, @CompanyCodeDiscount, @MembershipType, @MembershipTypeDiscount, @MembershipLevel, @MembershipLevelDiscount, @RateCode, @RateCodeDiscount, @RoomCategory, @RoomCategoryDiscount, @RoomClass, @RoomClassDiscount, @ComplementaryPlanID, @GuestChkInTime, @GuestExpChkOutTime, @DaysStay, @MinDaysForLSG, @LSGDiscount, @AppliedDiscount, @ActionTaken)"

            DatabaseUtil.AddInputParameter(com, "@BillID", DbType.String, billID)
            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, profile.GuestID)
            DatabaseUtil.AddInputParameter(com, "@GuestRoomNo", DbType.String, profile.GuestRoomNo)
            DatabaseUtil.AddInputParameter(com, "@GuestRegCode", DbType.String, profile.GuestRegCode)
            DatabaseUtil.AddInputParameter(com, "@GuestInitial", DbType.String, profile.GuestTitle)
            DatabaseUtil.AddInputParameter(com, "@GuestFirstName", DbType.String, profile.GuestFirstName)
            DatabaseUtil.AddInputParameter(com, "@GuestLastName", DbType.String, profile.GuestLastName)
            DatabaseUtil.AddInputParameter(com, "@BillPlanID", DbType.String, planID)
            DatabaseUtil.AddInputParameter(com, "@PlanAmount", DbType.String, planAmount)
            DatabaseUtil.AddInputParameter(com, "@BlockCode", DbType.String, profile.BlockCode)
            DatabaseUtil.AddInputParameter(com, "@BlockCodeDiscount", DbType.String, profile.BlockCodeDiscount)
            DatabaseUtil.AddInputParameter(com, "@CompanyCode", DbType.String, profile.CompanyCode)
            DatabaseUtil.AddInputParameter(com, "@CompanyCodeDiscount", DbType.String, profile.CompanyCodeDiscount)
            DatabaseUtil.AddInputParameter(com, "@MembershipType", DbType.String, profile.MembershipType)
            DatabaseUtil.AddInputParameter(com, "@MembershipTypeDiscount", DbType.String, profile.MembershipTypeDiscount)
            DatabaseUtil.AddInputParameter(com, "@MembershipLevel", DbType.String, profile.MembershipLevel)
            DatabaseUtil.AddInputParameter(com, "@MembershipLevelDiscount", DbType.String, profile.MembershipLevelDiscount)
            DatabaseUtil.AddInputParameter(com, "@RateCode", DbType.String, profile.RateCode)
            DatabaseUtil.AddInputParameter(com, "@RateCodeDiscount", DbType.String, profile.RateCodeDiscount)
            DatabaseUtil.AddInputParameter(com, "@RoomCategory", DbType.String, profile.RoomCategory)
            DatabaseUtil.AddInputParameter(com, "@RoomCategoryDiscount", DbType.String, profile.RoomCategoryDiscount)
            DatabaseUtil.AddInputParameter(com, "@RoomClass", DbType.String, profile.RoomClass)
            DatabaseUtil.AddInputParameter(com, "@RoomClassDiscount", DbType.String, profile.RoomClassDiscount)
            DatabaseUtil.AddInputParameter(com, "@ComplementaryPlanID", DbType.String, profile.ComplimentaryPlan)
            DatabaseUtil.AddInputParameter(com, "@GuestChkInTime", DbType.String, profile.GuestChkInTime)
            DatabaseUtil.AddInputParameter(com, "@GuestExpChkOutTime", DbType.String, profile.GuestExpChkOutTime)
            DatabaseUtil.AddInputParameter(com, "@DaysStay", DbType.String, profile.DaysStay)
            DatabaseUtil.AddInputParameter(com, "@MinDaysForLSG", DbType.String, profile.MinDaysForLSG)
            DatabaseUtil.AddInputParameter(com, "@LSGDiscount", DbType.String, profile.LSGDiscount)
            DatabaseUtil.AddInputParameter(com, "@AppliedDiscount", DbType.String, appliedDiscount)
            DatabaseUtil.AddInputParameter(com, "@ActionTaken", DbType.String, actionTaken)


            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("LogBillWithGuestProfileExp", ex)
        End Try
    End Sub

    Private Sub SetBasicGuestProfile()

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Select GuestRoomNo, GuestRegCode, GuestVIPStatus, GuestTitle, GuestFirstName, GuestName As GuestLastName, GuestChkInTime, GuestExpChkOutTime, " & vbNewLine & _
                                "	   dbo.GetNoOfDays(GuestChkInTime, GuestExpChkOutTime) As DaysStay, " & vbNewLine & _
                                "	   GuestCFA5 As BlockCode, GuestCFA3 As CompanyCode, GuestCFA1 As MembershipType, " & vbNewLine & _
                                "	   GuestCFA6 As MembershipLevel, GuestCFA0 As RateCode, GuestCFA4 As RoomCategory, " & vbNewLine & _
                                "	   GuestCFA7 As PrivilegeCode, '' As RoomClass " & vbNewLine & _
                                "From Guest Where GuestID = @GuestID"

            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, Me.GuestID)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Me.GuestRoomNo = GeneralDB.ResolveNull(result.Rows(0)("GuestRoomNo"))
                Me.GuestRegCode = GeneralDB.ResolveNull(result.Rows(0)("GuestRegCode"))
                Me.GuestVIPStatus = GeneralDB.ResolveNull(result.Rows(0)("GuestVIPStatus"))
                Me.GuestTitle = GeneralDB.ResolveNull(result.Rows(0)("GuestTitle"))
                Me.GuestFirstName = GeneralDB.ResolveNull(result.Rows(0)("GuestFirstName"))
                Me.GuestLastName = GeneralDB.ResolveNull(result.Rows(0)("GuestLastName"))
                Me.GuestChkInTime = GeneralDB.ResolveNull(result.Rows(0)("GuestChkInTime"))
                Me.GuestExpChkOutTime = GeneralDB.ResolveNull(result.Rows(0)("GuestExpChkOutTime"))

                Dim dayStayStr As String = GeneralDB.ResolveNull(result.Rows(0)("DaysStay"))
                Integer.TryParse(dayStayStr, Me.DaysStay)

                Me.BlockCode = GeneralDB.ResolveNull(result.Rows(0)("BlockCode"))
                Me.CompanyCode = GeneralDB.ResolveNull(result.Rows(0)("CompanyCode"))
                Me.MembershipType = GeneralDB.ResolveNull(result.Rows(0)("MembershipType"))
                Me.MembershipLevel = GeneralDB.ResolveNull(result.Rows(0)("MembershipLevel"))
                Me.RateCode = GeneralDB.ResolveNull(result.Rows(0)("RateCode"))
                Me.RoomCategory = GeneralDB.ResolveNull(result.Rows(0)("RoomCategory"))
                Me.RoomClass = GeneralDB.ResolveNull(result.Rows(0)("RoomClass"))
                Me.PrivilegeCode = GeneralDB.ResolveNull(result.Rows(0)("PrivilegeCode"))

            End If


        Catch ex As Exception
            objLog.writeExceptionLogFile("SetBasicGuestProfileExp", ex)
        End Try

    End Sub

    Private Sub SetLSGProfile()

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "Select (Select Variable_Value From Config Where Variable_Name = 'LSGDays1') As LSGDays, (Select Variable_Value From Config Where Variable_Name = 'LSGDiscount') As LSGDiscount, (Select Variable_Value From Config Where Variable_Name = 'DefaultComplimentaryPlan') As DefaultComplimentaryPlan, (Select Variable_Value From Config Where Variable_Name = 'DefaultDiscountPlan') As DefaultDiscountPlan, (Select Variable_Value From Config Where Variable_Name = 'AutoconnectDaysForDiscount') As AutoconnectDaysForDiscount"

            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Dim lsgDaysStr As String = GeneralDB.ResolveNull(result.Rows(0)("LSGDays"))
                Dim conLSGDinscountStr As String = GeneralDB.ResolveNull(result.Rows(0)("LSGDiscount"))
                Dim defaultCompPlanStr As String = GeneralDB.ResolveNull(result.Rows(0)("DefaultComplimentaryPlan"))
                Dim defaultDiscountPlanStr As String = GeneralDB.ResolveNull(result.Rows(0)("DefaultDiscountPlan"))
                Dim autoconnectDaysForDiscountStr As String = GeneralDB.ResolveNull(result.Rows(0)("AutoconnectDaysForDiscount"))

                Integer.TryParse(lsgDaysStr, Me.MinDaysForLSG)
                Double.TryParse(conLSGDinscountStr, Me.ConfiguredLSGDiscount)
                Integer.TryParse(defaultCompPlanStr, Me.DefaultComplementaryPlan)
                Integer.TryParse(defaultDiscountPlanStr, Me.DefaultDiscountPlan)
                Integer.TryParse(autoconnectDaysForDiscountStr, Me.DefaultAutoConnectDaysForDiscount)

                If Me.DaysStay >= Me.MinDaysForLSG Then
                    Me.LSGDiscount = Me.ConfiguredLSGDiscount
                End If


            End If

        Catch ex As Exception
            objLog.writeExceptionLogFile("SetLSGProfileExp", ex)
        End Try

    End Sub

    Private Function GetDicountByCode(ByVal privilegeCode As String) As Double

        Dim privileged_discount As Double = 0
        If Not String.IsNullOrEmpty(privilegeCode) Then
            privilegeCode = privilegeCode.Trim().ToLower()
            If privilegeCode = "comp" Then
                privileged_discount = 100
            Else
                Dim discountCode As String = privilegeCode.Replace("disc", "").Trim()
                Double.TryParse(discountCode, privileged_discount)

                If privileged_discount < 0 Then
                    privileged_discount = 0
                End If

                If privileged_discount >= 100 Then
                    privileged_discount = 0
                End If

            End If
        End If

        Return privileged_discount

    End Function




    Private Sub SetExtendedGuestProfile()

        Dim applicable_discount_type = ""

        Dim discDataList As New List(Of DiscountData)

        Me.DiscountApplicable = 0

        Dim discData As DiscountData
        'discData = New DiscountData("BlockCode", "Code", Me.BlockCode)
        'Me.BlockCodeDiscount = discData.Discount
        'If Me.BlockCodeDiscount > Me.DiscountApplicable Then
        '    Me.DiscountApplicable = Me.BlockCodeDiscount
        '    Me.DiscountTypeApplicable = "BlockCode"
        'End If
        'discDataList.Add(discData)

        'discData = New DiscountData("CompanyCode", "Description", Me.CompanyCode)
        'Me.CompanyCodeDiscount = discData.Discount
        'If Me.CompanyCodeDiscount > Me.DiscountApplicable Then
        '    Me.DiscountApplicable = Me.CompanyCodeDiscount
        '    Me.DiscountTypeApplicable = "CompanyCode"
        'End If
        'discDataList.Add(discData)

        'discData = New DiscountData("MembershipType", "Code", Me.MembershipType)
        'Me.MembershipTypeDiscount = discData.Discount
        'If Me.MembershipTypeDiscount > Me.DiscountApplicable Then
        '    Me.DiscountApplicable = Me.MembershipTypeDiscount
        '    Me.DiscountTypeApplicable = "MembershipType"
        'End If
        'discDataList.Add(discData)

        Try
            discData = New DiscountData("MembershipLevel", "Code", Me.MembershipLevel)

            Try

            Catch ex As Exception

            End Try

            Me.MembershipLevelDiscount = discData.Discount

            Try
                Dim ob As LoggerService
                ob = LoggerService.gtInstance

                ob.write2LogFile("MYLOG-" & _guestRoomNo, "MEMBERshp:- discData.Discount=" & discData.Discount & "Me.MembershipLevelDiscount" & Me.MembershipLevelDiscount)
            Catch ex As Exception

            End Try

            If Me.MembershipLevelDiscount > Me.DiscountApplicable Then
                Me.ComplimentaryPlan = 15
                Me.DiscountApplicable = Me.MembershipLevelDiscount
                Me.DiscountTypeApplicable = "MembershipLevel"
            End If

            Try
                Dim ob As LoggerService
                ob = LoggerService.gtInstance

                ob.write2LogFile("MYLOG-" & _guestRoomNo, "MEMBERshp:- discData.Discount=" & discData.Discount & "Me.DiscountApplicable" & Me.DiscountApplicable)
            Catch ex As Exception

            End Try


            discDataList.Add(discData)




        Catch ex As Exception

        End Try




        'discData = New DiscountData("RateCode", "Code", Me.RateCode)
        'Me.RateCodeDiscount = discData.Discount
        'If Me.RateCodeDiscount > Me.DiscountApplicable Then
        '    Me.DiscountApplicable = Me.RateCodeDiscount
        '    Me.DiscountTypeApplicable = "RateCode"
        'End If
        'discDataList.Add(discData)

        'discData = New DiscountData("RoomCategory", "Code", Me.RoomCategory)
        'Me.RoomCategoryDiscount = discData.Discount
        'If Me.RoomCategoryDiscount > Me.DiscountApplicable Then
        '    Me.DiscountApplicable = Me.RoomCategoryDiscount
        '    Me.DiscountTypeApplicable = "RoomCategory"
        'End If
        'discDataList.Add(discData)

        'discData = New DiscountData("RoomClass", "Code", Me.RoomClass)
        'Me.RoomClassDiscount = discData.Discount
        'If Me.RoomClassDiscount > Me.DiscountApplicable Then
        '    Me.DiscountApplicable = Me.RoomClassDiscount
        '    Me.DiscountTypeApplicable = "RoomClass"
        'End If
        'discDataList.Add(discData)

        'If Me.LSGDiscount > Me.DiscountApplicable Then
        '    Me.DiscountApplicable = Me.LSGDiscount
        '    Me.DiscountTypeApplicable = "LSG"
        'End If

        Dim typeForComplimentary As String = ""
        Dim typeForHidePlan As String = ""

        '---------- Check whether the checkin code implies complimentary access or discount
        '----------- If the checkin discount is bigger, it will be considered


        Try
            If website(_guestID) = 1 Then

                If Me.ComplimentaryPlan <> 15 Then
                    Me.ComplimentaryPlan = 16


                End If
                Me.DiscountApplicable = 100


                Try
                    Dim ob As LoggerService
                    ob = LoggerService.gtInstance

                    ob.write2LogFile("MYLOG-" & _guestRoomNo, "WEBSITE:-  " & "Me.DiscountApplicable" & Me.DiscountApplicable & "Plan " & Me.ComplimentaryPlan)
                Catch ex As Exception

                End Try


            End If


        Catch ex As Exception

        End Try



        If Not String.IsNullOrEmpty(PrivilegeCode) Then
            Dim privilege_code As String = Me.PrivilegeCode.Trim().ToLower()

            If privilege_code = "comp" Then

                If Me.ComplimentaryPlan <> 15 Then
                    Me.ComplimentaryPlan = 16


                End If





                Me.DiscountApplicable = 100
                Me.DiscountTypeApplicable = "CheckinCode"
                typeForComplimentary = "CheckinCode"

                Try
                    Dim ob As LoggerService
                    ob = LoggerService.gtInstance

                    ob.write2LogFile("MYLOG-" & _guestRoomNo, "COMP:-  " & "Me.DiscountApplicable" & Me.DiscountApplicable & "Plan " & Me.ComplimentaryPlan)
                Catch ex As Exception

                End Try


            Else
                If Me.ComplimentaryPlan = 15 Or Me.ComplimentaryPlan = 16 Then

                    Try
                        Dim ob As LoggerService
                        ob = LoggerService.gtInstance

                        ob.write2LogFile("MYLOG-" & _guestRoomNo, "DISCCOMP:-  " & "Me.DiscountApplicable" & Me.DiscountApplicable & "Plan " & Me.ComplimentaryPlan)
                    Catch ex As Exception

                    End Try
                Else

                    Dim checkinDiscount As Double = GetDicountByCode(privilege_code)
                    If checkinDiscount > Me.DiscountApplicable Then
                        Me.DiscountApplicable = checkinDiscount
                        Me.DiscountTypeApplicable = "CheckinCode"
                    End If

                    Me.ComplimentaryPlan = 17

                    Try
                        Dim ob As LoggerService
                        ob = LoggerService.gtInstance

                        ob.write2LogFile("MYLOG-" & _guestRoomNo, "DIScount:-  " & "Me.DiscountApplicable" & Me.DiscountApplicable & "Plan " & Me.ComplimentaryPlan)
                    Catch ex As Exception

                    End Try


                End If








            End If
        End If

        Try
            If OTA(_guestID) = 1 Then
                Dim privilege_code As String = ""

                If Not String.IsNullOrEmpty(PrivilegeCode) Then
                    privilege_code = Me.PrivilegeCode.Trim().ToLower()
                End If


                If privilege_code = "comp" Then

                    Me.DiscountApplicable = 100
                    Me.ComplimentaryPlan = 16

                Else
                    Me.DiscountApplicable = 0
                    Me.ComplimentaryPlan = 19

                End If




                Try
                    Dim ob As LoggerService
                    ob = LoggerService.gtInstance

                    ob.write2LogFile("MYLOG-" & _guestRoomNo, "OTA:-  " & "Me.DiscountApplicable" & Me.DiscountApplicable & "Plan " & Me.ComplimentaryPlan)
                Catch ex As Exception

                End Try

            End If


        Catch ex As Exception

        End Try


        Try
            If wifi(_guestID) = 1 Then

                Me.ComplimentaryPlan = 15
                Me.DiscountApplicable = 100



                Try
                    Dim ob As LoggerService
                    ob = LoggerService.gtInstance

                    ob.write2LogFile("MYLOG-" & _guestRoomNo, "WIFI:-  " & "Me.DiscountApplicable" & Me.DiscountApplicable & "Plan " & Me.ComplimentaryPlan)
                Catch ex As Exception

                End Try


            End If


        Catch ex As Exception

        End Try




        If Me.DiscountApplicable = 0 Then
            Me.ComplimentaryPlan = 19
        End If

        '------------------------------------------------------------------------------------

        'For Each discd As DiscountData In discDataList

        '    If Me.ComplimentaryPlan <= 0 And typeForComplimentary = "" Then
        '        If discd.ComplimentaryPlan > 0 Then
        '            Me.ComplimentaryPlan = discd.ComplimentaryPlan
        '            typeForComplimentary = discd.ProfileType
        '        End If

        '    ElseIf discd.ComplimentaryPlan > 0 Then
        '        Dim objPlan As New CPlan
        '        objPlan.getPlaninfo(Me.ComplimentaryPlan)
        '        Dim setAmount As Double = objPlan.planAmount
        '        objPlan.getPlaninfo(discd.ComplimentaryPlan)
        '        Dim currentAmount As Double = objPlan.planAmount

        '        If currentAmount > setAmount Then
        '            Me.ComplimentaryPlan = discd.ComplimentaryPlan
        '            typeForComplimentary = discd.ProfileType
        '        End If

        '    End If

        '    If Me.HidePlans = False And typeForHidePlan = "" Then
        '        If discd.HidePlans = True Then
        '            Me.HidePlans = discd.HidePlans
        '            typeForHidePlan = discd.ProfileType
        '        End If
        '    End If

        'Next

        'If Me.HidePlans = True And Me.ComplimentaryPlan <= 0 Then
        '    Me.ComplimentaryPlan = Me.DefaultComplementaryPlan
        '    typeForComplimentary = typeForHidePlan
        'End If

        'Me.ComplimentaryTypeApplicable = typeForComplimentary


        '------- Exceptional case where guest need to be autoconnected for a particular plan
        'For Each discd As DiscountData In discDataList

        '    If discd.AutoConnectPlan > 1 Then
        '        Me.ExceptionalAutoConnect = New AutoConnect(discd.AutoConnectPlan, discd.AutoConnectDiscount)
        '    End If

        'Next
        '------- Exceptional case where guest need to be autoconnected for a particular plan

    End Sub



    Public Function wifi(ByVal guestid As Long) As Integer
        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "select * from guest where upper(guestcfa9) like '%INRMWIFI%'  and guestid=@GuestID"
            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, guestid)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then

                Return 1

            Else

                Return 0

            End If



        Catch ex As Exception
            Return 0
        End Try







    End Function


    Public Function website(ByVal guestid As Long) As Integer
        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "select * from guest where upper(guestcfa8) in ('WGWB','STWB') and guestid=@GuestID"
            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, guestid)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then

                Return 1

            Else

                Return 0

            End If



        Catch ex As Exception
            Return 0
        End Try







    End Function

    Public Function OTA(ByVal guestid As Long) As Integer
        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "select * from guest where upper(guestcfa8) in ('BKGC','EXPE','MMTP','DESI','ORBI','TRAV') and guestid=@GuestID"
            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, guestid)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then

                Return 1

            Else

                Return 0

            End If



        Catch ex As Exception
            Return 0
        End Try







    End Function


    Private Sub SetMembershipName()

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            Dim membershipExists As Boolean = True

            If Me.MembershipLevel <> "" Then
                com.CommandText = "Select Case When [Description] Is Null Then '' Else [Description] End As [Description] From DiscountMappings1 Where [Type] = 'MembershipLevel' And Code = @Code"
                DatabaseUtil.AddInputParameter(com, "@Code", DbType.String, Me.MembershipLevel)
            ElseIf Me.MembershipType <> "" Then
                com.CommandText = "Select Case When [Description] Is Null Then '' Else [Description] End As [Description] From DiscountMappings1 Where [Type] = 'MembershipType' And Code = @Code"
                DatabaseUtil.AddInputParameter(com, "@Code", DbType.String, Me.MembershipType)
            Else
                membershipExists = False
            End If


            If membershipExists = True Then
                Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)
                If result.Rows.Count > 0 Then
                    Me.MembershipName = result.Rows(0)("Description").ToString()
                Else
                    Me.MembershipName = ""
                End If

            Else
                Me.MembershipName = ""
            End If


        Catch ex As Exception
            Me.MembershipName = ""
            objLog.writeExceptionLogFile("SetMembershipNameExp", ex)
        End Try

    End Sub

End Class