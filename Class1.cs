using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Traking;
using prueba;
using OpenCvSharp.Tracking;
using System.Drawing;
using OpenCvSharp.ImgHash;
using Point = OpenCvSharp.Point;
using static System.Net.Mime.MediaTypeNames;
//using Camara.Modbus.Servicio.control;
using System.Diagnostics;
using System.Threading;
using Size = OpenCvSharp.Size;
using static OpenCvSharp.LineIterator;

namespace prueba
{
    internal class Class1
    {
        public static int cont=248;
        static CancellationToken cancelationTokenCloseCameraB;
        public static Mat CameraObjectDetectionB(Mat self, Mat dst)
        {

            try
            {
               
                //Crea una matriz de puntos de opencv
                OpenCvSharp.Point[][] contornos_actual;

                //Crea una lista de indices jerarquicos opencv
                HierarchyIndex[] Indexes_actual;

                //Busca los contornos en una imagen binaria y los guarda en contornos_actual y Indexes_actual
                Cv2.FindContours(dst, out contornos_actual, out Indexes_actual, RetrievalModes.CComp, method: ContourApproximationModes.ApproxSimple);

                //crea un nueva imagen binarizada vacia
                Mat image_binarizada_rgb = new Mat();

                //Lee la imagen de escalas de grises a BGR
                Cv2.CvtColor(dst, image_binarizada_rgb, ColorConversionCodes.GRAY2BGR);

                //Llama al metodo TrackerObjectCameraB
                self = prueba4.TrackerObjecCameraB(self, image_binarizada_rgb, contornos_actual, Indexes_actual);
            }
            catch (Exception ex)
            {
                

            }

            return (self);

        }

        public static Mat PhiltroMorphologyCameraB(Mat self, int kernel)
        {
           

            int kernel_hor = 15;  //15x15
            int kernel_Ver = 15;
            int kernel_circ_1 = 11;  //8x 8
            int kernel_circ_2 = 11;

            Mat SSQUARE_KERNEL_VER = Mat.Ones(MatType.CV_8UC1, kernel_Ver * 1);
            Mat SQUARE_KERNEL_HOR = Mat.Ones(MatType.CV_8UC1, 1 * kernel_hor);
            //Mat SQUARE = Mat.Ones(MatType.CV_8UC1, 3 * 3);
            Mat kernel1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(kernel_circ_1, kernel_circ_2));
            var imagen = new Mat();
           
            Cv2.MorphologyEx(self, imagen, MorphTypes.Open, SSQUARE_KERNEL_VER);
            Cv2.MorphologyEx(imagen, imagen, MorphTypes.Open, SQUARE_KERNEL_HOR);
            Cv2.MorphologyEx(imagen, imagen, MorphTypes.Erode, kernel1, iterations: 2);

            


            //Devuelve la imagen procesada, con dos dilataciones y una erocion.
            return imagen;
        }


        //TODO esta vaina no hace nada, aplica un umbral a los pixeles de 255 para que queden en 255
        public static void CountVarillasCameraB(Mat frame)
        {
            //Se crea una nueva imagen
            Mat dst1 = new Mat();

            //Toma la imagen en RGB y la devuelve en escala de grises y lo guarda en dst1
            Cv2.CvtColor(frame, dst1, ColorConversionCodes.RGB2GRAY);






            //Rect roi = new Rect(800, 580, 500, 470);
            Mat roiImg = dst1;

            //Crea una nueva imagen
            Mat roiSegmentado = new Mat();

            //Se aplica una umbralizacion a la roiImg guarda en roiSegmentado
            Cv2.Threshold(roiImg, roiSegmentado, 255, 255, ThresholdTypes.Binary);

            //Cv2.ImShow("segmentacion", roiSegmentado);

            // Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(40, 5));
            //Mat ImgSegmentado=new Mat();

            // Cv2.Erode(roiSegmentado, ImgSegmentado, kernel);


            Mat open = PhiltroMorphologyCameraB(roiSegmentado, 1);
            // Mat close = PhiltroMorphologyCameraB(roiSegmentado, 2);

            // Cv2.Absdiff(close, open, ImgSegmentado);
            // Cv2.ImShow("filtros", open);

            dst1 = CameraObjectDetectionB(roiImg, open);


            //Cv2.ImShow("jaja", dst1);
            dst1.Dispose();



            Cv2.WaitKey(10);
            GC.Collect();
            // Mostrar la imagen en un control de imagen en el formulario

        }
        static void Main()
        {
            //Temporizador... :3 
            Stopwatch temporizador = new Stopwatch();
            //using (var capture = new VideoCapture("C:\\Users\\Usuario\\Videos\\9mm\\9mmBCA\\9mmBCA.mp4"))
            //using (var capture = new VideoCapture("C:\\Users\\Usuario\\Videos\\9mm\\9mmBCA\\9mmBCA.avi"))
            //using (var capture = new VideoCapture("C:\\Users\\Usuario\\Videos\\videos prueba 3_8 feas\\3_8BCA- Trim.avi"))

            //using (var capture = new VideoCapture("C:\\Users\\Usuario\\Videos\\videos prueba 3_8 feas\\linea B2- Trim.avi"))
            using (var capture = new VideoCapture("C:\\Users\\Usuario\\Videos\\videos prueba 3_8 feas\\contadorB54hz_tornillo.avi"))
            //using (var capture = new VideoCapture("C:\\Users\\Usuario\\Videos\\videos prueba 3_8 feas\\contadorB_tornillo.avi"))
            //using (var capture = new VideoCapture("C:\\Users\\Usuario\\Videos\\videos prueba 3_8 feas\\3_8BCBTrim.avi"))
            {

                temporizador.Start();


                if (!capture.IsOpened())
                {
                    // Si la cámara no se puede abrir, mostrar un mensaje de error
                    Console.WriteLine("No se pudo abrir la cámara");
                    return;
                }
                while (true)
                {

                    using (var frame = new Mat())
                    {
                        //Capturar un fotograma de la cámara
                        capture.Read(frame);

                        // Comprobar si se capturó un fotograma válido
                        if (frame.Empty())
                        {

                        }

                        //Crea dos imagenes
                        Mat dst1 = new Mat();
                        Mat dst2 = new Mat();

                        // Procesar la imagen aquí, por ejemplo, convertirla a escala de grises
                        Cv2.Flip(frame, dst2, FlipMode.Y);
                        Cv2.ImShow("imagen volteada", dst2);
                        Cv2.CvtColor(frame,  dst1, ColorConversionCodes.BGR2GRAY);
                       

                        //Cv2.CvtColor(frame, dst2, ColorConversionCodes.BGR2GRAY);
                        //  Mat HSV = new Mat();
                        //Cv2.Split(dst2, out Mat[] HSV);

                        //Mat H = HSV[0];
                        //Mat S = HSV[1];
                        //Mat V = HSV[2];

                        //Mat invertedImage = new Mat();

                        //Cv2.BitwiseNot(S, invertedImage);
                        //  Cv2.ImShow("imagen invetida S", invertedImage);
                        //Cv2.ImShow("RGBGris", dst1);
                        Mat dst3 = new Mat();

                        Mat imageFloat = new Mat();
                        dst1.ConvertTo(imageFloat, MatType.CV_32F);
                        Cv2.Pow(imageFloat, 1.2, dst3);
                        dst3.ConvertTo(dst3, MatType.CV_8UC1);
                        //dst3 = dst1;
                        Cv2.ImShow("imagen tratada", dst3);

                      

                        // Mostrar el resultado
                    



                        //dst3 = AutoAdjustContrast(dst1);
                        //dst2 = AutoAdjustBrightness((dst1));
                        //dst3 = dst2 - dst1;

                        //Cv2.Multiply(dst1, new Scalar(4), dst3);


                        //dst3 = dst1;//*********************adicionar al codigo**************

                        ////Guarda cada uno de los canales en un arreglo
                        //Mat H = HSV[0];
                        //Mat S = HSV[1];
                        //Mat V = HSV[2];

                        ////Crea una nueva imagen
                        //Mat invertedImage = new Mat();

                        ////Aplica negacion al canal S
                        //Cv2.BitwiseNot(S, invertedImage);

                        // =========================================    Prueba funcional de 70 en adelante ==========================================================
                        
                        //Nueva imagen vacia
                        Mat outputImage = new Mat();

                        // Todos los grises por encima de 70 se suben a 255 y se guardan en la variable outputImgae
                        //Cv2.Threshold(dst1, outputImage, 70, 255, ThresholdTypes.Binary);

                        //Mat segmentacionGRI = new Mat();
                        Mat segmentacionRGB2 = new Mat();
                        //Cv2.Threshold((dst1), segmentacionGRI, 150, 255, ThresholdTypes.Binary);
                        Cv2.Threshold(dst3, segmentacionRGB2, 200, 255, ThresholdTypes.Binary);// valor de threshold 
                                                                                                 // Aplicar umbralización adaptativa para obtener una máscara
                        Mat mask = new Mat();
                        Cv2.AdaptiveThreshold(segmentacionRGB2, mask, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 127, 0);

                        // Sustraer el fondo de la imagen original utilizando la máscara
                        Mat result = new Mat();
                        Cv2.BitwiseAnd(segmentacionRGB2, segmentacionRGB2, result, mask);
                        Cv2.ImShow("Resultado", result);
                        // Mat result = pruebaSegementacion(segmentacionRGB2);
                        //Cv2.ImShow("Segementacion GRIS", segmentacionGRI);
                        //Cv2.ImShow("segmentacion RGB2", segmentacionRGB2);

                        //Rect roi = new Rect(800, 580, 500, 470);
                        //Mat roiImg = dst1;

                        //Cv2.Rectangle(dst1, roi, Scalar.Red, 2);

                        // Cv2.NamedWindow("Roi");

                        Mat roiSegmentado = new Mat();

                       

                        Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));//***adicionar al codigo
                        Mat ImgSegmentado = new Mat();
                        Cv2.MorphologyEx(result, ImgSegmentado, MorphTypes.Open, kernel, iterations: 1);
                        Cv2.MorphologyEx(ImgSegmentado, ImgSegmentado, MorphTypes.Close, kernel, iterations: 2);
                        //Cv2.MorphologyEx(segmentacionGRI, roiSegmentado, MorphTypes.Close, kernel);
                        //Cv2.MorphologyEx(ImgSegmentado, ImgSegmentado, MorphTypes.Open, kernel);
                        //Cv2.MorphologyEx(roiSegmentado, roiSegmentado, MorphTypes.Open, kernel);
                        //Cv2.Erode(roiSegmentado, ImgSegmentado, kernel);
                        //Cv2.ImShow("filtros RGB", roiSegmentado);
                        Cv2.ImShow("close", ImgSegmentado);

                        Mat filtros = new Mat();
                        filtros = PhiltroMorphologyCameraB(ImgSegmentado, 1);
                        /*dst1 = CameraObjectDetectionB(roiImg, open);*/ //*************comentado
                        dst1 = CameraObjectDetectionB(result, filtros);

                        //Hace un closing a la imagen RGB2 y la guarda en ImgSegmentado
                        //Cv2.MorphologyEx(segmentacionRGB2, ImgSegmentado, MorphTypes.Close, kernel);

                   

                       

              

                        //Libera memoria asignada a dst1
                        dst1.Dispose();

                        //Espera un espacio o tecla
                        Cv2.WaitKey();
                        
                        //GarbageCollector
                        GC.Collect();

                        // Detiene el temporizador
                        temporizador.Stop();

                        // Obtiene el tiempo transcurrido como un TimeSpan
                        TimeSpan tiempoTranscurrido = temporizador.Elapsed;

                        // Convierte el tiempo transcurrido a milisegundos
                        double milisegundos = tiempoTranscurrido.TotalMilliseconds;

                    }
                }


            }


        }

        //Mejora de contraste usando CLAHE, convierte la imagen de mono8 en 16 bits sin signo, mejora el contraste y lo devuleve a mono8.
        public static Mat AutoAdjustContrast(Mat image)
        {
            //Crea una nueva imagen
            Mat grayImage16 = new Mat();

            //Convierte la imagen a formato 16bits sin signo
            image.ConvertTo(grayImage16, MatType.CV_16UC1);

            // Aplicar CLAHE a la imagen en escala de grises de 16 bits, sirve para mejorar el contraste
            var clahe = Cv2.CreateCLAHE();

            //El parámetro ClipLimit controla la cantidad de realce de contraste que se aplica
            clahe.ClipLimit = 1;

            //Crea una imagen
            Mat adjustedImage16 = new Mat();

            //Aplica la mejora de contraste
            clahe.Apply(grayImage16, adjustedImage16);

            // Convertir la imagen de vuelta a Mono8
            Mat adjustedImage8 = new Mat();
            adjustedImage16.ConvertTo(adjustedImage8, MatType.CV_8UC1);

            return adjustedImage8;
        }

        public static Mat AutoAdjustBrightness(Mat image)
        {
            // Aplicar CLAHE a la imagen Mono8
            var clahe = Cv2.CreateCLAHE();
            clahe.ClipLimit = 20.0;
            Mat adjustedImage = new Mat();
            clahe.Apply(image, adjustedImage);
            Cv2.ImShow("brillo", adjustedImage);

            return adjustedImage;
        }

        public static Mat ConvertMono8ToRgb(Mat monoImage)
        {
            // Convertir la imagen Mono8 a escala de grises
            Mat grayImage = new Mat();
            Cv2.CvtColor(monoImage, grayImage, ColorConversionCodes.GRAY2BGR);
            Cv2.ImShow("imagen rgb", grayImage);
            return grayImage;
        }

        public static void CheckImageColorSpace(Mat image)
        {
            int imageType = image.Type();

            // Comprobar el espacio de color de la imagen
            if (imageType == MatType.CV_8UC1)
            {
                // Imagen en escala de grises (Mono8)
                Console.WriteLine("La imagen está en escala de grises (Mono8).");
            }
            else if (imageType == MatType.CV_8UC3)
            {
                // Imagen en color RGB
                Console.WriteLine("La imagen está en color RGB.");
            }
            else if (imageType == MatType.CV_8UC4)
            {
                // Imagen en color RGBA
                Console.WriteLine("La imagen está en color RGBA.");
            }
            else
            {
                // Otro espacio de color
                Console.WriteLine("La imagen está en un espacio de color desconocido.");
            }
        }
        




    }
}
