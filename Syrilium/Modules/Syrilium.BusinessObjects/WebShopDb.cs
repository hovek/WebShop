using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.ModelConfiguration;
using Syrilium.Modules.BusinessObjects.Item;
using System.Transactions;
using Syrilium.Interfaces.BusinessObjectsInterface;
using System.Collections.ObjectModel;
using Syrilium.Interfaces.BusinessObjectsInterface.Item;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syrilium.Modules.BusinessObjects
{
    public delegate void BeforeSaveEventHandler<T>(T param, out bool useTransaction);
    public delegate void EventHandler<T>(T param);

    public class WebShopDb : DbContext
    {
        #region Item
        public DbSet<Item.Item> Item { get; set; }
        public DbSet<ItemDataString> ItemDataString { get; set; }
        public DbSet<ItemDefinition> ItemDefinition { get; set; }
        public DbSet<ItemDefinitionDataString> ItemDefinitionDataString { get; set; }
        #endregion
        public DbSet<Partner> Partner { get; set; }
        public DbSet<Page> Page { get; set; }
        public DbSet<PageTranslation> PageTranslation { get; set; }
        public DbSet<PageLocation> PageLocation { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Banner> Banner { get; set; }
        public DbSet<BannerLocation> BannerLocation { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<PermissionGroup> PermissionGroup { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscriber { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscription { get; set; }
        public DbSet<NewsletterMail> NewsletterMail { get; set; }
        public DbSet<Translation> Translation { get; set; }
        public DbSet<UserInquiry> UserInquiry { get; set; }
        public DbSet<County> County { get; set; }
        public DbSet<DistrictCity> DistrictCity { get; set; }
        public DbSet<NewsletterMailingList> NewsletterMailingList { get; set; }
        public DbSet<NewsletterMailingListSubscriber> NewsletterMailingListSubscriber { get; set; }
        public DbSet<SearchInquiry> SearchInquiry { get; set; }
        public DbSet<ImageFormat> ImageFormat { get; set; }
        public DbSet<ItemChangeHistory> ItemChangeHistory { get; set; }
        public DbSet<ProductGrade> ProductGrade { get; set; }

        //public event BeforeSaveEventHandler<WebShopDb> BeforeSave;
        //public event EventHandler<WebShopDb> BeforeSaveInTransaction;
        //public event EventHandler<WebShopDb> AfterSaveInTransaction;
        //public event EventHandler<WebShopDb> AfterSave;

        /// <summary>
        /// Returns new instance.
        /// </summary>
        public static WebShopDb I
        {
            get
            {
                return new WebShopDb();
            }
        }

        public WebShopDb()
        {
        }

        //public override int SaveChanges()
        //{
        //	bool useTransaction = false;
        //	if (BeforeSave != null)
        //	{
        //		BeforeSave(this, out useTransaction);
        //	}

        //	int rez;
        //	if (useTransaction)
        //	{
        //		using (var scope = new TransactionScope(TransactionScopeOption.Required,
        //									new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead }))
        //		{
        //			if (BeforeSaveInTransaction != null)
        //			{
        //				BeforeSaveInTransaction(this);
        //			}
        //			rez = base.SaveChanges();
        //			if (AfterSaveInTransaction != null)
        //			{
        //				AfterSaveInTransaction(this);
        //			}
        //			scope.Complete();
        //		}
        //	}
        //	else
        //	{
        //		rez = base.SaveChanges();
        //	}

        //	if (AfterSave != null)
        //	{
        //		AfterSave(this);
        //	}

        //	return rez;
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new BannerLocationConfig());
            modelBuilder.Configurations.Add(new ConfigConfig());
            modelBuilder.Configurations.Add(new LanguageConfig());
            modelBuilder.Configurations.Add(new PermissionConfig());
            modelBuilder.Configurations.Add(new PermissionGroupConfig());
            modelBuilder.Configurations.Add(new LoginConfig());
            modelBuilder.Configurations.Add(new UserConfig());
            modelBuilder.Configurations.Add(new GenderConfig());
            modelBuilder.Configurations.Add(new PageConfig());
            modelBuilder.Configurations.Add(new PageLocationConfig());
            modelBuilder.Configurations.Add(new NewsletterConfig());
            modelBuilder.Configurations.Add(new NewsletterSubscriptionConfig());
            modelBuilder.Configurations.Add(new UserInquiryConfig());
            modelBuilder.Configurations.Add(new PartnerConfig());
            modelBuilder.Configurations.Add(new ImageFormatConfig());
            modelBuilder.Configurations.Add(new ProductGradeConfig());

            #region Item
            modelBuilder.Configurations.Add(new ItemConfig());
            modelBuilder.Configurations.Add(new ItemDataDecimalConfig());
            modelBuilder.Configurations.Add(new ItemDataStringConfig());
            modelBuilder.Configurations.Add(new ItemDataIntConfig());
            modelBuilder.Configurations.Add(new ItemDataDateTimeConfig());

            modelBuilder.Configurations.Add(new ItemDefinitionConfig());
            modelBuilder.Configurations.Add(new ItemDefinitionDataDecimalConfig());
            modelBuilder.Configurations.Add(new ItemDefinitionDataStringConfig());
            modelBuilder.Configurations.Add(new ItemDefinitionDataIntConfig());
            modelBuilder.Configurations.Add(new ItemDefinitionDataDateTimeConfig());

            modelBuilder.Configurations.Add(new ItemChangeHistoryConfig());
            #endregion
        }
    }

    #region Configurations
    public class ImageFormatConfig : EntityTypeConfiguration<ImageFormat>
    {
        public ImageFormatConfig()
        {
            Property(p => p.Name).HasColumnType("varchar").HasMaxLength(50);
        }
    }

    public class BannerLocationConfig : EntityTypeConfiguration<BannerLocation>
    {
        public BannerLocationConfig()
        {
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasMany(p => p.Translation).WithRequired().HasForeignKey(p => p.ParentId);
            HasMany(p => p.Banners).WithRequired(p => p.BannerLocation).HasForeignKey(p => p.BannerLocationId);
        }
    }
    public class PageConfig : EntityTypeConfiguration<Page>
    {
        public PageConfig()
        {
            HasMany(p => p.Translation).WithRequired().HasForeignKey(p => p.ParentId);
        }
    }
    public class PageLocationConfig : EntityTypeConfiguration<PageLocation>
    {
        public PageLocationConfig()
        {
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasMany(p => p.Page).WithRequired(p => p.PageLocation).HasForeignKey(p => p.PagesLocationId);
            HasMany(p => p.Translation).WithRequired().HasForeignKey(p => p.ParentId);
        }
    }
    public class ConfigConfig : EntityTypeConfiguration<Config>
    {
        public ConfigConfig()
        {
            Property(p => p.Name).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.StringValue).HasColumnType("varchar(max)");
            Property(p => p.DecimalValue).HasPrecision(20, 6);
        }
    }



    public class LanguageConfig : EntityTypeConfiguration<Language>
    {
        public LanguageConfig()
        {
            Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }

    public class PermissionConfig : EntityTypeConfiguration<Permission>
    {
        public PermissionConfig()
        {
            HasMany(p => p.Groups).WithMany(set => set.Permissions).Map(m => m
            .MapLeftKey("PermissionId")
            .MapRightKey("PermissionGroupId")
            .ToTable("PermissionGroupLink")
            );
        }
    }

    public class PermissionGroupConfig : EntityTypeConfiguration<PermissionGroup>
    {
        public PermissionGroupConfig()
        {
            HasMany(p => p.Users).WithMany(p => p.PermissionGroups).Map(m => m
                .MapLeftKey("PermissionGroupId")
                .MapRightKey("UserId")
                .ToTable("PermissionGroupUserLink")
            );
            HasMany(p => p.Partners).WithMany(p => p.PermissionGroups).Map(m => m
                .MapLeftKey("PermissionGroupId")
                .MapRightKey("PartnerId")
                .ToTable("PermissionGroupPartnerLink")
            );
            HasMany(p => p.Logins).WithMany(p => p.PermissionGroups).Map(m => m
                .MapLeftKey("PermissionGroupId")
                .MapRightKey("LoginId")
                .ToTable("PermissionGroupLoginLink")
            );
        }
    }

    public class LoginConfig : EntityTypeConfiguration<Login>
    {
        public LoginConfig()
        {
            Property(p => p.MailConfirmationCode).HasColumnType("varchar").HasMaxLength(50);
            HasMany(p => p.Partners).WithOptional(p => p.Login).HasForeignKey(p => p.LoginId);
            HasMany(p => p.Users).WithOptional(p => p.Login).HasForeignKey(p => p.LoginId);
        }
    }

    public class UserConfig : EntityTypeConfiguration<User>
    {
        public UserConfig()
        {
            HasOptional(p => p.Gender).WithMany().HasForeignKey(p => p.GenderId);
            Property(p => p.Name).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Surname).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Email).HasColumnType("varchar").HasMaxLength(100);
            Property(p => p.Phone).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Mobile).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.City).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Address).HasColumnType("varchar").HasMaxLength(100);
        }
    }

    public class GenderConfig : EntityTypeConfiguration<Gender>
    {
        public GenderConfig()
        {
            HasMany(p => p.Translation).WithRequired().HasForeignKey(p => p.ParentId);
        }
    }

    public class NewsletterConfig : EntityTypeConfiguration<NewsletterSubscriber>
    {
        public NewsletterConfig()
        {
            Property(p => p.Email).HasColumnType("varchar").HasMaxLength(100);
            HasOptional(p => p.User).WithMany().HasForeignKey(p => p.UserId);
            HasOptional(p => p.Partner).WithMany().HasForeignKey(p => p.PartnerId);
        }
    }

    public class NewsletterSubscriptionConfig : EntityTypeConfiguration<NewsletterSubscription>
    {
        public NewsletterSubscriptionConfig()
        {
            HasRequired(p => p.NewsletterSubscriber).WithMany(p => p.Subscriptions).HasForeignKey(p => p.NewsletterSubscriberId);
            Property(p => p.DateOfEntry).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }

    public class PartnerConfig : EntityTypeConfiguration<Partner>
    {
        public PartnerConfig()
        {
            Property(p => p.Name).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Logo).HasColumnType("varchar").HasMaxLength(500);
            Property(p => p.About).HasColumnType("varchar(max)");
            Property(p => p.WorkDescription).HasColumnType("varchar(max)");
            Property(p => p.Services).HasColumnType("varchar(max)");
            Property(p => p.Address).HasColumnType("varchar").HasMaxLength(100);
            Property(p => p.Phone).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Email).HasColumnType("varchar").HasMaxLength(100);
            Property(p => p.URL).HasColumnType("varchar").HasMaxLength(256);
            Property(p => p.Fax).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.City).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.PostalCode).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Background).HasColumnType("varchar").HasMaxLength(256);
            Property(p => p.HtmlBackground).HasColumnType("varchar(max)");
        }
    }

    public class UserInquiryConfig : EntityTypeConfiguration<UserInquiry>
    {
        public UserInquiryConfig()
        {
            Property(p => p.Name).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.LastName).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Email).HasColumnType("varchar").HasMaxLength(100);
            Property(p => p.Phone).HasColumnType("varchar").HasMaxLength(50);
            Property(p => p.Message).HasColumnType("varchar(max)");
            Property(p => p.Title).HasColumnType("varchar(max)");
        }
    }

    public class ProductGradeConfig : EntityTypeConfiguration<ProductGrade>
    {
        public ProductGradeConfig()
        {
        }
    }

    #region Item
    public class ItemChangeHistoryConfig : EntityTypeConfiguration<ItemChangeHistory>
    {
        public ItemChangeHistoryConfig()
        {
            Property(p => p.DateOfEntry).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
        }
    }

    public class ItemConfig : EntityTypeConfiguration<Item.Item>
    {
        public ItemConfig()
        {
            Ignore(p => p.Parent);
            Ignore(p => p.Children);
            Ignore(p => p.IntData);
            Ignore(p => p.StringData);
            Ignore(p => p.DecimalData);
            Ignore(p => p.DateTimeData);
            HasMany(p => p.EFChildren).WithOptional(p => p.EFParent).HasForeignKey(p => p.ParentId);
        }
    }

    public class ItemDataDecimalConfig : EntityTypeConfiguration<ItemDataDecimal>
    {
        public ItemDataDecimalConfig()
        {
            Property(p => p.Value).HasPrecision(20, 6);
        }
    }

    public class ItemDataStringConfig : EntityTypeConfiguration<ItemDataString>
    {
        public ItemDataStringConfig()
        {
            Property(p => p.Value).HasColumnType("varchar").HasMaxLength(900);
        }
    }

    public class ItemDataIntConfig : EntityTypeConfiguration<ItemDataInt>
    {
        public ItemDataIntConfig()
        {
        }
    }

    public class ItemDataDateTimeConfig : EntityTypeConfiguration<ItemDataDateTime>
    {
        public ItemDataDateTimeConfig()
        {
        }
    }

    public class ItemDefinitionConfig : EntityTypeConfiguration<ItemDefinition>
    {
        public ItemDefinitionConfig()
        {
            Ignore(p => p.Parent);
            Ignore(p => p.Children);
            Ignore(p => p.IntData);
            Ignore(p => p.StringData);
            Ignore(p => p.DecimalData);
            Ignore(p => p.DateTimeData);
            HasMany(p => p.EFChildren).WithOptional(p => p.EFParent).HasForeignKey(p => p.ParentId);
        }
    }

    public class ItemDefinitionDataDecimalConfig : EntityTypeConfiguration<ItemDefinitionDataDecimal>
    {
        public ItemDefinitionDataDecimalConfig()
        {
            Ignore(p => p.ItemId);
            Property(p => p.Value).HasPrecision(20, 6);
        }
    }

    public class ItemDefinitionDataStringConfig : EntityTypeConfiguration<ItemDefinitionDataString>
    {
        public ItemDefinitionDataStringConfig()
        {
            Ignore(p => p.ItemId);
            Property(p => p.Value).HasColumnType("varchar").HasMaxLength(900);
        }
    }

    public class ItemDefinitionDataIntConfig : EntityTypeConfiguration<ItemDefinitionDataInt>
    {
        public ItemDefinitionDataIntConfig()
        {
            Ignore(p => p.ItemId);
        }
    }

    public class ItemDefinitionDataDateTimeConfig : EntityTypeConfiguration<ItemDefinitionDataDateTime>
    {
        public ItemDefinitionDataDateTimeConfig()
        {
            Ignore(p => p.ItemId);
        }
    }
    #endregion
    #endregion
}
