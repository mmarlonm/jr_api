using Microsoft.EntityFrameworkCore;
using jr_api.Models;
using System.Collections.Generic;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DbSets para las tablas
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }
    public DbSet<RolPermiso> RolPermisos { get; set; }
    public DbSet<UnidadDeNegocio> UnidadDeNegocio { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Vista> Vistas { get; set; }
    public DbSet<RolVista> RolVistas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Cotizaciones> Cotizaciones { get; set; }
    public DbSet<EstatusCotizacion> EstatusCotizacion { get; set; }
    public DbSet<EstatusProyecto> EstatusProyecto { get; set; }
    public DbSet<Prospecto> Prospectos { get; set; }
    public DbSet<SeguimientoProspecto> SeguimientoProspectos { get; set; }
    public DbSet<ProyectoArchivo> ProyectoArchivo { get; set; }
    public DbSet<ProyectoEstatusHistorial> ProyectoEstatusHistorial { get; set; }
    public DbSet<CotizacionesEstatusHistorial> CotizacionesEstatusHistorial { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<NotaProspecto> NotasProspecto { get; set; }

    //tablas venta
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<FormaDePago> FormasDePago { get; set; }
    // Método para configurar las relaciones entre las tablas
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔹 Configuración de la clave primaria en RolPermiso
        modelBuilder.Entity<RolPermiso>()
            .HasKey(rp => rp.RolPermisoId); // ✅ Usar RolPermisoId en lugar de clave compuesta

        // 🔹 Configuración de la relación de RolPermisos con las claves foráneas
        modelBuilder.Entity<RolPermiso>()
            .HasOne(rp => rp.Rol)
            .WithMany(r => r.RolPermisos) // ✅ Asegurar la relación bidireccional
            .HasForeignKey(rp => rp.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolPermiso>()
            .HasOne(rp => rp.Permiso)
            .WithMany(p => p.RolPermisos) // ✅ Relación corregida
            .HasForeignKey(rp => rp.PermisoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolPermiso>()
            .HasOne(rp => rp.Vista)
            .WithMany(v => v.RolPermisos) // ✅ Se corrigió la relación
            .HasForeignKey(rp => rp.VistaId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Configuración de la relación de RolVistas
        modelBuilder.Entity<RolVista>()
            .HasKey(rv => rv.RolVistaId); // ✅ Se establece la clave primaria correcta

        modelBuilder.Entity<RolVista>()
            .HasOne(rv => rv.Rol)
            .WithMany(r => r.RolVistas) // ✅ Relación corregida
            .HasForeignKey(rv => rv.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolVista>()
            .HasOne(rv => rv.Vista)
            .WithMany(v => v.RolVistas) // ✅ Se asegura la relación
            .HasForeignKey(rv => rv.VistaId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Configuración de la clave primaria compuesta para UsuarioRol
        modelBuilder.Entity<UsuarioRol>()
            .HasKey(ur => new { ur.UsuarioId, ur.RolId });

        // 🔹 Opcional: Habilitar el log de datos sensibles si sigues teniendo problemas
        // this.Database.SetCommandTimeout(300); // Si necesitas más tiempo de ejecución
    }
}