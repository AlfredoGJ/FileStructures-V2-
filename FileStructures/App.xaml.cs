using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FileStructures
{
    /// <summary>
    /// Proporciona un comportamiento específico de la aplicación para complementar la clase Application predeterminada.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Inicializa el objeto de aplicación Singleton. Esta es la primera línea de código creado
        /// ejecutado y, como tal, es el equivalente lógico de main() o WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
          
        }

        //public static StorageFolder projectsFolder { get; set; } 
        public static string CurrentFileName { get; set; }
        public static FileOrganization CurrentFileOrganization = FileOrganization.Indexed;
        public static int PrimaryKeyIndexNumber = 5;
        public static int SecondaryKeyIndexNumber=5;

        public static List<Char> Alphabet = new List<char> {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'}; 
        /// <summary>
        /// Se invoca cuando el usuario final inicia la aplicación normalmente. Se usarán otros puntos
        /// de entrada cuando la aplicación se inicie para abrir un archivo específico, por ejemplo.
        /// </summary>
        /// <param name="e">Información detallada acerca de la solicitud y el proceso de inicio.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // No repetir la inicialización de la aplicación si la ventana tiene contenido todavía,
            // solo asegurarse de que la ventana está activa.
            if (rootFrame == null)
            {
                // Crear un marco para que actúe como contexto de navegación y navegar a la primera página.
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Cargar el estado de la aplicación suspendida previamente
                }

                // Poner el marco en la ventana actual.
                Window.Current.Content = rootFrame;



            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Cuando no se restaura la pila de navegación, navegar a la primera página,
                    // configurando la nueva página pasándole la información requerida como
                    //parámetro de navegación
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Asegurarse de que la ventana actual está activa.
                Window.Current.Activate();
            }

//            string ss = "Jose alfredo Granja Jalomo, 812123, Juan alvarez # 136\n" +
// "Jose de jesus Zaval rico, 882877, av de los andes #345\n" +
// "everardo de jesus perez gonzalez, 32349909, av carranza #435\n" +
// "juliana sanchez rojas, 666736, rivas guillen # 323\n" +
//"pedro paramo de la O,278722, pedro rojas don juan #323, colonia las aves\n";

            //byte[] chars = new byte[stream.Length];
            //stream.Read(chars,0,(int)stream.Length);
            //var tochar= chars.Cast<char>();
            //string s = new string(tochar.ToArray());
            //var rows = CutStringToRegisters(ss);
            //int j = 23;
        }

        /// <summary>
        /// Se invoca cuando la aplicación la inicia normalmente el usuario final. Se usarán otros puntos
        /// </summary>
        /// <param name="sender">Marco que produjo el error de navegación</param>
        /// <param name="e">Detalles sobre el error de navegación</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Se invoca al suspender la ejecución de la aplicación. El estado de la aplicación se guarda
        /// sin saber si la aplicación se terminará o se reanudará con el contenido
        /// de la memoria aún intacto.
        /// </summary>
        /// <param name="sender">Origen de la solicitud de suspensión.</param>
        /// <param name="e">Detalles sobre la solicitud de suspensión.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Guardar el estado de la aplicación y detener toda actividad en segundo plano
            deferral.Complete();
        }


        public static List<List<string>> CutStringToRegisters( string  data)
        {
            List< List<string>> registers = new List<List<string>>();

            char[] newLine = new char[1];
            char[] comma = new char[1];
            newLine[0] = '\n';
            comma[0] = ',';

            var rows = data.Split(newLine);
            foreach (string row in rows)
            {
                if(!string.IsNullOrWhiteSpace(row))
                    registers.Add(row.Split(comma).ToList());
            }
               

            return registers;

        }

        public static object CloneObjPrimitive(object item, char type)
        {
            object o = new object();
            switch (type)
            {
                case 'I':
                    o = (int)item;
                    break;

                case 'S':
                    o = (string)item;
                    break;
            }
            return o;

        }

        public static int GetIntFirstDigit(int number)
        {
            var asString = number.ToString();
            int result=int.Parse(asString[0].ToString());
            return result;

        }

        // A>B --> -1 
        // A<B --> 1
        // A=B --> 0
        public static int CompareObjects(object objectA, object objectB)
        {
            int result = -1;
            string a = objectA.GetType().Name;
            string b = objectB.GetType().Name;
            //if (objectA.GetType().Name != objectB.GetType().Name)
            // {
            if (objectA.GetType() == typeof(int))
            {
                if ((int)objectA < (int)objectB)
                    result = 1;
                else if ((int)objectA == (int)objectB)
                    result = 0;
            }
            else if (objectA.GetType() == typeof(string))
            {
                result = string.Compare(objectA as string, objectB as string, StringComparison.CurrentCulture);
            }
            //}
            // else
            //   throw new Exception("arguments are not instances of the same type");

            return result;
        }

        public static int TDrawElemSize = 46;


    }

  
    }
