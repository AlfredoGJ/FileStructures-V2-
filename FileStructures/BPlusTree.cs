using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using FileStructures.Controls;
namespace FileStructures
{
    /// <summary>
    /// Enumeracion para clasificar los tipos de nodos en una arbol
    /// </summary>
    public enum NodeType { Header, Intermediate, Leaf };


    /// <summary>
    /// Clase que representa un arbol B+
    /// </summary>
    public class Tree
    {
        Node header;
        /// <summary>
        /// Lista de nodos del arbol
        /// </summary>
        public List<Node> Nodes { get => nodes; }

        private List<Node> nodes;
        /// <summary>
        /// Orden del arbol
        /// </summary>
        public int order;

        public Tree(int order)
        {
            this.order = order;
            nodes = new List<Node>();
            header = new Node(this, NodeType.Leaf);
            nodes.Add(header);

        }


        /// <summary>
        /// Metodo que maneja la insercion en el arbol
        /// </summary>
        /// <param name="key">Clave a insertar</param>
        /// <param name="address">Direccion en el archivo de datos del registro de la clave</param>
        public void Insert(object key, long address)
        {
            Child result=null;
            if(key.GetType()== typeof(int))
                result = header.Insert(new Child((int)key, address), true);

            if (key.GetType() == typeof(string))
                result = header.Insert(new Child((string)key, address), true);


            if (result != null)
            {
                Node n = new Node(this, NodeType.Header);
                n.Children.Add(result);


                header = n; ;
            }

        }

        /// <summary>
        /// Dibuja el arbol sobre un contro canvas
        /// </summary>
        /// <param name="canvas">Area donde se dibujara el arbol</param>
        public void Draw(Canvas canvas)
        {

            int treeWidth = nodes.FindAll(x => x.Type == NodeType.Leaf).Count() * 2 * order * App.TDrawElemSize;
            int canvasHeight =0;


            canvas.Width = treeWidth;
            canvas.Children.Clear();

            header.Print(canvas, 20, 20, treeWidth);
        }


    }





    /// <summary>
    /// Clase que representa un nodo del arbol B+
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Tipo del nodo
        /// </summary>
        public NodeType Type { get; set; }
        /// <summary>
        /// Lista de elementos que estan dentro del nodo
        /// </summary>
        public List<Child> Children { get; set; }
        /// <summary>
        /// Orden del nodo
        /// </summary>
        public int Order { get => order; }

        private int order;
        private Tree tree;

        public Node(Tree tree)
        {
            this.order = order;
            this.tree = tree;
            Type = NodeType.Header;
            Children = new List<Child>();
        }

        public Node(Tree tree, NodeType type)
        {
            this.order = tree.order;
            this.Type = type;
            this.tree = tree;
            Type = type;
            Children = new List<Child>();
        }

        /// <summary>
        /// Metodo que maneja la insercion en el nodo
        /// </summary>
        /// <param name="element">Elemento a insertar en el nodo</param>
        /// <param name="descending">Variable que indica en que sentido del recorrido va la insercion (hacia abajo o hacia arriba)</param>
        /// <returns>Regresa un elemento si es que se desborda el nodo</returns>
        public Child Insert(Child element, bool descending)
        {
            Child returnChild = null;
            // If we are going down the tree
            if (descending)
            {
                if (Type == NodeType.Leaf)
                {
                    if (PutChildren(element))
                    {
                        // if the node overflow
                        if (Children.Count >= 2 * order + 1)
                        {

                            returnChild = Split();
                        }
                    }
                    else
                        throw new Exception("The value already exist in the Binary tree");
                }
                if (Type == NodeType.Intermediate)
                {

                }
                if (Type == NodeType.Header || Type == NodeType.Intermediate)
                {
                    char insertionSide = 'X';
                    Node insertionPlace = null;
                    Child insertionChild = null;
                    // Se identifica en que nodo descendiente se va a insertar
                    foreach (Child c in Children)
                    {
                        int cmpResult = App.CompareObjects(element.value.Item1, c.value.Item1);
                        if (cmpResult == 0)
                            throw new Exception("The value already exist in the Binary tree");
                        else if (cmpResult == 1)
                        {

                            insertionPlace = c.LeftDescendant;
                            insertionChild = c;
                            insertionSide = 'L';
                            break;
                        }
                        else if (Children.IndexOf(c) == Children.Count() - 1)
                        {
                            insertionPlace = c.RightDescendant;
                            insertionChild = c;
                            insertionSide = 'R';
                        }
                    }

                    // Se realiza la insercion y se
                    Child insrtResult = insertionPlace.Insert(element, true);
                    if (insrtResult != null)
                    {
                        returnChild = Insert(insrtResult, false);

                        if (insertionSide == 'L' && insrtResult.Previous != null)
                            insrtResult.Previous.RightDescendant = insrtResult.LeftDescendant;


                        if (insertionSide == 'R')
                            insertionChild.RightDescendant = insrtResult.LeftDescendant;


                    }
                }


            }

            // If we are on the way up
            else
            {
                if (Type == NodeType.Header || Type == NodeType.Intermediate)
                {
                    if (PutChildren(element))
                    {
                        // if the node overflow
                        if (Children.Count >= 2 * order + 1)
                        {
                            returnChild = Split();
                        }
                    }
                    else
                        throw new Exception("The value already exist in the Binary tree");
                }
            }

            return returnChild;

        }


        /// <summary>
        /// Metodo que divide el nodo una vez que se alcanza el numero maximo de elementos
        /// </summary>
        /// <returns> Regresa el elemento central</returns>
        private Child Split()
        {

            NodeType newType = Type;

            if (Type == NodeType.Header)
                newType = NodeType.Intermediate;

            Type = newType;
            Node left = new Node(tree, newType);



            for (int i = 0; i < order; i++)
                left.Children.Add(Children[i]);

            tree.Nodes.Add(left);

            Child center = new Child(Children[order].value);


            center.RightDescendant = this;
            center.LeftDescendant = left;


            foreach (Child c in left.Children)
                Children.Remove(c);

            if (Type == NodeType.Header || Type == NodeType.Intermediate)
                Children.RemoveAt(0);

            return center;
        }




        /// <summary>
        /// Metodo que inserta un elemento en la lista de elementos del nodo
        /// </summary>
        /// <param name="child">Elemento que se inserta</param>
        /// <returns>Regresa true si se inserta correctamente el elemento, regresa false si el elemento esta repetido</returns>
        private bool PutChildren(Child child)
        {
            if (Children.Count == 0)
                Children.Add(child);
            else
            {
                int i;
                for (i = 0; i < Children.Count; i++)
                {
                    int compare = App.CompareObjects(child.value.Item1, Children[i].value.Item1);
                    if (compare == 1)
                        break;
                    if (compare == 0)
                        return false;
                }

                if (i == 0)
                {
                    child.Next = Children[i];
                    Children[i].Previous = child;
                    Children.Insert(0, child);
                }
                else if (i == Children.Count)
                {
                    child.Previous = Children.Last();
                    Children.Last().Next = child;
                    Children.Add(child);
                }
                else
                {
                    Children[i - 1].Next = child;
                    child.Previous = Children[i - 1];
                    child.Next = Children[i];
                    Children.Insert(i, child);
                }

            }
            return true;

        }

        internal void Print(Canvas canvas, int x, int y, int available_width)
        {
            
            float halfBlock=  ((float)Children.Count / 2) * App.TDrawElemSize;

            int xDrawPos =(int)( x + (available_width / 2) - halfBlock );
          

            TreeNode node = new TreeNode();
            foreach (Child c in Children)
            {
                TreeElement e = new TreeElement(c.value.Item1.ToString(), c.value.Item2.ToString());

                c.LeftDescendant?.Print(canvas, x + (available_width / (Children.Count() + 1)) * Children.IndexOf(c), y+120, available_width / (Children.Count() + 1));

                if (Children.IndexOf(c) == Children.Count - 1)
                    c.RightDescendant?.Print(canvas, x + available_width / (Children.Count() + 1) * Children.Count(), y+120, available_width / (Children.Count() + 1));
                node.Add(e);

            }

            canvas.Children.Add(node);
            node.SetValue(Canvas.TopProperty, y);
            node.SetValue(Canvas.LeftProperty, xDrawPos);




        }
    }



    /// <summary>
    /// Clase que representa el componente minimo de un arbol B+ la cual contiene una clave y una direccion , este elemento puede apuntar a los nodos del arbol
    /// </summary>
    public class Child
    {
        /// <summary>
        /// Elemento siguiente
        /// </summary>
        public Child Next { get; set; }
        /// <summary>
        /// Elemento anterior
        /// </summary>
        public Child Previous { get; set; }
        /// <summary>
        /// Nodo a la izquierda de este elemento
        /// </summary>
        public Node LeftDescendant { get; set; }
        /// <summary>
        /// Nodo a la derecha de este elemento
        /// </summary>
        public Node RightDescendant { get; set; }
        Node Node { get; set; }
        Tuple<object, long> Value { get => value; }
        /// <summary>
        /// Tupla que contiene el par clave - posicion
        /// </summary>
        public Tuple<object, long> value;

        public Child(Tuple<object, long> value)
        {
            this.value = value;
        }

        public Child(int key, long address)
        {
            value = new Tuple<object, long>(key, address);
        }

        public Child(string key, long address)
        {
            value = new Tuple<object, long>(key, address);
        }

    }
}

