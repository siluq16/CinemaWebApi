using System;
using System.Collections.Generic;
using CinemaWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Data;

public partial class CinemaWebApiContext : DbContext
{
    public CinemaWebApiContext()
    {
    }

    public CinemaWebApiContext(DbContextOptions<CinemaWebApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AvailableSeat> AvailableSeats { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingPromotion> BookingPromotions { get; set; }

    public virtual DbSet<BookingSeat> BookingSeats { get; set; }

    public virtual DbSet<CastMember> CastMembers { get; set; }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<CinemaRevenue> CinemaRevenues { get; set; }

    public virtual DbSet<Director> Directors { get; set; }

    public virtual DbSet<FoodCategory> FoodCategories { get; set; }

    public virtual DbSet<FoodComboItem> FoodComboItems { get; set; }

    public virtual DbSet<FoodItem> FoodItems { get; set; }

    public virtual DbSet<FoodOrder> FoodOrders { get; set; }

    public virtual DbSet<FoodOrderItem> FoodOrderItems { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<MembershipCard> MembershipCards { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieCrew> MovieCrews { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OauthAccount> OauthAccounts { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PointTransaction> PointTransactions { get; set; }

    public virtual DbSet<PricingRule> PricingRules { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ScreeningRoom> ScreeningRooms { get; set; }

    public virtual DbSet<SeatLayout> SeatLayouts { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<UpcomingShowtime> UpcomingShowtimes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPromotionUsage> UserPromotionUsages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AvailableSeat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("available_seats");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.RowLabel)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("row_label");
            entity.Property(e => e.SeatId)
                .ValueGeneratedOnAdd()
                .HasColumnName("seat_id");
            entity.Property(e => e.SeatNumber).HasColumnName("seat_number");
            entity.Property(e => e.SeatType)
                .HasMaxLength(30)
                .HasColumnName("seat_type");
            entity.Property(e => e.XPosition).HasColumnName("x_position");
            entity.Property(e => e.YPosition).HasColumnName("y_position");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__bookings__3213E83FA9556B93");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.BookingCode, "UQ__bookings__FF29040F78E66EFC").IsUnique();

            entity.HasIndex(e => e.BookingCode, "idx_bookings_code");

            entity.HasIndex(e => e.ExpiresAt, "idx_bookings_expires").HasFilter("([status]='pending')");

            entity.HasIndex(e => e.ShowtimeId, "idx_bookings_showtime");

            entity.HasIndex(e => e.Status, "idx_bookings_status");

            entity.HasIndex(e => e.UserId, "idx_bookings_user");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.BookingCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("booking_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.FinalAmount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("final_amount");
            entity.Property(e => e.FoodAmount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("food_amount");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__showti__73852659");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__user_i__74794A92");
        });

        modelBuilder.Entity<BookingPromotion>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.PromotionId }).HasName("PK__booking___FF2830E7A30356B3");

            entity.ToTable("booking_promotions");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.AppliedDiscount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("applied_discount");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingPromotions)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__booking_p__booki__6FB49575");

            entity.HasOne(d => d.Promotion).WithMany(p => p.BookingPromotions)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking_p__promo__70A8B9AE");
        });

        modelBuilder.Entity<BookingSeat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__booking___3213E83F81F9F86E");

            entity.ToTable("booking_seats");

            entity.HasIndex(e => new { e.SeatId, e.BookingId }, "UQ__booking___95B3D7C6ED3AAA14").IsUnique();

            entity.HasIndex(e => e.BookingId, "idx_booking_seats_booking");

            entity.HasIndex(e => e.SeatId, "idx_booking_seats_seat");

            entity.HasIndex(e => e.SeatId, "idx_unique_seat_showtime")
                .IsUnique()
                .HasFilter("([status] IN ('held', 'confirmed'))");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("price");
            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("held")
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingSeats)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__booking_s__booki__719CDDE7");

            entity.HasOne(d => d.Seat).WithOne(p => p.BookingSeat)
                .HasForeignKey<BookingSeat>(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__booking_s__seat___72910220");
        });

        modelBuilder.Entity<CastMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cast_mem__3213E83FAE5E3A59");

            entity.ToTable("cast_members");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Nationality)
                .HasMaxLength(50)
                .HasColumnName("nationality");
            entity.Property(e => e.PhotoUrl).HasColumnName("photo_url");
        });

        modelBuilder.Entity<Cinema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cinemas__3213E83F220A2E66");

            entity.ToTable("cinemas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.District)
                .HasMaxLength(50)
                .HasColumnName("district");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(10, 7)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(10, 7)")
                .HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<CinemaRevenue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("cinema_revenue");

            entity.Property(e => e.CinemaId).HasColumnName("cinema_id");
            entity.Property(e => e.CinemaName)
                .HasMaxLength(150)
                .HasColumnName("cinema_name");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.FoodRevenue)
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("food_revenue");
            entity.Property(e => e.TicketRevenue)
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("ticket_revenue");
            entity.Property(e => e.TotalBookings).HasColumnName("total_bookings");
            entity.Property(e => e.TotalRevenue)
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("total_revenue");
        });

        modelBuilder.Entity<Director>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__director__3213E83F528F7A9E");

            entity.ToTable("directors");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Nationality)
                .HasMaxLength(50)
                .HasColumnName("nationality");
            entity.Property(e => e.PhotoUrl).HasColumnName("photo_url");
        });

        modelBuilder.Entity<FoodCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__food_cat__3213E83FBCF7BEF1");

            entity.ToTable("food_categories");

            entity.HasIndex(e => e.Slug, "UQ__food_cat__32DD1E4C4983FEE1").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__food_cat__72E12F1BD70D8185").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.IconUrl).HasColumnName("icon_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<FoodComboItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__food_com__3213E83F4B400AB0");

            entity.ToTable("food_combo_items");

            entity.HasIndex(e => new { e.ComboId, e.ItemId }, "UQ__food_com__DDD76A5FADB051DF").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Combo).WithMany(p => p.FoodComboItemCombos)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK__food_comb__combo__756D6ECB");

            entity.HasOne(d => d.Item).WithMany(p => p.FoodComboItemItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__food_comb__item___76619304");
        });

        modelBuilder.Entity<FoodItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__food_ite__3213E83F0915DE90");

            entity.ToTable("food_items");

            entity.HasIndex(e => e.CategoryId, "idx_food_items_category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("base_price");
            entity.Property(e => e.Calories).HasColumnName("calories");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .HasColumnName("is_available");
            entity.Property(e => e.IsCombo).HasColumnName("is_combo");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.FoodItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__food_item__categ__7755B73D");
        });

        modelBuilder.Entity<FoodOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__food_ord__3213E83FD076131A");

            entity.ToTable("food_orders");

            entity.HasIndex(e => e.BookingId, "idx_food_orders_booking");

            entity.HasIndex(e => e.Status, "idx_food_orders_status");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CinemaId).HasColumnName("cinema_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PickupCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("pickup_code");
            entity.Property(e => e.PickupCounter)
                .HasMaxLength(20)
                .HasColumnName("pickup_counter");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Booking).WithMany(p => p.FoodOrders)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__food_orde__booki__7A3223E8");

            entity.HasOne(d => d.Cinema).WithMany(p => p.FoodOrders)
                .HasForeignKey(d => d.CinemaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__food_orde__cinem__7B264821");
        });

        modelBuilder.Entity<FoodOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__food_ord__3213E83FBB49D1E5");

            entity.ToTable("food_order_items");

            entity.HasIndex(e => e.FoodOrderId, "idx_food_order_items_order");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.FoodItemId).HasColumnName("food_item_id");
            entity.Property(e => e.FoodOrderId).HasColumnName("food_order_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.SpecialRequest).HasColumnName("special_request");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("subtotal");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.FoodItem).WithMany(p => p.FoodOrderItems)
                .HasForeignKey(d => d.FoodItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__food_orde__food___793DFFAF");

            entity.HasOne(d => d.FoodOrder).WithMany(p => p.FoodOrderItems)
                .HasForeignKey(d => d.FoodOrderId)
                .HasConstraintName("FK__food_orde__food___7849DB76");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__genres__3213E83FD5E95376");

            entity.ToTable("genres");

            entity.HasIndex(e => e.Slug, "UQ__genres__32DD1E4C3EAD30AF").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__genres__72E12F1B45038A3F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("slug");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__holidays__3213E83FF8461345");

            entity.ToTable("holidays");

            entity.HasIndex(e => e.HolidayDate, "UQ__holidays__30A67916400ADB54").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.HolidayDate).HasColumnName("holiday_date");
        });

        modelBuilder.Entity<MembershipCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__membersh__3213E83F7A5466E2");

            entity.ToTable("membership_cards");

            entity.HasIndex(e => e.CardNumber, "UQ__membersh__1E6E0AF4770F7BA2").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__membersh__B9BE370EEFB0DC43").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("card_number");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.Tier)
                .HasMaxLength(20)
                .HasDefaultValue("silver")
                .HasColumnName("tier");
            entity.Property(e => e.TotalPoints).HasColumnName("total_points");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsedPoints).HasColumnName("used_points");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.MembershipCard)
                .HasForeignKey<MembershipCard>(d => d.UserId)
                .HasConstraintName("FK__membershi__user___7C1A6C5A");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__movies__3213E83FB36F14F1");

            entity.ToTable("movies");

            entity.HasIndex(e => e.IsFeatured, "idx_movies_featured").HasFilter("([is_featured]=(1))");

            entity.HasIndex(e => e.ReleaseDate, "idx_movies_release_date");

            entity.HasIndex(e => e.Status, "idx_movies_status");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.AgeRating).HasColumnName("age_rating");
            entity.Property(e => e.AvgRating)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(3, 1)")
                .HasColumnName("avg_rating");
            entity.Property(e => e.BannerUrl).HasColumnName("banner_url");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsFeatured).HasColumnName("is_featured");
            entity.Property(e => e.Language)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("vi")
                .HasColumnName("language");
            entity.Property(e => e.OriginalTitle)
                .HasMaxLength(255)
                .HasColumnName("original_title");
            entity.Property(e => e.PosterUrl).HasColumnName("poster_url");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.ReviewCount).HasColumnName("review_count");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("coming_soon")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TrailerUrl).HasColumnName("trailer_url");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Genres).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("FK__movie_gen__genre__7FEAFD3E"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .HasConstraintName("FK__movie_gen__movie__00DF2177"),
                    j =>
                    {
                        j.HasKey("MovieId", "GenreId").HasName("PK__movie_ge__B249DF9D35FA7B87");
                        j.ToTable("movie_genres");
                        j.IndexerProperty<Guid>("MovieId").HasColumnName("movie_id");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genre_id");
                    });
        });

        modelBuilder.Entity<MovieCrew>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__movie_cr__3213E83FE1ACEDB8");

            entity.ToTable("movie_crew");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.CastMemberId).HasColumnName("cast_member_id");
            entity.Property(e => e.CharacterName)
                .HasMaxLength(100)
                .HasColumnName("character_name");
            entity.Property(e => e.DirectorId).HasColumnName("director_id");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.RoleLabel)
                .HasMaxLength(50)
                .HasColumnName("role_label");

            entity.HasOne(d => d.CastMember).WithMany(p => p.MovieCrews)
                .HasForeignKey(d => d.CastMemberId)
                .HasConstraintName("FK__movie_cre__cast___7D0E9093");

            entity.HasOne(d => d.Director).WithMany(p => p.MovieCrews)
                .HasForeignKey(d => d.DirectorId)
                .HasConstraintName("FK__movie_cre__direc__7E02B4CC");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieCrews)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__movie_cre__movie__7EF6D905");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__notifica__3213E83FD28D800F");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.UserId, "idx_notifications_unread").HasFilter("([is_read]=(0))");

            entity.HasIndex(e => e.UserId, "idx_notifications_user");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Metadata).HasColumnName("metadata");
            entity.Property(e => e.ReadAt).HasColumnName("read_at");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__notificat__user___01D345B0");
        });

        modelBuilder.Entity<OauthAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__oauth_ac__3213E83F58061715");

            entity.ToTable("oauth_accounts");

            entity.HasIndex(e => new { e.Provider, e.ProviderUserId }, "UQ__oauth_ac__0341DC9040489686").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.AccessToken).HasColumnName("access_token");
            entity.Property(e => e.LinkedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("linked_at");
            entity.Property(e => e.Provider)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("provider");
            entity.Property(e => e.ProviderUserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("provider_user_id");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.TokenExpiresAt).HasColumnName("token_expires_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.OauthAccounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__oauth_acc__user___02C769E9");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__password__3213E83FC8F2DF00");

            entity.ToTable("password_reset_tokens");

            entity.HasIndex(e => e.TokenHash, "UQ__password__9F6BDB130D62F6B3").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("token_hash");
            entity.Property(e => e.UsedAt).HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__password___user___03BB8E22");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payments__3213E83FC23411C2");

            entity.ToTable("payments");

            entity.HasIndex(e => e.TransactionId, "UQ__payments__85C600AEFF278463").IsUnique();

            entity.HasIndex(e => e.BookingId, "idx_payments_booking");

            entity.HasIndex(e => e.Status, "idx_payments_status");

            entity.HasIndex(e => e.TransactionId, "idx_payments_transaction");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.GatewayResponse).HasColumnName("gateway_response");
            entity.Property(e => e.Method)
                .HasMaxLength(30)
                .HasColumnName("method");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.PointsUsed).HasColumnName("points_used");
            entity.Property(e => e.RefundAmount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("refund_amount");
            entity.Property(e => e.RefundedAt).HasColumnName("refunded_at");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("transaction_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payments__bookin__04AFB25B");
        });

        modelBuilder.Entity<PointTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__point_tr__3213E83F03B886BD");

            entity.ToTable("point_transactions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.MembershipId).HasColumnName("membership_id");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");

            entity.HasOne(d => d.Booking).WithMany(p => p.PointTransactions)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_point_booking");

            entity.HasOne(d => d.Membership).WithMany(p => p.PointTransactions)
                .HasForeignKey(d => d.MembershipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__point_tra__membe__05A3D694");
        });

        modelBuilder.Entity<PricingRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__pricing___3213E83FAD54055A");

            entity.ToTable("pricing_rules");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("base_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DayOfWeekFilter)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("day_of_week_filter");
            entity.Property(e => e.EndTimeFilter).HasColumnName("end_time_filter");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsHoliday).HasColumnName("is_holiday");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.RuleName)
                .HasMaxLength(100)
                .HasColumnName("rule_name");
            entity.Property(e => e.ScreenFormat)
                .HasMaxLength(30)
                .HasColumnName("screen_format");
            entity.Property(e => e.SeatType)
                .HasMaxLength(30)
                .HasColumnName("seat_type");
            entity.Property(e => e.StartTimeFilter).HasColumnName("start_time_filter");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__promotio__3213E83F39930024");

            entity.ToTable("promotions");

            entity.HasIndex(e => e.Code, "UQ__promotio__357D4CF96E7030BC").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.AppliesToFood).HasColumnName("applies_to_food");
            entity.Property(e => e.Code)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(30)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("discount_value");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxDiscountAmount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("max_discount_amount");
            entity.Property(e => e.MaxUses).HasColumnName("max_uses");
            entity.Property(e => e.MaxUsesPerUser)
                .HasDefaultValue(1)
                .HasColumnName("max_uses_per_user");
            entity.Property(e => e.MinOrderValue)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("min_order_value");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.UsedCount).HasColumnName("used_count");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__reviews__3213E83F384E0114");

            entity.ToTable("reviews");

            entity.HasIndex(e => new { e.UserId, e.MovieId }, "UQ__reviews__3182E87AB26017D7").IsUnique();

            entity.HasIndex(e => e.MovieId, "idx_reviews_movie");

            entity.HasIndex(e => e.UserId, "idx_reviews_user");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsVerified).HasColumnName("is_verified");
            entity.Property(e => e.IsVisible)
                .HasDefaultValue(true)
                .HasColumnName("is_visible");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__reviews__booking__078C1F06");

            entity.HasOne(d => d.Movie).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK__reviews__movie_i__0880433F");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__reviews__user_id__09746778");
        });

        modelBuilder.Entity<ScreeningRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__screenin__3213E83F0B013593");

            entity.ToTable("screening_rooms");

            entity.HasIndex(e => new { e.CinemaId, e.Name }, "UQ__screenin__E14C958805279C2C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CinemaId).HasColumnName("cinema_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.RoomType)
                .HasMaxLength(30)
                .HasDefaultValue("2D")
                .HasColumnName("room_type");
            entity.Property(e => e.TotalSeats).HasColumnName("total_seats");

            entity.HasOne(d => d.Cinema).WithMany(p => p.ScreeningRooms)
                .HasForeignKey(d => d.CinemaId)
                .HasConstraintName("FK__screening__cinem__0A688BB1");
        });

        modelBuilder.Entity<SeatLayout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__seat_lay__3213E83F6DA789A1");

            entity.ToTable("seat_layouts");

            entity.HasIndex(e => new { e.RoomId, e.RowLabel, e.SeatNumber }, "UQ__seat_lay__768E1F1CC8516D60").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.RowLabel)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("row_label");
            entity.Property(e => e.SeatNumber).HasColumnName("seat_number");
            entity.Property(e => e.SeatType)
                .HasMaxLength(30)
                .HasDefaultValue("standard")
                .HasColumnName("seat_type");
            entity.Property(e => e.XPosition).HasColumnName("x_position");
            entity.Property(e => e.YPosition).HasColumnName("y_position");

            entity.HasOne(d => d.Room).WithMany(p => p.SeatLayouts)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__seat_layo__room___0B5CAFEA");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__showtime__3213E83FC1EAD26F");

            entity.ToTable("showtimes");

            entity.HasIndex(e => e.StartTime, "idx_showtimes_active").HasFilter("([is_cancelled]=(0))");

            entity.HasIndex(e => e.MovieId, "idx_showtimes_movie");

            entity.HasIndex(e => e.RoomId, "idx_showtimes_room");

            entity.HasIndex(e => e.StartTime, "idx_showtimes_start_time");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IsCancelled).HasColumnName("is_cancelled");
            entity.Property(e => e.Language)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("language");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.ScreenFormat)
                .HasMaxLength(30)
                .HasDefaultValue("2D")
                .HasColumnName("screen_format");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.SubtitleType)
                .HasMaxLength(20)
                .HasDefaultValue("vi")
                .HasColumnName("subtitle_type");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__showtimes__movie__0C50D423");

            entity.HasOne(d => d.Room).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__showtimes__room___0D44F85C");
        });

        modelBuilder.Entity<UpcomingShowtime>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("upcoming_showtimes");

            entity.Property(e => e.AgeRating).HasColumnName("age_rating");
            entity.Property(e => e.CinemaId).HasColumnName("cinema_id");
            entity.Property(e => e.CinemaName)
                .HasMaxLength(150)
                .HasColumnName("cinema_name");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.Language)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("language");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.PosterUrl).HasColumnName("poster_url");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .HasColumnName("room_name");
            entity.Property(e => e.RoomType)
                .HasMaxLength(30)
                .HasColumnName("room_type");
            entity.Property(e => e.ScreenFormat)
                .HasMaxLength(30)
                .HasColumnName("screen_format");
            entity.Property(e => e.SeatsTaken).HasColumnName("seats_taken");
            entity.Property(e => e.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.SubtitleType)
                .HasMaxLength(20)
                .HasColumnName("subtitle_type");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TotalSeats).HasColumnName("total_seats");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F5BB739D9");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E61647BAE50C0").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__users__B43B145F720A4B7D").IsUnique();

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Phone, "idx_users_phone");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsVerified).HasColumnName("is_verified");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("customer")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserPromotionUsage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_pro__3213E83FA5C001F9");

            entity.ToTable("user_promotion_usage");

            entity.HasIndex(e => new { e.UserId, e.PromotionId, e.BookingId }, "UQ__user_pro__AB2841FD113A2B41").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newsequentialid())")
                .HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.UsedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.UserPromotionUsages)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_prom__booki__0E391C95");

            entity.HasOne(d => d.Promotion).WithMany(p => p.UserPromotionUsages)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_prom__promo__0F2D40CE");

            entity.HasOne(d => d.User).WithMany(p => p.UserPromotionUsages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_prom__user___10216507");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
