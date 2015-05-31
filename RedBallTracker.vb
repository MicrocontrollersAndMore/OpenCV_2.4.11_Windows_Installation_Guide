'RedBallTracker.vb
'
'put this code in your main form, for example frmMain.vb
'
'add the following components to your form:
'ibOriginal (Emgu ImageBox)
'ibProcessed (Emgu ImageBox)
'btnPauseOrResume (Button)
'txtXYRadius (TextBox)
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

    ' member variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Dim capWebcam As Capture                        'Capture object to use with webcam
    Dim blnCapturingInProcess As Boolean = False    'use this to keep track of if we are capturing or not to facilitate pause/resume button feature
    
    Dim imgOriginal As Image(Of Bgr, Byte)          'original image
    Dim imgBlurredBGR As Image(Of Bgr, Byte)        'blurred color image
    Dim imgProcessed As Image(Of Gray, Byte)        'processed image

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub frmMain_Load( sender As Object,  e As EventArgs) Handles MyBase.Load
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
	    blnCapturingInProcess = True
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub ProcessFrameAndUpdateGUI(sender As Object, arg As EventArgs)
	    imgOriginal = capWebcam.QueryFrame()							'get next frame from the webcam
	    
        If(imgOriginal Is Nothing)										'if we did not get a frame
		                                                                'show error via message box
            MessageBox.Show("unable to read from webcam" + Environment.NewLine + Environment.NewLine + _
                            "exiting program")
            Environment.Exit(0)                                         'and exit program
	    End If
        
        imgBlurredBGR = imgOriginal.SmoothGaussian(5)        'blur

                                    'min filter value (if color is greater than or equal to this)
                                                        'max filter value (if color is less than or equal to this)
	    imgProcessed = imgBlurredBGR.InRange(New Bgr(0, 0, 175), New Bgr(100, 100, 256))

	    imgProcessed = imgProcessed.SmoothGaussian(5)		'blur again

        Dim structuringElementEx As StructuringElementEx = New StructuringElementEx(5, 5, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_RECT)  'declare structuring element to use in dilate and erode

        CvInvoke.cvDilate(imgProcessed, imgProcessed, structuringElementEx, 1)      'close image (dilate, then erode)
        CvInvoke.cvErode(imgProcessed, imgProcessed, structuringElementEx, 1)       'closing "closes" (i.e. fills in) foreground gaps

                        'fill variable circles with all circles in the processed image
															'Canny threshold
																		'accumulator threshold
																		    'size of image / this param = "accumulator resolution"
																		            'min distance in pixels between the centers of the detected circles
																		                                'min radius of detected circle
																		                                        'max radius of detected circle
																		                                                'get circles from the first channel
	    Dim circles As CircleF() = imgProcessed.HoughCircles(New Gray(100), New Gray(50), 2, imgProcessed.Height / 4, 10, 400)(0)

	    For Each CircleF In circles                         'for each circle
		    If(txtXYRadius.Text <> "") Then							'if we are not on the first line in the text box
			    txtXYRadius.AppendText(Environment.NewLine)			'then insert a new line char
		    End If
                        'print ball position and radius
			    						            'x position of center point of circle
			    						                                                                'y position of center point of circle
				    					                                                                                                                    'radius of circle
		    txtXYRadius.AppendText("ball position x = " + CircleF.Center.X.ToString().PadLeft(4) + ", y = " + CircleF.Center.Y.ToString().PadLeft(4) + ", radius = " + CircleF.Radius.ToString("###.000").PadLeft(7))
		    txtXYRadius.ScrollToCaret()             'scroll down in text box so most recent line added (at the bottom) will be shown

		            'Draw a small green circle at the center of the detected object. To do this, we will call the OpenCV 1.x function, this is necessary
			        'b/c we are drawing a circle of radius 3, even though the size of the detected circle will be much bigger.
			        'The CvInvoke object can be used to make OpenCV 1.x function calls
						    'draw on the original image
												        'center point of circle
																		                'radius of circle in pixels
																									        'draw pure green
																												    	'thickness of circle in pixels, -1 indicates to fill the circle
										        																			    		'use AA to smooth the pixels
												        																		    				 'no shift
		    CvInvoke.cvCircle(imgOriginal, New Point(CInt(CircleF.Center.X), CInt(CircleF.Center.Y)), 3, New MCvScalar(0, 255, 0), -1, LINE_TYPE.CV_AA, 0)

		            'draw a red circle around the detected object
				        'current circle we are on in For Each loop
                                        'draw pure red
                                                'thickness of circle in pixles
		    imgOriginal.Draw(CircleF, New Bgr(Color.Red), 3)
        Next
        ibOriginal.Image = imgOriginal              'update image boxes on form
        ibProcessed.Image = imgProcessed            '
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private Sub btnPauseOrResume_Click( sender As Object,  e As EventArgs) Handles btnPauseOrResume.Click
	    If(blnCapturingInProcess = True) Then                                           'if we are currently processing an image, user just choose pause, so . . .
		    RemoveHandler Application.Idle, New EventHandler(AddressOf Me.ProcessFrameAndUpdateGUI)     'remove the process image function from the application's list of tasks
		    blnCapturingInProcess = False																'update flag variable
		    btnPauseOrResume.Text = " resume "															'update button text
	    Else																			'else if we are not currently processing an image, user just choose resume, so . . .
		    AddHandler Application.Idle, New EventHandler(AddressOf Me.ProcessFrameAndUpdateGUI)        'add the process image function to the application's list of tasks
		    blnCapturingInProcess = True																'update flag variable
		    btnPauseOrResume.Text = " pause "															'new button will offer pause option
	    End If
    End Sub

End Class
