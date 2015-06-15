using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using System.Windows.Media;
using Juego;
using dibujo = System.Drawing;
using Rect = System.Windows.Rect;
using System.Windows;
using System.Collections;
using WMPLib;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;


namespace Juego
{
    public class Objetos
    {
        //Declaracion de variables
        #region Variables
        public delegate void d_Colision(string ID_objeto);
        public event d_Colision event_Colision;
        public event d_Colision event_NoColision;
        public Funciones clsFunciones;
        public Dictionary<string, Objeto> dObjetos = new Dictionary<string, Objeto>();
        public Dictionary<string, Objeto_Dibujado> ObjetosDibujados = new Dictionary<string, Objeto_Dibujado>();
        public Dictionary<JointType, Objeto_Cuerpo> ObjetosCuerpo = new Dictionary<JointType, Objeto_Cuerpo>();
        #endregion

        //Declaracion de las estructuras
        public struct Objeto
        {
            public string id_Objeto;
            public Rect rect_ini;
            public Rect rect_fin;
            public bool isEvent;
            public bool isInteractuable;
            public string PathAudio;
            public double life_time;
            public int seg_aparicion;
            public double tiempo_interaccion;
            public int animacion;
            public int seg_final;
        }
        public struct Objeto_Dibujado
        {
            public string id_Objeto;
            public Rect rectangulo;
            public WindowsMediaPlayer player;
        }
        public struct Objeto_Cuerpo
        {
            public JointType tipo;
            public bool isColision;
            public string id_objeto;
        }


        //Constructor
        public Objetos()
        {

        }



        //Dibuja los rectangulos que no tienen movimiento
        public void dibujarRect(System.Windows.Media.DrawingContext pantalla, string objeto_id)
        {

            Objeto objeto = dObjetos[objeto_id];
            Brush centerPointBrush = Brushes.Red;
            Objeto_Dibujado objeto_dibujado;

            if (ObjetosDibujados.TryGetValue(objeto_id, out objeto_dibujado))
            {
                //existe en objetos_dibujados
                //pantalla.DrawRectangle(centerPointBrush, null, objeto_dibujado.rectangulo);
                set_Objetos_Dibujados(objeto_id, objeto_dibujado.rectangulo);
            }
            else
            {
                //objeto.rect_ini.Size = new System.Windows.Size(0, 0);
                // pantalla.DrawRectangle(centerPointBrush, null, objeto.rect_ini);
                set_Objetos_Dibujados(objeto_id, objeto.rect_ini);
            }
        }


        //Dibuja los objetos que tienen movimiento
        public void dibujarRectMov(System.Windows.Media.DrawingContext pantalla, string objeto_id, float tiempoActual)
        {

            Objeto objeto = dObjetos[objeto_id];
            BitmapImage imagen;
            Objeto_Dibujado objeto_dibujado;

            if (tiempoActual >= objeto.seg_aparicion + (objeto.life_time - objeto.tiempo_interaccion))
            {
                //Cambia el color cuando es interactuable
                imagen = Animaciones.dibujarPelotasActivas(objeto.animacion);
            }
            else
            {
                imagen = Animaciones.dibujarPelotasInactivas(objeto.animacion);
            }

            if (ObjetosDibujados.TryGetValue(objeto_id, out objeto_dibujado))
            {
                //existe en objetos_dibujados
                pantalla.DrawImage(imagen, objeto_dibujado.rectangulo);

                set_Objetos_Dibujados(objeto_id, objeto_dibujado.rectangulo);
            }
            else
            {
                objeto.rect_ini.Size = new System.Windows.Size(0, 0);
                set_Objetos_Dibujados(objeto_id, objeto.rect_ini);
            }
        }

        //Elimina los objetos ya dibujados
        public void EliminarObjeto_Dibujado(string objeto_id)
        {
            ObjetosDibujados.Remove(objeto_id);
        }

        //Elimina los objetos cargados en la estructura objetos
        public void EliminarObjeto(string objeto_id)
        {
            dObjetos.Remove(objeto_id);
        }

        //Ingresa los objetos de la estructura objetos a la estructura objetos dibujados
        private void set_Objetos_Dibujados(string NombreTextura, Rect rectangulo)
        {
            Objeto objeto = dObjetos[NombreTextura];
            Objeto_Dibujado objeto_dibujado;


            if (ObjetosDibujados.TryGetValue(NombreTextura, out objeto_dibujado))
            {
                // si es que esta dibujado
                objeto_dibujado.rectangulo = rectangulo;
                ObjetosDibujados[NombreTextura] = objeto_dibujado;
            }
            else
            {
                //si no 
                objeto_dibujado = new Objeto_Dibujado();
                objeto_dibujado.id_Objeto = NombreTextura;
                objeto_dibujado.rectangulo = rectangulo;
                ObjetosDibujados.Add(NombreTextura, objeto_dibujado);
            }
        }

        //Metodo que identifica si los objetos estan en colision con el cuerpo
        public void Colision(float tiempoActual)
        {
            Rect RecCol;
            Point eskeleto;
            Dictionary<string, Objeto_Dibujado> ObjetosDibujados2 = ObjetosDibujados;

            //El foreach recorre todos los objetos de la estructura ObjetosCuerpo la cual contiene todos los puntos interactuables.
            foreach (JointType tipo in ObjetosCuerpo.Keys)
            {
                try
                {
                    //Recorre los objetos dibujados
                    foreach (string item in ObjetosDibujados2.Keys)
                    {
                        eskeleto = clsFunciones.SkeletonPointToScreenPoint(clsFunciones.skeleton, tipo);
                        RecCol = new Rect((eskeleto.X - 10), (eskeleto.Y - 10), 20, 20);

                        Objeto_Cuerpo objeto_cuerpo = ObjetosCuerpo[tipo];
                        //Verifica si el punto dibujado esta en colision con el objeto cuerpo.
                        if (ObjetosDibujados2[item].rectangulo.Contains(RecCol) || ObjetosDibujados2[item].rectangulo.IntersectsWith(RecCol))
                        {
                            //Si el objeto dibujado es interactuable, entonces:
                            if (dObjetos[item].isInteractuable)
                            {
                                //Si el tiempo actual es igual al tiempo de vida del objeto, entonces:
                                if (tiempoActual == dObjetos[item].seg_final - dObjetos[item].tiempo_interaccion)
                                {
                                    //El objeto genera evento
                                    Objeto tempObjeto = dObjetos[item];
                                    tempObjeto.isEvent = true;
                                    dObjetos[item] = tempObjeto;
                                }
                            }

                            //EL TOQUE DE EL OBJETO SOLO SE PERMITE EN los 2 ultimos segundos de vida 
                            //Si el objeto genera evento:
                            if (dObjetos[item].isEvent == true)
                            {
                                //Si el objeto cuerpo todavia no esta en colision, entonces: 
                                if (objeto_cuerpo.isColision == false)
                                {
                                    //Indicamos a la estructura que el objeto ya se encuentra en colision y posteriormente generamos un evento publico.
                                    objeto_cuerpo.isColision = true;
                                    objeto_cuerpo.id_objeto = item;
                                    ObjetosCuerpo[tipo] = objeto_cuerpo;
                                    event_Colision(item);
                                    return;
                                }
                            }
                        }
                            //Si el objeto no se encuentra en colision, entonces:
                        else
                        {
                            //Si el objeto cuerpo ya hizo colision, entonces:
                            if (objeto_cuerpo.isColision == true)
                            {
                                //restablece los valores.
                                objeto_cuerpo.isColision = false;
                                objeto_cuerpo.id_objeto = "";
                                ObjetosCuerpo[tipo] = objeto_cuerpo;
                                return;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }


        }

        //Detecta los pasos iniciales que dan inicio al juego
        public bool Detecta_Pasos(string item1, string item2)
        {
            Rect RecCol1;
            Rect RecCol2;
            Point eskeleto1;
            Point eskeleto2;
            Dictionary<string, Objeto_Dibujado> ObjetosDibujados2 = ObjetosDibujados;

            //foreach (JointType tipo in Enum.GetValues(typeof(JointType)))

            try
            {
                eskeleto1 = clsFunciones.SkeletonPointToScreenPoint(clsFunciones.skeleton, JointType.HandRight);
                eskeleto2 = clsFunciones.SkeletonPointToScreenPoint(clsFunciones.skeleton, JointType.HandLeft);
                RecCol1 = new Rect((eskeleto1.X), (eskeleto1.Y), 20, 20);
                RecCol2 = new Rect((eskeleto2.X), (eskeleto2.Y), 20, 20);

                // antes usaba Intersects 
                if (ObjetosDibujados.ContainsKey(item1) && ObjetosDibujados.ContainsKey(item2))
                {
                    //if (dObjetos[item2].rectangulo.IntersectsWith(RecCol1) && dObjetos[item1].rectangulo.IntersectsWith(RecCol2))
                    if (ObjetosDibujados[item2].rectangulo.IntersectsWith(RecCol1) && ObjetosDibujados[item1].rectangulo.IntersectsWith(RecCol2))
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        //Metodo matematico que entrega la posicion del objeto en razon al tiempo
        public System.Drawing.Point return_PosFinal_Objeto(double TiempoDeVida, string id_objeto)
        {
            int R = 640 / 160;
            Objeto objeto = new Objeto();
            objeto = dObjetos[id_objeto];

            //P = mitad de pantalla
            System.Drawing.Point P = new dibujo.Point(320, 240);

            //A = Posicion inicial del objeto
            System.Drawing.Point A = new dibujo.Point((int)objeto.rect_ini.X, (int)objeto.rect_ini.Y);

            //B = posicion final
            System.Drawing.Point B;
            System.Drawing.Point pTemp1, pTemp2;
            System.Drawing.Point pF;

            // Razon x posicion inicial X , Razon x posicion inicial Y
            pTemp1 = new System.Drawing.Point(R * A.X, R * A.Y);

            //
            pTemp2 = new System.Drawing.Point((1 - R) * P.X, (1 - R) * P.Y);

            //B = posicion final
            B = new System.Drawing.Point(pTemp1.X + pTemp2.X, pTemp1.Y + pTemp2.Y);


            pTemp1 = new System.Drawing.Point((int)(TiempoDeVida * B.X), (int)(TiempoDeVida * B.Y));
            pTemp2 = new System.Drawing.Point((int)((1 - TiempoDeVida) * A.X), (int)((1 - TiempoDeVida) * A.Y));

            pF = new System.Drawing.Point(pTemp1.X + pTemp2.X, pTemp1.Y + pTemp2.Y);

            return pF;
        }

        //Metodo matematico que entrega la posicion  final del objeto
        public System.Drawing.Point return_PosFinal_Objeto(System.Drawing.Point PuntoA)
        {
            int R = 640 / 160;

            //P = mitad de pantalla
            System.Drawing.Point P = new dibujo.Point(320, 240);

            //A = Posicion inicial del objeto
            System.Drawing.Point A = new dibujo.Point((int)PuntoA.X, (int)PuntoA.Y);

            //B = posicion final
            System.Drawing.Point B;
            System.Drawing.Point pTemp1, pTemp2;
            System.Drawing.Point pF;

            // Razon x posicion inicial X , Razon x posicion inicial Y
            pTemp1 = new System.Drawing.Point(R * A.X, R * A.Y);

            //
            pTemp2 = new System.Drawing.Point((1 - R) * P.X, (1 - R) * P.Y);

            //B = posicion final
            B = new System.Drawing.Point(pTemp1.X + pTemp2.X, pTemp1.Y + pTemp2.Y);


            pTemp1 = new System.Drawing.Point((int)(1 * B.X), (int)(1 * B.Y));
            pTemp2 = new System.Drawing.Point((int)((1 - 1) * A.X), (int)((1 - 1) * A.Y));

            pF = new System.Drawing.Point(pTemp1.X + pTemp2.X, pTemp1.Y + pTemp2.Y);

            return pF;
        }

        //Metodo que actualiza los puntos y aumenta el tamaño dentro de la estructura objetosDibujados en razon de su posicion final
        public void mueve_punto(string id_objeto)
        {

            Objeto objeto = dObjetos[id_objeto];
            Objeto_Dibujado objeto_dibujado;
            objeto_dibujado = ObjetosDibujados[id_objeto];
            System.Windows.Point dist;
            System.Windows.Point Mov;
            System.Windows.Size nuevoTamaño;

            dist = new System.Windows.Point((objeto.rect_fin.X - objeto.rect_ini.X), (objeto.rect_fin.Y - objeto.rect_ini.Y));
            Mov = new System.Windows.Point(dist.X / (objeto.life_time * 30), dist.Y / (objeto.life_time * 30));

            objeto_dibujado.rectangulo.X += Mov.X;
            objeto_dibujado.rectangulo.Y += Mov.Y;

            nuevoTamaño = objeto_dibujado.rectangulo.Size;
            nuevoTamaño = new System.Windows.Size(nuevoTamaño.Width + (objeto.rect_ini.Width / (objeto.life_time * 30)), nuevoTamaño.Height + (objeto.rect_ini.Height / (objeto.life_time * 30)));
            objeto_dibujado.rectangulo.Size = nuevoTamaño;

            ObjetosDibujados[id_objeto] = objeto_dibujado;

        }

    }
}
