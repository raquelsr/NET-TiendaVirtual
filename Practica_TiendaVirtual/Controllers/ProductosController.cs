using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Practica_TiendaVirtual;
using Practica_TiendaVirtual.Models;

namespace Practica_TiendaVirtual.Controllers
{
    public class ProductosController : Controller
    {
        private TiendaVirtualEntities db = new TiendaVirtualEntities();

        //[Authorize(Users="admin@admin.com")]
        [Authorize]
        public ActionResult anadirCarrito(int id, CarritoCompra cc)
        {
            Producto p = db.Productos.Find(id);

            List<Producto> totalProductos = cc.FindAll(x => x.Id == id);
            if (totalProductos.Count()+1 <= p.Cantidad)
            {
                cc.Add(p);
                return RedirectToAction("Index", "Productos");
            }
            else
            {
                return RedirectToAction("Carrito", "Productos", p);
            }
        }

        [Authorize]
        public ActionResult carrito (CarritoCompra cc)
        {
            return View(cc);
        }

        [Authorize]
        public ActionResult eliminarProductoCarrito(int id, CarritoCompra cc)
        {
            Producto producto = cc.Find(x => x.Id == id);
            cc.Remove(producto);

            return RedirectToAction("Carrito", "Productos", cc);
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

            return RedirectToAction("Index","Pedidos");
        }


        // GET: Productos
        public ActionResult Index()
        {
            return View(db.Productos.ToList());
        }

        // GET: Productos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: Productos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion,Precio,Cantidad,Nombre_imagen,Url_imagen")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Productos.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(producto);
        }

        // GET: Productos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Descripcion,Precio,Cantidad,Nombre_imagen,Url_imagen")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        // GET: Productos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Productos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Productos.Find(id);
            db.Productos.Remove(producto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
