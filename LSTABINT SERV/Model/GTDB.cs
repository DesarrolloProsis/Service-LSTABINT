namespace LSTABINT_SERV.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GTDB : DbContext
    {
        public GTDB()
            : base("name=GTDB")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AmountConfigurations> AmountConfigurations { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Clientes> Clientes { get; set; }
        public virtual DbSet<CortesCajeroes> CortesCajeroes { get; set; }
        public virtual DbSet<CuentasTelepeajes> CuentasTelepeajes { get; set; }
        public virtual DbSet<Historico> Historico { get; set; }
        public virtual DbSet<HistoricoListas> HistoricoListas { get; set; }
        public virtual DbSet<ListaNegra> ListaNegra { get; set; }
        public virtual DbSet<MontosRecargables> MontosRecargables { get; set; }
        public virtual DbSet<OperacionesCajeroes> OperacionesCajeroes { get; set; }
        public virtual DbSet<OperacionesSerBIpagos> OperacionesSerBIpagos { get; set; }
        public virtual DbSet<Parametrizables> Parametrizables { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }
        public virtual DbSet<v_InfoCliente> v_InfoCliente { get; set; }
        public virtual DbSet<v_Recargas> v_Recargas { get; set; }
        public virtual DbSet<v_RecargasMixta> v_RecargasMixta { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Clientes>()
                .HasMany(e => e.CuentasTelepeajes)
                .WithRequired(e => e.Clientes)
                .HasForeignKey(e => e.ClienteId);

            modelBuilder.Entity<CortesCajeroes>()
                .HasMany(e => e.OperacionesCajeroes)
                .WithRequired(e => e.CortesCajeroes)
                .HasForeignKey(e => e.CorteId);

            modelBuilder.Entity<CuentasTelepeajes>()
                .HasMany(e => e.Tags)
                .WithRequired(e => e.CuentasTelepeajes)
                .HasForeignKey(e => e.CuentaId);

            modelBuilder.Entity<v_RecargasMixta>()
                .Property(e => e.Origen)
                .IsUnicode(false);
        }
    }
}
