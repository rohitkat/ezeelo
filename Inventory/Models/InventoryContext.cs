using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class InventoryContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<Inventory.Models.InventoryContext>());

        public DbSet<ModelLayer.Models.PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<ModelLayer.Models.Invoice> Invoices { get; set; }

        public DbSet<ModelLayer.Models.WarehouseStock> WarehouseStocks { get; set; }
    }
}