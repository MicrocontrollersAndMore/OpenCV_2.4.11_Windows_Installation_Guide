using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

///////////////////////////////////////////////////////////////////////////////////////////////////
namespace CS_test_2
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public partial class frmMain : Form 
    {
        // member variables ///////////////////////////////////////////////////////////////////////
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
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            DialogResult drChosenFile;

            drChosenFile = ofdOpenFile.ShowDialog();

            if (drChosenFile != System.Windows.Forms.DialogResult.OK || ofdOpenFile.FileName == "")
            {
                lblChosenFile.Text = "file not chosen";
                return;
            }
            
            try
            {
                imgOriginal = new Image<Bgr, byte>(ofdOpenFile.FileName);
            }
            catch (Exception exception)
            {
                lblChosenFile.Text = "unable to open image, error: " + exception.Message;
                return;
            }

            if (imgOriginal == null)
            {
                lblChosenFile.Text = "unable to open image";
                return;
            }

            imgGrayscale = imgOriginal.Convert<Gray, Byte>();
            imgBlurred = imgGrayscale.SmoothGaussian(5);

            double dblCannyThresh = 180.0;
            double dblCannyThreshLinking = 120.0;

            imgCanny = imgBlurred.Canny(dblCannyThresh, dblCannyThreshLinking);

            ibOriginal.Image = imgOriginal;
            ibCanny.Image = imgCanny;
        }
    }
}
