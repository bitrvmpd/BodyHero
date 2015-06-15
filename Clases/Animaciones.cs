using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Windows.Media;

namespace Juego
{
    public class Animaciones
    {
        //Declaracion de variables
        #region Variables

        static int index = 0;
        static int indexAnimacion = 0;
        int indexWarp = 0;
        int indexWarp2 = 0;
        int indexGlass = 0;
        int indexParticulasVerde = 0;
        int indexParticulasAzul = 0;
        int indexParticulasCeleste = 0;
        int indexParticulasNaranja = 0;
        int indexIntro = 0;

        Stream imageStreamSource;
        GifBitmapDecoder decoder;
        BitmapSource bitmapSource;
        List<BitmapImage> sprites = new List<BitmapImage>();
        public DrawingContext dcGif;

        List<BitmapImage> warp = new List<BitmapImage>();
        List<BitmapImage> warp2 = new List<BitmapImage>();

        List<BitmapImage> glass = new List<BitmapImage>();

        List<BitmapImage> particulaAzul = new List<BitmapImage>();
        List<BitmapImage> particulaCeleste = new List<BitmapImage>();
        List<BitmapImage> particulaNaranja = new List<BitmapImage>();
        List<BitmapImage> particulaVerde = new List<BitmapImage>();

        static List<BitmapImage> objetoDeToqueVerdeActivo = new List<BitmapImage>();
        static List<BitmapImage> objetoDeToqueVerdeInactivo = new List<BitmapImage>();

        static List<BitmapImage> objetoDeToqueAzulActivo = new List<BitmapImage>();
        static List<BitmapImage> objetoDeToqueAzulInactivo = new List<BitmapImage>();

        static List<BitmapImage> objetoDeToqueMoradoActivo = new List<BitmapImage>();
        static List<BitmapImage> objetoDeToqueMoradoInactivo = new List<BitmapImage>();
        static List<BitmapImage> framesFondoAnimado = new List<BitmapImage>();

        static List<BitmapImage> imagenes_intro = new List<BitmapImage>();
        public static int index_objeto_verde = 0;
        public static int index_objeto_azul = 0;
        public static int index_objeto_morado = 0;
        public static int index_fondoAnimado = 0;

        #endregion

        //Constructor
        public Animaciones()
        {
            cargarElementos();
        }
        public Animaciones(string url)
        {
            imageStreamSource = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.Read);
            decoder = new GifBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            bitmapSource = decoder.Frames[0];

        }

        //Anima cualquier gif que se especifique en el parametro del constructor.
        public BitmapFrame animar()
        {

            if (index == decoder.Frames.Count)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            if (index >= decoder.Frames.Count)
                index = 0;
            return decoder.Frames[index];
        }

        /// <summary>
        /// Carga las imagenes de las animaciones
        /// </summary>
        public void cargarElementos()
        {


            //foreach (string file in Directory.EnumerateFiles(@"C:\Users\vina\Desktop\11-10\BodyHero\Images\bass1", "*.png"))
            //{
            //    sprites.Add(new BitmapImage(new Uri(file)));
            //}

            //Carga la ruta de las imagenes
            string pathEfectos = Application.ResourceAssembly.Location.ToString().Replace(Application.ResourceAssembly.ManifestModule.Name, "") + @"Images";

            //warps
            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\warp", "*.png"))
            {
                warp.Add(new BitmapImage(new Uri(file)));
            }

            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\warp2", "*.png"))
            {
                warp2.Add(new BitmapImage(new Uri(file)));
            }

            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\glass", "*.png"))
            {
                glass.Add(new BitmapImage(new Uri(file)));
            }

            //Particulas
            //verde
            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\particulas_renders\verde", "*.png"))
            {
                particulaVerde.Add(new BitmapImage(new Uri(file)));

            }

            //Azul
            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\particulas_renders\azul", "*.png"))
            {
                particulaAzul.Add(new BitmapImage(new Uri(file)));

            }

            //celeste
            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\particulas_renders\celeste", "*.png"))
            {
                particulaCeleste.Add(new BitmapImage(new Uri(file)));

            }

            //naranja
            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\particulas_renders\naranja", "*.png"))
            {
                particulaNaranja.Add(new BitmapImage(new Uri(file)));

            }


            // objetos de toque 

            //Azul
            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\objetos_renders\objetoAzul\activo", "*.png"))
            {
                objetoDeToqueAzulActivo.Add(new BitmapImage(new Uri(file)));

            }

            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\objetos_renders\objetoAzul\inactivo", "*.png"))
            {
                objetoDeToqueAzulInactivo.Add(new BitmapImage(new Uri(file)));

            }

            //Verde

            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\objetos_renders\objetoVerde\activo", "*.png"))
            {
                objetoDeToqueVerdeActivo.Add(new BitmapImage(new Uri(file)));

            }

            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\objetos_renders\objetoVerde\inactivo", "*.png"))
            {
                objetoDeToqueVerdeInactivo.Add(new BitmapImage(new Uri(file)));

            }

            //morado

            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\objetos_renders\objetoMorado\activo", "*.png"))
            {
                objetoDeToqueMoradoActivo.Add(new BitmapImage(new Uri(file)));

            }

            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\objetos_renders\objetoMorado\inactivo", "*.png"))
            {
                objetoDeToqueMoradoInactivo.Add(new BitmapImage(new Uri(file)));

            }

            //Fondo Animado
            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\fondo_render", "*.png"))
            {
                framesFondoAnimado.Add(new BitmapImage(new Uri(file)));
            }

            foreach (string file in Directory.EnumerateFiles(pathEfectos + @"\intro_render", "*.png"))
            {

                imagenes_intro.Add(new BitmapImage(new Uri(file)));

            }

        }


        //Entrega cada cuadro de la animaciones
        public BitmapImage DibujarBitMap()
        {

            if (indexAnimacion == sprites.Count)
            {
                index = 0;
            }
            else
            {
                indexAnimacion++;
            }
            if (indexAnimacion >= sprites.Count)
            {
                indexAnimacion = 0;

            }
            return sprites.ElementAt(indexAnimacion);
        }

        //Dibuja los Warps y Glass
        public BitmapImage DibujarWarp(out bool fin)
        {
            fin = false;
            if (indexWarp == warp.Count)
            {
                indexWarp = 0;

            }
            else
            {
                indexWarp++;
            }
            if (indexWarp >= warp.Count)
            {
                indexWarp = 0;
                fin = true;
            }

            return warp.ElementAt(indexWarp);

        }
        public BitmapImage DibujarWarp2(out bool fin)
        {
            fin = false;
            if (indexWarp2 == warp2.Count)
            {
                indexWarp2 = 0;

            }
            else
            {
                indexWarp2++;
            }
            if (indexWarp2 >= warp2.Count)
            {
                indexWarp2 = 0;
                fin = true;
            }

            return warp2.ElementAt(indexWarp2);

        }
        public BitmapImage DibujarGlass(out bool fin)
        {
            fin = false;
            if (indexGlass == glass.Count)
            {
                indexGlass = 0;

            }
            else
            {
                indexGlass++;
            }
            if (indexGlass >= glass.Count)
            {
                indexGlass = 0;
                fin = true;
            }

            return glass.ElementAt(indexGlass);

        }


        //Dibuja Particulas
        public BitmapImage DibujarParticulaVerde(out bool fin)
        {
            fin = false;
            if (indexParticulasVerde == particulaVerde.Count)
            {
                indexParticulasVerde = 0;

            }
            else
            {
                indexParticulasVerde++;
            }
            if (indexParticulasVerde >= particulaVerde.Count)
            {
                indexParticulasVerde = 0;
                fin = true;
            }

            return particulaVerde.ElementAt(indexParticulasVerde);

        }
        public BitmapImage DibujarParticulaAzul(out bool fin)
        {
            fin = false;
            if (indexParticulasAzul == particulaAzul.Count)
            {
                indexParticulasAzul = 0;

            }
            else
            {
                indexParticulasAzul++;
            }
            if (indexParticulasAzul >= particulaAzul.Count)
            {
                indexParticulasAzul = 0;
                fin = true;
            }

            return particulaAzul.ElementAt(indexParticulasAzul);

        }
        public BitmapImage DibujarParticulaCeleste(out bool fin)
        {
            fin = false;
            if (indexParticulasCeleste == particulaCeleste.Count)
            {
                indexParticulasCeleste = 0;

            }
            else
            {
                indexParticulasCeleste++;
            }
            if (indexParticulasCeleste >= particulaCeleste.Count)
            {
                indexParticulasCeleste = 0;
                fin = true;
            }

            return particulaCeleste.ElementAt(indexParticulasCeleste);

        }
        public BitmapImage DibujarParticulaNaranja(out bool fin)
        {
            fin = false;
            if (indexParticulasNaranja == particulaNaranja.Count)
            {
                indexParticulasNaranja = 0;

            }
            else
            {
                indexParticulasNaranja++;
            }
            if (indexParticulasNaranja >= particulaNaranja.Count)
            {
                indexParticulasNaranja = 0;
                fin = true;
            }

            return particulaNaranja.ElementAt(indexParticulasNaranja);

        }



        //Entrega los objetos interactuables
        public static BitmapImage dibujarPelotasActivas(int indexAnimacion)
        {
            switch (indexAnimacion)
            {
                case 0:
                    return dibujar_objetoDeToqueVerdeActivo();

                case 1:
                    return dibujar_objetoDeToqueAzulActivo();
                case 2:
                case 3:
                case 4:
                case 5:
                    return dibujar_objetoDeToqueMoradoActivo();
                case 6:
                    return dibujar_objetoDeToqueAzulActivo();
                default:
                    return dibujar_objetoDeToqueVerdeActivo();
            }
        }

        //Identifica y Entrega los objetos cuando no son interactuables
        public static BitmapImage dibujarPelotasInactivas(int indexAnimacion)
        {
            switch (indexAnimacion)
            {
                case 0:
                    return dibujar_objetoDeToqueVerdeInactivo();

                case 1:
                    return dibujar_objetoDeToqueAzulInactivo();

                case 2:

                case 3:

                case 4:

                case 5:
                    return dibujar_objetoDeToqueMoradoInactivo();
                case 6:
                    return dibujar_objetoDeToqueAzulInactivo();

                default:
                    return dibujar_objetoDeToqueVerdeInactivo();
            }
        }
        //verdes
        public static BitmapImage dibujar_objetoDeToqueVerdeActivo()
        {
            if (index_objeto_verde == objetoDeToqueVerdeActivo.Count)
            {
                index_objeto_verde = 0;

            }
            else
            {
                index_objeto_verde++;
            }
            if (index_objeto_verde >= objetoDeToqueVerdeActivo.Count)
            {
                index_objeto_verde = 0;

            }

            return objetoDeToqueVerdeActivo.ElementAt(index_objeto_verde);

        }
        public static BitmapImage dibujar_objetoDeToqueVerdeInactivo()
        {

            if (index_objeto_verde == objetoDeToqueVerdeInactivo.Count)
            {
                index_objeto_verde = 0;

            }
            else
            {
                index_objeto_verde++;
            }
            if (index_objeto_verde >= objetoDeToqueVerdeInactivo.Count)
            {
                index_objeto_verde = 0;

            }

            return objetoDeToqueVerdeInactivo.ElementAt(index_objeto_verde);

        }
        //azul
        public static BitmapImage dibujar_objetoDeToqueAzulActivo()
        {

            if (index_objeto_azul == objetoDeToqueAzulActivo.Count)
            {
                index_objeto_azul = 0;

            }
            else
            {
                index_objeto_azul++;
            }
            if (index_objeto_azul >= objetoDeToqueAzulActivo.Count)
            {
                index_objeto_azul = 0;

            }

            return objetoDeToqueAzulActivo.ElementAt(index_objeto_azul);

        }
        public static BitmapImage dibujar_objetoDeToqueAzulInactivo()
        {

            if (index_objeto_azul == objetoDeToqueAzulInactivo.Count)
            {
                index_objeto_azul = 0;

            }
            else
            {
                index_objeto_azul++;
            }
            if (index_objeto_azul >= objetoDeToqueAzulInactivo.Count)
            {
                index_objeto_azul = 0;

            }

            return objetoDeToqueAzulInactivo.ElementAt(index_objeto_azul);

        }
        //morado
        public static BitmapImage dibujar_objetoDeToqueMoradoActivo()
        {

            if (index_objeto_morado == objetoDeToqueMoradoActivo.Count)
            {
                index_objeto_morado = 0;

            }
            else
            {
                index_objeto_morado++;
            }
            if (index_objeto_morado >= objetoDeToqueMoradoActivo.Count)
            {
                index_objeto_morado = 0;

            }

            return objetoDeToqueMoradoActivo.ElementAt(index_objeto_morado);

        }
        public static BitmapImage dibujar_objetoDeToqueMoradoInactivo()
        {

            if (index_objeto_morado == objetoDeToqueMoradoInactivo.Count)
            {
                index_objeto_morado = 0;

            }
            else
            {
                index_objeto_morado++;
            }
            if (index_objeto_morado >= objetoDeToqueMoradoInactivo.Count)
            {
                index_objeto_morado = 0;

            }

            return objetoDeToqueMoradoInactivo.ElementAt(index_objeto_morado);

        }

        //Entrega los cuadros de la animacion del fondo BackGround
        public static BitmapImage dibujar_fondoAnimado()
        {
            if (index_fondoAnimado == framesFondoAnimado.Count)
            {
                index_fondoAnimado = 0;

            }
            else
            {
                index_fondoAnimado++;
            }
            if (index_fondoAnimado >= framesFondoAnimado.Count)
            {
                index_fondoAnimado = 0;

            }

            return framesFondoAnimado.ElementAt(index_fondoAnimado);
        }

        //Entrega los cuadros introductorios
        public BitmapImage Dibujar_Intro(bool intro)
        {
            if (!intro)
            {
                return imagenes_intro.ElementAt(0);
            }
            else if (indexIntro < imagenes_intro.Count - 1)
            {
                indexIntro++;
            }
            else
            {
                return imagenes_intro.ElementAt(35);

            }

            return imagenes_intro.ElementAt(indexIntro);
        }
    }
}
