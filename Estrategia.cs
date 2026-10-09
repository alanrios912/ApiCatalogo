
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using tp1;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tpfinal
{
    public class Estrategia
    {
        public void Agregar(ArbolGeneral<ItemCat> arbol, ItemCat dato, string rutaAlPadre)
        {
            if (arbol == null || dato == null) return;

            // Si la ruta está vacía o coincide con la raíz, agregamos directamente a la raíz
            if (string.IsNullOrWhiteSpace(rutaAlPadre) || 
                (arbol.getDatoRaiz() != null && arbol.getDatoRaiz().Nombre.Equals(rutaAlPadre, StringComparison.OrdinalIgnoreCase)))
            {
                arbol.agregarHijo(new ArbolGeneral<ItemCat>(dato));
                return;
            }

            string[] categorias = rutaAlPadre.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            ArbolGeneral<ItemCat> nodoActual = arbol;

            foreach (string cat in categorias)
            {
                // Si estamos en la raíz y la primera categoría coincide con la raíz, la salteamos
                if (nodoActual == arbol && nodoActual.getDatoRaiz() != null && 
                    nodoActual.getDatoRaiz().Nombre.Equals(cat, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                ArbolGeneral<ItemCat> subNodo = null;
                foreach (var hijo in nodoActual.getHijos())
                {
                    if (hijo.getDatoRaiz() != null && 
                        hijo.getDatoRaiz().Nombre.Equals(cat, StringComparison.OrdinalIgnoreCase))
                    {
                        subNodo = hijo;
                        break;
                    }
                }

                if (subNodo == null)
                {
                    subNodo = new ArbolGeneral<ItemCat>(new ItemCat { Nombre = cat });
                    nodoActual.agregarHijo(subNodo);
                }

                nodoActual = subNodo;
            }

            nodoActual.agregarHijo(new ArbolGeneral<ItemCat>(dato));
        }

        public List<ItemCat> Buscar(ArbolGeneral<ItemCat> arbol, string elementoABuscar)
        {
            List<ItemCat> resultado = new List<ItemCat>();
            BuscarRec(arbol, elementoABuscar ?? "", resultado);
            return resultado;
        }

        private void BuscarRec(ArbolGeneral<ItemCat> nodo, string texto, List<ItemCat> resultado)
        {
            if (nodo == null) return;

            if (nodo.getDatoRaiz() != null && 
                nodo.getDatoRaiz().Nombre != null && 
                nodo.getDatoRaiz().Nombre.ToLower().Contains(texto.ToLower()))
            {
                resultado.Add(nodo.getDatoRaiz());
            }

            foreach (var hijo in nodo.getHijos())
            {
                BuscarRec(hijo, texto, resultado);
            }
        }

        public List<ItemCat> Todos(ArbolGeneral<ItemCat> arbol)
        {
            List<ItemCat> productos = new List<ItemCat>();
            TodosRec(arbol, productos);
            return productos;
        }

        private void TodosRec(ArbolGeneral<ItemCat> nodo, List<ItemCat> productos)
        {
            if (nodo == null) return;

            // Agregamos TODOS los nodos existentes (sean hojas o nodos internos)
            if (nodo.getDatoRaiz() != null)
            {
                productos.Add(nodo.getDatoRaiz());
            }

            foreach (var hijo in nodo.getHijos())
            {
                TodosRec(hijo, productos);
            }
        }

        public List<string> GetURLsSEO(ArbolGeneral<ItemCat> arbol)
        {
            List<string> urls = new List<string>();
            GetURLsSEORec(arbol, "", urls);
            return urls;
        }

        private void GetURLsSEORec(ArbolGeneral<ItemCat> nodo, string ruta, List<string> urls)
        {
            if (nodo == null || nodo.getDatoRaiz() == null) return;

            string nombreNodo = nodo.getDatoRaiz().Nombre != null ? nodo.getDatoRaiz().Nombre.ToLower() : "";
            string nuevaRuta = string.IsNullOrEmpty(ruta) 
                ? nombreNodo 
                : $"{ruta}/{nombreNodo}";

            if (nodo.esHoja())
            {
                urls.Add(nuevaRuta);
            }
            else
            {
                foreach (var hijo in nodo.getHijos())
                {
                    GetURLsSEORec(hijo, nuevaRuta, urls);
                }
            }
        }

        public string GetUrlSeoPorId(ArbolGeneral<ItemCat> arbol, int id)
        {
            return GetUrlSeoPorIdRec(arbol, id, "");
        }

        private string GetUrlSeoPorIdRec(ArbolGeneral<ItemCat> nodo, int id, string ruta)
        {
            if (nodo == null || nodo.getDatoRaiz() == null) return null;

            string nombreNodo = nodo.getDatoRaiz().Nombre != null ? nodo.getDatoRaiz().Nombre.ToLower() : "";
            string nuevaRuta = string.IsNullOrEmpty(ruta) 
                ? nombreNodo 
                : $"{ruta}/{nombreNodo}";

            if (nodo.getDatoRaiz().Id == id)
            {
                return nuevaRuta;
            }

            foreach (var hijo in nodo.getHijos())
            {
                string res = GetUrlSeoPorIdRec(hijo, id, nuevaRuta);
                if (res != null) return res;
            }

            return null;
        }

        public List<List<string>> ConsultaNiveles(ArbolGeneral<ItemCat> arbol)
        {
            List<List<string>> resultado = new List<List<string>>();
            if (arbol == null) return resultado;

            Cola<ArbolGeneral<ItemCat>> cola = new Cola<ArbolGeneral<ItemCat>>();
            cola.encolar(arbol);

            while (!cola.esVacia())
            {
                List<string> nivelActual = new List<string>();
                Cola<ArbolGeneral<ItemCat>> aux = new Cola<ArbolGeneral<ItemCat>>();

                while (!cola.esVacia())
                {
                    ArbolGeneral<ItemCat> nodo = cola.desencolar();
                    if (nodo != null && nodo.getDatoRaiz() != null)
                    {
                        nivelActual.Add(nodo.getDatoRaiz().Nombre);
                    }

                    if (nodo != null && nodo.getHijos() != null)
                    {
                        foreach (var hijo in nodo.getHijos())
                        {
                            aux.encolar(hijo);
                        }
                    }
                }

                resultado.Add(nivelActual);
                cola = aux;
            }

            return resultado;
        }
    }
}