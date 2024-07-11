using OpenCvSharp;
using System.Linq;


namespace prueba
{
    internal class prueba4
    {
        public static int id_cont = 0;
        public static int contador = 0;
        public static int posXanterior = 0;
        public static int posYanterior = 0;
        public static int posXActual = 0;
        public static int posYActual = 0;
        public static int contorAnterior = 0;
        public static bool PLCimg_morfologica_display = true;
        public static int posicionX = 0;
        public static bool Error;
        public static int sumaX = 0;

        public static bool borredeldiccionario;

        public static int x_umbral1 = 0;
        public static int x_umbral2 = 0;
        public static int lineaEliminar;
        public static bool elimine;
        public static bool elimine1;

        public static int auxcontador1 = 0;
        public static int auxcontador2 = 0;
        public static int auxcontador3 = 0;

        public static int contadorEntrada = 0;
        public static Boolean banderaVerificador = false;
        public static int PosicionContorno = 0;
        public static int PosicionContornoAnt = 0;
        public static bool banderaEliminarMultiples = false;


        public static Dictionary<int, (int, int, int, int, int)> objetos = new Dictionary<int, (int, int, int, int, int)>();
        public static Mat TrackerObjecCameraB(Mat image_rgb, Mat image_binarizada_rgb, Point[][] Contornos, HierarchyIndex[] Indexes)
        {


            //Referencias referenciaB = NReferencia.ObtenerReferenciaB(mainB.IdReceta)[0];
            int dist_umbral_below = 0;

            int dist_umbral_above = 250;//no se puede poner distancia mayor a 60 
            double porcentaje_umbral_X = 0.6;
            int Area_umbral = 100;
            int width_umbral = 120;
            int height_humbral = 130;


            int img_morfologica_display = 1;
            var lista_aux = new List<int>();
            contadorEntrada = 0;

            //Si no hay contornos se controlan los 3 contadores
            if (Contornos.Length == 0)
            {
                if (auxcontador1 != auxcontador2 && auxcontador1 != auxcontador3 && auxcontador2 != auxcontador3)
                {
                    Error = true;
                }
                else { Error = false; }

                // Limpiamos los objetos.
                Console.WriteLine("limpies el diccionario");
                objetos.Clear();

                //Reiniciamos el contador
                id_cont = 0;

                //resultado conteo y los ordenamos
                int[] valores = { auxcontador1, auxcontador2, auxcontador3 };
                Array.Sort(valores);

                // Comparar los valores para encontrar el que más se repite
                int valorMasFrecuente;
                if (valores[0] == valores[1])
                {
                    valorMasFrecuente = valores[0];
                }
                else if (valores[1] == valores[2])
                {
                    valorMasFrecuente = valores[1];
                }
                else if (valores[0] == valores[2])
                {
                    valorMasFrecuente = valores[0];
                }
                else
                {
                    //valorMasFrecuente = valores[1];
                    valorMasFrecuente = auxcontador2;
                }

                //Se igualan los valores al mas frecuente
                contador = valorMasFrecuente;
                auxcontador1 = valorMasFrecuente;
                auxcontador2 = valorMasFrecuente;
                auxcontador3 = valorMasFrecuente;
            }
            var sortedContours1 = Contornos.OrderByDescending(contour => Cv2.BoundingRect(contour).Y);
            var sortedContours = Contornos.OrderByDescending(sortedContours1 => Cv2.BoundingRect(sortedContours1).X); //para camara B


            //var sortedContours1 = Contornos.OrderBy(contour => Cv2.BoundingRect(contour).Y);
            //var sortedContours = Contornos.OrderBy(sortedContours1 => Cv2.BoundingRect(sortedContours1).X);

            var numcontornos = Contornos.Length;

            //Se hace la resta de contornos
            var restacontor = contorAnterior - numcontornos;                                                                                      //var sortedContours = sortedContours1.OrderBy(sortedContours1 => Cv2.BoundingRect(sortedContours1).X);
                                                                                                                                                  //  bool img_morfologica_display = true;


            //Nueva lista
            var objetosAEliminar = new List<int>(objetos.Keys);

            //Objetos nuevos
            var objetosNuevos = new Dictionary<int, (int, int, int, int, int)>();

            //Boolean
            var mismoobjeto = false;

            //Calcula el umbral
            int x_umbral = (int)(image_rgb.Width * porcentaje_umbral_X);

            x_umbral1 = (int)(image_rgb.Width * 0.5);
            x_umbral2 = (int)(image_rgb.Width * 0.7);
            lineaEliminar = (int)(image_rgb.Width * 0.97);//linea para eliminar 0.009  0.97



            // Se dibujan las lineas
            Cv2.Line(image_rgb, x_umbral, 2, x_umbral, image_rgb.Height - 1, Scalar.Blue);
            Cv2.Line(image_rgb, x_umbral1, 2, x_umbral1, image_rgb.Height - 1, Scalar.Blue);
            Cv2.Line(image_rgb, x_umbral2, 2, x_umbral2, image_rgb.Height - 1, Scalar.Blue);

            // Se dibuja la linea que elimina
            Cv2.Line(image_binarizada_rgb, lineaEliminar, 2, lineaEliminar, image_rgb.Height - 1, Scalar.Blue);

            Cv2.PutText(image_rgb, Convert.ToString(contador), new Point(x_umbral + 20, x_umbral / 2), HersheyFonts.Italic, 1, Scalar.White);
            Cv2.PutText(image_rgb, Convert.ToString(auxcontador1), new Point(x_umbral1 + 10, 150 / 2), HersheyFonts.Italic, 1, Scalar.White);
            Cv2.PutText(image_rgb, Convert.ToString(auxcontador3), new Point(x_umbral2 + 10, 150 / 2), HersheyFonts.Italic, 1, Scalar.White);


            // Se inician dos variables en 0
            int count_obj = 0;

            Console.WriteLine("********************nuevo frame************************");

            int cantidadContornos = sortedContours.Count();
            int verificador = 0;
            banderaVerificador = false;
            for (var contourIndex = 0; contourIndex < cantidadContornos; contourIndex++)//para recorer B y A
            {
                verificador++;
                banderaEliminarMultiples = false;
                if (verificador == cantidadContornos)
                {
                    banderaVerificador = true;
                }
                var contour = sortedContours.ElementAt(contourIndex);
                var ultimiContor = sortedContours.Last();
                //var primercontor = sortedContours.First();
                var Area = Cv2.ContourArea(contour);
                Mat Rgb_ig = image_rgb;

                //Cv2.PutText(image_binarizada_rgb, Convert.ToString(sortedContours.Count()), new Point((posXActual  + posXanterior) / 2, (posYActual + posYanterior) / 2 - count_obj * 5), HersheyFonts.Italic, 1.2, new Scalar(0, 0, 0));
                //Cv2.ImShow("recorido contornos", image_binarizada_rgb);



                if (Area > Area_umbral)
                {
                    //Cv2.DrawContours(image_rgb, Contornos, contourIndex, Scalar.DarkGreen, 2);
                    //var BoundinRectUlt = Cv2.BoundingRect(ultimiContor);
                    var primercontor = sortedContours.First();
                    var BoundinRectUlt = Cv2.BoundingRect(primercontor);
                    var x1 = BoundinRectUlt.X;
                    var y1 = BoundinRectUlt.Y;
                    var w1 = BoundinRectUlt.Width;
                    var h1 = BoundinRectUlt.Height;
                    var momentsUtl = Cv2.Moments(contour);
                    PosicionContorno = x1;

                    var boundingRect = Cv2.BoundingRect(contour);
                    var x = boundingRect.X;
                    var y = boundingRect.Y;
                    var w = boundingRect.Width;
                    var h = boundingRect.Height;
                    int c = ((x / 2));

                    Console.WriteLine();
                    //posXActual = x + (w / 2);
                    //posYActual = y + (h / 2);
                    var moments = Cv2.Moments(contour);
                    posXActual = (int)(moments.M10 / moments.M00);
                    posYActual = (int)(moments.M01 / moments.M00);

                    if (posXActual < 100)
                    {
                        contadorEntrada++;
                    }

                    if (elimine1 == true && posXActual >= lineaEliminar) { Console.WriteLine("salte contorno"); elimine1 = false; continue; }//condicion para saltar contornos 
                    elimine1 = false;
                    //Console.WriteLine("contorno " + Convert.ToString(contourIndex) + " " + Convert.ToString(posXActual));
                    //if (posXActual<lineaEliminar)
                    //{


                    if (img_morfologica_display == 1)
                    {
                        Cv2.Circle(image_binarizada_rgb, new Point(posXActual, posYActual), 2, new Scalar(0, 0, 255));
                        Cv2.PutText(image_binarizada_rgb, $"pos({posXActual})({posYActual})", new Point(posXActual, posYActual + image_rgb.Height * 0.11), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 255));

                        if (Area > 2300)
                        {
                            Cv2.PutText(image_binarizada_rgb, $"A: ({Area})", new Point(posXActual, posYActual + image_rgb.Height * 0.2), HersheyFonts.HersheyScriptComplex, 0.5, Scalar.Red);
                        }
                        else if (Area > 2000)
                        {
                            Cv2.PutText(image_binarizada_rgb, $"A: ({Area})", new Point(posXActual, posYActual + image_rgb.Height * 0.2), HersheyFonts.HersheyScriptComplex, 0.5, Scalar.Yellow);
                        }
                        else if (Area < 400)
                        {
                            Cv2.PutText(image_binarizada_rgb, $"A: ({Area})", new Point(posXActual, posYActual + image_rgb.Height * 0.2), HersheyFonts.HersheyScriptComplex, 0.5, Scalar.Orange);
                        }
                        else
                        {
                            Cv2.PutText(image_binarizada_rgb, $"A: ({Area})", new Point(posXActual, posYActual + image_rgb.Height * 0.2), HersheyFonts.HersheyScriptComplex, 0.5, Scalar.LimeGreen);
                        }
                        Cv2.PutText(image_binarizada_rgb, $"w, h: ({w}),({h})", new Point(posXActual, posYActual + image_rgb.Height * 0.3), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 255));
                        //Cv2.PutText(image_binarizada_rgb, $", h: ({w}),({h})", new Point(posXActual, posYActual + image_rgb.Height * 0.3), HersheyFonts.HersheyScriptComplex, 0.5, new Scalar(255, 255, 255));
                    }

                    var done = 0;
                    var done1 = 0;
                    var done2 = 0;
                    mismoobjeto = false;
                    double dis = 0;

                    //if(Contornos.Length != objetos.Count() && entrada!=0)
                    //{
                    //    objetos.Add(id_cont, ((int)(momentsUtl.M10 / momentsUtl.M00), (int)(momentsUtl.M01 / momentsUtl.M00), 0, 0, 0));
                    //    id_cont++;
                    //    entrada++;
                    //}



                    var objetos1 = objetos.OrderByDescending(kpv => kpv.Value.Item2).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);//para recorrer B
                    objetos = objetos1.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);//para recorrer A
                    //Console.WriteLine("elementos del diciconario " + Convert.ToString(objetos.Count()));
                    int objetoevaluar = 0;
                    //if (objetos.Count()<sortedContours.Count())
                    //{ objetoevaluar = 1; }

                    if (objetos.Count() != 0)
                    {
                        var primerElemob = objetos.First();
                        if ((PosicionContornoAnt > PosicionContorno && objetos.Count() > sortedContours.Count()))
                        {
                            Console.WriteLine("borre por contornos " + Convert.ToString(primerElemob));
                            objetos.Remove(primerElemob.Key);
                            int auxiliarSalida = 0;//sirve para que a lo sumo busque 3 varillas, mas de eso no ocurre.
                            foreach (var data in objetos)
                            {
                                Console.WriteLine("Entre al foreach de eliminar varios");
                                var value = data.Value;
                                var idObjeto = data.Key;
                                auxiliarSalida += 1;
                                posXanterior = value.Item1;
                                Console.WriteLine(posXanterior.ToString(), PosicionContorno.ToString());
                                if ((PosicionContorno - posXanterior) < 5)
                                {
                                    Console.WriteLine("borre por contornos extra" + Convert.ToString(idObjeto));
                                    objetos.Remove(idObjeto);
                                }
                                if (auxiliarSalida == 3)
                                {
                                    break;
                                }
                            }
                        }

                    }
                    Cv2.PutText(image_rgb, Convert.ToString(objetos.Count()), new Point(10, 500), HersheyFonts.Italic, 1, Scalar.White);
                    Cv2.PutText(image_rgb, Convert.ToString(sortedContours.Count()), new Point(30, 500), HersheyFonts.Italic, 1, Scalar.White);
                    Console.WriteLine("contorno anterior: " + PosicionContornoAnt + " contorno actual: " + PosicionContorno);
                    PosicionContornoAnt = PosicionContorno;
                    foreach (var data in objetos)
                    {
                        var value = data.Value;
                        var idObjeto = data.Key;
                        posXanterior = value.Item1;
                        posYanterior = value.Item2;
                        var desplazamientoX = posXActual - posXanterior;
                        done = value.Item3;
                        done1 = value.Item4;
                        done2 = value.Item5;



                        dis = Math.Sqrt(Math.Pow((double)desplazamientoX, 2) + Math.Pow((double)(posYActual - posYanterior), 2));

                        dis = Convert.ToInt16(dis);


                        if (dis >= dist_umbral_below && dis < dist_umbral_above && !lista_aux.Contains(idObjeto) && !mismoobjeto && contourIndex == objetoevaluar && desplazamientoX >= 0)
                        {

                            if (img_morfologica_display == 1)
                            {

                                Cv2.PutText(image_binarizada_rgb, Convert.ToString(contourIndex), new Point((posXActual + 5 + posXanterior) / 2, (posYActual + posYanterior) / 2 - count_obj * 5), HersheyFonts.Italic, 1.2, new Scalar(0, 0, 0));
                                Cv2.Line(image_binarizada_rgb, posXActual, posYActual, posXanterior, posYanterior, new Scalar(255, 255, 255));
                                // Cv2.Rectangle(image_binarizada_rgb, new Point(x, y), new Point(x + w, y + h), Scalar.White, 2);
                                //Cv2.DrawContours(image_rgb, Contornos, contourIndex, Scalar.DarkGreen, 2);
                            }
                            Cv2.PutText(image_rgb, Convert.ToString(contourIndex), new Point(posXActual, posYanterior), HersheyFonts.Italic, 1.2, new Scalar(0, 0, 0));
                            Cv2.PutText(image_rgb, Convert.ToString(dis), new Point(posXActual, posYanterior + 60), HersheyFonts.Italic, 1.2, new Scalar(255, 255, 255));
                            Cv2.PutText(image_rgb, Convert.ToString(idObjeto), new Point(posXActual, posYanterior - 60), HersheyFonts.Italic, 1.2, new Scalar(255, 255, 255));

                            Cv2.Rectangle(image_rgb, new Point(x1, y1), new Point(x1 + w1, y1 + h1), Scalar.Red, 2);
                            Cv2.DrawContours(image_rgb, sortedContours, contourIndex, Scalar.DarkGreen, 2);

                            //==================================================================================================================================================================
                            //==================================================================================================================================================================
                            //==================================================================================================================================================================

                            if (done == 0) //linea a 0.3
                            {
                                Console.WriteLine("entro primera linea ");
                                done = Count(cantidadContornos, posXanterior, posXActual, x_umbral1, w, h, width_umbral, height_humbral, auxcontador1).Item1;
                                auxcontador1 = Count(cantidadContornos, posXanterior, posXActual, x_umbral1, w, h, width_umbral, height_humbral, auxcontador1).Item2;
                            }

                            if (done1 == 0)//Linea a 0.5
                            {
                                Console.WriteLine("entro segunda linea ");

                                done1 = Count(cantidadContornos, posXanterior, posXActual, x_umbral, w, h, width_umbral, height_humbral, auxcontador2).Item1;
                                auxcontador2 = Count(cantidadContornos, posXanterior, posXActual, x_umbral, w, h, width_umbral, height_humbral, auxcontador2).Item2;
                                contador = auxcontador2;
                            }

                            if (done2 == 0)//Linea a 0.7
                            {

                                Console.WriteLine("entro tercera  linea ");
                                done2 = Count(cantidadContornos, posXanterior, posXActual, x_umbral2, w, h, width_umbral, height_humbral, auxcontador3).Item1;
                                auxcontador3 = Count(cantidadContornos, posXanterior, posXActual, x_umbral2, w, h, width_umbral, height_humbral, auxcontador3).Item2;

                            }

                            if ((w > width_umbral || h > height_humbral) && posXActual != posXanterior)
                            {
                                //Comenzo antes de la primera Linea
                                if (posXanterior < x_umbral1)
                                {
                                    //Paso la primera linea
                                    if (x_umbral1 < posXActual && posXActual < x_umbral)
                                    {
                                        if (elimine)
                                        {
                                            if (VerificarUnion(1, cantidadContornos, 1))
                                            {
                                                auxcontador1++;
                                            }
                                        }
                                        else
                                        {
                                            if (VerificarUnion(0, cantidadContornos, 1))
                                            {
                                                auxcontador1++;
                                            }
                                        }
                                    }
                                    //Paso la primera y segunda linea
                                    if (x_umbral < posXActual && posXActual < x_umbral2)
                                    {
                                        if (elimine)
                                        {
                                            if (VerificarUnion(1, cantidadContornos, 1))
                                            {
                                                auxcontador1++;
                                                auxcontador2++;
                                                contador = auxcontador2;
                                            }
                                        }
                                        else
                                        {
                                            if (VerificarUnion(0, cantidadContornos, 1))
                                            {
                                                auxcontador1++;
                                                auxcontador2++;
                                                contador = auxcontador2;
                                            }
                                        }
                                    }
                                    if (x_umbral2 < posXActual)
                                    {
                                        if (elimine)
                                        {
                                            if (VerificarUnion(1, cantidadContornos, 1))
                                            {
                                                auxcontador1++;
                                                auxcontador3++;
                                                auxcontador2++;
                                                contador = auxcontador2;
                                            }
                                        }
                                        else
                                        {
                                            if (VerificarUnion(0, cantidadContornos, 1))
                                            {
                                                auxcontador1++;
                                                auxcontador3++;
                                                auxcontador2++;
                                                contador = auxcontador2;
                                            }
                                        }
                                    }
                                }
                                //comenzo antes de la segunda linea
                                else if (posXanterior < x_umbral)
                                {
                                    //Esta antes de la 3ra linea
                                    if (x_umbral < posXActual && posXActual < x_umbral2)
                                    {
                                        if (elimine)
                                        {
                                            if (VerificarUnion(1, cantidadContornos, 1))
                                            {
                                                auxcontador2++;
                                            }
                                        }
                                        else
                                        {
                                            if (VerificarUnion(0, cantidadContornos, 1))
                                            {
                                                auxcontador2++;
                                            }
                                        }
                                    }
                                    //Esta despues de la 3ra linea
                                    if (x_umbral2 < posXActual)
                                    {
                                        if (elimine)
                                        {
                                            if (VerificarUnion(1, cantidadContornos, 1))
                                            {
                                                auxcontador3++;
                                                auxcontador2++;
                                            }
                                        }
                                        else
                                        {
                                            if (VerificarUnion(0, cantidadContornos, 1))
                                            {
                                                auxcontador3++;
                                                auxcontador2++;
                                            }
                                        }
                                    }
                                }
                                else if (posXanterior < x_umbral2)
                                {

                                    if (x_umbral2 < posXActual)
                                    {
                                        if (elimine)
                                        {
                                            if (VerificarUnion(1, cantidadContornos, 1))
                                            {
                                                auxcontador3++;
                                            }
                                        }
                                        else
                                        {
                                            if (VerificarUnion(0, cantidadContornos, 1))
                                            {
                                                auxcontador3++;
                                            }
                                        }
                                    }
                                }
                            }

                            objetos[idObjeto] = (posXActual, posYActual, done, done1, done2);
                            objetosNuevos[idObjeto] = (posXActual, posYActual, done, done1, done2);
                            mismoobjeto = true;
                            //Cv2.DrawContours(image_binarizada_rgb, sortedContours, contourIndex, Scalar.Red, 2);
                            lista_aux.Add(idObjeto);

                            //Console.WriteLine("contorno " + contourIndex + "(" + posXActual + "," + posYActual + ")" + "(" + posXanterior + "," + posYanterior + ")" + "distancia: " + dis);
                            //Console.WriteLine("");
                            //Console.WriteLine("        ****");
                            //Console.WriteLine("me encontre  " + "contorno " + contourIndex + " posiciones x,y actuales  " + "(" + posXActual + "," + posYActual + ")" + " posiciones x,y anterior en el diccionario " + "(" + posXanterior + "," + posYanterior + ")" + "distancia: " + dis + " posicion el diccionario" + " " + Convert.ToString(objetoevaluar));
                            //Console.WriteLine("        ****");
                            //Console.WriteLine();
                            // break;
                        }
                        else
                        {
                            //Console.WriteLine("");
                            //Console.WriteLine("no me encontre contorno " + contourIndex + "(" + posXActual + "," + posYActual + ")" + "(" + posXanterior + "," + posYanterior + ")" + "distancia: " + dis + "posicion el diccionario" + " " + Convert.ToString(objetoevaluar));

                            if (img_morfologica_display == 1)
                            {
                                //Cv2.DrawContours(image_binarizada_rgb, sortedContours, contourIndex, Scalar.DarkGreen, 2);
                                Cv2.PutText(image_binarizada_rgb, Convert.ToString(dis), new Point((posXActual + posXanterior) / 2, (posYActual + posYanterior) / 2 - 10), HersheyFonts.Italic, 0.5, new Scalar(255, 255, 255));
                                Cv2.Line(image_binarizada_rgb, posXActual, posYActual, posXanterior, posYanterior, new Scalar(255, 255, 255));
                            }

                        }

                        //Console.WriteLine("contorno " + contourIndex + "(" + posXActual + "," + posYActual + ")" + "(" + posXanterior + "," + posYanterior + ")" + "distancia: " + dis + "posicion el diccionario" + " " + Convert.ToString(objetoevaluar));
                        objetoevaluar++;
                    }
                    Console.WriteLine("sali del for");
                    //Console.WriteLine("******************************************************************");
                    //}//if de contorno 
                    if (!mismoobjeto && posXActual < lineaEliminar)
                    {
                        Console.WriteLine();
                        Console.Write("adicione nuevo objeto " + Convert.ToString(porcentaje_umbral_X) + " linea adicionar" + Convert.ToString(porcentaje_umbral_X));
                        objetosNuevos.Add(id_cont, (posXActual, posYActual, 0, 0, 0));
                        id_cont++;

                    }



                }

            }
            // Console.WriteLine("contador: {0} " ,objetos);
            //Console.WriteLine("contador1: " + auxcontador1 + "  contador2: " + auxcontador2 + "  contador3: " + auxcontador3);


            Console.WriteLine("******************************************************************");

            ////}
            foreach (var idObjeto in objetosAEliminar)
            {
                objetos.Remove(idObjeto);
            }

            var valoresUnicos = new List<object>();
            foreach (var (idObjeto, value) in objetosNuevos)
            {
                var objetoAnonimo = new { Item1 = value.Item1, Item2 = value.Item2, Item3 = value.Item3, Item4 = value.Item4, Item5 = value.Item5 };
                if (!valoresUnicos.Any(o => o.Equals(objetoAnonimo)))
                {
                    var tupla = (objetoAnonimo.Item1, objetoAnonimo.Item2, objetoAnonimo.Item3, objetoAnonimo.Item4, objetoAnonimo.Item5);
                    objetos[idObjeto] = tupla;
                    valoresUnicos.Add(objetoAnonimo);
                }
            }

            //Donde se guarda la informacion de los contornos
            if (objetos.Count() != 0)
            {
                //Ordenamos la llaves de forma decendentes
                //objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);// para A y B

                //var primerElem = objetos.First();
                //Console.WriteLine(Convert.ToString("linea " + Convert.ToString(lineaEliminar) + "posicion para eliminar " + primerElem.Value.Item1));

                //if (primerElem.Value.Item1 >= lineaEliminar)
                //{
                //    var posComparar = primerElem.Value.Item1;
                //    Console.WriteLine("borre por linea " + Convert.ToString(primerElem));
                int auxiliarSalida = 0;
                //    foreach (var data in objetos)
                //    {
                //        Console.WriteLine("Entre al foreach de eliminar varios por linea ");
                //        var value = data.Value;
                //        var idObjeto = data.Key;
                //        auxiliarSalida += 1;
                //        posXanterior = value.Item1;
                //        Console.WriteLine(posXanterior.ToString(), PosicionContorno.ToString());
                //        if ((posComparar - posXanterior) < 5)
                //        {
                //            Console.WriteLine("borre por contornos extra por linea " + Convert.ToString(idObjeto));
                //            objetos.Remove(idObjeto);
                //        }
                //        if (auxiliarSalida == 3)
                //        {
                //            break;
                //        }
                //    }
                //    objetos.Remove(primerElem.Key);

                //    elimine = true;

                //}
                //elimine = false;

                objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);// para A y B

                var primerElem = objetos.First();
                if (primerElem.Value.Item1 >= lineaEliminar)
                {
                    objetos.Remove(primerElem.Key);
                    Console.WriteLine("primer borre por linea " + Convert.ToString(primerElem));
                    foreach (var data in objetos)
                    {
                        objetos = objetos.OrderByDescending(kpv => kpv.Value.Item1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                        primerElem = objetos.First();
                        Console.WriteLine("Entre al foreach de eliminar varios por linea ");
                        auxiliarSalida += 1;
                        if (primerElem.Value.Item1 >= lineaEliminar)
                        {
                            Console.WriteLine("borre por linea " + Convert.ToString(primerElem));
                            objetos.Remove(primerElem.Key);
                        }
                        if (auxiliarSalida == 2)
                        {
                            break;
                        }
                    }
                    elimine1 = true;
                    elimine = true;
                }
            }



            if (PLCimg_morfologica_display)
            {
                Cv2.ImShow("Imagen final B", image_rgb);
            }

            if (img_morfologica_display == 1)
            {
                Cv2.ImShow("Imagen final B", image_rgb);
                Cv2.ImShow("imagen segmentada Cam B", image_binarizada_rgb);
            }
            if (PLCimg_morfologica_display || img_morfologica_display == 1)
            {
                // Cv2.WaitKey(1);

            }

            return image_rgb;
        }
        public static (int, int) Count(int cantidadContornos, double CoordenadasXAnteriores, double CoordenadasXActuales, int x_umbral, int width, int height, int widht_umbral, int height_umbral, int contaFuncion)
        {

            int NewDone1;

            ////*******************************segunda linea *********************************************
            if (CoordenadasXActuales >= x_umbral && CoordenadasXAnteriores < x_umbral)
            {
                NewDone1 = 1;

                contaFuncion++;

                if (width >= widht_umbral || height > height_umbral)
                {
                    contaFuncion++;
                }

                int eliminar = 0;

                if (elimine)
                {
                    eliminar++;
                    elimine = false;
                }

                int estadoInicial = objetos.Count() - eliminar;
                int estadoFinal = cantidadContornos - contadorEntrada;

                if (VerificarUnion(estadoInicial, estadoFinal))
                {
                    contaFuncion++;
                }

                if (VerificarSeparacion(estadoInicial, estadoFinal))
                {
                    contaFuncion++;
                }
                else
                {
                    Console.WriteLine("Iguales o no se han visto todos los contornos");
                    Console.WriteLine(contadorEntrada);
                    Console.WriteLine(estadoFinal);
                    Console.WriteLine(estadoInicial);
                }
            }
            else
            {
                NewDone1 = 0;
            }
            return (NewDone1, contaFuncion);
        }
        public static bool VerificarUnion(int estadoInicial, int estadoFinal)
        {
            if (estadoInicial > estadoFinal && banderaVerificador)
            {
                Console.WriteLine("Se juntaron");
                Console.WriteLine(contadorEntrada);
                Console.WriteLine(estadoFinal);
                Console.WriteLine(estadoInicial);
                return true;
            }
            return false;
        }

        public static bool VerificarUnion(int eliminar, int cantidadContornos, int aux)
        {
            int estadoInicial = objetos.Count() - eliminar;
            int estadoFinal = cantidadContornos - contadorEntrada;
            if (estadoInicial > estadoFinal && banderaVerificador)
            {
                Console.WriteLine("Se juntaron");
                Console.WriteLine(contadorEntrada);
                Console.WriteLine(estadoFinal);
                Console.WriteLine(estadoInicial);
                return true;
            }
            return false;
        }

        public static bool VerificarSeparacion(int estadoInicial, int estadoFinal)
        {

            if (estadoInicial < estadoFinal && banderaVerificador)
            {
                Console.WriteLine("Se separaron");
                Console.WriteLine(contadorEntrada);
                Console.WriteLine(estadoFinal);
                Console.WriteLine(estadoInicial);
                return true;
            }
            return false;
        }

    }



}