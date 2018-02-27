using Practica_TiendaVirtual.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Practica_TiendaVirtual.Controllers
{
    public class CarritoController : Controller
    {
        private TiendaVirtualEntities db = new TiendaVirtualEntities();

        [Authorize]
        public ActionResult anadirCarrito(int id, CarritoCompra cc)
        {
            Producto p = db.Productos.Find(id);

            List<Producto> totalProductos = cc.FindAll(x => x.Id == id);

            if (totalProductos.Count() + 1 <= p.Cantidad)
            {
                cc.Add(p);
                return RedirectToAction("Index", "Productos");
            }
            else
            {
                return RedirectToAction("carrito", "Carrito", p);
            }
        }

        [Authorize]
        public ActionResult carrito(CarritoCompra cc)
        {
            return View(cc);
        }

        [Authorize]
        public ActionResult eliminarProductoCarrito(int id, CarritoCompra cc)
        {
            Producto producto = cc.Find(x => x.Id == id);
            cc.Remove(producto);

            return RedirectToAction("carrito", "Carrito", cc);
        }

        [Authorize]
        public ActionResult pedidos()
        {
            var pedidos = from pedido in db.Pedidoes
                          where pedido.AspNetUser.UserName.Equals(User.Identity.Name)
                          select pedido;

            return RedirectToAction("Index", "Pedidos", pedidos);
        }

        [Authorize]
        public ActionResult confirmarPedido(CarritoCompra cc)
        {
            if (cc.Count() > 0)
            {
                Pedido pedido = new Pedido();
                string productos = "";
                string nombre_productos = "";
                int cantidad = 0;
                float precio = 0;

                foreach (Producto prod in cc)
                {
                    productos = productos + ";" + prod.Nombre_imagen;
                    nombre_productos = nombre_productos + ";" + prod.Nombre;
                    cantidad++;
                    precio = precio + (float)prod.Precio;

                    var producto = from pr in db.Productos
                                   where pr.Id == prod.Id
                                   select pr;

                    Producto p = producto.First();
                    p.Cantidad--;

                    ProductosPedido pp = new ProductosPedido();
                    pp.Producto = p;
                    pp.Pedido = pedido;
                    db.ProductosPedidoes.Add(pp);
                }

                var users = from user in db.AspNetUsers
                            where user.UserName.Equals(User.Identity.Name)
                            select user;

                pedido.Id_Usuario = users.First().Id;

                DateTime date = DateTime.Now;

                pedido.Fecha = date;
                pedido.TotalPrecio = Math.Round(precio, 2);
                pedido.TotalProductos = cantidad;
                pedido.Productos = productos;
                pedido.NombreProductos = nombre_productos;

                db.Pedidoes.Add(pedido);
                cc.RemoveRange(0, cc.Count());

                db.SaveChanges();

                return RedirectToAction("Index", "Pedidos");
            }   else
            {
                return RedirectToAction("carrito", "Carrito");
            }

        }
    }
}