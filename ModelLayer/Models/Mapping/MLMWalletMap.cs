using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using ModelLayer.Models;

namespace ModelLayer.Models.Mapping
{
    public class MLMWalletMap : EntityTypeConfiguration<MLMWallet>
    {
        public MLMWalletMap()
        {
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.UserLoginID);

            this.Property(t => t.Points);

            this.Property(t => t.Amount);

            this.Property(t => t.IsMLMUser);

            this.Property(t => t.LastWalletTransactionID);

            this.Property(t => t.IsActive);           
            

            // Table & Column Mappings
            this.ToTable("MLMWallet");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserLoginID).HasColumnName("UserLoginID");
            this.Property(t => t.Points).HasColumnName("Points");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.IsMLMUser).HasColumnName("IsMLMUser");
            this.Property(t => t.LastWalletTransactionID).HasColumnName("LastWalletTransactionID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");                  
           
        }
    }
}
