'CannyStill.vb
'
'put this code in your main form, for example frmMain.vb
'
'add the following components to your form:
'btnOpenFile (Button)
'lblChosenFile (Label)
'ibOriginal (Emgu ImageBox)
'ibCanny (Emgu ImageBox)
'ofdOpenFile (OpenFileDialog)
'
'NOTE: Do NOT copy/paste the entire text of this file into Visual Studio !! It will not work if you do !!
'Follow the video on my YouTube channel to create the project and have Visual Studio write part of the code for you,
'then copy/pase the remaining text as needed
'
'in this example we are using code to resize components when the form is resized,
'if you used the layout managers you do not need to use the component resizing code

Option Explicit On
Option Strict On

Imports Emgu.CV
Imports Emgu.CV.CvEnum
Imports Emgu.CV.Structure
Imports Emgu.CV.UI

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class frmMain

    ' member variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Dim imgOriginal As Image(Of Bgr, Byte)          'input image
    Dim imgGrayscale As Image(Of Gray, Byte)        'grayscale of input image
    Dim imgBlurred As Image(Of Gray, Byte)          'intermediate blured image
    Dim imgCanny As Image(Of Gray, Byte)            'Canny edge image

    Dim blnFirstTimeInResizeEvent As Boolean = True     'see comment if frmMain_Resize for purpose of this variable
    Dim intButtonAndLabelHorizPadding As Integer        '
    Dim intImageBoxesHorizPadding As Integer            'original component padding for component resizing
    Dim intImageBoxesVertPadding As Integer             '

    ' constructor '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub New()
        InitializeComponent()               'this call is required by the designer

        intButtonAndLabelHorizPadding = Me.Width - btnOpenFile.Width - lblChosenFile.Width      '
        intImageBoxesHorizPadding = Me.Width - ibOriginal.Width - ibCanny.Width                 'get original padding for component resizing later
        intImageBoxesVertPadding = Me.Height - btnOpenFile.Height - ibOriginal.Height           '
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub frmMain_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        'This If Else statement is necessary to throw out the first time the Form1_Resize event is called.
        'For some reason, in VB.NET the Resize event is called once before the constructor, then the constructor is called,
        'then the Resize event is called each time the form is resized.  The first time the Resize event is called
        '(i.e. before the constructor is called) the coordinates of the components on the form all read zero,
        'therefore we have to throw out this first call, then the constructor will run and get the correct initial
        'component location data, then every time after that we can let the Resize event run as expected
        If (blnFirstTimeInResizeEvent = True) Then
            blnFirstTimeInResizeEvent = False
        Else
            lblChosenFile.Width = Me.Width - btnOpenFile.Width - intButtonAndLabelHorizPadding      'resize label

            ibOriginal.Width = CInt((Me.Width - intImageBoxesHorizPadding) / 2)             'resize image box widths
            ibCanny.Width = ibOriginal.Width

            ibCanny.Left = ibOriginal.Width + CInt(intImageBoxesHorizPadding * (1 / 3))     'update x position for Canny image box

            ibOriginal.Height = Me.Height - btnOpenFile.Height - intImageBoxesVertPadding   'resize image box heights
            ibCanny.Height = ibOriginal.Height
        End If
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub btnOpenFile_Click(sender As Object, e As EventArgs) Handles btnOpenFile.Click
        Dim drChosenFile As DialogResult

        drChosenFile = ofdOpenFile.ShowDialog()             'open file dialog
        lblChosenFile.Text = ofdOpenFile.FileName           'write file name to label

        If (drChosenFile <> Windows.Forms.DialogResult.OK Or ofdOpenFile.FileName = "") Then    'if user chose Cancel or filename is blank . . .
            lblChosenFile.Text = "file not chosen"              'show error message on label
            Return                                              'and exit function
        End If

        imgOriginal = New Image(Of Bgr, Byte)(ofdOpenFile.FileName)     'open image

        imgGrayscale = imgOriginal.Convert(Of Gray, Byte)()             'convert to grayscale

        imgBlurred = imgGrayscale.SmoothGaussian(5)                     'blur

        Dim dblCannyThresh As Double = 180.0                            'declare params for call to Canny
        Dim dblCannyThreshLinking As Double = 120.0                     '

        imgCanny = imgBlurred.Canny(dblCannyThresh, dblCannyThreshLinking)  'get Canny edges

        ibOriginal.Image = imgOriginal              'update image boxes
        ibCanny.Image = imgCanny                    '
    End Sub

End Class
