Imports System.Security.Cryptography

Public Class Hashing

    Public Shared Function Hash(ByVal text As String) As String
        Dim originalTextBytes As Byte()
        Dim encodedTextBytes As Byte()
        Dim md5 As MD5CryptoServiceProvider

        'Instantiate MD5CryptoServiceProvider, get bytes for user's original text and encode text in MD5 format.
        md5 = New MD5CryptoServiceProvider()
        originalTextBytes = System.Text.ASCIIEncoding.Default.GetBytes(text)
        encodedTextBytes = md5.ComputeHash(originalTextBytes)

        'Convert encoded text in 'readable" format.
        Return BitConverter.ToString(encodedTextBytes)

    End Function

End Class