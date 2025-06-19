using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OracleMES.Infrastructure.Entities;

namespace OracleMES.Infrastructure.Data;

public partial class MesDbContext : DbContext
{
    public MesDbContext()
    {
    }

    public MesDbContext(DbContextOptions<MesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Billofmaterial> Billofmaterials { get; set; }

    public virtual DbSet<Defect> Defects { get; set; }

    public virtual DbSet<Downtime> Downtimes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Machine> Machines { get; set; }

    public virtual DbSet<Materialconsumption> Materialconsumptions { get; set; }

    public virtual DbSet<Oeemetric> Oeemetrics { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Qualitycontrol> Qualitycontrols { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Workcenter> Workcenters { get; set; }

    public virtual DbSet<Workorder> Workorders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("Data Source=localhost:1521/XE;User Id=mes;Password=mes123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("MES")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Billofmaterial>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BILLOFMATERIALS");

            entity.Property(e => e.Bomid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("BOMID");
            entity.Property(e => e.Componentid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("COMPONENTID");
            entity.Property(e => e.Productid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PRODUCTID");
            entity.Property(e => e.Quantity)
                .HasColumnType("FLOAT")
                .HasColumnName("QUANTITY");
            entity.Property(e => e.Scrapfactor)
                .HasColumnType("FLOAT")
                .HasColumnName("SCRAPFACTOR");
        });

        modelBuilder.Entity<Defect>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DEFECTS");

            entity.Property(e => e.Actiontaken)
                .HasColumnType("CLOB")
                .HasColumnName("ACTIONTAKEN");
            entity.Property(e => e.Checkid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CHECKID");
            entity.Property(e => e.Defectid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("DEFECTID");
            entity.Property(e => e.Defecttype)
                .HasColumnType("CLOB")
                .HasColumnName("DEFECTTYPE");
            entity.Property(e => e.Location)
                .HasColumnType("CLOB")
                .HasColumnName("LOCATION");
            entity.Property(e => e.Quantity)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("QUANTITY");
            entity.Property(e => e.Rootcause)
                .HasColumnType("CLOB")
                .HasColumnName("ROOTCAUSE");
            entity.Property(e => e.Severity)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SEVERITY");
        });

        modelBuilder.Entity<Downtime>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DOWNTIMES");

            entity.Property(e => e.Category)
                .HasColumnType("CLOB")
                .HasColumnName("CATEGORY");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Downtimeid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("DOWNTIMEID");
            entity.Property(e => e.Duration)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("DURATION");
            entity.Property(e => e.Endtime)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ENDTIME");
            entity.Property(e => e.Machineid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MACHINEID");
            entity.Property(e => e.Orderid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ORDERID");
            entity.Property(e => e.Reason)
                .HasColumnType("CLOB")
                .HasColumnName("REASON");
            entity.Property(e => e.Reportedby)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("REPORTEDBY");
            entity.Property(e => e.Starttime)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("STARTTIME");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EMPLOYEES");

            entity.Property(e => e.Employeeid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("EMPLOYEEID");
            entity.Property(e => e.Hiredate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("HIREDATE");
            entity.Property(e => e.Hourlyrate)
                .HasColumnType("FLOAT")
                .HasColumnName("HOURLYRATE");
            entity.Property(e => e.Name)
                .HasColumnType("CLOB")
                .HasColumnName("NAME");
            entity.Property(e => e.Role).HasColumnType("CLOB");
            entity.Property(e => e.Shiftid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SHIFTID");
            entity.Property(e => e.Skills)
                .HasColumnType("CLOB")
                .HasColumnName("SKILLS");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("INVENTORY");

            entity.Property(e => e.Category)
                .HasColumnType("CLOB")
                .HasColumnName("CATEGORY");
            entity.Property(e => e.Cost)
                .HasColumnType("FLOAT")
                .HasColumnName("COST");
            entity.Property(e => e.Itemid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ITEMID");
            entity.Property(e => e.Lastreceiveddate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LASTRECEIVEDDATE");
            entity.Property(e => e.Leadtime)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("LEADTIME");
            entity.Property(e => e.Location)
                .HasColumnType("CLOB")
                .HasColumnName("LOCATION");
            entity.Property(e => e.Lotnumber)
                .HasColumnType("CLOB")
                .HasColumnName("LOTNUMBER");
            entity.Property(e => e.Name)
                .HasColumnType("CLOB")
                .HasColumnName("NAME");
            entity.Property(e => e.Quantity)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("QUANTITY");
            entity.Property(e => e.Reorderlevel)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("REORDERLEVEL");
            entity.Property(e => e.Supplierid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SUPPLIERID");
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MACHINES");

            entity.Property(e => e.Capacityuom)
                .HasColumnType("CLOB")
                .HasColumnName("CAPACITYUOM");
            entity.Property(e => e.Costperhour)
                .HasColumnType("FLOAT")
                .HasColumnName("COSTPERHOUR");
            entity.Property(e => e.Efficiencyfactor)
                .HasColumnType("FLOAT")
                .HasColumnName("EFFICIENCYFACTOR");
            entity.Property(e => e.Installationdate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("INSTALLATIONDATE");
            entity.Property(e => e.Lastmaintenancedate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LASTMAINTENANCEDATE");
            entity.Property(e => e.Machineid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MACHINEID");
            entity.Property(e => e.Maintenancefrequency)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MAINTENANCEFREQUENCY");
            entity.Property(e => e.Modelnumber)
                .HasColumnType("CLOB")
                .HasColumnName("MODELNUMBER");
            entity.Property(e => e.Name)
                .HasColumnType("CLOB")
                .HasColumnName("NAME");
            entity.Property(e => e.Nextmaintenancedate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NEXTMAINTENANCEDATE");
            entity.Property(e => e.Nominalcapacity)
                .HasColumnType("FLOAT")
                .HasColumnName("NOMINALCAPACITY");
            entity.Property(e => e.Productchangeovertime)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PRODUCTCHANGEOVERTIME");
            entity.Property(e => e.Setuptime)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SETUPTIME");
            entity.Property(e => e.Status)
                .HasColumnType("CLOB")
                .HasColumnName("STATUS");
            entity.Property(e => e.Type).HasColumnType("CLOB");
            entity.Property(e => e.Workcenterid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("WORKCENTERID");
        });

        modelBuilder.Entity<Materialconsumption>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("MATERIALCONSUMPTION");

            entity.Property(e => e.Actualquantity)
                .HasColumnType("FLOAT")
                .HasColumnName("ACTUALQUANTITY");
            entity.Property(e => e.Consumptiondate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CONSUMPTIONDATE");
            entity.Property(e => e.Consumptionid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CONSUMPTIONID");
            entity.Property(e => e.Itemid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ITEMID");
            entity.Property(e => e.Lotnumber)
                .HasColumnType("CLOB")
                .HasColumnName("LOTNUMBER");
            entity.Property(e => e.Orderid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ORDERID");
            entity.Property(e => e.Plannedquantity)
                .HasColumnType("FLOAT")
                .HasColumnName("PLANNEDQUANTITY");
            entity.Property(e => e.Variancepercent)
                .HasColumnType("FLOAT")
                .HasColumnName("VARIANCEPERCENT");
        });

        modelBuilder.Entity<Oeemetric>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OEEMETRICS");

            entity.Property(e => e.Actualproductiontime)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ACTUALPRODUCTIONTIME");
            entity.Property(e => e.Availability)
                .HasColumnType("FLOAT")
                .HasColumnName("AVAILABILITY");
            entity.Property(e => e.Date)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Downtime)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("DOWNTIME");
            entity.Property(e => e.Machineid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MACHINEID");
            entity.Property(e => e.Metricid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("METRICID");
            entity.Property(e => e.Oee)
                .HasColumnType("FLOAT")
                .HasColumnName("OEE");
            entity.Property(e => e.Performance)
                .HasColumnType("FLOAT")
                .HasColumnName("PERFORMANCE");
            entity.Property(e => e.Plannedproductiontime)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PLANNEDPRODUCTIONTIME");
            entity.Property(e => e.Quality)
                .HasColumnType("FLOAT")
                .HasColumnName("QUALITY");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("PRODUCTS");

            entity.Property(e => e.Category)
                .HasColumnType("CLOB")
                .HasColumnName("CATEGORY");
            entity.Property(e => e.Cost)
                .HasColumnType("FLOAT")
                .HasColumnName("COST");
            entity.Property(e => e.Description)
                .HasColumnType("CLOB")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Isactive)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ISACTIVE");
            entity.Property(e => e.Name)
                .HasColumnType("CLOB")
                .HasColumnName("NAME");
            entity.Property(e => e.Productid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PRODUCTID");
            entity.Property(e => e.Standardprocesstime)
                .HasColumnType("FLOAT")
                .HasColumnName("STANDARDPROCESSTIME");
        });

        modelBuilder.Entity<Qualitycontrol>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("QUALITYCONTROL");

            entity.Property(e => e.Checkid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CHECKID");
            entity.Property(e => e.Comments)
                .HasColumnType("CLOB")
                .HasColumnName("COMMENTS");
            entity.Property(e => e.Date)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Defectrate)
                .HasColumnType("FLOAT")
                .HasColumnName("DEFECTRATE");
            entity.Property(e => e.Inspectorid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("INSPECTORID");
            entity.Property(e => e.Orderid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ORDERID");
            entity.Property(e => e.Result).HasColumnType("CLOB");
            entity.Property(e => e.Reworkrate)
                .HasColumnType("FLOAT")
                .HasColumnName("REWORKRATE");
            entity.Property(e => e.Yieldrate)
                .HasColumnType("FLOAT")
                .HasColumnName("YIELDRATE");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SHIFTS");

            entity.Property(e => e.Capacity)
                .HasColumnType("FLOAT")
                .HasColumnName("CAPACITY");
            entity.Property(e => e.Endtime)
                .HasColumnType("CLOB")
                .HasColumnName("ENDTIME");
            entity.Property(e => e.Isweekend)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ISWEEKEND");
            entity.Property(e => e.Name)
                .HasColumnType("CLOB")
                .HasColumnName("NAME");
            entity.Property(e => e.Shiftid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SHIFTID");
            entity.Property(e => e.Starttime)
                .HasColumnType("CLOB")
                .HasColumnName("STARTTIME");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SUPPLIERS");

            entity.Property(e => e.Contactinfo)
                .HasColumnType("CLOB")
                .HasColumnName("CONTACTINFO");
            entity.Property(e => e.Leadtime)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("LEADTIME");
            entity.Property(e => e.Name)
                .HasColumnType("CLOB")
                .HasColumnName("NAME");
            entity.Property(e => e.Reliabilityscore)
                .HasColumnType("FLOAT")
                .HasColumnName("RELIABILITYSCORE");
            entity.Property(e => e.Supplierid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SUPPLIERID");
        });

        modelBuilder.Entity<Workcenter>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WORKCENTERS");

            entity.Property(e => e.Capacity)
                .HasColumnType("FLOAT")
                .HasColumnName("CAPACITY");
            entity.Property(e => e.Capacityuom)
                .HasColumnType("CLOB")
                .HasColumnName("CAPACITYUOM");
            entity.Property(e => e.Costperhour)
                .HasColumnType("FLOAT")
                .HasColumnName("COSTPERHOUR");
            entity.Property(e => e.Description)
                .HasColumnType("CLOB")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Isactive)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ISACTIVE");
            entity.Property(e => e.Location)
                .HasColumnType("CLOB")
                .HasColumnName("LOCATION");
            entity.Property(e => e.Name)
                .HasColumnType("CLOB")
                .HasColumnName("NAME");
            entity.Property(e => e.Workcenterid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("WORKCENTERID");
        });

        modelBuilder.Entity<Workorder>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WORKORDERS");

            entity.Property(e => e.Actualendtime)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ACTUALENDTIME");
            entity.Property(e => e.Actualproduction)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ACTUALPRODUCTION");
            entity.Property(e => e.Actualstarttime)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ACTUALSTARTTIME");
            entity.Property(e => e.Employeeid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("EMPLOYEEID");
            entity.Property(e => e.Leadtime)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("LEADTIME");
            entity.Property(e => e.Lotnumber)
                .HasColumnType("CLOB")
                .HasColumnName("LOTNUMBER");
            entity.Property(e => e.Machineid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MACHINEID");
            entity.Property(e => e.Orderid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ORDERID");
            entity.Property(e => e.Plannedendtime)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PLANNEDENDTIME");
            entity.Property(e => e.Plannedstarttime)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PLANNEDSTARTTIME");
            entity.Property(e => e.Priority)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PRIORITY");
            entity.Property(e => e.Productid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PRODUCTID");
            entity.Property(e => e.Quantity)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("QUANTITY");
            entity.Property(e => e.Scrap)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SCRAP");
            entity.Property(e => e.Setuptimeactual)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SETUPTIMEACTUAL");
            entity.Property(e => e.Status)
                .HasColumnType("CLOB")
                .HasColumnName("STATUS");
            entity.Property(e => e.Workcenterid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("WORKCENTERID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
