using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;
using System.Diagnostics;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using WMPLib;
using Juego;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace BodyHero
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Declaracion de variables
        #region "Variables"

        Animaciones animacion = new Animaciones();
        string pathImagenes = Application.ResourceAssembly.Location.ToString().Replace(Application.ResourceAssembly.ManifestModule.Name, "") + @"Images";
        string pathAudio = Application.ResourceAssembly.Location.ToString().Replace(Application.ResourceAssembly.ManifestModule.Name, "") + @"Sonidos";
        bool animacionParticula1 = false;
        bool animacionParticula2 = false;
        bool animacionParticula3 = false;
        bool animacionParticula4 = false;
        bool animacionWarp = false;
        bool animacionGlass = false;
        bool final = false;
        int contador = 0;
        bool primerLoop = true, segundoLoop = false;
        private static WindowsMediaPlayer loop = new WindowsMediaPlayer();
        private static WindowsMediaPlayer loop2 = new WindowsMediaPlayer();
        private static WindowsMediaPlayer loopKick = new WindowsMediaPlayer();
        private static WindowsMediaPlayer loopBeat = new WindowsMediaPlayer();
        private static WindowsMediaPlayer loopBeat2 = new WindowsMediaPlayer();
        private static WindowsMediaPlayer hihat = new WindowsMediaPlayer();
        private static WindowsMediaPlayer bassSideChain1 = new WindowsMediaPlayer();
        private static WindowsMediaPlayer bassSideChain2 = new WindowsMediaPlayer();
        private static WindowsMediaPlayer sintPt1v1 = new WindowsMediaPlayer();
        private static WindowsMediaPlayer sintPt1v2 = new WindowsMediaPlayer();
        private static WindowsMediaPlayer sintPt2v1 = new WindowsMediaPlayer();
        private static WindowsMediaPlayer sintPt2v2 = new WindowsMediaPlayer();

        public static DispatcherTimer timerDeBases = new DispatcherTimer();
        double intervaloBases = 0;

        /// <summary>
        /// Tamaño Máximo, Ancho
        /// </summary>
        private const float RenderAncho = 640.0f;

        /// <summary>
        ///Tamaño Máximo, Altura
        /// </summary>
        private const float RenderAlto = 480.0f;

        /// <summary>
        /// Tamaño de los cuadrados del esqueleto
        /// </summary>
        private const double CuadradoEsqueleto = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double grosorBorde = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private Brush colorRect = Brushes.Blue;

        /// <summary>
        /// Color y grosor de los huesos trackeados
        /// </summary>
        private readonly Pen huesos = new Pen(Brushes.Red, 5);

        /// <summary>
        ///  Sensor de la kinect
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Lugar donde se dibuja el Esqueleto
        /// </summary>
        private DrawingGroup dibujos;

        /// <summary>
        /// Imagen que se va a Mostar(FONDO)
        /// </summary>
        private DrawingImage imagenesDibujadas;
        /// <summary>
        /// Declaracion de objetos y funciones
        /// </summary>
        Objetos clsObjeto = new Objetos();
        Funciones clsFunciones;
        public static bool sonar = false;

        /// <summary>
        /// ronometro(declaracion)
        /// </summary>
        public static System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// Tiempo de juego, segundo
        /// </summary>
        int Segundo_Actual = 210;

        /// <summary>
        /// Iniciar Juego, todas las fases listas
        /// </summary>

        private static bool Iniciar_Juego = false;
        /// <summary>
        /// Iniciar Juego, rect al lado
        /// </summary>

        bool Iniciar_Paso1 = true;
        /// <summary>
        /// Iniciar Juego, rect arriba
        /// </summary>

        bool Iniciar_Paso2 = false;
        /// <summary>
        /// Iniciar Juego, rect abajo
        /// </summary>
        bool Iniciar_Paso3 = true;
        bool intro = false;
        #endregion

        //Constructor
        public MainWindow()
        {
            InitializeComponent();
            //inicializa todos los componentes y metodos necesarios para el funcionamiento de BodyHero
            Carga();
        }

        /// <summary>
        /// Dibuja Linea Roja En El Borde Cuando Un Punto Sale Del Rango
        /// </summary>
        /// <param name="eskeleto"></param>
        /// <param name="_dibujos"></param>
        private static void RenderClippedEdges(Skeleton eskeleto, DrawingContext _dibujos)
        {
            if (eskeleto.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                _dibujos.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderAlto - grosorBorde, RenderAncho, grosorBorde));
            }

            if (eskeleto.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                _dibujos.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderAncho, grosorBorde));
            }

            if (eskeleto.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                _dibujos.DrawRectangle(Brushes.Red, null, new Rect(0, 0, grosorBorde, RenderAlto));
            }

            if (eskeleto.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                _dibujos.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderAncho - grosorBorde, 0, grosorBorde, RenderAlto));
            }
        }

        /// <summary>
        /// Inicia Sensor de la Kinect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            
            // se crea el contenedor de los dibujos 
            this.dibujos = new DrawingGroup();

            // fuente que entregara los dibujos a la pantalla
            this.imagenesDibujadas = new DrawingImage(this.dibujos);

            // imagen a dibujar
            Image.Source = this.imagenesDibujadas;

            //busca los sensores conectados 

            foreach (var buscandoSensor in KinectSensor.KinectSensors)
            {
                if (buscandoSensor.Status == KinectStatus.Connected)
                {
                    // asigna el sensor  conectado encontrado
                    this.sensor = buscandoSensor;
                    break;
                }
            }         

            if (null != this.sensor)
            {
                // se inicia el canal de datos de eskeleto
                this.sensor.SkeletonStream.Enable();

               // se inscribe el evento para los cuadros del eskeleto
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

               
                try
                {
                   // segundoLoop inicia else sensor
                    this.sensor.Start();
                    clsFunciones = new Funciones(sensor);
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }



        }

        /// <summary>
        /// Objetos que se Cargan al abrir la Aplicacion
        /// </summary>
        private void Carga()
        {
            //Aqui se deben cargar todos los objetos que luego dibujaremos
            Carga_Objetos();

            //Partes del cuerpo que se usaran para la interaccion con el juego y un valor boolean inicializado en false para indicar que no esta en colision.
            Objetos.Objeto_Cuerpo objeto_cuerpo = new Objetos.Objeto_Cuerpo();
            objeto_cuerpo.tipo = JointType.HandLeft;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.WristLeft;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.HandRight;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.WristRight;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.HipLeft;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.HipRight;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.FootLeft;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.AnkleLeft;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.FootRight;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);

            objeto_cuerpo.tipo = JointType.AnkleRight;
            clsObjeto.ObjetosCuerpo.Add(objeto_cuerpo.tipo, objeto_cuerpo);


            //Declaro eventos
            clsObjeto.event_Colision += new Objetos.d_Colision(Colision);
          
            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = new TimeSpan(0, 0, 1);
            timerDeBases.Tick += new EventHandler(timerDeBases_Tick);
            timerDeBases.Interval = new TimeSpan(0, 0, 0, 0, 100);

            // objetos usados en la introduccion
            Objetos.Objeto objeto = new Objetos.Objeto();
            objeto.id_Objeto = "Objeto1Paso1";
            objeto.rect_ini.X = 112;
            objeto.rect_ini.Y = 230;
            objeto.rect_ini.Size = new Size(50, 50);
            objeto.isEvent = false;
            objeto.isInteractuable = false;
            clsObjeto.dObjetos.Add(objeto.id_Objeto, objeto);
            objeto.id_Objeto = "Objeto1Paso2";
            objeto.rect_ini.X = 490;
            objeto.rect_ini.Y = 230;
            objeto.rect_ini.Size = new Size(50, 50);
            objeto.isEvent = false;
            objeto.isInteractuable = false;
            clsObjeto.dObjetos.Add(objeto.id_Objeto, objeto);

            objeto.id_Objeto = "Objeto2Paso1";
            objeto.rect_ini.X = 188;
            objeto.rect_ini.Y = 391;
            objeto.rect_ini.Size = new Size(50, 50);
            objeto.isEvent = false;
            objeto.isInteractuable = false;
            clsObjeto.dObjetos.Add(objeto.id_Objeto, objeto);
            objeto.id_Objeto = "Objeto2Paso2";
            objeto.rect_ini.X = 368;
            objeto.rect_ini.Y = 391;
            objeto.rect_ini.Size = new Size(50, 50);
            objeto.isEvent = false;
            objeto.isInteractuable = false;
            clsObjeto.dObjetos.Add(objeto.id_Objeto, objeto);

            //objeto.id_Objeto = "Objeto3Paso1";
            //objeto.rect_ini.X = 150;
            //objeto.rect_ini.Y = 280;
            //objeto.rect_ini.Size = new Size(50, 50);
            //objeto.isEvent = false;
            //objeto.isInteractuable = false;
            //clsObjeto.dObjetos.Add(objeto.id_Objeto, objeto);
            //objeto.id_Objeto = "Objeto3Paso2";
            //objeto.rect_ini.X = 440;
            //objeto.rect_ini.Y = 280;
            //objeto.rect_ini.Size = new Size(50, 50);
            //objeto.isEvent = false;
            //objeto.isInteractuable = false;
            //clsObjeto.dObjetos.Add(objeto.id_Objeto, objeto);

        }

        /// <summary>
        /// Se lee el xml para cargar objetos al diccionario
        /// </summary>
        public void Carga_Objetos()
        {
            string path = Application.ResourceAssembly.Location.ToString().Replace(Application.ResourceAssembly.ManifestModule.Name, "") + @"Config\configuracion.xml";
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList xmlnode;
            Objetos.Objeto objeto;

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            xmlnode = xmldoc.GetElementsByTagName("item");
            for (int i = 0; i <= xmlnode.Count - 1; i++)
            {
                objeto = new Objetos.Objeto();
                objeto.rect_ini.Size = new Size(50, 50);
                objeto.isEvent = false;
                objeto.isInteractuable = true;
                objeto.tiempo_interaccion = 2;
                string nombre = "";
                try
                {
                    for (int j = 0; j <= xmlnode[i].ChildNodes.Count - 1; j++)
                    {
                        switch (xmlnode[i].ChildNodes.Item(j).Name)
                        {
                            case "nombre_objeto":
                                objeto.id_Objeto = xmlnode[i].ChildNodes.Item(j).LastChild.Value.Trim();
                                nombre = xmlnode[i].ChildNodes.Item(j).LastChild.Value.Trim();
                                break;
                            case "ruta_audio":
                                objeto.PathAudio = xmlnode[i].ChildNodes.Item(j).LastChild.Value.Trim();
                                break;
                            case "seg_final":
                                objeto.seg_final = Convert.ToInt32(xmlnode[i].ChildNodes.Item(j).LastChild.Value.Trim());
                                break;
                            case "pos_ini":
                                double x = Convert.ToDouble(xmlnode[i].ChildNodes.Item(j).LastChild.Value.Split(',')[0]);
                                double y = Convert.ToDouble(xmlnode[i].ChildNodes.Item(j).LastChild.Value.Split(',')[1]);
                                objeto.rect_ini.X = x;
                                objeto.rect_ini.Y = y;

                                objeto.rect_fin.X = clsObjeto.return_PosFinal_Objeto(new System.Drawing.Point((int)objeto.rect_ini.X, (int)objeto.rect_ini.Y)).X;
                                objeto.rect_fin.Y = clsObjeto.return_PosFinal_Objeto(new System.Drawing.Point((int)objeto.rect_ini.X, (int)objeto.rect_ini.Y)).Y;
                                break;
                            case "life_time":
                                objeto.life_time = Convert.ToDouble(xmlnode[i].ChildNodes.Item(j).LastChild.Value.Trim());
                                break;
                            case "animacion":
                                objeto.animacion = Convert.ToInt16(xmlnode[i].ChildNodes.Item(j).LastChild.Value.Trim());
                                break;
                        }
                        objeto.seg_aparicion = objeto.seg_final - (int)objeto.life_time;
                    }
                }
                catch (Exception e)
                {

                    string m = e.Message;
                }


                clsObjeto.dObjetos.Add(objeto.id_Objeto, objeto);
            }
        }

        /// <summary>
        /// Va aumentando en un segundo el tiempo de juego
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Timer1_Tick(object sender, EventArgs e)
        {
            Segundo_Actual++;
        }

        /// <summary>
        /// Apaga Sensor de la Kinect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        public void tocarAudio()
        {
            if (primerLoop && (loop2.playState == WMPPlayState.wmppsStopped || loop2.playState == WMPPlayState.wmppsReady || loop2.playState == WMPPlayState.wmppsUndefined))
            {
                loop.URL = pathAudio + @"\bajos (celeste)\bass 1 (pt1).wav";
                segundoLoop = true;
                primerLoop = false;
            }
            else if (segundoLoop && (loop.playState == WMPPlayState.wmppsStopped || loop.playState == WMPPlayState.wmppsReady || loop.playState == WMPPlayState.wmppsUndefined))
            {
                loop2.URL = pathAudio + @"\bajos (celeste)\bass 1 (pt2).wav";
                segundoLoop = false;
                primerLoop = true;
            }

        }

        /// <summary>
        /// metodo del evento SkeletonFrameReady que se encarga de los cuadros de la animacion del eskeleto 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
 
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            Skeleton[] skeletons = new Skeleton[0];
            
            // manejo de los cuadros del eskeleto
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);

                }
            }

            // dibjando 
            using (DrawingContext dc = this.dibujos.Open())
            {
                //fondo
                dc.DrawImage(Animaciones.dibujar_fondoAnimado(), new Rect(0.0, 0.0, RenderAncho, RenderAlto));

                
                if (!intro || !Iniciar_Juego)
                {
                    dc.DrawImage(animacion.Dibujar_Intro(intro), new Rect(0.0, 0.0, RenderAncho, RenderAlto));
                }
                else
                {
                    Timer1.Start();
                    timerDeBases.Start();

                }
                clsFunciones.escribirPantalla(dc, Segundo_Actual.ToString(), 30, new Point(5, 5), Brushes.White);

                if (animacionWarp)
                {
                    dc.DrawImage(animacion.DibujarWarp(out final), new Rect(0, 0, 640, 480));
                    animacionWarp = !final;
                }
                if (animacionGlass)
                {
                    dc.DrawImage(animacion.DibujarGlass(out final), new Rect(0, 0, 640, 480));
                    animacionGlass = !final;
                }
                if (animacionParticula1)
                {
                    dc.DrawImage(animacion.DibujarParticulaVerde(out final), new Rect(0, 0, 640, 480));
                    animacionParticula1 = !final;
                }
                if (animacionParticula2)
                {
                    dc.DrawImage(animacion.DibujarParticulaAzul(out final), new Rect(0, 0, 640, 480));
                    animacionParticula2 = !final;
                }
                if (animacionParticula3)
                {
                    dc.DrawImage(animacion.DibujarParticulaCeleste(out final), new Rect(0, 0, 640, 480));
                    animacionParticula3 = !final;
                }
                if (animacionParticula4)
                {
                    dc.DrawImage(animacion.DibujarParticulaNaranja(out final), new Rect(0, 0, 640, 480));
                    animacionParticula4 = !final;
                }

                //previene que la imagen no salga del cuadro en el que se esta dibujando
                this.dibujos.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderAncho, RenderAlto));
                if (clsFunciones.skeleton != null)
                {
                    if (Iniciar_Juego == true)
                    {
                        tocarBase();
                        tocarKickLoop();
                        tocarBeatLoop();
                        tocarBeat2Loop();
                        tocarHihatLoop();
                        tocarBassSideChain();
                        tocarSintePT1();
                        tocarSintePT2();
                        clsObjeto.Colision(Segundo_Actual);

                        foreach (string item in clsObjeto.dObjetos.Keys)
                        {
                            //Mostramos los objetos si son igual al tiempo establecido en el xml
                            Objetos.Objeto objeto = clsObjeto.dObjetos[item];


                            if (Segundo_Actual >= objeto.seg_aparicion && Segundo_Actual < objeto.seg_aparicion + objeto.life_time)
                            {
                                //Punto que da el "Movimiento"                                
                                clsObjeto.dibujarRectMov(dc, item, Segundo_Actual);
                                clsObjeto.mueve_punto(item);
                            }
                            else if (Segundo_Actual >= objeto.seg_aparicion + objeto.life_time)
                            {
                                clsObjeto.EliminarObjeto_Dibujado(item);
                            }
                        }
                    }
                    else
                    {
                        if (Iniciar_Paso1 == true)
                        {
                            Dibujar_Pasos(dc, 1);
                        }
                        else if (Iniciar_Paso2 == true)
                        {
                            Dibujar_Pasos(dc, 2);
                        }

                        if (clsObjeto.Detecta_Pasos("Objeto1Paso1", "Objeto1Paso2"))
                        {
                            Iniciar_Paso1 = false;
                            Iniciar_Paso2 = true;
                            intro = true;
                        }
                        else if (clsObjeto.Detecta_Pasos("Objeto2Paso1", "Objeto2Paso2"))
                        {
                            Iniciar_Paso2 = false;
                            Iniciar_Juego = true;
                        }
                    }
                }

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                            clsFunciones.skeleton = skel;
                            clsObjeto.clsFunciones = clsFunciones;
                            animacion.dcGif = dc;
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawRectangle(this.colorRect, null, new Rect(clsFunciones.SkeletonPointToScreenPoint(skel.Position), new Size(CuadradoEsqueleto, CuadradoEsqueleto)));
                        }
                    }
                }
            }
            if (Segundo_Actual == 215) {
                this.Close();
            }
        }

        /// <summary>
        /// Dibuja el Hueso(linea entre dos Joints), no las dibuja si no estan trackeados
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="drawingContext"></param>
        /// <param name="jointType0"></param>
        /// <param name="jointType1"></param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            if (joint0.TrackingState != JointTrackingState.NotTracked && joint1.TrackingState != JointTrackingState.NotTracked)
            {
                drawingContext.DrawLine(huesos, clsFunciones.SkeletonPointToScreenPoint(joint0.Position), clsFunciones.SkeletonPointToScreenPoint(joint1.Position));
            }
        }

        /// <summary>
        /// Dibuja los huesos(Lineas entre Joints) y los Joints
        /// </summary>
        /// <param name="eskeleto"></param>
        /// <param name="drawingContext"></param>
        private void DrawBonesAndJoints(Skeleton eskeleto, DrawingContext drawingContext)
        {
            if (Iniciar_Paso2 || Iniciar_Juego)
            {
                // Render cabeza
                this.DrawBone(eskeleto, drawingContext, JointType.Head, JointType.ShoulderCenter);

                // resto de torso
                this.DrawBone(eskeleto, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
                this.DrawBone(eskeleto, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
                this.DrawBone(eskeleto, drawingContext, JointType.ShoulderCenter, JointType.Spine);
                this.DrawBone(eskeleto, drawingContext, JointType.Spine, JointType.HipCenter);
                this.DrawBone(eskeleto, drawingContext, JointType.HipCenter, JointType.HipLeft);
                this.DrawBone(eskeleto, drawingContext, JointType.HipCenter, JointType.HipRight);

                // brazo izquierdo
                this.DrawBone(eskeleto, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
                this.DrawBone(eskeleto, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
                this.DrawBone(eskeleto, drawingContext, JointType.WristLeft, JointType.HandLeft);

                // Right Armbrazo derecho
                this.DrawBone(eskeleto, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
                this.DrawBone(eskeleto, drawingContext, JointType.ElbowRight, JointType.WristRight);
                this.DrawBone(eskeleto, drawingContext, JointType.WristRight, JointType.HandRight);

                // pierna izquierda
                this.DrawBone(eskeleto, drawingContext, JointType.HipLeft, JointType.KneeLeft);
                this.DrawBone(eskeleto, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
                this.DrawBone(eskeleto, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

                // pierna derecha
                this.DrawBone(eskeleto, drawingContext, JointType.HipRight, JointType.KneeRight);
                this.DrawBone(eskeleto, drawingContext, JointType.KneeRight, JointType.AnkleRight);
                this.DrawBone(eskeleto, drawingContext, JointType.AnkleRight, JointType.FootRight);
            }
            //imagenes de las uniones 
            BitmapImage joints = new BitmapImage(new Uri(pathImagenes + @"\u2ntitled.png"));

            //dibuja las uniones
            foreach (Joint joint in eskeleto.Joints)
            {
                Brush pincel = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    pincel = colorRect;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    pincel = colorRect;
                }

                if (pincel != null)
                {
                    if (Iniciar_Paso3)
                    {
                        this.colorRect = Brushes.Orange;
                    }
                    if (joint.JointType != JointType.Head)
                    {
                        drawingContext.DrawImage(joints, new Rect(clsFunciones.SkeletonPointToScreenPoint(joint.Position).X - 13, clsFunciones.SkeletonPointToScreenPoint(joint.Position).Y - 13, 26, 26));
                    }
                    else
                    {
                        drawingContext.DrawImage(joints, new Rect(clsFunciones.SkeletonPointToScreenPoint(joint.Position).X - 10, clsFunciones.SkeletonPointToScreenPoint(joint.Position).Y - 10, 25, 25));
                    }
                }
            }

        }

        /// <summary>
        /// Metodo del evento Colision generado en la clase Objetos. Hace que la pista de audio suene y que se muestren las animaciones cuando los colisionan los objetos 
        /// </summary>
        /// <param name="tipoPunto"></param>
        /// <param name="ID_objeto"></param>
        private void Colision(string ID_objeto)
        {
            contador++;

            Objetos.Objeto objeto = clsObjeto.dObjetos[ID_objeto];
            Objetos.Objeto_Dibujado objeto_dibujado = clsObjeto.ObjetosDibujados[ID_objeto];

            if (objeto_dibujado.player == null)
            {
                objeto_dibujado.player = new WindowsMediaPlayer();

                objeto_dibujado.player.URL = objeto.PathAudio;
                objeto_dibujado.player.controls.play();

                switch (objeto.animacion)
                {
                    case 1:
                        animacionWarp = true;
                        break;
                    case 2:
                        animacionParticula1 = true;
                        break;
                    case 3:
                        animacionParticula2 = true;
                        break;
                    case 4:
                        animacionParticula3 = true;
                        break;
                    case 5:
                        animacionParticula4 = true;
                        break;
                    case 6:
                        animacionGlass = true;
                        break;
                }
                objeto_dibujado.player.settings.volume = 150;
            }

            clsObjeto.ObjetosDibujados[ID_objeto] = objeto_dibujado;
        }

        /// <summary>
        /// Poses del inicio, todas las secuencias
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="paso"></param>
        private void Dibujar_Pasos(DrawingContext dc, int paso)
        {
            if (paso == 1)
            {
                clsObjeto.dibujarRect(dc, "Objeto1Paso1");
                clsObjeto.dibujarRect(dc, "Objeto1Paso2");
            }
            else if (paso == 2)
            {
                clsObjeto.dibujarRect(dc, "Objeto2Paso1");
                clsObjeto.dibujarRect(dc, "Objeto2Paso2");
            }
            else if (paso == 3)
            {
                clsObjeto.dibujarRect(dc, "Objeto3Paso1");
                clsObjeto.dibujarRect(dc, "Objeto3Paso2");
            }
        }

        void timerDeBases_Tick(object sender, EventArgs e)
        {
            intervaloBases += 0.1;
        }

        #region baseslistas
        double timingBase = 5.5;
        public void tocarBase()
        {
            if (intervaloBases <= 33)
            {
                if (intervaloBases >= timingBase && intervaloBases <= timingBase + 0.1)
                {
                    if (primerLoop)
                    {
                        loop.settings.volume = 75;
                        loop.URL = pathAudio + @"\bajos (celeste)\bass 1 (pt1).wav";
                        segundoLoop = true;
                        primerLoop = false;
                        timingBase += 3.6;
                    }
                    else if (segundoLoop)
                    {
                        loop2.settings.volume = 75;
                        loop2.URL = pathAudio + @"\bajos (celeste)\bass 1 (pt2).wav";
                        segundoLoop = false;
                        primerLoop = true;
                        timingBase += 3.6;
                    }
                }
            }
        }
        bool sintePT1v1 = true, sintePT1v2 = false;
        public void tocarSintePT1()
        {
            if ((intervaloBases >= 35.5 && intervaloBases <= 38.1) || (intervaloBases >= 108.7 - 0.5 && intervaloBases <= 109.2) || (intervaloBases >= 155.7 - 4.5 && intervaloBases <= 156 - 4.5))
            {
                if (sintePT1v1)
                {
                    sintPt1v1.settings.volume = 75;
                    sintPt1v1.URL = pathAudio + @"\sinte up pt1_a.wav";
                    sintePT1v2 = true;
                    sintePT1v1 = false;
                }
            }
            if ((intervaloBases >= 39.1 && intervaloBases <= 41.7) || (intervaloBases >= 112.4 - 0.5 && intervaloBases <= 115.6) || (intervaloBases >= 159.3 - 4.5 && intervaloBases <= 160.2 - 4.5))
            {
                if (sintePT1v2)
                {
                    sintPt1v2.settings.volume = 75;
                    sintPt1v2.URL = pathAudio + @"\sinte up pt1_b.wav";
                    sintePT1v2 = false;
                    sintePT1v1 = true;
                }
            }
        }
        bool sintePT2v1 = true, sintePT2v2 = false;
        public void tocarSintePT2()
        {
            if ((intervaloBases >= 42.8 && intervaloBases <= 45.1) || (intervaloBases >= 116 - 0.5 && intervaloBases <= 118.5) || (intervaloBases >= 162.9 - 4.5 && intervaloBases <= 163.5 - 4.5))
            {
                if (sintePT2v1)
                {
                    sintPt2v1.settings.volume = 75;
                    sintPt2v1.URL = pathAudio + @"\sinte up pt2_a.wav";
                    sintePT2v2 = true;
                    sintePT2v1 = false;
                }
            }
            if ((intervaloBases >= 46.5 && intervaloBases <= 48.9) || (intervaloBases >= 119.6 - 0.5 && intervaloBases <= 121.9) || (intervaloBases >= 166.5 - 4.5 && intervaloBases <= 167 - 4.5))
            {
                if (sintePT2v2)
                {
                    sintPt2v2.settings.volume = 75;
                    sintPt2v2.URL = pathAudio + @"\sinte up pt2_b.wav";
                    sintePT2v2 = false;
                    sintePT2v1 = true;
                }
            }
        }
        double timingKickLoop = 7.4;
        public void tocarKickLoop()
        {
            if ((intervaloBases >= 0 && intervaloBases <= 20.8) || (intervaloBases >= 77 && intervaloBases <= 92))
            {
                if (intervaloBases >= timingKickLoop && intervaloBases <= timingKickLoop + 2)
                {
                    if (timingKickLoop <= 20.8 || (timingKickLoop >= 80 && timingKickLoop <= 106.8))
                    {
                        loopKick.settings.volume = 75;
                        loopKick.URL = pathAudio + @"\Percusión (azúl)\kick.wav";
                        timingKickLoop += 1.8;
                    }
                }
            }
            else
            {
                timingKickLoop = 80;
            }
        }
        bool primerLoopSide = true, segundoLoopSide = false;
        double timing = 50.5;
        public void tocarBassSideChain()
        {
            if ((intervaloBases >= 50.2 && intervaloBases <= 110) || (intervaloBases >= 124 && intervaloBases <= 185.8))
            {
                if ((intervaloBases >= timing && intervaloBases <= timing + 2))
                {
                    if (primerLoopSide)
                    {
                        bassSideChain1.settings.volume = 75;
                        bassSideChain1.URL = pathAudio + @"\bajos (celeste)\bass sidechain pt1.wav";
                        segundoLoopSide = true;
                        primerLoopSide = false;
                        timing += 3.7;
                    }
                    else if (segundoLoopSide)
                    {
                        bassSideChain2.settings.volume = 75;
                        bassSideChain2.URL = pathAudio + @"\bajos (celeste)\bass sidechain pt2.wav";
                        segundoLoopSide = false;
                        primerLoopSide = true;
                        timing += 3.6;
                    }
                }
            }
            if (intervaloBases >= 115 && intervaloBases <= 118)
            {
                timing = 124;
            }

        }
        double timingHiHat = 50.2;
        public void tocarHihatLoop()
        {
            if ((intervaloBases >= 50.2 && intervaloBases <= 69) || (intervaloBases >= 124 && intervaloBases <= 142.8) || (intervaloBases >= 183 && intervaloBases <= 193))
            {
                if (intervaloBases >= timingHiHat && intervaloBases <= timingHiHat + 1)
                {
                    hihat.settings.volume = 75;
                    hihat.URL = pathAudio + @"\Percusión (azúl)\hi-hat.wav";
                    timingHiHat += 1.7;
                }
            }
            if (intervaloBases >= 69 && intervaloBases <= 70)
            {
                timingHiHat = 124;
            }
            if (intervaloBases >= 142.8 && intervaloBases <= 143.8)
            {
                timingHiHat = 183;
            }
        }
        double timingBeatLoop = 22.6;
        public void tocarBeatLoop()
        {
            if ((intervaloBases >= 22.6 && intervaloBases <= 34.5) || (intervaloBases >= 93.7 && intervaloBases <= 107))
            {
                if (intervaloBases >= timingBeatLoop && intervaloBases <= timingBeatLoop + 1)
                {
                    loopBeat.settings.volume = 75;
                    loopBeat.URL = pathAudio + @"\Percusión (azúl)\beat bombo caja.wav";
                    timingBeatLoop += 1.8;
                }
            }
            if (intervaloBases >= 35 && intervaloBases <= 36)
            {
                timingBeatLoop = 93.7;
            }
        }
        double timingBeatLoop2 = 50;
        public void tocarBeat2Loop()
        {
            if ((intervaloBases >= 50 && intervaloBases <= 76) || (intervaloBases >= 123 && intervaloBases <= 149) || (intervaloBases >= 163 && intervaloBases <= 193))
            {
                if (intervaloBases >= timingBeatLoop2 && intervaloBases <= timingBeatLoop2 + 1)
                {
                    loopBeat2.settings.volume = 75;
                    loopBeat2.URL = pathAudio + @"\Percusión (azúl)\beat dub-step.wav";
                    timingBeatLoop2 += 3.6;
                }
            }

            if (intervaloBases >= 77 && intervaloBases <= 78)
            {
                timingBeatLoop2 = 123;
            }
            if (intervaloBases >= 150 && intervaloBases <= 151)
            {
                timingBeatLoop2 = 163;
            }

        }
        #endregion
    }
}

