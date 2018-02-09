using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.Util;
using Emgu.Util.TypeEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


/*
this program file detect the rectangle from the image
also detect and save the co-ordinate point of rectangle and pass to other program file for calculation purpose
*/
namespace Practice_EmguCV_021220161
{
    class RectangleDetection
    {
        public void removeTwoSamePoint(Point input, Point output)
        {

            FileStream inputData = new FileStream("C:\\Users\\Anish\\Desktop\\Data.txt", FileMode.Open);
            FileStream outputFile = new FileStream("C:\\Users\\Anish\\Desktop\\output.txt", FileMode.OpenOrCreate);

            StreamReader readData = new StreamReader(inputData);
            StreamWriter writeData = new StreamWriter(outputFile);
            Console.SetOut(writeData);
            if (readData == null)
            {

                MessageBox.Show("Error opening file");
                string line = readData.ReadToEnd(); Console.WriteLine(line);
            }
            else
            {
                MessageBox.Show("Successful open");
            }



        }
        double angle(Point pt1, Point pt2, Point pt0)
        {
            double dx1 = pt1.X - pt0.X;
            double dy1 = pt1.Y - pt0.Y;
            double dx2 = pt2.X - pt0.X;
            double dy2 = pt2.Y - pt0.Y;
            return (dx1 * dx2 + dy1 * dy2) / Math.Sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10);
        }
        public void findSquares(Mat image, VectorOfVectorOfPoint squares, Mat testImage, double angel)
        {
            squares.Clear();

            Mat blurred = new Mat();
            Mat gray0 = new Mat();
            Mat gray = new Mat();
            // CvInvoke.MedianBlur(image, image, 9);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                const int threshold_level = 11;
                CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);
                for (int l = 0; l < threshold_level; l++)
                {
                    // Use Canny instead of zero threshold level, Canny helps to catch squares with gradient shading
                    if (l == 0)
                    {
                        CvInvoke.Canny(gray, gray0, 0, 200, 5);
                        Mat erodeElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
                        // Dilate helps to remove potential holes between edge segments
                        CvInvoke.Dilate(gray0, testImage, /*erodeElement*/new Mat(), new Point(-1, -1), 0, BorderType.Constant, new MCvScalar());
                    }
                    else
                    { // thresholding to image 
                        CvInvoke.Threshold(gray, gray0, 0, ((l + 1) * 255 / threshold_level), ThresholdType.Otsu);
                    }

                    //Mat hirechy = new Mat();
                    //VectorOfVectorOfPoint hirechy = new VectorOfVectorOfPoint();
                    // Find contours and store them in a list
                    //IOutputArray hirechy;

                    CvInvoke.FindContours(gray0, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                    //contourHirechy(hirechy);

                    #region
                    using (VectorOfPoint approx = new VectorOfPoint())
                    {
                        for (int i = 0; i < contours.Size; i++)
                        {
                            // approximate contour with accuracy proportional
                            // to the contour perimeter
                            CvInvoke.ApproxPolyDP(contours[i], approx, CvInvoke.ArcLength((contours[i]), true) * 0.02, true);
                            // if it is four sided object then process below block of code
                            // object like rectangle, square, and more if possible


                            // i was working on this one
                            //
                            /* for (int j = 0; j < contours.Size; j++)
                             {
                            MCvMoments momentCircle = CvInvoke.Moments(contours[i], true);
                            double areaCircle = momentCircle.M00;

                            if (areaCircle > 0 && areaCircle < 200)
                            {
                                setXValueCircle((int)(momentCircle.M10 / momentCircle.M00));
                                setYValueCircle((int)(momentCircle.M01 / momentCircle.M00));
                                Point centerInput = new Point(getXValueCircle(), getYValueCircle());
                                CvInvoke.Circle(image, centerInput, 5, new MCvScalar(0, 252, 34), 1, LineType.EightConnected, 0);

                            }
                            //}*/

                            if (approx.Size == 4 && (Math.Abs(CvInvoke.ContourArea(approx)) > 1000 && Math.Abs(CvInvoke.ContourArea(approx)) < 63000) && CvInvoke.IsContourConvex(approx))
                            {
                                MCvMoments moment = CvInvoke.Moments(contours[i], false); // moment to get the co-ordinate axis
                                double area = moment.M00; // to ge the area of contour
                                setXValue((int)(moment.M10 / moment.M00)); // get X value of contour
                                setYValue((int)(moment.M01 / moment.M00)); // get Y value of contour
                                                                           // point of center of rectangle or square
                                CvInvoke.PutText(image, /*"marker #"*/+getXValue() + " " + getYValue(), new Point((int)getXValue(), (int)getYValue()), FontFace.HersheyComplex, 0.5, new MCvScalar(255, 0, 150), 1, LineType.FourConnected, false);

                                double maxCosine = 0;
                                for (int j = 2; j < 4; j++)
                                {
                                    double cosine = Math.Abs(angle(approx[j % 4], approx[j - 2], approx[j - 1]));
                                    maxCosine = Math.Max(maxCosine, cosine);
                                }

                                if (maxCosine < 0.3)
                                    squares.Push(approx);

                            }

                        }
                    }
                    #endregion
                }
            }
        }
        public void debugSquares(VectorOfVectorOfPoint squares, Mat image, Image<Bgr, byte> byteImage)
        {
            for (int i = 0; i < squares.Size; i++)
            {
                TextWriter rectangleFile = new StreamWriter("C:\\Users\\Anish\\Desktop\\PaperGradingData\\rectangleFile.txt");

                RotatedRect rectRotate = CvInvoke.MinAreaRect(squares[i]);
                rectangleFile.WriteLine(Convert.ToString("Area of Rectangle = " + rectRotate.MinAreaRect()));
                PointF[] corner = new PointF[4];
                corner = rectRotate.GetVertices();
                rectangleFile.WriteLine(Convert.ToString("Point #1 = " + (int)corner[0].X + " " + (int)corner[0].Y));
                rectangleFile.WriteLine(Convert.ToString("Point #2 = " + (int)corner[1].X + " " + (int)corner[1].Y));
                rectangleFile.WriteLine(Convert.ToString("Point #3 = " + (int)corner[2].X + " " + (int)corner[2].Y));
                rectangleFile.WriteLine(Convert.ToString("Point #4 = " + (int)corner[3].X + " " + (int)corner[3].Y) + "\n");
                
                PointF[] point = new PointF[4];
                for (int j = 0; i < 4; i++)
                {
                    point[i] = corner[i];

                    if(i==0)setPoint1(point[0]);
                    else if (i == 1) setPoint2(point[1]);
                    else if (i == 2) setPoint3(point[2]);
                    else if (i == 3) setPoint4(point[3]);
                    rectangleFile.WriteLine(Convert.ToString("Point #" + j++ + " " + point[i] + "\n"));
                }

                CvInvoke.Line(image, new Point((int)corner[0].X, (int)corner[0].Y), new Point((int)corner[1].X, (int)corner[1].Y), new MCvScalar(255, 0, 0), 1, LineType.EightConnected, 0);
                CvInvoke.Line(image, new Point((int)corner[1].X, (int)corner[1].Y), new Point((int)corner[2].X, (int)corner[2].Y), new MCvScalar(255, 0, 0), 1, LineType.EightConnected, 0);
                CvInvoke.Line(image, new Point((int)corner[2].X, (int)corner[2].Y), new Point((int)corner[3].X, (int)corner[3].Y), new MCvScalar(255, 0, 0), 1, LineType.EightConnected, 0);
                CvInvoke.Line(image, new Point((int)corner[3].X, (int)corner[3].Y), new Point((int)corner[0].X, (int)corner[0].Y), new MCvScalar(255, 0, 0), 1, LineType.EightConnected, 0);

                CvInvoke.PutText(image, /*"marker*/ "1#" + (int)corner[0].X + " " + (int)corner[0].Y, new Point((int)corner[0].X, (int)corner[0].Y), FontFace.HersheyComplex, 0.5, new MCvScalar(211,27,232), 1, LineType.EightConnected, false);
                CvInvoke.PutText(image, /*"marker #" */"2#" + (int)corner[1].X + " " + (int)corner[1].Y, new Point((int)corner[1].X, (int)corner[1].Y), FontFace.HersheyComplex, 0.5, new MCvScalar(211, 27, 232), 1, LineType.EightConnected, false);
                CvInvoke.PutText(image, /*"marker #" */"3#" + (int)corner[2].X + " " + (int)corner[2].Y, new Point((int)corner[2].X, (int)corner[2].Y), FontFace.HersheyComplex, 0.5, new MCvScalar(211, 27, 232), 1, LineType.EightConnected, false);
                CvInvoke.PutText(image, /*"marker #" */"4#" + (int)corner[3].X + " " + (int)corner[3].Y, new Point((int)corner[3].X, (int)corner[3].Y), FontFace.HersheyComplex, 0.5, new MCvScalar(211, 27, 232), 1, LineType.EightConnected, false);

                rectangleFile.Close();
            }
            
        }

        public void contourHirechy(VectorOfVectorOfPoint hirechy)
        {

        }
        #region four co-ordinate points of rectanlge
        public PointF xy1, xy2, xy3,xy4;

        public void setPoint1(PointF xy)
        {
            xy1 = xy;
        }
        public PointF getPoint1()
        {
            return xy1;
        }
        public void setPoint2(PointF xy)
        {
            xy2 = xy;
        }
        public PointF getPoint2()
        {
            return xy2;
        }
        public void setPoint3(PointF xy)
        {
            xy3 = xy;
        }
        public PointF getPoint3()
        {
            return xy3;
        }
        public void setPoint4(PointF xy)
        {
            xy4 = xy;
        }
        public PointF getPoint4()
        {
            return xy4;
        }
        #endregion

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

        #region get and set for x-value and y-value of circle
        public int xPoseCircle, yPoseCircle;
        public void setXValueCircle(int x)
        {
            xPoseCircle = x;
        }
        public int getXValueCircle()
        {
            return xPoseCircle;
        }
        // get and set for y-value
        public void setYValueCircle(int y)
        {
            yPoseCircle = y;
        }
        public int getYValueCircle()
        {
            return yPoseCircle;
        }
        #endregion
    }
}




