'############## Jan 11,2011-Class Description Start ############################
'-- This class consider as a container of all the enums and structures used 
'-- all over the application
'-- It does not contain any initialization and any method
'############## Jan 11,2011-Class Description End   ############################
'Enum values ############################
Public Enum DISPLAY
    LOGINPANEL
    CONTINUEPANEL
End Enum
Public Enum PMSNAMES
    CLS
    IDS
    FIDELIO
    OPERA
    AMEDEUS
    BRILLIANT
    WINHMS
    UNKNOWN
End Enum
Public Enum LOGINTYPE
    NEWLOGIN = 0
    RELOGIN = 1
    UPGRADE = 2
    RENEW = 3
    FREEPLAN = 4
    APPEROR = 5
End Enum
Public Enum EUSERTYPE
    ROOM = 0
End Enum
Public Enum PMSBill
    ROOM = 0
    COUPON
    BC
    BCNSG
    ACCNOLAN
    ACCMOB
End Enum
Public Enum nomadixCommand
    ADD
    AUTHORIZE
    DELETE
    QUERY
    USERSTATUS
End Enum

Public Enum ACCESSTYPE
    ONLYMAC = 0
    ONLYCODE = 1
    ONLYID = 2
    ONLYROOM = 3
End Enum

Public Enum PLANSTATUS
    ALL
    ACTIVEONLY
    DIACTIVEONLY
End Enum

Public Enum PLANTYPES
    ROOM
    NORMALCOUPON
    BULKCOUPON
    BULKEXTRACOUPON
    BC
    BANQUET
    FREE
    DEMO
End Enum
Public Enum PLANORDERBY
    Duration = 0
    Speed = 1
End Enum
Public Enum MACTYPE
    ND = 0
    MOB = 1
    LAP = 2
End Enum
'Structures #####################################
Public Structure UserCredential
    Dim usrId As String
    Dim passwd As String
    Dim GuestID As Long
    Dim CurrPlanId As Integer
    Dim ACCID As Long
End Structure

Public Structure StructADCred
    Dim ADuserid As String
    Dim ADpassword As String
End Structure
Public Structure UserLogDetails
    Dim RemainTime As Long
    Dim LoginId As Long
    Dim FirstLoginTime As DateTime
    Dim ExpTime As DateTime
    Dim PlanId As Integer
End Structure
'Nomadix Related #######################
Public Structure structNomadixData
    Dim MACAddress As String
    Dim RoomNo As String
    Dim GuestLName As String
    Dim RegCode As String
    Dim PlanTime As String 'In seconds
    Dim PlanAmount As String
End Structure
Public Structure structNomadixItem
    Dim itemCode As String
    Dim itemDesc As String ' Item Describtion
    Dim itemAmount As Double
    Dim itemTaxAmount As Double
End Structure
Public Structure structAddMACItem
    Dim UName As String
    Dim URoomNo As String
    Dim UExpTime As Long
End Structure
Public Structure strLoginFails
    Dim FailMac As String
    Dim FailAccId As Long
    Dim FailRoomNo As String
    Dim FailGuestName As String
    Dim FailMsg As String
    Dim FailAccessType As Integer
    Dim Remarks As String
    Dim FailAccessCode As String
End Structure
