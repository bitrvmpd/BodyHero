using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Juego
{
    public class Funciones
    {
        private KinectSensor sensor;
        public Skeleton skeleton;

        public Funciones(KinectSensor _sensor)
        {
            sensor = _sensor;

        }

        /// <summary>
        /// Convierte un punto skeleton a punto de pantalla, especificando la articulacion
        /// </summary>
        /// <param name="skeleton">arreglo skeleton</param>
        /// <param name="joint">tipo de articulacion</param>
        /// <returns>Punto con coordenada X,Y para usar en pantalla</returns>
        public Point SkeletonPointToScreenPoint(Skeleton skeleton, JointType joint)
        {
            DepthImagePoint puntoDePantalla = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[joint].Position, DepthImageFormat.Resolution640x480Fps30);
            return new Point(puntoDePantalla.X, puntoDePantalla.Y);
        }
        public Point SkeletonPointToScreenPoint(SkeletonPoint skelpoint)
        {
            DepthImagePoint puntoDePantalla = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(puntoDePantalla.X, puntoDePantalla.Y);
        }

        /// <summary>
        /// escribe en pantalla el texto que se requiera
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="texto"></param>
        /// <param name="tamano"></param>
        /// <param name="posicion"></param>
        /// <param name="color"></param>
        public void escribirPantalla(DrawingContext dc, string texto, int tamano, Point posicion, Brush color)
        {
            dc.DrawText(new FormattedText(texto, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), tamano, color), posicion);
        }


    }
}


