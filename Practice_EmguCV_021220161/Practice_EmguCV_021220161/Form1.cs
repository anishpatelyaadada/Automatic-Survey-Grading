using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;

using Emgu.Util;
using Emgu.Util.TypeEnum;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
//using Microsoft.Office.Interop.Excel;

namespace Practice_EmguCV_021220161
{
    public partial class Form1 : Form
    { 
        //Capture cap;
      //  bool pause, dataFileCondition = false;
        RectangleDetection object1 = new RectangleDetection();
        Calculation calculation = new Calculation();
        PixellnsideRectangleSquare answerUsingPixelsInImage = new PixellnsideRectangleSquare();
        //Mat imageOriginal = new Mat();

        string fileName;
        public Form1()
        {
            InitializeComponent();
        }
        // main form 
        private void Form1_Load(object sender, EventArgs e)
        {
            // exception to check the camera
            try
            {
                // get the frame
                //cap = new Capture(0);
            }
            catch (NullReferenceException except)
            {
                // display text in case camera dont open
                MessageBox.Show(except.Message);
                return;
            }
            // function to process continusly

            //Application.Idle += imageTransformation; // if we want to run camera continusly

            //pause = true;
            // captureImageProcess = true;
        }

        // to close the form after use
        private void form1_formclosed(object sender, FormClosedEventArgs arg)
        {
            //if (cap != null)
            //{
            //    cap.Dispose();
            //}
        }

        // transform image and send to another function for calculation
        public void imageTransformation(object sender, EventArgs arg, Mat imageOriginal)
        {
            VectorOfVectorOfPoint square = new VectorOfVectorOfPoint();
            // Mat imageOriginal = new Mat();

            //imageOriginal = CvInvoke.Imread("C:\\Users\\Anish\\OneDrive - Texas Southern University\\Texas Southern University\\Summer 2016\\Sample Scranton\\20.jpg",LoadImageType.Unchanged);
            //imageOriginal = CvInvoke.Imread(fileName, LoadImageType.Unchanged);
            // img = img.Reshape(1,1);
            // img = cap.QueryFrame();
            //imageRealOriginal.Image = imageOriginal;
            ImageSize.Text = Convert.ToString(imageOriginal.Cols + ", " + imageOriginal.Rows); // get the size of image rows and column and display on screen
            // convert the original image into byte form column by column and row by row
            Image<Bgr, byte> byteImage = imageOriginal.ToImage<Bgr, byte>();//.Resize(600, 600, Emgu.CV.CvEnum.Inter.Linear, true);
            Image<Bgr, byte> byteImageCopy = imageOriginal.ToImage<Bgr, Byte>();//.Resize(600, 450, Emgu.CV.CvEnum.Inter.Linear, true);
            Image<Gray, byte> byteGrayImage = byteImage.Convert<Gray, byte>(); // convert the image into byte format
            if (imageOriginal == null) return;
            else { } //textBox.Text = "Good!";

            #region initialization of Mat

            // define variable for the Mat to hold images
            Mat imageGray = new Mat();
            Mat imageGrayNoise = new Mat();
            Mat imageSubtract = new Mat();
            Mat RoughImage1 = new Mat();
            Mat RoughImage2 = new Mat();
            Mat imageMorphCross = new Mat();
            Mat imageMorphAnswer = new Mat();
            Mat cannyImageCross = new Mat();
            Mat cannyImageAnswer = new Mat();
            Mat cropImage = new Mat();
            Mat smallRectangleCropImage = new Mat();
            Mat invertImage = new Mat();
            VectorOfVectorOfPoint Hirechy = new VectorOfVectorOfPoint();
            #endregion

            #region initialization of file, to get data
            // crete file to save output data , error
            //Initializes a new instance of the FileStream class for the specified file handle, with the specified read/write permission.
            FileStream outPut = new FileStream(@"C:\Users\kings\Desktop\Paper\data.txt", FileMode.OpenOrCreate);
            // to write the data to the file
            StreamWriter sw = new StreamWriter(outPut);
            // this help us to use file inside another function, beacause the file we initialize is local, not global,
            // so we have to use below function to get the data from anotehr fucntion to the file
            Console.SetOut(sw);
            #endregion

            #region rectangle detection and drawing around paper, but at this moment it is comment, not in use in program
            /*find square by connecting four lines
            object1.findSquares(imageOriginal, square, testImage, angle);
            textBox.Text = Convert.ToString("square = " + square.Size + " angle = " + angle);
            object1.debugSquares(square, imageOriginal, byteImage);
            SquareDetectionImage.Image = imageOriginal;*/
            /* new stuff in rectangle detection file
            object1.contourHirechy(Hirechy);
            SquareContourHirechy.Text = Convert.ToString(Hirechy.Size);
            */
            //display all the square / rectangle point on user form
            //rectanglePoint1.Text = Convert.ToString((int)object1.getPoint1().X + " ,  " + (int)object1.getPoint1().Y);
            //rectanglePoint2.Text = Convert.ToString((int)object1.getPoint2().X + " ,  " + (int)object1.getPoint2().Y);
            //rectanglePoint3.Text = Convert.ToString((int)object1.getPoint3().X + " ,  " + (int)object1.getPoint3().Y);
            //rectanglePoint4.Text = Convert.ToString((int)object1.getPoint4().X + " ,  " + (int)object1.getPoint4().Y);
            #endregion

            #region cvtColor, morphs, median blur, canny, threshold

            // convert the image into grayscale form
            CvInvoke.CvtColor(imageOriginal, imageGray, ColorConversion.Bgr2Gray);

            // denoising image
            addNoiseToImage_opencv(imageGray, imageGrayNoise);

            // morphology for cross detection
            morphsCrossDetection(imageOriginal, imageMorphCross, imageGrayNoise);
            // automatic threhsolding and canny edge detection
            cannyAndThresholdCross(imageOriginal, imageMorphCross, cannyImageCross);


            // morphology for answer detection with size(3,3), so it can detect small marks too
            morphsAnswer(imageOriginal, imageMorphAnswer, imageGrayNoise, invertImage);
            // canny for answer detection 
            thresholdAnswer(imageOriginal, imageMorphAnswer, cannyImageAnswer);
            // detect black and while pixcels 
            blackAndWhitePixcel(cannyImageAnswer);

            #endregion
            // get the total number of white pixel in an image using the pre-define function

            /*
            VectorOfPoint a = new VectorOfPoint();
            CvInvoke.FindNonZero(cannyImageAnswer, a);
            rectanglePoint1.Text = Convert.ToString(a.Size);
            */

            // cross detection function, and passing the data of cross to anther file for calculation
            crossDetection(cannyImageCross, byteImage, imageOriginal, cannyImageAnswer, cropImage, smallRectangleCropImage);

            subSquareCropImage.Image = smallRectangleCropImage;
            RectangleCropImage.Image = cropImage;
            imageRealOriginal.Image = cannyImageAnswer;

            sw.Close();// close the stream writer
        }

        // add noise to image, which reduce the total number of contour in the image
        public void addNoiseToImage_opencv(Mat imageGray, Mat imageGrayNoise)
        {
            Mat image_16Sc = new Mat();
            int noiseValue = 3;

            CvInvoke.FastNlMeansDenoising(imageGray, imageGrayNoise, noiseValue, 7, 21);
            textNoiseValue.Text += trackBarNoiseValue.Value.ToString();
            //subSquareCropImage.Image = finalImage;  
        }


        // morphological transformation of image to get the answer
        public void morphsAnswer(Mat imageOriginal, Mat morphImage, Mat grayImage, Mat invertImage)
        {
            Mat erodeElement = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Erode(grayImage, morphImage, erodeElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            // CvInvoke.Dilate(morphImage, morphImage, erodeElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            CvInvoke.MorphologyEx(morphImage, morphImage, MorphOp.Dilate, erodeElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            CvInvoke.BitwiseNot(morphImage, morphImage); // inverse every bit of array element

        }

        // morphological transformation, to determine the cross in the image
        public void morphsCrossDetection(Mat image, Mat morphImage, Mat grayImage)
        {
            CvInvoke.GaussianBlur(grayImage, grayImage, new Size(5, 5), 0, 0, BorderType.Default); // blur the image
            Mat erodeElement = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(19, 19), new Point(-1, -1));
            // CvInvoke.Erode(grayImage, morphImage, erodeElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            //CvInvoke.Dilate(grayImage, morphImage, erodeElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            //CvInvoke.Subtract(grayImage, image, finalImage, null, DepthType.Default);
            CvInvoke.MorphologyEx(grayImage, morphImage, MorphOp.Dilate, erodeElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
        }

        // thresholding to image for answer
        public void thresholdAnswer(Mat image, Mat morphologyImage, Mat CannyImage)
        {
            CvInvoke.Threshold(morphologyImage, CannyImage, 50, 255, ThresholdType.Binary);
        }

        // automatically select the threshold value of the image 
        public void cannyAndThresholdCross(Mat image, Mat morphologyImage, Mat CannyImage)
        {

            Mat threshImage = new Mat();
            double threshValue = new double();
            double threshMax = new double();
            double threshMin = new double();

            threshValue = CvInvoke.Threshold(morphologyImage, CannyImage, 0, 255, ThresholdType.Binary); // get the threshold double value
            threshValueForCalculation.Text = Convert.ToString(threshValue);// display the thresh value for calcualtion on user form
            threshMax = threshValue; // maximum value of threshold
            threshMin = threshValue * 0.25; // minimumm value of threshold
            automaticThreshMax.Text = Convert.ToString(threshMax); // display the max thresh value
            automaticThreshMin.Text = Convert.ToString(threshMin);// display the  min thresh value
            int a = trackBarMaximum.Value; // get the maximum value of threshold, trackbar from user form
            int b = trackBarMinimum.Value;// get the minimum value of threshold, trackbar from user form

            CvInvoke.Canny(morphologyImage, CannyImage, a, b, 3, true); // canny edge detection using threshold value

            cannyThresholdMax.Text = Convert.ToString(a); // pass back value of threhsold to textbox
            cannyThresholdMin.Text = Convert.ToString(b);// pass back value of threhsold to textbox
        }


        // to count black and while pixcel
        public void blackAndWhitePixcel(Mat image)
        {
            // convert the whole image in matrix form
            Matrix<byte> matrixImage = new Matrix<byte>(image.Rows, image.Cols, image.NumberOfChannels);
            image.CopyTo(matrixImage);
            int a = new int();
            int b = new int();
            int row = image.Rows;
            int cols = image.Cols;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < row; j++)
                {

                    if (matrixImage[j, i] == 0) // black pixcel in an image
                    {
                        b++;
                    }
                    else // white pixcel in an image
                    {
                        a++;
                    }

                }
            }
            whitePixcel.Text = Convert.ToString(a);
            blackPixcel.Text = Convert.ToString(b);
        }

        // overload function to detect the circle using moment 
        void crossDetection(Mat cannyImageCross, Image<Bgr, byte> byteImage, Mat image, Mat cannyImageAnswer, Mat cropImage, Mat smallRectangleCropImage)
        {
            // write data to file and rewrite it when it compile again or restart
            TextWriter CircleDetectionPoint = new StreamWriter(@"C: \Users\kings\Desktop\Paper\CircleDetectionPoint.txt"); // to get out put of all data in file
            VectorOfPoint PointsForCalculation = new VectorOfPoint(); // to save the point in dynamic array, means vector
            VectorOfPoint FinalPointAfterRemovingSamePoint = new VectorOfPoint();

            // ge the contours from image of specific area
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyImageCross, contours, null, RetrType.External, ChainApproxMethod.ChainApproxNone);
                int count = contours.Size;
                circleContour.Text = Convert.ToString(count);
                Console.WriteLine("Total contour = " + count);
                int j = 1;
               
                VectorOfVectorOfPoint hull = new VectorOfVectorOfPoint();
                Point[] a = new Point[count]; // create an array of point to save the contour points

                #region loop to get the position of cross
                for (int i = 0; i < count; i++)
                {
                    MCvMoments moment = CvInvoke.Moments(contours[i], true);
                    double area = moment.M00;

                    if (area > 0 && area < 20)/* default = (area > 0 || area < 200) */
                    {
                        setXValue((int)(moment.M10 / moment.M00));
                        setYValue((int)(moment.M01 / moment.M00));
                        Point centerInput = new Point(getXValue(), getYValue()); // get the point of detected contour

                        // reassign the point in the form of array because we have to pass to calculation file
                        a[i].X = centerInput.X;
                        a[i].Y = centerInput.Y;
                        // write all detected point of the countour to the file
                        CircleDetectionPoint.WriteLine(Convert.ToString("Point #" + j + " , i " + i + " = " + /*centerInput*/ centerInput + "   ,A = " + a[i] + "   ,A lenght= " + a.Length + "   ,Counter total = " + count + " vector of point size = " + PointsForCalculation.Size));

                        Console.WriteLine(Convert.ToString("Point #1 " + object1.getPoint1()));
                        Console.WriteLine(Convert.ToString("Point #2 " + object1.getPoint2()));
                        Console.WriteLine(Convert.ToString("Point #3 " + object1.getPoint3()));
                        Console.WriteLine(Convert.ToString("Point #4 " + object1.getPoint4()) + "\n");

                        // put data on screen
                        CvInvoke.PutText(image, /*"marker #" +*/Convert.ToString("  " + j) + " " + getXValue() + " " + getYValue(), new Point((int)getXValue(), (int)getYValue()), FontFace.HersheyComplex, 0.5, new MCvScalar(255, 0, 150), 1, LineType.FourConnected, false);
                        //draw circle
                        CvInvoke.Circle(image, centerInput, 5, new MCvScalar(0, 255, 0), -1, LineType.EightConnected, 0);
                        j++;
                    }
                    #endregion
                    //CircleDetectionPoint.WriteLine(Convert.ToString("Point #" + j + " = " + /*centerInput centerInput*/ "   ,A = " + a[i] + "   ,A lenght= " + a.Length + "   ,Counter total = " + count + " vector of point size = " + PointsForCalculation.Size));
                }
                markContour.Text = Convert.ToString(j - 1);
                // add all the detected points to the vector, which will pass to another file later
                PointsForCalculation.Push(a);
            }

            // pass all the detected points to the another file for calculation
            //calculation.detectedPointsInPaper(PointsForCalculation);
            answerUsingPixelsInImage.answerApplyingPixelValue(PointsForCalculation, cannyImageAnswer, image, cropImage, smallRectangleCropImage);
            drawMarkImage.Image = image;

            CircleDetectionPoint.Close();
        }


        #region get and set for x-value and y-value
        public int xPose, yPose;
        public void setXValue(int x)
        {
            xPose = x;
        }
        public int getXValue()
        {
            return xPose;
        }
        // get and set for y-value
        public void setYValue(int y)
        {
            yPose = y;
        }
        public int getYValue()
        {
            return yPose;
        }
        #endregion

        // open an image by clicking the button, then process it
        private void openImageFromFile_Click(object sender, EventArgs e)
        {
            Mat imageOriginal = new Mat();

            OpenFileDialog openImage = new OpenFileDialog();
            openImage.Filter = "JPEG|*.jpg"; // list of format to save image
            openImage.DefaultExt = @"C:\Users\Anish\OneDrive - Texas Southern University\Texas Southern University\Summer 2016\Sample Scranton";
            if (openImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openImage.FileName;

                imageOriginal = CvInvoke.Imread(fileName, LoadImageType.Unchanged);

                imageTransformation(sender, e, imageOriginal);
                //Application.Idle += imageTransformation(sender, e, imageOriginal);

            }
        }

        // function to send the data to file, after clicking button on user form
        /*private void dataToFile_Click(object sender, EventArgs e)
        {
            dataFileCondition = true;
        }
        
        // function for button, to pause and resume program
        private void button1_Click(object sender, EventArgs e)
        {
            if (pause == true)
            {
                Application.Idle -= imageTransformation;
                pause = false;
                pauseButton.Text = "Start";
            }
            else
            {
                Application.Idle += imageTransformation;
                pause = true;
                pauseButton.Text = "Pause";
            }
        }*/
    }
}

