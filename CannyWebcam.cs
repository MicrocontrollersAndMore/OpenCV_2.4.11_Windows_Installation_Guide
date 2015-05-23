// CannyWebcam.cs
//
// put this code in your main form, for example frmMain.cs
// add the following components to your form:
// TableLayoutPanel1 (TableLayoutPanel) (name does not really matter for this, we will not need to refer to it in code
// ibOriginal (Emgu ImageBox)
// ibCanny (Emgu ImageBox)
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

using Emgu.CV;                  //
using Emgu.CV.CvEnum;           // usual Emgu CV imports
using Emgu.CV.Structure;        //
using Emgu.CV.UI;               //

///////////////////////////////////////////////////////////////////////////////////////////////////
namespace RedBallTracker1
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public partial class frmMain : Form
    {
        // member variables ///////////////////////////////////////////////////////////////////////
        Capture capWebcam;

        Image<Bgr, Byte> imgOriginal;
        Image<Gray, Byte> imgGrayscale;
        Image<Gray, Byte> imgBlurred;
        Image<Gray, Byte> imgCanny;

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
            catch(Exception ex)
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
            imgGrayscale = imgOriginal.Convert<Gray, Byte>();
            imgBlurred = imgGrayscale.SmoothGaussian(5);

            double dblCannyThresh = 150.0;
            double dblCannyThreshLinking = 75.0;

            imgCanny = imgBlurred.Canny(dblCannyThresh, dblCannyThreshLinking);

            ibOriginal.Image = imgOriginal;
            ibCanny.Image = imgCanny;
        }
    }
}
