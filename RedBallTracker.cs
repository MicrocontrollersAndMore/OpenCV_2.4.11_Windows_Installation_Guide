// RedBallTracker.cs
//
// put this code in your main form, for example frmMain.vb
//
// add the following components to your form:
// ibOriginal (Emgu ImageBox)
// ibProcessed (Emgu ImageBox)
// btnPauseOrResume (Button)
// txtXYRadius (TextBox)
//
// NOTE: Do NOT copy/paste the entire text of this file into Visual Studio !! It will not work if you do !!
// Follow the video on my YouTube channel to create the project and have Visual Studio write part of the code for you,
// then copy/pase the remaining text as needed

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;                 //
using Emgu.CV.CvEnum;          // usual Emgu Cv imports
using Emgu.CV.Structure;       //
using Emgu.CV.UI;              //

///////////////////////////////////////////////////////////////////////////////////////////////////
namespace RedBallTracker1
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public partial class frmMain : Form
    {
        // member variables ///////////////////////////////////////////////////////////////////////
        Capture capWebcam;                      // Capture object to use with webcam
        bool blnCapturingInProcess = false;     // use this to keep track of if we are capturing or not to facilitate pause/resume button feature

        Image<Bgr, Byte> imgOriginal;
        Image<Bgr, Byte> imgBlurredBGR;
        Image<Gray, Byte> imgProcessed;

        // constructor ////////////////////////////////////////////////////////////////////////////
        public frmMain()
        {
            InitializeComponent();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                capWebcam = new Capture();
            }
            catch (Exception ex)
            {
                MessageBox.Show("unable to read from webcam, error: " + Environment.NewLine + Environment.NewLine +
                                ex.Message + Environment.NewLine + Environment.NewLine +
                                "exiting program");
                Environment.Exit(0);
            }
            Application.Idle += processFrameAndUpdateGUI;       // add process image function to the application's list of tasks
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void processFrameAndUpdateGUI(object sender, EventArgs arg)
        {
            imgOriginal = capWebcam.QueryFrame();               // get next frame from the webcam

            if (imgOriginal == null)
            {
                MessageBox.Show("unable to read from webcam" + Environment.NewLine + Environment.NewLine +
                                "exiting program");
                Environment.Exit(0);                            // and exit program
            }

            imgBlurredBGR = imgOriginal.SmoothGaussian(5);        // blur

            imgProcessed = imgBlurredBGR.InRange(new Bgr(0, 0, 175), new Bgr(100, 100, 256));

            imgProcessed = imgProcessed.SmoothGaussian(5);

            StructuringElementEx structuringElementEx = new StructuringElementEx(5, 5, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_RECT);

            CvInvoke.cvDilate(imgProcessed, imgProcessed, structuringElementEx, 1);
            CvInvoke.cvErode(imgProcessed, imgProcessed, structuringElementEx, 1);

            CircleF[] circles = imgProcessed.HoughCircles(new Gray(100), new Gray(50), 2, imgProcessed.Height / 4, 10, 400)[0];

            foreach (CircleF circle in circles)
            {
                if (txtXYRadius.Text != "") txtXYRadius.AppendText(Environment.NewLine);

                txtXYRadius.AppendText("ball position = x " + circle.Center.X.ToString().PadLeft(4) +
                                       ", y = " + circle.Center.Y.ToString().PadLeft(4) +
                                       ", radius = " + circle.Radius.ToString("###.000").PadLeft(7));

                txtXYRadius.ScrollToCaret();

                CvInvoke.cvCircle(imgOriginal, new Point((int)circle.Center.X, (int)circle.Center.Y), 3, new MCvScalar(0, 255, 0), -1, LINE_TYPE.CV_AA, 0);

                imgOriginal.Draw(circle, new Bgr(Color.Red), 3);
            }
            ibOriginal.Image = imgOriginal;
            ibProcessed.Image = imgProcessed;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnPauseOrResume_Click(object sender, EventArgs e)
        {
            if (blnCapturingInProcess == true)
            {
                Application.Idle -= processFrameAndUpdateGUI;
                blnCapturingInProcess = false;
                btnPauseOrResume.Text = " resume ";
            }
            else
            {
                Application.Idle += processFrameAndUpdateGUI;
                blnCapturingInProcess = true;
                btnPauseOrResume.Text = " pause ";
            }

        }
    }
}
