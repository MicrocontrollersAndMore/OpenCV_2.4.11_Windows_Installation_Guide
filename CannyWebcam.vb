'CannyWebcam.vb
'
'put this code in your main form, for example frmMain.vb
'
'add the following components to your form:
'TableLayoutPanel1 (TableLayoutPanel) (name does not really matter for this, we will not need to refer to it in code
'ibOriginal (Emgu ImageBox)
'ibCanny (Emgu ImageBox)
'
'NOTE: Do NOT copy/paste the entire text of this file into Visual Studio !! It will not work if you do !!
'Follow the video on my YouTube channel to create the project and have Visual Studio write part of the code for you,
'then copy/pase the remaining text as needed

Option Explicit On      'require explicit declaration of variables, this is NOT Python !!
Option Strict On        'restrict implicit data type conversions to only widening conversions

Imports Emgu.CV                 '
Imports Emgu.CV.CvEnum          'usual Emgu Cv imports
Imports Emgu.CV.Structure       '
Imports Emgu.CV.UI              '

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class frmMain

    ' member variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Dim capWebcam As Capture                        'Capture object to use with webcam

    Dim imgOriginal As Image(Of Bgr, Byte)          'input image
    Dim imgGrayscale As Image(Of Gray, Byte)        'grayscale of input image
    Dim imgBlurred As Image(Of Gray, Byte)          'intermediate blured image
    Dim imgCanny As Image(Of Gray, Byte)            'Canny edge image

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            capWebcam = New Capture()                               'associate the capture object to the default webcam
        Catch ex As Exception                                       'catch error if unsuccessful
                                                                    'show error via message box
            MessageBox.Show("unable to read from webcam, error: " + Environment.NewLine + Environment.NewLine + _
                            ex.Message + Environment.NewLine + Environment.NewLine + _
                            "exiting program")
            Environment.Exit(0)                                     'and exit program
        End Try
        
        AddHandler Application.Idle, New EventHandler(AddressOf Me.ProcessFrameAndUpdateGUI)        'add process image function to the application's list of tasks
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub ProcessFrameAndUpdateGUI(sender As Object, arg As EventArgs)
        imgOriginal = capWebcam.QueryFrame()                            'get next frame from the webcam
        
        If (imgOriginal Is Nothing) Then                                'if we did not get a frame
                                                                        'show error via message box
            MessageBox.Show("unable to read from webcam" + Environment.NewLine + Environment.NewLine + _
                            "exiting program")
            Environment.Exit(0)                                         'and exit program
        End If

        imgGrayscale = imgOriginal.Convert(Of Gray, Byte)()             'convert to grayscale
        imgBlurred = imgGrayscale.SmoothGaussian(5)                     'blur

        Dim dblCannyThresh As Double = 150.0                            'declare params for call to Canny
        Dim dblCannyThreshLinking As Double = 75.0                     '

        imgCanny = imgBlurred.Canny(dblCannyThresh, dblCannyThreshLinking)  'get Canny edges

        ibOriginal.Image = imgOriginal              'update image boxes
        ibCanny.Image = imgCanny                    '
    End Sub

End Class
