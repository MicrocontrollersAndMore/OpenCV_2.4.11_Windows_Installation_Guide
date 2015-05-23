'frmMain.vb
'
'NOTE: Do NOT copy/paste the entire text of this file into Visual Studio !! It will not work if you do !!
'Follow the video on my YouTube channel to create the project and have Visual Studio write part of the code for you,
'then copy/pase the remaining text as needed

Option Explicit On
Option Strict On

Imports Emgu.CV
Imports Emgu.CV.CvEnum
Imports Emgu.CV.Structure
Imports Emgu.CV.UI

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class frmMain

    ' member variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Dim imgOriginal As Image(Of Bgr, Byte)
    Dim imgGrayscale As Image(Of Gray, Byte)
    Dim imgBlurred As Image(Of Gray, Byte)
    Dim imgCanny As Image(Of Gray, Byte)

    Private Sub btnOpenFile_Click(sender As Object, e As EventArgs) Handles btnOpenFile.Click
        Dim drChosenFile As DialogResult

        drChosenFile = ofdOpenFile.ShowDialog()
        lblChosenFile.Text = ofdOpenFile.FileName

        If (drChosenFile <> Windows.Forms.DialogResult.OK Or ofdOpenFile.FileName = "") Then
            lblChosenFile.Text = "file not chosen"
            Return
        End If

        imgOriginal = New Image(Of Bgr, Byte)(ofdOpenFile.FileName)           ' load image

        imgGrayscale = imgOriginal.Convert(Of Gray, Byte)()

        imgBlurred = imgGrayscale.SmoothGaussian(5)

        Dim dblCannyThresh As Double = 180.0
        Dim dblCannyThreshLinking As Double = 120.0

        imgCanny = imgBlurred.Canny(dblCannyThresh, dblCannyThreshLinking)

        ibOriginal.Image = imgOriginal
        ibCanny.Image = imgCanny
    End Sub

End Class
