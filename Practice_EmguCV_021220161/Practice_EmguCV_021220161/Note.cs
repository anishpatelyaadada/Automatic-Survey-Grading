/*using Emgu.CV;
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
namespace Practice_EmguCV_021220161
{
    class MarkerPoint
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

            CvInvoke.MedianBlur(image, blurred, 9);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                /*for (int c = 0; c < 3; c++)
                {
                    int[] ch = { c, 0 };
                    CvInvoke.MixChannels(image, gray, ch);
                    
                    
                // try several threshold levels
                const int threshold_level = 11;
                CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);
                /* for (int l = 0; l < threshold_level; l++)
                 {
                     // Use Canny instead of zero threshold level!
                     // Canny helps to catch squares with gradient shading
                     if (l == 0)
                     {
                CvInvoke.Canny(gray, gray0, 10, 255, 3);

                Mat erodeElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
                // Dilate helps to remove potential holes between edge segments
                CvInvoke.Dilate(gray0, testImage, erodeElement, new Point(-1, -1), 0, BorderType.Default, new MCvScalar());
                }
                // big error in else statement
                else
                {
                   // MessageBox.Show("good");
                    CvInvoke.Threshold(gray, gray0, 0, 255, ThresholdType.Mask);
                    CvInvoke.Canny(gray0, gray, 0, 255, 3);
                    Mat erodeElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));

                    // Dilate helps to remove potential holes between edge segments
                    CvInvoke.Dilate(gray, testImage, erodeElement, new Point(-1, -1), 2, BorderType.Default, new MCvScalar());
                }
                
                // Find contours and store them in a list
                CvInvoke.FindContours(gray0, contours, null, RetrType.Ccomp, ChainApproxMethod.ChainApproxSimple);
                using (VectorOfPoint approx = new VectorOfPoint())
                {

                    for (int i = 0; i < contours.Size; i++)
                    {
                        // approximate contour with accuracy proportional
                        // to the contour perimeter
                        CvInvoke.ApproxPolyDP(contours[i], approx, CvInvoke.ArcLength((contours[i]), true) * 0.02, false);

                        if (approx.Size == 4 /*&& Math.Abs(CvInvoke.ContourArea(approx)) >200 && CvInvoke.IsContourConvex(approx))
                        {
                            double maxCosine = 0;

                            for (int j = 2; j < 4; j++)
                            {
                                double cosine = Math.Abs(angle(approx[j % 4], approx[j - 2], approx[j - 1]));
                                maxCosine = Math.Max(maxCosine, cosine);

                                /* if (maxCosine < 1 && maxCosine >0.99)
                                 {
                                     MessageBox.Show(Convert.ToString(cosine));
                                 }
                            }

                            if (maxCosine < 1)
                            {
                                //MessageBox.Show("aa");
                                squares.Push(approx);
                            }
                        }
                    }
                }
            }
        }

        public Mat debugSquares(VectorOfVectorOfPoint squares, Mat image)
        {
            for (int i = 0; i < squares.Size; i++)
            {
                // draw contour
                // CvInvoke.DrawContours(image, squares, i, new MCvScalar(255, 0, 0), 1, LineType.EightConnected,null, 0,new Point());

                // draw bounding rect
                Rectangle rect = CvInvoke.BoundingRectangle(squares[i]);
                CvInvoke.Rectangle(image, rect, new MCvScalar(0, 255, 0), 1, LineType.EightConnected, 1);

                /*
                   // draw rotated rect
                   RotatedRect minRect = CvInvoke.MinAreaRect((squares[i]));
                   Point[] rect_points = new Point[4];
                   //minRect.points(rect_points);
                   for (int j = 0; j < 4; j++)
                   {
                      CvInvoke.Line(image, rect_points[j], rect_points[(j + 1) % 4], new MCvScalar(0, 0, 255), 1,LineType.EightConnected); // blue
                   }

            }

            return image;
        }

    }
}*/
/* line detection
 
void lineDetection(Mat img, Image<Bgr, byte> byteImage, VectorOfVectorOfPoint square)
        {
            Mat cannyEdge = new Mat();
            Mat imageHSV = new Mat();

            cannyDetection(img, cannyEdge, imageHSV);

            LineSegment2D[] lines = CvInvoke.HoughLinesP(
               cannyEdge,
               1, //Distance resolution in pixel-related units
               Math.PI / 45.0, //Angle resolution measured in radians.
               20, //threshold
               10, //min Line width
               10); //gap between lines
            CvInvoke.HoughLines(cannyEdge, cannyEdge, 1, Math.PI / 45.0, 20, 10, 10);


            Image<Bgr, Byte> lineImage = byteImage;
            foreach (LineSegment2D line in lines)
                lineImage.Draw(line, new Bgr(Color.Green), 2);
            imageOriginal.Image = lineImage;
        }
 */

//overload function to get the contour of image for the detection of rectangle and traiangle and connected to main form
/* void RectangleAndTriangle(Mat img, Image<Bgr, byte> byteImage, int a)
 {

     UMat cannyEdge = new UMat();
     UMat imageHSV = new UMat();
     CvInvoke.CvtColor(byteImage, imageHSV, ColorConversion.Bgr2Gray);

    // for the detection of the edge from the image
     CvInvoke.Canny(imageHSV, cannyEdge, 180, 120);

     roundImage.Image = cannyEdge;

     // pass the image data to function to find the contour from the image and draw respective diagram around them
     #region Find triangles and rectangles and circle, using list, rotata
     List<Triangle2DF> triangleList = new List<Triangle2DF>();
     List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle
     #endregion

     #region to  find contour and draw rectangle
     using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
     {
         // to find the contour from the images
         CvInvoke.FindContours(cannyEdge, contours, null, RetrType.List, ChainApproxMethod.ChainApproxNone);
         int count = contours.Size; // to ge the total deteted contour
         for (int i = 0; i < count; i++)
         {
             using (VectorOfPoint contour = contours[i])
             using (VectorOfPoint approxContour = new VectorOfPoint())
                 {
                     MCvMoments moment = CvInvoke.Moments(contour, false);
                     double area = moment.M00;
                     // determine the approx polygon curve  of contour
                     CvInvoke.ApproxPolyDP(contour, approxContour,Double.Epsilon,true);
                     textBox.Text = Convert.ToString( "approx = " +approxContour.Size + "\n");
                     if (area > 30 || area < 300) //only consider contours with area greater than 250
                     {

                         #region The contour has 3 vertices, it is a triangle
                         if (approxContour.Size == 3)
                         {
                             Point[] pts = approxContour.ToArray();
                             triangleList.Add(new Triangle2DF(pts[0], pts[1], pts[2]));
                         }
                         #endregion
                         #region rectange or square in counter [82, 95] degree
                         else if (approxContour.Size == 4) //The contour has 4 vertices.
                         {
                             //MessageBox.Show("Square started");
                             bool isRectangle = true;
                             Point[] pts = approxContour.ToArray();
                             LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                             for (int j = 0; j < edges.Length; j++)
                             {
                                 double angle = Math.Abs(edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                 if (angle < 80 || angle > 100)
                                 {
                                     isRectangle = false;
                                     break;
                                 }
                             }
                             if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                         }
                         #endregion
                     }
                 }
             }

     }
     #endregion

     Image<Bgr, Byte> triangleRectangleImage = byteImage;
     foreach (Triangle2DF triangle in triangleList)
         triangleRectangleImage.Draw(triangle, new Bgr(Color.Red), 1);
     foreach (RotatedRect box in boxList)
         triangleRectangleImage.Draw(box, new Bgr(Color.Blue), 1);
     //straightImage.Image = triangleRectangleImage;
     //setXValue((int)(moment.M10 / moment.M00));
     //setYValue((int)(moment.M01 / moment.M00));
     //textBox.Text = Convert.ToString("(x,y) = " + getXValue() + " " + getYValue() + "\n");
     // CvInvoke.PutText(byteImage, "marker" + getXValue() + " " + getYValue(), new Point((int)getXValue(), (int)getYValue()), FontFace.HersheyComplex, 1, new MCvScalar(10, 10, 10, 10), 1, LineType.EightConnected, false);
     // byteImage.Draw(box, new Bgr(Color.Blue), 1);
     // straightImage.Image = byteImage;
 }
 */
/* cicrle detection
                               if (approx.Size >= 4 && approx.Size <= 6 && ((getPoint1().X < getPoint4().X) && (getPoint2().X < getPoint3().X)) || ((getPoint1().Y > getPoint4().Y) && (getPoint2().Y > getPoint3().Y)))
                               {
                                   MCvMoments momentCircle = CvInvoke.Moments(contours[i], false);
                                   double areaCircle = momentCircle.M00;

                                   if (areaCircle > 0 || areaCircle < 200)
                                   {
                                       setXValueCircle((int)(momentCircle.M10 / momentCircle.M00));
                                       setYValueCircle((int)(momentCircle.M01 / momentCircle.M00));
                                       Point centerInput = new Point(getXValueCircle(), getYValueCircle());
                                       CvInvoke.Circle(image, centerInput, 5, new MCvScalar(0, 252, 34), 1, LineType.EightConnected, 0);

                                   }
                               }

                               */



                               /*
                                 public void detectedPointsInPaper(VectorOfPoint foundPoint)
        {
           TextWriter PointReceivedForCalculation = new StreamWriter("C:\\Users\\Anish\\Desktop\\PointReceivedForCalculation.txt");
            int size = foundPoint.Size;
            int XchangeableValue = new int();
            int X1 = new int();
            int X2 = new int();
            int X4 = new int();
            int X5 = new int();
            int X1X2Diffs = new int();
            int X4X5Diffs = new int();
            int XAvg = new int();
            
             
            X1 = foundPoint[0].X;
            X2 = foundPoint[1].X;
            X4 = foundPoint[3].X;
            X5 = foundPoint[4].X;

            ///now take the difference of first and second point, third and fift point
            X1X2Diffs = Math.Abs(X1 - X2);
            X4X5Diffs = Math.Abs(X4 - X5);

            // take the average of above two difference point
            XAvg = (X1X2Diffs + X4X5Diffs) / 2;
            PointReceivedForCalculation.WriteLine(Convert.ToString("Received"));

            for (int i = 0; i<size; i++)
            {
               
                 PointReceivedForCalculation.WriteLine(Convert.ToString("Point #" + i + " = " + foundPoint[i] + "   ,Size of found Point = " + foundPoint.Size));
                
               
                // to get the x-axis value of first cross out of total four cross
                if (i == 0 || i == 5 || i == 15 || i == 20 || i == 25 || i == 30 || i == 35)
                {
                    XchangeableValue = foundPoint[i].X;
                }

                // array where the answer is saved
                if (i == 2 || i == 7 || i == 12 || i == 17 || i == 22 || i == 27 || i == 32)
                {
                    if ((foundPoint[i].X > XchangeableValue) && (foundPoint[i].X< (XchangeableValue + XAvg)))
                    {

                    }
                    else if ((foundPoint[i].X > (XchangeableValue + XAvg)) && (foundPoint[i].X< (XchangeableValue + (XAvg* 2))))
                    {

                    }
                    else if ((foundPoint[i].X > (XchangeableValue + (XAvg* 2))) && (foundPoint[i].X< (XchangeableValue + (XAvg* 3))))
                    {

                    }
                    else if ((foundPoint[i].X > (XchangeableValue + (XAvg* 3))) && (foundPoint[i].X< (XchangeableValue + (XAvg* 4))))
                    {

                    }
                    else if ((foundPoint[i].X > (XchangeableValue + (XAvg* 4))) && (foundPoint[i].X< (XchangeableValue + (XAvg* 5))))
                    {

                    }
                }
            }
        }
                                */
                                 /*
                                    for (; colY1 > colY2 | colY3 > colY4; colY2++, colY4++)
                    {
                        // fourCrossPoint.WriteLine(Convert.ToString("Column Start"));

                        for (; rowX2 > rowX1 | rowX4 > rowX3; rowX1++, rowX3++)
                        {
                            //fourCrossPoint.WriteLine(Convert.ToString("Row Start"));
                            diffRow++;
                            
                            if (matrixImage[rowX3, colY4] == 0 && matrixImage[rowX1,colY2] == 0) // black pixel
                            {
                               // fourCrossPoint.WriteLine(Convert.ToString("If condition Start"));
                                b++;
                            }
                            else // white pixcel in an image
                            {
                                a++;
                            }
                        }
                        diffCols++;
                    }
                                  */