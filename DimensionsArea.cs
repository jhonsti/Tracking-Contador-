


//using Microsoft.VisualBasic;
//using OpenCvSharp;

//using SpinnakerNET;
//using SpinnakerNET.GenApi;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Threading;
//using static System.Net.Mime.MediaTypeNames;
//using prueba;

//namespace Camara.Modbus.Servicio.control
//{

//    public class ControlCameraB
//    {
//        // public static int condicion;

//        public static int contador = 0;
//        public static int contador2 = 0;

//        public static List<OpenCvSharp.Mat> buffer = new List<OpenCvSharp.Mat>();
//        public static int RecorrerBuffer = 0;
//        public static int contreBuffer = 0;
//        //  static ConcurrentQueue<Bitmap> processingQueue = new ConcurrentQueue<Bitmap>();
//        //static ConcurrentQueue<Bitmap> acquisitionBuffer = new ConcurrentQueue<Bitmap>();

//        public static Bitmap ImageProcess;
//        static object lockObj = new object();
//        public static int DisableHeartbeat(IManagedCamera cam, INodeMap nodeMap, INodeMap nodeMapTLDevice)
//        {
//            Console.WriteLine("Checking device type to see if we need to disable the camera's heartbeat...\n\n");


//            IEnum iDeviceType = nodeMapTLDevice.GetNode<IEnum>("DeviceType");
//            IEnumEntry iDeviceTypeGEV = iDeviceType.GetEntryByName("GigEVision");
//            // We first need to confirm that we're working with a GEV camera
//            if (iDeviceType != null && iDeviceType.IsReadable)
//            {
//                if (iDeviceType.Value == iDeviceTypeGEV.Value)
//                {

//                    IBool iGEVHeartbeatDisable = nodeMap.GetNode<IBool>("GevGVCPHeartbeatDisable");
//                    if (iGEVHeartbeatDisable == null || !iGEVHeartbeatDisable.IsWritable)
//                    {

//                    }
//                    else
//                    {
//                        iGEVHeartbeatDisable.Value = true;

//                    }
//                }
//                else
//                {
//                    // Console.WriteLine("Camera does not use GigE interface. Resuming normal execution...\n\n");
//                }
//            }
//            else
//            {
//                //Console.WriteLine("Unable to access TL device nodemap. Aborting...");
//                return -1;
//            }

//            return 0;
//        }
//        public static void Capturando(IManagedCamera cam, CancellationToken PeticionPlc)

//        {
//            IManagedImageProcessor processor = new ManagedImageProcessor();
//            //*************** determinar el color de la imagenes en el procesador 
//            processor.SetColorProcessing(ColorProcessingAlgorithm.HQ_LINEAR);
//            Queue<Bitmap> acquisitionBuffer = new Queue<Bitmap>();
//            while (!PeticionPlc.IsCancellationRequested)
//            //for (int imageCnt = 0; imageCnt < NumImages; imageCnt++) guarda un determinado numero de imagenes 
//            {


//                //double timeout = 18446744073709551615;




//                try
//                {


//                    using (IManagedImage rawImage = cam.GetNextImage())

//                    // using (IManagedImage rawImage = cam.GetNextImage(10))//// habia un 10 
//                    {
//                        //
//                        // Ensure image completion



//                        if (rawImage.IsIncomplete)
//                        {
//                            Console.WriteLine("Image incomplete with image status {0}...", rawImage.ImageStatus);
//                        }
//                        else
//                        {

//                            uint width = rawImage.Width;

//                            uint height = rawImage.Height;




//                            //
//                            // Convert image to mono 8 pero se puede cambiar a otro para este ejemplo tenemos RGB8

//                            using (
//                            IManagedImage convertedImage = processor.Convert(rawImage, PixelFormatEnums.Mono8))
//                            {
//                                // Create a unique filename
//                                // String filename = "C:\\Users\\Usuario\\Pictures\\Saved Pictures\\INTECOL.jpg";

//                                //  string filename = CamaraTerniumDA.ObtenerParametro("rutaImagen") + "\\INTECOLB.jpg";

//                                //contador++;
//                                //String filename = "C:\\Users\\intecol\\Pictures\\prueba\\img" + Convert.ToString(contador) + ".jpg";
//                                //// Save image

//                                //convertedImage.Save(filename); //linea de codigo para guardar las imagenes 


//                                Bitmap bmp = convertedImage.bitmap;
//                                // acquisitionBuffer.Enqueue(bmp);



//                                // Convertir el bitmap a un arreglo de bytes
//                                byte[] imageData;
//                                using (MemoryStream stream = new MemoryStream())
//                                {
//                                    bmp.Save(stream, ImageFormat.Bmp);
//                                    imageData = stream.ToArray();
//                                }

//                                // Crear una imagen Mat a partir de los datos de imagen en formato BGR
//                                Mat imagen = Cv2.ImDecode(imageData, ImreadModes.Color);
//                                Class1.CountVarillasCameraB(imagen);



//                                //  ProcessingImageB.CountVarillasCameraB(imagen, _Client);

//                                //if (RecorrerBuffer <= 150)
//                                //{
//                                //    buffer.Insert(RecorrerBuffer, imagen);
//                                //}

//                                //if (RecorrerBuffer == 3)
//                                //{
//                                //    ///hilo
//                                //    var process = new Thread(() => ProcessingB(PeticionPlc));
//                                //    process.Start();
//                                //    contador2++;
//                                //}
//                                //if (contreBuffer == 150)
//                                //{
//                                //    contreBuffer = 0;
//                                //}
//                                //if (RecorrerBuffer >= 150)
//                                //{
//                                //    buffer[contreBuffer] = imagen;
//                                //}


//                                //RecorrerBuffer++;
//                                //contreBuffer++;

//                                // ProcessingImageB.CountVarillasCameraB(imagen);
//                                //Cv2.ImShow("ventana", imagen);
//                                //Cv2.WaitKey(1);
//                                //  imagen.Release();
//                                //GC.Collect();
//                                // acquisitionBuffer.Dequeue();

//                                // Liberar recursos de la imagen procesada
//                                // bmp.Dispose();
//                                // imagen.Release();
//                            }

//                            //Task.Run(() => { ProcessingImageB.CountVarillasCameraB(imagen, _Client); });

//                            //if (contador2 == 0)
//                            //{
//                            //    var process = new Thread(() => ProcessingB( PeticionPlc));
//                            //    process.Start();
//                            //    contador2++;
//                            //}










//                        }
//                    }



//                }

//                catch (SpinnakerException ex)
//                {
//                    Console.WriteLine("Error: {0}", ex.Message);

//                }

//            }
//        }


//        public static void ProcessingB(CancellationToken PeticionPlc)
//        {

//            //int contadorBuffer = 0;
//            //while (!PeticionPlc.IsCancellationRequested)
//            //{

//            //    try
//            //    {

//            //        ProcessingImageB.CountVarillasCameraB(buffer[contadorBuffer]);
//            //        buffer[contadorBuffer].Release();

//            //    }
//            //    catch (SpinnakerException ex)
//            //    {
//            //        Console.WriteLine("Error: {0}", ex.Message);
//            //        LoggerFacade.doLog((int)LoggerFacade.NivelLog.EROR, "Error Spinnaker", ex);
//            //    }
//            //    contadorBuffer++;
//            //    if (contadorBuffer == 150)
//            //    {
//            //        contadorBuffer = 0;
//            //    }

//            //}
//        }
//        public static Thread StartTheThread(IManagedCamera cam, CancellationToken PeticionPlc)
//        {
//            var t = new Thread(() => Capturando(cam, PeticionPlc));
//            t.Priority = ThreadPriority.Highest;
//            t.Start();
//            return t;
//        }


//        public static int AcquireImagesCameraB(IManagedCamera cam, INodeMap nodeMap, INodeMap nodeMapTLDevice, CancellationToken PeticionPlc, string serialCam1)
//        {
//            int result = 0;


//            try
//            {

//                IEnum iAcquisitionMode = nodeMap.GetNode<IEnum>("AcquisitionMode");


//                // Retrieve entry node from enumeration node
//                IEnumEntry iAcquisitionModeContinuous = iAcquisitionMode.GetEntryByName("Continuous");

//                // Set symbolic from entry node as new value for enumeration node
//                iAcquisitionMode.Value = iAcquisitionModeContinuous.Symbolic;

//                Console.WriteLine("Acquisition mode set to continuous...");

//#if DEBUG
//                Console.WriteLine("\n\n*** DEBUG ***\n\n");
//                // If using a GEV camera and debugging, should disable heartbeat first to prevent further issues

//                //if (DisableHeartbeat(cam, nodeMap, nodeMapTLDevice) != 0)
//                //{
//                //    return -1;
//                //}

//                Console.WriteLine("\n\n*** END OF DEBUG ***\n\n");
//#endif
//                //****** adquisicion de imagenes de manera continua
//                cam.BeginAcquisition();

//                Console.WriteLine("Acquiring images...");

//                String deviceSerialNumber = "";

//                IString iDeviceSerialNumber = nodeMapTLDevice.GetNode<IString>("DeviceSerialNumber");
//                if (iDeviceSerialNumber != null && iDeviceSerialNumber.IsReadable)
//                {
//                    deviceSerialNumber = iDeviceSerialNumber.Value;

//                    Console.WriteLine("Device serial number retrieved as {0}...", deviceSerialNumber);
//                }
//                Console.WriteLine();



//                //********************** capturar imagenes y guardar en funcion de lo que envie modbus********


//                StartTheThread(cam, PeticionPlc);
//                //cam.EndAcquisition();
//                //cam.DeInit();
//            }
//            catch (SpinnakerException ex)
//            {
//                Console.WriteLine("Error: {0}", ex.Message);


//                result = -1;
//            }

//            return result;
//        }






//        public static int RunSingleCameraB(IManagedCamera cam, CancellationToken PeticionPlc, string serialCam1)
//        {
//            int result = 0;

//            try
//            {
//                // Retrieve TL device nodemap and print device information
//                INodeMap nodeMapTLDevice = cam.GetTLDeviceNodeMap();

//                // result = PrintDeviceInfo(nodeMapTLDevice);

//                // Initialize camera
//                cam.Init();

//                // Retrieve GenICam nodemap
//                INodeMap nodeMap = cam.GetNodeMap();

//                // Frame rate
//                cam.AcquisitionFrameRateEnable.Value = true;
//                cam.AcquisitionFrameRate.Value = 13.00;




//                // Ganancia
//                cam.GainAuto.Value = "Off";
//                cam.Gain.Value = 10.0;

//                // Gamma
//                cam.GammaEnable.Value = false;

//                //Roi
//                cam.Width.Value = 572;
//                cam.Height.Value = 600;

//                cam.OffsetX.Value = 808;
//                cam.OffsetY.Value = 248;




//                // Acquire images
//                result = result | AcquireImagesCameraB(cam, nodeMap, nodeMapTLDevice, PeticionPlc, serialCam1);



//                cam.DeInit();


//            }
//            catch (SpinnakerException ex)
//            {
//                //Escribir log de eventos, no inicio la camara
//                Console.WriteLine("Error: {0}", ex.Message);

//                result = -1;
//            }

//            return result;
//        }



//        public static int MainCameraB(string serialCam1, CancellationToken ParametroModbus)
//        {

//            int result = 0;

//            ManagedSystem system = new ManagedSystem();

//            // Retrieve list of cameras from the system

//            ManagedCameraList camList1 = system.GetCameras();

//            IManagedCamera cam1 = camList1.GetBySerial(serialCam1);

//            if (cam1 != null)
//            {
//                try
//                {
//                    result = result | RunSingleCameraB(cam1, ParametroModbus, serialCam1);
//                }
//                catch (SpinnakerException ex)
//                {


//                    Console.WriteLine("Error: {0}", ex.Message);
//                    result = -1;
//                    //EScribir en base de datos Log de eventos 
//                }
//            }

//            // Clear camera list before releasing system
//            camList1.Clear();

//            // Release system
//            system.Dispose();

//            //********Escribir log de eventos "termino la captura de imagenes"
//            return result;
//        }
//    }
//}

////using OpenCvSharp;
////using System.Linq;
////using System.Reflection.Metadata.Ecma335;

////namespace prueba
////{
////    internal class prueba4
////    {
////        public static int id_cont = 0;
////        public static int contador = 0;
////        public static int posXanterior = 0;
////        public static int posYanterior = 0;
////        public static int posXActual = 0;
////        public static int posYActual = 0;
////        public static int contorAnterior = 0;
////        public static bool PLCimg_morfologica_display = true;
////        public static int posicionX = 0;

////        public static Dictionary<int, (int, int, int)> objetos = new Dictionary<int, (int, int, int)>();
////        //public static List<int> objetosAEliminar = new List<int>(objetos.Keys);
////        //public static Dictionary<int, Tuple<int, int, int>> objetosNuevos = new Dictionary<int, Tuple<int, int, int>>();
////        public static Mat TrackerObjecCameraB(Mat image_rgb, Mat image_binarizada_rgb, Point[][] Contornos, HierarchyIndex[] Indexes)
////        {


////            //Referencias referenciaB = NReferencia.ObtenerReferenciaB(mainB.IdReceta)[0];
////            int dist_umbral_below = 0;
////            int dist_umbral_above = 70;
////            double porcentaje_umbral_X = 0.5;
////            int Area_umbral = 20;
////            int width_umbral = 100;
////            int height_humbral = 100;
////            int img_morfologica_display = 1;

////            var lista_aux = new List<int>();
////            if (Contornos.Length == 0)
////            {
////                Console.Clear();
////                id_cont = 0;

////            }
////            var sortedContours = Contornos.OrderByDescending(contour => Cv2.BoundingRect(contour).X); //para camara B

////            //if (primerContorno != null)
////            //{
////            //    var rectangulo = Cv2.BoundingRect(primerContorno);
////            //    posicionX = rectangulo.X;
////            //}                                                        //  bool img_morfologica_display = true;
////            var numcontornos = Contornos.Length;
////            var restacontor = contorAnterior - numcontornos;

////            if (restacontor == 0)
////            {
////                posicionX = objetos.Count;
////            }



////            if (restacontor > 0)
////            {

////                for (int i = 0; i < restacontor; i++)
////                {
////                    objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);// para B
////                    var primerElem = objetos.First();//para B eliminar el primero
////                    objetos.Remove(primerElem.Key);

////                }


////            }
////            contorAnterior = numcontornos;

////            var objetosNuevos = new Dictionary<int, (int, int, int)>();
////            var mismoobjeto = false;
////            int x_umbral = (int)(image_rgb.Width * porcentaje_umbral_X);



////            Cv2.Line(image_rgb, x_umbral, 2, x_umbral, image_rgb.Height - 1, Scalar.Blue);
////            Cv2.PutText(image_rgb, Convert.ToString(contador), new Point(x_umbral + 30, x_umbral / 2), HersheyFonts.Italic, 2, Scalar.White);

////            int counter = 0;
////            int count_obj = 0;
////            //if (Indexes.Length > 0)
////            //{


////            for (var contourIndex = 0; contourIndex < sortedContours.Count(); contourIndex++)//para recorer B y A
////            {
////                var contour = sortedContours.ElementAt(contourIndex);


////                var Area = Cv2.ContourArea(contour);
////                Mat Rgb_ig = image_rgb;

////                if (Area > Area_umbral)
////                {
////                    //Cv2.DrawContours(src, Contornos, -1, 255, 1);
////                    var boundingRect = Cv2.BoundingRect(contour);
////                    var x = boundingRect.X;
////                    var y = boundingRect.Y;
////                    var w = boundingRect.Width;
////                    var h = boundingRect.Height;
////                    posXActual = x + (w / 2);
////                    posYActual = y + (h / 2);
////                    //var moments = Cv2.Moments(contour);
////                    //posXActual =(int)( moments.M10 / moments.M00);
////                    //posYActual =(int)( moments.M01 / moments.M00);
////                    if (img_morfologica_display == 1)
////                    {
////                        Cv2.Circle(image_binarizada_rgb, new Point(posXActual, posYActual), 2, new Scalar(0, 0, 255));
////                        Cv2.PutText(image_binarizada_rgb, $"pos({posXActual})({posYActual})", new Point(posXActual, posYActual + image_rgb.Height * 0.11), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 255));
////                        Cv2.PutText(image_binarizada_rgb, $"A: ({Area})", new Point(posXActual, posYActual + image_rgb.Height * 0.2), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 255));
////                        Cv2.PutText(image_binarizada_rgb, $"w, h: ({w}),({h})", new Point(posXActual, posYActual + image_rgb.Height * 0.3), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 255));
////                    }

////                    var done = 0;
////                    mismoobjeto = false;

////                    objetos = objetos.OrderBy(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);//para recorrer B

////                    foreach (var (idObjeto, value) in objetos)
////                    {
////                        count_obj = count_obj + 1;

////                        posXanterior = value.Item1;
////                        posYanterior = value.Item2;

////                        done = value.Item3;

////                        var dis = Math.Sqrt(Math.Pow(posXActual - posXanterior, 2) + Math.Pow(posYActual - posYanterior, 2));

////                        dis = Convert.ToInt16(dis);


////                        if (dis >= dist_umbral_below && dis < dist_umbral_above && !lista_aux.Contains(idObjeto) && !mismoobjeto)
////                        {
////                            if (img_morfologica_display == 1)
////                            {
////                                Cv2.PutText(image_binarizada_rgb, Convert.ToString(contourIndex), new Point((posXActual + posXanterior) / 2, (posYActual + posYanterior) / 2 - count_obj * 5), HersheyFonts.Italic, 1.2, new Scalar(255, 255, 255));
////                                Cv2.Line(image_binarizada_rgb, posXActual, posYActual, posXanterior, posYanterior, new Scalar(255, 255, 255));
////                                Cv2.Rectangle(image_binarizada_rgb, new Point(x, y), new Point(x + w, y + h), Scalar.White, 2);
////                            }

////                            Cv2.Rectangle(image_rgb, new Point(x, y), new Point(x + w, y + h), Scalar.Red, 2);



////                            if (done == 0)
////                            {

////                                done = Count(posXanterior, posXActual, x_umbral, w, h, width_umbral, height_humbral);


////                            }
////                            objetos[idObjeto] = (posXActual, posYActual, done);
////                            objetosNuevos[idObjeto] = (posXActual, posYActual, done);
////                            mismoobjeto = true;

////                            lista_aux.Add(idObjeto);

////                        }
////                        else
////                        {
////                            if (img_morfologica_display == 1)
////                            {
////                                Cv2.PutText(image_binarizada_rgb, Convert.ToString(dis), new Point((posXActual + posXanterior) / 2, (posYActual + posYanterior) / 2 - 10), HersheyFonts.Italic, 0.5, new Scalar(255, 255, 255));
////                                Cv2.Line(image_binarizada_rgb, posXActual, posYActual, posXanterior, posYanterior, new Scalar(255, 255, 255));
////                            }

////                        }

////                    }

////                    if (!mismoobjeto)
////                    {
////                        objetosNuevos.Add(id_cont, (posXActual, posYActual, 0));
////                        id_cont++;

////                    }
////                }
////            }
////            //}


////            var valoresUnicos = new List<object>();
////            foreach (var (idObjeto, value) in objetosNuevos)
////            {
////                var objetoAnonimo = new { Item1 = value.Item1, Item2 = value.Item2, Item3 = value.Item3 };
////                if (!valoresUnicos.Any(o => o.Equals(objetoAnonimo)))
////                {
////                    var tupla = (objetoAnonimo.Item1, objetoAnonimo.Item2, objetoAnonimo.Item3);
////                    objetos[idObjeto] = tupla;
////                    valoresUnicos.Add(objetoAnonimo);
////                }
////            }


////            if (PLCimg_morfologica_display)
////            {
////                Cv2.ImShow("Imagen final B", image_rgb);
////            }

////            if (img_morfologica_display == 1)
////            {
////                Cv2.ImShow("Imagen final B", image_rgb);
////                Cv2.ImShow("imagen segmentada Cam B", image_binarizada_rgb);
////            }
////            if (PLCimg_morfologica_display || img_morfologica_display == 1)
////            {
////                Cv2.WaitKey(1);

////            }

////            return image_rgb;
////        }
////        public static int Count(double CoordenadasXAnteriores, double CoordenadasXActuales, int x_umbral, int width, int height, int widht_umbral, int height_umbral)
////        {
////            int NewDone;


////            if (CoordenadasXActuales >= x_umbral && CoordenadasXAnteriores < x_umbral)
////            {

////                NewDone = 1;

////                contador++;
////                if (width >= widht_umbral || height > height_umbral)
////                {
////                    contador++;
////                }
////            }

////            else
////            {
////                NewDone = 0;

////            }


////            return (NewDone);
////        }

////    }



////}

